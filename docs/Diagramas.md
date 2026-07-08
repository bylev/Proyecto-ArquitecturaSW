# Arquitectura de Transportes GGP

## Diagrama Modelo C4

El modelo C4 es una forma de representar la arquitectura de un sistema de software. En donde, el primer nivel es el contexto del sistema, quién lo usa y qué es. El siguiente nivel, es el contenedor (aplicaciones, bases de datos, etc.), el tercero son los componentes, y el cuarto son el código.

## Nivel 1

```mermaid
C4Context
    title TransGGP — Contexto del Sistema

    Person(admin, "Administrador", "Usa el sistema para gestionar clientes, operadores, unidades y servicios.")
    Person(consulta, "Usuario de Consulta", "Consulta servicios y catálogos.")

    System(transggp, "TransGGP", "Sistema web para gestionar el transporte de carga.")

    SystemDb(db, "MySQL", "Guarda toda la información del sistema.")

    Rel(admin, transggp, "Usa", "Navegador web")
    Rel(consulta, transggp, "Usa", "Navegador web")
    Rel(transggp, db, "Lee y escribe", "TCP/IP")
```