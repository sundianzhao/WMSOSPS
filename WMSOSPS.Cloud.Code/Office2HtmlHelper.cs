using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSOSPS.Cloud.Code
{
    public  class Office2HtmlHelper
    { 

        public static void Priview(string inFilePath, string outputFile)
        {
            Microsoft.Office.Interop.Excel.Application excel = null;
            Microsoft.Office.Interop.Excel.Workbook xls = null;
            excel = new Microsoft.Office.Interop.Excel.Application();
            object missing = Type.Missing;
            object trueObject = true;
            excel.Visible = false;
            excel.DisplayAlerts = false;

           // string randomName = DateTime.Now.Ticks.ToString();  //output fileName

            xls = excel.Workbooks.Open(inFilePath, missing, trueObject, missing,
                                        missing, missing, missing, missing, missing, missing, missing, missing,
                                        missing, missing, missing);

            //Save Excel to Html
            object format = Microsoft.Office.Interop.Excel.XlFileFormat.xlHtml;
            Workbook wsCurrent = xls;//(Workbook)wsEnumerator.Current;
            //String outputFile = outDirPath + randomName + ".html";
            wsCurrent.SaveAs(outputFile, format, missing, missing, missing,
                              missing, XlSaveAsAccessMode.xlNoChange, missing,
                              missing, missing, missing, missing);
            excel.Quit();

            ////Open generated Html
            //Process process = new Process();
            //process.StartInfo.UseShellExecute = true;
            //process.StartInfo.FileName = outputFile;
            //process.Start();
        }

        /// <summary>
        /// excel 转换为html
      　/// </summary>
        /// <param name="path">要转换的文档的路径</param>
      　/// <param name="savePath">转换成的html的保存路径</param>
      　/// <param name="wordFileName">转换后html文件的名字</param>
        public static void ExcelToHtml(string path, string savePath, string wordFileName)
        {
            string str = string.Empty;
            Microsoft.Office.Interop.Excel.Application repExcel = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook workbook = null;
            Microsoft.Office.Interop.Excel.Worksheet worksheet = null;
            workbook = repExcel.Application.Workbooks.Open(path, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets[1];
            object htmlFile = savePath + wordFileName + ".html";
            object ofmt = Microsoft.Office.Interop.Excel.XlFileFormat.xlHtml;
            workbook.SaveAs(htmlFile, ofmt, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            object osave = false;
            workbook.Close(osave, Type.Missing, Type.Missing);
            repExcel.Quit();
        }

    }
}
