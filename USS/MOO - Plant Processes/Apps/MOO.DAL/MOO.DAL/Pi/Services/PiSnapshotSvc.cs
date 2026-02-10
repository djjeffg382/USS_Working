using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOO.DAL.Pi.Models;
using System.Data.OleDb;
using System.Data;

namespace MOO.DAL.Pi.Services
{
    /// <summary>
    /// Service used for obtaining the most recent value of a PI Tag
    /// </summary>
    public static class PiSnapshotSvc
    {
        static PiSnapshotSvc()
        {
            Util.RegisterOLEDB();
        }
        public static PiSnapshot Get(string tagName)
        {
            StringBuilder sql = new ();
            sql.AppendLine("SELECT a.*, DIGSTRING(CAST(a.value as Int32)) as state");
            sql.AppendLine("FROM [piarchive]..[pisnapshot] a");
            sql.AppendLine($"WHERE tag = ?");
            sql.AppendLine("");

            OleDbDataAdapter da = new(sql.ToString(),MOO.Data.GetConnectionString(Data.MNODatabase.MTC_Pi));
            da.SelectCommand.Parameters.Add("tagName", tagName);

            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count > 0)
                return DatarowToObj(ds.Tables[0].Rows[0]);
            else
                return null;
        }

        private static PiSnapshot DatarowToObj(DataRow row)
        {
            PiSnapshot retVal = new();
            retVal.Tag = row.Field<string>("tag");
            retVal.Time = row.Field<DateTime>("time");
            if (!row.IsNull("value"))
            {
                if (MOO.General.IsNumber(row["value"]))
                    retVal.ValueNbr = Convert.ToDecimal(row["value"]);
                else if (row["value"].GetType() == typeof(DateTime))
                        retVal.ValueDate = Convert.ToDateTime(row["value"]);
                else if (row["value"].GetType() == typeof(string))
                    retVal.ValueStr = Convert.ToString(row["value"]);
            }
            retVal.DigitalState = row.Field<string>("state");  
            retVal.Status = row.Field<int>("status");
            retVal.Substituted = row.Field<bool>("Substituted");
            retVal.Questionable = row.Field<bool>("questionable");
            retVal.Annotated = row.Field<bool>("annotated");
            retVal.Annotations = row.Field<string>("annotations");

            return retVal;
        }
        
    }
}
