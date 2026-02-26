using MOO.DAL.Pi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PI = MOO.DAL.Pi;

namespace MOO.DAL.Pi.Services
{
    public static class MtcAggScaleMonitorSvc
    {

        /// <summary>
        /// The PI Timestep that will be used for all of the points
        /// </summary>
        public const string TIME_STEP = "10s";

        private struct PointAverage
        {

            public PointAverage()
            {
                Count = 0;
                Sum = 0;
            }
            public double Count;
            public double Sum;
            public readonly double Average()
            {
                if (Count > 0)
                    return Math.Round(Sum / Count, 2, MidpointRounding.AwayFromZero);
                else
                    return 0;
            }
            public void AddToSum(double value)
            {
                Sum += value;
                Count++;
            }
        }

        /// <summary>
        /// Returns the data for the 
        /// </summary>
        /// <param name="StartDate">Start Time</param>
        /// <param name="EndDate">End Time</param>
        /// <param name="Req_Good_Time_Seconds">Number of seconds the where it is running above required percent of setpoing</param>
        /// <param name="Req_Pct_Of_Setpoint">Required percent of setpoint scale is running at to consider good time</param>
        public static IEnumerable<MtcAggScaleMonitor> GetScaleMonitorData(DateTime StartDate, DateTime EndDate, short Req_Good_Time_Seconds = 900, short Req_Pct_Of_Setpoint = 60)
        {            
            var assembly = Assembly.GetExecutingAssembly(); 
            using var stream = assembly.GetManifestResourceStream("MOO.DAL.Pi.MtcAgglScaleMonitorPoints.json"); 
            using var reader = new StreamReader(stream); 
            var json = reader.ReadToEnd();
            var scales = JsonSerializer.Deserialize<MtcAggScaleMonitor[]>(json);
            foreach (var scale in scales)
            {
                CalcMonitor(scale,StartDate,EndDate, Req_Good_Time_Seconds, Req_Pct_Of_Setpoint);
            }
            return scales; 
        }


        /// <summary>
        /// Gets the data report for the specified PI Points of the scale monitor
        /// </summary>
        /// <param name="StartDate">Start Time</param>
        /// <param name="EndDate">End Time</param>
        /// <param name="Req_Good_Time_Seconds">Number of seconds the where it is running above required percent of setpoing</param>
        /// <param name="Req_Pct_Of_Setpoint">Required percent of setpoint scale is running at to consider good time</param>
        /// <returns></returns>
        private static void CalcMonitor(MtcAggScaleMonitor Scale, DateTime StartDate, DateTime EndDate, short Req_Good_Time_Seconds = 900, short Req_Pct_Of_Setpoint = 60)
        {
            var mcx027List = PiDigSvc.Get(StartDate, EndDate, Scale.Mcx027Tag, TIME_STEP);
            var mcx030List = PiDigSvc.Get(StartDate, EndDate, Scale.Mcx030Tag, TIME_STEP);
            var sp027List = PiInterpSvc.Get(StartDate, EndDate, Scale.Sp027Tag, TIME_STEP);
            var ai027List = PiInterpSvc.Get(StartDate, EndDate, Scale.Ai027Tag, TIME_STEP);
            var ai030List = PiInterpSvc.Get(StartDate, EndDate, Scale.Ai030Tag, TIME_STEP);
            var pa030List = PiInterpSvc.Get(StartDate, EndDate, Scale.Pa030Tag, TIME_STEP);

            //join all the tags together on the time property
            var query = from Mcx027 in mcx027List
                        join Mcx030 in mcx030List
                            on Mcx027.Time equals Mcx030.Time
                        join Sp027 in sp027List
                            on Mcx027.Time equals Sp027.Time
                        join Ai027 in ai027List
                            on Mcx027.Time equals Ai027.Time
                        join Ai030 in ai030List
                            on Mcx027.Time equals Ai030.Time
                        join Pa030 in pa030List
                            on Mcx027.Time equals Pa030.Time
                        select new
                        {
                            Mcx027.Time,
                            Mcx027 = Mcx027.Value,
                            Mcx030 = Mcx030.Value,
                            Sp027 = Sp027.Value,
                            Ai027 = Ai027.Value,
                            Ai030 = Ai030.Value,
                            Pa030 = Pa030.Value

                        };

            //this will be used to know when we start to have good values for averaging
            DateTime GoodTime = DateTime.MaxValue;

            PointAverage ai027Avg = new();
            PointAverage ai030Avg = new();
            PointAverage pa030Avg = new();
            foreach (var data in query)
            {
                if (GoodTime > data.Time)
                {
                    //we are currently not at the correct level so check if we are now
                    if (IsRunningGood(data.Mcx027, data.Mcx030, data.Sp027, data.Ai027, Req_Pct_Of_Setpoint))
                        GoodTime = data.Time;
                }
                else
                {
                    //we have been running good so lets make sure we still are
                    if (!IsRunningGood(data.Mcx027, data.Mcx030, data.Sp027, data.Ai027, Req_Pct_Of_Setpoint))
                        GoodTime = DateTime.MaxValue;  //we aren't running good anymore so set goodtime to max date so we stop recording
                }
                //now check if we have been running good for the required time
                if (data.Time.Subtract(GoodTime).TotalSeconds > Req_Good_Time_Seconds)
                {
                    //record averages
                    ai027Avg.AddToSum(data.Ai027.GetValueOrDefault(0));
                    ai030Avg.AddToSum(data.Ai030.GetValueOrDefault(0));
                    pa030Avg.AddToSum(data.Pa030.GetValueOrDefault(0) * 120); //pulse must be multiplied by 120
                }
            }
            Scale.Ana030Avg = ai030Avg.Average();
            Scale.Ana027Avg = ai027Avg.Average();
            Scale.Pulse030Avg = pa030Avg.Average();


        }


        private static bool IsRunningGood(int? Mcx027, int? Mcx030, double? Sp027, double? Ai027, short Req_Pct_Of_Setpoint)
        {
            if (Sp027.GetValueOrDefault(0) > 0)
            {
                double ltphPct = Ai027.GetValueOrDefault(0) / Sp027.GetValueOrDefault(0) * 100;
                if (Mcx027.GetValueOrDefault(0) == 1 && Mcx030.GetValueOrDefault(0) == 1 && ltphPct > Req_Pct_Of_Setpoint)
                    return true;
            }
            return false;
        }

    }
}
