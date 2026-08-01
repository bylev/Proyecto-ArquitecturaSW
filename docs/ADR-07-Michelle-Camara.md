# ADR-07: Migración de infraestructura — WampServer a AWS RDS y EC2 a Elastic Beanstalk

| Campo  | Valor |
|--------|-------|
| Autor  | Michelle Cámara |
| Fecha  | 31/07/2026 |
| Estado | `Aceptado` |

---

## Contexto

**TransGGP** es un sistema web de gestión de servicios de transporte de carga desarrollado para un emprendimiento familiar. Desde el inicio del desarrollo, el entorno local utilizaba **WampServer** como servidor de base de datos MySQL en `localhost`, lo que permitía desarrollar y probar rápidamente sin configuración adicional. Para el despliegue en la nube, el ADR-01 estableció que se usaría una instancia **EC2** de AWS con la aplicación corriendo directamente sobre el sistema operativo.

Conforme el proyecto maduró y se incorporaron el pipeline de CI/CD con GitHub Actions (ADR-06) y la arquitectura hexagonal (ADR-04), surgieron dos problemas concretos que esta infraestructura inicial no resolvía bien:

1. **Base de datos local (WampServer)**: el entorno de desarrollo dependía de una instalación local de WampServer que variaba entre máquinas. Cada desarrollador tenía que instalar y configurar MySQL manualmente, lo que generaba inconsistencias. Además, no existía un entorno de producción separado: la misma base local servía para desarrollo y demo, mezclando datos de prueba con datos reales. Para producción real, se necesitaba una base de datos accesible desde la nube, con respaldos automáticos, alta disponibilidad y sin mantenimiento de servidor.

2. **Despliegue manual en EC2**: la instancia EC2 requería que se configurara manualmente el entorno de ejecución (.NET 10 runtime, Nginx como proxy reverso, certificados SSL, systemd para mantener la app corriendo). Cada despliegue implicaba conectarse por SSH, subir los archivos, reiniciar el servicio y verificar que todo funcionara. Esto era propenso a errores, lento y difícil de automatizar correctamente con el pipeline de CI/CD.

Las restricciones del proyecto son: presupuesto limitado (emprendimiento familiar), una sola desarrolladora, necesidad de despliegue automatizado desde GitHub Actions y de que el sistema funcione en producción real.

---

## Decisión

Se toman dos decisiones de infraestructura:

### 1. Migrar de WampServer (MySQL local) a AWS RDS (MySQL administrado)

Se reemplaza la base de datos local de WampServer por una instancia **Amazon RDS for MySQL** que sirve tanto al entorno de producción como al de desarrollo remoto.

### 2. Migrar de EC2 (servidor manual) a AWS Elastic Beanstalk (plataforma administrada)

Se reemplaza la instancia EC2 con configuración manual por un entorno de **AWS Elastic Beanstalk** que administra automáticamente el servidor, el balanceador de carga, el escalamiento y los despliegues.

---

## ¿Por qué RDS en lugar de MySQL en EC2 o WampServer?

La característica concreta que resuelve el problema es que **RDS es un servicio administrado**: AWS se encarga del mantenimiento del motor de base de datos, los respaldos automáticos, las actualizaciones de seguridad y la alta disponibilidad, sin que la desarrolladora tenga que administrar un servidor de base de datos.

| Aspecto | WampServer (local) | MySQL en EC2 | AWS RDS |
|---------|---------------------|--------------|---------|
| **Mantenimiento** | Manual (instalar, actualizar, configurar) | Manual (parches de SO, MySQL, backups) | Automático (AWS lo administra) |
| **Respaldos** | Ninguno (responsabilidad del usuario) | Manuales (scripts + cron) | Automáticos (snapshots diarios) |
| **Alta disponibilidad** | No | Requiere configurar réplicas manualmente | Multi-AZ con un clic |
| **Acceso remoto** | Solo local (o configurar tunnels) | Sí, pero hay que administrar el servidor | Sí, sin administrar servidor |
| **Costo** | Gratis (pero no sirve en producción) | ~$8/mes (t3.micro) + tiempo de administración | ~$15/mes (db.t3.micro) |
| **Seguridad** | Sin cifrado, sin parches automáticos | Responsabilidad del usuario | Cifrado en reposo y en tránsito, parches automáticos |

WampServer era adecuado para las primeras semanas de desarrollo pero **no es viable para producción**: no tiene respaldos, no es accesible desde la nube y depende de que la computadora local esté encendida.

MySQL en EC2 habría funcionado, pero agrega la responsabilidad de administrar el servidor de base de datos además de la aplicación: parches, backups, monitoreo, seguridad. Para una sola desarrolladora con presupuesto limitado, eso es carga operativa innecesaria.

---

## ¿Por qué Elastic Beanstalk en lugar de EC2?

La característica concreta que resuelve el problema es que **Elastic Beanstalk automatiza el despliegue y la administración del servidor**. En lugar de configurar manualmente .NET, Nginx, systemd y SSL en una instancia EC2, Elastic Beanstalk recibe un paquete `.zip` con la aplicación y se encarga de todo lo demás.

| Aspecto | EC2 (manual) | Elastic Beanstalk |
|---------|-------------|-------------------|
| **Despliegue** | SSH + copiar archivos + reiniciar servicios | Subir ZIP a S3 → EB actualiza automáticamente |
| **Configuración del servidor** | Manual (instalar .NET runtime, Nginx, systemd) | Automática (EB configura la plataforma) |
| **Escalamiento** | Manual (crear más instancias, configurar balanceador) | Automático (configurable con políticas) |
| **Monitoreo** | Configurar CloudWatch manualmente | Integrado (health checks, logs, métricas) |
| **Rollback** | Manual (restaurar archivos anteriores) | Automático (versiones anteriores disponibles) |
| **CI/CD** | Complejo (scripts SSH, rsync, reinicio de servicios) | Simple (API de EB desde GitHub Actions) |
| **Costo** | Solo pagas la instancia EC2 | Solo pagas la instancia EC2 subyacente (EB es gratis) |

La razón principal para elegir Elastic Beanstalk sobre EC2 fue la **integración con el pipeline de CI/CD**. Con EC2, automatizar el despliegue desde GitHub Actions requería scripts complejos de SSH, transferencia de archivos y reinicio de servicios, todo propenso a fallar silenciosamente. Con Elastic Beanstalk, el despliegue se reduce a tres comandos de AWS CLI:

```bash
# 1. Subir el paquete a S3
aws s3 cp despliegue.zip s3://bucket/despliegues/...

# 2. Registrar la versión
aws elasticbeanstalk create-application-version ...

# 3. Actualizar el entorno
aws elasticbeanstalk update-environment ...
```

Además, Elastic Beanstalk **no tiene costo adicional**: se paga únicamente por los recursos subyacentes (EC2, RDS, S3), lo que lo hace equivalente en costo a EC2 puro pero con mucha menos carga operativa.

### Alternativas consideradas

| Alternativa | Por qué la descarté |
|-------------|---------------------|
| **Mantener EC2 con scripts de despliegue** | Funciona, pero cada despliegue requiere scripts SSH frágiles y configuración manual del servidor. El riesgo de errores en producción aumenta con cada despliegue. |
| **AWS ECS (contenedores)** | Requiere dockerizar la aplicación y aprender conceptos de orquestación de contenedores. Es una buena opción a futuro pero excesiva para el alcance actual y una desarrolladora principiante. |
| **AWS Lambda + API Gateway (serverless)** | No es compatible con ASP.NET Core MVC con vistas Razor. Está diseñado para funciones individuales, no para aplicaciones web completas con interfaz gráfica. |
| **Azure App Service** | Excelente soporte para .NET, pero el dominio y la infraestructura existente ya están en AWS. Mezclar proveedores de nube agrega complejidad de facturación y configuración. |
| **Railway / Render / Fly.io** | Plataformas modernas y simples, pero el equipo ya tiene experiencia con AWS y el dominio está en Cloudflare apuntando a recursos de AWS. Cambiar de proveedor no aporta beneficio suficiente. |

---

## Implementación

### Arquitectura de despliegue

```text
Usuario → Cloudflare (DNS + HTTPS) → Elastic Beanstalk (Linux + .NET 10) → RDS (MySQL)
```

### Configuración de RDS

- **Motor**: MySQL 8.0
- **Instancia**: db.t3.micro (Free Tier eligible)
- **Almacenamiento**: 20 GB SSD (gp2)
- **Respaldos**: Automáticos con retención de 7 días
- **Acceso**: Security Group restringido al entorno de Elastic Beanstalk

### Configuración de Elastic Beanstalk

- **Plataforma**: .NET on Linux
- **Tipo de instancia**: t3.micro
- **Región**: us-east-2
- **Application**: `transportesggp`
- **Environment**: `transportesggp-env`

### Pipeline de CI/CD

El pipeline de GitHub Actions (`.github/workflows/ci.yml`) se integra directamente con Elastic Beanstalk:

1. **Etapa 1 (Pruebas)**: Compila y ejecuta las 9 pruebas unitarias
2. **Etapa 2 (Deploy)**: Solo si las pruebas pasaron y no es un PR
   - Publica la aplicación autocontenida para Linux (`--self-contained true`)
   - Genera el `Procfile` y el script de inicio
   - Sube el paquete ZIP a S3
   - Registra la nueva versión en Elastic Beanstalk
   - Actualiza el entorno de producción

### Cadena de conexión

La cadena de conexión a RDS se configura mediante **variables de entorno** en Elastic Beanstalk, no se versiona en el repositorio. En `appsettings.json` solo se deja un valor de ejemplo:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=TU_HOST_RDS;Database=transggp;User=TU_USUARIO;Password=TU_PASSWORD;Port=3306"
}
```

Las credenciales reales se configuran como variables de entorno en el entorno de Elastic Beanstalk y se leen mediante la configuración de ASP.NET Core.

---

## Consecuencias

**✅ Lo que gano:**

- **Técnica**: la base de datos tiene respaldos automáticos, cifrado y alta disponibilidad sin administración manual. El despliegue es automático desde GitHub Actions y se puede hacer rollback con un clic. La aplicación se publica autocontenida, sin depender del runtime instalado en el servidor.
- **Proceso**: el flujo completo es push → pruebas → despliegue automático. Se eliminan los pasos manuales de SSH, transferencia de archivos y reinicio de servicios. Cualquier miembro del equipo puede desplegar con solo hacer push.
- **Negocio**: el sistema está disponible en producción real con un dominio propio (`transportesggp.com`), lo que permite que la familia use el sistema desde cualquier dispositivo. El costo se mantiene bajo (~$25/mes entre RDS + EC2 subyacente).

**⚠️ Lo que sacrifico o asumo:**

- **Técnica**: Elastic Beanstalk abstrae la configuración del servidor, lo que puede dificultar diagnósticos avanzados. Si se necesita acceso bajo nivel al servidor, se puede conectar por SSH a la instancia EC2 subyacente, pero no es el flujo principal.
- **Deuda o riesgo**: las credenciales de AWS (`AWS_ACCESS_KEY_ID` y `AWS_SECRET_ACCESS_KEY`) se almacenan como secretos de GitHub Actions. Si el repositorio cambia de dueño o se expone, habría que rotar las credenciales. A futuro se recomienda usar OIDC con IAM Roles para eliminar las credenciales de larga duración.

---

## Cláusula de IA

Se utilizó una herramienta de inteligencia artificial como apoyo para redactar, estructurar y organizar este ADR. La decisión de migrar a RDS y Elastic Beanstalk se tomó con base en la experiencia del equipo con AWS, el análisis de costos y la necesidad de automatizar el despliegue desde GitHub Actions. Se contrastó con la documentación oficial de AWS y las diapositivas del profesor.
