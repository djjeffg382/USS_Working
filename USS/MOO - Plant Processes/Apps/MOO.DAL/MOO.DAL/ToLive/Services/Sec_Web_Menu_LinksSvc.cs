using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Services
{
    public static class Sec_Web_Menu_LinksSvc
    {
        static Sec_Web_Menu_LinksSvc()
        {
            Util.RegisterOracle();
        }


        public static Sec_Web_Menu_Links Get(int Wml_Id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE wml_id = :wml_id");


            Sec_Web_Menu_Links retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("wml_id", Wml_Id);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }


        public static List<Sec_Web_Menu_Links> GetByWebMenuId(int Web_Menu_Id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE Web_Menu_Id = :Web_Menu_Id");
            sql.AppendLine("ORDER BY sort_order");  //this should put the links in the order they are supposed to show up as

            List<Sec_Web_Menu_Links> elements = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("Web_Menu_Id", Web_Menu_Id);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    elements.Add(DataRowToObject(rdr));
                }
            }
            conn.Close();

            //loop through the elements list and rearrange them to parent->child relation
            for(int nIdx = elements.Count - 1;nIdx>=0; nIdx--)
            {
                var lnk = elements[nIdx];
                if (lnk.Parent_Id.HasValue)
                {
                    //this link has a parent
                    var parent = getParent(lnk.Parent_Id.Value, elements);

                    if (parent != null) {  
                        parent.Children.Add(lnk);
                    }
                    elements.RemoveAt(nIdx);
                }
            }

            return elements;
        }


        internal static Sec_Web_Menu_Links? getParent(int? id, List<Sec_Web_Menu_Links> searchList)
        {
            foreach (var link in searchList)
            {
                if (link.Wml_Id == id)
                {
                    return link;
                }
                if (link.Children.Count > 0)
                {
                    var ret = getParent(id, link.Children);
                    if (ret != null)
                    {
                        return ret;
                    }
                }
            }
            return null;
        }


        internal static string GetColumns(string TableAlias = "", string ColPrefix = "")
        {
            string ta = "";
            if (!string.IsNullOrEmpty(TableAlias))
                ta = TableAlias + ".";
            StringBuilder cols = new();
            cols.AppendLine($"{ta}wml_id {ColPrefix}wml_id, {ta}web_menu_id {ColPrefix}web_menu_id, ");
            cols.AppendLine($"{ta}parent_id {ColPrefix}parent_id, {ta}display_text {ColPrefix}display_text, ");
            cols.AppendLine($"{ta}sort_order {ColPrefix}sort_order, {ta}url {ColPrefix}url, ");
            cols.AppendLine($"{ta}open_new_window {ColPrefix}open_new_window, {ta}roles {ColPrefix}roles, ");
            cols.AppendLine($"{ta}no_access_show {ColPrefix}no_access_show, {ta}tooltip {ColPrefix}tooltip, ");
            cols.AppendLine($"{ta}is_iis_id_app {ColPrefix}is_iis_id_app, {ta}active {ColPrefix}active, ");
            cols.AppendLine($"{ta}sy_program_code {ColPrefix}sy_program_code, {ta}image_url {ColPrefix}image_url, ");
            cols.AppendLine($"{ta}recurse_folder {ColPrefix}recurse_folder, {ta}menu_type {ColPrefix}menu_type, ");
            cols.AppendLine($"{ta}modified_by {ColPrefix}modified_by, ");
            cols.AppendLine($"{ta}include_in_global_menu {ColPrefix}include_in_global_menu");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.sec_web_menu_links");
            return sql.ToString();
        }


        public static int Insert(Sec_Web_Menu_Links obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Sec_Web_Menu_Links obj, OracleConnection conn)
        {
            if (obj.Wml_Id <= 0)
                obj.Wml_Id = Convert.ToInt32(MOO.Data.GetNextSequence("TOLIVE.Seq_Security"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO TOLIVE.SEC_WEB_MENU_LINKS(");
            sql.AppendLine("wml_id, web_menu_id, parent_id, display_text, sort_order, url, open_new_window, ");
            sql.AppendLine("roles, no_access_show, tooltip, is_iis_id_app, active, sy_program_code, image_url, ");
            sql.AppendLine("recurse_folder, menu_type, modified_by, include_in_global_menu)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":wml_id, :web_menu_id, :parent_id, :display_text, :sort_order, :url, :open_new_window, ");
            sql.AppendLine(":roles, :no_access_show, :tooltip, :is_iis_id_app, :active, :sy_program_code, ");
            sql.AppendLine(":image_url, :recurse_folder, :menu_type, :modified_by, :include_in_global_menu)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("wml_id", obj.Wml_Id);
            ins.Parameters.Add("web_menu_id", obj.Web_Menu_Id);
            ins.Parameters.Add("parent_id", obj.Parent_Id);
            ins.Parameters.Add("display_text", obj.Display_Text);
            ins.Parameters.Add("sort_order", obj.Sort_Order);
            ins.Parameters.Add("url", obj.Url);
            ins.Parameters.Add("open_new_window", obj.Open_New_Window ? 1 : 0);
            ins.Parameters.Add("roles", obj.Roles);
            ins.Parameters.Add("no_access_show", obj.No_Access_Show ? 1 : 0);
            ins.Parameters.Add("tooltip", obj.Tooltip);
            ins.Parameters.Add("is_iis_id_app", obj.Is_Iis_Id_App ? 1 : 0);
            ins.Parameters.Add("active", obj.Active ? 1 : 0);
            ins.Parameters.Add("sy_program_code", obj.Sy_Program_Code);
            ins.Parameters.Add("image_url", obj.Image_Url);
            ins.Parameters.Add("recurse_folder", obj.Recurse_Folder ? 1 : 0);
            ins.Parameters.Add("menu_type", (short) obj.Menu_Type);
            ins.Parameters.Add("modified_by", obj.Modified_By);
            if (obj.Include_In_Global_Menu != null)
                ins.Parameters.Add("include_in_global_menu", obj.Include_In_Global_Menu.Value ? 1 : 0);
            else
                ins.Parameters.Add("include_in_global_menu", null);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Sec_Web_Menu_Links obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Sec_Web_Menu_Links obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE TOLIVE.SEC_WEB_MENU_LINKS SET");
            sql.AppendLine("web_menu_id = :web_menu_id, ");
            sql.AppendLine("parent_id = :parent_id, ");
            sql.AppendLine("display_text = :display_text, ");
            sql.AppendLine("sort_order = :sort_order, ");
            sql.AppendLine("url = :url, ");
            sql.AppendLine("open_new_window = :open_new_window, ");
            sql.AppendLine("roles = :roles, ");
            sql.AppendLine("no_access_show = :no_access_show, ");
            sql.AppendLine("tooltip = :tooltip, ");
            sql.AppendLine("is_iis_id_app = :is_iis_id_app, ");
            sql.AppendLine("active = :active, ");
            sql.AppendLine("sy_program_code = :sy_program_code, ");
            sql.AppendLine("image_url = :image_url, ");
            sql.AppendLine("recurse_folder = :recurse_folder, ");
            sql.AppendLine("menu_type = :menu_type, ");
            sql.AppendLine("modified_by = :modified_by, ");
            sql.AppendLine("include_in_global_menu = :include_in_global_menu");
            sql.AppendLine("WHERE wml_id = :wml_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("web_menu_id", obj.Web_Menu_Id);
            upd.Parameters.Add("parent_id", obj.Parent_Id);
            upd.Parameters.Add("display_text", obj.Display_Text);
            upd.Parameters.Add("sort_order", obj.Sort_Order);
            upd.Parameters.Add("url", obj.Url);
            upd.Parameters.Add("open_new_window", obj.Open_New_Window ? 1 : 0);
            upd.Parameters.Add("roles", obj.Roles);
            upd.Parameters.Add("no_access_show", obj.No_Access_Show ? 1 : 0);
            upd.Parameters.Add("tooltip", obj.Tooltip);
            upd.Parameters.Add("is_iis_id_app", obj.Is_Iis_Id_App ? 1 : 0);
            upd.Parameters.Add("active", obj.Active ? 1 : 0);
            upd.Parameters.Add("sy_program_code", obj.Sy_Program_Code);
            upd.Parameters.Add("image_url", obj.Image_Url);
            upd.Parameters.Add("recurse_folder", obj.Recurse_Folder ? 1 : 0);
            upd.Parameters.Add("menu_type", (short) obj.Menu_Type);
            upd.Parameters.Add("modified_by", obj.Modified_By);
            if (obj.Include_In_Global_Menu != null)
                upd.Parameters.Add("include_in_global_menu", obj.Include_In_Global_Menu.Value ? 1 : 0);
            else
                upd.Parameters.Add("include_in_global_menu", null);
            upd.Parameters.Add("wml_id", obj.Wml_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(Sec_Web_Menu_Links obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(Sec_Web_Menu_Links obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM TOLIVE.SEC_WEB_MENU_LINKS");
            sql.AppendLine("WHERE wml_id = :wml_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("wml_id", obj.Wml_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static Sec_Web_Menu_Links DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Sec_Web_Menu_Links RetVal = new();
            RetVal.Wml_Id = (int)(decimal)Util.GetRowVal(row, $"{ColPrefix}wml_id");
            RetVal.Web_Menu_Id = (int)(decimal)Util.GetRowVal(row, $"{ColPrefix}web_menu_id");
            RetVal.Parent_Id = (int?)(decimal?)Util.GetRowVal(row, $"{ColPrefix}parent_id");
            RetVal.Display_Text = (string)Util.GetRowVal(row, $"{ColPrefix}display_text");
            RetVal.Sort_Order = (short?)(decimal?)Util.GetRowVal(row, $"{ColPrefix}sort_order");
            RetVal.Url = (string)Util.GetRowVal(row, $"{ColPrefix}url");
            RetVal.Open_New_Window = (decimal)Util.GetRowVal(row, $"{ColPrefix}open_new_window")==1;
            RetVal.Roles = (string)Util.GetRowVal(row, $"{ColPrefix}roles");
            RetVal.No_Access_Show = (decimal)Util.GetRowVal(row, $"{ColPrefix}no_access_show")== 1;
            RetVal.Tooltip = (string)Util.GetRowVal(row, $"{ColPrefix}tooltip");
            RetVal.Is_Iis_Id_App = (decimal)Util.GetRowVal(row, $"{ColPrefix}is_iis_id_app")== 1;
            RetVal.Active = (decimal)Util.GetRowVal(row, $"{ColPrefix}active")== 1;
            RetVal.Sy_Program_Code = (string)Util.GetRowVal(row, $"{ColPrefix}sy_program_code");
            RetVal.Image_Url = (string)Util.GetRowVal(row, $"{ColPrefix}image_url");
            RetVal.Recurse_Folder = (decimal)Util.GetRowVal(row, $"{ColPrefix}recurse_folder")== 1;
            RetVal.Menu_Type = (Enums.SecWebMenuType)(short)(decimal)Util.GetRowVal(row, $"{ColPrefix}menu_type");
            RetVal.Modified_By = (string)Util.GetRowVal(row, $"{ColPrefix}modified_by");
            decimal? globalMnu = (decimal?)Util.GetRowVal(row, $"{ColPrefix}include_in_global_menu");
            if (globalMnu.HasValue)
                RetVal.Include_In_Global_Menu = globalMnu.Value == 1;
            else
                RetVal.Include_In_Global_Menu = null;
            return RetVal;
        }

    }
}
