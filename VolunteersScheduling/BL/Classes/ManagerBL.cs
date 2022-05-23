using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using MODELS;

namespace BL.Classes
{
    public class ManagerBL:VolunteersSchedulingBL
    {
        DBConnection dbCon;
        List<MODELS.ManagerModel> listOfManagers;

        public ManagerBL()
        {
            dbCon = new DBConnection();
            listOfManagers =ConvertListToModel(dbCon.GetDbSet<manager>().ToList());
        }

        public List<MODELS.ManagerModel> GetAllManagers()
        {
            return listOfManagers;
        }

        public string InsertManager(MODELS.ManagerModel manager1)
        {
            if (listOfManagers.Find(m => m.manager_Id == manager1.manager_Id) == null)
                try
                {
                    dbCon.Execute<manager>(ConvertManagerToEF(manager1),
                    DBConnection.ExecuteActions.Insert);
                    listOfManagers = ConvertListToModel(dbCon.GetDbSet<manager>().ToList());
                    return listOfManagers.First(m => m.manager_Id == manager1.manager_Id).manager_Id;
                }
                catch (Exception ex)
                {
                    return "";
                }
            return listOfManagers.First(m => m.manager_Id == manager1.manager_Id).manager_Id;
        } 

        public string UpdateManager(MODELS.ManagerModel manager1)
        {
            if (listOfManagers.Find(m => m.manager_Id == manager1.manager_Id) != null)
                try
                {
                    dbCon.Execute<manager>(ConvertManagerToEF(manager1),
                    DBConnection.ExecuteActions.Update);
                    listOfManagers = ConvertListToModel(dbCon.GetDbSet<manager>().ToList());
                    return listOfManagers.First(m => m.manager_Id == manager1.manager_Id).manager_Id;
                }
                catch (Exception ex)
                {
                    return "";
                }
            return listOfManagers.First(m => m.manager_Id == manager1.manager_Id).manager_Id;
        } 

        public bool DeleteManager(string managerID)
        {
            ManagerModel manager1 = listOfManagers.First(manager => manager.manager_Id == managerID);
            if (manager1!= null)
                try
                {
                    dbCon.Execute<manager>(ConvertManagerToEF(manager1),
                    DBConnection.ExecuteActions.Insert);
                    listOfManagers = ConvertListToModel(dbCon.GetDbSet<manager>().ToList());
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            return false;
        } 

        #region convert functions
        public static manager ConvertManagerToEF(MODELS.ManagerModel m)
        {
            return new manager
            {
                manager_ID = m.manager_Id,
                manager_full_name = m.manager_full_name,
                manager_email = m.manager_email,
                manager_phone = m.manager_phone,
                manager_org_code = m.manager_org_code,
                manager_password = m.manager_password,
                is_general_manager = m.is_general_manager,
            };
        }
        public static MODELS.ManagerModel ConvertManagerToModel(manager m)
        {
            return new MODELS.ManagerModel
            {
                manager_Id = m.manager_ID,
                manager_full_name = m.manager_full_name,
                manager_email = m.manager_email,
                manager_phone = m.manager_phone,
                manager_org_code = m.manager_org_code,
                manager_password = m.manager_password,
                is_general_manager = m.is_general_manager,
            };
        }

        public static List<MODELS.ManagerModel> ConvertListToModel(List<manager> li)
        {
            return li.Select(l => ConvertManagerToModel(l)).ToList();
        } 
        #endregion
    }  
}
