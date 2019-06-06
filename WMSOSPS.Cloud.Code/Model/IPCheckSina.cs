namespace WMSOSPS.Cloud.Code.Model
{
    public class IPCheckSina
    {
        public int ret;
        public string start;
        public string end;
        public string country;

        public string province;

        public string city;

        public string district;

        public string isp;

        public string type;

        public string desc;
    }
    public class IPTaoBaoSina
    {
        public string code { get; set; }
        public IpTaobaoEntity data { get; set; }
    }
    public class IpTaobaoEntity
    {
        public string ip { get; set; }
        public string country { get; set; }
        public string area { get; set; }
        public string region { get; set; }
        public string city { get; set; }
        public string county { get; set; }
        public string isp { get; set; }
        public string country_id { get; set; }
        public string area_id { get; set; }
        public string region_id { get; set; }
        public string city_id { get; set; }
        public string county_id { get; set; }
        public string isp_id { get; set; }
    }
}
