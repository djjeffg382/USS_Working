namespace OM_Lab.Data.Models
{
    public class WinSvcInfo
    {
        public string ServerName { get; set; } = "";
        public string ServiceName { get; set; } = "";
        public string Status { get; set; } = "";
        public bool IsRestarting { get; set; } = false;
    }
}
