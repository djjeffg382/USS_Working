namespace OM_Lab.Data.Models
{
    public class AggTonLineVals
    {
        public int Hour { get; set; }
        public DateTime StartDate;
        public DateTime EndDate { get { return StartDate.AddHours(1); } }
        public decimal ReportedTons { get; set; }
        public double PulseTons { get; set; }
        public double AnalogTons { get; set; }
        public double PulseMinusAnalog { get { return Math.Round(PulseTons - AnalogTons,2, MidpointRounding.AwayFromZero); } }
        public int Shift { get { return (int)Math.Floor((Hour - 1) / 8.0) + 1; } }
        public MOO.DAL.West_Main.Models.West_Hourly? WestHourlyRecord { get; set; }

    }
}
