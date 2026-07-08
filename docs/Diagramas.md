# Arquitectura de Transportes GGP

## Diagrama Modelo C4

El modelo C4 es una forma de representar la arquitectura de un sistema de software. En donde, el primer nivel es el contexto del sistema, quién lo usa y qué es. El siguiente nivel, es el contenedor (aplicaciones, bases de datos, etc.), el tercero son los componentes, y el cuarto son el código.

## Nivel 1

```mermaid
graph TB

Admin[Administrador]
Consulta[Capturista]
TransportesGGP[TransportesGGP - Sistema de gestión de servicios de transporte de carga]

Admin --> |Usa| TransportesGGP
Consulta -->|Usa| TransportesGGP

```
En este nivel, se muestra los usuarios Administrador y Capturista, los cuales interactuan con el sistema de gestión de servicios de transporte de carga. 

## Nivel 2

```mermaid
graph TB
    Admin[Administrador]
    Capturista[Capturista]

    Cloudflare["Cloudflare - DNS y proxy del dominio"]

    subgraph EC2["AWS EC2"]
        Web["TransGGP.Web - Aplicación MVC con Razor Views"]
        API["TransGGP.API - API REST con Swagger"]
        App["TransGGP.Application - Lógica de negocio y servicios"]
        Domain["TransGGP.Domain - Entidades del dominio"]
        Infra["TransGGP.Infrastructure - Acceso a datos con EF Core"]
    end

    RDS[("AWS RDS - MySQL")]

    Admin -->|Usa| Cloudflare
    Capturista -->|Usa| Cloudflare
    Cloudflare -->|Proxy HTTPS| Web
    Web --> App
    API --> App
    App --> Domain
    App --> Infra
    Infra --> Domain
    Infra -->|Lee y escribe| RDS

```

En este nivel, se muestra los contenedores de la arquitectura, es decir, las aplicaciones, bases de datos, etc. En este caso, se muestra el frontend, la api, la base de datos, etc. Y como se comunican entre sí. 

## Nivel 3


```mermaid
graph TB
    Admin[Administrador]
    Capturista[Capturista]

    subgraph Web["TransGGP.Web - Adaptador de entrada MVC"]
        HC[HomeController]
        CC[ClientesController]
        OC[OperadoresController]
        UC[UnidadesController]
        SC[ServiciosController]
        Views[Razor Views]
    end

    subgraph API["TransGGP.API - Adaptador de entrada REST"]
        ClientesAPI[ClientesApiController]
        Swagger[Swagger]
        DTO[ClienteCreateDto]
    end

    subgraph Application["TransGGP.Application - Núcleo: casos de uso"]
        CS[ClienteService]
        OS[OperadorService]
        US[UnidadService]
        SS[ServicioService]
        ICR[IClienteRepository - Puerto]
        IOR[IOperadorRepository - Puerto]
        IUR[IUnidadRepository - Puerto]
        ISR[IServicioRepository - Puerto]
        subgraph FactoryMethod["Factory Method"]
            RC[ReporteCreator]
            RTC[ReporteTextoCreator]
            RCC[ReporteCsvCreator]
            IR[IReporte]
        end
    end

    subgraph Infrastructure["TransGGP.Infrastructure - Adaptador de salida"]
        CR[ClienteRepository]
        OR[OperadorRepository]
        UR[UnidadRepository]
        SR[ServicioRepository]
        subgraph Decorator["Decorator"]
            CRLD[ClienteRepositoryLoggingDecorator]
        end
        DbCtx[ApplicationDbContext]
    end

    RDS[("AWS RDS - MySQL")]

    Admin -->|Usa| Web
    Capturista -->|Usa| Web
    CC --> CS
    OC --> OS
    UC --> US
    SC --> SS
    CC --> RC
    ClientesAPI --> CS
    CS --> ICR
    OS --> IOR
    US --> IUR
    SS --> ISR
    ICR -.->|Decorator envuelve| CRLD
    CRLD -->|Delega| CR
    IOR -.-> OR
    IUR -.-> UR
    ISR -.-> SR
    CR --> DbCtx
    OR --> DbCtx
    UR --> DbCtx
    SR --> DbCtx
    DbCtx -->|EF Core| RDS
```
En este nivel, se muestra los componentes de la arquitectura, es decir, las aplicaciones, bases de datos, etc. En este caso, se muestra el frontend, la api, la base de datos, etc. Y además, los patrones de diseño que se utilizan en cada componente y como se comunican entre sí para lograr la funcionalidad del sistema.