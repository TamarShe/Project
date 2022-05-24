using System;
using System.Collections.Generic;
using System.Linq;
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
                return volunteers_scheduling_DBEntities.GetDbSet<T>().ToList();
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
