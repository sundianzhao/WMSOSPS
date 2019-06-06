using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSOSPS.Cloud.Domain.Entity.YdkManage
{
    public class OrderEntity
    {
        public string F_OrderNo { get; set; }
        public string F_WMSCode { get; set; }
        public string F_EnterpriseCode { get; set; }
        public string F_OrderType { get; set; }
        public string F_ICCard { get; set; }
        public string F_Company { get; set; }
        public string F_TruckNo { get; set; }
        public string F_DriverName { get; set; }
        public string F_DriverIDNo { get; set; }
        public Nullable<float> F_OILDefine { get; set; }
        public string F_Unit { get; set; }
        public string F_OILName { get; set; }
        public string F_MaterialID { get; set; }
        public Nullable<float> F_ActualPrice { get; set; }
        public Nullable<System.DateTime> F_OrderTime { get; set; }
        public string F_OpOrder { get; set; }
        public string F_CraneName { get; set; }
        public Nullable<System.DateTime> F_StartTime { get; set; }
        public Nullable<System.DateTime> F_EndTime { get; set; }
        public Nullable<float> F_OILValue { get; set; }
        public Nullable<float> F_TruckWeight { get; set; }
        public Nullable<System.DateTime> F_LoginTime { get; set; }
        public string F_OpIn { get; set; }
        public Nullable<float> F_GrossWeight { get; set; }
        public Nullable<float> F_NetWeight { get; set; }
        public Nullable<float> F_BillWeight { get; set; }
        public Nullable<System.DateTime> F_LogoutTime { get; set; }
        public string F_OpOut { get; set; }
        public Nullable<int> F_OrderStatus { get; set; }
        public string F_TransCompany { get; set; }
        public string F_SendCompany { get; set; }
        public string F_OMeterID { get; set; }
        public Nullable<int> F_Priority { get; set; }
        public string F_SuperCargoIDNo { get; set; }
        public Nullable<int> F_Upload { get; set; }
        public Nullable<int> F_Download { get; set; }
        public Nullable<int> F_OBillStatus { get; set; }
        public string F_OBillImage { get; set; }
        public string F_OrderInfo { get; set; }

        public string F_WMSName { get; set; }
        public string F_EnterpriseName { get; set; }
    }
}
