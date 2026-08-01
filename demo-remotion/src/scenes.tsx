import React from "react";
import {
  AbsoluteFill,
  Easing,
  interpolate,
  spring,
  staticFile,
  useCurrentFrame,
  useVideoConfig,
} from "remotion";
import { QRCodeSVG } from "qrcode.react";
import { theme } from "./theme";
import {
  Background,
  Counter,
  LogoBadge,
  Reveal,
  StatusBadge,
} from "./components";
import {
  BadgeIcon,
  ChartIcon,
  CheckIcon,
  ContainerIcon,
  SparkIcon,
  TruckIcon,
  UsersIcon,
} from "./icons";

const EASE = Easing.bezier(0.16, 1, 0.3, 1);

const Eyebrow: React.FC<{ children: React.ReactNode; dark?: boolean }> = ({
  children,
  dark,
}) => (
  <div
    style={{
      color: theme.gold,
      fontWeight: 900,
      fontSize: 26,
      letterSpacing: 6,
      textTransform: "uppercase",
      opacity: dark ? 0.9 : 1,
    }}
  >
    {children}
  </div>
);

/* ───────────────────────── 1 · HERO ───────────────────────── */
export const SceneHero: React.FC = () => {
  const frame = useCurrentFrame();
  const line = interpolate(frame, [22, 45], [0, 320], {
    extrapolateLeft: "clamp",
    extrapolateRight: "clamp",
    easing: EASE,
  });
  return (
    <AbsoluteFill
      style={{
        fontFamily: theme.font,
        alignItems: "center",
        justifyContent: "center",
      }}
    >
      <Background variant="dark" />
      <div style={{ textAlign: "center", padding: 80 }}>
        <Reveal delay={0}>
          <div style={{ display: "flex", justifyContent: "center", marginBottom: 40 }}>
            <LogoBadge size={168} delay={4} />
          </div>
        </Reveal>
        <Reveal delay={12}>
          <Eyebrow>Sistema de gestión de transporte</Eyebrow>
        </Reveal>
        <Reveal delay={20}>
          <h1
            style={{
              margin: "18px 0 0",
              color: theme.white,
              fontWeight: 900,
              fontSize: 122,
              lineHeight: 1.02,
              letterSpacing: -2,
            }}
          >
            Cada viaje,
            <br />
            <span style={{ color: theme.gold }}>bajo control.</span>
          </h1>
        </Reveal>
        <div
          style={{
            height: 6,
            width: line,
            backgroundColor: theme.gold,
            borderRadius: 99,
            margin: "40px auto 0",
          }}
        />
      </div>
    </AbsoluteFill>
  );
};

/* ─────────────────────── 2 · MÓDULOS ──────────────────────── */
const modules = [
  { icon: TruckIcon, title: "Servicios", desc: "Cada embarque de carga, de origen a destino." },
  { icon: UsersIcon, title: "Clientes", desc: "Directorio y contacto de tu cartera." },
  { icon: BadgeIcon, title: "Operadores", desc: "Choferes asignados a cada viaje." },
  { icon: ContainerIcon, title: "Unidades", desc: "Tractos, semirremolques y dollys." },
];

export const SceneModulos: React.FC = () => {
  const { fps } = useVideoConfig();
  const frame = useCurrentFrame();
  return (
    <AbsoluteFill style={{ fontFamily: theme.font }}>
      <Background variant="light" />
      <AbsoluteFill style={{ padding: "90px 100px" }}>
        <Reveal delay={0}>
          <Eyebrow>Todo en un solo lugar</Eyebrow>
        </Reveal>
        <Reveal delay={6}>
          <h2
            style={{
              margin: "14px 0 0",
              color: theme.ink,
              fontSize: 76,
              fontWeight: 900,
              letterSpacing: -1.5,
            }}
          >
            Administra tu operación completa
          </h2>
        </Reveal>
        <div
          style={{
            display: "grid",
            gridTemplateColumns: "1fr 1fr",
            gap: 34,
            marginTop: 60,
          }}
        >
          {modules.map((m, i) => {
            const s = spring({
              frame: frame - (18 + i * 8),
              fps,
              config: { damping: 200 },
            });
            const Icon = m.icon;
            return (
              <div
                key={m.title}
                style={{
                  opacity: s,
                  translate: `0px ${interpolate(s, [0, 1], [50, 0])}px`,
                  backgroundColor: theme.white,
                  borderRadius: 26,
                  padding: "34px 38px",
                  display: "flex",
                  alignItems: "center",
                  gap: 28,
                  boxShadow: `0 20px 45px ${theme.navy}12`,
                  border: `1px solid ${theme.line}`,
                }}
              >
                <div
                  style={{
                    width: 88,
                    height: 88,
                    borderRadius: 22,
                    backgroundColor: `${theme.gold}22`,
                    color: theme.navy,
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "center",
                    flexShrink: 0,
                  }}
                >
                  <Icon size={44} />
                </div>
                <div>
                  <div style={{ fontSize: 40, fontWeight: 900, color: theme.ink }}>
                    {m.title}
                  </div>
                  <div style={{ fontSize: 27, color: theme.muted, marginTop: 4 }}>
                    {m.desc}
                  </div>
                </div>
              </div>
            );
          })}
        </div>
      </AbsoluteFill>
    </AbsoluteFill>
  );
};

/* ────────────────────── 3 · DASHBOARD ─────────────────────── */
const meses = [
  { m: "Ene", v: 42 }, { m: "Feb", v: 55 }, { m: "Mar", v: 61 },
  { m: "Abr", v: 48 }, { m: "May", v: 73 }, { m: "Jun", v: 80 },
  { m: "Jul", v: 92 }, { m: "Ago", v: 76 }, { m: "Sep", v: 88 },
  { m: "Oct", v: 97 }, { m: "Nov", v: 84 }, { m: "Dic", v: 68 },
];
const maxV = Math.max(...meses.map((x) => x.v));

const KpiTile: React.FC<{
  label: string;
  to: number;
  suffix?: string;
  decimals?: number;
  delay: number;
  accent: string;
}> = ({ label, to, suffix, decimals, delay, accent }) => (
  <Reveal delay={delay} style={{ flex: 1 }}>
    <div
      style={{
        backgroundColor: theme.white,
        borderRadius: 24,
        padding: "30px 34px",
        border: `1px solid ${theme.line}`,
        boxShadow: `0 18px 40px ${theme.navy}10`,
        borderTop: `5px solid ${accent}`,
      }}
    >
      <div style={{ fontSize: 24, color: theme.muted, fontWeight: 700 }}>{label}</div>
      <div style={{ fontSize: 78, fontWeight: 900, color: theme.ink, lineHeight: 1.1 }}>
        <Counter to={to} suffix={suffix} decimals={decimals} delay={delay + 6} />
      </div>
    </div>
  </Reveal>
);

export const SceneDashboard: React.FC = () => {
  const frame = useCurrentFrame();
  return (
    <AbsoluteFill style={{ fontFamily: theme.font }}>
      <Background variant="light" />
      <AbsoluteFill style={{ padding: "84px 100px" }}>
        <Reveal delay={0}>
          <div style={{ display: "flex", alignItems: "center", gap: 16 }}>
            <ChartIcon size={40} color={theme.gold} />
            <Eyebrow>Panel de control</Eyebrow>
          </div>
        </Reveal>
        <Reveal delay={5}>
          <h2
            style={{
              margin: "12px 0 0",
              color: theme.ink,
              fontSize: 72,
              fontWeight: 900,
              letterSpacing: -1.5,
            }}
          >
            Mide el crecimiento del negocio
          </h2>
        </Reveal>

        <div style={{ display: "flex", gap: 28, marginTop: 44 }}>
          <KpiTile label="Servicios este año" to={862} delay={14} accent={theme.navy} />
          <KpiTile label="% Entregados" to={96} suffix="%" delay={20} accent={theme.entregado} />
          <KpiTile label="Clientes activos" to={48} delay={26} accent={theme.gold} />
        </div>

        {/* Gráfica mensual */}
        <Reveal delay={30} style={{ marginTop: 40 }}>
          <div
            style={{
              backgroundColor: theme.white,
              borderRadius: 24,
              padding: "30px 40px 24px",
              border: `1px solid ${theme.line}`,
              boxShadow: `0 18px 40px ${theme.navy}10`,
            }}
          >
            <div style={{ fontSize: 26, fontWeight: 700, color: theme.ink, marginBottom: 20 }}>
              Servicios por mes
            </div>
            <div
              style={{
                display: "flex",
                alignItems: "flex-end",
                gap: 18,
                height: 240,
              }}
            >
              {meses.map((mo, i) => {
                const grow = interpolate(
                  frame - (40 + i * 3),
                  [0, 22],
                  [0, (mo.v / maxV) * 210],
                  { extrapolateLeft: "clamp", extrapolateRight: "clamp", easing: EASE },
                );
                const isPeak = mo.v === maxV;
                return (
                  <div key={mo.m} style={{ flex: 1, textAlign: "center" }}>
                    <div
                      style={{
                        height: grow,
                        borderRadius: 10,
                        background: isPeak
                          ? `linear-gradient(${theme.goldSoft}, ${theme.gold})`
                          : `linear-gradient(${theme.navy}, #2C4270)`,
                      }}
                    />
                    <div style={{ fontSize: 20, color: theme.muted, marginTop: 10 }}>
                      {mo.m}
                    </div>
                  </div>
                );
              })}
            </div>
          </div>
        </Reveal>
      </AbsoluteFill>
    </AbsoluteFill>
  );
};

/* ─────────────────────── 4 · SERVICIOS ────────────────────── */
const filas = [
  { emb: "EMB-2048", cli: "Cementos del Norte", op: "J. Ramírez", ruta: "Monterrey → CDMX", est: "Entregado", color: theme.entregado },
  { emb: "EMB-2049", cli: "AgroExport S.A.", op: "L. Hernández", ruta: "Culiacán → Guadalajara", est: "En proceso", color: theme.enProceso },
  { emb: "EMB-2050", cli: "Acero Industrial", op: "M. Torres", ruta: "Saltillo → Querétaro", est: "Agendado", color: theme.agendado },
  { emb: "EMB-2051", cli: "Distribuidora Bajío", op: "R. Salas", ruta: "León → Puebla", est: "Entregado", color: theme.entregado },
];

export const SceneServicios: React.FC = () => {
  const { fps } = useVideoConfig();
  const frame = useCurrentFrame();
  const cols = "1.1fr 2fr 1.6fr 2.2fr 1.4fr";
  return (
    <AbsoluteFill style={{ fontFamily: theme.font }}>
      <Background variant="dark" />
      <AbsoluteFill style={{ padding: "84px 100px" }}>
        <Reveal delay={0}>
          <Eyebrow>Historial confiable</Eyebrow>
        </Reveal>
        <Reveal delay={5}>
          <h2
            style={{
              margin: "12px 0 0",
              color: theme.white,
              fontSize: 72,
              fontWeight: 900,
              letterSpacing: -1.5,
            }}
          >
            Cada servicio, registrado y rastreable
          </h2>
        </Reveal>

        <Reveal delay={12} style={{ marginTop: 46 }}>
          <div
            style={{
              backgroundColor: theme.white,
              borderRadius: 24,
              overflow: "hidden",
              boxShadow: `0 24px 60px ${theme.navyDeep}66`,
            }}
          >
            <div
              style={{
                display: "grid",
                gridTemplateColumns: cols,
                padding: "22px 34px",
                backgroundColor: theme.light,
                fontSize: 24,
                fontWeight: 900,
                color: theme.muted,
                letterSpacing: 1,
                textTransform: "uppercase",
              }}
            >
              <div>Embarque</div>
              <div>Cliente</div>
              <div>Operador</div>
              <div>Ruta</div>
              <div>Estatus</div>
            </div>
            {filas.map((f, i) => {
              const s = spring({
                frame: frame - (22 + i * 9),
                fps,
                config: { damping: 200 },
              });
              return (
                <div
                  key={f.emb}
                  style={{
                    opacity: s,
                    translate: `${interpolate(s, [0, 1], [40, 0])}px 0px`,
                    display: "grid",
                    gridTemplateColumns: cols,
                    alignItems: "center",
                    padding: "26px 34px",
                    borderTop: `1px solid ${theme.line}`,
                    fontSize: 28,
                    color: theme.ink,
                  }}
                >
                  <div style={{ fontWeight: 900, color: theme.navy }}>{f.emb}</div>
                  <div style={{ fontWeight: 700 }}>{f.cli}</div>
                  <div style={{ color: theme.muted }}>{f.op}</div>
                  <div style={{ color: theme.muted }}>{f.ruta}</div>
                  <div>
                    <StatusBadge label={f.est} color={f.color} delay={30 + i * 9} />
                  </div>
                </div>
              );
            })}
          </div>
        </Reveal>
      </AbsoluteFill>
    </AbsoluteFill>
  );
};

/* ─────────────────────── 5 · ASISTENTE ────────────────────── */
export const SceneAsistente: React.FC = () => {
  const frame = useCurrentFrame();
  const bars = [0.9, 0.55, 0.75];
  return (
    <AbsoluteFill
      style={{ fontFamily: theme.font, alignItems: "center", justifyContent: "center" }}
    >
      <Background variant="light" />
      <div style={{ display: "flex", gap: 70, alignItems: "center", padding: "0 110px" }}>
        <div style={{ flex: 1 }}>
          <Reveal delay={0}>
            <div style={{ display: "flex", alignItems: "center", gap: 14 }}>
              <SparkIcon size={40} color={theme.gold} />
              <Eyebrow>Inteligencia del negocio</Eyebrow>
            </div>
          </Reveal>
          <Reveal delay={6}>
            <h2
              style={{
                margin: "16px 0 0",
                color: theme.ink,
                fontSize: 78,
                fontWeight: 900,
                letterSpacing: -1.5,
                lineHeight: 1.05,
              }}
            >
              Asistente de análisis
            </h2>
          </Reveal>
          <Reveal delay={12}>
            <p style={{ fontSize: 34, color: theme.muted, lineHeight: 1.4, marginTop: 22 }}>
              Genera reportes y descubre tendencias de tus servicios,
              rutas y clientes — en segundos.
            </p>
          </Reveal>
        </div>

        {/* Tarjeta del asistente */}
        <Reveal delay={16} style={{ flex: 1 }}>
          <div
            style={{
              backgroundColor: theme.navy,
              borderRadius: 28,
              padding: 40,
              boxShadow: `0 30px 70px ${theme.navy}44`,
            }}
          >
            <div style={{ display: "flex", alignItems: "center", gap: 14, marginBottom: 28 }}>
              <div
                style={{
                  width: 54,
                  height: 54,
                  borderRadius: 16,
                  backgroundColor: theme.gold,
                  color: theme.navy,
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "center",
                }}
              >
                <SparkIcon size={30} />
              </div>
              <div style={{ color: theme.white, fontWeight: 900, fontSize: 30 }}>
                Resumen del mes
              </div>
            </div>

            {["Servicios entregados a tiempo", "Cliente con mayor volumen", "Ruta más rentable"].map(
              (t, i) => {
                const o = interpolate(frame - (26 + i * 10), [0, 10], [0, 1], {
                  extrapolateLeft: "clamp",
                  extrapolateRight: "clamp",
                });
                const w = interpolate(frame - (30 + i * 10), [0, 24], [0, bars[i] * 100], {
                  extrapolateLeft: "clamp",
                  extrapolateRight: "clamp",
                  easing: EASE,
                });
                return (
                  <div key={t} style={{ opacity: o, marginBottom: 22 }}>
                    <div
                      style={{
                        display: "flex",
                        alignItems: "center",
                        gap: 10,
                        color: "#C7D2E8",
                        fontSize: 25,
                        marginBottom: 10,
                      }}
                    >
                      <CheckIcon size={22} color={theme.gold} />
                      {t}
                    </div>
                    <div
                      style={{
                        height: 14,
                        borderRadius: 99,
                        backgroundColor: "#243B5B",
                      }}
                    >
                      <div
                        style={{
                          width: `${w}%`,
                          height: "100%",
                          borderRadius: 99,
                          background: `linear-gradient(90deg, ${theme.gold}, ${theme.goldSoft})`,
                        }}
                      />
                    </div>
                  </div>
                );
              },
            )}
          </div>
        </Reveal>
      </div>
    </AbsoluteFill>
  );
};

/* ───────────────────────── 6 · OUTRO + QR ─────────────────── */
const SITE_URL = "https://www.transportesggp.com/";

export const SceneOutro: React.FC = () => {
  const frame = useCurrentFrame();
  const { fps } = useVideoConfig();
  const glow = interpolate(frame, [0, 40], [0, 1], {
    extrapolateLeft: "clamp",
    extrapolateRight: "clamp",
  });
  const qrPop = spring({ frame: frame - 24, fps, config: { damping: 200 } });
  return (
    <AbsoluteFill
      style={{
        fontFamily: theme.font,
        alignItems: "center",
        justifyContent: "center",
      }}
    >
      <Background variant="dark" />
      <div style={{ textAlign: "center" }}>
        <Reveal delay={0}>
          <div style={{ display: "flex", justifyContent: "center", marginBottom: 28 }}>
            <LogoBadge size={118} delay={2} />
          </div>
        </Reveal>
        <Reveal delay={10}>
          <h2
            style={{
              margin: 0,
              color: theme.white,
              fontSize: 78,
              fontWeight: 900,
              letterSpacing: -1.5,
            }}
          >
            Cada viaje, <span style={{ color: theme.gold }}>bajo control.</span>
          </h2>
        </Reveal>

        {/* Tarjeta con QR */}
        <div
          style={{
            marginTop: 40,
            display: "flex",
            flexDirection: "column",
            alignItems: "center",
            opacity: qrPop,
            scale: String(interpolate(qrPop, [0, 1], [0.85, 1])),
            translate: `0px ${interpolate(qrPop, [0, 1], [30, 0])}px`,
          }}
        >
          <div
            style={{
              backgroundColor: theme.white,
              borderRadius: 28,
              padding: 26,
              boxShadow: `0 24px 60px ${theme.navyDeep}, 0 0 0 4px ${theme.gold}`,
            }}
          >
            <QRCodeSVG
              value={SITE_URL}
              size={264}
              level="H"
              bgColor={theme.white}
              fgColor={theme.navy}
              imageSettings={{
                src: staticFile("logo.png"),
                height: 62,
                width: 62,
                excavate: true,
              }}
            />
          </div>
          <div
            style={{
              marginTop: 20,
              color: "#C7D2E8",
              fontSize: 30,
              fontWeight: 700,
              letterSpacing: 1,
            }}
          >
            Escanea para visitarnos
          </div>
        </div>

        <Reveal delay={30}>
          <div
            style={{
              marginTop: 24,
              display: "inline-flex",
              alignItems: "center",
              gap: 14,
              padding: "16px 40px",
              borderRadius: 999,
              backgroundColor: theme.gold,
              color: theme.navy,
              fontSize: 36,
              fontWeight: 900,
              boxShadow: `0 0 ${40 * glow}px ${theme.gold}88`,
            }}
          >
            transportesggp.com
          </div>
        </Reveal>
      </div>
    </AbsoluteFill>
  );
};
