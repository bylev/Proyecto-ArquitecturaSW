# Arquitectura de Transportes GGP

## Diagrama Modelo C4

El modelo C4 es una forma de representar la arquitectura de un sistema de software. En donde, el primer nivel es el contexto del sistema, quién lo usa y qué es. El siguiente nivel, es el contenedor (aplicaciones, bases de datos, etc.), el tercero son los componentes, y el cuarto son el código.

## Nivel 1

```graph TD
    admin["🧑‍💼 Administrador<br/><small>Gestiona clientes, operadores,<br/>unidades y servicios</small>"]
    consulta["🧑 Usuario de Consulta<br/><small>Consulta servicios y catálogos</small>"]
    transggp["🖥️ TransGGP<br/><small>Sistema web para gestionar<br/>el transporte de carga</small>"]
    db[("🗄️ MySQL<br/><small>Base de datos</small>")]
    admin -->|Usa - Navegador web| transggp
    consulta -->|Usa - Navegador web| transggp
    transggp -->|Lee y escribe| db
    style admin fill:#08427b,stroke:#052e56,color:#fff
    style consulta fill:#08427b,stroke:#052e56,color:#fff
    style transggp fill:#1168bd,stroke:#0b4884,color:#fff
    style db fill:#438dd5,stroke:#2e6295,color:#fff
```
