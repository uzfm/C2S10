using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
using System.Diagnostics;

namespace MVision
{





    class ReportXLSX
    {


   public  void CreateXlsx(DTchart dTchart, string URL_Save) {


        var reportExcel = new MarketExcelGenerator().Generate(dTchart);
       string DateNameFile = DateTime.Now.ToString("dd_MM_yyyy HH_mm_ss");

            DateNameFile = URL_Save+"\\Report " + DateNameFile + ".xlsx";
            File.WriteAllBytes(DateNameFile, reportExcel);



            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = DateNameFile,
                UseShellExecute = true
            };  Process.Start(psi);



        }


    







  public class DTchart
        {
         public string PeriodFrom { get; set; }
         public string PeriodTo { get; set; }
         public String [] Name { set; get; }
         public DT     [] DT { set; get; }
    } 

         public class DT{
        public string [] Value { set; get; }
       }









    public class MarketExcelGenerator
    {

            public byte[] Generate(DTchart dTchart)
            {

      
                    // If you have a commercial license
                    ExcelPackage.LicenseContext = LicenseContext.Commercial;

                // If you're using EPPlus for non-commercial purposes
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      using (var package = new ExcelPackage(new FileInfo("MyWorkbook.xlsx"))) {

                var sheet = package.Workbook.Worksheets.Add("Report"); //name List

                sheet.Cells["B3"].Value = "C2S10 Analyzer";
                sheet.Cells["B3"].Style.Font.Size = 26;
                sheet.Cells["B3"].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                sheet.Row(3).Height = 30; 

                sheet.Cells["B4"].Value = "Period from : " + dTchart.PeriodFrom;
                sheet.Cells["B4"].Style.Font.Bold = true;
                sheet.Cells["B4"].Style.Font.Color.SetColor(System.Drawing.Color.Gray);

                sheet.Cells["B5"].Value = "Period to : " + dTchart.PeriodTo;
                sheet.Cells["B5"].Style.Font.Bold = true;
                sheet.Cells["B5"].Style.Font.Color.SetColor(System.Drawing.Color.Gray);

                DateTime dateOnly = new DateTime();
                dateOnly = DateTime.Now;
                sheet.Cells["B6"].Value = "Date of creation : " + dateOnly.ToString();
                sheet.Cells["B6"].Style.Font.Bold = true;
                sheet.Cells["B6"].Style.Font.Color.SetColor(System.Drawing.Color.Black);

                //Name Chart - Fulling 
                sheet.Cells[8, 2, 8, dTchart.Name.Length + 2].LoadFromArrays(new object[][] { dTchart.Name });
                sheet.Cells[8, 2, 8, dTchart.Name.Length + 2].Style.WrapText = true; // перенос по строкам
                sheet.Cells[8, 2, 8, dTchart.Name.Length + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                //заміна коліру заголовка 11-12
                sheet.Cells[8, 2, 8, 11].Style.Font.Color.SetColor(System.Drawing.Color.Black);
                sheet.Cells[8, 12, 8, dTchart.Name.Length + 2].Style.Font.Color.SetColor(System.Drawing.Color.DarkGreen);

                sheet.Cells[8, 2, 8, dTchart.Name.Length + 2].Style.Font.Bold = true;
                sheet.Cells[8, 2, 8, dTchart.Name.Length + 2].Style.Font.Size = 12;
                sheet.Cells[8, 2, 8, 5].Style.Font.UnderLine = true;
                sheet.Row(8).Height = 30;

                    //Chart - Fulling
                    var row = 9;
                var column = 2;
                int ID = 1;
                foreach (DT item in dTchart.DT) {
                    //********* " string char " separate write  *******//
                    sheet.Cells[row, column - 1].Value = ID++;
                    int i = 0;
                    sheet.Cells[row, column + i].Value = item.Value[i];
                    i++;
                    sheet.Cells[row, column + i].Value = item.Value[i];
                    i++;
                    sheet.Cells[row, column + i].Value = item.Value[i];
                    i++;
                    sheet.Cells[row, column + i].Value = item.Value[i];
                    i++;
                    sheet.Cells[row, column + i].Value = item.Value[i];
                    i++;
                    sheet.Cells[row, column + i].Value = item.Value[i];
                    i++;
                    sheet.Cells[row, column + i].Value = item.Value[i];
                    i++;
                    sheet.Cells[row, column + i].Value = item.Value[i];
                    i++;
                    sheet.Cells[row, column + i].Value = item.Value[i];
                    i++;
                    sheet.Cells[row, column + i].Value = item.Value[i];
                    i++;
                    //***********************************************//
                    try {
                        for (; i < item.Value.Length; i++){
                            if (item.Value[i] != "")
                            {
                                sheet.Cells[row, column + i].Value = Convert.ToDouble(item.Value[i]);
                            }
                        }
                    } catch { Help.ErrorMesag("Input string was not in correct format (XLSX)"); }
                    row++;  }
              

                for (int i = 3; i < dTchart.Name.Length; i++) {
                    if (6 < i) { sheet.Column(i).Width = 14; }
                    else { sheet.Column(i).Width = 10; } }

                sheet.Column(1).Width = 5;
                sheet.Column(2).Width = 19;

                //sheet.Cells[9, 4, 9 + dTchart.Name.Length, 4].Style.Numberformat.Format = "yyyy";
                //sheet.Cells[9, 2, 9 + dTchart.Name.Length, 2].Style.Numberformat.Format = "### ### ### ##0";

                //sheet.Column(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                //sheet.Cells[8, 3, 8 + report.History.Length, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //sheet.Column(4).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                //sheet.Cells[8, 2, 8 + report.History.Length, 4].Style.Border.BorderAround(ExcelBorderStyle.Double);
                //sheet.Cells[8, 2, 8, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                //var capitalizationChart = sheet.Drawings.AddChart("FindingsChart", OfficeOpenXml.Drawing.Chart.eChartType.Line);
                //capitalizationChart.Title.Text = "Capitalization";
                //capitalizationChart.SetPosition(7, 0, 5, 0);
                //capitalizationChart.SetSize(800, 400);
                //var capitalizationData = (ExcelChartSerie)(capitalizationChart.Series.Add(sheet.Cells["B9:B28"], sheet.Cells["D9:D28"]));
                //capitalizationData.Header = report.Company.Currency;

                sheet.Protection.IsProtected = true;
                    return package.GetAsByteArray();
                }
            return null;
        }
    }


    }

}





