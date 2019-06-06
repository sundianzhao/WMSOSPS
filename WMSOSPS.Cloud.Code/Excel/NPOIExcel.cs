using System.Data;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util; 
using System.Web;
using System;
using System.Linq;
using System.Text;
using NPOI.HSSF.Util;
using WMSOSPS.Cloud.Code.Extend;

namespace WMSOSPS.Cloud.Code.Excel
{
    public class NPOIExcel
    {
        private string _title;
        private string _sheetName;
        private string _filePath;
        private string[] _headersTitle;

        /// <summary>
        /// 导出到Excel
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public bool ToExcel(DataTable table)
        {
            FileStream fs = new FileStream(this._filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            IWorkbook workBook = new HSSFWorkbook();
            this._sheetName = this._sheetName.IsEmpty() ? "sheet1" : this._sheetName;
            ISheet sheet = workBook.CreateSheet(this._sheetName);

            //处理表格标题
            IRow row = sheet.CreateRow(0);
            row.CreateCell(0).SetCellValue(this._title);
            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, table.Columns.Count - 1));
            row.Height = 500;

            ICellStyle cellStyle = workBook.CreateCellStyle();
            IFont font = workBook.CreateFont();
            font.FontName = "微软雅黑";
            font.FontHeightInPoints = 17;
            cellStyle.SetFont(font);
            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            cellStyle.Alignment = HorizontalAlignment.Center;
            row.Cells[0].CellStyle = cellStyle;

            //处理表格列头
            row = sheet.CreateRow(1);
            for (int i = 0; i < table.Columns.Count; i++)
            {
                row.CreateCell(i).SetCellValue(table.Columns[i].ColumnName);
                row.Height = 350;
                sheet.AutoSizeColumn(i);
            }

            //处理数据内容
            for (int i = 0; i < table.Rows.Count; i++)
            {
                row = sheet.CreateRow(2 + i);
                row.Height = 250;
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    row.CreateCell(j).SetCellValue(table.Rows[i][j].ToString());
                    sheet.SetColumnWidth(j, 256 * 15);
                }
            }

            //写入数据流
            workBook.Write(fs);
            fs.Flush();
            fs.Close();

            return true;
        }

        /// <summary>
        /// 导出到Excel
        /// </summary>
        /// <param name="table"></param>
        /// <param name="title"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public bool ToExcel(DataTable table, string title, string sheetName, string filePath)
        {
            this._title = title;
            this._sheetName = sheetName;
            this._filePath = filePath;
            return ToExcel(table);
        }

        public bool ToExcel(DataTable table, string[] headersTitle, string title, string sheetName, string filePath)
        {
            this._title = title;
            this._sheetName = sheetName;
            this._headersTitle = headersTitle;
            this._filePath = filePath;
            return Export(table, headersTitle, title);
        }
        public bool Export(DataTable dtSource, string[] headersTitle, string strHeaderText)
        {
            FileStream fs = new FileStream(this._filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            HSSFWorkbook workbook = new HSSFWorkbook();
            this._sheetName = this._sheetName.IsEmpty() ? "sheet1" : this._sheetName;
            ISheet sheet = workbook.CreateSheet(this._sheetName);
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
            //using (MemoryStream ms = new MemoryStream())
            //{
            //    workbook.Write(ms);
            //    ms.Flush();
            //    ms.Position = 0;
            //    //sheet.Dispose();
            //    //workbook.Dispose();
            //    return true;
            //}
            //写入数据流
            workbook.Write(fs);
            fs.Flush();
            fs.Close();
            return true;
        }








        //导出Excel
        public void GetExportExcel(HttpResponse r, DataTable dt, string FileName)
        {
            if (dt != null)
            {
                HttpResponse resp;
                resp = r;
                resp.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
                resp.ContentType = "text/plain";
                resp.AppendHeader("Content-Disposition", "attachment;filename=" + FileName);
                string colHeaders = "", ls_item = "";
                DataRow[] myRow = dt.Select();//可以类似dt.Select("id>10")之形式达到数据筛选目的
                int i = 0;
                int cl = dt.Columns.Count;
                //取得数据表各列标题，各标题之间以\t分割，最后一个列标题后加回车符 
                for (i = 0; i < cl; i++)
                {
                    if (i == (cl - 1))//最后一列，加\n
                    {
                        colHeaders += dt.Columns[i].Caption.ToString() + "\n";
                    }
                    else
                    {
                        colHeaders += dt.Columns[i].Caption.ToString() + "\t";
                    }
                }
                if (colHeaders == "")
                    colHeaders = "数据导出异常";
                resp.Write(colHeaders);
                //向HTTP输出流中写入取得的数据信息 
                //逐行处理数据   
                foreach (DataRow row in myRow)
                {
                    //当前行数据写入HTTP输出流，并且置空ls_item以便下行数据     
                    for (i = 0; i < cl; i++)
                    {
                        if (i == (cl - 1))//最后一列，加\n
                        {
                            if (dt.Columns[i].DataType == typeof(DateTime) && row[i].ToString() != "")
                                ls_item += Convert.ToDateTime(row[i]).ToString("yyyy-MM-dd HH:mm:ss") + "\n";
                            else
                                ls_item += row[i].ToString().Replace("\n", "  ").Replace("\t", "  ") + "\n";
                        }
                        else
                        {
                            if (dt.Columns[i].DataType == typeof(DateTime) && row[i].ToString() != "")
                                ls_item += Convert.ToDateTime(row[i]).ToString("yyyy-MM-dd HH:mm:ss") + "\t";
                            else
                                ls_item += row[i].ToString().Replace("\n", "  ").Replace("\t", "  ") + "\t";
                        }

                    }
                    resp.Write(ls_item);
                    ls_item = "";
                }
                resp.End();
            }
        }

    }
}
