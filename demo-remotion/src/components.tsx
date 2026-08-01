import React from "react";
import {
  AbsoluteFill,
  Easing,
  Img,
  interpolate,
  spring,
  staticFile,
  useCurrentFrame,
  useVideoConfig,
} from "remotion";
import { theme } from "./theme";

const EASE_OUT = Easing.bezier(0.16, 1, 0.3, 1);

/** Entra desde abajo con fade, controlado por spring. */
export const Reveal: React.FC<{
  children: React.ReactNode;
  delay?: number;
  y?: number;
  style?: React.CSSProperties;
}> = ({ children, delay = 0, y = 40, style }) => {
  const frame = useCurrentFrame();
  const { fps } = useVideoConfig();
  const s = spring({ frame: frame - delay, fps, config: { damping: 200 } });
  return (
    <div
      style={{
        opacity: s,
        translate: `0px ${interpolate(s, [0, 1], [y, 0])}px`,
        ...style,
      }}
    >
      {children}
    </div>
  );
};

/** Fondo con gradiente de marca + textura sutil de "rutas". */
export const Background: React.FC<{ variant?: "dark" | "light" }> = ({
  variant = "dark",
}) => {
  const frame = useCurrentFrame();
  const drift = interpolate(frame, [0, 300], [0, -60]);
  if (variant === "light") {
    return (
      <AbsoluteFill style={{ backgroundColor: theme.light }}>
        <AbsoluteFill
          style={{
            background: `radial-gradient(1200px 600px at 85% -10%, ${theme.gold}22, transparent 60%)`,
          }}
        />
        <RouteLines color={theme.navy} opacity={0.05} drift={drift} />
      </AbsoluteFill>
    );
  }
  return (
    <AbsoluteFill
      style={{
        background: `linear-gradient(160deg, ${theme.navyDeep} 0%, ${theme.navy} 55%, #1B2A4E 100%)`,
      }}
    >
      <AbsoluteFill
        style={{
          background: `radial-gradient(900px 500px at 80% 0%, ${theme.gold}22, transparent 55%)`,
        }}
      />
      <RouteLines color={theme.gold} opacity={0.12} drift={drift} />
    </AbsoluteFill>
  );
};

/** Líneas diagonales que evocan carreteras/rutas de carga. */
const RouteLines: React.FC<{ color: string; opacity: number; drift: number }> = ({
  color,
  opacity,
  drift,
}) => (
  <AbsoluteFill style={{ opacity }}>
    <svg width="100%" height="100%" style={{ translate: `${drift}px 0px` }}>
      {Array.from({ length: 14 }).map((_, i) => (
        <line
          key={i}
          x1={-200 + i * 180}
          y1={-100}
          x2={200 + i * 180}
          y2={1200}
          stroke={color}
          strokeWidth={2}
          strokeDasharray="2 26"
          strokeLinecap="round"
        />
      ))}
    </svg>
  </AbsoluteFill>
);

/** Logo GGP con anillo dorado animado. */
export const LogoBadge: React.FC<{ size?: number; delay?: number }> = ({
  size = 150,
  delay = 0,
}) => {
  const frame = useCurrentFrame();
  const { fps } = useVideoConfig();
  const pop = spring({ frame: frame - delay, fps, config: { damping: 12, mass: 0.7 } });
  return (
    <div
      style={{
        width: size,
        height: size,
        borderRadius: size * 0.28,
        backgroundColor: theme.white,
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        boxShadow: `0 24px 60px ${theme.navyDeep}88, 0 0 0 6px ${theme.gold}`,
        scale: String(pop),
        overflow: "hidden",
      }}
    >
      <Img
        src={staticFile("logo.png")}
        style={{ width: "76%", height: "76%", objectFit: "contain" }}
      />
    </div>
  );
};

/** Número que cuenta desde 0 hasta el valor final. */
export const Counter: React.FC<{
  to: number;
  delay?: number;
  duration?: number;
  suffix?: string;
  decimals?: number;
}> = ({ to, delay = 0, duration = 45, suffix = "", decimals = 0 }) => {
  const frame = useCurrentFrame();
  const v = interpolate(frame - delay, [0, duration], [0, to], {
    extrapolateLeft: "clamp",
    extrapolateRight: "clamp",
    easing: EASE_OUT,
  });
  return (
    <span>
      {v.toLocaleString("es-MX", {
        minimumFractionDigits: decimals,
        maximumFractionDigits: decimals,
      })}
      {suffix}
    </span>
  );
};

export const StatusBadge: React.FC<{
  label: string;
  color: string;
  delay?: number;
}> = ({ label, color, delay = 0 }) => {
  const frame = useCurrentFrame();
  const o = interpolate(frame - delay, [0, 8], [0, 1], {
    extrapolateLeft: "clamp",
    extrapolateRight: "clamp",
  });
  return (
    <span
      style={{
        display: "inline-flex",
        alignItems: "center",
        gap: 10,
        padding: "8px 18px",
        borderRadius: 999,
        backgroundColor: `${color}1F`,
        color,
        fontWeight: 700,
        fontSize: 26,
        opacity: o,
      }}
    >
      <span
        style={{ width: 12, height: 12, borderRadius: 99, backgroundColor: color }}
      />
      {label}
    </span>
  );
};
