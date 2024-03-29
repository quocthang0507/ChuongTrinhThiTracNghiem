﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace ChuongTrinhThiTracNghiem
{
    class Microsoft_Excel
    {
        int rowCount, colCount;

        /// <summary>
        /// Find and return path of thuvien.xlsx
        /// </summary>
        /// <returns>the path</returns>
        public static string OpenFile()
        {
            OpenFileDialog open_file = new OpenFileDialog();
            open_file.InitialDirectory = Directory.GetCurrentDirectory();
            open_file.Title = "Duyệt đến tập tin thư viện câu hỏi";
            open_file.Filter = "Thư viện câu hỏi|thuvien.xlsx";
            open_file.RestoreDirectory = true;
            DialogResult r = open_file.ShowDialog();
            if (r == DialogResult.OK)
            {
                string file_path = open_file.FileName;
                file_path = new FileInfo(file_path).FullName;
                return file_path;
            }
            else
                return null;
        }

        /// <summary>
        /// Read and get data from excel file to object array
        /// </summary>
        /// <returns>Array of object</returns>
        public object[,] GetDataFromExcel(string path)
        {
            //Create COM Objects. Create a COM object for everything that is referenced
            Excel.Application xlsApp = new Excel.Application();
            Excel.Workbook xlsWorkbook = xlsApp.Workbooks.Open(path);
            Excel.Worksheet xlsWorksheet = xlsWorkbook.Sheets[1]; //excel is not zero based
            Excel.Range xlsRange = xlsWorksheet.UsedRange;

            object[,] valueArray = (object[,])xlsRange.get_Value(Excel.XlRangeValueDataType.xlRangeValueDefault);

            rowCount = xlsWorksheet.UsedRange.Rows.Count;
            colCount = xlsWorksheet.UsedRange.Columns.Count;

            //cleanup
            GC.Collect();
            GC.WaitForPendingFinalizers();

            //release com objects to fully kill excel process from running in the background
            Marshal.ReleaseComObject(xlsRange);
            Marshal.ReleaseComObject(xlsWorksheet);

            //close and release
            xlsWorkbook.Close();
            Marshal.ReleaseComObject(xlsWorkbook);

            //quit and release
            xlsApp.Quit();
            Marshal.ReleaseComObject(xlsApp);

            return valueArray;
        }

        /// <summary>
        /// Convert original data from object array to structured list
        /// </summary>
        /// <param name="path">The path of the data file</param>
        /// <returns></returns>
        public List_Question ToList(string path)
        {
            object[,] array = GetDataFromExcel(path);
            List_Question lq = new List_Question();
            Question q;
            for (int i = 2; i <= rowCount; i = i + 5)
            {
                if (array[i, 1] != null)
                {
                    q = new Question();
                    q.question = array[i, 2].ToString();
                    for (int j = 1; j <= 4; j++)
                    {
                        q.answers.Add(array[i + j, 2].ToString());
                        if (array[i + j, 1] != null)
                            q.correctAns = j;
                    }
                    lq.Add(q);
                }
            }
            return lq;
        }
    }
}
