using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Core.Models.KTCLab
{
    public class KTC_Lab
    {
       
        /// <summary>
        /// Shift Date
        /// </summary>
        public DateTime Shift_Date { get; set; }

        public DateTime Start_Date_No_DST { get
            {
                return Start_Date.IsDaylightSavingTime() ? Start_Date.AddHours(-1) : Start_Date;
            }
        }
        public DateTime Start_Date { get
            {
                return MOO.Shifts.Shift8.ShiftStartEndTime(Shift_Date, 1, Plant.Keetac)[0];
            }
        }


        public decimal? ConcScreenStartWeight { get; set; }

        /// <summary>
        /// Concentrate screen +100 Mesh
        /// </summary>
        public decimal? ConcScreen100Mesh { get; set; }
        /// <summary>
        /// Concentrate Screen +200 Mesh
        /// </summary>
        public decimal? ConcScreen200Mesh { get; set; }
        /// <summary>
        /// concentrate Screen +325 Mesh
        /// </summary>
        public decimal? ConcScreen325Mesh { get; set; }

        /// <summary>
        /// Concentrate Screen 100 Mesh cumulative Percent
        /// </summary>
        public decimal? ConcScreen100MeshPct { get
            {
                if (ConcScreenStartWeight.HasValue && ConcScreen100Mesh.HasValue && ConcScreenStartWeight.Value > 0)
                    return (ConcScreen100Mesh / ConcScreenStartWeight) * 100;
                else return null;
            }
        }


        /// <summary>
        /// Concentrate Screen 200 Mesh cumulative Percent
        /// </summary>
        public decimal? ConcScreen200MeshPct
        {
            get
            {
                if (ConcScreenStartWeight.HasValue && ConcScreen200Mesh.HasValue && ConcScreenStartWeight.Value > 0)
                    return (ConcScreen200Mesh / ConcScreenStartWeight) * 100;
                else return null;
            }
        }


        /// <summary>
        /// Concentrate Screen 325 Mesh cumulative Percent
        /// </summary>
        public decimal? ConcScreen325MeshPct
        {
            get
            {
                if (ConcScreenStartWeight.HasValue && ConcScreen325Mesh.HasValue && ConcScreenStartWeight.Value > 0)
                    return (ConcScreen325Mesh / ConcScreenStartWeight) * 100;
                else return null;
            }
        }

        public Approval Approval { get; set; }


        private readonly List<PelletLab> _pelletLab = new();
        public List<PelletLab> PelletLab { get { return _pelletLab; } }



        private readonly List<ConcShift> _concShift = new();
        public List<ConcShift> ConcShift { get { return _concShift; } }


        /// <summary>
        /// Tails Magnetic Iron calculated value
        /// </summary>
        public decimal? TailsMagFePct { get
            {
                decimal sum = 0 ;
                byte count = 0;

                foreach(ConcShift cs in _concShift)
                {
                    if(cs.TailsHydro.HasValue && cs.TailsThickener.HasValue)
                    {
                        sum += cs.TailsHydro.Value + cs.TailsThickener.Value;
                        count++;
                    }
                }

                if (count > 0)
                    return (sum / count) / 2;
                else
                    return null;
            } 
        }


    }
}
