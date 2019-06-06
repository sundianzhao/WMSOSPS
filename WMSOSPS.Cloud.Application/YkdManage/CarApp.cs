using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using WMSOSPS.Cloud.Code;
using WMSOSPS.Cloud.Code.Enum;
using WMSOSPS.Cloud.Code.Extend;
using WMSOSPS.Cloud.Code.Operator;
using WMSOSPS.Cloud.Code.Web;
using WMSOSPS.Cloud.Data.CloudContext;
using WMSOSPS.Cloud.Domain.Entity.YdkManage;
using WMSOSPS.Cloud.Domain.IRepository.YdkManage;
using WMSOSPS.Cloud.Repository.YdkManage;

namespace WMSOSPS.Cloud.Application.YkdManage
{
    public class CarApp : IDisposable
    {
        private readonly IYdkRepository _repository = new YdkRepository();
        public void Dispose()
        {
            if (_repository != null)
            {
                _repository.Dispose();
            }
        }
        #region 已进场车辆
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public object GetLoginGridJson(Pagination pagination, string location)
        {
            var op = OperatorProvider.Provider.GetCurrent();


            int nOBillStatus = (int)BillStatus.待审批;
            int nOrderStatus = (int)OrderStatus.已进场;
            var Query = _repository.IQueryableAsNoTracking<T_Order>(p => p.F_OrderStatus <= nOrderStatus && p.F_OBillStatus == nOBillStatus);
            Query = Query.Where(p => p.F_WMSCode == location);
            var q1 = _repository.IQueryableAsNoTracking<T_WMS>();
            var q2 = _repository.IQueryableAsNoTracking<T_Enterprise>();
           

            var query1 = from t1 in Query
                         join t2 in q1 on t1.F_WMSCode equals t2.F_Code
                         join t3 in q2 on t1.F_EnterpriseCode equals t3.F_Code
                         select new OrderEntity
                         {
                             F_OrderNo = t1.F_OrderNo,
                             F_WMSCode = t1.F_WMSCode,
                             F_EnterpriseCode = t1.F_EnterpriseCode,
                             F_OrderType = t1.F_OrderType,
                             F_ICCard = t1.F_ICCard,
                             F_Company = t1.F_Company,
                             F_TruckNo = t1.F_TruckNo,
                             F_DriverName = t1.F_DriverName,
                             F_DriverIDNo = t1.F_DriverIDNo,
                             F_OILDefine = t1.F_OILDefine,
                             F_Unit = t1.F_Unit,
                             F_OILName = t1.F_OILName,
                             F_MaterialID = t1.F_MaterialID,
                             F_ActualPrice = t1.F_ActualPrice,
                             F_OrderTime = t1.F_OrderTime,
                             F_OpOrder = t1.F_OpOrder,
                             F_CraneName = t1.F_CraneName,
                             F_StartTime = t1.F_StartTime,
                             F_EndTime = t1.F_EndTime,
                             F_OILValue = t1.F_OILValue,
                             F_TruckWeight = t1.F_TruckWeight,
                             F_LoginTime = t1.F_LoginTime,
                             F_OpIn = t1.F_OpIn,
                             F_GrossWeight = t1.F_GrossWeight,
                             F_NetWeight = t1.F_NetWeight,
                             F_BillWeight = t1.F_BillWeight,
                             F_LogoutTime = t1.F_LogoutTime,
                             F_OpOut = t1.F_OpOut,
                             F_OrderStatus = t1.F_OrderStatus,
                             F_TransCompany = t1.F_TransCompany,
                             F_SendCompany = t1.F_SendCompany,
                             F_OMeterID = t1.F_OMeterID,
                             F_Priority = t1.F_Priority,
                             F_SuperCargoIDNo = t1.F_SuperCargoIDNo,
                             F_Upload = t1.F_Upload,
                             F_Download = t1.F_Download,
                             F_OBillStatus = t1.F_OBillStatus,
                             F_OBillImage = t1.F_OBillImage,
                             F_OrderInfo = t1.F_OrderInfo,
                             F_WMSName=t2.F_Name,
                             F_EnterpriseName=t3.F_Name
                         };
            return query1.ToList();
        }
        /// <summary>
        /// 表单
        /// </summary>
        /// <param name="F_OrderNo"></param>
        /// <returns></returns>
        public object GetLoginFormJson(string F_OrderNo)
        {
            var entity = _repository.IQueryableAsNoTracking<T_Order>(p => p.F_OrderNo == F_OrderNo).FirstOrDefault();
            if (entity==null)
            {
                return new T_Order();
            }
            return new
            {
                F_OrderNo= entity.F_OrderNo,
                F_WMSCode=entity.F_WMSCode,
                F_Company=entity.F_Company,
                F_TruckNo=entity.F_TruckNo,
                F_GrossWeight=entity.F_GrossWeight.Value.ToString("F2"),
                F_TruckWeight=entity.F_TruckWeight.Value.ToString("F2"),
                F_NetWeight=entity.F_NetWeight.Value.ToString("F2")
            };
        }

        /// <summary>
        /// 审批
        /// </summary>
        /// <param name="F_OrderNo"></param>
        /// <param name="TruckWeight"></param>
        /// <param name="GrossWeight"></param>
        /// <param name="NetWeight"></param>
        /// <returns></returns>
        public object AuditForm(string F_OrderNo,string TruckWeight,string GrossWeight,string NetWeight)
        {
            try
            {
                if (string.IsNullOrEmpty(F_OrderNo) || string.IsNullOrEmpty(TruckWeight) || string.IsNullOrEmpty(GrossWeight) || string.IsNullOrEmpty(NetWeight))
                {
                    return new { state = false, message = "参数不正确" };
                }
                var op = OperatorProvider.Provider.GetCurrent();
                if (string.IsNullOrEmpty(op.UserCode))
                {
                    return new { state = false, message = "当前登录失效，请重新登录！" };
                }
                var result = ValidateData(TruckWeight, GrossWeight, NetWeight);
                if (!result.Item1)
                {
                    return new { state = false, message = result.Item2 };
                }
                var order = _repository.FindEntity<T_Order>(p => p.F_OrderNo == F_OrderNo);
                if (order == null)
                {
                    return new { state = false, message = "实体查询失败" };
                }
                TransactionOptions to = new TransactionOptions();
                to.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, to))
                {
                    order.F_OrderStatus = (int)OrderStatus.过毛重;
                    order.F_TruckWeight = ConvertVal.GetValFloat(TruckWeight);
                    order.F_GrossWeight = ConvertVal.GetValFloat(GrossWeight);
                    float? fNetWeight = order.F_GrossWeight - order.F_TruckWeight;
                    order.F_NetWeight = fNetWeight;
                    order.F_BillWeight = fNetWeight;
                    order.F_OBillStatus = (int)BillStatus.已通过;
                    order.F_LogoutTime = DateTime.Now;
                    order.F_OpOut = op.UserCode;
                    _repository.Update(order);

                    var approve = new T_Approve()
                    {
                        F_OrderNo = order.F_OrderNo,
                        F_WMSCode = order.F_WMSCode,
                        F_EnterpriseCode = order.F_EnterpriseCode,
                        F_UserID = op.UserCode,
                        F_TruckWeight = order.F_TruckWeight,
                        F_GrossWeight = order.F_GrossWeight,
                        F_NetWeight = order.F_NetWeight,
                        F_BillWeight = order.F_BillWeight,
                        F_DateTime = DateTime.Now
                    };
                    _repository.Insert(approve);
                }
              
                return new { state = true };
            }
            catch (Exception ex)
            {
                return new { state = false, message = "审批失败，原因：" + ex.ToString() };
            }
           

        }
        private Tuple<bool,string> ValidateData(string TruckWeight, string GrossWeight, string NetWeight)
        {
            float fTruckWeight = ConvertVal.GetValFloat(TruckWeight);
            if (fTruckWeight <= 0.1)
            { 
                return new Tuple<bool ,string>(false, "皮重输入有误，请重新输入!");
            }

            float fGrossWeight = ConvertVal.GetValFloat(GrossWeight);
            if (fGrossWeight <= 0.1)
            {
                return new Tuple<bool, string>(false, "毛重输入有误，请重新输入!"); 
            }
            if (string.IsNullOrEmpty(NetWeight.Trim()))
            {
                return new Tuple<bool, string>(false, "净重数据有误，请重新输入!"); 
            }
            return new Tuple<bool, string>(true,"");
        }
        #endregion

        #region 待审批车辆
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public object GetGrossGridJson(Pagination pagination, string location)
        {
            var op = OperatorProvider.Provider.GetCurrent();


            int nOBillStatus = (int)BillStatus.待审批;
            int nOrderStatus = (int)OrderStatus.过毛重;
            var Query = _repository.IQueryableAsNoTracking<T_Order>(p => p.F_OrderStatus >= nOrderStatus && p.F_OBillStatus == nOBillStatus);
            Query = Query.Where(p => p.F_WMSCode == location);
            var q1 = _repository.IQueryableAsNoTracking<T_WMS>();
            var q2 = _repository.IQueryableAsNoTracking<T_Enterprise>(); 
            var query1 = from t1 in Query
                         join t2 in q1 on t1.F_WMSCode equals t2.F_Code
                         join t3 in q2 on t1.F_EnterpriseCode equals t3.F_Code
                         select new OrderEntity
                         {
                             F_OrderNo = t1.F_OrderNo,
                             F_WMSCode = t1.F_WMSCode,
                             F_EnterpriseCode = t1.F_EnterpriseCode,
                             F_OrderType = t1.F_OrderType,
                             F_ICCard = t1.F_ICCard,
                             F_Company = t1.F_Company,
                             F_TruckNo = t1.F_TruckNo,
                             F_DriverName = t1.F_DriverName,
                             F_DriverIDNo = t1.F_DriverIDNo,
                             F_OILDefine = t1.F_OILDefine,
                             F_Unit = t1.F_Unit,
                             F_OILName = t1.F_OILName,
                             F_MaterialID = t1.F_MaterialID,
                             F_ActualPrice = t1.F_ActualPrice,
                             F_OrderTime = t1.F_OrderTime,
                             F_OpOrder = t1.F_OpOrder,
                             F_CraneName = t1.F_CraneName,
                             F_StartTime = t1.F_StartTime,
                             F_EndTime = t1.F_EndTime,
                             F_OILValue = t1.F_OILValue,
                             F_TruckWeight = t1.F_TruckWeight,
                             F_LoginTime = t1.F_LoginTime,
                             F_OpIn = t1.F_OpIn,
                             F_GrossWeight = t1.F_GrossWeight,
                             F_NetWeight = t1.F_NetWeight,
                             F_BillWeight = t1.F_BillWeight,
                             F_LogoutTime = t1.F_LogoutTime,
                             F_OpOut = t1.F_OpOut,
                             F_OrderStatus = t1.F_OrderStatus,
                             F_TransCompany = t1.F_TransCompany,
                             F_SendCompany = t1.F_SendCompany,
                             F_OMeterID = t1.F_OMeterID,
                             F_Priority = t1.F_Priority,
                             F_SuperCargoIDNo = t1.F_SuperCargoIDNo,
                             F_Upload = t1.F_Upload,
                             F_Download = t1.F_Download,
                             F_OBillStatus = t1.F_OBillStatus,
                             F_OBillImage = t1.F_OBillImage,
                             F_OrderInfo = t1.F_OrderInfo,
                             F_WMSName = t2.F_Name,
                             F_EnterpriseName = t3.F_Name
                         };
            return query1.ToList();
        }
        /// <summary>
        /// 表单
        /// </summary>
        /// <param name="F_OrderNo"></param>
        /// <returns></returns>
        public object GetGrossFormJson(string F_OrderNo)
        {
            var entity = _repository.IQueryableAsNoTracking<T_Order>(p => p.F_OrderNo == F_OrderNo).FirstOrDefault();
            if (entity == null)
            {
                return new T_Order();
            }
            return new
            {
                F_OrderNo = entity.F_OrderNo,
                F_WMSCode = entity.F_WMSCode,
                F_Company = entity.F_Company,
                F_TruckNo = entity.F_TruckNo,
                F_GrossWeight = entity.F_GrossWeight.Value.ToString("F2"),
                F_TruckWeight = entity.F_TruckWeight.Value.ToString("F2"),
                F_NetWeight = entity.F_NetWeight.Value.ToString("F2"),
                F_OBillImage=entity.F_OBillImage
            };
        }
        #endregion

        #region 待结算提货单
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public object GetUnFinishedGridJson(Pagination pagination, string location)
        {
            var op = OperatorProvider.Provider.GetCurrent();


            int nOBillStatus = (int)BillStatus.已通过;
            int nOrderStatus = (int)OrderStatus.过毛重;
            var Query = _repository.IQueryableAsNoTracking<T_Order>(p => p.F_OrderStatus <= nOrderStatus && p.F_OBillStatus == nOBillStatus);
            Query = Query.Where(p => p.F_WMSCode == location);
            var q1 = _repository.IQueryableAsNoTracking<T_WMS>();
            var q2 = _repository.IQueryableAsNoTracking<T_Enterprise>();
            var query1 = from t1 in Query
                         join t2 in q1 on t1.F_WMSCode equals t2.F_Code
                         join t3 in q2 on t1.F_EnterpriseCode equals t3.F_Code
                         select new OrderEntity
                         {
                             F_OrderNo = t1.F_OrderNo,
                             F_WMSCode = t1.F_WMSCode,
                             F_EnterpriseCode = t1.F_EnterpriseCode,
                             F_OrderType = t1.F_OrderType,
                             F_ICCard = t1.F_ICCard,
                             F_Company = t1.F_Company,
                             F_TruckNo = t1.F_TruckNo,
                             F_DriverName = t1.F_DriverName,
                             F_DriverIDNo = t1.F_DriverIDNo,
                             F_OILDefine = t1.F_OILDefine,
                             F_Unit = t1.F_Unit,
                             F_OILName = t1.F_OILName,
                             F_MaterialID = t1.F_MaterialID,
                             F_ActualPrice = t1.F_ActualPrice,
                             F_OrderTime = t1.F_OrderTime,
                             F_OpOrder = t1.F_OpOrder,
                             F_CraneName = t1.F_CraneName,
                             F_StartTime = t1.F_StartTime,
                             F_EndTime = t1.F_EndTime,
                             F_OILValue = t1.F_OILValue,
                             F_TruckWeight = t1.F_TruckWeight,
                             F_LoginTime = t1.F_LoginTime,
                             F_OpIn = t1.F_OpIn,
                             F_GrossWeight = t1.F_GrossWeight,
                             F_NetWeight = t1.F_NetWeight,
                             F_BillWeight = t1.F_BillWeight,
                             F_LogoutTime = t1.F_LogoutTime,
                             F_OpOut = t1.F_OpOut,
                             F_OrderStatus = t1.F_OrderStatus,
                             F_TransCompany = t1.F_TransCompany,
                             F_SendCompany = t1.F_SendCompany,
                             F_OMeterID = t1.F_OMeterID,
                             F_Priority = t1.F_Priority,
                             F_SuperCargoIDNo = t1.F_SuperCargoIDNo,
                             F_Upload = t1.F_Upload,
                             F_Download = t1.F_Download,
                             F_OBillStatus = t1.F_OBillStatus,
                             F_OBillImage = t1.F_OBillImage,
                             F_OrderInfo = t1.F_OrderInfo,
                             F_WMSName = t2.F_Name,
                             F_EnterpriseName = t3.F_Name
                         };
            return query1.ToList();
        }
        /// <summary>
        /// 表单
        /// </summary>
        /// <param name="F_OrderNo"></param>
        /// <returns></returns>
        public object GetUnFinishedFormJson(string F_OrderNo)
        {
            var entity = _repository.IQueryableAsNoTracking<T_Order>(p => p.F_OrderNo == F_OrderNo).FirstOrDefault();
            if (entity == null)
            {
                return new T_Order();
            }
            return new
            {
                F_OrderNo = entity.F_OrderNo,
                F_WMSCode = entity.F_WMSCode,
                F_Company = entity.F_Company,
                F_TruckNo = entity.F_TruckNo,
                F_GrossWeight = entity.F_GrossWeight.Value.ToString("F2"),
                F_TruckWeight = entity.F_TruckWeight.Value.ToString("F2"),
                F_NetWeight = entity.F_NetWeight.Value.ToString("F2"),
                F_OBillImage = entity.F_OBillImage
            };
        }
        
        /// <summary>
        /// 结算
        /// </summary>
        /// <param name="F_OrderNo"></param>
        /// <returns></returns>
        public object Accounts(string F_OrderNo)
        {
            var op = OperatorProvider.Provider.GetCurrent();
            if (string.IsNullOrEmpty(op.UserCode))
            {
                return new { state = false, message = "当前登录失效，请重新登录！" };
            }
            var order = _repository.FindEntity<T_Order>(p => p.F_OrderNo == F_OrderNo);
            if (order==null)
            {
                return new { state = false, message = "查询提单数据失败！" };
            }
            try
            {
                TransactionOptions to = new TransactionOptions();
                to.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, to))
                {
                    order.F_OpOut = op.UserCode;
                    order.F_OrderStatus = (int)OrderStatus.已结算;
                    _repository.Update(order);
                    var approve = new T_Approve()
                    {
                        F_OrderNo = order.F_OrderNo,
                        F_WMSCode = order.F_WMSCode,
                        F_EnterpriseCode = order.F_EnterpriseCode,
                        F_UserID = op.UserCode,
                        F_TruckWeight = order.F_TruckWeight,
                        F_GrossWeight = order.F_GrossWeight,
                        F_NetWeight = order.F_NetWeight,
                        F_BillWeight = order.F_BillWeight,
                        F_DateTime = DateTime.Now
                    };
                    _repository.Insert(approve);
                    scope.Complete();
                }
                return new { state = true };
            }
            catch (Exception ex)
            {
                return new { state = false, message = "结算失败！，原因："+ex.ToString() };
            }
        }
        #endregion
    }
}
