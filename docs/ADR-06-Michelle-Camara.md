# ADR-06: Deuda técnica identificada en el proyecto y pruebas unitarias.

| Campo  | Valor |
|--------|-------|
| Autor  | Michelle Cámara |
| Fecha  | 15/07/2026 |
| Ultima act. | 22/07/2026|
| Estado | `Propuesto` |

---

## Contexto

**TransGGP** es un sistema web de gestión de servicios de transporte de carga desarrollado para un emprendimiento familiar que opera un tráiler. En los ADR anteriores se definió el stack (MVC), las vistas arquitectónicas, la **Arquitectura Hexagonal**, la exposición mediante **API REST + Swagger** y la incorporación de dos **patrones GOF** (Decorator y Factory Method).

El sistema creció por iteraciones: cada módulo (Clientes, Operadores, Unidades y Servicios) se fue agregando conforme se necesitaba y bajo presión de entrega. Ese ritmo dejó decisiones que hoy funcionan pero que, si no se corrigen, encarecerán el mantenimiento. Este ADR **documenta la deuda técnica** detectada en el código actual: qué es, por qué existe, qué cuesta no pagarla y con qué técnica de refactorización se resolvería.

Se documentan cuatro deudas, ordenadas por impacto. **La deuda #4 es de configuración/infraestructura** (credenciales escritas a mano en el repositorio).

---

## Decisión

Se reconoce formalmente la deuda técnica del proyecto y se registra un plan de refactorización para cada punto. No se cancela toda la deuda en esta iteración, pero se deja documentada y priorizada para pagarla de forma ordenada en las siguientes fases.

---

## Deudas técnicas identificadas

### 1) Tight coupling en la composición de dependencias

**Qué es**

`Program.cs` registra y resuelve servicios concretos directamente: `ClienteService`, `OperadorService`, `UnidadService`, `ServicioService`, e incluso un `ClienteRepository` concreto envuelto manualmente por un decorador. Además, los controladores dependen de clases concretas en vez de abstraer detrás de interfaces.

**Por qué existe**

Parece una decisión consciente para avanzar rápido y dejar la inyección "funcionando" sin montar una arquitectura más desacoplada. También es típico de una solución que creció por iteraciones y fue incorporando servicios concretos sin pasar por una capa de abstracción uniforme.

**Costo de no pagarla**

- Dificulta pruebas unitarias reales, porque los controladores quedan atados a implementaciones específicas.
- Cada cambio en la infraestructura o en la forma de crear repositorios obliga a tocar el arranque de la app y quizá varios controladores.
- Crece el riesgo de efectos colaterales: cambiar un servicio puede romper la web aunque la lógica de negocio no haya cambiado.

**Propuesta de solución**

Aplicaría **Dependency Inversion + registro por interfaces**. Dirección:

- `ClientesController` debería depender de `IClienteService`, no de `ClienteService`.
- Igual para `OperadoresController`, `UnidadesController`, `ServiciosController`.
- Encapsular el *wiring* del decorador en una extensión de infraestructura, no en `Program.cs`.
- Si hace falta, introducir un patrón tipo *Application Services + Interfaces* consistente.

---

### 2) God class / clase con demasiadas responsabilidades en `ServiciosController`

**Qué es**

`ServiciosController` no solo coordina la UI. También:

- carga *dropdowns* para clientes, operadores y unidades;
- valida y corrige fechas "inválidas" antes de persistir;
- decide cómo se crea un servicio;
- coordina cuatro servicios distintos;
- contiene lógica de negocio/seguridad de datos mezclada con lógica de presentación.

**Por qué existe**

Muy probablemente por una decisión pragmática: evitar crear más clases para algo que "solo era un controlador". También puede venir de un descuido progresivo: cada nueva necesidad se fue agregando allí porque era el sitio más fácil de tocar.

**Costo de no pagarla**

- El controlador se vuelve frágil y difícil de entender.
- Cualquier cambio en catálogos, validación o flujo de creación obliga a editar el mismo archivo.
- Aumenta el riesgo de bugs al editar un método, porque varias responsabilidades están acopladas.
- Las pruebas se complican: para probar un caso simple terminas montando demasiadas dependencias.

**Propuesta de solución**

Aplicaría **Extract Class** y **Move Business Rules to Service**:

- Extraer la carga de *dropdowns* a un servicio/fachada, por ejemplo `ServicioFormDataService`.
- Mover la normalización de fechas a una regla de negocio o a un método de dominio/servicio, no al controlador.
- Si el flujo de creación crece, introducir un *Application Service* o un *Use Case Handler* para `CrearServicio` y `EditarServicio`.

---

### 3) Controlador con lógica repetida y validación "defensiva" duplicada

**Qué es**

En `ServiciosController` hay la misma lógica de fechas repetida en `Create` y `Edit`:

- si `FechaCarga < 2000-01-01`, se cambia a `DateTime.Now`;
- si `FechaEntrega < 2000-01-01`, se cambia a `DateTime.Now`.

Eso es una señal de que la regla no está en el lugar correcto.

**Por qué existe**

Normalmente aparece por un problema detectado tarde: la base de datos rechaza ciertos valores y la solución rápida fue parchearlo en el controlador. Es una corrección reactiva, no un diseño intencional.

**Costo de no pagarla**

- La regla queda duplicada y puede divergir con el tiempo.
- Si mañana aparece otro punto de entrada a `Servicio`, la validación se puede olvidar.
- La lógica de negocio queda escondida en la UI, haciendo más difícil reutilizarla.

**Propuesta de solución**

Aplicaría **Extract Method** y después **Move Validation**:

- Extraer la normalización a un método privado temporalmente.
- Luego moverla a una validación de aplicación o a un *factory*/constructor del dominio.
- Idealmente, encapsular la regla de fechas en un único sitio del flujo de creación/actualización.

---

### 4) Credenciales y parámetros de conexión escritos a mano en el repositorio *(configuración / infraestructura)*

**Qué es**

La cadena de conexión a MySQL está escrita directamente en `TransGGP/appsettings.json` y versionada en el repositorio, con usuario y contraseña incluidos:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=transggp;User=root;Password=;Port=3306"
}
```

Los parámetros (host, usuario, contraseña, puerto) están escritos a mano en un solo archivo, sin separar el valor de desarrollo del de producción y sin usar variables de entorno ni un gestor de secretos.

**Por qué existe**

Es una decisión pragmática para que la app "conecte y corra" de inmediato durante el desarrollo local, apuntando a un MySQL en `localhost` con el usuario `root`. Al crecer por iteraciones, nunca se separó la configuración sensible del código, y el archivo quedó tal cual dentro del control de versiones.

**Costo de no pagarla**

- **Seguridad**: al desplegar en el EC2/RDS definido en el ADR-01, cualquier credencial real quedaría expuesta en el historial de Git, visible para quien tenga acceso al repositorio.
- **Operación**: cambiar de servidor, rotar la contraseña o mover a producción obliga a editar y recompilar/redeployar en vez de cambiar una variable de entorno.
- **Riesgo de error**: es fácil subir por accidente credenciales de producción, o pisar la configuración de un ambiente con la de otro, porque todo vive en el mismo archivo.

**Propuesta de solución**

Aplicaría **Externalize Configuration** (extraer la configuración fuera del código versionado):

- Mover la cadena de conexión a **variables de entorno** o *User Secrets* en desarrollo, y a variables de entorno del servidor en producción.
- Dejar en `appsettings.json` solo un valor de ejemplo o vacío, y crear un `appsettings.Example.json` documentado; excluir del control de versiones cualquier archivo con credenciales reales (`.gitignore`).
- Separar por ambiente (`appsettings.Development.json` / `appsettings.Production.json`) y, a futuro, usar un gestor de secretos (por ejemplo AWS Secrets Manager, coherente con el despliegue en AWS del ADR-01).

---

## Consecuencias

**✅ Lo que gano:**

- **Técnica**: al pagar las deudas, los controladores dependerán de interfaces y serán realmente testeables; las responsabilidades quedarán separadas y la regla de fechas vivirá en un único lugar. La configuración sensible saldrá del código.
- **Proceso**: la deuda queda documentada y priorizada, así se paga de forma ordenada por fases en lugar de acumularse en silencio.
- **Negocio**: reducir el acoplamiento y proteger las credenciales baja el riesgo de bugs y de exposición de datos cuando el sistema pase a producción real.

**⚠️ Lo que sacrifico o asumo:**

- **Técnica**: refactorizar toma tiempo que no produce funcionalidad nueva visible para el usuario; hay que introducir interfaces, clases y configuración adicional.
- **Deuda o riesgo**: en esta iteración se documenta y prioriza, pero **no se cancela toda la deuda**. Mientras no se paguen las deudas #1–#3 los controladores siguen acoplados, y mientras no se pague la #4 las credenciales siguen en el repositorio; es una deuda asumida conscientemente para el alcance actual.

---

---

## Pruebas automatizadas e integración continua

Como parte de esta iteración se agregó una suite de pruebas unitarias utilizando **xUnit**. El objetivo es verificar el comportamiento de las clases de la capa de aplicación sin depender de la base de datos ni de la infraestructura real del sistema.

### Clases probadas

Se eligieron las siguientes clases:

- `ClienteService`
- `OperadorService`
- `UnidadService`

Estas clases fueron seleccionadas porque coordinan operaciones importantes del sistema, como registrar, consultar y eliminar clientes, operadores y unidades.

Además, reciben sus dependencias mediante las interfaces `IClienteRepository`, `IOperadorRepository` e `IUnidadRepository`. Esto permite sustituir los repositorios reales por repositorios falsos durante las pruebas y comprobar la lógica de los servicios sin conectarse a MySQL.

### Casos de prueba

Para cada clase se agregaron tres pruebas:

1. Registrar una entidad y comprobar que fue almacenada correctamente.
2. Buscar una entidad por su identificador y comprobar que se devuelve la información esperada.
3. Eliminar una entidad existente y comprobar que ya no se encuentra en el repositorio.

En total, la suite contiene **9 pruebas unitarias**:

- 3 pruebas para `ClienteService`.
- 3 pruebas para `OperadorService`.
- 3 pruebas para `UnidadService`.

Las pruebas utilizan la estructura **Arrange-Act-Assert**:

- **Arrange:** se preparan el repositorio falso, el servicio y los datos necesarios.
- **Act:** se ejecuta el método que se desea probar.
- **Assert:** se comprueba que el resultado y el estado del repositorio sean los esperados.

### Repositorios falsos

Para aislar las clases de servicio se crearon las siguientes implementaciones:

- `FakeClienteRepository`
- `FakeOperadorRepository`
- `FakeUnidadRepository`

Se decidió utilizar repositorios falsos en lugar de una biblioteca de mocks porque permiten representar de manera sencilla un almacenamiento en memoria y facilitan la comprensión del funcionamiento de cada prueba.

### Integración continua

También se configuró un workflow de **GitHub Actions** que se ejecuta automáticamente en cada `push` y en cada actualización de un Pull Request.

El pipeline realiza las siguientes operaciones:

1. Descarga el repositorio.
2. Configura el SDK de .NET 10.
3. Restaura las dependencias.
4. Compila la solución.
5. Ejecuta todas las pruebas unitarias.

Esta automatización permite detectar errores antes de integrar los cambios en la rama principal y asegura que las pruebas continúen funcionando después de cada modificación.

## Cláusula de IA

Se utilizó una herramienta de inteligencia artificial como apoyo para revisar el código existente (`Program.cs`, `ServiciosController` y `appsettings.json`), identificar la deuda técnica y redactar y organizar este ADR. El análisis de las deudas y las técnicas de refactorización se contrastó con el propio código del proyecto y con las diapositivas del profesor.
