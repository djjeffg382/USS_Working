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
    public class PatternSvc
    {

        static PatternSvc()
        {
            Util.RegisterSqlServer();
        }


        public static Pattern Get(int Pattern_Id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE Pattern_Id = {Pattern_Id}");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.USSDrillData);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }


        public static Pattern Get(MOO.Plant Plant, string Pattern_Number)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE p.Plant = @Plant");
            sql.AppendLine($"AND p.Pattern_Number = @Pattern_Number");

            SqlDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.USSDrillData));
            da.SelectCommand.Parameters.AddWithValue("Plant", Plant.ToString());
            da.SelectCommand.Parameters.AddWithValue("Pattern_Number", Pattern_Number);

            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }

        public static List<Pattern> GetAll(MOO.Plant Plant)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE p.Plant = '{Plant}'");


            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.USSDrillData);

            List<Pattern> retVal = new();
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
            sql.AppendLine("p.Pattern_Id, p.Plant, p.Pattern_Number, p.Pit, ");

            sql.AppendLine("ps.pattern_size_id ps_pattern_size_id, ps.description ps_description, ");
            sql.AppendLine("ps.holes_length ps_holes_length, ps.holes_width ps_holes_width, ps.ltons_per_foot ps_ltons_per_foot, ps.plant ps_plant");
            sql.AppendLine("FROM Drill.dbo.Pattern p");
            sql.AppendLine("LEFT OUTER JOIN Drill.dbo.Pattern_Size ps");
            sql.AppendLine("    ON p.Pattern_Size_Id = ps.pattern_size_id");
            return sql.ToString();
        }



        /// <summary>
        /// Inserts the Pattern into the Drill database
        /// </summary>
        /// <param name="Ptrn">Pattern To insert</param>
        /// <returns></returns>
        public static int Insert(Pattern Ptrn)
        {
            SqlConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.USSDrillData));
            conn.Open();
            try
            {
                return Insert(Ptrn, conn);
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
        /// Inserts the Pattern into the Drill Database
        /// </summary>
        /// <param name="Ptrn">Pattern To insert</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Insert(Pattern Ptrn, SqlConnection conn)
        {

            if (Ptrn.Pattern_Id <= 0)
                Ptrn.Pattern_Id = Convert.ToInt32(MOO.Data.GetNextSequence("Drill.dbo.seq_Drill", MOO.Data.MNODatabase.USSDrillData));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO Drill.dbo.Pattern (Pattern_Id, Plant, Pattern_Number, Pit, Pattern_Size_Id)");
            sql.AppendLine("VALUES(@Pattern_Id, @Plant, @Pattern_Number, @Pit, @Pattern_Size_Id)");

            SqlCommand ins = new(sql.ToString(), conn);
            ins.Parameters.AddWithValue("Pattern_Id", Ptrn.Pattern_Id);
            ins.Parameters.AddWithValue("Plant", Ptrn.Plant.ToString());
            ins.Parameters.AddWithValue("Pattern_Number", Ptrn.Pattern_Number);
            ins.Parameters.AddWithValue("Pattern_Size_Id", (Ptrn.Pattern_Size == null) ? DBNull.Value : Ptrn.Pattern_Size.Pattern_Size_Id);
            ins.Parameters.AddWithValue("Pit", Ptrn.Pit.ToString());

            return ins.ExecuteNonQuery();

        }



        /// <summary>
        /// Updates the Pattern in the Drill database
        /// </summary>
        /// <param name="Ptrn">Pattern To update</param>
        /// <returns></returns>
        public static int Update(Pattern Ptrn)
        {
            SqlConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.USSDrillData));
            conn.Open();
            try
            {
                return Update(Ptrn, conn);
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
        /// Update the Pattern in the Drill database
        /// </summary>
        /// <param name="Ptrn">Pattern To update</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Update(Pattern Ptrn, SqlConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE Drill.dbo.Pattern ");
            sql.AppendLine("    SET Plant = @Plant, Pattern_Number = @Pattern_Number, Pit = @Pit,");
            sql.AppendLine("    pattern_size_id = @pattern_size_id");
            sql.AppendLine("WHERE Pattern_Id = @Pattern_Id");

            SqlCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.AddWithValue("Plant", Ptrn.Plant.ToString());
            cmd.Parameters.AddWithValue("Pattern_Number", Ptrn.Pattern_Number);
            cmd.Parameters.AddWithValue("Pit", Ptrn.Pit.ToString());
            cmd.Parameters.AddWithValue("pattern_size_id", (Ptrn.Pattern_Size == null) ? DBNull.Value : Ptrn.Pattern_Size.Pattern_Size_Id);
            cmd.Parameters.AddWithValue("Pattern_Id", Ptrn.Pattern_Id);

            return cmd.ExecuteNonQuery();

        }


        /// <summary>
        /// Deletes the Pattern in the Drill database
        /// </summary>
        /// <param name="Ptrn">Pattern To delete</param>
        /// <returns></returns>
        public static int Delete(Pattern Ptrn)
        {
            SqlConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.USSDrillData));
            conn.Open();
            try
            {
                return Delete(Ptrn, conn);
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
        /// Deletes the Pattern in the Drill database
        /// </summary>
        /// <param name="Ptrn">Pattern To Delete</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Delete(Pattern Ptrn, SqlConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM Drill.dbo.Pattern ");
            sql.AppendLine("WHERE Pattern_Id = @Pattern_Id");

            SqlCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.AddWithValue("Pattern_Id", Ptrn.Pattern_Id);

            return cmd.ExecuteNonQuery();

        }



        /// <summary>
        /// Returns whether the specified point is in the Minntac east pit or the west pit
        /// </summary>
        /// <param name="Northing"></param>
        /// <param name="Easting"></param>
        /// <returns></returns>
        public static Pit GetMTCPit(decimal Northing, decimal Easting)
        {
            //These are points along the road that splits East/West Pit (Northing, Easting)
            //Use this to calculate East Pit vs West Pit
            double[,] divider = new double[,] {
                { int.MinValue,3343 },
                { -4058, 3343 },
                {783, 2964 },
                { 3300,598 },
                { 5700, -240},
                { 9400, 1900},
                { int.MaxValue, 1900}
            };
            //Get the 2 points where northing is greater and less than the northing
            double[] point1 = new double[] { 0, 0 };
            double[] point2 = new double[] { 0, 0 }; 
            for(int i = 0; i < divider.GetLength(0); i++)
            {
                if ((double)Northing >= divider[i,0])
                    point1 = new double[] {divider[i,0],divider[i,1]};
                else
                {
                    point2 = new double[] { divider[i, 0], divider[i, 1] };
                    //once we are greater than the northing, this is point 2 and we are done
                    break;
                }
            }
            //use the 2 points to draw a line, find the easting coordinate on that line that is at the same northing
            if (point1[1] == point2[1])
            {
                //vertical line, no slope
                if ((double)Easting < point1[1])
                    return Pit.MTC_West_Pit;
                else
                    return Pit.MTC_East_Pit;
            }
            else
            {
                double slope = (point2[0] - point1[0]) / (point2[1] - point1[1]);
                //double east = (double)Northing * slope;

                double east = point1[1] + (((double)Northing - point1[0]) / slope) ;
                //vertical line, no slope
                if ((double)Easting < east)
                    return Pit.MTC_West_Pit;
                else
                    return Pit.MTC_East_Pit;
            }            
        }

        internal static Pattern DataRowToObject(DataRow row, string FieldPrefix = "")
        {
            Pattern RetVal = new();
            RetVal.Plant = Enum.Parse<MOO.Plant>(row.Field<string>($"{FieldPrefix}plant"));
            RetVal.Pattern_Id = row.Field<int>($"{FieldPrefix}pattern_id");
            RetVal.Pattern_Number = row.Field<string>($"{FieldPrefix}pattern_number");
            RetVal.Pit = Enum.Parse<Pit>(row.Field<string>($"{FieldPrefix}pit"));
            if (row.IsNull("ps_pattern_size_id"))
                RetVal.Pattern_Size = null;
            else
                RetVal.Pattern_Size = Pattern_SizeSvc.DataRowToObject(row, "ps_");
            return RetVal;
        }
    }
}
