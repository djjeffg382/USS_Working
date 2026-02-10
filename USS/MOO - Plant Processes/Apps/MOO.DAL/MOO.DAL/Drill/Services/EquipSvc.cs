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
    public class EquipSvc
    {

        static EquipSvc()
        {
            Util.RegisterSqlServer();
        }


        public static Equip Get(int Equip_Id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE Equip_Id = {Equip_Id}");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.USSDrillData);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }


        public static Equip Get(MOO.Plant Plant, string Equip_Number)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE Plant = @Plant");
            sql.AppendLine($"AND Equip_Number = @Equip_Number");

            SqlDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.USSDrillData));
            da.SelectCommand.Parameters.AddWithValue("Plant", Plant.ToString());
            da.SelectCommand.Parameters.AddWithValue("Equip_Number", Equip_Number);

            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }

        /// <summary>
        /// Gets the equipment based on the reference id from the foreign application
        /// </summary>
        /// <param name="Plant"></param>
        /// <param name="Ref_Id">The string relating to the foreign application equipment number</param>
        /// <returns></returns>
        public static Equip GetByReferenceId(MOO.Plant Plant, string Ref_Id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE Plant = @Plant");
            sql.AppendLine($"AND Reference_Id = @Reference_Id");

            SqlDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.USSDrillData));
            da.SelectCommand.Parameters.AddWithValue("Plant", Plant.ToString());
            da.SelectCommand.Parameters.AddWithValue("Reference_Id", Ref_Id);

            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }


        public static List<Equip> GetAll(MOO.Plant Plant, bool ShowInactive = true)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE Plant = '{Plant}'");
            if (!ShowInactive)
                sql.AppendLine("AND active = 1");
            sql.AppendLine("ORDER BY Equip_Number");


            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.USSDrillData);

            List<Equip> retVal = new();
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
            sql.AppendLine("Equip_Id, Plant, Equip_Number, Drill_System, Active, Reference_Id ");
            sql.AppendLine("FROM Drill.dbo.Equip");
            return sql.ToString();
        }



        /// <summary>
        /// Inserts the Equipment into the Drill database
        /// </summary>
        /// <param name="Eq">Equip To insert</param>
        /// <returns></returns>
        public static int Insert(Equip Eq)
        {
            SqlConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.USSDrillData));
            conn.Open();
            try
            {
                return Insert(Eq, conn);
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
        /// Inserts the Equipment into the Drill Database
        /// </summary>
        /// <param name="Eq">Equip To insert</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Insert(Equip Eq, SqlConnection conn)
        {

            if (Eq.Equip_Id <= 0)
                Eq.Equip_Id = Convert.ToInt32(MOO.Data.GetNextSequence("Drill.dbo.seq_Drill", MOO.Data.MNODatabase.USSDrillData));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO Drill.dbo.Equip (Equip_Id, Plant, Equip_Number, Drill_System, Active, Reference_Id)");
            sql.AppendLine("VALUES(@Equip_Id, @Plant, @Equip_Number, @Drill_System, @Active, @Reference_Id)");

            SqlCommand ins = new(sql.ToString(), conn);
            ins.Parameters.AddWithValue("Equip_Id", Eq.Equip_Id);
            ins.Parameters.AddWithValue("Plant", Eq.Plant.ToString());
            ins.Parameters.AddWithValue("Equip_Number", Eq.Equip_Number);
            ins.Parameters.AddWithValue("Drill_System", Eq.Drill_System.ToString());
            ins.Parameters.AddWithValue("Active", Eq.Active);
            ins.Parameters.AddWithValue("Reference_Id", Eq.Reference_Id);

            return ins.ExecuteNonQuery();

        }



        /// <summary>
        /// Updates the Equipment in the Drill database
        /// </summary>
        /// <param name="Eq">Equip To update</param>
        /// <returns></returns>
        public static int Update(Equip Eq)
        {
            SqlConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.USSDrillData));
            conn.Open();
            try
            {
                return Update(Eq, conn);
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
        /// Updates the Equipment in the Drill database
        /// </summary>
        /// <param name="Eq">Equip To update</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Update(Equip Eq, SqlConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE Drill.dbo.Equip ");
            sql.AppendLine("    SET Plant = @Plant, Equip_Number = @Equip_Number, Drill_System = @Drill_System, Active = @Active,");
            sql.AppendLine("    Reference_Id = @Reference_Id");
            sql.AppendLine("WHERE Equip_Id = @Equip_Id");

            SqlCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.AddWithValue("Plant", Eq.Plant.ToString());
            cmd.Parameters.AddWithValue("Equip_Number", Eq.Equip_Number);
            cmd.Parameters.AddWithValue("Drill_System", Eq.Drill_System.ToString());
            cmd.Parameters.AddWithValue("Active", Eq.Active);
            cmd.Parameters.AddWithValue("Reference_Id", Eq.Reference_Id);
            cmd.Parameters.AddWithValue("Equip_Id", Eq.Equip_Id);

            return cmd.ExecuteNonQuery();

        }


        /// <summary>
        /// Deletes the Equipment in the Drill database
        /// </summary>
        /// <param name="Eq">Equip To delete</param>
        /// <returns></returns>
        public static int Delete(Equip Eq)
        {
            SqlConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.USSDrillData));
            conn.Open();
            try
            {
                return Delete(Eq, conn);
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
        /// Deletes the Equipment in the Drill database
        /// </summary>
        /// <param name="Eq">Equip To Delete</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Delete(Equip Eq, SqlConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM Drill.dbo.Equip ");
            sql.AppendLine("WHERE Equip_Id = @Equip_Id");

            SqlCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.AddWithValue("Equip_Id", Eq.Equip_Id);

            return cmd.ExecuteNonQuery();

        }


        internal static Equip DataRowToObject(DataRow row, string FieldPrefix = "")
        {
            Equip RetVal = new();
            RetVal.Plant = Enum.Parse<MOO.Plant>( row.Field<string>($"{FieldPrefix}plant"));
            RetVal.Equip_Id = row.Field<int>($"{FieldPrefix}equip_id") ;
            RetVal.Equip_Number = row.Field<string>($"{FieldPrefix}equip_number");
            RetVal.Drill_System = Enum.Parse<DrillSystem>(row.Field<string>($"{FieldPrefix}Drill_System"));
            RetVal.Active = row.Field<bool>($"{FieldPrefix}active");
            RetVal.Reference_Id = row.Field<string>($"{FieldPrefix}reference_id");
            return RetVal;
        }
    }
}
