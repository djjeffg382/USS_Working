using MOO.DAL.Pi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Pi.Services
{
    public static class DigitalStateSvc
    {
        static DigitalStateSvc()
        {
            Util.RegisterOLEDB();
        }



        public static List<DigitalState> GetAll()
        {
            List<DigitalState> retVal = new();
            StringBuilder sql = new();
            sql.Append(GetSelect());


            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.MTC_Pi);
            DigitalState digState = new();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                if (dr.Field<string>("digitalset") != digState.DigitalSet)
                {
                    //new digital set, create new and add to list
                    digState = new()
                    {
                        DigitalSet = dr.Field<string>("digitalset"),
                        Code = dr.Field<int>("code")
                    };
                    retVal.Add(digState);
                }
                digState.DigValues.Add(dr.Field<string>("name"), dr.Field<int>("offset"));
            }
            return retVal;
        }


        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT *");
            sql.AppendLine("FROM [pids]..[pids]");
            sql.AppendLine("WHERE digitalset NOT IN ('SYSTEM', 'pialarm33','pialarmcontrol','pisqcalarm','UFO_State')");
            sql.AppendLine("ORDER BY digitalset, offset");
            return sql.ToString();
        }




    }
}
