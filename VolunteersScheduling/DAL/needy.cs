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
    
    public partial class needy
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public needy()
        {
            this.neediness_details = new HashSet<neediness_details>();
        }
    
        public string needy_ID { get; set; }
        public string needy_full_name { get; set; }
        public string needy_address { get; set; }
        public string needy_phone { get; set; }
        public string needy_email { get; set; }
        public string needy_password { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<neediness_details> neediness_details { get; set; }
    }
}