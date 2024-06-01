using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using MigraDoc.DocumentObjectModel.Shapes.Charts;
using System.Drawing;
using System.IO;
using System.Reflection;
//using System.Runtime.InteropServices;

namespace MVision
{
    public class Report
    {

        //private MigraDoc.DocumentObjectModel.TabAlignment tabAlignment = new MigraDoc.DocumentObjectModel.TabAlignment();
        private Document document = new Document();
        private Table table = new Table();
        private TextFrame textFrame = new TextFrame();
        // private MigraDoc.DocumentObjectModel.Color TableBorder;
        // private MigraDoc.DocumentObjectModel.Color TableGray;
        private TextFrame addressFrame { get; set; }
        private Section section;
        private ReportDT report;


        // [Obsolete]
        Document CreateDocument(ReportDT Report)
        {
            report = Report;

            // Create a new MigraDoc document
            DefineStyles();
            CreatePage();
            FillContent();


            DefineCharts(document);
            SetImg();





            Document documentRender = new Document();
            documentRender = document;
            document = new Document();


            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer()
            {
                //    // Передайте документ візуалісту:
                Document = documentRender
            };

            // Нехай візуаліст виконує свою роботу:
            pdfRenderer.RenderDocument();

            string DateNameFile = DateTime.Now.ToString("  dd_MM_yyyy HH_mm_ss");

            DateNameFile = SAV.DT.Report.PathFileSave+"\\"+ SAV.DT.Report.NameReport + DateNameFile + ".pdf";
            // DateNameFile = SV.DT_BIN.NameReport + DateNameFile + ".xlsx";
           
            try
            {
                //Збережіть PDF у файл:
  
                pdfRenderer.PdfDocument.Save(DateNameFile);



                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = DateNameFile,
                    UseShellExecute = true
                };
                Process.Start(psi);


            }
            catch { Help.ErrorMesag("The path is not correct 'select the folder path' "); }

            return this.document;
        }


        public static void testStart(string data)
        {

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = data,
                UseShellExecute = true
            };
            Process.Start(psi);

        }






        private void DefineStyles()
        {

            // Get the predefined style Normal.
            Style style = this.document.Styles["Normal"];
            // Because all styles are derived from Normal, the next line changes the 
            // font of the whole document. Or, more exactly, it changes the font of
            // all styles and paragraphs that do not redefine the font.
            style.Font.Name = "Verdana";

            style = this.document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("6cm", MigraDoc.DocumentObjectModel.TabAlignment.Right);

            style = this.document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop("8cm", MigraDoc.DocumentObjectModel.TabAlignment.Center);

            // Create a new style called Table based on style Normal
            style = this.document.Styles.AddStyle("Table", "Normal");
            style.Font.Name = "Verdana";
            style.Font.Name = "Times New Roman";
            style.Font.Size = 9;



            // Create a new style called Reference based on style Normal
            style = this.document.Styles.AddStyle("Reference", "Normal");
            style.ParagraphFormat.SpaceBefore = "2mm";  //відступ перед таблицею
            style.ParagraphFormat.SpaceAfter = "2mm";   //відступ після таблиці
            style.Font.Name = "Times New Roman";
            style.ParagraphFormat.TabStops.AddTabStop("16cm", MigraDoc.DocumentObjectModel.TabAlignment.Left);
            style.ParagraphFormat.Shading.Color = Colors.LightGray;

            // Create a new style called Reference based on style Normal
            style = this.document.Styles.AddStyle("StyleType1", "Normal");
            style.ParagraphFormat.SpaceBefore = "2mm";  //відступ перед таблицею
            style.ParagraphFormat.SpaceAfter = "2mm";   //відступ після таблиці
            style.Font.Name = "Times New Roman";
            style.ParagraphFormat.TabStops.AddTabStop("15cm", MigraDoc.DocumentObjectModel.TabAlignment.Left);
            //style.ParagraphFormat.Shading.Color = Colors.LightGray;

            // Create a new style called Reference based on style Normal
            style = this.document.Styles.AddStyle("StyleType2", "Normal");
            style.ParagraphFormat.SpaceBefore = "2mm";  //відступ перед таблицею
            style.ParagraphFormat.SpaceAfter = "2mm";   //відступ після таблиці
            style.Font.Name = "Times New Roman";
            style.ParagraphFormat.TabStops.AddTabStop("15cm", MigraDoc.DocumentObjectModel.TabAlignment.Center);
            //style.ParagraphFormat.Shading.Color = Colors.LightGray;


            // Create a new style called TextBox based on style Normal
            style = document.Styles.AddStyle("TextBox", "Normal");
            style.ParagraphFormat.Alignment = ParagraphAlignment.Justify;
            style.ParagraphFormat.Borders.Width = 2.5;
            style.ParagraphFormat.Borders.Distance = "3pt";
            style.ParagraphFormat.Shading.Color = Colors.SkyBlue;




        }




        string MigraDocFilenameFromByteArray(byte[] image)
        {
            return "base64:" + Convert.ToBase64String(image);
        }

        byte[] LoadImage(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(name))
            {
                if (stream == null)
                    throw new ArgumentException("No resource with name " + name);

                int count = (int)stream.Length;
                byte[] data = new byte[count];
                stream.Read(data, 0, count);
                return data;
            }
        }


        private byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }


        private void CreatePage()
        {

            // Each MigraDoc document needs at least one section.
            section = this.document.AddSection();
            MigraDoc.DocumentObjectModel.Shapes.Image image = new MigraDoc.DocumentObjectModel.Shapes.Image();
            section.PageSetup.OddAndEvenPagesHeaderFooter = true;
            section.PageSetup.StartingNumber = 1;

            // Put a logo in the header
            image = section.Headers.Primary.AddImage("MicroOptik.tif");      //                   section.Headers.Primary.AddImage("../../PowerBooks.png");


            image.Height = "2cm";
            image.LockAspectRatio = true;
            image.RelativeVertical = RelativeVertical.Line;
            image.RelativeHorizontal = RelativeHorizontal.Margin;
            image.Top = ShapePosition.Top;
            image.Left = ShapePosition.Right;
            image.WrapFormat.Style = WrapStyle.TopBottom;



            // Create footer
            Paragraph paragraph = section.Footers.Primary.AddParagraph();
            //paragraph.AddText("Pge 1");

            paragraph.Format.Font.Size = 9;
            paragraph.Format.Alignment = ParagraphAlignment.Center;

            // Create the text frame for the address
            addressFrame = section.AddTextFrame();
            this.addressFrame.Height = "3.0cm";
            this.addressFrame.Width = "7.0cm";
            this.addressFrame.Left = ShapePosition.Left;
            this.addressFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            this.addressFrame.Top = "5.0cm";
            this.addressFrame.RelativeVertical = RelativeVertical.Page;

            // Put sender in address frame
            paragraph = this.addressFrame.AddParagraph("Morphious C2S10");
            paragraph.Format.Font.Name = "Times New Roman";
            paragraph.Format.Font.Size = 24;
            paragraph.Format.SpaceAfter = 3;

            // Put sender in address frame
            paragraph = this.addressFrame.AddParagraph("Sample Name : " + SAV.DT.Report.NameReport);
            paragraph.Format.Font.Name = "Times New Roman";
            paragraph.Format.Font.Color = Colors.Blue;
            paragraph.Format.Font.Size = 14;
            paragraph.Format.SpaceAfter = 5;

            // Put sender in address frame
            paragraph = this.addressFrame.AddParagraph("Created by :    " + SAV.DT.Report.CreatedBy);
            paragraph.Format.Font.Name = "Times New Roman";
            paragraph.Format.Font.Color = Colors.Black;
            paragraph.Format.Font.Size = 14;
            paragraph.Format.SpaceAfter = 5;
            paragraph = section.AddParagraph();  // Add the print date field

            // Put sender in address frame ///////////////////////////////////
            paragraph.Style = "StyleType1";
            paragraph.Format.SpaceBefore = "3cm";
            paragraph = this.addressFrame.AddParagraph("Comments :    " + SAV.DT.Report.Comments);
            paragraph.Format.Font.Name = "Times New Roman";
            paragraph.Format.Font.Color = Colors.Black;
            paragraph.Format.Font.Size = 14;
            paragraph.Format.SpaceAfter = 5;

            // paragraph.AddTab();
            paragraph = section.AddParagraph();  // Add the print date field
            //////////////////////////////////

            paragraph.Format.SpaceBefore = "8cm";
            paragraph.Style = "StyleType1";
            paragraph.Format.Font.Size = 14;
            paragraph.AddFormattedText("Rejects in " + SAV.DT.Report.SempleTyp, TextFormat.Bold/* жирний шрифт */ );

            paragraph.AddTab();
            paragraph.AddText("Measure Date: ");
            paragraph.AddDateField("dd.MM.yyyy   hh:mm:ss");



            // Create the item table
            this.table = section.AddTable();
            //////////////////////////////
            this.table.Style = "Table";
            this.table.Borders.Color = Colors.Black;           //TableBorder;
            this.table.Borders.Width = 0.5;
            this.table.Borders.Left.Width = 0.5;
            this.table.Borders.Right.Width = 0.5;
            this.table.Rows.LeftIndent = 0;

            // Before you can add a row, you must define the columns
            Column column = this.table.AddColumn("1cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = this.table.AddColumn("6cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = this.table.AddColumn("3cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = this.table.AddColumn("3cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = this.table.AddColumn("3cm");
            column.Format.Alignment = ParagraphAlignment.Center;




            // Create the header of the table
            Row row = table.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;
            row.Shading.Color = Colors.LightGray;
            row.Format.Font.Size = 12;

            row.Cells[0].AddParagraph("№");
            row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Top;
            row.Cells[0].MergeDown = 0; //обєднати стовці

            row.Cells[1].AddParagraph("REJECTS");
            row.Cells[1].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[1].VerticalAlignment = VerticalAlignment.Top;
            row.Cells[1].MergeDown = 0; //обєднати стовці

            row.Cells[2].AddParagraph("PERCENTAGE OF REJECTS %");
            row.Cells[2].Format.Font.Bold = false;
            row.Cells[2].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[2].VerticalAlignment = VerticalAlignment.Top;
            row.Cells[2].MergeRight = 0; // обєднати рядки


            row.Cells[3].AddParagraph("QUANTITY PCS");
            row.Cells[3].Format.Font.Bold = false;
            row.Cells[3].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[3].VerticalAlignment = VerticalAlignment.Top;
            row.Cells[3].MergeDown = 0;

            row.Cells[4].AddParagraph("WEIGHT    Grams ");
            row.Cells[4].Format.Font.Bold = false;
            row.Cells[4].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[4].VerticalAlignment = VerticalAlignment.Top;
            row.Cells[4].MergeDown = 0;




            this.table.SetEdge(0, 0, 5, 1, Edge.Box, MigraDoc.DocumentObjectModel.BorderStyle.Single, 0.75, MigraDoc.DocumentObjectModel.Color.Empty);
        }




        private void FillContent()
        {

            Row row;
            int CoutSumPCS = 0;
            double CoutSumGm = 0;
            double TotalReject = 0;
            for (int i = 0; i < report.Idx; i++)
            {
                CoutSumPCS = CoutSumPCS + report.DataPic[i];



                CoutSumGm = Math.Round((CoutSumGm + report.Weight[i]), 3);

                if (report.Name[i] != "good") { TotalReject = Math.Round((TotalReject + report.DataPct[i]), 3); }




                // Create the item table
                row = this.table.AddRow();
                //nema
                row.Cells[0].AddParagraph((i + 1).ToString());
                row.Cells[0].Format.Font.Bold = false;
                row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[0].VerticalAlignment = VerticalAlignment.Top;
                row.Cells[0].MergeRight = 0; // обєднати рядки
                row.Cells[0].Shading.Color = Colors.Honeydew;
                row.Cells[0].Format.Font.Size = 14;


                //nema
                row.Cells[1].AddParagraph(report.Name[i]);
                row.Cells[1].Format.Font.Bold = false;
                row.Cells[1].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[1].VerticalAlignment = VerticalAlignment.Top;
                row.Cells[1].MergeRight = 0; // обєднати рядки
                row.Cells[1].Shading.Color = Colors.Honeydew;
                row.Cells[1].Format.Font.Size = 14;

                //%
                row.Cells[2].AddParagraph(report.DataPct[i].ToString());
                row.Cells[2].Format.Font.Bold = false;
                row.Cells[2].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[2].VerticalAlignment = VerticalAlignment.Top;
                row.Cells[2].MergeRight = 0; // обєднати рядки
                row.Cells[2].Shading.Color = Colors.Honeydew;
                row.Cells[2].Format.Font.Size = 14;

                //pisec
                row.Cells[3].AddParagraph(report.DataPic[i].ToString());
                row.Cells[3].Format.Font.Bold = false;
                row.Cells[3].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[3].VerticalAlignment = VerticalAlignment.Top;
                row.Cells[3].MergeRight = 0; // обєднати рядки
                row.Cells[3].Shading.Color = Colors.Honeydew;
                row.Cells[3].Format.Font.Size = 14;

                //Weight //MASSA
                row.Cells[4].AddParagraph(report.Weight[i].ToString());
                row.Cells[4].Format.Font.Bold = false;
                row.Cells[4].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[4].VerticalAlignment = VerticalAlignment.Top;
                row.Cells[4].MergeRight = 0; // обєднати рядки
                row.Cells[4].Shading.Color = Colors.Honeydew;
                row.Cells[4].Format.Font.Size = 14;

                // Set the borders of the specified cell range
                this.table.SetEdge(1, this.table.Rows.Count - 1, 2, 1, Edge.Box, MigraDoc.DocumentObjectModel.BorderStyle.Single, 0.75);

            }




            // Create the item table
            row = this.table.AddRow();


            //nema
            row.Cells[0].AddParagraph("TOTAL");
            row.Cells[0].Format.Font.Bold = false;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Top;
            row.Cells[0].MergeRight = 1; // обєднати рядки
            row.Cells[0].Shading.Color = Colors.Honeydew;
            row.Cells[0].Format.Font.Size = 16;

            //%
            row.Cells[2].AddParagraph(TotalReject.ToString());
            row.Cells[2].Format.Font.Bold = false;
            row.Cells[2].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[2].VerticalAlignment = VerticalAlignment.Top;
            row.Cells[2].MergeRight = 0; // обєднати рядки
            row.Cells[2].Shading.Color = Colors.Honeydew;
            row.Cells[2].Format.Font.Size = 16;

            //pisec
            row.Cells[3].AddParagraph(CoutSumPCS.ToString());
            row.Cells[3].Format.Font.Bold = false;
            row.Cells[3].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[3].VerticalAlignment = VerticalAlignment.Top;
            row.Cells[3].MergeRight = 0; // обєднати рядки
            row.Cells[3].Shading.Color = Colors.Honeydew;
            row.Cells[3].Format.Font.Size = 16;

            //Weight //MASSA
            row.Cells[4].AddParagraph(CoutSumGm.ToString());
            row.Cells[4].Format.Font.Bold = false;
            row.Cells[4].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[4].VerticalAlignment = VerticalAlignment.Top;
            row.Cells[4].MergeRight = 0; // обєднати рядки
            row.Cells[4].Shading.Color = Colors.Honeydew;
            row.Cells[4].Format.Font.Size = 16;

            // Set the borders of the specified cell range
            this.table.SetEdge(1, this.table.Rows.Count - 1, 2, 1, Edge.Box, MigraDoc.DocumentObjectModel.BorderStyle.Single, 0.75);










        }




        private void DefineCharts(Document document)
        {

            Section section = document.AddSection();

            // Create footer
            Paragraph paragraph = section.Footers.Primary.AddParagraph();
            //paragraph.AddText("Pge 2");
            paragraph.Format.Font.Size = 9;
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            document.LastSection.AddParagraph();
            document.LastSection.AddParagraph();


            document.LastSection.AddParagraph("Chart  " + SAV.DT.Report.NameReport, "Heading2");
            Chart chart = new Chart();
            chart.Left = 0;
            document.LastSection.AddParagraph();
            document.LastSection.AddParagraph();
            paragraph.Format.SpaceAfter = "3cm";
            paragraph.Format.SpaceBefore = "3cm";



            chart.Width = Unit.FromCentimeter(17);
            chart.Height = Unit.FromCentimeter(12);
            Series series = chart.SeriesCollection.AddSeries();

            series.ChartType = ChartType.Column2D;
            series.Add(report.DataPct);
            series.HasDataLabel = false;
            //series.DataLabel.Font.Color = Colors.Red;
            //series.DataLabel.Type = DataLabelType.None;
            //series.DataLabel.Position = DataLabelPosition.InsideBase;


            // System.IO.MemoryStream stream = new System.IO.MemoryStream();
            //chart.SaveImage(stream, System.Drawing.Imaging.ImageFormat.Bmp);
            // Bitmap bmp = new Bitmap(stream);



            string[] NemaReport;
            XSeries xseries = chart.XValues.AddXSeries();

            NemaReport = (string[])report.Name.Clone();

            for (int i = 0; i < report.Idx; i++)
            {
                //NemaReport[i] +=" " ;
                //NemaReport[i] += report.DataPct[i].ToString();
                NemaReport[i] = (i + 1).ToString();
            }


            xseries.Add(NemaReport);
            chart.Format.Font.Size = 7;


            chart.XAxis.LineFormat.Color = Colors.Plum;

            //chart.XAxis.MajorTickMark = TickMarkType.Outside;
            //chart.XAxis.Title.Caption = "Y-" + SV.DT_BIN.NameReport;

            chart.YAxis.MajorTickMark = TickMarkType.Outside;
            chart.YAxis.HasMajorGridlines = true;



            chart.PlotArea.LineFormat.Color = Colors.DarkGray;
            chart.PlotArea.LineFormat.Width = 1;
            document.LastSection.Add(chart);
        }





        void SetImg()
        {

            // Each MigraDoc document needs at least one section.
            section = this.document.AddSection();
            MigraDoc.DocumentObjectModel.Shapes.Image image = new MigraDoc.DocumentObjectModel.Shapes.Image();




            //// Put a logo in the header
            image = section.Headers.Primary.AddImage("MicroOptik.tif");      //                   section.Headers.Primary.AddImage("../../PowerBooks.png");
            image.Height = "2cm";
            image.LockAspectRatio = true;
            image.RelativeVertical = RelativeVertical.Line;
            image.RelativeHorizontal = RelativeHorizontal.Margin;
            image.Top = ShapePosition.Top;
            image.Left = ShapePosition.Right;
            image.WrapFormat.Style = WrapStyle.Through;

            document.LastSection.AddParagraph();
            document.LastSection.AddParagraph();
            //document.LastSection.AddParagraph();
            //document.LastSection.AddParagraph();
            //document.LastSection.AddParagraph();








            if ((report.IMG != null)/*&&(report.IMG[1].Count != 0)*/)
            {
                // MemoryStream strm = new MemoryStream();
                //report.IMG[1][0].Save(strm, System.Drawing.Imaging.ImageFormat.Jpeg);
                //string imageFilename = MigraDocFilenameFromByteArray(strm.ToArray());
                // Document document = new Document();

                //image.Width = "50cm";
                //image.LockAspectRatio = false;
                //image.RelativeVertical = RelativeVertical.Page;
                //image.RelativeHorizontal = RelativeHorizontal.Page;
                //image.WrapFormat.Style = WrapStyle.TopBottom;
                //section.AddImage(imageFilename);

                //Paragraph paragraph = section.Footers.Primary.AddParagraph();
                //paragraph.AddText("TEX");
                //document.LastSection.AddParagraph();
                //image.Width = "10cm";
                //image.LockAspectRatio = false;
                //image.RelativeVertical = RelativeVertical.Margin;
                //image.RelativeHorizontal = RelativeHorizontal.Column;
                //section.AddImage(imageFilename);
                // paragraph.Format.SpaceAfter = 300;
                //Paragraph paragraph = section.AddParagraph();
                // Add some text to the paragraph
                //paragraph.AddFormattedText("Hello, World!", TextFormat.Bold);
                // paragraph.AddText("TEX2");
                // Create footer




                Paragraph paragraph = section.Footers.Primary.AddParagraph();







                for (int Q = 0; Q < report.IMG.Length; Q++)
                {
                    if (report.IMG[Q].Count != 0)
                    {



                        // Add the print date field
                        paragraph = section.AddParagraph();

                        paragraph.Format.SpaceBefore = "1cm";
                        paragraph.Style = "Reference";
                        paragraph.Style.AsQueryable();
                        paragraph.AddFormattedText(report.Name[Q], TextFormat.Bold);
                        paragraph.AddTab();
                        paragraph.Format.SpaceAfter = "1cm";

                        if (report.IMG[Q].Count != 0)
                        {

                            Bitmap Data;
                            string imageFilename = "";
                            MemoryStream strm = new MemoryStream();


                            for (int i = 0; i < report.IMG[Q].Count; i++)
                            {
                                //запис Bitmap через стрім
                                strm = new MemoryStream();
                                Data = new Bitmap(report.IMG[Q][i]);


                                Data.Save(strm, System.Drawing.Imaging.ImageFormat.Bmp);

                                imageFilename = MigraDocFilenameFromByteArray(strm.ToArray());


                                paragraph.AddImage(imageFilename).Clone();

                                strm.Close();

                            }

                        }
                    }






                }




                //Create footer
                //paragraph = section.Footers.Primary.AddParagraph();
                //paragraph.AddText("www.MicrOptic.com ");
                //paragraph.Format.Font.Size = 10;
                //paragraph.Format.Alignment = ParagraphAlignment.Justify;

                // Create the text frame for the address
                //this.addressFrame = section.AddTextFrame();
                //this.addressFrame.Height = "3.0cm";
                //this.addressFrame.Width = "7.0cm";
                //this.addressFrame.Left = ShapePosition.Left;
                //this.addressFrame.RelativeHorizontal = RelativeHorizontal.Margin;
                //this.addressFrame.RelativeVertical = RelativeVertical.Page;




            }

        }




        public void ReportSet(ReportDT Report){

            report = Report;
            ReportDT reportDTrw;

            try {
           
                  int i = 0;
                if (report == null)
                {
                    report = new ReportDT(report.IMG.Length);
                    for ( i = 0; i < report.IMG.Length; i++)
                    {
                        report.Name[i] = " ";
                        report.DataPct[i] = 0;
                        report.DataPic[i] = 0;
                        report.Weight[i] = 0;

                    }
                }


                int cot = 0;
                for (i = 0; i < report.IMG.Length; i++) { if (report.IMG[i].Count != 0) { cot++; } }
                reportDTrw = new ReportDT(cot);

                cot = 0;
               
                for (i = 0; i < report.IMG.Length; i++)
                {
                    if ((report.IMG[i].Count != 0) && (report.Name[i] != "Good"))
                    {
                        reportDTrw.Name[cot] = report.Name[i];
                        reportDTrw.DataPct[cot] = report.DataPct[i];
                        reportDTrw.DataPic[cot] = report.DataPic[i];
                        reportDTrw.Weight[cot] = report.Weight[i];
                        reportDTrw.IMG[cot] = report.IMG[i];
                        cot++;
                        reportDTrw.Idx = cot;
                    }
                }

               
                for (i=0; i < report.IMG.Length; i++)
                {
                    if((report.IMG[i].Count != 0) && (report.Name[i] == "Good"))
                    {
                        reportDTrw.Name[cot] = report.Name[i];
                        reportDTrw.DataPct[cot] = report.DataPct[i];
                        reportDTrw.DataPic[cot] = report.DataPic[i];
                        reportDTrw.Weight[cot] = report.Weight[i];
                        reportDTrw.IMG[cot] = report.IMG[i];
                        cot++;
                        reportDTrw.Idx = cot;
                    }
                }




                report = reportDTrw;
                CreateDocument(report);

            }catch { Help.ErrorMesag(" Experiment cannot be empty ! "); }
            

        }

    }








    public class ReportDT
    {

        public int Idx;

        public ReportDT(int idx)
        {
            this.Idx = idx;
            Name = new string[Idx];
            DataPct = new double[Idx];
            DataPic = new int[Idx];
            Weight = new double[Idx];
            //img = new Bitmap[Idx];
            IMG = new List<Bitmap>[Idx];
        }


        public string[] Name;
        public double[] DataPct;
        public int[] DataPic;
        public double[] Weight;
        //public Bitmap[]  img ;
        public List<Bitmap>[] IMG;



    }

    }
