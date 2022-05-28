﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Linq.Expressions;

    public partial class volunteers_scheduling_DBEntities : DbContext
    {
        public volunteers_scheduling_DBEntities()
            : base("name=volunteers_scheduling_DBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<hour> hours { get; set; }
        public virtual DbSet<manager> managers { get; set; }
        public virtual DbSet<neediness_details> neediness_details { get; set; }
        public virtual DbSet<needy> needies { get; set; }
        public virtual DbSet<needy_possible_time> needy_possible_time { get; set; }
        public virtual DbSet<organization> organizations { get; set; }
        public virtual DbSet<schedule> schedules { get; set; }
        public virtual DbSet<time_slot> time_slot { get; set; }
        public virtual DbSet<volunteer> volunteers { get; set; }
        public virtual DbSet<volunteer_possible_time> volunteer_possible_time { get; set; }
        public virtual DbSet<volunteering_details> volunteering_details { get; set; }

        public DbSet<T> GetDbSet<T>() where T : class
        {
            return this.Set<T>();
        }

        public IQueryable<neediness_details> IncludeMultiple()
        {
            return this.neediness_details.Include("needy_possible_time.time_slot");
        }

        public IQueryable<T> IncludeMultiple<T>(IQueryable<T> query, string[] includes) where T : class
        {
            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }
            return query;
        }
    }
}
