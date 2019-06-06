using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using WMSOSPS.Cloud.Code.Extend;
using WMSOSPS.Cloud.Code.Operator;
using WMSOSPS.Cloud.Code.Web;
using WMSOSPS.Cloud.Data.CloudContext;
using WMSOSPS.Cloud.Domain.Entity.YdkManage;
using WMSOSPS.Cloud.Domain.IRepository.YdkManage;
using WMSOSPS.Cloud.Repository.YdkManage;

namespace WMSOSPS.Cloud.Application.YkdManage
{
    public  class YdkApp : IDisposable
    {
        private readonly IYdkRepository _repository = new YdkRepository();

        public void Dispose()
        {
            if (_repository != null)
            {
                _repository.Dispose();
            }
        }
        #region 异地库管理
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="pagination"></param> 
        /// <returns></returns>
        public object GetGridJson(Pagination pagination, string keyword)
        {
            var op = OperatorProvider.Provider.GetCurrent(); 
            var Query = _repository.IQueryableAsNoTracking<T_WMS>();
            if (!op.IsSystem)
            {
                Query = Query.Where(p => p.F_BelongTo == op.F_BelongTo);
            }
            if (!string.IsNullOrEmpty(keyword))
            { 
                Query = Query.Where(p => p.F_Code == keyword);
            }
            

            var query = ExtLinq.QueryPaging(Query, pagination).ToList();
            var F_BelongTos = query.Select(p => p.F_BelongTo).Distinct().ToList();
            var F_BillMethods = query.Select(p => p.F_BillMethod).Distinct().ToList();
            var EnterS = _repository.IQueryableAsNoTracking<T_Enterprise>(p => F_BelongTos.Contains(p.F_Code)).ToList();
            var BillMethodS = _repository.IQueryableAsNoTracking<T_BillMethod>(p => F_BillMethods.Contains(p.F_ID)).ToList();
            var list = new List<YdkEntity>();
            foreach (var item in query)
            {
                var enterprise = EnterS.Where(p => p.F_Code == item.F_BelongTo).FirstOrDefault();
                var billMethod = BillMethodS.Where(p => p.F_ID == item.F_BillMethod).FirstOrDefault(); 
                list.Add(new YdkEntity()
                {

                    F_Code = item.F_Code,
                    F_Name = item.F_Name,
                    F_BelongTo = item.F_BelongTo,
                    F_BillMethod = item.F_BillMethod,
                    F_AllowError = item.F_AllowError,
                    F_IPAddress = item.F_IPAddress,
                    F_Position = item.F_Position,
                    F_EnterpriseName = enterprise?.F_Name,
                    F_BillName=billMethod?.F_Name
                });
            }
            return list;
        }
        public object Sumbit(T_WMS entity)
        {
            var op= OperatorProvider.Provider.GetCurrent(); 
            if (_repository.IQueryableAsNoTracking<T_WMS>(p => p.F_Code == entity.F_Code).Count() > 0)
            {
                return new { state = false, Message = "已存在当前编号异地库" };
            }
            _repository.Insert(entity);
            return new { state = true, Message = "操作成功" };
        }
        public object DeleteInfo(string keyValue)
        {
            _repository.Delete<T_WMS>(p => p.F_Code == keyValue);
            return new { state = true, Message = "操作成功" };
        }
        /// <summary>
        /// 获取所属企业
        /// </summary>
        /// <returns></returns>
        public object GetEnterprise()
        {
            var op = OperatorProvider.Provider.GetCurrent();

            var query= _repository.IQueryableAsNoTracking<T_Enterprise>();
            if (!op.IsSystem)
            {
                if (!string.IsNullOrEmpty(op.F_BelongTo))
                {
                    query = query.Where(p => p.F_Code == op.F_BelongTo);
                }
            } 
            
            return query.OrderBy(p => p.F_Code).ToList();
        }
        /// <summary>
        /// 获取选中当前人的设置异地库信息
        /// </summary>
        /// <returns></returns>
        public object GetYdkAll(string UserCode)
        { 
            
            var ydkList = _repository.IQueryableAsNoTracking<T_WMS>().ToList();
            var SetYdklst = _repository.IQueryableAsNoTracking<T_WMSManage>(p => p.F_UserID == UserCode).ToList();
            var setcodes = SetYdklst.Select(p => p.F_WMSCode).Distinct().ToList();//已设置异地库
            return new
            {
                queryYes = ydkList.Where(p => setcodes.Contains(p.F_Code)),
                queryNo = ydkList.Where(p => !setcodes.Contains(p.F_Code))
            };
        }
        /// <summary>
        /// 获得计量方式
        /// </summary>
        /// <returns></returns>
        public object GetBillMethod()
        {
            return _repository.IQueryableAsNoTracking<T_BillMethod>().OrderBy(p => p.F_ID).ToList();
        }
        #endregion

        #region 异地库配置
        //用户关联异地库

        public object GetYdkItem()
        {
            var op=OperatorProvider.Provider.GetCurrent();
            var query = _repository.IQueryableAsNoTracking<T_WMS>();
            //if (!string.IsNullOrEmpty(op.F_BelongTo))
            //{
            //    query = query.Where(p => p.F_BelongTo == op.F_BelongTo);
            //}
            if (op.RoleCode== "Rydk")
            {
                //异地库管理员，查T_WMSManage
                var F_Codes = _repository.IQueryableAsNoTracking<T_WMSManage>(p => p.F_UserID == op.UserCode).Select(p => p.F_WMSCode).Distinct().ToList();
                query = query.Where(p => F_Codes.Contains(p.F_Code));
            }
            else
            {
                //不是异地库则查当前所属企业全部异地库 
                query = query.Where(p => p.F_BelongTo == op.F_BelongTo);
            }

            
            return query.ToList();
        }
        /// <summary>
        /// 设置用户的异地库管理
        /// </summary>
        /// <param name="UserCode"></param>
        /// <param name="F_WMSCodes"></param>
        /// <returns></returns>
        public object SetYdk(string UserCode,string F_WMSCodes)
        {
            try
            {
                var op = OperatorProvider.Provider.GetCurrent();
                if (string.IsNullOrWhiteSpace(UserCode))
                {
                    return new { state = false, message = "参数不正确" };
                }
                TransactionOptions to = new TransactionOptions();
                to.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, to))
                {
                    //首先删除已设置异地库信息
                    _repository.DeleteList<T_WMSManage>(p => p.F_UserID == UserCode);
                    if (!string.IsNullOrWhiteSpace(F_WMSCodes))
                    {
                        List<T_WMSManage> list = new List<T_WMSManage>();
                        if (F_WMSCodes.Contains(","))
                        {
                            string[] strs = F_WMSCodes.Split(',');
                            foreach (var item in strs)
                            {
                                list.Add(new T_WMSManage()
                                { 
                                    F_UserID = UserCode,
                                    F_WMSCode = item 
                                });
                            }
                        }
                        else
                        {
                            list.Add(new T_WMSManage()
                            { 
                                F_UserID = UserCode,
                                F_WMSCode = F_WMSCodes
                            });
                        }
                        _repository.Insert<T_WMSManage>(list);
                    }
                    scope.Complete();
                }
                return new { state = true };
            }
            catch (Exception ex)
            { 
                return new { state = false, message = "操作失败！原因："+ex.ToString() };
            }
        }
        #endregion
    }
}
