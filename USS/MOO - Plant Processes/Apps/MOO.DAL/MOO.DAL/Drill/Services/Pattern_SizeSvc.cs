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
    public class Pattern_SizeSvc
    {
        static Pattern_SizeSvc()
        {
            Util.RegisterSqlServer();
        }


        public static Pattern_Size Get(int Pattern_Size_Id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE Pattern_Size_Id = {Pattern_Size_Id}");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.USSDrillData);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }


        public static List<Pattern_Size> GetAll(MOO.Plant Plant)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE Plant = '{Plant}'");
            sql.AppendLine("ORDER BY description");


            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.USSDrillData);

            List<Pattern_Size> retVal = new();
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
            sql.AppendLine("pattern_size_id, description, holes_length, holes_width, ltons_per_foot, plant ");
            sql.AppendLine("FROM Drill.dbo.pattern_size");
            return sql.ToString();
        }



        /// <summary>
        /// Inserts the Pattern_Size into the Drill database
        /// </summary>
        /// <param name="ps">Pattern_Size To insert</param>
        /// <returns></returns>
        public static int Insert(Pattern_Size ps)
        {
            SqlConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.USSDrillData));
            conn.Open();
            try
            {
                return Insert(ps, conn);
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
        /// Inserts the Pattern_Size into the Drill Database
        /// </summary>
        /// <param name="ps">Pattern_Size To insert</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Insert(Pattern_Size ps, SqlConnection conn)
        {
            
            if (ps.Pattern_Size_Id <= 0)
                ps.Pattern_Size_Id = Convert.ToInt32(MOO.Data.GetNextSequence("Drill.dbo.seq_Drill", MOO.Data.MNODatabase.USSDrillData));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO Drill.dbo.Pattern_Size (Pattern_Size_Id, Plant, Description,");
            sql.AppendLine("    holes_length, holes_width, ltons_per_foot)");
            sql.AppendLine("VALUES(@Pattern_Size_Id, @Plant, @Description, ");
            sql.AppendLine("    @holes_length, @holes_width, @ltons_per_foot)");

            SqlCommand ins = new(sql.ToString(), conn);
            ins.Parameters.AddWithValue("Pattern_Size_Id", ps.Pattern_Size_Id);
            ins.Parameters.AddWithValue("Plant", ps.Plant.ToString());
            ins.Parameters.AddWithValue("Description", ps.Description);
            ins.Parameters.AddWithValue("holes_length", ps.Holes_Length);
            ins.Parameters.AddWithValue("holes_width", ps.Holes_Width);
            ins.Parameters.AddWithValue("ltons_per_foot", ps.LTons_Per_Foot);

            return ins.ExecuteNonQuery();

        }



        /// <summary>
        /// Updates the Pattern_Size in the Drill database
        /// </summary>
        /// <param name="db">Pattern_Size To update</param>
        /// <returns></returns>
        public static int Update(Pattern_Size ps)
        {
            SqlConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.USSDrillData));
            conn.Open();
            try
            {
                return Update(ps, conn);
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
        /// <param name="ps">Drill Bit To update</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Update(Pattern_Size ps, SqlConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE Drill.dbo.pattern_size ");
            sql.AppendLine("    SET Plant = @Plant, Description = @Description, holes_length = @holes_length,");
            sql.AppendLine("    holes_width = @holes_width, ltons_per_foot = @ltons_per_foot");
            sql.AppendLine("WHERE pattern_size_id = @pattern_size_id");

            SqlCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.AddWithValue("Plant", ps.Plant.ToString());
            cmd.Parameters.AddWithValue("Description", ps.Description);
            cmd.Parameters.AddWithValue("holes_length", ps.Holes_Length);
            cmd.Parameters.AddWithValue("holes_width", ps.Holes_Width);
            cmd.Parameters.AddWithValue("ltons_per_foot", ps.LTons_Per_Foot);
            cmd.Parameters.AddWithValue("pattern_size_id", ps.Pattern_Size_Id);

            return cmd.ExecuteNonQuery();

        }


        /// <summary>
        /// Deletes the Pattern_Size in the Drill database
        /// </summary>
        /// <param name="ps">Pattern_Size To delete</param>
        /// <returns></returns>
        public static int Delete(Pattern_Size ps)
        {
            SqlConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.USSDrillData));
            conn.Open();
            try
            {
                return Delete(ps, conn);
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
        /// Deletes the Pattern_Size in the Drill database
        /// </summary>
        /// <param name="ps">Pattern_Size To Delete</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Delete(Pattern_Size ps, SqlConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM Drill.dbo.pattern_size ");
            sql.AppendLine("WHERE pattern_size_id = @pattern_size_id");

            SqlCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.AddWithValue("pattern_size_id",ps.Pattern_Size_Id);

            return cmd.ExecuteNonQuery();

        }



        internal static Pattern_Size DataRowToObject(DataRow row, string FieldPrefix = "")
        {
            Pattern_Size RetVal = new();
            RetVal.Plant = Enum.Parse<MOO.Plant>(row.Field<string>($"{FieldPrefix}plant"));
            RetVal.Pattern_Size_Id = row.Field<int>($"{FieldPrefix}pattern_size_id");
            RetVal.Description = row.Field<string>($"{FieldPrefix}description");
            RetVal.Holes_Length = row.Field<int>($"{FieldPrefix}Holes_Length");
            RetVal.Holes_Width = row.Field<int>($"{FieldPrefix}Holes_Width");
            RetVal.LTons_Per_Foot = row.Field<decimal>($"{FieldPrefix}LTons_Per_Foot");
            return RetVal;
        }
    }
}
