using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace WMSOSPS.Cloud.Code.Excel
{
    public class NPOIHelper
    {
        #region  DataTable导出到Excel的MemoryStream
        /// <summary>
        /// DataTable导出到Excel的MemoryStream
        /// </summary>
        /// <param name="dtSource">源DataTable</param>
        ///  <param name="headersTitle">列头名称</param>
        /// <param name="strHeaderText">表头文本</param>
        public MemoryStream Export(DataTable dtSource, String[] headersTitle, string strHeaderText)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet();
            #region 右击文件属性相关信息
            //{
            //    DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            //    dsi.Company = "公司名称";
            //    workbook.DocumentSummaryInformation = dsi;
            //    SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            //    si.Author = "zhaolf";
            //    si.ApplicationName = "创建程序信息";
            //    si.LastAuthor = "zhaolf";
            //    si.Comments = "软件部";
            //    si.Title = "报表导出";
            //    si.Subject = "报表导出";
            //    si.CreateDateTime = DateTime.Now;
            //    workbook.SummaryInformation = si;
            //}
            #endregion

            NPOI.SS.UserModel.ICellStyle dateStyle = workbook.CreateCellStyle();
            NPOI.SS.UserModel.IDataFormat format = workbook.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");
            dateStyle.Alignment = HorizontalAlignment.Center;// HorizontalAlign.Center;// CellHorizontalAlignment.CENTER;
            dateStyle.BorderBottom = BorderStyle.Thin;   // CellBorderType.THIN;
            dateStyle.BorderTop = BorderStyle.Thin; //CellBorderType.THIN;
            dateStyle.BorderLeft = BorderStyle.Thin; //CellBorderType.THIN;
            dateStyle.BorderRight = BorderStyle.Thin; //CellBorderType.THIN;
            //取得列宽
            int[] arrColWidth = new int[dtSource.Columns.Count];
            foreach (DataColumn item in dtSource.Columns)
            {
                arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName.ToString()).Length;
            }
            for (int index = 0; index < dtSource.Rows.Count; index++)
            {
                for (int icount = 0; icount < dtSource.Columns.Count; icount++)
                {
                    int intTemp = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[index][icount].ToString()).Length;
                    if (intTemp > arrColWidth[icount])
                    {
                        arrColWidth[icount] = intTemp;
                    }
                }
            }
            int rowIndex = 0;
            foreach (DataRow row in dtSource.Rows)
            {
                #region 新建表，填充表头，填充列头，样式
                if (rowIndex == 65535 || rowIndex == 0)
                {
                    if (rowIndex != 0)
                    {
                        sheet = workbook.CreateSheet();
                    }

                    #region 表头及样式
                    {
                        IRow headerRow = sheet.CreateRow(0);
                        headerRow.HeightInPoints = 25;
                        headerRow.CreateCell(0).SetCellValue(strHeaderText);

                        ICellStyle headStyle = workbook.CreateCellStyle();
                        headStyle.Alignment = HorizontalAlignment.Center;  // CellHorizontalAlignment.CENTER;

                        IFont font = workbook.CreateFont();
                        font.FontHeightInPoints = 20;
                        font.Boldweight = 700;
                        headStyle.SetFont(font);
                        headerRow.GetCell(0).CellStyle = headStyle;
                        sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, dtSource.Columns.Count - 1));
                        //sheet.AddMergedRegion(new Region(0, 0, 0, dtSource.Columns.Count - 1));
                        // headerRow.Dispose();                        
                    }
                    #endregion

                    #region 列头及样式
                    {
                        IRow headerRow = sheet.CreateRow(1);
                        ICellStyle headStyle = workbook.CreateCellStyle();
                        headStyle.Alignment = HorizontalAlignment.Center; // CellHorizontalAlignment.CENTER;
                        // 填充列的背景色
                        headStyle.FillForegroundColor = HSSFColor.Grey50Percent.Index;   // HSSFColor.GREY_50_PERCENT.index;
                        headStyle.FillPattern = FillPattern.SolidForeground;  // CellFillPattern.SOLID_FOREGROUND;
                        headStyle.BorderBottom = BorderStyle.Thin;    // CellBorderType.THIN;
                        headStyle.BorderTop = BorderStyle.Thin;  //CellBorderType.THIN;
                        headStyle.BorderLeft = BorderStyle.Thin;  //CellBorderType.THIN;
                        headStyle.BorderRight = BorderStyle.Thin;  //CellBorderType.THIN;

                        IFont font = workbook.CreateFont();
                        font.FontHeightInPoints = 10;
                        font.Boldweight = 700;
                        headStyle.SetFont(font);
                        for (int index = 0; index < headersTitle.Count(); index++)
                        {
                            headerRow.CreateCell(index).SetCellValue(headersTitle[index]);
                            headerRow.GetCell(index).CellStyle = headStyle;
                            sheet.SetColumnWidth(index, (arrColWidth[index] + 1) * 256);
                        }
                        //for (int i = 0; i < dtSource.Columns.Count; i++)
                        //{
                        //    headerRow.CreateCell(i).SetCellValue(dtSource.Columns[i].ColumnName.ToString());
                        //    headerRow.GetCell(i).CellStyle = headStyle;
                        //    sheet.SetColumnWidth(i, (arrColWidth[i] + 1) * 256);
                        //}


                        //  headerRow.Dispose();
                    }
                    #endregion
                    rowIndex = 2;
                }
                #endregion

                #region 填充内容
                IRow dataRow = sheet.CreateRow(rowIndex);
                foreach (DataColumn column in dtSource.Columns)
                {

                    ICell newCell = dataRow.CreateCell(column.Ordinal);
                    /****-----------------此段是设置Excel表格样式----------------------****/
                    /*
                     * 2014-06-09 
                     * 注释原因：当数据超过334行时，当到达第335行的第10列时程序会发生异常，暂时不知道问题所在,所以先注释
                     * 没有样式，不影响数据的导出
                     */
                    //ICellStyle headStyle = workbook.CreateCellStyle();
                    //headStyle.Alignment = HorizontalAlignment.Center; // CellHorizontalAlignment.CENTER;
                    //// 填充列的背景色
                    //headStyle.Alignment = HorizontalAlignment.Center;  // CellHorizontalAlignment.CENTER;
                    //headStyle.BorderBottom = BorderStyle.Thin;   // CellBorderType.THIN;
                    //headStyle.BorderTop = BorderStyle.Thin;    // CellBorderType.THIN;
                    //headStyle.BorderLeft = BorderStyle.Thin;   // CellBorderType.THIN;
                    //headStyle.BorderRight = BorderStyle.Thin;   // CellBorderType.THIN;
                    //  dataRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                    //dataRow.GetCell(column.Ordinal).CellStyle = headStyle;
                    /****-----------------此段是设置Excel表格样式----------------------****/
                    string drValue = row[column].ToString();
                    switch (column.DataType.ToString())
                    {
                        case "System.String":
                            newCell.SetCellValue(drValue);
                            break;
                        case "System.DateTime":
                            DateTime dateV;
                            DateTime.TryParse(drValue, out dateV);
                            newCell.SetCellValue(dateV);
                            newCell.CellStyle = dateStyle;
                            break;
                        case "System.Boolean":
                            bool boolV = false;
                            bool.TryParse(drValue, out boolV);
                            newCell.SetCellValue(boolV);
                            break;
                        case "System.Int16":
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            int intV = 0;
                            int.TryParse(drValue, out intV);
                            newCell.SetCellValue(intV);
                            break;
                        case "System.Decimal":
                        case "System.Double":
                            double doubV = 0;
                            double.TryParse(drValue, out doubV);
                            newCell.SetCellValue(doubV);
                            break;
                        case "System.DBNull":
                            newCell.SetCellValue("");
                            break;
                        default:
                            newCell.SetCellValue("");
                            break;
                    }
                }
                #endregion
                rowIndex++;
            }
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;
                //sheet.Dispose();
                //workbook.Dispose();
                return ms;
            }
        }

        #region 用于Web导出到EXCEL
        /// <summary>
        /// 用于Web导出:此种方法不支持异步（AJAX）方式
        /// </summary>
        /// <param name="dtSource">源DataTable</param>
        /// <param name="headersTitle">列标题信息集合</param>
        /// <param name="strHeaderText">表头文本</param>
        /// <param name="strFileName">文件名</param>
        public void ExportExcelByWeb(DataTable dtSource, String[] headersTitle, string strHeaderText, string strFileName)
        {
            HttpContext curContext = HttpContext.Current;
            curContext.Response.Clear();
            curContext.Response.Buffer = true;
            //设置Http的头信息,编码格式
            curContext.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlPathEncode(strFileName));
            curContext.Response.ContentType = "application/ms-excel";
            //设置编码
            curContext.Response.Charset = "GB2312";
            curContext.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            MemoryStream ms = new MemoryStream();
            ms = Export(dtSource, headersTitle, strHeaderText);
            curContext.Response.BinaryWrite(ms.GetBuffer());
            curContext.Response.End();
        }
        #endregion
        #endregion
    }
}
