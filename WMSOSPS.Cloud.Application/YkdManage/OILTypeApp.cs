using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMSOSPS.Cloud.Code;
using WMSOSPS.Cloud.Code.Extend;
using WMSOSPS.Cloud.Code.Operator;
using WMSOSPS.Cloud.Code.Web;
using WMSOSPS.Cloud.Data.CloudContext;
using WMSOSPS.Cloud.Domain.Entity.YdkManage;
using WMSOSPS.Cloud.Domain.IRepository.YdkManage;
using WMSOSPS.Cloud.Repository.YdkManage;

namespace WMSOSPS.Cloud.Application.YkdManage
{
    public class OILTypeApp : IDisposable
    {
        private readonly IYdkRepository _repository = new YdkRepository();
        public void Dispose()
        {
            if (_repository != null)
            {
                _repository.Dispose();
            }
        }
        #region 油品配置
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="pagination"></param> 
        /// <returns></returns>
        public object GetGridJson(Pagination pagination, string F_EnterpriseCode)
        {
            var op = OperatorProvider.Provider.GetCurrent();
            var Query = _repository.IQueryableAsNoTracking<T_OILType>();
            var enterpriseQuery = _repository.IQueryableAsNoTracking<T_Enterprise>();
            if (!string.IsNullOrEmpty(F_EnterpriseCode))
            {
                enterpriseQuery = enterpriseQuery.Where(p => p.F_Code == F_EnterpriseCode);
            }
            if (!op.IsSystem)
            {
                enterpriseQuery = enterpriseQuery.Where(p => p.F_Code == op.F_BelongTo);
            }
            var EnterpriseData = enterpriseQuery.ToList();
            var enterpriseCodes = EnterpriseData.Select(p => p.F_Code).Distinct().ToList();
            Query = Query.Where(p => enterpriseCodes.Contains(p.F_EnterpriseCode));
            var query = ExtLinq.QueryPaging(Query, pagination).ToList();
           
            var list = new List<OILTypeEntity>();
            foreach (var item in query)
            {
                var enterprise = EnterpriseData.Where(p => p.F_Code == item.F_EnterpriseCode).FirstOrDefault(); 
                list.Add(new OILTypeEntity()
                {

                    F_ID = item.F_ID,
                    F_Name = item.F_Name,
                    F_EnterpriseCode = item.F_EnterpriseCode, 
                    F_EnterpriseName = enterprise?.F_Name, 
                });
            }
            return list;
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public object GetFormJsonOILType(string keyValue)
        {
            int fId = ConvertVal.GetValInt(keyValue);
            var m = _repository.IQueryableAsNoTracking<T_OILType>(p => p.F_ID == fId).FirstOrDefault();
            return m;
        }

        public object SumbitOILType(T_OILType entity,string keyValue)
        {
            var op = OperatorProvider.Provider.GetCurrent();
            try
            {
                if (string.IsNullOrEmpty(op.UserCode))
                {
                    return new { state = false, message = "当前登录人失效,请重新登录！" };
                }
                if (string.IsNullOrEmpty(keyValue))
                {
                    //添加
                    _repository.Insert(entity);
                }
                else
                {
                    //编辑
                    _repository.Update(entity);
                }
                return new { state = true };
            }
            catch (Exception ex)
            {
                return new { state = false, message = ex.ToString() };
            }
        }

        public object DeleteOILType(string keyValue)
        {
            try
            {
                int fId = ConvertVal.GetValInt(keyValue);
                if (_repository.Delete<T_OILType>(p => p.F_ID == fId) > 0)
                {
                    return new { state = true, message = "删除成功！" };
                }
                return new { state = false, message = "删除失败！" };
            }
            catch (Exception ex)
            {
                return new { state = false, message = "删除失败！" +ex.ToString()};
            }
          
        }
        #endregion

        #region SAP油品配置
        public object GetSAPGridJson(Pagination pagination, string location)
        {
            var op = OperatorProvider.Provider.GetCurrent();
            var Query = _repository.IQueryableAsNoTracking<T_SAPOIL>();
            Query = Query.Where(p => p.F_Location == location);
            return ExtLinq.QueryPaging(Query, pagination).ToList();
        }
        /// <summary>
        /// 获取油品名称
        /// </summary>
        /// <returns></returns>
        public object GetOILTypes()
        {
            var op = OperatorProvider.Provider.GetCurrent();
            var Query = _repository.IQueryableAsNoTracking<T_OILType>();
            if (!op.IsSystem)
            {
                if (!string.IsNullOrEmpty(op.F_BelongTo))
                {
                    Query = Query.Where(p => p.F_EnterpriseCode == op.F_BelongTo);
                }
            } 
            return Query.ToList();
        }
        /// <summary>
        /// 添加/编辑
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public object Sumbit(T_SAPOIL entity, string keyValue)
        {
            var op = OperatorProvider.Provider.GetCurrent();
            try
            {
                if (string.IsNullOrEmpty(op.UserCode))
                {
                    return new { state = false, message = "当前登录人失效,请重新登录！" };
                }
                if (string.IsNullOrEmpty(keyValue))
                {
                    //添加
                    _repository.Insert(entity);
                }
                else
                {
                    //编辑
                    _repository.Update(entity);
                }
                return new { state = true };
            }
            catch (Exception ex)
            {
                return new { state = false, message =ex.ToString() };
            }  
        }

        public object GetFormJson(string keyValue)
        {
            int fId = ConvertVal.GetValInt(keyValue);
            var m = _repository.IQueryableAsNoTracking<T_SAPOIL>(p => p.F_ID == fId).FirstOrDefault();
            return m;
        }
        #endregion
    }
}
