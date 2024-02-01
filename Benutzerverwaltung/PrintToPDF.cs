using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Snippets.Font;

namespace Benutzerverwaltung
{
    static internal class PrintToPDF
    {
        static XFont fontheading = new("Arial", 20);
        static XFont fonttext = new("Arial", 10);
        static XBrush brush = XBrushes.Black;

        static XRect Rempf = new(
            new XPoint(MillimeterToPointWidth(25), MillimeterToPointHeight(32)),
            new XPoint(MillimeterToPointWidth(105), MillimeterToPointHeight(72)));
        static XRect Rabs = new(
            new XPoint(MillimeterToPointWidth(125), MillimeterToPointHeight(32)),
            new XPoint(MillimeterToPointWidth(200), MillimeterToPointHeight(72)));
        static XRect Rlogo = new(
            new XPoint(MillimeterToPointWidth(25), MillimeterToPointHeight(0)),
            new XPoint(MillimeterToPointWidth(190), MillimeterToPointHeight(27)));
        static XRect Rtext = new(
            new XPoint(MillimeterToPointWidth(25), MillimeterToPointHeight(80)),
            new XPoint(MillimeterToPointWidth(190), MillimeterToPointHeight(250)));
        static XRect Rfirma = new(
            new XPoint(MillimeterToPointWidth(25), MillimeterToPointHeight(260)),
            new XPoint(MillimeterToPointWidth(190), MillimeterToPointHeight(287)));
        static XRect Rseite = new(
            new XPoint(MillimeterToPointWidth(160), MillimeterToPointHeight(251)),
            new XPoint(MillimeterToPointWidth(190), MillimeterToPointHeight(259)));

        public static void PrintUserToInvoice(User user, string file, DataBaseConnection.Date today)
        {
            if (Capabilities.Build.IsCoreBuild) GlobalFontSettings.FontResolver = new FailsafeFontResolver();

            var document = new PdfDocument();
            document.Info.Title = string.Format("Rechnung {0} {1} {2}", user.Vorname, user.Name, today);
            var page = document.AddPage();
            page.Size = PageSize.A4;
            var gfx = XGraphics.FromPdfPage(page);

            string sseite = "Seite 1";
            gfx.DrawString(sseite, fonttext, brush, Rseite, XStringFormats.CenterRight);

            string sempf = string.Format("{0} {1}\n{2}\n{3} {4}", user.Vorname, user.Name, user.Strasse, user.PLZ, user.Ort);
            gfx.DrawString(sempf, fonttext, brush, Rempf, XStringFormats.TopLeft);

            string sabs = "Gartenverein\nAnke Schneider\nFabrikweg 3\n95176 Konradsreuth";
            gfx.DrawString(sabs, fonttext, brush, Rabs, XStringFormats.TopLeft);

            string shead = "Rechnung Titel";
            gfx.DrawString(shead, fontheading, brush, Rlogo, XStringFormats.Center);

            int iseite = 1;
            int starty = 90;
            int intervally = 20;
            int endy = 250;

            string ssubhead = string.Format("Rechnung {0} {1} {2}", user.Vorname, user.Name, today);
            XRect Rsub = new XRect(
                new XPoint(MillimeterToPointWidth(25), MillimeterToPointHeight(starty)),
                new XPoint(MillimeterToPointWidth(190), MillimeterToPointHeight(starty + intervally)));
            gfx.DrawString(ssubhead, fonttext, brush, )


            document.Save(file);
            document.Close();
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(file) { UseShellExecute = true });
        }

        public static void PrintUserListToTable(List<User> users, string file) 
        {
            if (Capabilities.Build.IsCoreBuild) GlobalFontSettings.FontResolver = new FailsafeFontResolver();

            var document = new PdfDocument();
            document.Info.Title = string.Format("Nutzer Liste");
            var page = document.AddPage();
            page.Size = PageSize.A4;
            page.Orientation = PageOrientation.Landscape;
            var gfx = XGraphics.FromPdfPage(page);

            document.Save(file);
            document.Close();
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(file) { UseShellExecute = true });
        }

        private static double MillimeterToPointWidth(double mm)
        {
            //DIN A4 210 mm -> 595 pt
            var percent = mm / 210;
            return percent * 595;
        }
        private static double MillimeterToPointHeight(double mm)
        {
            //DIN A4 297 mm -> 842 pt
            var percent = mm / 297;
            return percent * 842;
        }
    }
}
