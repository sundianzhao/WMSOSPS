﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSOSPS.Cloud.Code.Enum
{
    public class Organize
    {
        public string F_Id { get; set; }
        public string F_ParentId { get; set; }
        public Nullable<int> F_Layers { get; set; }
        public string F_EnCode { get; set; }
        public string F_FullName { get; set; }
        public string F_ShortName { get; set; }
        public string F_CategoryId { get; set; }
        public string F_ManagerId { get; set; }
        public string F_TelePhone { get; set; }
        public string F_MobilePhone { get; set; }
        public string F_WeChat { get; set; }
        public string F_Fax { get; set; }
        public string F_Email { get; set; }
        public string F_AreaId { get; set; }
        public string F_Address { get; set; }
        public Nullable<bool> F_AllowEdit { get; set; }
        public Nullable<bool> F_AllowDelete { get; set; }
        public Nullable<int> F_SortCode { get; set; }
        public Nullable<bool> F_DeleteMark { get; set; }
        public Nullable<bool> F_EnabledMark { get; set; }
        public string F_Description { get; set; }
        public Nullable<System.DateTime> F_CreatorTime { get; set; }
        public string F_CreatorUserId { get; set; }
        public Nullable<System.DateTime> F_LastModifyTime { get; set; }
        public string F_LastModifyUserId { get; set; }
        public Nullable<System.DateTime> F_DeleteTime { get; set; }
        public string F_DeleteUserId { get; set; }
    }
}
