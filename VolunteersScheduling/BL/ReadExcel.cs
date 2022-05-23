using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using BL;

namespace BL
{
    public class ReadExcel
    {
        public int MyProperty { get; set; }
        public Excel.Application xlApp { get; set; }
        public Excel.Workbook xlWorkBook { get; set; }
        public Excel.Worksheet xlWorkSheet { get; set; }
        public Dictionary<string, List<string>> words { get; set; }
        public Dictionary<string, List<string>> details { get; set; }
        public Dictionary<string, int> DictionaryColumns { get; set; }
        public Dictionary<string, string> valuesString { get; set; }
        //פתיחת קובץ האקסל 
        public void open(string path)
        {
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();

            xlApp = new Excel.Application();
            xlWorkBook = xlApp.Workbooks.Open(path, 0, true, 5, "", "", true, Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

        }
        //סגירת קובץ האקסל
        public void close()
        {
            xlWorkBook.Close(true, null, null);
            xlApp.Quit();
            Marshal.ReleaseComObject(xlWorkSheet);
            Marshal.ReleaseComObject(xlWorkBook);
            Marshal.ReleaseComObject(xlApp);
        }

        public void FillWordsForVolunteer()
        {

            using (StreamReader sr = File.OpenText((@"D:\תיקיית הורדות\לימודי תומס\טז אדר א\VolunteersScheduling\BL\Volunteers.txt")))
            {
                words = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(sr.ReadToEnd());
            }
            using (StreamReader sr = File.OpenText((@"D:\תיקיית הורדות\לימודי תומס\טז אדר א\VolunteersScheduling\BL\VolunteeringDetails.txt")))
            {
                details=JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(sr.ReadToEnd());
            }
        }

        public void FillWordsForNeedy()
        {

            using (StreamReader sr = File.OpenText((@"D:\תיקיית הורדות\לימודי תומס\טז אדר א\VolunteersScheduling\BL\Needies.txt")))
            {
                words = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(sr.ReadToEnd());
            }
            using (StreamReader sr = File.OpenText((@"D:\תיקיית הורדות\לימודי תומס\טז אדר א\VolunteersScheduling\BL\NeedinessDetails.txt")))
            {
                details = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(sr.ReadToEnd());

            }
        }


        //מילוי מילון שמות העמודות
        //הפונקציה מוצאת עבור כל מפתח במילון המבטא שדה בטבלה במסד הנתונים מהי מספר העמודה המתאימה לה
        //הפונקציה מחזירה תשובה לשאלה האם לכל השדות בטבלה זו במסד הנתונים יש עמודה מתאימה בטבלה
        public bool FindColumnsOfNeedy()
        {
            FillWordsForNeedy();
            DictionaryColumns = new Dictionary<string, int>();
            for (int cCnt = 1; cCnt <= xlWorkSheet.UsedRange.Columns.Count; cCnt++)
            {
                string st1 = (string)(xlWorkSheet.UsedRange.Cells[1/*שורה*/, cCnt/*עמודה*/] as Excel.Range).Value2;
                foreach (var item in words)
                {
                    if (item.Value.Contains(st1))
                    {
                        DictionaryColumns[item.Key] = cCnt;
                        break;
                    }
                }
                foreach (var item in details)
                {
                    if (item.Value.Contains(st1))
                    {
                        DictionaryColumns[item.Key] = cCnt;
                        break;
                    }
                }
            }
            if (DictionaryColumns.Count < words.Count)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool FindColumnsOfVolunteer()
        {
            FillWordsForVolunteer();
            DictionaryColumns = new Dictionary<string, int>();
            for (int cCnt = 1; cCnt <= xlWorkSheet.UsedRange.Columns.Count; cCnt++)
            {
                string st1 = (string)(xlWorkSheet.UsedRange.Cells[1/*שורה*/, cCnt/*עמודה*/] as Excel.Range).Value2;
                foreach (var item in words)
                {
                    if (item.Value.Contains(st1))
                    {
                        DictionaryColumns[item.Key] = cCnt;
                        break;
                    }
                }
                foreach (var item in details)
                {
                    if (item.Value.Contains(st1))
                    {
                        DictionaryColumns[item.Key] = cCnt;
                        break;
                    }
                }
            }
            if (DictionaryColumns.Count < words.Count)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}