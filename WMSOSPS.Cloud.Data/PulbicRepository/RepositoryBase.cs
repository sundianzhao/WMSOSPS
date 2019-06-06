using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Z.EntityFramework.Plus;
using WMSOSPS.Cloud.Code.Web;
using WMSOSPS.Cloud.Data.CloudContext;

namespace WMSOSPS.Cloud.Data.PulbicRepository
{
    /// <summary>
    /// 仓储实现
    /// </summary>
    public class RepositoryBase : IRepositoryBase
    {
        protected readonly ExamCloudDBEntities dbcontext = new ExamCloudDBEntities();
        private DbTransaction dbTransaction { get; set; }
        public IRepositoryBase BeginTrans()
        {
            DbConnection dbConnection = ((IObjectContextAdapter)dbcontext).ObjectContext.Connection;
            if (dbConnection.State == ConnectionState.Closed)
            {
                dbConnection.Open();
            }
            dbTransaction = dbConnection.BeginTransaction();
            return this;
        }
        public int Commit()
        {
            try
            {
                var returnValue = dbcontext.SaveChanges();
                if (dbTransaction != null)
                {
                    dbTransaction.Commit();
                }
                return returnValue;
            }
            catch (Exception ex)
            {
                if (dbTransaction != null)
                {
                    this.dbTransaction.Rollback();
                }
                throw;
            }
            finally
            {
                //this.Dispose();
            }
        }
        public void Dispose()
        {
            if (dbTransaction != null)
            {
                this.dbTransaction.Dispose();
            }
            this.dbcontext.Dispose();
        }
        public int Insert<TEntity>(TEntity entity) where TEntity : class
        {
            dbcontext.Entry<TEntity>(entity).State = EntityState.Added;
            return dbTransaction == null ? this.Commit() : 0;
        }
        public int Insert<TEntity>(List<TEntity> entitys) where TEntity : class
        {
            foreach (var entity in entitys)
            {
                dbcontext.Entry<TEntity>(entity).State = EntityState.Added;
            }
            return dbTransaction == null ? this.Commit() : 0;
        }
        public int Update<TEntity>(TEntity entity) where TEntity : class
        {
            dbcontext.Set<TEntity>().Attach(entity);
            PropertyInfo[] props = entity.GetType().GetProperties();
            foreach (PropertyInfo prop in props)
            {
                if (prop.GetValue(entity, null) != null)
                {
                    if (prop.GetValue(entity, null).ToString() == "&nbsp;")
                        dbcontext.Entry(entity).Property(prop.Name).CurrentValue = null;
                    dbcontext.Entry(entity).Property(prop.Name).IsModified = true;
                }
            }
            return dbTransaction == null ? this.Commit() : 0;
        }

        public int UpdateEntity<TEntity>(TEntity entity) where TEntity : class
        {
            dbcontext.Entry<TEntity>(entity).State = System.Data.Entity.EntityState.Modified;
            return dbcontext.SaveChanges();
        }
        //public int Update<TEntity>(List<TEntity> entity) where TEntity : class
        //{
        //    foreach (var item in entity)
        //    {
        //        dbcontext.Set<TEntity>().Attach(item);
        //        PropertyInfo[] props = entity.GetType().GetProperties();
        //        foreach (PropertyInfo prop in props)
        //        {
        //            if (prop.GetValue(entity, null) != null)
        //            {
        //                if (prop.GetValue(entity, null).ToString() == "&nbsp;")
        //                    dbcontext.Entry(entity).Property(prop.Name).CurrentValue = null;
        //                dbcontext.Entry(entity).Property(prop.Name).IsModified = true;
        //            }
        //        }
        //    }
        //    return dbTransaction == null ? this.Commit() : 0;
        //}
        public int Delete<TEntity>(TEntity entity) where TEntity : class
        {
            dbcontext.Set<TEntity>().Attach(entity);
            dbcontext.Entry<TEntity>(entity).State = EntityState.Deleted;
            return dbTransaction == null ? this.Commit() : 0;
        }
        public int Delete<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            var entitys = dbcontext.Set<TEntity>().Where(predicate).ToList();
            entitys.ForEach(m => dbcontext.Entry<TEntity>(m).State = EntityState.Deleted);
            return dbTransaction == null ? this.Commit() : 0;
        }
        public TEntity FindEntity<TEntity>(object keyValue) where TEntity : class
        {
            return dbcontext.Set<TEntity>().Find(keyValue);
        }
        public TEntity FindEntity<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            try
            {
                return dbcontext.Set<TEntity>().FirstOrDefault(predicate);

            }
            catch (Exception ex)
            {
                return null;
                throw;
            };
        }
        public IQueryable<TEntity> IQueryable<TEntity>() where TEntity : class
        {
            return dbcontext.Set<TEntity>();
        }
        public IQueryable<TEntity> IQueryable<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return dbcontext.Set<TEntity>().Where(predicate);
        }
        public IQueryable<TEntity> IQueryableAsNoTracking<TEntity>() where TEntity : class
        {
            return dbcontext.Set<TEntity>().AsNoTracking();
        }
        public IQueryable<TEntity> IQueryableAsNoTracking<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return dbcontext.Set<TEntity>().AsNoTracking().Where(predicate);
        }
        public List<TEntity> FindList<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return dbcontext.Set<TEntity>().Where(predicate).ToList<TEntity>();
        }

        public List<TEntity> FindList<TEntity>(string strSql) where TEntity : class
        {
            return dbcontext.Database.SqlQuery<TEntity>(strSql).ToList<TEntity>();
        }
        public List<TEntity> FindList<TEntity>(string strSql, DbParameter[] dbParameter) where TEntity : class
        {
            return dbcontext.Database.SqlQuery<TEntity>(strSql, dbParameter).ToList<TEntity>();
        }
        public List<TEntity> FindList<TEntity>(Pagination pagination) where TEntity : class, new()
        {
            bool isAsc = pagination.sord.ToLower() == "asc" ? true : false;
            string[] _order = pagination.sidx.Split(',');
            //MethodCallExpression resultExp = null;
            var tempData = dbcontext.Set<TEntity>().AsQueryable().AsNoTracking();
            bool ThenAsc = false;
            var parameter = Expression.Parameter(typeof(TEntity), "t");
            foreach (string item in _order)
            {
                string _orderPart = item;
                _orderPart = Regex.Replace(_orderPart, @"\s+", " ");
                string[] _orderArry = _orderPart.Split(' ');
                string _orderField = _orderArry[0];
                bool sort = isAsc;
                if (_orderArry.Length == 2)
                {
                    isAsc = _orderArry[1].ToUpper() == "ASC" ? true : false;
                }
                var property = typeof(TEntity).GetProperty(_orderField);
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExp = Expression.Lambda(propertyAccess, parameter);
                string OrderName = "";
                if (!ThenAsc)
                {
                    OrderName = isAsc ? "OrderBy" : "OrderByDescending";

                }
                else
                {
                    OrderName = isAsc ? "ThenBy" : "ThenByDescending";
                }
                MethodCallExpression resultExp = Expression.Call(typeof(Queryable), OrderName, new Type[] { typeof(TEntity), property.PropertyType }, tempData.Expression, Expression.Quote(orderByExp));
                tempData = tempData.Provider.CreateQuery<TEntity>(resultExp);
                ThenAsc = true;
            }

            pagination.records = tempData.DeferredCount().FutureValue().Value;
            tempData = tempData.Skip<TEntity>(pagination.rows * (pagination.page - 1)).Take<TEntity>(pagination.rows).AsQueryable();
            return tempData.Future().ToList();
        }

        public List<TEntity> FindList<TEntity>(Expression<Func<TEntity, bool>> predicate, Pagination pagination) where TEntity : class, new()
        {
            bool isAsc = pagination.sord.ToLower() == "asc" ? true : false;
            string[] _order = pagination.sidx.Split(',');
            // MethodCallExpression resultExp = null;
            var tempData = dbcontext.Set<TEntity>().Where(predicate).AsNoTracking();
            bool ThenAsc = false;
            var parameter = Expression.Parameter(typeof(TEntity), "t");
            foreach (string item in _order)
            {
                string _orderPart = item;
                _orderPart = Regex.Replace(_orderPart, @"\s+", " ");
                string[] _orderArry = _orderPart.Split(' ');
                string _orderField = _orderArry[0];
                bool sort = isAsc;
                if (_orderArry.Length == 2)
                {
                    isAsc = _orderArry[1].ToUpper() == "ASC" ? true : false;
                }
                var property = typeof(TEntity).GetProperty(_orderField);
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExp = Expression.Lambda(propertyAccess, parameter);
                string OrderName = "";
                if (!ThenAsc)
                {
                    OrderName = isAsc ? "OrderBy" : "OrderByDescending";

                }
                else
                {
                    OrderName = isAsc ? "ThenBy" : "ThenByDescending";
                }
                MethodCallExpression resultExp = Expression.Call(typeof(Queryable), OrderName, new Type[] { typeof(TEntity), property.PropertyType }, tempData.Expression, Expression.Quote(orderByExp));
                tempData = tempData.Provider.CreateQuery<TEntity>(resultExp);
                ThenAsc = true;
            }

            pagination.records = tempData.DeferredCount().FutureValue().Value;
            tempData = tempData.Skip<TEntity>(pagination.rows * (pagination.page - 1)).Take<TEntity>(pagination.rows).AsQueryable();
            return tempData.Future().ToList();
        }
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public int DeleteList<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return dbcontext.Set<TEntity>().Where(predicate).Delete();
        }
        /// <summary>
        /// 批量修改
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="predicate1"></param>
        /// <returns></returns>
        public int UpdateList<TEntity>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> predicate1) where TEntity : class
        {
            return dbcontext.Set<TEntity>().Where(predicate).Update(predicate1);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public List<TEntity> FindListByPage<TEntity>(Expression<Func<TEntity, bool>> predicate, Pagination pagination) where TEntity : class, new()
        {
            bool isAsc = pagination.sord.ToLower() == "asc" ? true : false;
            string[] _order = pagination.sidx.Split(',');
            //MethodCallExpression resultExp = null;
            var tempData = dbcontext.Set<TEntity>().Where(predicate).AsNoTracking();
            bool ThenAsc = false;
            var parameter = Expression.Parameter(typeof(TEntity), "t");
            foreach (string item in _order)
            {
                string _orderPart = item;
                _orderPart = Regex.Replace(_orderPart, @"\s+", " ");
                string[] _orderArry = _orderPart.Split(' ');
                string _orderField = _orderArry[0];
                bool sort = isAsc;
                if (_orderArry.Length == 2)
                {
                    isAsc = _orderArry[1].ToUpper() == "ASC" ? true : false;
                }

                var property = typeof(TEntity).GetProperty(_orderField);
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExp = Expression.Lambda(propertyAccess, parameter);
                string OrderName = "";
                if (!ThenAsc)
                {
                    OrderName = isAsc ? "OrderBy" : "OrderByDescending";

                }
                else
                {
                    OrderName = isAsc ? "ThenBy" : "ThenByDescending";
                }
                MethodCallExpression resultExp = Expression.Call(typeof(Queryable), OrderName, new Type[] { typeof(TEntity), property.PropertyType },
                    tempData.Expression, Expression.Quote(orderByExp));

                tempData = tempData.Provider.CreateQuery<TEntity>(resultExp);
                ThenAsc = true;
            }

            pagination.records = tempData.DeferredCount().FutureValue().Value;
            tempData = tempData.Skip<TEntity>(pagination.rows * (pagination.page - 1)).Take<TEntity>(pagination.rows).AsQueryable();
            return tempData.Future().ToList();
        }
    }
}
