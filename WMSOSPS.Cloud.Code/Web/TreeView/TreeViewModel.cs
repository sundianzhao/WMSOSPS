
namespace WMSOSPS.Cloud.Code.Web.TreeView
{
    public class TreeViewModel
    {
        public string parentId { get; set; }
        public string id { get; set; }
        public string nodeType { get; set; }
        public string text { get; set; }
        public string value { get; set; }
        public int? checkstate { get; set; }
        public bool showcheck { get; set; }
        public bool complete { get; set; }
        /// <summary>
        /// 前台展示树是否默认展开
        /// </summary>
        public bool isexpand { get; set; }
        public bool hasChildren { get; set; }
        public string img { get; set; }
        public string title { get; set; }


        public string ExtrasInfo { get; set; }
        public string EnterpriseID { get; set; }
        public string CallInType { get; set; }
        public string WorkCallFile { get; set; }
        public string strIsWorker { get; set; }
        public string ISAdd { get; set; }
        public string CallID { get; set; }
        public string ACDType { get; set; }
        public string WorkerID { get; set; }
        public string State { get; set; }
        public  string Pid { get; set; }
        public string IsWorker { get; set; }
        public string IVRType { get; set; }

        public string iconCls { get; set; }
    }
    
}
