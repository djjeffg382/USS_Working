using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;

namespace MOO.DAL.ToLive.Services
{
    public class Rental_EquipSvc
    {
        static Rental_EquipSvc()
        {
            Util.RegisterOracle();
        }


        public static Rental_Equip Get(decimal rental_equip_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE rental_equip_id = :rental_equip_id");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("rental_equip_id", rental_equip_id);

            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }


        public static List<Rental_Equip> GetAll(bool onlyActive = true, MOO.Plant? plant = null)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            if (onlyActive)
                sql.AppendLine($"WHERE OUTDATE IS NULL");
            if (plant != null && onlyActive)
                sql.AppendLine($"AND Plantloc = '{plant}'");
            else if(plant != null && onlyActive == false)
                sql.AppendLine($"WHERE Plantloc = '{plant}'");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));

            DataSet ds = MOO.Data.ExecuteQuery(da);
            List<Rental_Equip> elements = new();
            if (ds.Tables[0].Rows.Count >= 1)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    elements.Add(DataRowToObject(dr));
                }
            }
            return elements;
        }


        private static string GetSelect(string addField = "")
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT re.rental_equip_id, re.plantloc, re.manager, re.vendor, re.vehicletype, re.location,");
            sql.AppendLine("    re.locationdesc, re.entrydate, re.outdate, re.inspectiondate, re.inspectedby, re.po_number, ");
            sql.AppendLine("    re.soifilename, re.comments, re.phonenumber,");
            sql.AppendLine(PeopleSvc.GetColumns("m", "m_"));
            sql.AppendLine(",");
            sql.AppendLine(PeopleSvc.GetColumns("p", "p_"));
            if (!string.IsNullOrEmpty(addField))
            {
                sql.Append(", ");
                sql.AppendLine(addField);
            }
            sql.AppendLine("FROM tolive.rental_equip re");
            sql.AppendLine("INNER JOIN tolive.people m");
            sql.AppendLine("ON m.person_id = re.manager");
            sql.AppendLine("LEFT JOIN tolive.people p");
            sql.AppendLine("ON p.person_id = re.Inspectedby");
            return sql.ToString();
        }


        public static int Insert(Rental_Equip obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Rental_Equip obj, OracleConnection conn)
        {
            if (obj.Rental_Equip_Id <= 0)
                obj.Rental_Equip_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.seq_firesec"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.rental_equip(");
            sql.AppendLine("rental_equip_id, plantloc, manager, vendor, vehicletype, location, locationdesc, ");
            sql.AppendLine("entrydate, outdate, inspectiondate, inspectedby, po_number, soifilename, ");
            sql.AppendLine("comments, phonenumber)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":rental_equip_id, :plantloc, :manager, :vendor, :vehicletype, :location, ");
            sql.AppendLine(":locationdesc, :entrydate, :outdate, :inspectiondate, :inspectedby, :po_number, ");
            sql.AppendLine(":soifilename, :comments, :phonenumber)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.Parameters.Add("rental_equip_id", obj.Rental_Equip_Id);
            ins.Parameters.Add("plantloc", obj.Plantloc.ToString());
            ins.Parameters.Add("manager", obj.Manager.Person_Id);
            ins.Parameters.Add("vendor", obj.Vendor);
            ins.Parameters.Add("vehicletype", obj.Vehicletype);
            ins.Parameters.Add("location", obj.Location.ToString());
            ins.Parameters.Add("locationdesc", obj.Locationdesc);
            ins.Parameters.Add("entrydate", obj.Entrydate);
            ins.Parameters.Add("outdate", obj.Outdate);
            ins.Parameters.Add("inspectiondate", obj.Inspectiondate);
            ins.Parameters.Add("inspectedby", obj.Inspectedby?.Person_Id);
            ins.Parameters.Add("po_number", obj.Po_Number);
            ins.Parameters.Add("soifilename", obj.Soifilename);
            ins.Parameters.Add("comments", obj.Comments);
            ins.Parameters.Add("phonenumber", obj.Phonenumber);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Rental_Equip obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Rental_Equip obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.rental_equip SET");
            sql.AppendLine("plantloc = :plantloc, ");
            sql.AppendLine("manager = :manager, ");
            sql.AppendLine("vendor = :vendor, ");
            sql.AppendLine("vehicletype = :vehicletype, ");
            sql.AppendLine("location = :location, ");
            sql.AppendLine("locationdesc = :locationdesc, ");
            sql.AppendLine("entrydate = :entrydate, ");
            sql.AppendLine("outdate = :outdate, ");
            sql.AppendLine("inspectiondate = :inspectiondate, ");
            sql.AppendLine("inspectedby = :inspectedby, ");
            sql.AppendLine("po_number = :po_number, ");
            sql.AppendLine("soifilename = :soifilename, ");
            sql.AppendLine("comments = :comments, ");
            sql.AppendLine("phonenumber = :phonenumber");
            sql.AppendLine("WHERE rental_equip_id = :rental_equip_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.Parameters.Add("plantloc", obj.Plantloc.ToString());
            upd.Parameters.Add("manager", obj.Manager.Person_Id);
            upd.Parameters.Add("vendor", obj.Vendor);
            upd.Parameters.Add("vehicletype", obj.Vehicletype);
            upd.Parameters.Add("location", obj.Location.ToString());
            upd.Parameters.Add("locationdesc", obj.Locationdesc);
            upd.Parameters.Add("entrydate", obj.Entrydate);
            upd.Parameters.Add("outdate", obj.Outdate);
            upd.Parameters.Add("inspectiondate", obj.Inspectiondate);
            upd.Parameters.Add("inspectedby", obj.Inspectedby?.Person_Id);
            upd.Parameters.Add("po_number", obj.Po_Number);
            upd.Parameters.Add("soifilename", obj.Soifilename);
            upd.Parameters.Add("comments", obj.Comments);
            upd.Parameters.Add("phonenumber", obj.Phonenumber);
            upd.Parameters.Add("rental_equip_id", obj.Rental_Equip_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(Rental_Equip obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(Rental_Equip obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.rental_equip");
            sql.AppendLine("WHERE rental_equip_id = :rental_equip_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.Parameters.Add("rental_equip_id", obj.Rental_Equip_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        private static Rental_Equip DataRowToObject(DataRow row)
        {
            Rental_Equip RetVal = new();
            RetVal.Rental_Equip_Id = row.Field<decimal>("rental_equip_id");
            RetVal.Plantloc = Enum.Parse<MOO.Plant>(row.Field<string>("PlantLoc"));
            RetVal.Manager = PeopleSvc.DataRowToObject(row, "m_");
            RetVal.Vendor = row.Field<string>("vendor");
            RetVal.Vehicletype = row.Field<string>("vehicletype");
            RetVal.Location = row.Field<string>("Location");
            RetVal.Locationdesc = row.Field<string>("locationdesc");
            RetVal.Entrydate = row.Field<DateTime>("entrydate");
            RetVal.Outdate = row.Field<DateTime?>("outdate");
            RetVal.Inspectiondate = row.Field<DateTime?>("inspectiondate");
            if (row.Field<decimal?>("Inspectedby") != null)
                RetVal.Inspectedby = PeopleSvc.DataRowToObject(row, "p_");
            else
                RetVal.Inspectedby = null;
            
            RetVal.Po_Number = row.Field<string>("po_number");
            RetVal.Soifilename = row.Field<string>("soifilename");
            RetVal.Comments = row.Field<string>("comments");
            RetVal.Phonenumber = row.Field<decimal?>("phonenumber");
            return RetVal;
        }

    }
}
