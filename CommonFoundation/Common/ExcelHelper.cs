using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HPSF;

namespace CommonFoundation.Common
{
    public class ExcelHelper
    {
        /// <summary>
        ///  读取execl，.xls   HSSFWorkbook workbook = new HSSFWorkbook(file);
        ///  2007以后.xlsx  XSSFWorkbook workbook = new XSSFWorkbook(file);
        ///  手动修改的后缀只能按原来版本，如xls手动改成xlsx,需要通过HSSFWorkbook   
        /// </summary>
        /// <param name="fullPath">excel全路径，包含文件名.后缀</param>
        /// <param name="sheetName">要读取的sheet表名，空则默认读取第一个</param>
        /// <returns>读取sheet表整个信息</returns>
        public static DataTable GetExcelData(string fullPath, string sheetName = null)
        {
            IWorkbook workbook = null;
            #region//初始化信息
            try
            {
                using (FileStream file = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                {
                    if (fullPath.ToLower().EndsWith(".xls"))
                    {
                        workbook = new HSSFWorkbook(file);
                    }
                    else if (fullPath.ToLower().EndsWith(".xlsx"))
                    {
                        workbook = new XSSFWorkbook(file);
                    }
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            #endregion
            return ExeclToDataTable(workbook, sheetName);
        }
        /// <summary>
        /// 读取execl
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        private static DataTable ExeclToDataTable(IWorkbook workbook, string sheetName = null)
        {
            DataTable dataTable = new DataTable();

            try
            {
                //获取excel的第一个sheet
                ISheet sheet = workbook.GetSheet(sheetName);

                //如果通过sheet名取不到，则去execl第一个sheet
                if (sheet == null)
                {
                    sheet = workbook.GetSheetAt(0);
                }

                //获取sheet的首行
                IRow headerRow = sheet.GetRow(sheet.FirstRowNum);

                //一行最后一个方格的编号 即总的列数
                int cellCount = headerRow.LastCellNum;

                for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                {
                    DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                    dataTable.Columns.Add(column);
                }
                //最后一列的标号  即总的行数
                int rowCount = sheet.LastRowNum;

                for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);

                    if (row.Cells.Count == cellCount)
                    {
                        DataRow dataRow = dataTable.NewRow();

                        for (int j = row.FirstCellNum; j < cellCount; j++)
                        {
                            if (row.GetCell(j) != null)
                                dataRow[j] = row.GetCell(j).ToString();
                        }

                        dataTable.Rows.Add(dataRow);
                    }
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            return dataTable;

        }

        /// <summary>
        /// 创建EXCEL文档
        /// </summary>
        /// <param name="path">保存路径</param>
        /// <param name="dt">DataTable</param>
        public static void CreatExcel(string fullPath, DataTable dt)
        {
            HSSFWorkbook hssfworkbook = new HSSFWorkbook();
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "主题";
            hssfworkbook.SummaryInformation = si;

            NPOI.SS.UserModel.ISheet hssfSheet = hssfworkbook.CreateSheet("Sheet");
            hssfSheet.DefaultColumnWidth = 18;
            NPOI.SS.UserModel.ICellStyle cellStyle = hssfworkbook.CreateCellStyle();

            cellStyle.Alignment = HorizontalAlignment.Center;
            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            //表头
            NPOI.SS.UserModel.IRow tagRow = hssfSheet.CreateRow(0);
            tagRow.RowStyle = cellStyle;
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                tagRow.CreateCell(i).SetCellValue(dt.Columns[i].ColumnName);
                tagRow.GetCell(i).CellStyle = cellStyle;
            }
            int rowNum = 1;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                NPOI.SS.UserModel.IRow row = hssfSheet.CreateRow(rowNum);
                if (rowNum == 50001)//超过五万条数据,新建一张工作表
                {
                    hssfSheet = hssfworkbook.CreateSheet();
                    rowNum = 1;
                    NPOI.SS.UserModel.IRow newrow = hssfSheet.CreateRow(0);
                    row = hssfSheet.CreateRow(1);
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        newrow.CreateCell(j).SetCellValue(dt.Columns[j].ColumnName.ToString());
                    }
                }
                //写入记录
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    row.CreateCell(j).SetCellValue(dt.Rows[i][dt.Columns[j].ColumnName].ToString());
                }
                rowNum++;
            }
            string fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";//文件名称
            FileStream file = new FileStream(fullPath + fileName, FileMode.Create);//路径格式为：@"C:\Users\wangzl\Desktop\picture\"
            hssfworkbook.Write(file);
            file.Close();
        }
    }
}
