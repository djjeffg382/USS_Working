using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using MOO.DAL.ToLive.Models;

namespace MOO.DAL.ToLive.Services
{
    public class Sec_ApplicationSvc
    {
        static Sec_ApplicationSvc()
        {
            Util.RegisterOracle();
        }

        /// <summary>
        /// Gets the application object by the application ID
        /// </summary>
        /// <param name="app_Id"></param>
        /// <returns></returns>
        public static Sec_Application Get(int app_Id)
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT *");
            sql.AppendLine("FROM tolive.sec_application");
            sql.AppendLine($"WHERE application_id = {app_Id}");

            DataSet ds = Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            if (ds.Tables[0].Rows.Count == 1)
            {
                return DataRowToObject(ds.Tables[0].Rows[0]);
            }
            else
            {
                return null;
            }

        }


        public static List<Sec_Application> GetAll()
        {
            List<Sec_Application> retVal = new();
            StringBuilder sql = new();
            sql.AppendLine("SELECT *");
            sql.AppendLine("FROM tolive.sec_application");
            sql.AppendLine($"ORDER BY application_name");

            DataSet ds = Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                retVal.Add(DataRowToObject(dr));
            }
            return retVal;
        }


        /// <summary>
        /// Updates the specified sec_application record
        /// </summary>
        /// <param name="app"></param>
        public static void Update(Sec_Application app)
        {
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                Update(app, conn);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }
        }
        /// <summary>
        /// Updates the specified sec_application record
        /// </summary>
        /// <param name="app"></param>
        /// <param name="conn"></param>
        public static void Update(Sec_Application app, OracleConnection conn)
        {
            StringBuilder sql = new();

            sql.AppendLine("UPDATE tolive.sec_application");
            sql.AppendLine("    SET application_name = :application_name,");
            sql.AppendLine("    application_description = :application_description");
            sql.AppendLine("    created_date = :created_date, created_by = :created_by,");
            sql.AppendLine("    modified_by = :modified_by,");
            sql.AppendLine("    application_notes = :application_notes");
            sql.AppendLine($"WHERE application_id = {app.Application_Id}");

            OracleCommand upd = new(sql.ToString(), conn);
            upd.Parameters.Add("application_name", app.Application_Name);
            upd.Parameters.Add("application_description", app.Application_Description);
            upd.Parameters.Add("created_date", app.Created_Date);
            upd.Parameters.Add("created_by", app.Created_By);
            upd.Parameters.Add("modified_by", app.Modified_By);
            upd.Parameters.Add("application_notes", app.Application_Notes);

            MOO.Data.ExecuteNonQuery(upd);
        }


        /// <summary>
        /// Inserts the application into the sec_application table
        /// </summary>
        /// <param name="app"></param>
        public static void Insert(Sec_Application app)
        {
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                Insert(app, conn);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// Inserts the application into the sec_application table
        /// </summary>
        /// <param name="app"></param>
        /// <param name="conn"></param>
        public static void Insert(Sec_Application app, OracleConnection conn)
        {
            if(app.Application_Id <= 0)
                app.Application_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.seq_security"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.sec_application");
            sql.AppendLine("(application_id, application_name, application_description,");
            sql.AppendLine("    created_date, created_by, modified_by, application_notes)");
            sql.AppendLine($"VALUES({app.Application_Id}, :application_name, :application_description,");
            sql.AppendLine($"   :created_date, :created_by, :modified_by, :application_notes)");

            OracleCommand ins = new(sql.ToString(), conn);
            ins.Parameters.Add("application_name", app.Application_Name);
            ins.Parameters.Add("application_description", app.Application_Description);
            ins.Parameters.Add("created_date", app.Created_Date);
            ins.Parameters.Add("created_by", app.Created_By);
            ins.Parameters.Add("modified_by", app.Modified_By);
            ins.Parameters.Add("application_notes", app.Application_Notes);

            MOO.Data.ExecuteNonQuery(ins);
        }


        public static List<Sec_Application> GetAllApplications()
        {
            List<Sec_Application> RetVal = new();
            StringBuilder sql = new();
            sql.AppendLine("SELECT sa.* ");
            sql.AppendLine("FROM tolive.sec_application sa");           
            sql.AppendLine("ORDER BY application_name");

            DataSet ds = Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                RetVal.Add(DataRowToObject(row));
            }
            return RetVal;
        }

        private static Sec_Application DataRowToObject(DataRow row)
        {
            Sec_Application RetVal = new();

            RetVal.Application_Id = Convert.ToInt32(row["Application_Id"]);
            RetVal.Application_Name = row["Application_Name"].ToString();
            RetVal.Application_Description = row["Application_Description"].ToString();
            RetVal.Created_Date = Convert.ToDateTime(row["created_date"]);
            RetVal.Created_By = row.IsNull("Created_By") ? "" : row["Created_By"].ToString();
            RetVal.Modified_By = row.IsNull("Modified_By") ? "" : row["Modified_By"].ToString();
            RetVal.Application_Notes = row.IsNull("Application_Notes") ? "" : row["Application_Notes"].ToString();

            return RetVal;
        }
    }
}
