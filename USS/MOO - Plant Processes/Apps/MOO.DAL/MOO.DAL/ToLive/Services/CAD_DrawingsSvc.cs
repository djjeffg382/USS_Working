using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols.WsAddressing;
using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using static MOO.DAL.ToLive.Services.ErrorSvc;

namespace MOO.DAL.ToLive.Services
{

    public class CAD_DrawingsSvc
    {
        static CAD_DrawingsSvc()
        {
            Util.RegisterOracle();
        }

        /// <summary>
        /// returns the specified drawing object by ID
        /// </summary>
        /// <param name="DrawingId"></param>
        /// <returns></returns>
        public static CAD_Drawings Get(decimal DrawingId)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE drawing_id = :drawing_id");


            CAD_Drawings retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("drawing_id", DrawingId);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }



        /// <summary>
        /// Returns a pagedData object containing the data and total row count
        /// </summary>
        /// <param name="Offset"></param>
        /// <param name="TotalItems"></param>
        /// <param name="Plant"></param>
        /// <param name="Area"></param>
        /// <param name="Title"></param>
        /// <param name="DrawingNumber"></param>
        /// <param name="DrawingName"></param>
        /// <param name="PlantLocation"></param>
        /// <param name="PrintLocation"></param>
        /// <param name="ReferenceNumber"></param>
        /// <param name="EquipNumber"></param>
        /// <param name="orderBy"></param>
        /// <param name="orderDirection"></param>
        /// <returns></returns>
        public static PagedData<List<CAD_Drawings>> GetPagedData(int Offset, int TotalItems, MOO.Plant Plant, string Area = "", string Title = "",
                        string DrawingNumber = "", string DrawingName = "", string PlantLocation = "", string PrintLocation = "",
                        string ReferenceNumber = "", string EquipNumber = "",
                        string OrderBy = "drawing_id", string OrderDirection = "ASC")
        {
            PagedData<List<CAD_Drawings>> retObj = new();
            StringBuilder sql = new();

            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            OracleCommand cmd = new("", conn);
            string filter = AddFilters(cmd, Plant, Area, Title, DrawingNumber, DrawingName, PrintLocation, PlantLocation, ReferenceNumber, EquipNumber);
            sql.AppendLine("SELECT * FROM (");
            sql.AppendLine(GetSelect(OrderBy, OrderDirection));
            sql.AppendLine(filter); // will always have a where clause.
            sql.AppendLine(") tbl");
            sql.AppendLine($"WHERE rn BETWEEN {Offset} AND {Offset + TotalItems - 1}");


            cmd.Connection.Open();
            cmd.CommandText = sql.ToString();
            List<CAD_Drawings> elements = new();
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    elements.Add(DataRowToObject(rdr));
                }
            }
            retObj.Data = elements;
            retObj.TotalRows = GetRowCount(Plant, Area, Title, DrawingNumber, DrawingName, PrintLocation, PlantLocation, ReferenceNumber, EquipNumber);

            return retObj;

        }

        private static string AddFilters(OracleCommand Cmd, MOO.Plant Plant, string Area, string Title, string DrawingNumber, string DrawingName, string PrintLocation,
                        string PlantLocation, string ReferenceNumber, string EquipNumber)
        {
            StringBuilder filter = new();

            /*****************Plant Parameter*************/

            filter.AppendLine("WHERE plant = :plant");
            Cmd.Parameters.Add("plant", Plant.ToString());

            /*****************Title Parameter*************/
            if (!string.IsNullOrEmpty(Title))
            {
                filter.AppendLine("AND LOWER(title) LIKE :Title");
                Cmd.Parameters.Add("Title", $"%{Title.ToLower()}%");
            }

            /*****************Area Parameter*************/
            if (!string.IsNullOrEmpty(Area))
            {
                filter.AppendLine("AND LOWER(area) LIKE :Area");
                Cmd.Parameters.Add("Area", $"%{Area.ToLower()}%");
            }

            /*****************DrawingNumber Parameter*************/
            if (!string.IsNullOrEmpty(DrawingNumber))
            {
                filter.AppendLine("AND LOWER(drawing_number) LIKE :DrawingNumber");
                Cmd.Parameters.Add("DrawingNumber", $"%{DrawingNumber.ToLower()}%");
            }
            /*****************Drawing Name Parameter*************/
            if (!string.IsNullOrEmpty(DrawingName))
            {
                filter.AppendLine("AND LOWER(Drawing_Name) LIKE :DrawingName");
                Cmd.Parameters.Add("DrawingName", $"%{DrawingName.ToLower()}%");
            }

            /*****************Print Location Parameter*************/
            if (!string.IsNullOrEmpty(PrintLocation))
            {
                filter.AppendLine("AND LOWER(print_location) LIKE :PrintLocation");
                Cmd.Parameters.Add("PrintLocation", $"%{PrintLocation.ToLower()}%");
            }

            /*****************Plant Location Parameter*************/
            if (!string.IsNullOrEmpty(PlantLocation))
            {
                filter.AppendLine("AND LOWER(plant_location) LIKE :PlantLocation");
                Cmd.Parameters.Add("PlantLocation", $"%{PlantLocation.ToLower()}%");
            }

            /*****************Ref Number Parameter*************/
            if (!string.IsNullOrEmpty(ReferenceNumber))
            {
                filter.AppendLine("AND LOWER(reference_number) LIKE :ReferenceNumber");
                Cmd.Parameters.Add("ReferenceNumber", $"%{ReferenceNumber.ToLower()}%");
            }

            /*****************Equip Number Parameter*************/
            if (!string.IsNullOrEmpty(EquipNumber))
            {
                filter.AppendLine("AND LOWER(equip_number) LIKE :EquipNumber");
                Cmd.Parameters.Add("EquipNumber", $"%{EquipNumber.ToLower()}%");
            }
            return filter.ToString();
        }


        private static int GetRowCount(MOO.Plant Plant, string Area, string Title, string DrawingNumber, string DrawingName, string PrintLocation,
                        string PlantLocation, string ReferenceNumber, string EquipNumber)
        {
            StringBuilder sql = new();
            OracleDataAdapter da = new("", MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));

            string filter = AddFilters(da.SelectCommand, Plant, Area, Title, DrawingNumber, DrawingName, PrintLocation, PlantLocation, ReferenceNumber, EquipNumber);

            sql.AppendLine("SELECT COUNT(*) FROM tolive.cad_drawings");
            sql.AppendLine(filter);
            da.SelectCommand.CommandText = sql.ToString();
            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            }
            else
                return 0;
        }



        public static List<CAD_Drawings> GetAll(
            MOO.Plant plant,
            string title = "",
            string drawingNumber = "",
            string drawingName = "",
            string plantLocation = "",
            string printLocation = "",
            string referenceNumber = "",
            string eqpnumber = "",
            string area = "",
            int MaxRecords = 500)
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT * FROM (");
            sql.AppendLine(GetSelect());
            sql.AppendLine("WHERE PLANT = :plant"); // will always have a where clause.

            if (!string.IsNullOrEmpty(title + drawingNumber + drawingName + plantLocation + printLocation + referenceNumber + eqpnumber + area))
                sql.AppendLine(GetFilters(title, drawingNumber, drawingName,
                            plantLocation, printLocation, referenceNumber, eqpnumber, area));

            sql.AppendLine(")");
            sql.AppendLine($"WHERE rn <= {MaxRecords}");

            List<CAD_Drawings> elements = new();
            using OracleCommand cmd = new(sql.ToString(),
                    new OracleConnection(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART)));
            cmd.BindByName = true;
            cmd.Parameters.Add("plant", plant.ToString());

            if (title != "")
                cmd.Parameters.Add("title", "%" + title.ToLower() + "%");
            if (drawingNumber != "")
                cmd.Parameters.Add("drawing_number", "%" + drawingNumber.ToLower() + "%");
            if (drawingName != "")
                cmd.Parameters.Add("drawing_name", "%" + drawingName.ToLower() + "%");
            if (printLocation != "")
                cmd.Parameters.Add("printLocation", "%" + printLocation.ToLower() + "%");
            if (plantLocation != "")
                cmd.Parameters.Add("plant_location", "%" + plantLocation.ToLower() + "%");
            if (referenceNumber != "")
                cmd.Parameters.Add("reference_number", "%" + referenceNumber.ToLower() + "%");
            if (eqpnumber != "")
                cmd.Parameters.Add("equip_number", "%" + eqpnumber.ToLower() + "%");
            if (area != "")
                cmd.Parameters.Add("area", "%" + area.ToLower() + "%");

            cmd.Connection.Open();
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    elements.Add(DataRowToObject(rdr));
                }
            }
            return elements;
        }

        private static string GetFilters(string title, string drawingNumber, string drawingName,
                                                string plantLocation, string printLocation,
                                                string referenceNumber, string eqpnumber,
                                                string area)
        {
            var filter = new StringBuilder();
            if (title != "")
                filter.AppendLine("AND lower(title) LIKE :title");
            if (drawingNumber != "")
                filter.AppendLine("AND lower(drawing_number) LIKE :drawing_number");
            if (drawingName != "")
                filter.AppendLine("AND lower(drawing_name) LIKE :drawing_name");
            if (printLocation != "")
                filter.AppendLine("AND lower(print_Location) LIKE :printLocation");
            if (plantLocation != "")
                filter.AppendLine("AND lower(plant_location) LIKE :plant_location");
            if (referenceNumber != "")
                filter.AppendLine("AND lower(reference_number) LIKE :reference_number");
            if (eqpnumber != "")
                filter.AppendLine("AND lower(equip_number) LIKE :equip_number");
            if (area != "")
                filter.AppendLine("AND lower(area) LIKE :area");

            return filter.ToString();
        }

        internal static string GetColumns(string TableAlias = "", string ColPrefix = "")
        {
            string ta = "";
            if (!string.IsNullOrEmpty(TableAlias))
                ta = TableAlias + ".";
            StringBuilder cols = new();
            cols.AppendLine($"{ta}drawing_id {ColPrefix}drawing_id, {ta}plant {ColPrefix}plant, {ta}area {ColPrefix}area, ");
            cols.AppendLine($"{ta}title {ColPrefix}title, {ta}file_location {ColPrefix}file_location, ");
            cols.AppendLine($"{ta}drawing_number {ColPrefix}drawing_number, {ta}drawing_name {ColPrefix}drawing_name, ");
            cols.AppendLine($"{ta}drawing_date {ColPrefix}drawing_date, {ta}plant_location {ColPrefix}plant_location, ");
            cols.AppendLine($"{ta}print_location {ColPrefix}print_location, {ta}reference_number {ColPrefix}reference_number, ");
            cols.AppendLine($"{ta}equip_number {ColPrefix}equip_number, {ta}comments {ColPrefix}comments,");
            cols.AppendLine($"{ta}full_path {ColPrefix}full_path, {ta}last_modified {ColPrefix}last_modified,");
            cols.AppendLine($"{ta}last_modified_by {ColPrefix}last_modified_by");
            return cols.ToString();
        }
        private static string GetSelect(string OrderByColumn = "drawing_id", string OrderByDirection = "desc")
        {
            string orderBy = (string.IsNullOrEmpty(OrderByColumn) ? "drawing_id" : OrderByColumn) + " " +
                                (string.IsNullOrEmpty(OrderByDirection) ? "asc" : OrderByDirection);
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine($",row_number() over (order by {orderBy}) rn");
            sql.AppendLine("FROM tolive.cad_drawings");
            return sql.ToString();
        }


        public static int Insert(CAD_Drawings obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(CAD_Drawings obj, OracleConnection conn)
        {
            if (obj.Drawing_Id <= 0)
                obj.Drawing_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.seq_cad_drawings"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.cad_drawings(");
            sql.AppendLine("drawing_id, plant, area, title, drawing_number, drawing_name, ");
            sql.AppendLine("drawing_date, plant_location, print_location, reference_number, equip_number, ");
            sql.AppendLine("comments, full_path, last_modified, last_modified_by)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":drawing_id, :plant, :area, :title, :drawing_number, :drawing_name, ");
            sql.AppendLine(":drawing_date, :plant_location, :print_location, :reference_number, :equip_number, ");
            sql.AppendLine(":comments, :full_path, :last_modified, :last_modified_by)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("drawing_id", obj.Drawing_Id);
            ins.Parameters.Add("plant", obj.Plant == Plant.Minntac ? "Minntac" : "Keetac");
            ins.Parameters.Add("area", obj.Area);
            ins.Parameters.Add("title", obj.Title);
            ins.Parameters.Add("drawing_number", obj.Drawing_Number);
            ins.Parameters.Add("drawing_name", obj.Drawing_Name);
            ins.Parameters.Add("drawing_date", obj.Drawing_Date);
            ins.Parameters.Add("plant_location", obj.Plant_Location);
            ins.Parameters.Add("print_location", obj.Print_Location);
            ins.Parameters.Add("reference_number", obj.Reference_Number);
            ins.Parameters.Add("equip_number", obj.Equip_Number);
            ins.Parameters.Add("comments", obj.Comments);
            ins.Parameters.Add("full_path", obj.Full_Path);
            ins.Parameters.Add("last_modified", obj.Last_Modified);
            ins.Parameters.Add("last_modified_by", obj.Last_Modified_By);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(CAD_Drawings obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(CAD_Drawings obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.cad_drawings SET");
            sql.AppendLine("plant = :plant, ");
            sql.AppendLine("area = :area, ");
            sql.AppendLine("title = :title, ");
            sql.AppendLine("drawing_number = :drawing_number, ");
            sql.AppendLine("drawing_name = :drawing_name, ");
            sql.AppendLine("drawing_date = :drawing_date, ");
            sql.AppendLine("plant_location = :plant_location, ");
            sql.AppendLine("print_location = :print_location, ");
            sql.AppendLine("reference_number = :reference_number, ");
            sql.AppendLine("equip_number = :equip_number, ");
            sql.AppendLine("comments = :comments,");
            sql.AppendLine("full_path = :full_path, ");
            sql.AppendLine("last_modified = :last_modified, ");
            sql.AppendLine("last_modified_by = :last_modified_by ");
            sql.AppendLine("WHERE drawing_id = :drawing_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("plant", obj.Plant == Plant.Minntac ? "Minntac" : "Keetac");
            upd.Parameters.Add("area", obj.Area);
            upd.Parameters.Add("title", obj.Title);
            upd.Parameters.Add("drawing_number", obj.Drawing_Number);
            upd.Parameters.Add("drawing_name", obj.Drawing_Name);
            upd.Parameters.Add("drawing_date", obj.Drawing_Date);
            upd.Parameters.Add("plant_location", obj.Plant_Location);
            upd.Parameters.Add("print_location", obj.Print_Location);
            upd.Parameters.Add("reference_number", obj.Reference_Number);
            upd.Parameters.Add("equip_number", obj.Equip_Number);
            upd.Parameters.Add("comments", obj.Comments);
            upd.Parameters.Add("full_path", obj.Full_Path);
            upd.Parameters.Add("Last_Modified", obj.Last_Modified);
            upd.Parameters.Add("Last_Modified_By", obj.Last_Modified_By);
            upd.Parameters.Add("drawing_id", obj.Drawing_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }

        /// <summary>
        /// Gets a list of Distinct areas used for the cad drawings
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetDistinctAreas()
        {
            return GetDistinctColumn("area");
        }

        /// <summary>
        /// Gets a list of Distinct plant locations used for the cad drawings
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetDistinctPlantLocations()
        {
            return GetDistinctColumn("plant_location");
        }

        /// <summary>
        /// Gets a list of Distinct areas used for the cad drawings
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetDistinctPrintLocations()
        {
            return GetDistinctColumn("print_location");
        }
        /// <summary>
        /// Gets a list of Distinct areas used for the cad drawings
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetDistinctEquipNumbers()
        {
            return GetDistinctColumn("equip_number");
        }

        /// <summary>
        /// Gets a list of Distinct {ColumnName} used for the cad drawings
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<string> GetDistinctColumn(string ColumnName)
        {
            List<string> retVal = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new($"SELECT distinct {ColumnName} FROM tolive.cad_drawings WHERE {ColumnName} IS NOT NULL Order by 1", conn);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    retVal.Add(rdr.GetFieldValue<string>(0));
                }
            }
            conn.Close();
            return retVal;
        }

        private static CAD_Drawings DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            CAD_Drawings RetVal = new();
            RetVal.Drawing_Id = (decimal)Util.GetRowVal(row, $"{ColPrefix}drawing_id");
            RetVal.Plant = (string)Util.GetRowVal(row, $"{ColPrefix}plant") == "Minntac" ? MOO.Plant.Minntac : MOO.Plant.Keetac;
            RetVal.Area = (string)Util.GetRowVal(row, $"{ColPrefix}area");
            RetVal.Title = (string)Util.GetRowVal(row, $"{ColPrefix}title");
            RetVal.Drawing_Number = (string)Util.GetRowVal(row, $"{ColPrefix}drawing_number");
            RetVal.Drawing_Name = (string)Util.GetRowVal(row, $"{ColPrefix}drawing_name");
            RetVal.Drawing_Date = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}drawing_date");
            RetVal.Plant_Location = (string)Util.GetRowVal(row, $"{ColPrefix}plant_location");
            RetVal.Print_Location = (string)Util.GetRowVal(row, $"{ColPrefix}print_location");
            RetVal.Reference_Number = (string)Util.GetRowVal(row, $"{ColPrefix}reference_number");
            RetVal.Equip_Number = (string)Util.GetRowVal(row, $"{ColPrefix}equip_number");
            RetVal.Comments = (string)Util.GetRowVal(row, $"{ColPrefix}comments");
            RetVal._File_Location = (string)Util.GetRowVal(row, $"{ColPrefix}File_Location");
            RetVal.Full_Path = (string)Util.GetRowVal(row, $"{ColPrefix}Full_Path");
            RetVal._Last_Modified = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}Last_Modified");
            RetVal.Last_Modified_By = (string)Util.GetRowVal(row, $"{ColPrefix}Last_Modified_By");
            return RetVal;
        }
    }
}
