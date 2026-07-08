# Arquitectura de Transportes GGP

## Diagrama Modelo C4

El modelo C4 es una forma de representar la arquitectura de un sistema de software. En donde, el primer nivel es el contexto del sistema, quién lo usa y qué es. El siguiente nivel, es el contenedor (aplicaciones, bases de datos, etc.), el tercero son los componentes, y el cuarto son el código.

## Nivel 1

```mermaid
C4Context
    title TransGGP — Contexto del Sistema

    %% 1. Usar <br/> en las descripciones para evitar cajas demasiado anchas
    Person(admin, "Administrador", "Usa el sistema para gestionar clientes,<br/>operadores, unidades y servicios.")
    
    System(transggp, "TransGGP", "Sistema web para gestionar<br/>el transporte de carga.")
    
    Person(consulta, "Usuario de Consulta", "Consulta servicios<br/>y catálogos.")

    Rel_R(admin, transggp, "Usa", "Navegador web")
    Rel_L(consulta, transggp, "Usa", "Navegador web")
    
    UpdateElementStyle(admin, $bgColor="#f8fea3ff", $fontColor="#2a1a18ff", $borderColor="#757260ff")
    UpdateElementStyle(consulta, $bgColor="#f8fea3ff", $fontColor="#2a1a18ff", $borderColor="#757260ff")
    UpdateElementStyle(transggp, $bgColor="#f8fea3ff", $fontColor="#2a1a18ff", $borderColor="#757260ff")
    UpdateRelStyle(admin, transggp, $textColor="#d3d3d3", $lineColor="#d3d3d3")
    UpdateRelStyle(consulta, transggp, $textColor="#d3d3d3", $lineColor="#d3d3d3")
```