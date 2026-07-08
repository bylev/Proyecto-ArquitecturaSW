# Transportes GGP 🚚
Sistema de gestión de servicios de transporte de carga.

---

# Descripción 

**Transportes GGP**, es un sistema que digitaliza el proceso de registro y consulta de servicios de transporte de carga. Permite al cliente llevar un control de sus viajes y al mismo tiempo, al personal de la empresa le permite gestionar los servicios de transporte. Este proyecto sustituye el uso de hojas de cálculo para llevar el control de los servicios de transporte.

## Objetivo

El objetivo de este proyecto es implementar una arquitectura de software robusta, escalable y mantenible que permita satisfacer las necesidades de gestión de servicios de transporte de carga. Para ello, se ha optado por una arquitectura limpia, separando la lógica de negocio, la infraestructura y la interfaz de usuario.

## Arquitectura del Sistema

La arquitectura del sistema está compuesta por un patrón de diseño **Hexagonal**, en donde, se separa el dominio, la aplicación, la infrastructura, y API.

## Tecnologías a utilizar
- ASP.NET Core 10
- MySQL --> AWS RDS
- EC2 --> AWS EC2
- API con Swagger UI
- Cloudflare como dominio.

## Diagrama C4

En esta sección, se muestra el diagrama C4 del sistema, en donde se puede ver la arquitectura del sistema, los componentes y las interacciones entre ellos.

↪︎ Aquí puedes encontrar los diagramas: [Diagramas C4](docs/Diagramas.md)

# Estructura del Proyecto

```text
Proyecto-ArquitecturaSW/
├── TransGGP/                                    # 🌐 Aplicación Web MVC
│   ├── Areas/
│   │   └── Identity/                            # Área de autenticación
│   ├── Controllers/
│   │   ├── HomeController.cs
│   │   ├── ClientesController.cs
│   │   ├── OperadoresController.cs
│   │   ├── UnidadesController.cs
│   │   └── ServiciosController.cs
│   ├── Views/
│   │   ├── Home/
│   │   ├── Clientes/
│   │   ├── Operadores/
│   │   ├── Servicios/
│   │   ├── Unidades/
│   │   ├── Shared/
│   │   ├── _ViewImports.cshtml
│   │   └── _ViewStart.cshtml
│   ├── ViewModels/
│   │   └── ErrorViewModel.cs
│   ├── wwwroot/                                 # Archivos estáticos (CSS, JS, imágenes)
│   ├── Properties/
│   ├── Program.cs
│   ├── TransGGP.Web.csproj
│   └── TransGGP.slnx                           # Archivo de solución
│
├── TransGGP.API/                                # 🔌 API REST
│   ├── Controllers/
│   │   └── ClientesAPI.cs
│   ├── Properties/
│   ├── Program.cs
│   ├── TransGGP.API.csproj
│   ├── TransGGP.API.http
│   └── appsettings.json.example
│
├── TransGGP.Application/                        # ⚙️ Capa de Aplicación (Lógica de negocio)
│   ├── DTOs/
│   │   └── ClienteCreateDto.cs
│   ├── Interfaces/                              # Puertos (contratos)
│   │   └── IClienteRepository.cs
│   ├── Services/
│   │   └── ClienteService.cs
│   ├── Reports/                                 # Patrón Factory Method
│   │   ├── IReporte.cs                          # Interfaz + ReporteTexto, ReporteCsv
│   │   └── ReporteCreator.cs                    # Creator abstracto + Creators concretos
│   └── TransGGP.Application.csproj
│
├── TransGGP.Domain/                             # 🏛️ Capa de Dominio (Entidades)
│   ├── Models/
│   │   ├── Cliente.cs
│   │   ├── Configuracion.cs
│   │   ├── Dolly.cs
│   │   ├── Operador.cs
│   │   ├── Semirremolque.cs
│   │   ├── Servicio.cs
│   │   ├── Unidad.cs
│   │   └── Usuario.cs
│   └── TransGGP.Domain.csproj
│
├── TransGGP.Infrastructure/                     # 🗄️ Capa de Infraestructura (Adaptadores)
│   ├── Data/
│   │   └── ApplicationDbContext.cs              # DbContext de EF Core
│   ├── Repositories/
│   │   └── ClienteRepository.cs                 # Implementación del repositorio
│   ├── Decorators/
│   │   └── ClienteRepositoryLoggingDecorator.cs # Patrón Decorator (logging)
│   ├── Migrations/                              # Migraciones de EF Core
│   ├── DependencyInjection.cs                   # Registro de servicios
│   └── TransGGP.Infrastructure.csproj
│
├── docs/                                        # 📖 Documentación
│   ├── ADR-01-Michelle-Camara.md                # Arquitectura seleccionada
│   ├── ADR-02-Michelle-Camara.md                # Vistas arquitectónicas y diagramas
│   ├── ADR-03-Michelle-Camara.md                # Incorporación de API REST
│   ├── ADR-04-Michelle-Camara.md                # Migración a Arquitectura Hexagonal
│   ├── ADR-05-Michelle-Camara.md                # Patrones de diseño GoF
│   └── Diagramas.md                             # Diagramas C4 en Mermaid
│
├── images/                                      # 🖼️ Imágenes de documentación
│   ├── API.png
│   ├── ArqHexagonal.png
│   ├── DiagaramArquitectonico.jpg
│   ├── DiagramaProcesos.png
│   ├── GET.png
│   ├── Nivel1.png
│   ├── Nivel2.png
│   ├── POST1.png
│   └── POST2.png
│
├── .github/                                     # Configuración de GitHub
│   └── Nivel2.png
├── .gitignore
└── README.md

```
