using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSOSPS.Cloud.Domain.Entity.SystemManage
{
    public  class UploadContentEntity
    {
        public string F_ID { get; set; }
        public string F_Filename { get; set; }
        public string F_Description { get; set; }
        public string F_Operator { get; set; }
        public Nullable<System.DateTime> F_DateTime { get; set; }
        public Nullable<int> F_TypeID { get; set; }
        public string F_Url { get; set; }

        public string F_UserName { get; set; }
    }
}
