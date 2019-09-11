using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using System.Data;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using UI.Web;

/// <summary>
///ExcelHelper 的摘要说明
/// </summary>
public static class ExcelHelper
{
    public struct HeaderStruct
    {
        public string headerText;
        public int rowSpan;
        public int colSpan;

        public HeaderStruct(string headerText, int rowSpan, int colSpan)
        {
            this.headerText = headerText;
            this.rowSpan = rowSpan;
            this.colSpan = colSpan;
        }
    }
    public static byte[] DataSetToExcel(DataSet ds, List<KeyValuePair<int, List<HeaderStruct>>> listTitle, List<string> colsNameList)
    {

        /* */
        
        Workbook workbook = new HSSFWorkbook();
        int rowIndex = 0;

        foreach (DataTable table in ds.Tables)
        {
            Sheet sheet = string.IsNullOrEmpty(table.TableName) ? workbook.CreateSheet() : workbook.CreateSheet(table.TableName);
            rowIndex = 0;
            if (listTitle != null && listTitle.Count > 0) rowIndex = initHeader(listTitle, sheet);//初始化表头
            else
            {
                Row headerRow = sheet.CreateRow(rowIndex);
                headerRow = sheet.CreateRow(rowIndex);
                rowIndex++;
                // handling header.
                foreach (DataColumn column in table.Columns)
                {
                    int colsIndex = 0;
                    colsNameList.ForEach(item =>
                    {
                        headerRow.CreateCell(colsIndex++).SetCellValue(StringHelper.DynamicToString(item));
                    });
                    rowIndex++;
                }
            }
            // handling value.
            foreach (DataRow row in table.Rows)
            {
                Row dataRow = sheet.CreateRow(rowIndex);
                int colsIndex = 0;
                colsNameList.ForEach(item =>
                {
                    dataRow.CreateCell(colsIndex++).SetCellValue(StringHelper.DynamicToString(row[item]));
                });
                rowIndex++;
            }

            AutoSizeColumns(sheet);
        }
        MemoryStream ms = new MemoryStream();

        workbook.Write(ms);

        
        byte[] buf = ms.ToArray();
        return buf;
    }
    private static MemoryStream RenderToExcel(DataSet ds, List<KeyValuePair<int, List<HeaderStruct>>> listTitle, List<string> exceptCols)
    {
        MemoryStream ms = new MemoryStream();
        Workbook workbook = new HSSFWorkbook();
        int rowIndex = 0;

        foreach (DataTable table in ds.Tables)
        {
            Sheet sheet = string.IsNullOrEmpty(table.TableName) ? workbook.CreateSheet() : workbook.CreateSheet(table.TableName);
            rowIndex = 0;
            if (listTitle != null && listTitle.Count > 0) rowIndex = initHeader(listTitle, sheet);//初始化表头
            else
            {
                Row headerRow = sheet.CreateRow(rowIndex);
                headerRow = sheet.CreateRow(rowIndex);
                rowIndex++;
                // handling header.
                foreach (DataColumn column in table.Columns)
                {
                    if (exceptCols != null && exceptCols.Contains(column.Caption)) continue;
                    headerRow.CreateCell(column.Ordinal).SetCellValue(column.Caption);//If Caption not set, returns the ColumnName value
                }
            }
            // handling value.

            foreach (DataRow row in table.Rows)
            {
                Row dataRow = sheet.CreateRow(rowIndex);

                foreach (DataColumn column in table.Columns)
                {
                    if (exceptCols != null && exceptCols.Contains(column.Caption)) continue;
                    dataRow.CreateCell(column.Ordinal).SetCellValue(row[column].ToString());
                }

                rowIndex++;
            }

            AutoSizeColumns(sheet);
        }
        workbook.Write(ms);
        ms.Flush();
        ms.Position = 0;
        return ms;
    }
    private static void SaveToFile(MemoryStream ms, string fileName, List<KeyValuePair<int, List<HeaderStruct>>> listTitle, List<string> exceptCols)
    {
        using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
        {
            byte[] data = ms.ToArray();

            fs.Write(data, 0, data.Length);
            fs.Flush();

            data = null;
        }
    }
    public static void SaveToFile(DataSet ds, string fileName, List<KeyValuePair<int, List<HeaderStruct>>> listTitle, List<string> exceptCols)
    {
        MemoryStream ms = RenderToExcel(ds, listTitle, exceptCols);
        SaveToFile(ms, fileName, listTitle, exceptCols);
    }
    private static void RenderToBrowser(MemoryStream ms, string fileName)
    {
        if (System.Web.HttpContext.Current.Request.Browser.Browser == "IE")
            fileName = HttpUtility.UrlEncode(fileName);
        System.Web.HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
        System.Web.HttpContext.Current.Response.ContentEncoding = Encoding.UTF8;
        System.Web.HttpContext.Current.Response.Charset = "";
        System.Web.HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName, Encoding.UTF8));
        System.Web.HttpContext.Current.Response.BinaryWrite(ms.GetBuffer());
        System.Web.HttpContext.Current.Response.End();
    }
    public static void RenderToBrowser(DataSet ds, string fileName, List<KeyValuePair<int, List<HeaderStruct>>> listTitle, List<string> exceptCols)
    {
        MemoryStream ms = RenderToExcel(ds, listTitle, exceptCols);
        RenderToBrowser(ms, fileName);
    }
    private static int initHeader(List<KeyValuePair<int, List<HeaderStruct>>> listTitle, Sheet sheet)
    {
        int counter = 0; ;
        foreach (var item in listTitle)
        {
            setColumnHeader(sheet, counter, item.Key, item.Value);
            counter++;
        }
        return counter;
    }
    private static void setColumnHeader(Sheet sheet, int rowStartIndex, int colStartIndex, List<HeaderStruct> listTitle)
    {
        int colIndex = 0;
        int rowCount = 1;
        Row headerRow = null;
        int beginRow = rowStartIndex;
        for (int i = 0; i < listTitle.Count; i++)
        {
            if (listTitle[i].rowSpan > rowCount)
            {
                rowCount = listTitle[i].rowSpan;
                colStartIndex = i;
            }
        }

        for (int i = 0; i < rowCount; i++)
        {
            if (sheet.GetRow(i + beginRow) == null)
                headerRow = sheet.CreateRow(i + beginRow);
        }

        colIndex = colStartIndex;
        for (int k = 0; k < listTitle.Count; k++)
        {
            HeaderStruct title = listTitle[k];
            for (int i = 0; i < title.rowSpan; i++)
            {
                headerRow = sheet.GetRow(i + beginRow);
                for (int j = 0; j < title.colSpan; j++)
                {
                    headerRow.CreateCell(colIndex + j).SetCellValue(title.headerText);
                }
            }

            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(beginRow, beginRow + title.rowSpan - 1, colIndex, colIndex + title.colSpan - 1));
            colIndex += title.colSpan;
        }
    }

    /// <summary>
    /// 自动设置Excel列宽
    /// </summary>
    /// <param name="sheet">Excel表</param>
    private static void AutoSizeColumns(Sheet sheet)
    {
        if (sheet.PhysicalNumberOfRows > 0)
        {
            Row headerRow = sheet.GetRow(0);

            for (int i = 0, l = headerRow.LastCellNum; i < l; i++)
            {
                sheet.AutoSizeColumn(i);
            }
        }
    }
}