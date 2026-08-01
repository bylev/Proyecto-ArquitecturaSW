using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TransGGP.Domain.Models;
using TransGGP.Application.Dashboards;

namespace TransGGP.Application.Interfaces
{
    public class ReporteArchivo
    {
        public byte[] Contenido { get; set; } = Array.Empty<byte>();
        public string TipoContenido { get; set; } = string.Empty;
        public string NombreArchivo { get; set; } = string.Empty;
    }

    public interface IReporteCreator
    {
        ReporteArchivo Generar();
    }

    public static class ColoresReporte
    {
        public const string Navy = "#1b2a4a";
        public const string Gold = "#b08d4f";
        public const string Cream = "#f7f5f0";
        public const string Blanco = "#ffffff";
    }

    public class ReporteClientesPdf : IReporteCreator
    {
        private readonly List<Cliente> _clientes;
        private readonly byte[]? _logo;

        public ReporteClientesPdf(List<Cliente> clientes, byte[]? logo)
        {
            _clientes = clientes;
            _logo = logo;
        }

        public ReporteArchivo Generar()
        {
            var documento = Document.Create(doc =>
            {
                doc.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header().Element(e => Encabezado(e, _logo, "Reporte de Clientes", $"Total: {_clientes.Count} clientes"));

                    page.Content().PaddingVertical(12).Table(tabla =>
                    {
                        tabla.ColumnsDefinition(columnas =>
                        {
                            columnas.ConstantColumn(50);
                            columnas.RelativeColumn();
                            columnas.ConstantColumn(130);
                        });

                        tabla.Header(encabezado =>
                        {
                            encabezado.Cell().Background(ColoresReporte.Navy).Padding(6).Text("Id").FontColor(ColoresReporte.Blanco).Bold();
                            encabezado.Cell().Background(ColoresReporte.Navy).Padding(6).Text("Nombre").FontColor(ColoresReporte.Blanco).Bold();
                            encabezado.Cell().Background(ColoresReporte.Navy).Padding(6).Text("Fecha de creación").FontColor(ColoresReporte.Blanco).Bold();
                        });

                        var alterno = false;
                        foreach (var c in _clientes)
                        {
                            var fondo = alterno ? ColoresReporte.Cream : ColoresReporte.Blanco;
                            tabla.Cell().Background(fondo).Padding(6).Text(c.Id.ToString());
                            tabla.Cell().Background(fondo).Padding(6).Text(c.Nombre);
                            tabla.Cell().Background(fondo).Padding(6).Text(c.FechaCreacion.ToString("dd/MM/yyyy"));
                            alterno = !alterno;
                        }
                    });

                    page.Footer().Element(PiePagina);
                });
            });

            return new ReporteArchivo
            {
                Contenido = documento.GeneratePdf(),
                TipoContenido = "application/pdf",
                NombreArchivo = $"Reporte_Clientes_{DateTime.Now:yyyyMMdd}.pdf"
            };
        }

        private static void Encabezado(IContainer contenedor, byte[]? logo, string titulo, string subtitulo)
        {
            ReporteVisual.Encabezado(contenedor, logo, titulo, subtitulo);
        }

        private static void PiePagina(IContainer contenedor)
        {
            ReporteVisual.PiePagina(contenedor);
        }
    }

    public class ReporteServiciosPdf : IReporteCreator
    {
        private readonly DashboardResumen _resumen;
        private readonly List<ServicioReporteFila> _servicios;
        private readonly byte[]? _logo;

        public ReporteServiciosPdf(DashboardResumen resumen, List<ServicioReporteFila> servicios, byte[]? logo)
        {
            _resumen = resumen;
            _servicios = servicios;
            _logo = logo;
        }

        public ReporteArchivo Generar()
        {
            var documento = Document.Create(doc =>
            {
                doc.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Element(e => ReporteVisual.Encabezado(e, _logo, $"Reporte de Servicios {_resumen.Anio}", "Resumen ejecutivo y detalle"));

                    page.Content().PaddingVertical(12).Column(columna =>
                    {
                        columna.Item().Text("Indicadores del año").FontSize(13).Bold().FontColor(ColoresReporte.Navy);
                        columna.Item().PaddingTop(6).Table(tabla =>
                        {
                            tabla.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(); c.RelativeColumn(); c.RelativeColumn();
                                c.RelativeColumn(); c.RelativeColumn(); c.RelativeColumn();
                            });
                            Kpi(tabla, "Total", _resumen.Total);
                            Kpi(tabla, "Entregados", _resumen.Entregados);
                            Kpi(tabla, "En proceso", _resumen.EnProceso);
                            Kpi(tabla, "Agendados", _resumen.Agendados);
                            Kpi(tabla, "Cancelados", _resumen.Cancelados);
                            Kpi(tabla, "% Entregados", _resumen.PorcentajeEntregados);
                        });

                        columna.Item().PaddingTop(16).Text("Resumen mensual").FontSize(13).Bold().FontColor(ColoresReporte.Navy);
                        columna.Item().PaddingTop(6).Table(tabla =>
                        {
                            tabla.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(2); c.RelativeColumn(); c.RelativeColumn();
                                c.RelativeColumn(); c.RelativeColumn(); c.RelativeColumn(); c.RelativeColumn();
                            });
                            tabla.Header(h =>
                            {
                                CeldaTitulo(h, "Mes"); CeldaTitulo(h, "Total"); CeldaTitulo(h, "Entregados");
                                CeldaTitulo(h, "En proceso"); CeldaTitulo(h, "Agendados"); CeldaTitulo(h, "Cancelados"); CeldaTitulo(h, "% Entr.");
                            });
                            var alterno = false;
                            foreach (var m in _resumen.Meses)
                            {
                                var fondo = alterno ? ColoresReporte.Cream : ColoresReporte.Blanco;
                                Celda(tabla, fondo, m.Mes); Celda(tabla, fondo, m.Total.ToString()); Celda(tabla, fondo, m.Entregados.ToString());
                                Celda(tabla, fondo, m.EnProceso.ToString()); Celda(tabla, fondo, m.Agendados.ToString()); Celda(tabla, fondo, m.Cancelados.ToString()); Celda(tabla, fondo, $"{m.PorcentajeEntregados}%");
                                alterno = !alterno;
                            }
                        });

                        columna.Item().PaddingTop(10).Text(texto =>
                        {
                            texto.Span("Histórico: ").Bold().FontColor(ColoresReporte.Navy);
                            texto.Span($"{_resumen.TotalHistorico} servicios · {_resumen.ClientesActivos} clientes activos · Mes más activo: {_resumen.MesMasActivo} · Operador más activo: {_resumen.OperadorMasActivo}");
                        });

                        columna.Item().PaddingTop(16).Text($"Detalle de servicios {_resumen.Anio}").FontSize(13).Bold().FontColor(ColoresReporte.Navy);
                        columna.Item().PaddingTop(6).Table(tabla =>
                        {
                            tabla.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(2); c.RelativeColumn(3); c.RelativeColumn(3);
                                c.RelativeColumn(3); c.RelativeColumn(2); c.RelativeColumn(2);
                            });
                            tabla.Header(h =>
                            {
                                CeldaTitulo(h, "Embarque"); CeldaTitulo(h, "Cliente"); CeldaTitulo(h, "Operador");
                                CeldaTitulo(h, "Ruta"); CeldaTitulo(h, "Fecha carga"); CeldaTitulo(h, "Estatus");
                            });
                            if (_servicios.Count == 0)
                            {
                                tabla.Cell().ColumnSpan(6).Padding(6).Text("No hay servicios registrados en este año.");
                            }
                            else
                            {
                                var alterno = false;
                                foreach (var s in _servicios)
                                {
                                    var fondo = alterno ? ColoresReporte.Cream : ColoresReporte.Blanco;
                                    Celda(tabla, fondo, s.Embarque); Celda(tabla, fondo, s.Cliente); Celda(tabla, fondo, s.Operador);
                                    Celda(tabla, fondo, $"{s.Origen} → {s.Destino}"); Celda(tabla, fondo, s.FechaCarga.ToString("dd/MM/yyyy")); Celda(tabla, fondo, s.Estatus);
                                    alterno = !alterno;
                                }
                            }
                        });
                    });

                    page.Footer().Element(ReporteVisual.PiePagina);
                });
            });

            return new ReporteArchivo
            {
                Contenido = documento.GeneratePdf(),
                TipoContenido = "application/pdf",
                NombreArchivo = $"Reporte_Servicios_{_resumen.Anio}.pdf"
            };
        }

        private static void Kpi(TableDescriptor tabla, string titulo, int valor)
        {
            tabla.Cell().Border(1).BorderColor(ColoresReporte.Cream).Background(ColoresReporte.Navy).Padding(8).Column(col =>
            {
                col.Item().AlignCenter().Text(valor.ToString()).FontSize(16).Bold().FontColor(ColoresReporte.Blanco);
                col.Item().AlignCenter().Text(titulo).FontSize(8).FontColor(ColoresReporte.Blanco);
            });
        }

        private static void CeldaTitulo(TableCellDescriptor descriptor, string texto)
        {
            descriptor.Cell().Background(ColoresReporte.Navy).Padding(5).Text(texto).FontColor(ColoresReporte.Blanco).Bold();
        }

        private static void Celda(TableDescriptor tabla, string fondo, string texto)
        {
            tabla.Cell().Background(fondo).Padding(5).Text(texto);
        }
    }

    public static class ReporteVisual
    {
        public static void Encabezado(IContainer contenedor, byte[]? logo, string titulo, string subtitulo)
        {
            contenedor.Column(columna =>
            {
                columna.Item().Row(fila =>
                {
                    if (logo != null)
                        fila.ConstantItem(90).AlignMiddle().Image(logo).FitWidth();

                    fila.RelativeItem().PaddingLeft(10).Column(texto =>
                    {
                        texto.Item().Text("Transportes GGP").FontSize(20).Bold().FontColor(ColoresReporte.Navy);
                        texto.Item().Text(titulo).FontSize(12).FontColor(ColoresReporte.Gold);
                    });
                });
                columna.Item().PaddingTop(8).LineHorizontal(2).LineColor(ColoresReporte.Gold);
                columna.Item().PaddingTop(4).Text(subtitulo).FontSize(10).FontColor(ColoresReporte.Navy);
            });
        }

        public static void PiePagina(IContainer contenedor)
        {
            contenedor.AlignCenter().Text(texto =>
            {
                texto.Span($"Transportes GGP · Generado el {DateTime.Now:dd/MM/yyyy HH:mm} · Página ").FontSize(9).FontColor(ColoresReporte.Navy);
                texto.CurrentPageNumber().FontSize(9).FontColor(ColoresReporte.Navy);
                texto.Span(" de ").FontSize(9).FontColor(ColoresReporte.Navy);
                texto.TotalPages().FontSize(9).FontColor(ColoresReporte.Navy);
            });
        }
    }
}
