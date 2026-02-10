using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL
{

    /// <summary>
    /// Static class used to create SQL on the fly
    /// </summary>
    /// <remarks>
    /// To use this you must have use the following attributes on properties in your model
    /// System.ComponentModel.DataAnnotations.Key - Use this attribute to specify the columns used for the primary key.  This is what will be used for Update/Delete statements where clause
    /// System.ComponentModel.DataAnnotations.Schema.NotMapped - Use this if you have a property that you do not want mapped to a DB Column
    /// 
    /// All properties MUST be the same name as the column name
    /// </remarks>
    public static class SqlBuilder
    {

        /// <summary>
        /// Gets columns of a model for a select statement
        /// </summary>
        /// <param name="Model">Model type</param>
        /// <param name="TableAlias">Alias of the table if needed</param>
        /// <param name="TableColumnPrefix">what to prefix the columns for aliasing</param>
        /// <returns></returns>
        public static string GetColumnsForSelect(Type Model, string TableAlias = "", string TableColumnPrefix = "")
        {
            StringBuilder sql = new();
            var props = Model.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var prop in props)
            {

                //check if the property has a NotMapped attribute, if so, then exclude it
                if (!prop.CustomAttributes.Any(x => x.AttributeType.FullName == "System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute"))
                {
                    if (sql.Length > 0)
                        sql.AppendLine(", ");

                    if (!string.IsNullOrEmpty(TableAlias))
                        sql.Append($"{TableAlias}.");

                    sql.Append(prop.Name);

                    if (!string.IsNullOrEmpty(TableColumnPrefix))
                        sql.Append($" {TableColumnPrefix}{prop.Name}");
                }
            }

            return sql.ToString();
        }



        /// <summary>
        /// Generates a select statement for a given data model
        /// </summary>
        /// <param name="Model">Model type</param>
        /// <param name="TableName">Table name</param>
        /// <param name="TableAlias">Alias of the table if needed</param>
        /// <param name="TableColumnPrefix">what to prefix the columns for aliasing</param>
        /// <returns></returns>
        public static string GetSelect(Type Model, string TableName, string TableAlias = "", string TableColumnPrefix = "")
        {
            StringBuilder sql = new();
            sql.Append("SELECT ");
            sql.AppendLine(GetColumnsForSelect(Model, TableAlias, TableColumnPrefix));
            sql.Append($"FROM {TableName}");

            if (!string.IsNullOrEmpty(TableAlias))
                sql.Append($" {TableAlias}");

            sql.AppendLine();
            return sql.ToString();
        }

        /// <summary>
        /// Generates an Insert command object based on the object model
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="TableName"></param>
        /// <param name="Db"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static System.Data.Common.DbCommand GetInsert(Object Model, string TableName, MOO.Data.DBType DBType)
        {
            string paramPrefix = GetParamPrefix(DBType);
            var cmd = CreateCommand(DBType);
            StringBuilder insSql = new();
            StringBuilder valuesSql = new();
            var props = Model.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            short paramNbr = 0;
            foreach (var prop in props)
            {

                //check if the property has a NotMapped attribute, if so, then exclude it
                if (!prop.CustomAttributes.Any(x => x.AttributeType.FullName == "System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute"))
                {

                    if (insSql.Length > 0)
                    {
                        insSql.Append(", ");
                        valuesSql.Append(", ");
                    }

                    insSql.Append(prop.Name);
                    //insert paramater name, we will use numbering p1, p2, p3 etc. in case a column is using a reserved word
                    valuesSql.Append(paramPrefix + "p" + paramNbr);

                    //Oracle cannot convert a bool to number so handle that here
                    var parm = cmd.CreateParameter();
                    var parmValue = Model.GetType().GetProperty(prop.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(Model, null);
                    if (parmValue is bool parmBool)
                        parm.Value = parmBool ? 1 : 0;
                    else if(parmValue is Enum parmEnum)
                        parm.Value = parmEnum.ToString();
                    else if (parmValue is Guid parmGuid)
                        parm.Value = parmGuid.ToByteArray();
                    else
                        parm.Value = parmValue;

                    parm.ParameterName = "p" + paramNbr;
                    cmd.Parameters.Add(parm);
                    paramNbr++;
                }
            }
            if (insSql.Length == 0)
            {
                throw new Exception($"Error on creating insert statement for {TableName} no columns were specified");
            }
            cmd.CommandText = $"INSERT INTO {TableName} \n({insSql})\nVALUES({valuesSql}) ";

            return cmd;
        }



        /// <summary>
        /// Returns an Update DB command with parameters set to point to the values of the object model
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="TableName"></param>
        /// <param name="Db"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static System.Data.Common.DbCommand GetUpdate(Object Model, string TableName, MOO.Data.DBType DBType)
        {
            string paramPrefix = GetParamPrefix(DBType);
            var cmd = CreateCommand(DBType);
            StringBuilder updSql = new();
            StringBuilder whereClause = new();
            var props = Model.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            short paramNbr = 0;
            foreach (var prop in props)
            {

                //check if the property has a NotMapped attribute, if so, then exclude it
                if (!prop.CustomAttributes.Any(x => x.AttributeType.FullName == "System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute"))
                {
                    //check if this is the key
                    if (prop.CustomAttributes.Any(x => x.AttributeType.FullName == "System.ComponentModel.DataAnnotations.KeyAttribute"))
                    {
                        if (whereClause.Length > 0)
                        {
                            whereClause.Append("AND ");
                        }
                        whereClause.AppendLine($"{prop.Name} = {paramPrefix}p{paramNbr}");
                    }
                    else
                    {
                        //normal column
                        if (updSql.Length > 0)
                            updSql.AppendLine(",");

                        updSql.Append($"{prop.Name} = {paramPrefix}p{paramNbr}");
                    }

                    var parm = cmd.CreateParameter();
                    //Oracle cannot convert a bool to number so handle that here
                    var parmValue = Model.GetType().GetProperty(prop.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(Model, null);
                    if (parmValue is bool parmBool)
                        parm.Value = parmBool ? 1 : 0;
                    else if (parmValue is Enum parmEnum)
                        parm.Value = parmEnum.ToString();
                    else if (parmValue is Guid parmGuid)
                        parm.Value = parmGuid.ToByteArray();
                    else
                        parm.Value = parmValue;
                    parm.ParameterName = "p" + paramNbr;
                    cmd.Parameters.Add(parm);                    
                    
                }
                paramNbr++;
            }
            if (whereClause.Length == 0)
            {
                throw new Exception($"Error on creating update statement for {TableName} no keys were specified");
            }

            cmd.CommandText = $"UPDATE {TableName} SET {updSql} \n WHERE {whereClause}";

            return cmd;

        }


        /// <summary>
        /// Generates a delete command object based on the object model
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="TableName"></param>
        /// <param name="Db"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static System.Data.Common.DbCommand GetDelete(Object Model, string TableName, MOO.Data.DBType DBType)
        {
            string paramPrefix = GetParamPrefix(DBType);
            var cmd = CreateCommand(DBType);
            StringBuilder whereClause = new();
            var props = Model.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            short paramNbr = 0;
            foreach (var prop in props)
            {

                //check if the property has a NotMapped attribute, if so, then exclude it
                if (!prop.CustomAttributes.Any(x => x.AttributeType.FullName == "System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute"))
                {
                    //check if this is the key
                    if (prop.CustomAttributes.Any(x => x.AttributeType.FullName == "System.ComponentModel.DataAnnotations.KeyAttribute"))
                    {
                        if (whereClause.Length > 0)
                        {
                            whereClause.Append("AND ");
                        }
                        whereClause.AppendLine($"{prop.Name} = {paramPrefix}p{paramNbr}");

                        //Oracle cannot convert a bool to number so handle that here
                        var parm = cmd.CreateParameter();
                        var parmValue = Model.GetType().GetProperty(prop.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(Model, null);
                        if (parmValue is bool parmBool)
                            parm.Value = parmBool ? 1 : 0;
                        else if (parmValue is Enum parmEnum)
                            parm.Value = parmEnum.ToString();
                        else if (parmValue is Guid parmGuid)
                            parm.Value = parmGuid.ToByteArray();
                        else
                            parm.Value = parmValue;

                        parm.ParameterName = "p" + paramNbr;
                        cmd.Parameters.Add(parm);
                    }


                }
                paramNbr++;
            }
            if (whereClause.Length == 0)
            {
                throw new Exception($"Error on creating delete statement for {TableName} no keys were specified");
            }

            cmd.CommandText = $"DELETE FROM {TableName} \n WHERE {whereClause}";

            return cmd;

        }


        /// <summary>
        /// Creates a DB Command for a given database without the commandtext set
        /// </summary>
        /// <param name="Db"></param>
        /// <returns></returns>
        private static System.Data.Common.DbCommand CreateCommand(MOO.Data.DBType DBType)
        {

            DbCommand cmd;
            DbProviderFactory Factory = System.Data.Common.DbProviderFactories.GetFactory(DBType.ToString());
            cmd = Factory.CreateCommand();

            //set bindbyname to true for Oracle as parameters may not be in order
            if (cmd is OracleCommand oracleCommand)
                oracleCommand.BindByName = true;

            return cmd;
        }

        private static string GetParamPrefix(MOO.Data.DBType DBType)
        {
            switch (DBType)
            {
                case Data.DBType.Oracle:
                    return ":";
                case Data.DBType.SQLServer:
                    return "@";
                default:
                    return "?";
            }
        }







        #region "Insert, Update, Delete Executes"



        /// <summary>
        /// Executes the insert for the object with a specified primary key.  Creates needed DB Connection
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="Database"></param>
        /// <param name="TableName"></param>
        /// <returns>records affected</returns>
        public static int Insert(object Model, MOO.Data.MNODatabase Database, string TableName)
        {
            return InsertAsync(Model,Database,TableName).GetAwaiter().GetResult();

        }


        /// <summary>
        /// Executes the insert for the object with a specified primary key.  Uses passed in connection object if transactions were used
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="conn"></param>
        /// <param name="DBType"></param>
        /// <param name="TableName"></param>
        /// <returns>records affected</returns>
        public static int Insert(object Model, DbConnection conn, Data.DBType DBType, string TableName)
        {
            return InsertAsync(Model,conn,DBType,TableName).GetAwaiter().GetResult();
        }


        /// <summary>
        /// Executes the insert for the object with a specified primary key.  Creates needed DB Connection
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="Database"></param>
        /// <param name="TableName"></param>
        /// <returns>records affected</returns>
        public static async Task<int> InsertAsync(object Model, MOO.Data.MNODatabase Database, string TableName)
        {
            var db = MOO.Data.DBConnections[Database.ToString()];

            DbProviderFactory Factory = System.Data.Common.DbProviderFactories.GetFactory(db.DBType.ToString());

            using DbConnection conn = Factory.CreateConnection();
            conn.ConnectionString = db.ConnectionString;
            await conn.OpenAsync();

            int recsAffected = await InsertAsync(Model, conn, db.DBType, TableName);
            await conn.CloseAsync();
            return recsAffected;

        }


        /// <summary>
        /// Executes the insert for the object with a specified primary key.  Uses passed in connection object if transactions were used
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="conn"></param>
        /// <param name="DBType"></param>
        /// <param name="TableName"></param>
        /// <returns>records affected</returns>
        public static async Task<int> InsertAsync(object Model, DbConnection conn, Data.DBType DBType, string TableName)
        {
            var cmd = SqlBuilder.GetInsert(Model, TableName, DBType);
            cmd.Connection = conn;
            int recsAffected = await cmd.ExecuteNonQueryAsync();
            return recsAffected;
        }




        /// <summary>
        /// Executes the update for the object with a specified primary key.  Creates needed DB Connection
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="Database"></param>
        /// <param name="TableName"></param>
        /// <returns>records affected</returns>
        public static int Update(object Model, MOO.Data.MNODatabase Database, string TableName)
        {
            return UpdateAsync(Model,Database,TableName).GetAwaiter().GetResult();

        }

        /// <summary>
        /// Executes the update for the object with a specified primary key.  Uses passed in connection object if transactions were used
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="conn"></param>
        /// <param name="DBType"></param>
        /// <param name="TableName"></param>
        /// <returns>records affected</returns>
        public static int Update(object Model, DbConnection conn, Data.DBType DBType, string TableName)
        {
            return UpdateAsync(Model,conn,DBType,TableName).GetAwaiter().GetResult();
        }


        /// <summary>
        /// Executes the update for the object with a specified primary key.  Creates needed DB Connection
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="Database"></param>
        /// <param name="TableName"></param>
        /// <returns>records affected</returns>
        public static async Task<int> UpdateAsync(object Model, MOO.Data.MNODatabase Database, string TableName)
        {
            var db = MOO.Data.DBConnections[Database.ToString()];

            DbProviderFactory Factory = System.Data.Common.DbProviderFactories.GetFactory(db.DBType.ToString());

            using DbConnection conn = Factory.CreateConnection();
            conn.ConnectionString = db.ConnectionString;
            await conn.OpenAsync();

            int recsAffected = await UpdateAsync(Model, conn, db.DBType, TableName);
            await conn.CloseAsync();
            return recsAffected;

        }

        /// <summary>
        /// Executes the update for the object with a specified primary key.  Uses passed in connection object if transactions were used
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="conn"></param>
        /// <param name="DBType"></param>
        /// <param name="TableName"></param>
        /// <returns>records affected</returns>
        public static async Task<int> UpdateAsync(object Model, DbConnection conn, Data.DBType DBType, string TableName)
        {
            var cmd = SqlBuilder.GetUpdate(Model, TableName, DBType);
            cmd.Connection = conn;
            int recsAffected = await cmd.ExecuteNonQueryAsync();
            return recsAffected;
        }


        /// <summary>
        /// Executes the delete for the object with a specified primary key.  Creates needed DB Connection
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="Database"></param>
        /// <param name="TableName"></param>
        /// <returns>records affected</returns>
        public static int Delete(object Model, MOO.Data.MNODatabase Database, string TableName)
        {
            return DeleteAsync(Model,Database,TableName).GetAwaiter().GetResult();

        }

        /// <summary>
        /// Executes the delete for the object with a specified primary key.  Uses passed in connection object if transactions were used
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="conn"></param>
        /// <param name="DBType"></param>
        /// <param name="TableName"></param>
        /// <returns>records affected</returns>
        public static int Delete(object Model, DbConnection conn, Data.DBType DBType, string TableName)
        {
            return DeleteAsync(Model,conn,DBType,TableName).GetAwaiter().GetResult();
        }



        /// <summary>
        /// Executes the delete for the object with a specified primary key.  Creates needed DB Connection
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="Database"></param>
        /// <param name="TableName"></param>
        /// <returns>records affected</returns>
        public static async Task<int> DeleteAsync(object Model, MOO.Data.MNODatabase Database, string TableName)
        {
            var db = MOO.Data.DBConnections[Database.ToString()];

            DbProviderFactory Factory = System.Data.Common.DbProviderFactories.GetFactory(db.DBType.ToString());

            using DbConnection conn = Factory.CreateConnection();
            conn.ConnectionString = db.ConnectionString;
            await conn.OpenAsync();

            int recsAffected = await DeleteAsync(Model, conn, db.DBType, TableName);
            await conn.CloseAsync();
            return recsAffected;

        }

        /// <summary>
        /// Executes the delete for the object with a specified primary key.  Uses passed in connection object if transactions were used
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="conn"></param>
        /// <param name="DBType"></param>
        /// <param name="TableName"></param>
        /// <returns>records affected</returns>
        public static async Task<int> DeleteAsync(object Model, DbConnection conn, Data.DBType DBType, string TableName)
        {
            var cmd = SqlBuilder.GetDelete(Model, TableName, DBType);
            cmd.Connection = conn;
            int recsAffected = await cmd.ExecuteNonQueryAsync();
            return recsAffected;
        }

        #endregion
    }
}
