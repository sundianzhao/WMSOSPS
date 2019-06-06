using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSOSPS.Cloud.Domain.Entity.YdkManage
{
    public class YdkEntity
    {
        public string F_Code { get; set; }
        public string F_Name { get; set; }
        public string F_BelongTo { get; set; }
        public Nullable<int> F_BillMethod { get; set; }
        public Nullable<int> F_AllowError { get; set; }
        public string F_IPAddress { get; set; }
        public string F_Position { get; set; }

        public string F_EnterpriseName { get; set; }
        public string F_BillName { get; set; }
    }
}
