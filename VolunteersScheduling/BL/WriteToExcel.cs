using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using System.Windows;
using System.IO;

namespace BL
{
    public class WriteToExcel
    {
        public Microsoft.Office.Interop.Excel.Application oXL;
        public Microsoft.Office.Interop.Excel._Workbook oWB;
        public Microsoft.Office.Interop.Excel._Worksheet oSheet;
        public Microsoft.Office.Interop.Excel.Range oRng;
        public object misvalue = System.Reflection.Missing.Value;
        public static int rowsCounter = 0;


        public void GenerateExcel()
        {
            try
            {
                oXL = new Microsoft.Office.Interop.Excel.Application();
                oXL.Visible = true;

                //Get a new workbook.
                oWB = (Microsoft.Office.Interop.Excel._Workbook)(oXL.Workbooks.Add(""));
                oSheet = (Microsoft.Office.Interop.Excel._Worksheet)oWB.ActiveSheet;

                //Add table headers going cell by cell.
                oSheet.Cells[1, 1] = "תז";
                oSheet.Cells[1, 2] = "סיסמה";
                oSheet.get_Range("A1", "D1").Font.Bold = true;
                oSheet.get_Range("A1", "D1").VerticalAlignment =
                    Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
            }
            catch (Exception ex) { }
        }

        public void WriteLine(int rowNumber, int columnNumber, string value)
        {
            oSheet.Cells[rowNumber, columnNumber] = value;
        }


        public void SaveExcel(string path)
        {
           // oRng.EntireColumn.AutoFit();

            oXL.Visible = false;
            oXL.UserControl = false;
            oWB.SaveAs(path, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing,
                false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            oWB.Close();
            oXL.Quit();
        }
    }
}