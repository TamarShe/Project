using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MODELS;

namespace DAL
{
    public class DBConnection
    {
        public DBConnection() { }
        public List<T> GetDbSet<T>() where T : class
        {
            using (volunteers_scheduling_DBEntities volunteers_scheduling_DBEntities = new volunteers_scheduling_DBEntities())
            {
                volunteers_scheduling_DBEntities.Configuration.LazyLoadingEnabled = false;
                return volunteers_scheduling_DBEntities.GetDbSet<T>().ToList();
            }
        }

        public List<T> GetDbSetWithIncludes<T>(string[] includes) where T : class
        {
            using (volunteers_scheduling_DBEntities volunteers_scheduling_DBEntities = new volunteers_scheduling_DBEntities())
            {
                volunteers_scheduling_DBEntities.Configuration.LazyLoadingEnabled = false;

                return volunteers_scheduling_DBEntities.IncludeMultiple<T>(volunteers_scheduling_DBEntities.GetDbSet<T>(),includes).ToList();
            }
        }

        public enum ExecuteActions
        {
            Insert,
            Update,
            Delete
        }

        public void Execute<T>(T entity, ExecuteActions exAction) where T : class
        {
            using (volunteers_scheduling_DBEntities volunteers_scheduling_DBEntities = new volunteers_scheduling_DBEntities())
            {
                var model = volunteers_scheduling_DBEntities.Set<T>();
                switch (exAction)
                {
                    case ExecuteActions.Insert:
                        model.Add(entity);
                        break;
                    case ExecuteActions.Update:
                        model.Attach(entity);
                        volunteers_scheduling_DBEntities.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                        break;
                    case ExecuteActions.Delete:
                        model.Attach(entity);
                        volunteers_scheduling_DBEntities.Entry(entity).State = System.Data.Entity.EntityState.Deleted;
                        break;
                    default:
                        break;
                }
                volunteers_scheduling_DBEntities.SaveChanges();
            }
        }
    }
}
