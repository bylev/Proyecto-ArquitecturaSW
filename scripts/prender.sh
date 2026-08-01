#!/usr/bin/env bash
#
# prender.sh — Vuelve a levantar el entorno de TransGGP.
#
#   1. Arranca la instancia de RDS y espera a que esté disponible
#   2. Regresa la capacidad de Elastic Beanstalk a 1 instancia
#   3. Espera a que la salud del entorno esté en verde
#   4. Verifica que el sitio responda por el dominio de Cloudflare
#
# El orden importa: primero la base de datos. Si la app levanta antes que
# RDS, arranca sin poder conectarse y el entorno se marca como no saludable.
#
# Uso:   ./prender.sh
#
# Requiere: AWS CLI con credenciales configuradas (en CloudShell ya vienen).

set -euo pipefail

# ─────────────────────────────────────────────────────────────────────
# Configuración
# ─────────────────────────────────────────────────────────────────────
REGION="us-east-2"
ENV_NAME="Transportesggp-env"
DOMINIO="https://www.transportesggp.com"

# Capacidad a la que se regresa el entorno.
MIN_INSTANCIAS=1
MAX_INSTANCIAS=1

# Identificador de RDS. Vacío = detección automática.
DB_ID="${DB_ID:-}"

ESTADO="$(dirname "$0")/.ultimo-endpoint"

export AWS_PAGER=""
AWS="aws --region $REGION --no-cli-pager"

# ─────────────────────────────────────────────────────────────────────
# Utilidades
# ─────────────────────────────────────────────────────────────────────
info()  { echo -e "\033[1;36m›\033[0m $*"; }
ok()    { echo -e "\033[1;32m✓\033[0m $*"; }
warn()  { echo -e "\033[1;33m!\033[0m $*"; }
error() { echo -e "\033[1;31m✗\033[0m $*" >&2; }

# ─────────────────────────────────────────────────────────────────────
# 1. RDS → disponible
# ─────────────────────────────────────────────────────────────────────
if [[ -z "$DB_ID" ]]; then
  DB_ID="$($AWS rds describe-db-instances \
    --query "DBInstances[].DBInstanceIdentifier" --output text)"
  if [[ -z "$DB_ID" || "$DB_ID" == *$'\t'* ]]; then
    error "Hay 0 o más de una instancia RDS. Indica cuál usar:"
    error "    DB_ID=mi-instancia ./prender.sh"
    exit 1
  fi
fi

ESTADO_DB="$($AWS rds describe-db-instances --db-instance-identifier "$DB_ID" \
  --query "DBInstances[0].DBInstanceStatus" --output text)"

if [[ "$ESTADO_DB" == "available" ]]; then
  ok "RDS '$DB_ID' ya está disponible."
else
  info "Arrancando la base de datos '$DB_ID' (estado actual: $ESTADO_DB)…"
  if [[ "$ESTADO_DB" == "stopped" ]]; then
    $AWS rds start-db-instance --db-instance-identifier "$DB_ID" \
      --query "DBInstance.DBInstanceIdentifier" --output text > /dev/null
  fi

  info "Esperando a que quede disponible (suele tardar 5-10 min)…"
  for _ in $(seq 1 90); do   # hasta ~15 min
    ESTADO_DB="$($AWS rds describe-db-instances --db-instance-identifier "$DB_ID" \
      --query "DBInstances[0].DBInstanceStatus" --output text)"
    if [[ "$ESTADO_DB" == "available" ]]; then
      ok "RDS disponible."
      break
    fi
    printf "    estado: %s\r" "$ESTADO_DB"
    sleep 10
  done
  echo

  if [[ "$ESTADO_DB" != "available" ]]; then
    error "RDS quedó en estado '$ESTADO_DB'. No se levanta la app hasta que la base esté lista."
    exit 1
  fi
fi

# ─────────────────────────────────────────────────────────────────────
# 2. Elastic Beanstalk → capacidad normal
# ─────────────────────────────────────────────────────────────────────
info "Regresando la capacidad de Elastic Beanstalk a $MIN_INSTANCIAS instancia(s)…"

$AWS elasticbeanstalk update-environment \
  --environment-name "$ENV_NAME" \
  --option-settings \
    Namespace=aws:autoscaling:asg,OptionName=MinSize,Value=$MIN_INSTANCIAS \
    Namespace=aws:autoscaling:asg,OptionName=MaxSize,Value=$MAX_INSTANCIAS \
  --query "EnvironmentName" --output text > /dev/null

info "Esperando a que el entorno quede listo y en verde…"
for _ in $(seq 1 90); do   # hasta ~15 min
  read -r ESTADO_EB SALUD_EB ENDPOINT <<<"$(
    $AWS elasticbeanstalk describe-environments \
      --environment-names "$ENV_NAME" \
      --query "Environments[0].[Status,Health,EndpointURL]" --output text
  )"
  if [[ "$ESTADO_EB" == "Ready" && "$SALUD_EB" == "Green" ]]; then
    ok "Entorno listo y en verde."
    break
  fi
  printf "    estado: %s — salud: %s\r" "$ESTADO_EB" "$SALUD_EB"
  sleep 10
done
echo

if [[ "$SALUD_EB" != "Green" ]]; then
  warn "El entorno quedó en salud '$SALUD_EB' (estado: $ESTADO_EB)."
  warn "Revisa los logs:  eb logs $ENV_NAME   o la consola de EB."
fi

# ─────────────────────────────────────────────────────────────────────
# 3. ¿Cambió la IP? (entorno single-instance)
# ─────────────────────────────────────────────────────────────────────
echo
info "Endpoint del entorno: $ENDPOINT"

if [[ -f "$ESTADO" ]]; then
  ANTERIOR="$(cat "$ESTADO")"
  if [[ "$ENDPOINT" == "$ANTERIOR" ]]; then
    ok "El endpoint no cambió respecto al apagado. No hay que tocar Cloudflare."
  else
    warn "¡EL ENDPOINT CAMBIÓ!"
    echo "    antes:  $ANTERIOR"
    echo "    ahora:  $ENDPOINT"
    warn "Si en Cloudflare tienes un registro A con la IP anterior, actualízalo ahora."
  fi
else
  warn "No hay endpoint previo guardado; si Cloudflare usa un registro A, verifícalo."
fi

# ─────────────────────────────────────────────────────────────────────
# 4. Verificación final por el dominio real
# ─────────────────────────────────────────────────────────────────────
echo
info "Probando $DOMINIO …"
CODIGO="$(curl -s -o /dev/null -w "%{http_code}" -L --max-time 20 "$DOMINIO" || echo "000")"

case "$CODIGO" in
  200|301|302)
    ok "El sitio responde ($CODIGO). Todo listo."
    ;;
  000)
    error "No hubo respuesta del dominio. Revisa Cloudflare y la salud del entorno."
    ;;
  *)
    warn "El dominio respondió con HTTP $CODIGO. Ábrelo en el navegador para ver qué pasa."
    ;;
esac

echo
echo "Antes de exponer, verifica a mano:"
echo "    • Iniciar sesión en $DOMINIO"
echo "    • Abrir el Dashboard (confirma que la app habla con RDS)"
echo "    • Dar de alta un registro de prueba (confirma la escritura)"
