using System;
using System.Collections.Generic;
using System.IO;
using NPOI.SS.UserModel;
using System.ComponentModel;
using NPOI.HSSF.UserModel;
using System.Linq;
using System.Data;
using System.Data.OleDb;
using System.Collections;

namespace BMW.Frameworks
{
    public class ExcelTool
    {
        private ExcelTool()
        {
            // Nothing to do
        }

        /// <summary>
        /// 导出给定列表到Excel中
        /// </summary>
        /// <param name="list">object列表，通常有linq中select new 给出</param>
        /// <param name="cellfields">显示在Excel中的表头</param>
        /// <returns></returns>
        public static MemoryStream ExportToExcel(IList<object> list, string[] cellfields)
        {
            try
            {
                //文件流对象
                MemoryStream stream = new MemoryStream();

                //新建一个工作表
                IWorkbook workbook = new HSSFWorkbook();

                //写入第一行表头
                ISheet sheet1 = workbook.CreateSheet("sheet1");

                IRow HeaderRow = sheet1.CreateRow(0);
                if (cellfields.Length > 0)
                {
                    for (int i = 0; i < cellfields.Length; i++)
                    {
                        HeaderRow.CreateCell(i).SetCellValue(cellfields[i]);
                    }
                }

                //写入数据到表格
                if (list.Count > 0)
                {
                    //反射所有字段
                    PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(list.First().GetType());

                    if (properties.Count > 0)
                    {
                        int rows = 1;
                        foreach (var item in list)
                        {
                            IRow row = sheet1.CreateRow(rows);
                            for (int j = 0; j < properties.Count; j++)
                            {
                                object obj = properties[j].GetValue(item);
                                row.CreateCell(j).SetCellValue(obj == null ? "" : obj.ToString());
                            }
                            rows += 1;
                        }
                    }
                }


                workbook.Write(stream);
                //workbook.Dispose();
                return stream;
            }
            catch
            {
                return new MemoryStream();
            }
        }

        /// <summary>
        /// 导出给定列表到Excel中
        /// </summary>
        /// <param name="list">object列表，通常有linq中select new 给出</param>
        /// <returns></returns>
        public static MemoryStream ExportToExcel(IList<object> list)
        {
            try
            {
                //文件流对象
                MemoryStream stream = new MemoryStream();

                if (list.Count > 0)
                {
                    //反射所有字段
                    PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(list.First().GetType());
                    if (properties.Count > 0)
                    {
                        //新建一个工作表
                        IWorkbook workbook = new HSSFWorkbook();

                        //写入第一行表头
                        ISheet sheet1 = workbook.CreateSheet("sheet1");

                        //工作表表头取反射属性
                        IRow HeaderRow = sheet1.CreateRow(0);

                        for (int i = 0; i < properties.Count; i++)
                        {
                            HeaderRow.CreateCell(i).SetCellValue(properties[i].Name);
                        }


                        //写入数据到表格
                        if (properties.Count > 0)
                        {
                            int rows = 1;
                            foreach (var item in list)
                            {
                                IRow row = sheet1.CreateRow(rows);
                                for (int j = 0; j < properties.Count; j++)
                                {
                                    object obj = properties[j].GetValue(item);
                                    row.CreateCell(j).SetCellValue(obj == null ? "" : obj.ToString());
                                }
                                rows += 1;
                            }
                        }

                        workbook.Write(stream);
                        //workbook.Dispose();
                    }
                }
                return stream;
            }
            catch
            {
                return new MemoryStream();
            }
        }

        /// <summary>
        /// 导入execel到数据库
        /// </summary>
        /// <param name="filepath">文件路径</param>
        /// <param name="sheetname">表名【注意不正确会报错】</param>
        /// <returns></returns>
        public static DataTable ExcelDataSource(string filepath, string sheetname)
        {
            string strConn;
            strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filepath + ";Extended Properties='Excel 8.0;IMEX=1;'";
            OleDbConnection conn = new OleDbConnection(strConn);
            OleDbDataAdapter oada = new OleDbDataAdapter("select *  from [" + sheetname + "$]", strConn);
            DataTable dt = new DataTable();
            oada.Fill(dt);
            DataTable newdt = dt.Clone();

            //过滤空行
            for (int t = 0; t < dt.Rows.Count; t++)
            {
                bool flag = false;

                DataRow dr = dt.Rows[t];
                for (int t1 = 0; t1 < dr.ItemArray.Length; t1++)
                {
                    if (dr.ItemArray[t1].ToString() != "")
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                {
                    newdt.Rows.Add(dr.ItemArray);
                }

            }
            dt = newdt;

            return dt;
        }

        /// <summary>
        /// 获取一个execel内所有的表
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static ArrayList ExcelSheetName(string filepath)
        {
            ArrayList al = new ArrayList();
            string strConn;
            strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filepath + ";Extended Properties=Excel 8.0;";
            OleDbConnection conn = new OleDbConnection(strConn);
            conn.Open();
            DataTable sheetNames = conn.GetOleDbSchemaTable
            (System.Data.OleDb.OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
            conn.Close();
            foreach (DataRow dr in sheetNames.Rows)
            {
                al.Add(dr[2]);
            }
            return al;
        }


        public static DataTable ImportExcelFileUseNPOI(string filePath)
        {
            HSSFWorkbook hssfworkbook;
            #region//初始化信息
            try
            {
                using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    hssfworkbook = new HSSFWorkbook(file);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            #endregion

            ISheet sheet = hssfworkbook.GetSheetAt(0);
            IEnumerator rows = sheet.GetRowEnumerator();
            DataTable dt = new DataTable();
            rows.MoveNext();
            HSSFRow row = (HSSFRow)rows.Current;
            for (int j = 0; j < (sheet.GetRow(0).LastCellNum); j++)
            {
                //dt.Columns.Add(Convert.ToChar(((int)'A') + j).ToString());
                //将第一列作为列表头
                dt.Columns.Add(row.GetCell(j).ToString());
            }
            while (rows.MoveNext())
            {
                row = (HSSFRow)rows.Current;
                DataRow dr = dt.NewRow();
                for (int i = 0; i < row.LastCellNum; i++)
                {
                    ICell cell = row.GetCell(i);
                    if (cell == null)
                    {
                        dr[i] = null;
                    }
                    else
                    {
                        dr[i] = cell.ToString();
                    }
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

    }
}
