# ADR-04: Migración a Arquitectura Hexagonal (Puertos y Adaptadores)

| Campo  | Valor |
|--------|-------|
| Autor  | Michelle Cámara González |
| Fecha  | 18/06/2026 |
| Estado | `Aceptado` |

---

## Contexto

**TransGGP** es un sistema web de gestión de servicios de transporte de carga para un emprendimiento familiar que opera un tráiler. En el **ADR-01** se eligió el patrón **MVC** con ASP.NET Core como punto de partida y, en ese momento, la **Arquitectura Hexagonal** se descartó por considerarse *demasiado compleja para una desarrolladora principiante*, dejándola como una evolución futura.

Conforme el proyecto avanzó, esa evolución dejó de ser opcional. Aparecieron dos necesidades concretas que el MVC en un solo proyecto no resolvía bien:

- Se requería exponer el sistema **también como API REST** (para una futura app móvil y para el requerimiento académico), es decir, **dos formas de entrada distintas** —web MVC y API— que debían compartir exactamente la misma lógica de negocio sin duplicarla.
- La lógica del negocio (cómo se registran clientes, operadores, unidades y servicios) estaba mezclada con detalles de infraestructura (Entity Framework Core, MySQL), lo que dificultaba probar el sistema y volvía riesgoso cualquier cambio en la base de datos.

Mantener todo en un solo proyecto MVC implicaba que, al agregar la API, se duplicara código o que la lógica quedara amarrada al framework web. Por eso se decide **reorganizar el sistema hacia una Arquitectura Hexagonal (Puertos y Adaptadores)**, revirtiendo de forma consciente la postura del ADR-01 una vez que se entendió que el costo de aprendizaje se justificaba por el beneficio.

Las restricciones que influyeron en esta decisión son: tiempo de desarrollo de tres meses, una desarrolladora principiante, el stack ya elegido (.NET + Entity Framework Core + MySQL) y la necesidad de soportar varios adaptadores de entrada (web y API) sin reescribir la lógica.

---

## Decisión

Se **migra la solución a una Arquitectura Hexagonal (Ports and Adapters)**, separando el sistema en proyectos independientes según su responsabilidad, donde el **dominio** queda en el centro y todo lo externo (base de datos, web, API) se conecta a él mediante **puertos** (interfaces) y **adaptadores** (implementaciones).

| Proyecto | Rol en la arquitectura | Responsabilidad |
|----------|------------------------|-----------------|
| `TransGGP.Domain` | Núcleo | Entidades del negocio (Cliente, Operador, Unidad, Servicio…). Sin dependencias externas. |
| `TransGGP.Application` | Núcleo | Casos de uso (*Services*) y **puertos** (interfaces de repositorio, ej. `IClienteRepository`). |
| `TransGGP.Infrastructure` | Adaptador de salida | Persistencia con Entity Framework Core + MySQL (Pomelo); implementa los puertos de repositorio. |
| `TransGGP.Web` | Adaptador de entrada | Interfaz MVC con vistas Razor + Bootstrap. |
| `TransGGP.API` | Adaptador de entrada | API REST documentada con Swagger. |

La regla central es la **dirección de las dependencias**: `Web` y `API` dependen de `Application`, y `Application` define interfaces que `Infrastructure` implementa. El dominio **no depende de nadie**; son los adaptadores los que dependen del núcleo, nunca al revés.

## ¿Por qué?

La característica concreta que resuelve el problema de TransGGP es que la Arquitectura Hexagonal permite **agregar nuevos adaptadores de entrada sin tocar la lógica de negocio**. Gracias a esto, la interfaz web MVC y la API REST son simplemente dos adaptadores que consumen los **mismos** casos de uso de `Application`, eliminando la duplicación de código que habría provocado un MVC monolítico.

Además, al definir los repositorios como **puertos** (interfaces) en `Application` e implementarlos en `Infrastructure`, la lógica de negocio queda aislada de Entity Framework Core y MySQL. Esto facilita probar los casos de uso y, llegado el caso, cambiar de base de datos o de ORM sin reescribir el dominio. Este mismo desacoplamiento es el que después permitió aplicar el patrón **Decorator** sobre el puerto `IClienteRepository` (ver ADR-05).

Aunque el ADR-01 había descartado esta arquitectura por su complejidad, la práctica demostró que el costo de aprendizaje era manejable y que el beneficio —reutilizar la lógica entre web y API— era indispensable para el rumbo del proyecto.

### Alternativas consideradas

| Alternativa | Por qué la descarté |
|-------------|---------------------|
| **Mantener MVC en un solo proyecto** | Era lo más rápido al inicio, pero al agregar la API obligaba a duplicar la lógica de negocio o a amarrarla al framework web. No escala hacia múltiples adaptadores de entrada. |
| **Arquitectura en capas tradicional (N-capas)** | Separa presentación, negocio y datos, pero normalmente la capa de negocio termina dependiendo de la de datos. No invierte las dependencias, así que el dominio seguiría acoplado a Entity Framework. |
| **Clean Architecture / Onion** | Muy similar y igualmente válida, pero agrega más capas y reglas (casos de uso, gateways, presenters) que suponen más complejidad de la necesaria para el alcance actual y una desarrolladora principiante. Hexagonal logra el mismo aislamiento con menos ceremonia. |
| **Microservicios** | Separar cada módulo en un servicio independiente con su propia base de datos es excesivo para un sistema de bajo volumen operado por una familia. Agrega complejidad de despliegue y comunicación sin beneficio real en esta etapa. |

---

## Consecuencias

**✅ Lo que gano:**

- **Técnica**: La lógica de negocio queda aislada en `Domain` y `Application`. La web MVC y la API REST reutilizan los mismos casos de uso sin duplicar código, y la base de datos se conecta a través de puertos, lo que permite probar y cambiar la infraestructura sin tocar el núcleo.
- **Proceso**: Cada proyecto tiene una responsabilidad clara, así se desarrolla y se entiende módulo por módulo. Agregar un nuevo adaptador (por ejemplo, la API en el ADR-03) no obliga a modificar el dominio.
- **Negocio**: El sistema queda preparado para crecer —una futura app móvil consume la misma lógica vía API— lo que respeta la visión de crecimiento del negocio sin reescribir lo ya construido.

**⚠️ Lo que sacrifico o asumo:**

- **Técnica**: Hay más proyectos e interfaces que en un MVC simple. Para alguien que empieza, seguir el flujo Adaptador → Caso de uso → Puerto → Repositorio requiere entender primero la idea de puertos y adaptadores. Es complejidad asumida a cambio del desacoplamiento.
- **Deuda o riesgo**: Esta arquitectura revierte la decisión del ADR-01, por lo que parte del código inicial tuvo que reorganizarse en los nuevos proyectos. Es una inversión de tiempo asumida al inicio para evitar una refactorización mucho mayor más adelante.

---

## Estructura de la solución

```text
TransGGP.sln
├── TransGGP.Domain/             ← Núcleo: entidades del negocio (sin dependencias)
├── TransGGP.Application/        ← Núcleo: casos de uso (Services) + puertos (interfaces)
├── TransGGP.Infrastructure/     ← Adaptador de salida: EF Core + MySQL (implementa los puertos)
├── TransGGP.Web/                ← Adaptador de entrada: MVC + Razor + Bootstrap
└── TransGGP.API/                ← Adaptador de entrada: API REST + Swagger
```

## Diagrama

![Arquitectura Hexagonal de TransGGP](images/ArqHexagonal.png)

## Cláusula de IA

Utilicé IA para redactar y organizar este ADR y para comprender mejor cómo separar el sistema en puertos y adaptadores dentro de la Arquitectura Hexagonal. La mayoría del proyecto está realizado con las diapositivas del profesor.
