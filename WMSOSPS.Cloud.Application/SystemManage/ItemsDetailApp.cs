 

using System;
using System.Collections.Generic;
using System.Linq;
using WMSOSPS.Cloud.Code;
using WMSOSPS.Cloud.Code.Enum;
using WMSOSPS.Cloud.Code.Extend;
using WMSOSPS.Cloud.Code.Logger;
using WMSOSPS.Cloud.Code.Operator;
using WMSOSPS.Cloud.Data.CloudContext;
using WMSOSPS.Cloud.Domain.Entity.SystemManage;
using WMSOSPS.Cloud.Domain.IRepository.SystemManage;
using WMSOSPS.Cloud.Repository.SystemManage;

namespace WMSOSPS.Cloud.Application.SystemManage
{
    public class ItemsDetailApp:IDisposable 
    {
        private IItemsDetailRepository service = new ItemsDetailRepository();

        public void Dispose()
        {
            service?.Dispose();

        }

        public List<Sys_ItemsDetail> GetList(string itemId = "", string keyword = "")
        {
            var expression = ExtLinq.True<Sys_ItemsDetail>();
            if (!string.IsNullOrEmpty(itemId))
            {
                expression = expression.And(t => t.F_ItemId == itemId);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                expression = expression.And(t => t.F_ItemName.Contains(keyword));
                expression = expression.Or(t => t.F_ItemCode.Contains(keyword));
            }
            return service.IQueryable(expression).OrderBy(t => t.F_SortCode).ToList();
        }
        public List<Sys_ItemsDetail> GetItemList(string enCode)
        {
            return service.GetItemList(enCode);
        }
        public Sys_ItemsDetail GetForm(string keyValue)
        {
            return service.FindEntity<Sys_ItemsDetail>(keyValue);
        }
        public void DeleteForm(string keyValue)
        {

            OperatorModel op = OperatorProvider.Provider.GetCurrent();
            var F_ItemName = service.FindEntity<Sys_ItemsDetail>(m => m.F_Id == keyValue).F_ItemName;
            service.Delete<Sys_ItemsDetail>(t => t.F_Id == keyValue);
            LogHelper.Info("字典：【" + F_ItemName + "】删除！操作人账号：" + op.UserCode + ",操作人名称:" + op.UserName + ",操作时间" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),  OpType.System, null, "", "", ViewLevel.Admin);
          
        }
        public void SubmitForm(Sys_ItemsDetail itemsDetailEntity, string keyValue)
        {
            OperatorModel op = OperatorProvider.Provider.GetCurrent();
            if (!string.IsNullOrEmpty(keyValue))
            {
                //itemsDetailEntity.Modify(keyValue);
                itemsDetailEntity.F_Id = keyValue;
                itemsDetailEntity.F_LastModifyTime = DateTime.Now;
                itemsDetailEntity.F_LastModifyUserId = OperatorProvider.Provider.GetCurrent().UserId;
                itemsDetailEntity.F_DeleteMark = false;
                service.Update(itemsDetailEntity);
                LogHelper.Info("字典：【" + itemsDetailEntity.F_ItemName + "】编辑！操作人账号：" + op.UserCode + ",操作人名称:" + op.UserName + ",操作时间" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), OpType.System, null, "", "", ViewLevel.Admin);
            }
            else
            {
                //itemsDetailEntity.Create();
                itemsDetailEntity.F_Id = Common.GuId();
                itemsDetailEntity.F_CreatorUserId = OperatorProvider.Provider.GetCurrent().UserId;
                itemsDetailEntity.F_CreatorTime = DateTime.Now;
                itemsDetailEntity.F_DeleteMark = false;
                service.Insert(itemsDetailEntity);
                LogHelper.Info("字典：【" + itemsDetailEntity.F_ItemName + "】新增！操作人账号：" + op.UserCode + ",操作人名称:" + op.UserName + ",操作时间" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), OpType.System, null, "", "", ViewLevel.Admin);
            }
        } 

       
    }
}
