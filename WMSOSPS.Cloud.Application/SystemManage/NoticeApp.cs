using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Exam.Cloud.Code.Enum;
using Exam.Cloud.Code.Extend;
using Exam.Cloud.Code.Operator;
using Exam.Cloud.Code.Web;
using Exam.Cloud.Data.CloudContext;
using Exam.Cloud.Domain.Entity.SystemManage;
using Exam.Cloud.Domain.IRepository.SystemManage;
using Exam.Cloud.Repository.SystemManage;

namespace Exam.Cloud.Application.SystemManage
{
    public  class NoticeApp : IDisposable
    {
        private readonly INoticeRepository _repository = new NoticeRepository();
        public void Dispose()
        {
            _repository?.Dispose();
        }

        public object GetList(Pagination pagination,string title,string d1,string d2)
        {
            var imember = OperatorProvider.Provider.GetCurrent();
           
            var exp = _repository.IQueryableAsNoTracking<Sys_Notice>();
            if (imember.LoginType == LoginTypeEnum.AgentMember)
            {
                exp = exp.Where(p => p.AuditStatus == 1);
            }
            if (!string.IsNullOrEmpty(title))
            {
                exp = exp.Where(p => p.Title.Contains(title));
            }
            if (!string.IsNullOrEmpty(d1))
            {
                DateTime _d1 = DateTime.Parse(d1);
                exp = exp.Where(p => p.JoinTime >= _d1);
            }
            if (!string.IsNullOrEmpty(d2))
            {
                DateTime _d2 = DateTime.Parse(d2);
                exp = exp.Where(p => p.JoinTime <= _d2);
            }
            return ExtLinq.QueryPaging(exp, pagination).ToList();
        }
        /// <summary>
        /// 添加公告
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool SubmitNoticeForm(NoticeEntity entity)
        {
            OperatorModel imember = OperatorProvider.Provider.GetCurrent();
            if (imember.LoginType == LoginTypeEnum.ManageMember)
            {
                Sys_Notice sysNotice = new Sys_Notice
                {
                    Title = HttpUtility.UrlDecode(entity.Title),
                    Content = HttpUtility.UrlDecode(entity.Content),
                    IsTop = entity.IsTop==1?true:false,
                    AuditStatus = 0,
                    IsUse = entity.IsUse==1?true:false,
                    CreateWorkerID = imember.UserCode,
                    JoinTime = DateTime.Now
                };
                int count = 0;
                if (entity.Type == "add")
                {
                    count = _repository.Insert(sysNotice);
                }
                else if (entity.Type == "edit")
                {
                    sysNotice.nID = entity.nId;
                    count = _repository.Update(sysNotice);
                }

                if (count > 0)
                    return true;
                return false;
            }
            else
            {
                return false;
            }
        }

        public object GetNoticeFormJson(string keyValue)
        {
            int nId = Convert.ToInt32(keyValue);
            var data = _repository.IQueryableAsNoTracking<Sys_Notice>().FirstOrDefault(a => a.nID == nId);
            return data;
        }

        /// <summary>
        /// 审核 
        /// </summary>
        /// <returns></returns>
        public object AuditSuccess(string keyValue,byte state,string failmsg)
        {
            var op  = OperatorProvider.Provider.GetCurrent();
            int nId = Convert.ToInt32(keyValue);
            var entity = _repository.FindEntity<Sys_Notice>(p => p.nID == nId);
            if (entity==null)
            {
                return new { state = false };
            }
            entity.AuditStatus = state;
            entity.Aditor = op.UserCode;
            if (state==2)
            {
                entity.FailMsg = failmsg;
            }
            if(_repository.Update(entity)>0)
            {
                return new { state = true };
            }
            return new { state = false };
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public object DeleteNotice(string keyValue)
        {
            int nId = Convert.ToInt32(keyValue);
            if (_repository.Delete<Sys_Notice >(p=>p.nID==nId)>0)
            {
                return new { state = true };
            }
            return new { state = false };
        }

        public object GetPlatformNoticeDefault()
        {
            var data = _repository.IQueryableAsNoTracking<Sys_Notice>(p => p.AuditStatus == 1 && p.IsUse).OrderByDescending(p => p.IsTop).ThenByDescending(p => p.JoinTime).ToList();
            return data;
        }

        /// <summary>
        /// 超管之外的人查询公告
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="title"></param>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public object GetListBySearch(Pagination pagination, string title, string d1, string d2)
        {
            var exp = _repository.IQueryableAsNoTracking<Sys_Notice>(p=>p.IsUse&&p.AuditStatus==1);
            if (!string.IsNullOrEmpty(title))
            {
                exp = exp.Where(p => p.Title.Contains(title));
            }
            if (!string.IsNullOrEmpty(d1))
            {
                DateTime _d1 = DateTime.Parse(d1);
                exp = exp.Where(p => p.JoinTime >= _d1);
            }
            if (!string.IsNullOrEmpty(d2))
            {
                DateTime _d2 = DateTime.Parse(d2);
                exp = exp.Where(p => p.JoinTime <= _d2);
            }
            return ExtLinq.QueryPaging(exp, pagination);
        }

        #region 首页统计数据查询
        /// <summary>
        /// 获取超管数据
        /// </summary>
        /// <returns></returns>
        public object GetList1()
        {
            var list = new List<PointModal>();
            var query = _repository.IQueryableAsNoTracking<LeagueInfo>();
            int yxkh = query.Where(p => p.Type == "意向客户").Count();
            int Acount= query.Where(p => p.Type == "A").Count();
            int Bcount = query.Where(p => p.Type == "B").Count();
            int Ccount = query.Where(p => p.Type == "C").Count();
            int Dcount = query.Where(p => p.Type == "D").Count();
            int Lcount = query.Where(p => p.Type == "签约客户").Count();
            list.Add(new PointModal { Count = yxkh.ToString(), Name = "意向客户" });
            list.Add(new PointModal { Count = Acount.ToString(), Name = "A" });
            list.Add(new PointModal { Count = Bcount.ToString(), Name = "B" });
            list.Add(new PointModal { Count = Ccount.ToString(), Name = "C" });
            list.Add(new PointModal { Count = Dcount.ToString(), Name = "D" });
            list.Add(new PointModal { Count = Lcount.ToString(), Name = "签约客户" });

            //今日电话量
            var querygj = _repository.IQueryableAsNoTracking<LeagueInfo_gjInfo>();
            DateTime d1 = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));
            DateTime d2 = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 23:59:59"));

            DateTime _d1 = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")); 
            var q = querygj.Where(p => p.CreateTime >= d1 && p.CreateTime <= d2).ToList();
            int jrDxCount = q.Where(p=>p.t_Type== "打电话"|| p.t_Type == "微信沟通").Count();
            int jrDxCountY = q.Where(p => p.IsValid == 1).Count();
            int jrDxCountN = q.Where(p => p.IsValid == 0).Count();

            var q1 = querygj.Where(p => p.CreateTime >= _d1 && p.CreateTime <= d2).ToList(); 
            int byDxCount = q1.Where(p => p.t_Type == "打电话" || p.t_Type == "微信沟通").Count();
            int byDxCountY = q1.Where(p => p.IsValid == 1).Count();
            int byDxCountN = q1.Where(p => p.IsValid == 0).Count();
            list.Add(new PointModal { Count = jrDxCount.ToString(), Name = "今日电话数量" });
            list.Add(new PointModal { Count = jrDxCountY.ToString(), Name = "今日电话有效" });
            list.Add(new PointModal { Count = jrDxCountN.ToString(), Name = "今日电话无效" });
            list.Add(new PointModal { Count = byDxCount.ToString(), Name = "本月电话数量" });
            list.Add(new PointModal { Count = byDxCountY.ToString(), Name = "本月电话数量有效" });
            list.Add(new PointModal { Count = byDxCountN.ToString(), Name = "本月电话数量无效" });

            int jmDxCount = q1.Where(p => p.t_Type == "邀约见面沟通").Count();
            list.Add(new PointModal { Count = jmDxCount.ToString(), Name = "本月邀约量" });
            return list;
        }
        public class PointModal
        {
            public string Count { get; set; }
            public string Name { get; set; }
        }
        /// <summary>
        /// 获取学生数据
        /// </summary>
        /// <returns></returns>
        public object GetList2()
        {
            var op = OperatorProvider.Provider.GetCurrent();
            var list = new List<PointModal>();
            var query = _repository.IQueryableAsNoTracking<Student_Info>(p => p.Type == "入园宝宝");
            var queryL = _repository.IQueryableAsNoTracking<LeagueInfo>(p => p.Type == "签约客户");
            if (op.ServiceCode== "Service")
            {
                queryL = queryL.Where(p => p.ServiceUser == op.UserCode);
                var lstAgentIds = _repository.IQueryableAsNoTracking<LeagueInfo>(p => p.ServiceUser == op.UserCode).Select(p => p.LoginCode).ToList();
                query = query.Where(p => lstAgentIds.Contains(p.AgentWorkID));
            }
            list.Add(new PointModal { Count = query.Count().ToString(), Name = "学生数量" });
            list.Add(new PointModal { Count = queryL.Count().ToString(), Name = "加盟商数量" });

            return list;
        }
        #endregion
    }
}
