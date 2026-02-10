using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Pi.Models
{
    public class PiPointClassic
    {
        public bool Archiving { get; set; }
        public DateTime ChangedDate { get; set; }
        public string Changer { get; set; }
        public float CompDev { get; set; }
        public float CompDevPercent { get; set; }
        public double CompMax { get; set; }
        public int CompMin { get; set; }
        public bool Compressing { get; set; }
        public float Convers { get; set; }
        public DateTime CreationDate { get; set; }
        public string Creator { get; set; }
        public string DataAccess { get; set; }
        public string DataGroup { get; set; }
        public string DataOwner { get; set; }
        public string DataSecurity { get; set; }
        public string Descriptor { get; set; }
        public string DigitalSet { get; set; }
        public int DisplayDigits { get; set; }
        public string EngUnits { get; set; }
        public float ExcDev { get; set; }
        public float ExcDevPercent { get; set; }
        public double ExcMax { get; set; }
        public int ExcMin { get; set; }
        public string ExDesc { get; set; }
        public int FilterCode { get; set; }
        public bool Future { get; set; } = false;
        public string InstrumentTag { get; set; }
        public int Location1 { get; set; }
        public int Location2 { get; set; }
        public int Location3 { get; set; }
        public int Location4 { get; set; }
        public int Location5 { get; set; }
        public double PointId { get; set; }
        public double PointNumber { get; set; }
        public string PointSource { get; set; }
        public string PointType { get; set; }
        public string PointTypeX { get; set; }
        public string PtAccess { get; set; }

        public double PtClassId { get; set; }
        public string PtClassName { get; set; }
        public double PtClassRev { get; set; }
        public string PtGroup { get; set; }
        public string PtOwner { get; set; }
        public string PtSecurity { get; set; }
        public bool Scan { get; set; }
        public bool Shutdown { get; set; }
        public string SourceTag { get; set; }
        public float Span { get; set; }
        public short SquareRoot { get; set; }
        public int SrcPtId { get; set; }
        public bool Step { get; set; } = false;
        public string Tag { get; set; }
        public short TotalCode { get; set; }
        public float TypicalValue { get; set; }
        public int UserInt1 { get; set; }
        public int UserInt2 { get; set; }
        public float UserReal1 { get; set; }
        public float UserReal2 { get; set; }
        public float Zero { get; set; }



    }
}
