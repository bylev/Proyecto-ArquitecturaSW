import React from "react";
import { AbsoluteFill, Sequence, useCurrentFrame, interpolate } from "remotion";
import {
  SceneHero,
  SceneModulos,
  SceneDashboard,
  SceneServicios,
  SceneAsistente,
  SceneOutro,
} from "./scenes";

// Duración de cada escena (frames @ 30fps)
export const SCENES = [
  { comp: SceneHero, dur: 100 },
  { comp: SceneModulos, dur: 130 },
  { comp: SceneDashboard, dur: 155 },
  { comp: SceneServicios, dur: 140 },
  { comp: SceneAsistente, dur: 120 },
  { comp: SceneOutro, dur: 150 },
];

const FADE = 14; // frames de cruce entre escenas

// Total descontando los solapamientos entre escenas.
export const DEMO_DURATION =
  SCENES.reduce((a, s) => a + s.dur, 0) - (SCENES.length - 1) * FADE;

/** Envuelve una escena con fade-in / fade-out para transiciones suaves. */
const Fade: React.FC<{ dur: number; children: React.ReactNode }> = ({ dur, children }) => {
  const frame = useCurrentFrame();
  const opacity = interpolate(
    frame,
    [0, FADE, dur - FADE, dur],
    [0, 1, 1, 0],
    { extrapolateLeft: "clamp", extrapolateRight: "clamp" },
  );
  return <AbsoluteFill style={{ opacity }}>{children}</AbsoluteFill>;
};

export const DemoGGP: React.FC = () => {
  let from = 0;
  return (
    <AbsoluteFill style={{ backgroundColor: "#0B1224" }}>
      {SCENES.map((s, i) => {
        const Comp = s.comp;
        const el = (
          <Sequence key={i} from={from} durationInFrames={s.dur}>
            <Fade dur={s.dur}>
              <Comp />
            </Fade>
          </Sequence>
        );
        from += s.dur - FADE; // solapa las escenas para que el cruce sea continuo
        return el;
      })}
    </AbsoluteFill>
  );
};
