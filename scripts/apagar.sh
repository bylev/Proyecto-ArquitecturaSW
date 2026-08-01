#!/usr/bin/env bash
#
# apagar.sh — Pausa el entorno de TransGGP para dejar de pagar cómputo.
#
#   1. Baja la capacidad de Elastic Beanstalk a 0 instancias
#   2. Detiene la instancia de RDS
#
# El orden importa: primero la app, luego la base de datos. Si se apaga
# RDS primero, la aplicación queda lanzando errores de conexión.
#
# Uso:   ./apagar.sh          (pide confirmación)
#        ./apagar.sh -y       (sin confirmación)
#
# Requiere: AWS CLI con credenciales configuradas (en CloudShell ya vienen).

set -euo pipefail

# ─────────────────────────────────────────────────────────────────────
# Configuración
# ─────────────────────────────────────────────────────────────────────
REGION="us-east-2"
ENV_NAME="Transportesggp-env"

# Identificador de RDS. Si se deja vacío, el script lo detecta solo
# (funciona mientras haya una sola base de datos en la región).
DB_ID="${DB_ID:-}"

# Archivo donde se guarda la IP/endpoint actual, para poder compararla
# al volver a prender el entorno.
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

confirmar() {
  [[ "${1:-}" == "-y" ]] && return 0
  read -r -p "¿Apagar el entorno de TransGGP? [s/N] " respuesta
  [[ "$respuesta" =~ ^[sS]$ ]] || { echo "Cancelado."; exit 0; }
}

# ─────────────────────────────────────────────────────────────────────
# 0. Estado actual
# ─────────────────────────────────────────────────────────────────────
info "Consultando el estado actual…"

read -r ESTADO_EB SALUD_EB ENDPOINT <<<"$(
  $AWS elasticbeanstalk describe-environments \
    --environment-names "$ENV_NAME" \
    --query "Environments[0].[Status,Health,EndpointURL]" --output text
)"

if [[ "$ESTADO_EB" == "None" || -z "$ESTADO_EB" ]]; then
  error "No se encontró el entorno '$ENV_NAME' en $REGION."
  exit 1
fi

echo "    Elastic Beanstalk : $ENV_NAME — $ESTADO_EB / $SALUD_EB"
echo "    Endpoint actual   : $ENDPOINT"

if [[ -z "$DB_ID" ]]; then
  DB_ID="$($AWS rds describe-db-instances \
    --query "DBInstances[].DBInstanceIdentifier" --output text)"
  if [[ -z "$DB_ID" || "$DB_ID" == *$'\t'* ]]; then
    error "Hay 0 o más de una instancia RDS. Indica cuál usar:"
    error "    DB_ID=mi-instancia ./apagar.sh"
    exit 1
  fi
fi

ESTADO_DB="$($AWS rds describe-db-instances --db-instance-identifier "$DB_ID" \
  --query "DBInstances[0].DBInstanceStatus" --output text)"
echo "    RDS               : $DB_ID — $ESTADO_DB"
echo

confirmar "${1:-}"
echo

# Se guarda el endpoint para compararlo al prender (ver prender.sh).
# Si el entorno ya estaba en 0 instancias, EndpointURL viene vacío o 'None':
# en ese caso se conserva el valor guardado antes, no se pisa con basura.
if [[ -n "$ENDPOINT" && "$ENDPOINT" != "None" ]]; then
  echo "$ENDPOINT" > "$ESTADO"
elif [[ -f "$ESTADO" ]]; then
  warn "El entorno no tiene endpoint activo; se conserva el guardado: $(cat "$ESTADO")"
else
  warn "El entorno no tiene endpoint activo y no hay uno guardado."
  warn "Si Cloudflare usa un registro A, anota la IP a mano antes de continuar."
fi

# ─────────────────────────────────────────────────────────────────────
# 1. Elastic Beanstalk → 0 instancias
# ─────────────────────────────────────────────────────────────────────
info "Bajando la capacidad de Elastic Beanstalk a 0 instancias…"

$AWS elasticbeanstalk update-environment \
  --environment-name "$ENV_NAME" \
  --option-settings \
    Namespace=aws:autoscaling:asg,OptionName=MinSize,Value=0 \
    Namespace=aws:autoscaling:asg,OptionName=MaxSize,Value=0 \
  --query "EnvironmentName" --output text > /dev/null

info "Esperando a que se terminen las instancias…"
for _ in $(seq 1 60); do   # hasta ~10 min
  INSTANCIAS="$($AWS elasticbeanstalk describe-environment-resources \
    --environment-name "$ENV_NAME" \
    --query "length(EnvironmentResources.Instances)" --output text)"
  ESTADO_EB="$($AWS elasticbeanstalk describe-environments \
    --environment-names "$ENV_NAME" \
    --query "Environments[0].Status" --output text)"

  if [[ "$INSTANCIAS" == "0" && "$ESTADO_EB" == "Ready" ]]; then
    ok "Elastic Beanstalk en 0 instancias (estado: $ESTADO_EB)."
    break
  fi
  printf "    instancias: %s — estado: %s\r" "$INSTANCIAS" "$ESTADO_EB"
  sleep 10
done
echo

if [[ "${INSTANCIAS:-1}" != "0" ]]; then
  warn "El entorno todavía reporta $INSTANCIAS instancia(s)."
  warn "Revisa la consola de EB antes de continuar; RDS no se detuvo."
  exit 1
fi

# ─────────────────────────────────────────────────────────────────────
# 2. RDS → detenida
# ─────────────────────────────────────────────────────────────────────
if [[ "$ESTADO_DB" == "stopped" ]]; then
  ok "RDS '$DB_ID' ya estaba detenida."
else
  info "Deteniendo la base de datos '$DB_ID'…"
  $AWS rds stop-db-instance --db-instance-identifier "$DB_ID" \
    --query "DBInstance.DBInstanceIdentifier" --output text > /dev/null

  info "Esperando a que quede detenida…"
  for _ in $(seq 1 60); do   # hasta ~10 min
    ESTADO_DB="$($AWS rds describe-db-instances --db-instance-identifier "$DB_ID" \
      --query "DBInstances[0].DBInstanceStatus" --output text)"
    if [[ "$ESTADO_DB" == "stopped" ]]; then
      ok "RDS detenida."
      break
    fi
    printf "    estado: %s\r" "$ESTADO_DB"
    sleep 10
  done
  echo
  [[ "$ESTADO_DB" == "stopped" ]] || warn "RDS quedó en estado '$ESTADO_DB'; verifica en la consola."
fi

# ─────────────────────────────────────────────────────────────────────
# Resumen
# ─────────────────────────────────────────────────────────────────────
echo
ok "Entorno pausado. Ya no se paga cómputo (el almacenamiento sigue corriendo)."
echo
warn "Recordatorios mientras esté apagado:"
echo "    • No hagas push a main: el pipeline intentaría desplegar a un entorno sin instancias."
echo "    • AWS reinicia sola una instancia RDS detenida a los 7 días."
echo "    • Endpoint guardado: $ENDPOINT"
echo
echo "Para volver a prenderlo:  ./prender.sh"
