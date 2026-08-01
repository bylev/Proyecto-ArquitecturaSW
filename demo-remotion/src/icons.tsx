import React from "react";

type IconProps = { size?: number; color?: string };
const base = (size: number) => ({
  width: size,
  height: size,
  viewBox: "0 0 24 24",
  fill: "none",
  stroke: "currentColor",
  strokeWidth: 1.9,
  strokeLinecap: "round" as const,
  strokeLinejoin: "round" as const,
});

export const TruckIcon: React.FC<IconProps> = ({ size = 34, color = "currentColor" }) => (
  <svg {...base(size)} style={{ color }}>
    <path d="M2 5h11v11H2z" />
    <path d="M13 8h4l4 4v4h-8z" />
    <circle cx="6.5" cy="18" r="2" />
    <circle cx="17.5" cy="18" r="2" />
  </svg>
);

export const UsersIcon: React.FC<IconProps> = ({ size = 34, color = "currentColor" }) => (
  <svg {...base(size)} style={{ color }}>
    <circle cx="9" cy="8" r="3.2" />
    <path d="M3 20c0-3.3 2.7-6 6-6s6 2.7 6 6" />
    <path d="M16 6.5a3 3 0 0 1 0 5.8M21 20c0-2.5-1.4-4.6-3.4-5.6" />
  </svg>
);

export const BadgeIcon: React.FC<IconProps> = ({ size = 34, color = "currentColor" }) => (
  <svg {...base(size)} style={{ color }}>
    <rect x="4" y="3" width="16" height="18" rx="2.5" />
    <circle cx="12" cy="9" r="2.6" />
    <path d="M8 17c.6-2 2.1-3 4-3s3.4 1 4 3" />
  </svg>
);

export const ContainerIcon: React.FC<IconProps> = ({ size = 34, color = "currentColor" }) => (
  <svg {...base(size)} style={{ color }}>
    <rect x="3" y="7" width="18" height="11" rx="1.5" />
    <path d="M7 7v11M12 7v11M17 7v11" />
  </svg>
);

export const ChartIcon: React.FC<IconProps> = ({ size = 34, color = "currentColor" }) => (
  <svg {...base(size)} style={{ color }}>
    <path d="M4 20V4M4 20h16" />
    <rect x="7" y="12" width="3" height="5" />
    <rect x="12" y="8" width="3" height="9" />
    <rect x="17" y="5" width="3" height="12" />
  </svg>
);

export const SparkIcon: React.FC<IconProps> = ({ size = 34, color = "currentColor" }) => (
  <svg {...base(size)} style={{ color }}>
    <path d="M12 3l1.9 5.1L19 10l-5.1 1.9L12 17l-1.9-5.1L5 10l5.1-1.9z" />
    <path d="M19 15l.8 2.2L22 18l-2.2.8L19 21l-.8-2.2L16 18l2.2-.8z" />
  </svg>
);

export const CheckIcon: React.FC<IconProps> = ({ size = 34, color = "currentColor" }) => (
  <svg {...base(size)} style={{ color }}>
    <path d="M20 6L9 17l-5-5" />
  </svg>
);
