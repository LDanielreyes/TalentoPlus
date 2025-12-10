using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TalentoPlus.Application.Services;
using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Application.Services.Impl;

public class PdfService : IPdfService
{
    public byte[] GenerateWorkerCv(Worker worker)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header()
                    .Text($"Hoja de Vida - {worker.FullName}")
                    .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(x =>
                    {
                        x.Spacing(20);

                        x.Item().Text("Datos Personales").FontSize(16).SemiBold().FontColor(Colors.Grey.Darken2);
                        x.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(150);
                                columns.RelativeColumn();
                            });

                            table.Cell().Text("Nombre Completo:").SemiBold();
                            table.Cell().Text(worker.FullName);

                            table.Cell().Text("Documento:");
                            table.Cell().Text(worker.DocumentoIdentidad);

                            table.Cell().Text("Email:");
                            table.Cell().Text(worker.Email);

                            table.Cell().Text("Teléfono:");
                            table.Cell().Text(worker.PhoneNumber ?? "N/A");

                            table.Cell().Text("Dirección:");
                            table.Cell().Text(worker.Address ?? "N/A");
                        });

                        x.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        x.Item().Text("Información Laboral").FontSize(16).SemiBold().FontColor(Colors.Grey.Darken2);
                        x.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(150);
                                columns.RelativeColumn();
                            });

                            table.Cell().Text("Cargo:");
                            table.Cell().Text(worker.Position ?? "N/A");

                            table.Cell().Text("Departamento:");
                            table.Cell().Text(worker.Department.ToString());

                            table.Cell().Text("Salario:");
                            table.Cell().Text($"{worker.Wage:C}");

                            table.Cell().Text("Fecha de Ingreso:");
                            table.Cell().Text(worker.RegisterDate.ToString("yyyy-MM-dd"));

                            table.Cell().Text("Estado:");
                            table.Cell().Text(worker.Status.ToString());
                        });

                        x.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        x.Item().Text("Formación y Perfil").FontSize(16).SemiBold().FontColor(Colors.Grey.Darken2);
                        x.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(150);
                                columns.RelativeColumn();
                            });

                            table.Cell().Text("Nivel Educativo:");
                            table.Cell().Text(worker.EducationalLevel.ToString());

                            table.Cell().Text("Perfil Profesional:");
                            table.Cell().Text(worker.ProfessionalProfile ?? "N/A");
                        });
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Generado por TalentoPlus - ");
                        x.CurrentPageNumber();
                    });
            });
        })
        .GeneratePdf();
    }
}
