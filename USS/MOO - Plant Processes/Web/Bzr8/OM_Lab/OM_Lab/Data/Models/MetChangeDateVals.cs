namespace OM_Lab.Data.Models
{
    /// <summary>
    /// class used for what options were selected upon closing of the MetChangeDate dialog
    /// </summary>
    public class MetChangeDateVals
    {
        public DateTime MetDate { get; set; } = DateTime.Today.AddDays(-1);
        public bool IsMonthEnd { get; set; }
        public MOO.DAL.ToLive.Models.Met_Material Material { get; set; } = MOO.DAL.ToLive.Models.Met_Material.Flux;
    }
}
