# Arquitectura de Transportes GGP

## Diagrama Modelo C4

El modelo C4 es una forma de representar la arquitectura de un sistema de software. En donde, el primer nivel es el contexto del sistema, quién lo usa y qué es. El siguiente nivel, es el contenedor (aplicaciones, bases de datos, etc.), el tercero son los componentes, y el cuarto son el código.

## Nivel 1

```mermaid
C4Context
    title TransGGP — Contexto del Sistema

    %% Definición de Personas en la parte superior
    Person(admin, "Administrador", "Gestiona clientes, operadores,<br/>unidades y servicios.")
    %% Sistema principal abajo
    System(transggp, "TransGGP", "Sistema web para gestionar<br/>el transporte de carga.")

    Person(consulta, "Usuario de Consulta", "Consulta servicios<br/>y catálogos.")

    %% Relaciones apuntando hacia abajo (Rel_D = Down) para forzar un diseño vertical limpio
    Rel_D(admin, transggp, "Usa", "Navegador web")
    Rel_D(consulta, transggp, "Usa", "Navegador web")
```