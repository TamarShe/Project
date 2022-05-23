using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS
{
    public class ManagerModel
    {
        public string manager_Id { get; set; }
        public string manager_full_name { get; set; }
        public string manager_phone { get; set; }
        public string manager_email { get; set; }
        public string manager_password { get; set; }
        public int manager_org_code { get; set; }
        public bool is_general_manager { get; set; }
    }
}
