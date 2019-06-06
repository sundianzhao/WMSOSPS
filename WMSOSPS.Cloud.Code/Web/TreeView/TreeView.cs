 

using System.Collections.Generic;
using System.Text;

namespace WMSOSPS.Cloud.Code.Web.TreeView
{
    public static class TreeView
    {
        public static string TreeViewJson(this List<TreeViewModel> data, string parentId = "0")
        {
            StringBuilder strJson = new StringBuilder();
            List<TreeViewModel> item = data.FindAll(t => t.parentId == parentId);
            strJson.Append("[");
            if (item.Count > 0)
            {
                foreach (TreeViewModel entity in item)
                {
                    strJson.Append("{");
                    strJson.Append("\"id\":\"" + entity.id + "\",");
                    strJson.Append("\"nodeType\":\"" + entity.nodeType + "\",");
                    strJson.Append("\"text\":\"" + entity.text + "\",");
                    if (!string.IsNullOrEmpty(entity.ExtrasInfo))
                        strJson.Append("\"ExtrasInfo\":\"" + entity.ExtrasInfo.Replace("&nbsp;", "") + "\",");
                    if (!string.IsNullOrEmpty(entity.iconCls))
                        strJson.Append("\"iconCls\":\"" + entity.iconCls.Replace("&nbsp;", "") + "\",");
                    if (!string.IsNullOrEmpty(entity.EnterpriseID))
                        strJson.Append("\"EnterpriseID\":\"" + entity.EnterpriseID.Replace("&nbsp;", "") + "\",");
                    if (!string.IsNullOrEmpty(entity.CallInType))
                        strJson.Append("\"CallInType\":\"" + entity.CallInType.Replace("&nbsp;", "") + "\",");
                    if (!string.IsNullOrEmpty(entity.WorkCallFile))
                        strJson.Append("\"WorkCallFile\":\"" + entity.WorkCallFile.Replace("&nbsp;", "") + "\",");
                    if (!string.IsNullOrEmpty(entity.strIsWorker))
                        strJson.Append("\"strIsWorker\":\"" + entity.strIsWorker.Replace("&nbsp;", "") + "\",");
                    if (!string.IsNullOrEmpty(entity.ISAdd))
                        strJson.Append("\"ISAdd\":\"" + entity.ISAdd.Replace("&nbsp;", "") + "\",");
                    if (!string.IsNullOrEmpty(entity.CallID))
                        strJson.Append("\"CallID\":\"" + entity.CallID.Replace("&nbsp;", "") + "\",");
                    if (!string.IsNullOrEmpty(entity.ACDType))
                        strJson.Append("\"ACDType\":\"" + entity.ACDType.Replace("&nbsp;", "") + "\",");
                    if (!string.IsNullOrEmpty(entity.WorkerID))
                        strJson.Append("\"WorkerID\":\"" + entity.WorkerID.Replace("&nbsp;", "") + "\",");
                    if (!string.IsNullOrEmpty(entity.State))
                        strJson.Append("\"State\":\"" + entity.State.Replace("&nbsp;", "") + "\",");
                    if (!string.IsNullOrEmpty(entity.Pid))
                        strJson.Append("\"Pid\":\"" + entity.Pid.Replace("&nbsp;", "") + "\",");
                    if (!string.IsNullOrEmpty(entity.IsWorker))
                        strJson.Append("\"IsWorker\":\"" + entity.IsWorker.Replace("&nbsp;", "") + "\",");
                    if (!string.IsNullOrEmpty(entity.IVRType))
                        strJson.Append("\"IVRType\":\"" + entity.IVRType.Replace("&nbsp;", "") + "\",");
                    strJson.Append("\"value\":\"" + entity.value + "\",");
                    if (entity.title != null && !string.IsNullOrEmpty(entity.title.Replace("&nbsp;", "")))
                    {
                        strJson.Append("\"title\":\"" + entity.title.Replace("&nbsp;", "") + "\",");
                    }
                    if (entity.img != null && !string.IsNullOrEmpty(entity.img.Replace("&nbsp;", "")))
                    {
                        strJson.Append("\"img\":\"" + entity.img.Replace("&nbsp;", "") + "\",");
                    }
                    if (entity.checkstate != null)
                    {
                        strJson.Append("\"checkstate\":" + entity.checkstate + ",");
                    }
                    if (entity.parentId != null)
                    {
                        strJson.Append("\"parentnodes\":\"" + entity.parentId + "\",");
                    }
                    strJson.Append("\"showcheck\":" + entity.showcheck.ToString().ToLower() + ",");
                    strJson.Append("\"isexpand\":" + entity.isexpand.ToString().ToLower() + ",");
                    if (entity.complete == true)
                    {
                        strJson.Append("\"complete\":" + entity.complete.ToString().ToLower() + ",");
                    }
                    strJson.Append("\"hasChildren\":" + entity.hasChildren.ToString().ToLower() + ",");
                    strJson.Append("\"ChildNodes\":" + TreeViewJson(data, entity.id) + "");
                    strJson.Append("},");
                }
                strJson = strJson.Remove(strJson.Length - 1, 1);
            }
            strJson.Append("]");
            return strJson.ToString();
        }
    }
}
