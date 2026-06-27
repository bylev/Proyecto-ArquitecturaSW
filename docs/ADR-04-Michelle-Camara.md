# ADR-04: Patrones de diseño GOF (Decorator y Factory Method)

| Campo  | Valor |
|--------|-------|
| Autor  | Michelle Cámara |
| Fecha  | 26/06/2026 |
| Estado | `Propuesto` |

---

## Contexto

**TransGGP** es un sistema web de gestión de servicios de transporte de carga desarrollado para un emprendimiento familiar que opera un tráiler. Actualmente, el negocio registra todos sus viajes en un archivo *Excel* con macros, lo que genera tres problemas principales: solo una persona puede usarlo a la vez, no es accesible desde dispositivos móviles y existe riesgo de pérdida de información.

En los ADR anteriores se definió el stack inicial (MVC), las vistas arquitectónicas, la **Arquitectura Hexagonal** y la exposición mediante **API REST + Swagger**. Sobre esa base ya se construyeron los módulos de **Clientes, Operadores, Unidades y Servicios** (CRUD en MVC y API), y se normalizó la base de datos con **llaves foráneas** que relacionan los servicios con sus catálogos.

Con el sistema funcionando aparecieron dos necesidades transversales que no encajaban en una sola capa: por un lado, **registrar (loguear)** cada operación que toca la base de datos para auditar y depurar, sin ensuciar la lógica del repositorio; y por otro, **generar reportes** de los datos en distintos formatos (texto, CSV y, a futuro, PDF) sin que el controlador conozca los detalles de cada formato. Además, existe el requer**patrones dediseño GOF**.

Este ADR documenta la decisión de incorporar dos patrones GOF que aprovechan el desacoplamiento que ya ofrece la arquitectura hex

---

## Decisión

Se adoptan **dos patrones de diseño GOF**, uno de cada categoría principal:

* **Decorator** (patrón *estructural*) para agregar *logging* a la capa de persistencia. Se implementa la clase `ClienteRepositoryLoggingDecorator`, que implementa la misma interfaz `IClienteRepository` y **envuelve** al repositorio real (`ClienteRepository`), añadiendo registros antedespués de cada positoriooriginal. Se activa desde la **inyección de dependencias**: cuando alguien pide un `IClienteRepository`, el contenedor entrega el decorador, que internamente delega en el repositorio real

* **Factory Method** (patrón *creacional*) para la generación de reportes. Se define un *creator* abstracto `ReporteCreator` que declara el método fábrica `CrearReporte()`, y dos *creators* concretos (`ReporteTextoCreator` y `ReporteCsvCreator`) que deciden qué **producto** fabricar (`ReporteTexto` o `ReporteCsv`, ambos implementan `IReporte`). El controlador solo elige el *creator* según el formato pedido y solicito se arma cadauno.

---

## ¿Por qué?

La característica concreta que resuelve cada problema es el **desacoplamiento del comportamiento respecto del código que ya existe**.

El **Decorator** se eligió porque la arquitectura hexagonal ya define el puerto `IClienteRepository`. Eso permite envolver la implementacióle la mismainterfaz y le agrega logging, **sin tocar ni una línea** del
repositorio origede activar odesactivar solo cambiando el registro en la inyección de dependencias, cumpliendo el principio *Open/Closed*: el repositorio queda abierto a extensión pero cerrado a modificación.

El **Factory Method** se eligió porque los reportes comparten una misma operación (generarse a partir de una lista de clientes) pero cambian en el formato de salida. Encapsular la creación de cada formato en un *creator* concreto permite agregar un nuevo formato —por ejemplo, PDF— creando solo una subclase nueva, **sin modificar** el controlador ni los reportes existentes. Esto evita que el controlador acumule condicionales (`if`) por cada formato.

### Alternativas consideradas

| Alternativa | Por qué la descarté |
|-------------|---------------------|
| **Logging directo dentro del repositorio** | Mezcla dos responsabilidades (acceder a datos y registrar), viola *Single Responsibility* y obliga a modificar el repositorio cada vez que cambie la forma de loguear. |
| **Programación orientada a aspectos (AOP)** | Resuelve el *cross-cutting concern*, pero agrega una librería y
complejidad (int para el alcancedel proyecto y una desarrolladora principiante. |
| **Patrón Strategy para reportes** | Strategy intercambia algoritmos, pero aquí el problema es *crear* el objeto correcto según el formato, no intercambiar un comportamiento. Factory Method modela mejor la creación de productos. |
| **Condicionales (if/switch) en el controlador** | Funciona al inicio, pero el controlador crece con un `if` por cada formato nuevo y rompe *Open/Closed*. |
| **Singleton para el servicio de reportes** | No aporta nada: el contenedor de dependencias de ASP.NET ya gestiona el ciclo de vida de los servicios. |

---

## Consecuencias

**✅ Lo que gano:**

- **Técnica**: El logging se agrega y se quita sin modificar el repositorio, solo cambiando el registro de dependencias. Los reportes admiten nuevos formatos creando una subclase, sin tocar el controlador. Ambos patrones refuerzan *Open/Closed*.
- **Proceso**: Cada patrón vive en una clase aislada (el decorador y los *creators*), así se entiende, explica y prueba de forma independiente, lo que facilita el mantenimiento.
- **Negocio**: La auditoría de operaciones y los reportes en varios formatos aportan valor directo al dueño para analizar
la operación, y  sin reescribirel núcleo.

**⚠️ Lo que sacrifico o asumo:**

- **Técnica**: A clases eindirección. Para alguien no familiarizado con GOF, seguir el flujo requiere entender primero el patrón. Es complejidad asumida a cambio
- **Deuda o Riesde logging solose aplica al repositorio de Clientes; auditar también Operadores, Unidades y Servicios requeriría sus propios decoradores. Igualmente, el Factory Method de reportes está implementado parServicios en unafase posterior. Es una deuda de cobertura asumida para el alcance actual.

---

## Estructura de carpetas

```text
TransGGP.sln
├── TransGGP.Application/
│   └── Reports/                         ← PATRÓN FACTORY METHOD
│       ├── IReporte.cs                  (producto abstracto + ReporteTexto, ReporteCsv)
│       └── ReporteCreator.cs            (creator abstracto + ReporteTextoCrea
│
├── TransGGP.Infrastructure/
│   └── Decorators/                      ← PATRÓN DECORATOR
│       └── ClienteRepositoryLoggingDecorator.cs
│
└── TransGGP.Web/
    ├── Program.cs                       (registra el decorador en la inyección de dependencias)
    └── Controllers/
        └── ClientesController.cs        (acción Reporte: elige el creator según el formato)             
```

## Cláusula de IA

Se utilizó una herramienta de inteligencia artificial como apoyo para implementar los dos patrones GOF en el código existente, para redactar y organizar este ADR. La comprensión de los patrones Decorator y Factory Method se reforzó con las diapositivas del profesor y la documentación de referencia de patrones de diseño.