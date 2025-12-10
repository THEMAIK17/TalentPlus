using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TalentPlus.Application.Interfaces;
using TalentPlus.Domain.Entities;

namespace TalentPlus.Infraestructure.Services;

public class PdfService : IPdfService
    {
        public byte[] GenerateEmployeeCv(Employee employee)
        {
            // I create the PDF document structure
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));
                    
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text($"{employee.FirstName} {employee.LastName}")
                                .FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);
                            
                            col.Item().Text(employee.Position)
                                .FontSize(14).FontColor(Colors.Grey.Darken2);
                        });

                        row.ConstantItem(100).AlignRight().Text("TalentPlus HR")
                            .FontSize(10).FontColor(Colors.Grey.Medium);
                    });

                    // I organize the main content into sections
                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                    {
                        //  Personal Data & Contact
                        col.Item().Text("Datos Personales y Contacto").FontSize(14).SemiBold().Underline();
                        col.Item().PaddingTop(5).Table(table =>
                        {
                            table.ColumnsDefinition(columns => {
                                columns.ConstantColumn(120);
                                columns.RelativeColumn();
                            });

                            table.Cell().Text("Documento:");
                            table.Cell().Text(employee.DocumentNumber).SemiBold();
                            
                            table.Cell().Text("Email:");
                            table.Cell().Text(employee.Email);

                            table.Cell().Text("Teléfono:");
                            table.Cell().Text(employee.Phone);

                            table.Cell().Text("Dirección:");
                            table.Cell().Text(employee.Address);
                        });

                        col.Item().PaddingVertical(15).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        //  Work Information
                        col.Item().Text("Información Laboral").FontSize(14).SemiBold().Underline();
                        col.Item().PaddingTop(5).Table(table =>
                        {
                            table.ColumnsDefinition(columns => {
                                columns.ConstantColumn(120);
                                columns.RelativeColumn();
                            });

                            table.Cell().Text("Departamento:");
                            table.Cell().Text(employee.Department?.Name ?? "N/A").SemiBold();

                            table.Cell().Text("Cargo:");
                            table.Cell().Text(employee.Position);

                            table.Cell().Text("Salario:");
                            table.Cell().Text($"$ {employee.Salary:N0}");

                            // I format the date to be readable
                            table.Cell().Text("Fecha Ingreso:");
                            table.Cell().Text(employee.HiringDate.ToString("dd/MM/yyyy"));

                            table.Cell().Text("Estado:");
                            table.Cell().Text(employee.Status);
                        });

                        col.Item().PaddingVertical(15).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        //  Professional Profile & Education
                        col.Item().Text("Perfil Profesional").FontSize(14).SemiBold().Underline();
                        
                        col.Item().PaddingTop(5).Text(text => 
                        {
                            text.Span("Nivel Educativo: ").Bold();
                            text.Span(employee.EducationLevel);
                        });

                        col.Item().PaddingTop(10).Text(employee.Profile ?? "Sin perfil registrado.")
                            .Justify();
                    });
                    
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Generado por TalentPlus - ");
                        x.CurrentPageNumber();
                    });
                });
            });
            
            return document.GeneratePdf();
        }
    }