using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using MODELS;

namespace BL.Classes
{
    public class VolunteerBL:VolunteersSchedulingBL
    {
        DBConnection dbCon;
        List<VolunteerModel> listOfVolunteerss;

        public VolunteerBL()
        {
            dbCon = new DBConnection();
            listOfVolunteerss = ConvertListToModel(dbCon.GetDbSet<volunteer>().ToList());
        }

        public List<VolunteerModel> GetAllvolunteers()
        {
            return listOfVolunteerss;
        } 

        public string InsertVolunteer(VolunteerModel volunteer1)
        {
            if (listOfVolunteerss.Find(v => v.volunteer_ID == volunteer1.volunteer_ID) == null)
                try
                {
                    dbCon.Execute<volunteer>(ConvertVolunteerToEF(volunteer1),
                    DBConnection.ExecuteActions.Insert);
                    listOfVolunteerss = ConvertListToModel(dbCon.GetDbSet<volunteer>().ToList());
                    return listOfVolunteerss.First(v => v.volunteer_ID == volunteer1.volunteer_ID).volunteer_ID;
                }
                catch (Exception ex)
                {
                    return "";
                }
            return volunteer1.volunteer_ID;
        } 

        public string UpdateVolunteer(VolunteerModel volunteer1)
        {
            if (listOfVolunteerss.Find(v => v.volunteer_ID == volunteer1.volunteer_ID) != null)
                try
                {
                    dbCon.Execute<volunteer>(ConvertVolunteerToEF(volunteer1),
                    DBConnection.ExecuteActions.Update);
                    listOfVolunteerss = ConvertListToModel(dbCon.GetDbSet<volunteer>().ToList());
                    return listOfVolunteerss.First(v => v.volunteer_ID == volunteer1.volunteer_ID).volunteer_ID;
                }
                catch (Exception ex)
                {
                    return "";
                }
            return "";
        } 

        public bool DeleteVolunteer(string volunteerID)
        {
            VolunteerModel volunteer1 = listOfVolunteerss.First(volunteer => volunteer.volunteer_ID == volunteerID);
            if (volunteer1 != null)
                try
                {
                    dbCon.Execute<volunteer>(ConvertVolunteerToEF(volunteer1),
                    DBConnection.ExecuteActions.Delete);
                    listOfVolunteerss = ConvertListToModel(dbCon.GetDbSet<volunteer>().ToList());
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            return false;
        } 

        #region convert functions
        public static volunteer ConvertVolunteerToEF(MODELS.VolunteerModel v)
        {
            return new volunteer
            {
                volunteer_ID = v.volunteer_ID,
                volunteer_full_name = v.volunteer_full_name,
                volunteer_address = v.volunteer_address,
                volunteer_birth_date = v.volunteer_birth_date,
                volunteer_email = v.volunteer_email,
                volunteer_phone = v.volunteer_phone,
                volunteer_password = v.volunteer_password
            };
        }
        public static MODELS.VolunteerModel ConvertVolunteerToModel(volunteer v)
        {
            return new MODELS.VolunteerModel
            {
                volunteer_ID = v.volunteer_ID,
                volunteer_full_name = v.volunteer_full_name,
                volunteer_address = v.volunteer_address,
                volunteer_birth_date = v.volunteer_birth_date,
                volunteer_email = v.volunteer_email,
                volunteer_phone = v.volunteer_phone,
                volunteer_password = v.volunteer_password
            };
        }

        public static List<MODELS.VolunteerModel> ConvertListToModel(List<volunteer> li)
        {
            return li.Select(l => ConvertVolunteerToModel(l)).ToList();
        }
        #endregion

        Random random = new Random();

        public string[,] InsertVolunteersFromExcelFile(string path, int orgCode)
        {
            ReadExcel excel = new ReadExcel();
            int mone = 0;
            excel.open(path);
            VolunteerModel newVolunteer;
            VolunteeringDetailsModel volunteeringDetailsModel;
            VolunteeringDetailsBL volunteeringDetailsBL = new VolunteeringDetailsBL();
            int randomPassword;
            string[,] passwords = new string[0, 2];
            List<string[]> list = new List<string[]>();

            if (excel.FindColumnsOfVolunteer())
            {
                for (int rCnt = 2; rCnt <= excel.xlWorkSheet.UsedRange.Rows.Count; rCnt++)
                {
                    randomPassword = random.Next(10000, 99999);
                    newVolunteer = new VolunteerModel();
                    newVolunteer.volunteer_ID = (excel.xlWorkSheet.UsedRange.Cells[rCnt/*שורה*/, excel.DictionaryColumns["volunteer_ID"]] as Microsoft.Office.Interop.Excel.Range).Value2.ToString();
                    newVolunteer.volunteer_full_name = (excel.xlWorkSheet.UsedRange.Cells[rCnt/*שורה*/, excel.DictionaryColumns["volunteer_full_name"]] as Microsoft.Office.Interop.Excel.Range).Value2.ToString();
                    newVolunteer.volunteer_address = (excel.xlWorkSheet.UsedRange.Cells[rCnt/*שורה*/, excel.DictionaryColumns["volunteer_address"]] as Microsoft.Office.Interop.Excel.Range).Value2.ToString();
                    newVolunteer.volunteer_email = (excel.xlWorkSheet.UsedRange.Cells[rCnt/*שורה*/, excel.DictionaryColumns["volunteer_email"]] as Microsoft.Office.Interop.Excel.Range).Value2.ToString();
                    newVolunteer.volunteer_phone = (excel.xlWorkSheet.UsedRange.Cells[rCnt/*שורה*/, excel.DictionaryColumns["volunteer_phone"]] as Microsoft.Office.Interop.Excel.Range).Value2.ToString();
                    var a= (excel.xlWorkSheet.UsedRange.Cells[rCnt/*שורה*/, excel.DictionaryColumns["volunteer_birth_date"]] as Microsoft.Office.Interop.Excel.Range).Value2;
                    newVolunteer.volunteer_birth_date =Convert.ToDateTime(a);
                    while (this.CheckIfPasswordIsFree(newVolunteer.volunteer_ID, randomPassword.ToString()))
                    {
                        randomPassword = random.Next(10000, 99999);
                    }

                    newVolunteer.volunteer_password = randomPassword.ToString();
                    if (this.InsertVolunteer(newVolunteer) != "") mone++;
                    string[] row = new string[2];
                    row[0] = newVolunteer.volunteer_ID;
                    row[1] = newVolunteer.volunteer_password;
                    list.Add(row);

                    volunteeringDetailsModel = new VolunteeringDetailsModel();
                    volunteeringDetailsModel.volunteer_ID = newVolunteer.volunteer_ID;
                    volunteeringDetailsModel.org_code = orgCode;

                    if (excel.DictionaryColumns.ContainsKey("monthly_hours"))
                    {
                        volunteeringDetailsModel.weekly_hours = Convert.ToDouble((excel.xlWorkSheet.UsedRange.Cells[rCnt/*שורה*/, excel.DictionaryColumns["monthly_hours"]] as Microsoft.Office.Interop.Excel.Range).Value2.ToString());
                    }
                    else
                    {
                        volunteeringDetailsModel.weekly_hours = 0;
                    }
                    volunteeringDetailsBL.InsertVolunteeringDetails(volunteeringDetailsModel);
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


        public bool CheckIfPasswordIsFree(string userID, string password)
        {
            ManagerBL managerBL = new ManagerBL();
            NeedyBL needyBL = new NeedyBL();
            bool exist = false;
            if (needyBL.GetAllNeedies().Find(n => n.needy_ID == userID && n.needy_password == password) != null) exist = true;
            else if (managerBL.GetAllManagers().Find(m => m.manager_Id == userID && m.manager_password == password) != null) exist = true;

            return exist;
        }

    }
}
