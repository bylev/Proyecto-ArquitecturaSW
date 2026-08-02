# Scripts de infraestructura

Scripts para **pausar y reanudar** el entorno de producción de TransGGP cuando no se está usando, y así no pagar cómputo. Están pensados para ejecutarse en **AWS CloudShell** (ya trae credenciales y AWS CLI), aunque funcionan en cualquier shell con el AWS CLI configurado.

| Script | Qué hace |
|--------|----------|
| `apagar.sh` | Baja Elastic Beanstalk a 0 instancias y detiene la instancia de RDS |
| `prender.sh` | Arranca RDS, regresa EB a 1 instancia y verifica que el sitio responda |

## Uso

Desde CloudShell, clonando el repositorio:

```bash
git clone https://github.com/bylev/Proyecto-ArquitecturaSW.git
cd Proyecto-ArquitecturaSW/scripts

bash apagar.sh      # pausar el entorno
bash prender.sh     # volver a levantarlo
```

Si la detección automática de la base de datos falla (por ejemplo, si hay más de una instancia RDS en la región), se le indica cuál usar:

```bash
DB_ID=mi-instancia bash apagar.sh
```

## Cosas que conviene saber

- **El orden importa.** Al apagar: primero la app, luego la base. Al prender: primero la base, luego la app. Los scripts ya lo respetan y esperan a que cada paso termine antes de seguir.
- **No hagas push a `main` mientras el entorno esté pausado.** El pipeline de CI/CD intentaría desplegar a un entorno sin instancias.
- **AWS reinicia sola una instancia RDS detenida a los 7 días.** Si la pausa va a durar más, hay que volver a detenerla.
- **El almacenamiento se sigue cobrando** (los 20 GB de RDS y los paquetes en S3). Lo que se ahorra es el cómputo: la instancia EC2 y las horas de la base.
- **El entorno es *single-instance***, así que su endpoint es una IP pública. `apagar.sh` la guarda en `.ultimo-endpoint` y `prender.sh` avisa si cambió, porque en ese caso hay que actualizar el registro **A** en Cloudflare. Si Cloudflare apunta por CNAME al dominio de Elastic Beanstalk, no hay nada que hacer.

## Antes de una demostración

`prender.sh` verifica que el dominio responda, pero eso no comprueba que la aplicación esté hablando con la base de datos. Conviene revisar a mano:

1. Iniciar sesión en <https://www.transportesggp.com>
2. Abrir el Dashboard (confirma la lectura desde RDS)
3. Dar de alta un registro de prueba (confirma la escritura)

Y hazlo con horas de anticipación, no minutos antes.
