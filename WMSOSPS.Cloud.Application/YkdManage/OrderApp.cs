using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMSOSPS.Cloud.Code;
using WMSOSPS.Cloud.Code.Enum;
using WMSOSPS.Cloud.Code.Operator;
using WMSOSPS.Cloud.Code.Web;
using WMSOSPS.Cloud.Data.CloudContext;
using WMSOSPS.Cloud.Domain.Entity.YdkManage;
using WMSOSPS.Cloud.Domain.IRepository.YdkManage;
using WMSOSPS.Cloud.Repository.YdkManage;

namespace WMSOSPS.Cloud.Application.YkdManage
{
    public class OrderApp : IDisposable
    {
        private readonly IYdkRepository _repository = new YdkRepository();
        public void Dispose()
        {
            if (_repository != null)
            {
                _repository.Dispose();
            }
        }
        public object GetGridJson(Pagination pagination, string location, string d1, string d2, string strNo)
        {
            var op = OperatorProvider.Provider.GetCurrent();


            int nOBillStatus = (int)BillStatus.已通过;
            int nOrderStatus = (int)OrderStatus.过毛重;
            var Query = _repository.IQueryableAsNoTracking<T_Order>(p => p.F_OrderStatus >= nOrderStatus && p.F_OBillStatus == nOBillStatus);
            Query = Query.Where(p => p.F_WMSCode == location);
            if (!string.IsNullOrEmpty(d1))
            {
                DateTime _d1 = ConvertVal.GetValDateTime(d1);
                Query = Query.Where(p => p.F_LogoutTime >= _d1);
            }
            if (!string.IsNullOrEmpty(d2))
            {
                DateTime _d2 = ConvertVal.GetValDateTime(d2);
                Query = Query.Where(p => p.F_LogoutTime <= _d2);
            }
            if (!string.IsNullOrEmpty(strNo))
            {
                Query = Query.Where(p => p.F_OrderNo.Contains(strNo));
            }
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

         
    }
}
