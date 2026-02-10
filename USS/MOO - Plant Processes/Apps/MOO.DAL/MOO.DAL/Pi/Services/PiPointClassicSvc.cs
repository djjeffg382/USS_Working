using MOO.DAL.Pi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;
using System.Data.Common;

namespace MOO.DAL.Pi.Services
{
    /// <summary>
    /// Service used for obtaining List of PI Points
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "PI Uses OLEDB, this will only be on Windows")]
    public static class PiPointClassicSvc
    {

        public const string PI_DEFAULT_SECURITY = "piadmin: A(r,w) | piadmins: A(r,w) | PIWorld: A(r)";
        static PiPointClassicSvc()
        {
            Util.RegisterOLEDB();
        }

        /// <summary>
        /// Gets the PI Point by the specified tag name
        /// </summary>
        /// <param name="Tag"></param>
        /// <returns></returns>
        public static PiPointClassic Get(string Tag)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE Tag = ?");


            using OleDbConnection conn = new(Data.GetConnectionString(Data.MNODatabase.MTC_Pi));
            conn.Open();
            OleDbCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.AddWithValue("Tag", Tag.ToUpper());
            var rdr = cmd.ExecuteReader();
            PiPointClassic retVal = null;
            if (rdr.HasRows)
            {
                rdr.Read();                
                retVal =  DataRowToObject(rdr);                
            }
            conn.Close();
            return retVal;
            
        }

        /// <summary>
        /// Returns a list of tags that matches the tag search string
        /// </summary>
        /// <param name="Tag"></param>
        /// <returns></returns>
        public static List<PiPointClassic> SearchByTag(string Tag)
        {
            List<PiPointClassic> retVal = new();
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE Tag LIKE '%' + ? + '%'");


            using OleDbConnection conn = new(Data.GetConnectionString(Data.MNODatabase.MTC_Pi));
            conn.Open();
            OleDbCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.AddWithValue("Tag", Tag.ToUpper());
            var rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    retVal.Add(DataRowToObject(rdr));
                }
                
            }

            conn.Close();
            return retVal;
        }

        /// <summary>
        /// Returns a list of tags that are from the PointSource
        /// </summary>
        /// <param name="PointSource"></param>
        /// <returns></returns>
        public static List<PiPointClassic> SearchByPointSource(string PointSource)
        {
            List<PiPointClassic> retVal = new();
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE PointSource = ?");


            using OleDbConnection conn = new(Data.GetConnectionString(Data.MNODatabase.MTC_Pi));
            conn.Open();
            OleDbCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.AddWithValue("PointSource", PointSource);
            var rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    retVal.Add(DataRowToObject(rdr));
                }

            }
            conn.Close();
            return retVal;
        }

        /// <summary>
        /// Returns all PI Tags
        /// </summary>
        /// <returns></returns>        
        public static List<PiPointClassic> GetAll()
        {
            List<PiPointClassic> retVal = new();
            StringBuilder sql = new();
            sql.Append(GetSelect());


            using OleDbConnection conn = new(Data.GetConnectionString(Data.MNODatabase.MTC_Pi));
            conn.Open();
            OleDbCommand cmd = new(sql.ToString(), conn);
            var rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    retVal.Add(DataRowToObject(rdr));
                }

            }
            conn.Close();
            return retVal;
        }



        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT *");
            sql.AppendLine("FROM [pipoint]..[classic]");
            return sql.ToString();
        }




        public static PiPointClassic InsertAnalogPoint(PiPointSource src, string TagName, string Description, string InstrumentTag, string EngUnits, Type type,
                            float MinValue, float MaxValue, bool StepPlot, bool ExceptionIsPercent, float ExceptionValue, bool CompressionIsPercent, float CompressionValue,
                            ScanTime Scan)
        {
            PiPointClassic pt = new()
            {
                Tag = TagName,
                Descriptor = Description,
                EngUnits = EngUnits,
                PtSecurity = PI_DEFAULT_SECURITY,
                TypicalValue = MinValue,
                Zero = MinValue,
                Span = (MaxValue - MinValue),
                Step = StepPlot,
                Location2 = 0,
                Location3 = 1,
                Location4 = GetScanLocationNumber(Scan),
                Location5 = 0,
                Convers = 0,
                TotalCode = 0,
                InstrumentTag = InstrumentTag,
                Compressing = true
                
            };

            if (ExceptionIsPercent)
                pt.ExcDevPercent = ExceptionValue;
            else
                pt.ExcDev = ExceptionValue;

            if (CompressionIsPercent)
                pt.CompDevPercent = CompressionValue;
            else
                pt.CompDev = CompressionValue;



            SetPropertiesBySource(src, pt);
            if (type == typeof(short))
                pt.PointTypeX = "int16";
            else if (type == typeof(int))
                pt.PointTypeX = "int32";
            else if (type == typeof(float))
                pt.PointTypeX = "float32";
            else if (type == typeof(double))
                pt.PointTypeX = "float64";
            else
                throw new Exception("Invalid parameter 'type', valid types are (short, int, float, and double)");

            Insert(pt);
            return Get(TagName);
        }

        public static PiPointClassic InsertFuturePoint(string TagName, string Description, string EngUnits, 
                            float Zero, float Span, string ExDesc, string PointSource)
        {
            PiPointClassic pt = new()
            {
                Tag = TagName,
                Descriptor = Description,
                EngUnits = EngUnits,
                PtSecurity = PI_DEFAULT_SECURITY,
                TypicalValue = Zero,
                Zero = Zero,
                Span = Span,
                Step = true,
                ExDesc = ExDesc,
                Compressing = false,
                PointSource = PointSource,
                PointTypeX = "float32"

            };
            Insert(pt);
            return Get(TagName);
        }



        public static PiPointClassic InsertDigitalPoint(PiPointSource src, string TagName, string Description, string InstrumentTag, string EngUnits,                            
                            ScanTime Scan, string DigitalSet)
        {
            PiPointClassic pt = new()
            {
                Tag = TagName,
                Descriptor = Description,
                EngUnits = EngUnits,
                PtSecurity = PI_DEFAULT_SECURITY,
                Location2 = 0,
                Location3 = 1,
                Location4 = GetScanLocationNumber(Scan),
                Location5 = 0,
                Convers = 1,
                TotalCode = 6, //needed to do an AND calculation with value of convers
                InstrumentTag = InstrumentTag,
                PointTypeX = "digital",
                DigitalSet = DigitalSet
            };

            SetPropertiesBySource(src, pt);
            

            Insert(pt);
            return Get(TagName);
        }



        /// <summary>
        /// Sets the pointsource and location1 value base on the what the source will be
        /// </summary>
        /// <param name="src"></param>
        /// <param name="pt"></param>
        private static void SetPropertiesBySource(PiPointSource src, PiPointClassic pt)
        {
            switch (src)
            {
                case PiPointSource.MTC_Crush:
                    pt.PointSource = "M_CRUSH";
                    pt.Location1 = 0;
                    break;
                case PiPointSource.MTC_Conc:
                    pt.PointSource = "M_CONC";
                    pt.Location1 = 0;
                    break;
                case PiPointSource.MTC_Agg2:
                    pt.PointSource = "M_AGG2_DROP202";
                    pt.Location1 = 1;
                    break;
                case PiPointSource.MTC_Agg3:
                    pt.PointSource = "M_AGG3";
                    pt.Location1 = 0;
                    break;
                case PiPointSource.MTC_Util:
                    pt.PointSource = "M_UTIL_DROP212";
                    pt.Location1 = 4;
                    break;
                case PiPointSource.KTC_Scada:
                    pt.PointSource = "K_SCADA";
                    pt.Location1 = 0;
                    break;
            }
        }

        private static short GetScanLocationNumber(ScanTime st)
        {
            //see "Q:\Departments\BSC\Plant Processes\Systems\Applications\PI\PI.docx" for table of Location4 values.  
            //all opc's *should* be set up this way.  Some scan times have multiple scan classes but are staggered.
            //example 30 second scan has 5 total classes.  we will use random number to pick which one to use so we stagger the collection of data

            Random rand = new();
            return st switch
            {
                ScanTime.Seconds_1 => 1,
                ScanTime.Seconds_5 => (short)(3 + rand.Next(0, 1)),
                ScanTime.Seconds_10 => (short)(7 + rand.Next(0, 4)),
                ScanTime.Seconds_30 => (short)(17 + rand.Next(0, 4)),
                _ => (short)(17 + rand.Next(0, 4)),
            };
        }


        /// <summary>
        /// Updates specific properites of the pi tag (descriptor, zero, span, exdesc)
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        /// <remarks>Columns we are updating are restricted as I don't want to mess up how a point works in the system</remarks>
        public static int Update(PiPointClassic pt)
        {
            using OleDbConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.MTC_Pi));
            conn.Open();
            StringBuilder sql = new();
            sql.AppendLine("UPDATE [pipoint]..[classic]");
            sql.AppendLine("SET descriptor = ?, zero = ?, span = ?,");
            sql.AppendLine("exdesc = ?");
            sql.AppendLine("WHERE tag = ?");
            OleDbCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.AddWithValue("descriptor", pt.Descriptor);
            cmd.Parameters.AddWithValue("zero", pt.Zero);
            cmd.Parameters.AddWithValue("span", pt.Span);
            cmd.Parameters.AddWithValue("ExDesc", pt.ExDesc);

            cmd.Parameters.AddWithValue("Tag", pt.Tag);
            int retVal = cmd.ExecuteNonQuery();
            conn.Close();

            return retVal;
        }


        /// <summary>
        /// Inserts the pi point
        /// </summary>
        /// <param name="pt">Pi Point to be inserted</param>
        /// <returns></returns>
        private static int Insert(PiPointClassic pt)
        {
            using OleDbConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.MTC_Pi));
            conn.Open();
            StringBuilder sql = new();
            sql.AppendLine($"INSERT INTO [pipoint]..[classic] ");
            sql.AppendLine("(Tag, Descriptor, pointsource, engunits, pointtypex, ptsecurity, ");
            sql.Append("    typicalvalue, zero, span, step, future, exdesc, compressing, ");

            //determine if we are using exception/compression percent or individual
            if (pt.ExcDevPercent > 0)
                sql.AppendLine("excdevpercent, ");
            else
                sql.AppendLine("excdev, ");

            if (pt.CompDevPercent > 0)
                sql.AppendLine("compdevpercent, ");
            else
                sql.AppendLine("compdev, ");

            sql.AppendLine("    location1, location2, location3, location4, location5,");
            sql.AppendLine("    convers, totalcode, instrumenttag, digitalset)");
            sql.AppendLine("VALUES(?, ?, ?, ?, ?, ?,");
            sql.AppendLine("        ?, ?, ?, ?, ?, ?, ?,");
            sql.AppendLine("        ?, ?,");  //excdevpercent, excdev
            sql.AppendLine("        ?, ?, ?, ?, ?,");  //Loaction 1 - 5
            sql.AppendLine("        ?, ?, ?, ?)");

            OleDbCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.AddWithValue("Tag", pt.Tag);
            cmd.Parameters.AddWithValue("Descriptor", pt.Descriptor);
            cmd.Parameters.AddWithValue("pointsource", pt.PointSource);
            cmd.Parameters.AddWithValue("engunits", pt.EngUnits);
            cmd.Parameters.AddWithValue("pointtypex", pt.PointTypeX);
            cmd.Parameters.AddWithValue("ptsecurity", pt.PtSecurity);


            cmd.Parameters.AddWithValue("typicalvalue", pt.TypicalValue);
            cmd.Parameters.AddWithValue("zero", pt.Zero);
            cmd.Parameters.AddWithValue("span", pt.Span);
            cmd.Parameters.AddWithValue("step", pt.Step);
            cmd.Parameters.AddWithValue("future", pt.Future ? 1:0);
            cmd.Parameters.AddWithValue("ExDesc", pt.ExDesc);
            cmd.Parameters.AddWithValue("Compressing", pt.Compressing ? 1:0);



            if (pt.ExcDevPercent > 0)
                cmd.Parameters.AddWithValue("excdevpercent", pt.ExcDevPercent);
            else
                cmd.Parameters.AddWithValue("excdev", pt.ExcDev);

            if (pt.CompDevPercent > 0)
                cmd.Parameters.AddWithValue("compdev", pt.CompDevPercent); 
            else
                cmd.Parameters.AddWithValue("compdev", pt.CompDev);

            cmd.Parameters.AddWithValue("location1", pt.Location1);
            cmd.Parameters.AddWithValue("location2", pt.Location2);
            cmd.Parameters.AddWithValue("location3", pt.Location3);
            cmd.Parameters.AddWithValue("location4", pt.Location4);
            cmd.Parameters.AddWithValue("location5", pt.Location5);


            cmd.Parameters.AddWithValue("convers", pt.Convers);
            cmd.Parameters.AddWithValue("totalcode", pt.TotalCode);
            cmd.Parameters.AddWithValue("instrumenttag", pt.InstrumentTag);
            cmd.Parameters.AddWithValue("digitalset", pt.DigitalSet);

            return cmd.ExecuteNonQuery();

        }

        private static PiPointClassic DataRowToObject(DbDataReader row)
        {
            PiPointClassic retVal = new();

            retVal.Archiving = ((short)Util.GetRowVal(row, "archiving")) == 1;
            retVal.ChangedDate = (DateTime)Util.GetRowVal(row, "changedate");
            retVal.Changer = (string)Util.GetRowVal(row, "changer");  
            retVal.CompDev = (float)Util.GetRowVal(row, "compdev"); 
            retVal.CompDevPercent = (float)Util.GetRowVal(row, "compdevpercent"); 
            retVal.CompMin = (int)Util.GetRowVal(row, "compmin"); 
            retVal.CompMax = (double)Util.GetRowVal(row, "compmax"); 
            retVal.Compressing = (short)Util.GetRowVal(row, "compressing") == 1;
            retVal.Convers = (float)Util.GetRowVal(row, "convers"); 
            retVal.CreationDate = (DateTime)Util.GetRowVal(row, "creationdate");
            retVal.Creator = (string)Util.GetRowVal(row, "creator");
            retVal.DataAccess = (string)Util.GetRowVal(row, "dataaccess"); 
            retVal.DataGroup = (string)Util.GetRowVal(row, "datagroup"); 
            retVal.DataOwner = (string)Util.GetRowVal(row, "dataowner"); 
            retVal.DataSecurity = (string)Util.GetRowVal(row, "datasecurity");
            retVal.Descriptor = (string)Util.GetRowVal(row, "descriptor"); 
            retVal.DigitalSet = (string)Util.GetRowVal(row, "digitalset"); 
            retVal.DisplayDigits = (short)Util.GetRowVal(row, "displaydigits"); 
            retVal.EngUnits = (string)Util.GetRowVal(row, "engunits"); 
            retVal.ExcDev = (float)Util.GetRowVal(row, "excdev");
            retVal.ExcDevPercent = (float)Util.GetRowVal(row, "excdevpercent");
            retVal.ExcMax = (double)Util.GetRowVal(row, "excmax"); 
            retVal.ExcMin = (int)Util.GetRowVal(row, "excmin");
            retVal.ExDesc = (string)Util.GetRowVal(row, "exdesc");
            retVal.FilterCode = (short)Util.GetRowVal(row, "filtercode"); 
            retVal.Future = (short)Util.GetRowVal(row, "future") == 1;
            retVal.InstrumentTag = (string)Util.GetRowVal(row, "instrumenttag");
            retVal.Location1 = (int)Util.GetRowVal(row, "location1");
            retVal.Location2 = (int)Util.GetRowVal(row, "location2"); 
            retVal.Location3 = (int)Util.GetRowVal(row, "location3"); 
            retVal.Location4 = (int)Util.GetRowVal(row, "location4"); 
            retVal.Location5 = (int)Util.GetRowVal(row, "location5"); 
            retVal.PointId = (double)Util.GetRowVal(row, "pointid"); 
            retVal.PointNumber = (double)Util.GetRowVal(row, "pointnumber");
            retVal.PointSource = (string)Util.GetRowVal(row, "pointsource"); 
            retVal.PointType = (string)Util.GetRowVal(row, "pointtype"); 
            retVal.PointTypeX = (string)Util.GetRowVal(row, "pointtypex");
            retVal.PtAccess = (string)Util.GetRowVal(row, "ptaccess"); 
            retVal.PtClassId = (double)Util.GetRowVal(row, "ptclassid");
            retVal.PtClassName = (string)Util.GetRowVal(row, "ptclassname"); 
            retVal.PtClassRev = (double)Util.GetRowVal(row, "ptclassrev");
            retVal.PtGroup = (string)Util.GetRowVal(row, "ptgroup"); 
            retVal.PtOwner = (string)Util.GetRowVal(row, "ptowner");
            retVal.PtSecurity = (string)Util.GetRowVal(row, "ptsecurity"); 
            retVal.Scan = (short)Util.GetRowVal(row, "scan") == 1; 
            retVal.Shutdown = (short)Util.GetRowVal(row, "shutdown") == 1; 
            retVal.SourceTag = (string)Util.GetRowVal(row, "sourcetag");
            retVal.Span = (float)Util.GetRowVal(row, "span"); 
            retVal.SquareRoot = (short)Util.GetRowVal(row, "squareroot"); 
            retVal.SrcPtId = (int)Util.GetRowVal(row, "srcptid"); 
            retVal.Step = (short)Util.GetRowVal(row, "step") == 1; 
            retVal.Tag = (string)Util.GetRowVal(row, "tag"); 
            retVal.TotalCode = (short)Util.GetRowVal(row, "totalcode"); 
            retVal.TypicalValue = (float)Util.GetRowVal(row, "typicalvalue");
            retVal.UserInt1 = (int)Util.GetRowVal(row, "userint1");
            retVal.UserInt2 = (int)Util.GetRowVal(row, "userint2");
            retVal.UserReal1 = (float)Util.GetRowVal(row, "userreal1");
            retVal.UserReal2 = (float)Util.GetRowVal(row, "userreal2");
            retVal.Zero = (float)Util.GetRowVal(row, "zero");

            return retVal;
        }
    }
}
