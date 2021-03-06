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
    
    public partial class time_slot
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public time_slot()
        {
            this.needy_possible_time = new HashSet<needy_possible_time>();
            this.schedules = new HashSet<schedule>();
            this.volunteer_possible_time = new HashSet<volunteer_possible_time>();
        }
    
        public int time_slot_code { get; set; }
        public int day_of_week { get; set; }
        public System.DateTime start_at_date { get; set; }
        public System.DateTime end_at_date { get; set; }
        public int end_at_hour { get; set; }
        public int start_at_hour { get; set; }
    
        public virtual hour hour { get; set; }
        public virtual hour hour1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<needy_possible_time> needy_possible_time { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<schedule> schedules { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<volunteer_possible_time> volunteer_possible_time { get; set; }
    }
}
