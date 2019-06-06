using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSOSPS.Cloud.Code.Enum
{
    public enum OrderStatus
    {
        创建业务 = 0,
        取消订单,
        已制卡,
        已叫号,
        已进场,
        叫号超时,
        过皮重,
        正在装车,
        装车结束,
        过毛重,
        已结算,
        已出场,
        已作废,
        含水计算,           //正在计算含水
        请求制卡,           //请求制卡
        制卡失败            //制卡失败
    }

    public enum WMSOrderStatus
    {
        已进场 = OrderStatus.已进场,
        过毛重 = OrderStatus.过毛重,
        已结算 = OrderStatus.已结算
    }

    public enum UploadStatus
    {
        等待上传 = 0,
        上传成功
    }

    public enum BillStatus
    {
        待审批 = 0,
        已通过
    }

}
