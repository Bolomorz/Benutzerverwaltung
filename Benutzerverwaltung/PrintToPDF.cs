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
using System.Windows;
using PdfSharp.UniversalAccessibility.Drawing;

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

        public static void PrintUserListToTable(List<User> users, List<Static> statics, List<Variable> variables, string file) 
        {
            var document = new PdfDocument();
            document.Info.Title = string.Format("Nutzer Liste");
            var page = document.AddPage();
            page.Size = PageSize.A4;
            page.Orientation = PageOrientation.Landscape;
            var gfx = XGraphics.FromPdfPage(page);

            /*      Nr      Name    Vorname Sta     Var     Sum
             *      5mm     20mm    20mm    20mm    20mm    20mm
             */
            decimal[] colsums = new decimal[statics.Count + variables.Count + 1];
            int intervally = 15;
            int intervallx = 30;
            int ix = 5;
            int iy = 5;
            int endx = 270;
            int endy = 200;

            XRect curr = new XRect(
                new XPoint(MillimeterToPointHeight(ix), MillimeterToPointWidth(iy)),
                new XPoint(MillimeterToPointHeight(ix + 5), MillimeterToPointWidth(iy + intervally)));
            gfx.DrawString("Nr", fonttext, brush, curr, XStringFormats.CenterLeft);
            ix += 5;
            curr = new XRect(
                new XPoint(MillimeterToPointHeight(ix), MillimeterToPointWidth(iy)),
                new XPoint(MillimeterToPointHeight(ix + intervallx), MillimeterToPointWidth(iy + intervally)));
            gfx.DrawString(" Name", fonttext, brush, curr, XStringFormats.CenterLeft);
            ix += intervallx;
            curr = new XRect(
                new XPoint(MillimeterToPointHeight(ix), MillimeterToPointWidth(iy)),
                new XPoint(MillimeterToPointHeight(ix + intervallx), MillimeterToPointWidth(iy + intervally)));
            gfx.DrawString(" Vorname", fonttext, brush, curr, XStringFormats.CenterLeft);
            ix += intervallx;
            curr = new XRect(
                new XPoint(MillimeterToPointHeight(ix), MillimeterToPointWidth(iy)),
                new XPoint(MillimeterToPointHeight(270), MillimeterToPointWidth(iy + intervally)));
            gfx.DrawString("Rechnungsposten", fonttext, brush, curr, XStringFormats.Center);
            ix = 270;
            curr = new XRect(
                new XPoint(MillimeterToPointHeight(ix), MillimeterToPointWidth(iy)),
                new XPoint(MillimeterToPointHeight(ix + intervallx), MillimeterToPointWidth(iy + intervally)));
            gfx.DrawString(" Summe", fonttext, brush, curr, XStringFormats.CenterLeft);

            iy += intervally;
            gfx.DrawLine(new XPen(XColors.Black),
                new XPoint(MillimeterToPointHeight(5), MillimeterToPointWidth(iy)),
                new XPoint(MillimeterToPointHeight(290), MillimeterToPointWidth(iy)));

            int col;
            int usernr = 1;
            foreach (var user in users)
            {
                col = 0;
                decimal usersum = 0;

                ix = 5;
                curr = new XRect(
                new XPoint(MillimeterToPointHeight(ix), MillimeterToPointWidth(iy)),
                new XPoint(MillimeterToPointHeight(ix + 5), MillimeterToPointWidth(iy + intervally)));
                gfx.DrawString(" " + usernr.ToString(), fonttext, brush, curr, XStringFormats.CenterLeft);
                ix += 5;
                curr = new XRect(
                    new XPoint(MillimeterToPointHeight(ix), MillimeterToPointWidth(iy)),
                    new XPoint(MillimeterToPointHeight(ix + intervallx), MillimeterToPointWidth(iy + intervally)));
                gfx.DrawString(" " + user.Name, fonttext, brush, curr, XStringFormats.CenterLeft);
                ix += intervallx;
                curr = new XRect(
                    new XPoint(MillimeterToPointHeight(ix), MillimeterToPointWidth(iy)),
                    new XPoint(MillimeterToPointHeight(ix + intervallx), MillimeterToPointWidth(iy + intervally)));
                gfx.DrawString(" " + user.Vorname, fonttext, brush, curr, XStringFormats.CenterLeft);
                ix = 70;

                foreach(var s in statics)
                {
                    var find = FindStaticInList(s, user.statics);
                    if(find.b)
                    {
                        curr = new XRect(
                            new XPoint(MillimeterToPointHeight(ix), MillimeterToPointWidth(iy)),
                            new XPoint(MillimeterToPointHeight(ix + intervallx), MillimeterToPointWidth(iy + intervally / 2)));
                        gfx.DrawString(find.s.Name, fonttext, brush, curr, XStringFormats.Center);
                        curr = new XRect(
                            new XPoint(MillimeterToPointHeight(ix), MillimeterToPointWidth(iy + intervally / 2)),
                            new XPoint(MillimeterToPointHeight(ix + intervallx), MillimeterToPointWidth(iy + intervally)));
                        gfx.DrawString(find.s.Wert.ToString(), fonttext, brush, curr, XStringFormats.Center);
                        usersum += find.s.Wert;
                        colsums[col] += find.s.Wert;
                    }
                    else
                    {
                        curr = new XRect(
                            new XPoint(MillimeterToPointHeight(ix), MillimeterToPointWidth(iy)),
                            new XPoint(MillimeterToPointHeight(ix + intervallx), MillimeterToPointWidth(iy + intervally / 2)));
                        gfx.DrawString(find.s.Name, fonttext, brush, curr, XStringFormats.Center);
                        curr = new XRect(
                            new XPoint(MillimeterToPointHeight(ix), MillimeterToPointWidth(iy + intervally / 2)),
                            new XPoint(MillimeterToPointHeight(ix + intervallx), MillimeterToPointWidth(iy + intervally)));
                        gfx.DrawString("-", fonttext, brush, curr, XStringFormats.Center);
                    }
                    if (ix + 2 * intervallx <= endx) ix += intervallx;
                    else
                    {
                        if(iy + 2 * intervally <= endy) iy += intervally;
                        else
                        {
                            gfx = NewPageLandscape(document);
                            iy = 20;
                        }
                        ix = 50;
                    }
                    col++;
                }

                foreach(var v in variables)
                {
                    var find = FindVariableInList(v, user.variables);
                    var value = v.CalcValue(find.w);
                    curr = new XRect(
                            new XPoint(MillimeterToPointHeight(ix), MillimeterToPointWidth(iy)),
                            new XPoint(MillimeterToPointHeight(ix + intervallx), MillimeterToPointWidth(iy + intervally / 2)));
                    gfx.DrawString(find.v.Name, fonttext, brush, curr, XStringFormats.Center);
                    curr = new XRect(
                        new XPoint(MillimeterToPointHeight(ix), MillimeterToPointWidth(iy + intervally / 2)),
                        new XPoint(MillimeterToPointHeight(ix + intervallx), MillimeterToPointWidth(iy + intervally)));
                    gfx.DrawString(value.ToString(), fonttext, brush, curr, XStringFormats.Center);
                    usersum += value;
                    colsums[col] += value;
                    if (ix + 2 * intervallx <= endx) ix += intervallx;
                    else
                    {
                        if (iy + 2 * intervally <= endy) iy += intervally;
                        else
                        {
                            gfx = NewPageLandscape(document);
                            iy = 20;
                        }
                        ix = 50;
                    }
                    col++;
                }

                curr = new XRect(
                    new XPoint(MillimeterToPointHeight(270), MillimeterToPointWidth(iy)),
                    new XPoint(MillimeterToPointHeight(290), MillimeterToPointWidth(iy + intervally)));
                gfx.DrawString(" " + usersum.ToString(), fonttext, brush, curr, XStringFormats.CenterLeft);

                colsums[colsums.Length - 1] += usersum;

                usernr++;

                if (iy + 2 * intervally <= endy) iy += intervally;
                else
                {
                    gfx = NewPageLandscape(document);
                    iy = 20;
                }
                gfx.DrawLine(new XPen(XColors.Black),
                    new XPoint(MillimeterToPointHeight(5), MillimeterToPointWidth(iy)),
                    new XPoint(MillimeterToPointHeight(290), MillimeterToPointWidth(iy)));
            }

            curr = new XRect(
                    new XPoint(MillimeterToPointHeight(40), MillimeterToPointWidth(iy)),
                    new XPoint(MillimeterToPointHeight(70), MillimeterToPointWidth(iy + intervally)));
            gfx.DrawString(" Summe", fonttext, brush, curr, XStringFormats.CenterLeft);

            ix = 70;
            col = 0;
            foreach(var s in statics)
            {
                curr = new XRect(
                    new XPoint(MillimeterToPointHeight(ix), MillimeterToPointWidth(iy)),
                    new XPoint(MillimeterToPointHeight(ix + intervallx), MillimeterToPointWidth(iy + intervally / 2)));
                gfx.DrawString(s.Name, fonttext, brush, curr, XStringFormats.Center);
                curr = new XRect(
                    new XPoint(MillimeterToPointHeight(ix), MillimeterToPointWidth(iy + intervally / 2)),
                    new XPoint(MillimeterToPointHeight(ix + intervallx), MillimeterToPointWidth(iy + intervally)));
                gfx.DrawString(colsums[col].ToString(), fonttext, brush, curr, XStringFormats.Center);
                if (ix + 2 * intervallx <= endx) ix += intervallx;
                else
                {
                    if (iy + 2 * intervally <= endy) iy += intervally;
                    else
                    {
                        gfx = NewPageLandscape(document);
                        iy = 20;
                    }
                    ix = 50;
                }
                col++;
            }
            foreach(var v in variables)
            {
                curr = new XRect(
                    new XPoint(MillimeterToPointHeight(ix), MillimeterToPointWidth(iy)),
                    new XPoint(MillimeterToPointHeight(ix + intervallx), MillimeterToPointWidth(iy + intervally / 2)));
                gfx.DrawString(v.Name, fonttext, brush, curr, XStringFormats.Center);
                curr = new XRect(
                    new XPoint(MillimeterToPointHeight(ix), MillimeterToPointWidth(iy + intervally / 2)),
                    new XPoint(MillimeterToPointHeight(ix + intervallx), MillimeterToPointWidth(iy + intervally)));
                gfx.DrawString(colsums[col].ToString(), fonttext, brush, curr, XStringFormats.Center);
                if (ix + 2 * intervallx <= endx) ix += intervallx;
                else
                {
                    if (iy + 2 * intervally <= endy) iy += intervally;
                    else
                    {
                        gfx = NewPageLandscape(document);
                        iy = 20;
                    }
                    ix = 50;
                }
                col++;
            }
            curr = new XRect(
                    new XPoint(MillimeterToPointHeight(270), MillimeterToPointWidth(iy)),
                    new XPoint(MillimeterToPointHeight(290), MillimeterToPointWidth(iy + intervally)));
            gfx.DrawString(" " + colsums[colsums.Length - 1].ToString(), fonttext, brush, curr, XStringFormats.CenterLeft);

            iy += intervally;
            gfx.DrawLine(new XPen(XColors.Black),
                    new XPoint(MillimeterToPointHeight(10), MillimeterToPointWidth(5)),
                    new XPoint(MillimeterToPointHeight(10), MillimeterToPointWidth(iy)));
            gfx.DrawLine(new XPen(XColors.Black),
                    new XPoint(MillimeterToPointHeight(40), MillimeterToPointWidth(5)),
                    new XPoint(MillimeterToPointHeight(40), MillimeterToPointWidth(iy)));
            gfx.DrawLine(new XPen(XColors.Black),
                    new XPoint(MillimeterToPointHeight(70), MillimeterToPointWidth(5)),
                    new XPoint(MillimeterToPointHeight(70), MillimeterToPointWidth(iy)));
            gfx.DrawLine(new XPen(XColors.Black),
                    new XPoint(MillimeterToPointHeight(270), MillimeterToPointWidth(5)),
                    new XPoint(MillimeterToPointHeight(270), MillimeterToPointWidth(iy)));

            document.Save(file);
            document.Close();
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(file) { UseShellExecute = true });
        }

        private static XGraphics NewPageLandscape(PdfDocument document)
        {
            var page = document.AddPage();
            page.Size = PageSize.A4;
            page.Orientation = PageOrientation.Landscape;
            var gfx = XGraphics.FromPdfPage(page);
            XRect curr = new XRect(
                new XPoint(MillimeterToPointHeight(5), MillimeterToPointWidth(5)),
                new XPoint(MillimeterToPointHeight(10), MillimeterToPointWidth(20)));
            gfx.DrawString("Nr", fonttext, brush, curr, XStringFormats.CenterLeft);
            curr = new XRect(
                new XPoint(MillimeterToPointHeight(10), MillimeterToPointWidth(5)),
                new XPoint(MillimeterToPointHeight(40), MillimeterToPointWidth(20)));
            gfx.DrawString("Name", fonttext, brush, curr, XStringFormats.CenterLeft);
            curr = new XRect(
                new XPoint(MillimeterToPointHeight(40), MillimeterToPointWidth(5)),
                new XPoint(MillimeterToPointHeight(70), MillimeterToPointWidth(20)));
            gfx.DrawString("Vorname", fonttext, brush, curr, XStringFormats.CenterLeft);
            curr = new XRect(
                new XPoint(MillimeterToPointHeight(70), MillimeterToPointWidth(5)),
                new XPoint(MillimeterToPointHeight(270), MillimeterToPointWidth(20)));
            gfx.DrawString("Rechnungsposten", fonttext, brush, curr, XStringFormats.CenterLeft);
            curr = new XRect(
                new XPoint(MillimeterToPointHeight(270), MillimeterToPointWidth(5)),
                new XPoint(MillimeterToPointHeight(290), MillimeterToPointWidth(20)));
            gfx.DrawString("Summe", fonttext, brush, curr, XStringFormats.CenterLeft);
            gfx.DrawLine(new XPen(XColors.Black),
                new XPoint(MillimeterToPointHeight(5), MillimeterToPointWidth(20)),
                new XPoint(MillimeterToPointHeight(290), MillimeterToPointWidth(20)));
            return gfx;
        }

        private static (Variable v, decimal w) FindVariableInList(Variable v, List<(Variable v, decimal w)> variables)
        {
            foreach(var element in variables)
            {
                if(element.v == v) return element;
            }
            return (v, v.Default);
        }
        private static (Static s, bool b) FindStaticInList(Static s, List<(Static s, bool b)> statics)
        {
            foreach(var element in statics)
            {
                if (element.s == s) return element;
            }
            return (s, false);
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
