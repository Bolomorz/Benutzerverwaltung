using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using System.Security.Cryptography.Xml;
using iText.Layout.Element;

namespace Benutzerverwaltung
{
    static internal class PrintToPDF
    {
        public static void PrintUserToInvoice(User user, string file, DataBaseConnection.Date today)
        {
            PdfWriter writer = new PdfWriter(file);
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf);

            Paragraph header = new Paragraph(string.Format("Rechnung vom {0}\n", today))
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .SetFontSize(20);
            document.Add(header);

            Paragraph empf = new Paragraph(string.Format("{0} {1}\n{2}\n{3} {4}\n", user.Vorname, user.Name, user.Strasse, user.PLZ, user.Ort))
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                .SetFontSize(10);
            document.Add(empf);

            Paragraph abs = new Paragraph("Garten\nAnke Schneider\nFabrikweg 3\n95176 Konradsreuth\n")
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT)
                .SetFontSize(10);
            document.Add(abs);

            Paragraph subheader = new Paragraph("Rechnungsposten:\n")
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                .SetFontSize(10);
            document.Add(subheader);

            decimal sum = 0;
            Table statics = new Table(2, false);
            foreach(var s in user.statics)
            {
                if(s.b)
                {
                    Cell cell1 = new Cell(1, 1)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                        .Add(new Paragraph(s.s.Name));
                    Cell cell2 = new Cell(1, 1)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                        .Add(new Paragraph(s.s.Wert.ToString()));
                    sum += s.s.Wert;
                    statics.AddCell(cell1);
                    statics.AddCell(cell2);
                }
            }
            document.Add(statics);
            document.Add(new Paragraph("\n"));
            Table variables = new Table(3, false);
            foreach(var v in user.variables)
            {
                var value = v.v.CalcValue(v.w);
                Cell cell1, cell2, cell3;
                if(value == v.w)
                {
                    cell1 = new Cell(1, 1)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                        .Add(new Paragraph(v.v.Name));
                    cell2 = new Cell(1, 1)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                        .Add(new Paragraph("-"));
                    cell3 = new Cell(1, 1)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                        .Add(new Paragraph(value.ToString()));
                }
                else
                {
                    cell1 = new Cell(1, 1)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                        .Add(new Paragraph(v.v.Name));
                    cell2 = new Cell(1, 1)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                        .Add(new Paragraph(v.w.ToString()));
                    cell3 = new Cell(1, 1)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                        .Add(new Paragraph(value.ToString()));
                }
                sum += value;
                variables.AddCell(cell1);
                variables.AddCell(cell2);
                variables.AddCell(cell3);
            }
            document.Add(variables);
            document.Add(new Paragraph("\n"));
            Table summe = new Table(2, false);
            Cell ctext = new Cell(1, 1)
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                .Add(new Paragraph("Summe:"));
            Cell csum = new Cell(1, 1)
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .Add(new Paragraph(sum.ToString()));
            summe.AddCell(ctext);
            summe.AddCell(csum);
            document.Add(summe);


            document.Close();
        }

        public static void PrintUserListToTable(List<User> users, string file) 
        {

        }
    }
}
