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

        static XRect Rlogo = new(
            new XPoint(MillimeterToPointWidth(25), MillimeterToPointHeight(0)),
            new XPoint(MillimeterToPointWidth(190), MillimeterToPointHeight(27)));
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

            XRect Rempf = new(
            new XPoint(MillimeterToPointWidth(25), MillimeterToPointHeight(32)),
            new XPoint(MillimeterToPointWidth(105), MillimeterToPointHeight(72)));
            string sempf = string.Format("{0} {1}", user.Vorname, user.Name);
            gfx.DrawString(sempf, fonttext, brush, Rempf, XStringFormats.TopLeft);
            Rempf = new(
            new XPoint(MillimeterToPointWidth(25), MillimeterToPointHeight(37)),
            new XPoint(MillimeterToPointWidth(105), MillimeterToPointHeight(72)));
            sempf = string.Format("{0}", user.Strasse);
            gfx.DrawString(sempf, fonttext, brush, Rempf, XStringFormats.TopLeft);
            Rempf = new(
            new XPoint(MillimeterToPointWidth(25), MillimeterToPointHeight(42)),
            new XPoint(MillimeterToPointWidth(105), MillimeterToPointHeight(72)));
            sempf = string.Format("{0} {1}", user.PLZ, user.Ort);
            gfx.DrawString(sempf, fonttext, brush, Rempf, XStringFormats.TopLeft);

            XRect Rabs = new(
            new XPoint(MillimeterToPointWidth(125), MillimeterToPointHeight(32)),
            new XPoint(MillimeterToPointWidth(200), MillimeterToPointHeight(72)));
            string sabs = "Gartenverein";
            gfx.DrawString(sabs, fonttext, brush, Rabs, XStringFormats.TopLeft);
            Rabs = new(
            new XPoint(MillimeterToPointWidth(125), MillimeterToPointHeight(37)),
            new XPoint(MillimeterToPointWidth(200), MillimeterToPointHeight(72)));
            sabs = "Anke Schneider";
            gfx.DrawString(sabs, fonttext, brush, Rabs, XStringFormats.TopLeft); 
            Rabs = new(
            new XPoint(MillimeterToPointWidth(125), MillimeterToPointHeight(42)),
            new XPoint(MillimeterToPointWidth(200), MillimeterToPointHeight(72)));
            sabs = "Fabrikweg 3";
            gfx.DrawString(sabs, fonttext, brush, Rabs, XStringFormats.TopLeft);
            Rabs = new(
            new XPoint(MillimeterToPointWidth(125), MillimeterToPointHeight(47)),
            new XPoint(MillimeterToPointWidth(200), MillimeterToPointHeight(72)));
            sabs = "95176 Konradsreuth";
            gfx.DrawString(sabs, fonttext, brush, Rabs, XStringFormats.TopLeft);

            string shead = "Rechnung Titel";
            gfx.DrawString(shead, fontheading, brush, Rlogo, XStringFormats.Center);

            string sdaten = "Firmen Daten";
            gfx.DrawString(sdaten, fonttext, brush, Rfirma, XStringFormats.Center);

            int iseite = 1;
            int starty = 90;
            int intervally = 10;
            int endy = 250;

            decimal sum = 0;

            string ssubhead = string.Format("Rechnung {0} {1} {2}", user.Vorname, user.Name, today);
            XRect Rsub = new XRect(
                new XPoint(MillimeterToPointWidth(25), MillimeterToPointHeight(starty)),
                new XPoint(MillimeterToPointWidth(190), MillimeterToPointHeight(starty + intervally)));
            gfx.DrawString(ssubhead, fonttext, brush, Rsub, XStringFormats.CenterLeft);

            starty += intervally;

            XRect RTable = new XRect(
                new XPoint(MillimeterToPointWidth(40), MillimeterToPointHeight(starty)),
                new XPoint(MillimeterToPointWidth(190), MillimeterToPointHeight(starty + intervally)));
            gfx.DrawString("Rechnungsposten", fonttext, brush, RTable, XStringFormats.CenterLeft);
            RTable = new XRect(
                new XPoint(MillimeterToPointWidth(140), MillimeterToPointHeight(starty)),
                new XPoint(MillimeterToPointWidth(190), MillimeterToPointHeight(starty + intervally)));
            gfx.DrawString("Wert", fonttext, brush, RTable, XStringFormats.CenterLeft);
            RTable = new XRect(
                new XPoint(MillimeterToPointWidth(140), MillimeterToPointHeight(starty)),
                new XPoint(MillimeterToPointWidth(190), MillimeterToPointHeight(starty + intervally)));
            gfx.DrawString("Rechnungsbetrag", fonttext, brush, RTable, XStringFormats.CenterRight);
            gfx.DrawLine(new XPen(XColors.Black), new XPoint(MillimeterToPointWidth(40), MillimeterToPointHeight(starty + intervally)),
                new XPoint(MillimeterToPointWidth(190), MillimeterToPointHeight(starty + intervally)));

            starty += intervally;

            foreach (var s in user.statics)
            {
                if (s.b)
                {
                    XRect Rstatic = new XRect(
                        new XPoint(MillimeterToPointWidth(40), MillimeterToPointHeight(starty)),
                        new XPoint(MillimeterToPointWidth(190), MillimeterToPointHeight(starty + intervally)));
                    string ssname = string.Format("{0}", s.s.Name);
                    string sswert = string.Format("{0} €", s.s.Wert);
                    gfx.DrawString(ssname, fonttext, brush, Rstatic, XStringFormats.CenterLeft);
                    gfx.DrawString(sswert, fonttext, brush, Rstatic, XStringFormats.CenterRight);

                    sum += s.s.Wert;

                    if(starty + 2 * intervally < endy)
                    {
                        starty += intervally;
                    }
                    else
                    {
                        if (iseite == 1) gfx.DrawString("Seite 1", fonttext, brush, Rseite, XStringFormats.CenterRight);
                        iseite++;
                        page = document.AddPage();
                        page.Size = PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        string sseite = string.Format("Seite {0}", iseite);
                        gfx.DrawString(sseite, fonttext, brush, Rseite, XStringFormats.CenterRight);
                        gfx.DrawString(shead, fontheading, brush, Rlogo, XStringFormats.Center);
                        gfx.DrawString(sdaten, fonttext, brush, Rfirma, XStringFormats.Center);
                        starty = 90;
                        RTable = new XRect(
                            new XPoint(MillimeterToPointWidth(40), MillimeterToPointHeight(starty)),
                            new XPoint(MillimeterToPointWidth(190), MillimeterToPointHeight(starty + intervally)));
                        gfx.DrawString("Rechnungsposten", fonttext, brush, RTable, XStringFormats.CenterLeft);
                        RTable = new XRect(
                            new XPoint(MillimeterToPointWidth(140), MillimeterToPointHeight(starty)),
                            new XPoint(MillimeterToPointWidth(190), MillimeterToPointHeight(starty + intervally)));
                        gfx.DrawString("Wert", fonttext, brush, RTable, XStringFormats.CenterLeft);
                        RTable = new XRect(
                            new XPoint(MillimeterToPointWidth(140), MillimeterToPointHeight(starty)),
                            new XPoint(MillimeterToPointWidth(190), MillimeterToPointHeight(starty + intervally)));
                        gfx.DrawString("Rechnungsbetrag", fonttext, brush, RTable, XStringFormats.CenterRight);
                        gfx.DrawLine(new XPen(XColors.Black), new XPoint(MillimeterToPointWidth(40), MillimeterToPointHeight(starty + intervally)),
                            new XPoint(MillimeterToPointWidth(190), MillimeterToPointHeight(starty + intervally)));

                        starty += intervally;
                    }
                }
            }
            foreach(var v in user.variables)
            {
                var value = v.v.CalcValue(v.w);
                XRect RVariable = new XRect(
                        new XPoint(MillimeterToPointWidth(40), MillimeterToPointHeight(starty)),
                        new XPoint(MillimeterToPointWidth(190), MillimeterToPointHeight(starty + intervally)));
                string svname = string.Format("{0}", v.v.Name);
                string svwert = string.Format("{0} €", value);
                gfx.DrawString(svname, fonttext, brush, RVariable, XStringFormats.CenterLeft);
                gfx.DrawString(svwert, fonttext, brush, RVariable, XStringFormats.CenterRight);
                if(value != v.w)
                {
                    RVariable = new XRect(
                        new XPoint(MillimeterToPointWidth(140), MillimeterToPointHeight(starty)),
                        new XPoint(MillimeterToPointWidth(190), MillimeterToPointHeight(starty + intervally)));
                    gfx.DrawString(string.Format("{0}", v.w), fonttext, brush, RVariable, XStringFormats.CenterLeft);
                }

                sum += value;

                if (starty + 2 * intervally < endy)
                {
                    starty += intervally;
                }
                else
                {
                    if (iseite == 1) gfx.DrawString("Seite 1", fonttext, brush, Rseite, XStringFormats.CenterRight);
                    iseite++;
                    page = document.AddPage();
                    page.Size = PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    string sseite = string.Format("Seite {0}", iseite);
                    gfx.DrawString(sseite, fonttext, brush, Rseite, XStringFormats.CenterRight);
                    gfx.DrawString(shead, fontheading, brush, Rlogo, XStringFormats.Center);
                    gfx.DrawString(sdaten, fonttext, brush, Rfirma, XStringFormats.Center);
                    starty = 90;
                    RTable = new XRect(
                        new XPoint(MillimeterToPointWidth(40), MillimeterToPointHeight(starty)),
                        new XPoint(MillimeterToPointWidth(190), MillimeterToPointHeight(starty + intervally)));
                    gfx.DrawString("Rechnungsposten", fonttext, brush, RTable, XStringFormats.CenterLeft);
                    RTable = new XRect(
                        new XPoint(MillimeterToPointWidth(140), MillimeterToPointHeight(starty)),
                        new XPoint(MillimeterToPointWidth(190), MillimeterToPointHeight(starty + intervally)));
                    gfx.DrawString("Wert", fonttext, brush, RTable, XStringFormats.CenterLeft);
                    RTable = new XRect(
                        new XPoint(MillimeterToPointWidth(140), MillimeterToPointHeight(starty)),
                        new XPoint(MillimeterToPointWidth(190), MillimeterToPointHeight(starty + intervally)));
                    gfx.DrawString("Rechnungsbetrag", fonttext, brush, RTable, XStringFormats.CenterRight);
                    gfx.DrawLine(new XPen(XColors.Black), new XPoint(MillimeterToPointWidth(40), MillimeterToPointHeight(starty + intervally)),
                        new XPoint(MillimeterToPointWidth(190), MillimeterToPointHeight(starty + intervally)));

                    starty += intervally;
                }
            }

            XRect RSum = new XRect(
                        new XPoint(MillimeterToPointWidth(40), MillimeterToPointHeight(starty)),
                        new XPoint(MillimeterToPointWidth(190), MillimeterToPointHeight(starty + intervally)));
            string ssum = "Summe";
            string ssumvalue = string.Format("{0} €", sum);
            gfx.DrawString(ssum, fonttext, brush, RSum, XStringFormats.CenterLeft);
            gfx.DrawString(ssumvalue, fonttext, brush, RSum, XStringFormats.CenterRight);
            gfx.DrawLine(new XPen(XColors.Black), new XPoint(MillimeterToPointWidth(40), MillimeterToPointHeight(starty)),
                new XPoint(MillimeterToPointWidth(190), MillimeterToPointHeight(starty)));

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
