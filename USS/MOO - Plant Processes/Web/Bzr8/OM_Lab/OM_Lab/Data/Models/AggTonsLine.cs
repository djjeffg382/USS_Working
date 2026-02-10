namespace OM_Lab.Data.Models
{
    public class AggTonsLine
    {
        public int Line { get; set; }
        public int WestPointId { get; set; }
        public string AnalogPoint { get; set; } = "";
        public string PulsePoint { get; set; } = "";
        public MOO.DAL.West_Main.WestMainPlants Plant { get; set; }
        public string GBPoint { get; set; } = "";
        public List<AggTonLineVals> Values { get; set; } = [];

        /// <summary>
        /// Gets the values for the given shiftDate
        /// </summary>
        /// <param name="ShiftDate"></param>
        public async Task GetValues(DateTime ShiftDate)
        {
            Values = [];
            //If using timestep, the filter will show time as end value
            //Shift 1 starts at 22:30 but ends at 23:30 so we need the filter to be from 23:30 - 23:30.
            DateTime startDate = ShiftDate.AddMinutes(-90).AddHours(1);  
            DateTime endDate = startDate.AddHours(23);

            var pulseVals = await Task.Run(() => MOO.DAL.Pi.Services.PiAggregateSvc.GetPiTotal(startDate, endDate, PulsePoint, TimeStep: "1h"));
            int hr = 1;
            //Get the pulse values
            foreach (var pVal in pulseVals)
            {
                Values.Add(new AggTonLineVals() { Hour = hr, PulseTons = Math.Round(pVal.Value.GetValueOrDefault(0) *2880,2, MidpointRounding.AwayFromZero), StartDate = pVal.Time.AddHours(-1) });
                hr++;
            }

            var analogVals = await Task.Run(() => MOO.DAL.Pi.Services.PiAggregateSvc.GetPiTotal(startDate, endDate, AnalogPoint, TimeStep: "1h"));
            var gbValPI = await Task.Run(() => MOO.DAL.Pi.Services.PiCalcSvc.Get(startDate, startDate, $"'{GBPoint}'"));
            double gbVal = gbValPI[0].Value.GetValueOrDefault(0);
            hr = 1;
            //Get the analog values
            foreach (var aVal in analogVals)
            {
                var lineVal = Values.FirstOrDefault(x => x.Hour == hr);
                if (lineVal != null)
                    lineVal.AnalogTons = Math.Round(aVal.Value.GetValueOrDefault(0) * 24 * gbVal, 2, MidpointRounding.AwayFromZero);
                hr++;
            }

            //get values from west_hourly_{PLANT}
            int ymd = int.Parse(ShiftDate.ToString("yyyyMMdd"));
            var reported = await Task.Run(() => MOO.DAL.West_Main.Services.West_HourlySvc.GetYMDRange(Plant, WestPointId, ymd, ymd));
            foreach(var rVal in reported)
            {
                var lineVal = Values.FirstOrDefault(x=> x.Hour == rVal.Hour);
                if (lineVal != null)
                {
                    lineVal.ReportedTons = Math.Round(rVal.Hour_Total, 2, MidpointRounding.AwayFromZero);
                    lineVal.WestHourlyRecord = rVal;
                }
                    
            }
        }
    }
}
