using WMSOSPS.Cloud.Code;
using WMSOSPS.Cloud.Code.Extend;
using WMSOSPS.Cloud.Code.Operator;
using WMSOSPS.Cloud.Code.Web;
using WMSOSPS.Cloud.Data.CloudContext;
using WMSOSPS.Cloud.Domain.Entity.SystemManage;
using WMSOSPS.Cloud.Domain.IRepository.SystemManage;
using WMSOSPS.Cloud.Repository.SystemManage;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSOSPS.Cloud.Application.SystemManage
{
    public class UploadContentApp : IDisposable
    {
        private readonly IUploadContentRepository _repository = new UploadContentRepository();

        public void Dispose()
        {
            if (_repository != null)
            {
                _repository.Dispose();
            }
        }
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object GetGridJson(Pagination pagination, string d1, string d2, int type = 0)
        {
            var op = OperatorProvider.Provider.GetCurrent();
            var userList = _repository.IQueryableAsNoTracking<Sys_User>().ToList();
            var TypeList = _repository.IQueryableAsNoTracking<T_Type>().ToList();
            var Query = _repository.IQueryableAsNoTracking<T_UploadContent>(p=>p.F_TypeID==type);
            if (!string.IsNullOrEmpty(d1))
            {
                DateTime dt1 = DateTime.Parse(d1);
                Query = Query.Where(p => p.F_DateTime >= dt1);
            }
            if (!string.IsNullOrEmpty(d2))
            {
                DateTime dt2 = DateTime.Parse(d2);
                Query = Query.Where(p => p.F_DateTime < dt2);
            }

            var query = ExtLinq.QueryPaging(Query, pagination).ToList();
            var list = new List<UploadContentEntity>();
            foreach (var item in query)
            {
                var user = userList.Where(p => p.F_Account == item.F_Operator).FirstOrDefault(); 
                list.Add(new UploadContentEntity()
                {
                    F_ID = item.F_ID,
                    F_Filename = item.F_Filename,
                    F_Description = item.F_Description,
                    F_Operator = item.F_Operator,
                    F_DateTime = item.F_DateTime,
                    F_TypeID = item.F_TypeID,
                    F_Url = item.F_Url,
                    F_UserName = user != null ? user.F_RealName : "",
                });
            }
            return list;
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="type"></param>
        /// <param name="FileName"></param>
        /// <param name="FilePath"></param>
        /// <param name="F_Description"></param>
        /// <returns></returns>
        public object AddUploadContent(string type,string FileName,string FilePath,string F_Description,string f_date)
        {
            if (string.IsNullOrEmpty(type)|| string.IsNullOrEmpty(FileName) || string.IsNullOrEmpty(FilePath) || string.IsNullOrEmpty(f_date))
            {
                return new { state = false , msg ="缺少参数"};
            }
            try
            {
                var _date = DateTime.Parse(f_date);
                var op = OperatorProvider.Provider.GetCurrent();
                _repository.Insert(new T_UploadContent()
                {
                    F_ID = Guid.NewGuid().ToString("N"),
                    F_Filename = FileName,
                    F_Description = F_Description,
                    F_Operator = op.UserCode,
                    F_DateTime = _date,
                    F_TypeID = ConvertVal.GetValInt(type),
                    F_Url = FilePath,
                    F_CreateTime=DateTime.Now
                });
                return new { state = true };
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Code.Logger.LogHelper.Error(string.Format("Class: {0}, Property: {1}, Error: {2}", validationErrors.Entry.Entity.GetType().FullName,
                                            validationError.PropertyName,
                                            validationError.ErrorMessage), Code.Enum.OpType.Add);
                    }
                }
                return new { state = false, msg = "操作失败" };
            }
            catch (Exception ex)
            {
                return new { state = false ,msg= ex.ToString() };
            }
            
        }


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public object DeleteInfo(string keyValue)
        {
            try
            {
                if (_repository.Delete<T_UploadContent>(p => p.F_ID == keyValue) > 0)
                {
                    return new { state = true };
                }
                return new { state = false };
            }
            catch (Exception)
            {
                return new { state = false };
            }
          
        }
    }
}
