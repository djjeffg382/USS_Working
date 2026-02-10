namespace MOO.DAL.ToLive.Models
{
    public class CO_Reason
    {
        public int Reason_Id { get; set; }
        public string? Reason_Name { get; set; }
        public bool Enabled { get; set; }
        public bool Req_Comment { get; set; }
        public string Additional_Info { get; set; }


    }
}