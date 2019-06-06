 

using System;
using System.Collections.Generic;
using System.Linq;
using WMSOSPS.Cloud.Code;
using WMSOSPS.Cloud.Code.Enum;
using WMSOSPS.Cloud.Code.Logger;
using WMSOSPS.Cloud.Code.Operator;
using WMSOSPS.Cloud.Data.CloudContext;
using WMSOSPS.Cloud.Domain.Entity.SystemManage;
using WMSOSPS.Cloud.Domain.IRepository.SystemManage;
using WMSOSPS.Cloud.Repository.SystemManage;

namespace WMSOSPS.Cloud.Application.SystemManage
{
    public class ItemsApp
    {
        private IItemsRepository service = new ItemsRepository();

        public List<Sys_Items> GetList()
        {
            return service.IQueryable<Sys_Items>().ToList();
        }
        public Sys_Items GetForm(string keyValue)
        {
            return service.FindEntity<Sys_Items>(keyValue);
        }
        public void DeleteForm(string keyValue)
        {
            OperatorModel op = OperatorProvider.Provider.GetCurrent();
            if (service.IQueryable<Sys_Items>().Count(t => t.F_ParentId.Equals(keyValue)) > 0)
            {

                throw new Exception("删除失败！操作的对象包含了下级数据。");
            }
            else
            {
                var F_FullName = service.FindEntity<Sys_Items>(m => m.F_Id == keyValue).F_FullName;
                service.Delete<Sys_Items>(t => t.F_Id == keyValue);
                LogHelper.Info("字典：【" + F_FullName + "】删除！操作人账号：" + op.UserCode + ",操作人名称:" + op.UserName + ",操作时间" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), OpType.System, null, "", "", ViewLevel.Admin);
            }
        }
        public void SubmitForm(Sys_Items itemsEntity, string keyValue)
        {
            OperatorModel op = OperatorProvider.Provider.GetCurrent();
            if (!string.IsNullOrEmpty(keyValue))
            {
                //itemsEntity.Modify(keyValue);
                itemsEntity.F_Id = keyValue;
                itemsEntity.F_LastModifyTime = DateTime.Now;
                itemsEntity.F_LastModifyUserId = OperatorProvider.Provider.GetCurrent().UserId;
                service.Update(itemsEntity);
                LogHelper.Info("字典：【" + itemsEntity.F_FullName + "】编辑！操作人账号：" + op.UserCode + ",操作人名称:" + op.UserName + ",操作时间" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), OpType.System, null, "", "", ViewLevel.Admin);
            }
            else
            {
                //itemsEntity.Create();
                itemsEntity.F_Id = Common.GuId();
                itemsEntity.F_CreatorUserId = OperatorProvider.Provider.GetCurrent().UserId;
                itemsEntity.F_CreatorTime = DateTime.Now;
                service.Insert(itemsEntity);
                LogHelper.Info("字典：【" + itemsEntity.F_FullName + "】新增！操作人账号：" + op.UserCode + ",操作人名称:" + op.UserName + ",操作时间" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), OpType.System, null, "", "", ViewLevel.Admin);
            }
        }
    }
}
