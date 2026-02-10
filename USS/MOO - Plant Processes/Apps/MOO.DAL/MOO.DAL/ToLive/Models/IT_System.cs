using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MOO.DAL.ToLive.Models
{
    public sealed class IT_System
    {
       

        public enum ItSystemType
        {

            [Display(Name = "ASP.Net 4.x Web Application")]
            ASPNetWebApp,

            [Display(Name = "Blazor Web Application")]
            BlazorWeb,

            [Display(Name = "Classic ASP")]
            ClassicASP,


            [Display(Name = "Scheduled Job Task")]
            Job,

            [Display(Name = "REST or SOAP API")]
            WebAPI,


            [Display(Name = "Windows Service")]
            WindowsService,

            [Display(Name = "Library")]
            Library,

            [Display(Name = "Other")]
            Other
        }

        [Key]
        public int It_System_Id { get; set; }

        public string System_Name { get; set; }
        public string Description { get; set; }
        public ItSystemType? System_Type { get; set; }
        public string Other_Documents { get; set; }
        public string Running_On { get; set; }
        public string Graylog_App_Name { get; set; }
        public string DotNetVersion { get; set; }
        public bool Active { get; set; } = true;
        public string Notes { get; set; }

        
    }
}
