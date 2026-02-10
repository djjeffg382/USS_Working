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
    public class OperatorSvc
    {

        static OperatorSvc()
        {
            Util.RegisterSqlServer();
        }


        public static Operator Get(int Operator_Id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE Operator_Id = {Operator_Id}");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.USSDrillData);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }


        public static Operator Get(MOO.Plant Plant, string Employee_Number)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE Plant = @Plant");
            sql.AppendLine($"AND Employee_Number = @Employee_Number");

            SqlDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.USSDrillData));
            da.SelectCommand.Parameters.AddWithValue("Plant", Plant.ToString());
            da.SelectCommand.Parameters.AddWithValue("Employee_Number", Employee_Number);

            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }

        public static List<Operator> GetAll(MOO.Plant Plant, bool ShowInactive = true)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE Plant = '{Plant}'");
            if (!ShowInactive)
                sql.AppendLine("AND active = 1");
            sql.AppendLine("ORDER BY last_name");


            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.USSDrillData);

            List<Operator> retVal = new();
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
            sql.AppendLine("Operator_Id, Plant, Employee_Number, First_Name, Last_Name, Active ");
            sql.AppendLine("FROM Drill.dbo.Operator");
            return sql.ToString();
        }



        /// <summary>
        /// Inserts the Operator into the Drill database
        /// </summary>
        /// <param name="Op">Equip To insert</param>
        /// <returns></returns>
        public static int Insert(Operator Op)
        {
            SqlConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.USSDrillData));
            conn.Open();
            try
            {
                return Insert(Op, conn);
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
        /// Inserts the Operator into the Drill Database
        /// </summary>
        /// <param name="Op">Equip To insert</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Insert(Operator Op, SqlConnection conn)
        {

            if (Op.Operator_Id <= 0)
                Op.Operator_Id = Convert.ToInt32(MOO.Data.GetNextSequence("Drill.dbo.seq_Drill", MOO.Data.MNODatabase.USSDrillData));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO Drill.dbo.Operator (Operator_id, Plant, Employee_Number, First_Name, Last_name, Active)");
            sql.AppendLine("VALUES(@Operator_id, @Plant, @Employee_Number, @First_Name, @Last_name, @Active)");

            SqlCommand ins = new(sql.ToString(), conn);
            ins.Parameters.AddWithValue("Operator_id", Op.Operator_Id);
            ins.Parameters.AddWithValue("Plant", Op.Plant.ToString());
            ins.Parameters.AddWithValue("Employee_Number", Op.Employee_Number);
            ins.Parameters.AddWithValue("First_Name", Op.First_Name);
            ins.Parameters.AddWithValue("Last_name", Op.Last_Name);
            ins.Parameters.AddWithValue("Active", Op.Active);

            return ins.ExecuteNonQuery();

        }



        /// <summary>
        /// Updates the Operator in the Drill database
        /// </summary>
        /// <param name="Op">Equip To update</param>
        /// <returns></returns>
        public static int Update(Operator Op)
        {
            SqlConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.USSDrillData));
            conn.Open();
            try
            {
                return Update(Op, conn);
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
        /// Updates the Operator in the Drill database
        /// </summary>
        /// <param name="Op">Equip To update</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Update(Operator Op, SqlConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE Drill.dbo.Operator ");
            sql.AppendLine("    SET Plant = @Plant, Employee_Number = @Employee_Number, First_Name = @First_Name, Last_Name = @Last_Name, Active = @Active");
            sql.AppendLine("WHERE Operator_Id = @Operator_Id");

            SqlCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.AddWithValue("Plant", Op.Plant.ToString());
            cmd.Parameters.AddWithValue("Employee_Number", Op.Employee_Number);
            cmd.Parameters.AddWithValue("First_Name", Op.First_Name);
            cmd.Parameters.AddWithValue("Last_Name", Op.Last_Name);
            cmd.Parameters.AddWithValue("Active", Op.Active);
            cmd.Parameters.AddWithValue("Operator_Id", Op.Operator_Id);

            return cmd.ExecuteNonQuery();

        }


        /// <summary>
        /// Deletes the Operator in the Drill database
        /// </summary>
        /// <param name="Op">Equip To delete</param>
        /// <returns></returns>
        public static int Delete(Operator Op)
        {
            SqlConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.USSDrillData));
            conn.Open();
            try
            {
                return Delete(Op, conn);
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
        /// Deletes the Operator in the Drill database
        /// </summary>
        /// <param name="Op">Equip To Delete</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Delete(Operator Op, SqlConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM Drill.dbo.Operator ");
            sql.AppendLine("WHERE Operator_Id = @Operator_Id");

            SqlCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.AddWithValue("Operator_Id", Op.Operator_Id);

            return cmd.ExecuteNonQuery();

        }


        internal static Operator DataRowToObject(DataRow row, string FieldPrefix = "")
        {
            Operator RetVal = new();
            RetVal.Plant = Enum.Parse<MOO.Plant>(row.Field<string>($"{FieldPrefix}plant"));
            RetVal.Operator_Id = row.Field<int>($"{FieldPrefix}operator_id");
            RetVal.Employee_Number = row.Field<string>($"{FieldPrefix}employee_number");
            RetVal.First_Name = row.Field<string>($"{FieldPrefix}first_name");
            RetVal.Last_Name = row.Field<string>($"{FieldPrefix}last_name");
            RetVal.Active = row.Field<bool>($"{FieldPrefix}active");
            return RetVal;
        }
    }
}
