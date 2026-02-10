using MOO.DAL.ToLive.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOO.Enums;
using MOO.Enums.Extension;
using System.ComponentModel.DataAnnotations;

namespace MOO.DAL.ToLive.Models
{
    /// <summary>
    /// Data imported from weather.gov website
    /// </summary>
    public sealed class MOO_Weather
    {
        /// <summary>
        /// Enums for different weather cities
        /// </summary>
        /// <remarks>
        /// Use https://www.weather.gov/zse/ZSEStationInfo?id=KHIB&tab=Map& to view nearby weather stations
        /// use Name for the Station Name and Description for the city name.
        /// </remarks>
        public enum Weather_City
        {
            [Display(Name ="KEVM", Description ="Eveleth")]
            Eveleth,
            [Display(Name = "KHIB", Description = "Hibbing")]
            Hibbing,
            [Display(Name = "HOYM5", Description = "Hoyt Lakes")]
            Hoyt_Lakes,
            [Display(Name = "KCQM", Description = "Cook")]
            Cook,
            [Display(Name = "KORB", Description = "Orr")]
            Orr,
            [Display(Name = "SIDM5", Description = "Side Lake")]
            Side_Lake

        }


        public long Weatherkey { get; set; }
        public Weather_City City { get; set; }
        public DateTime Thedate { get; set; }
        public string Sky { get; set; }



        //the following fields are VARCHARS in the database but should be as numbers.  I will convert to numbers for the Model
        private double? _dewpoint;
        //This is actually duepoint in the database but this is a spelling error
        public double? Dewpoint { get => _dewpoint;
            set
            {
                
                if (value.HasValue)
                    _dewpoint = Math.Round(value.Value, 2);
                else
                    _dewpoint = null;
            }
        }

        private double? _temperature;
        public double? Temperature
        {
            get => _temperature;
            set
            {
                if (value.HasValue)
                    _temperature = Math.Round(value.Value, 2);
                else
                    _temperature = null;
            }
        }

        private double? _relativeHumidity;
        public double? Relativehumidity
        {
            get => _relativeHumidity;
            set
            {
                if (value.HasValue)
                    _relativeHumidity = Math.Round(value.Value, 0);
                else
                    _relativeHumidity = null;
            }
        }

        private double? _pressure;
        public double? Pressure
        {
            get => _pressure;
            set
            {
                if (value.HasValue)
                    _pressure = Math.Round(value.Value, 5);
                else
                    _pressure = null;
            }
        }



        public string Wind { get; set; }
        public string Rawdata { get; set; }

    }
}
