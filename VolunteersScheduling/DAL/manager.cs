//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class manager
    {
        public string manager_ID { get; set; }
        public string manager_full_name { get; set; }
        public string manager_phone { get; set; }
        public string manager_email { get; set; }
        public string manager_password { get; set; }
        public int manager_org_code { get; set; }
        public bool is_general_manager { get; set; }
    
        public virtual organization organization { get; set; }
    }
}
