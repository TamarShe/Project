using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using MODELS;
using Newtonsoft.Json;

namespace BL.Classes
{
    public class NeedyBL : VolunteersSchedulingBL
    {
        DBConnection dbCon;
        List<MODELS.NeedyModel> listOfNeedies;


        public NeedyBL()
        {
            dbCon = new DBConnection();
            listOfNeedies = ConvertListToModel(dbCon.GetDbSet<needy>().ToList());
        }

        public List<MODELS.NeedyModel> GetAllNeedies()
        {
            return listOfNeedies;
        }

        public string InsertNeedy(MODELS.NeedyModel needy1)
        {
            try
            {
                dbCon.Execute<needy>(ConvertNeedyToEF(needy1),
                DBConnection.ExecuteActions.Insert);
                listOfNeedies = ConvertListToModel(dbCon.GetDbSet<needy>().ToList());
                return needy1.needy_ID;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public string UpdateNeedy(MODELS.NeedyModel needy1)
        {
            if (listOfNeedies.Find(n => n.needy_ID == needy1.needy_ID) != null)
                try
                {
                    dbCon.Execute<needy>(ConvertNeedyToEF(needy1),
                    DBConnection.ExecuteActions.Update);
                    listOfNeedies = ConvertListToModel(dbCon.GetDbSet<needy>().ToList());
                    return listOfNeedies.First(n => n.needy_ID == needy1.needy_ID).needy_ID;
                }
                catch (Exception ex)
                {
                    return "";
                }
            return listOfNeedies.First(n => n.needy_ID == needy1.needy_ID).needy_ID;
        }

        public bool DeleteNeedy(string needy_ID)
        {
            NeedyModel needy1 = listOfNeedies.Find(n => n.needy_ID == needy_ID);
            if (needy1 != null)
                try
                {
                    dbCon.Execute<needy>(ConvertNeedyToEF(needy1),
                    DBConnection.ExecuteActions.Update);
                    listOfNeedies = ConvertListToModel(dbCon.GetDbSet<needy>().ToList());
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            return false;
        }

        Random random = new Random();

        public string[,] InsertNeediesFromExcelFile(string path,int orgCode)
        {
            Dictionary<string, List<string>> words;
            ReadExcel excel = new ReadExcel();
            //WriteToExcel writeToExcel = new WriteToExcel();
            int mone = 0;            
            excel.open(path);
            NeedyModel newNeedy;
            NeedinessDetailsModel needinessDetailsModel;
            NeedinessDetailsBL needinessDetailsBL = new NeedinessDetailsBL();
            int randomPassword;
            string[,] passwords = new string[0, 2];
            List<string[]> list = new List<string[]>();  

            if (excel.FindColumnsOfNeedy())
            {
                for (int rCnt = 2; rCnt <= excel.xlWorkSheet.UsedRange.Rows.Count; rCnt++)
                {
                    randomPassword = random.Next(10000, 99999);
                    newNeedy = new NeedyModel();
                    newNeedy.needy_ID = (excel.xlWorkSheet.UsedRange.Cells[rCnt/*שורה*/, excel.DictionaryColumns["needy_ID"]] as Microsoft.Office.Interop.Excel.Range).Value2.ToString();
                    newNeedy.needy_full_name = (excel.xlWorkSheet.UsedRange.Cells[rCnt/*שורה*/, excel.DictionaryColumns["needy_full_name"]] as Microsoft.Office.Interop.Excel.Range).Value2.ToString();
                    newNeedy.needy_address = (excel.xlWorkSheet.UsedRange.Cells[rCnt/*שורה*/, excel.DictionaryColumns["needy_address"]] as Microsoft.Office.Interop.Excel.Range).Value2.ToString();
                    newNeedy.needy_email = (excel.xlWorkSheet.UsedRange.Cells[rCnt/*שורה*/, excel.DictionaryColumns["needy_email"]] as Microsoft.Office.Interop.Excel.Range).Value2.ToString();
                    newNeedy.needy_phone = (excel.xlWorkSheet.UsedRange.Cells[rCnt/*שורה*/, excel.DictionaryColumns["needy_phone"]] as Microsoft.Office.Interop.Excel.Range).Value2.ToString();

                    while (this.CheckIfPasswordIsFree(newNeedy.needy_ID,randomPassword.ToString()))
                    {
                        randomPassword = random.Next(10000, 99999);
                    }

                    newNeedy.needy_password = randomPassword.ToString();
                    if (this.InsertNeedy(newNeedy) != "") mone++;
                    string[] row = new string[2];
                    row[0]= newNeedy.needy_ID;
                    row[1] = newNeedy.needy_password;
                    list.Add(row);



                    needinessDetailsModel = new NeedinessDetailsModel();
                    needinessDetailsModel.needy_ID = newNeedy.needy_ID;
                    needinessDetailsModel.org_code = orgCode;
                    if(excel.DictionaryColumns.ContainsKey("details"))
                    {
                        needinessDetailsModel.details = (excel.xlWorkSheet.UsedRange.Cells[rCnt/*שורה*/, excel.DictionaryColumns["details"]] as Microsoft.Office.Interop.Excel.Range).Value2.ToString();
                    }
                    else
                    {
                        needinessDetailsModel.details = "";
                    }
                    if (excel.DictionaryColumns.ContainsKey("monthly_hours"))
                    {
                        needinessDetailsModel.weekly_hours =Convert.ToDouble((excel.xlWorkSheet.UsedRange.Cells[rCnt/*שורה*/, excel.DictionaryColumns["monthly_hours"]] as Microsoft.Office.Interop.Excel.Range).Value2.ToString());
                    }
                    else
                    {
                        needinessDetailsModel.weekly_hours = 0;
                    }
                    needinessDetailsBL.Insertneediness_detailss(needinessDetailsModel);          
                }
                excel.close();
            }
            string[,] passwordsMatrix = new string[mone, 2];
            for (int i = 0; i < mone; i++)
            {
                passwordsMatrix[i, 0] = list[i][0].ToString();
                passwordsMatrix[i, 1] = list[i][1].ToString();
            }
            return passwordsMatrix;
        }

        #region convret function
        public static needy ConvertNeedyToEF(MODELS.NeedyModel n)
        {
            return new needy
            {
                needy_ID = n.needy_ID,
                needy_full_name = n.needy_full_name,
                needy_phone = n.needy_phone,
                needy_address = n.needy_address,
                needy_email = n.needy_email,
                needy_password = n.needy_password
            };
        }
        public static MODELS.NeedyModel ConvertNeedyToModel(needy n)
        {
            return new MODELS.NeedyModel
            {
                needy_ID = n.needy_ID,
                needy_full_name = n.needy_full_name,
                needy_phone = n.needy_phone,
                needy_address = n.needy_address,
                needy_email = n.needy_email,
                needy_password = n.needy_password
            };
        }

        public static List<MODELS.NeedyModel> ConvertListToModel(List<needy> li)
        {
            return li.Select(l => ConvertNeedyToModel(l)).ToList();
        }


        #endregion

        public bool CheckIfPasswordIsFree(string userID, string password)
        {
            VolunteerBL volunteerBL = new VolunteerBL();
            ManagerBL managerBL = new ManagerBL();
            bool exist = false;
            if (volunteerBL.GetAllvolunteers().Find(v => v.volunteer_ID == userID && v.volunteer_password == password) != null) exist = true;
            else if (managerBL.GetAllManagers().Find(m => m.manager_Id == userID && m.manager_password == password) != null) exist = true;

            return exist;
        }


    }
}