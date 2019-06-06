using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSOSPS.Cloud.Domain.Entity.SystemManage
{
    public  class NoticeEntity
    {
        public string Content { get; set; }
        public string CreateWorkerId { get; set; }
        public DateTime? ExpireTime { get; set; }
        public string InceptRole { get; set; }
        public string InceptWorkerId { get; set; }
        public byte? IsNew { get; set; }
        public byte? IsRed { get; set; }
        public byte? IsTop { get; set; }
        public byte? IsUse { get; set; }
        public DateTime? JoinTime { get; set; }
        public int nId { get; set; }
        public string RoleCode { get; set; }
        public short? ServerId { get; set; }
        public string Title { get; set; }
        public string WorkerId { get; set; }
        public string Type { get; set; }
    }
}
