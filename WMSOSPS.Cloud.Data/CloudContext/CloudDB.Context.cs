﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace WMSOSPS.Cloud.Data.CloudContext
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ExamCloudDBEntities : DbContext
    {
        public ExamCloudDBEntities()
            : base("name=ExamCloudDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Sys_Area> Sys_Area { get; set; }
        public virtual DbSet<Sys_DbBackup> Sys_DbBackup { get; set; }
        public virtual DbSet<Sys_FilterIP> Sys_FilterIP { get; set; }
        public virtual DbSet<Sys_Items> Sys_Items { get; set; }
        public virtual DbSet<Sys_ItemsDetail> Sys_ItemsDetail { get; set; }
        public virtual DbSet<Sys_Log> Sys_Log { get; set; }
        public virtual DbSet<Sys_Module> Sys_Module { get; set; }
        public virtual DbSet<Sys_ModuleButton> Sys_ModuleButton { get; set; }
        public virtual DbSet<Sys_ModuleForm> Sys_ModuleForm { get; set; }
        public virtual DbSet<Sys_ModuleFormInstance> Sys_ModuleFormInstance { get; set; }
        public virtual DbSet<Sys_Organize> Sys_Organize { get; set; }
        public virtual DbSet<Sys_Role> Sys_Role { get; set; }
        public virtual DbSet<Sys_RoleAuthorize> Sys_RoleAuthorize { get; set; }
        public virtual DbSet<Sys_UserLogOn> Sys_UserLogOn { get; set; }
        public virtual DbSet<T_Approve> T_Approve { get; set; }
        public virtual DbSet<T_BillMethod> T_BillMethod { get; set; }
        public virtual DbSet<T_Enterprise> T_Enterprise { get; set; }
        public virtual DbSet<T_Group> T_Group { get; set; }
        public virtual DbSet<T_OBillStatus> T_OBillStatus { get; set; }
        public virtual DbSet<T_Order> T_Order { get; set; }
        public virtual DbSet<T_OrderStatus> T_OrderStatus { get; set; }
        public virtual DbSet<T_User> T_User { get; set; }
        public virtual DbSet<T_WMS> T_WMS { get; set; }
        public virtual DbSet<T_WMSManage> T_WMSManage { get; set; }
        public virtual DbSet<T_OILType> T_OILType { get; set; }
        public virtual DbSet<T_SAPOIL> T_SAPOIL { get; set; }
        public virtual DbSet<T_WMSSrvUploadStatus> T_WMSSrvUploadStatus { get; set; }
        public virtual DbSet<Sys_User> Sys_User { get; set; }
    }
}
