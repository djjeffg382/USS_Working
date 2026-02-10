using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOO.DAL.Drill.Models;
using Microsoft.Data.SqlClient;

namespace MOO.DAL.Drill.Services
{
    public class Drill_BitSvc
    {
        static Drill_BitSvc()
        {
            Util.RegisterSqlServer();
        }


        public static Drill_Bit Get(int Drill_Bit_Id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE Drill_Bit_id = {Drill_Bit_Id}");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.USSDrillData);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }


        public static Drill_Bit Get(MOO.Plant Plant, string Serial_Number)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE Plant = @Plant");
            sql.AppendLine($"AND Serial_Number = @Serial_Number");

            SqlDataAdapter da = new(sql.ToString(),MOO.Data.GetConnectionString(Data.MNODatabase.USSDrillData));
            da.SelectCommand.Parameters.AddWithValue("Plant", Plant.ToString());
            da.SelectCommand.Parameters.AddWithValue("Serial_Number", Serial_Number);

            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }

        public static List<Drill_Bit> GetAll(MOO.Plant Plant)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE Plant = '{Plant}'");
            sql.AppendLine("ORDER BY Serial_Number");


            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.USSDrillData);

            List<Drill_Bit> retVal = new();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                retVal.Add(DataRowToObject(dr));
            }

            return retVal;
        }

        /// <summary>
        /// Returns a list of drill bits where filtered where the bit has been used in a hole within the date range or the bit last used date is null
        /// </summary>
        /// <param name="Plant"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<Drill_Bit> GetActiveByDateRange(MOO.Plant Plant, DateTime StartDate, DateTime EndDate)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE drill_bit_id IN (SELECT db.drill_bit_id");
            sql.AppendLine("                FROM Drill.dbo.drill_bit db");
            sql.AppendLine("                LEFT JOIN Drill.dbo.drilled_hole dh ON db.Drill_Bit_Id = dh.Drill_Bit_Id");
            sql.AppendLine($"               WHERE db.Plant = '{Plant}'");
            sql.AppendLine("                GROUP BY db.drill_bit_id");
            sql.AppendLine($"                HAVING MAX(Shift_Date) BETWEEN '{StartDate:MM/dd/yyyy}' AND '{EndDate:MM/dd/yyyy}' ");
            sql.AppendLine("                OR MAX(Shift_Date) IS NULL)");
            sql.AppendLine("ORDER BY Serial_Number");


            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.USSDrillData);

            List<Drill_Bit> retVal = new();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                retVal.Add(DataRowToObject(dr));
            }

            return retVal;
        }

        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine("Drill_Bit_Id, Plant, Serial_Number, manufacturer ");
            sql.AppendLine("FROM Drill.dbo.Drill_Bit");
            return sql.ToString();
        }



        /// <summary>
        /// Inserts the Drill Bit into the Drill database
        /// </summary>
        /// <param name="db">Drill Bit To insert</param>
        /// <returns></returns>
        public static int Insert(Drill_Bit db)
        {
            SqlConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.USSDrillData));
            conn.Open();
            try
            {
                return Insert(db, conn);
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
        /// Inserts the Drill Bit into the Drill Database
        /// </summary>
        /// <param name="db">Drill Bit To insert</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Insert(Drill_Bit db, SqlConnection conn)
        {
            
            if (db.Drill_Bit_Id <= 0)
                db.Drill_Bit_Id = Convert.ToInt32(MOO.Data.GetNextSequence("Drill.dbo.seq_Drill", MOO.Data.MNODatabase.USSDrillData));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO Drill.dbo.Drill_Bit (Drill_Bit_Id, Plant, Serial_Number,Manufacturer)");
            sql.AppendLine("VALUES(@Drill_Bit_Id, @Plant, @Serial_Number, @Manufacturer)");

            SqlCommand ins = new(sql.ToString(), conn);
            ins.Parameters.AddWithValue("Drill_Bit_Id", db.Drill_Bit_Id);
            ins.Parameters.AddWithValue("Plant", db.Plant.ToString());
            ins.Parameters.AddWithValue("Serial_Number", db.Serial_Number);
            ins.Parameters.AddWithValue("Manufacturer", (db.Manufacturer == null) ? DBNull.Value : db.Manufacturer);

            return ins.ExecuteNonQuery();

        }



        /// <summary>
        /// Updates the Drill Bit in the Drill database
        /// </summary>
        /// <param name="db">Drill Bit To update</param>
        /// <returns></returns>
        public static int Update(Drill_Bit db)
        {
            SqlConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.USSDrillData));
            conn.Open();
            try
            {
                return Update(db, conn);
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
        /// Update the Drill Bit in the Drill database
        /// </summary>
        /// <param name="db">Drill Bit To update</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Update(Drill_Bit db, SqlConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE Drill.dbo.Drill_Bit ");
            sql.AppendLine("    SET Plant = @Plant, Serial_Number = @Serial_Number, manufacturer = @manufacturer");
            sql.AppendLine("WHERE Drill_Bit_Id = @Drill_Bit_Id");

            SqlCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.AddWithValue("Plant", db.Plant.ToString());
            cmd.Parameters.AddWithValue("Serial_Number", db.Serial_Number);
            cmd.Parameters.AddWithValue("manufacturer", db.Manufacturer);
            cmd.Parameters.AddWithValue("Drill_Bit_Id", db.Drill_Bit_Id);

            return cmd.ExecuteNonQuery();

        }


        /// <summary>
        /// Deletes the Drill Bit in the Drill database
        /// </summary>
        /// <param name="db">Drill Bit To delete</param>
        /// <returns></returns>
        public static int Delete(Drill_Bit db)
        {
            SqlConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.USSDrillData));
            conn.Open();
            try
            {
                return Delete(db, conn);
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
        /// Deletes the Drill Bit in the Drill database
        /// </summary>
        /// <param name="db">Drill Bit To Delete</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Delete(Drill_Bit db, SqlConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM Drill.dbo.Drill_Bit ");
            sql.AppendLine("WHERE Drill_Bit_Id = @Drill_Bit_Id");

            SqlCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.AddWithValue("Drill_Bit_Id", db.Drill_Bit_Id);

            return cmd.ExecuteNonQuery();

        }



        internal static Drill_Bit DataRowToObject(DataRow row, string FieldPrefix = "")
        {
            Drill_Bit RetVal = new();
            RetVal.Plant = Enum.Parse<MOO.Plant>(row.Field<string>($"{FieldPrefix}plant"));
            RetVal.Drill_Bit_Id = row.Field<int>($"{FieldPrefix}drill_bit_id");
            RetVal.Serial_Number = row.Field<string>($"{FieldPrefix}serial_number");
            RetVal.Manufacturer = row.Field<string>($"{FieldPrefix}manufacturer");
            return RetVal;
        }
    }
}
