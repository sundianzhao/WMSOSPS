 

using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using WMSOSPS.Cloud.Data.CloudContext;
using WMSOSPS.Cloud.Data.PulbicRepository;
using WMSOSPS.Cloud.Domain.Entity.SystemManage;
using WMSOSPS.Cloud.Domain.IRepository.SystemManage;

namespace WMSOSPS.Cloud.Repository.SystemManage
{
    public class ItemsDetailRepository : RepositoryBase, IItemsDetailRepository
    {
        public List<Sys_ItemsDetail> GetItemList(string enCode)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT  d.*
                            FROM    Sys_ItemsDetail d
                                    INNER  JOIN Sys_Items i ON i.F_Id = d.F_ItemId
                            WHERE   1 = 1
                                    AND i.F_EnCode = @enCode
                                    AND d.F_EnabledMark = 1
                                    AND d.F_DeleteMark = 0
                            ORDER BY d.F_SortCode ASC");
            DbParameter[] parameter = 
            {
                 new SqlParameter("@enCode",enCode)
            }; 
            return this.FindList<Sys_ItemsDetail>(strSql.ToString(), parameter);
        }
    }
}
