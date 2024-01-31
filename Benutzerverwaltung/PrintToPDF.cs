using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;

namespace Benutzerverwaltung
{
    static internal class PrintToPDF
    {
        public static XFont font = new XFont("Verdana", 20);
        public static void PrintUserToInvoice(User user, string file, DataBaseConnection.Date today)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            PdfDocument document = new PdfDocument();
            document.Info.Title = string.Format("Rechnung {0} {1}", user.Vorname, user.Name);
            PdfPage page = document.AddPage();
            page.Size = PdfSharp.PageSize.A4;
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XTextFormatter tf = new XTextFormatter(gfx);


            document.Save(file);
        }

        public static void PrintUserListToTable(List<User> users, string file) 
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Nutzer Liste";
            PdfPage page = document.AddPage();
            page.Size = PdfSharp.PageSize.A4;
            page.Orientation = PdfSharp.PageOrientation.Landscape;
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XTextFormatter tf = new XTextFormatter(gfx);


            document.Save(file);
        }
    }
}
