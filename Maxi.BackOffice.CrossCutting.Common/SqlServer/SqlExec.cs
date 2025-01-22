using System.ComponentModel;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Reflection;
using System.Text;

using Maxi.BackOffice.CrossCutting.Common.Attributes;
using Maxi.BackOffice.CrossCutting.Common.Common;
using Maxi.BackOffice.CrossCutting.Common.Configurations;

namespace Maxi.BackOffice.CrossCutting.Common.SqlServer
{
    public static class SqlExec
    {
        #region Not Transaction
        public static int Insert<T>(this T Row, string Conexion, bool Conexionstring = false) where T : IEntityType
        {
            SqlConnection Sqlconexion;
            int Identity = 0;
            if (Conexionstring)
            {
                Sqlconexion = new SqlConnection(Conexion);
            }
            else
            {
                Sqlconexion = new SqlConnection(AppSettings.ConnectionStrings(Conexion));
            }

            StringBuilder insert = new StringBuilder();
            StringBuilder Values = new StringBuilder();
            SqlCommand ComandoSQL = new SqlCommand();
            ComandoSQL.Connection = Sqlconexion;

            #region Proceso de Insercion de Registro

            insert.Append("/* Consulta Generada AutomaticamentePor TaxeiaFramework */" + System.Environment.NewLine);
            insert.Append("INSERT INTO ");
            Values.Append("VALUES (");

            Attribute[] AtributosEntidad = Attribute.GetCustomAttributes(typeof(T));


            #region Asignacion de Nombre de esquema y  Tabla
            foreach (System.Attribute Atributo in AtributosEntidad)
            {
                if (Atributo is EntityAtributes)
                {
                    EntityAtributes DetalleTabla = (EntityAtributes)Atributo;
                    insert.Append(DetalleTabla.Schema + "." + DetalleTabla.Tablename + "(");
                }
            }
            #endregion

            #region Recorremos cada Propiedad (Campo) de la clase (Tabla)
            PropertyDescriptorCollection Propiedades = TypeDescriptor.GetProperties(typeof(T));
            foreach (PropertyDescriptor Propiedad in Propiedades)
            {
                AttributeCollection Atributos = Propiedad.Attributes;

                foreach (System.Attribute Atributo in Atributos)
                {

                    if (Atributo is PropEntityAtributes)
                    {

                        insert.Append(string.Format("[{0}],", Propiedad.Name));
                        Values.AppendFormat("@{0} ,", Propiedad.Name);

                        SqlParameter Param = new SqlParameter();
                        PropEntityAtributes DetalleColumna = (PropEntityAtributes)Atributo;
                        Param = GetParameter(Propiedad.PropertyType.FullName, Propiedad.Name, Propiedad.GetValue(Row), DetalleColumna);
                        ComandoSQL.Parameters.Add(Param);
                    }
                }
            }
            #endregion
            char[] Caracteres = { ',' };
            ComandoSQL.CommandText = string.Format("{0}){1}) SELECT  ID = CONVERT(int,SCOPE_IDENTITY())", insert.ToString().TrimEnd(Caracteres), Values.ToString().TrimEnd(Caracteres));

            #endregion
            if (Sqlconexion.State != ConnectionState.Open)
            {
                Sqlconexion.Open();
            }
            try
            {
                Identity = Convert.ToInt32(ComandoSQL.ExecuteScalar());

                ComandoSQL.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Sqlconexion.Close();
            }
            return Identity;
        }
        public static void InsertBulk(DataTable dtDatos, string Table, string Conexion, bool Conexionstring = false)
        {
            SqlConnection Sqlconexion;
            if (Conexionstring)
            {
                Sqlconexion = new SqlConnection(Conexion);
            }
            else
            {
                Sqlconexion = new SqlConnection(AppSettings.ConnectionStrings(Conexion));
            }

            if (Sqlconexion.State != ConnectionState.Open)
            {
                Sqlconexion.Open();
            }

            try
            {
                SqlBulkCopy sbc = new SqlBulkCopy(Sqlconexion);
                sbc.DestinationTableName = Table;
                sbc.WriteToServer(dtDatos);
                sbc.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Sqlconexion.Close();
            }
        }
        public static int Update<T>(this T Row, string Conexion, bool Conexionstring = false) where T : IEntityType
        {
            SqlConnection Sqlconexion;
            if (Conexionstring)
            {
                Sqlconexion = new SqlConnection(Conexion);
            }
            else
            {
                Sqlconexion = new SqlConnection(AppSettings.ConnectionStrings(Conexion));
            }
            int Rows = 0;

            StringBuilder Update = new StringBuilder();
            StringBuilder Values = new StringBuilder();
            StringBuilder Where = new StringBuilder();
            SqlCommand ComandoSQL = new SqlCommand();
            ComandoSQL.Connection = Sqlconexion;

            Attribute[] AtributosEntidad = System.Attribute.GetCustomAttributes(typeof(T));

            Update.Append("/*Consulta Generada Automaticamente Por TaxeiaFramework*/" + System.Environment.NewLine);
            Update.Append(" UPDATE ");
            Values.Append(" SET ");

            foreach (System.Attribute Atributo in AtributosEntidad)
            {

                if (Atributo is EntityAtributes)
                {
                    EntityAtributes DetalleAtributosEntidad = (EntityAtributes)Atributo;

                    Update.Append(DetalleAtributosEntidad.Schema + "." + DetalleAtributosEntidad.Tablename);
                }

            }

            PropertyDescriptorCollection Propiedades = TypeDescriptor.GetProperties(typeof(T));
            foreach (PropertyDescriptor Propiedad in Propiedades)
            {
                AttributeCollection Atributos = Propiedad.Attributes;

                foreach (System.Attribute Atributo in Atributos)
                {

                    if (Atributo is PropEntityAtributes)
                    {
                        PropEntityAtributes DetalleAtributos = (PropEntityAtributes)Atributo;

                        if (DetalleAtributos.Update == true && DetalleAtributos.Key != true)
                        {

                            Values.AppendFormat("[{0}] = @{0} ,", Propiedad.Name);

                            SqlParameter Param = new SqlParameter();
                            PropEntityAtributes DetalleColumna = (PropEntityAtributes)Atributo;
                            Param = GetParameter(Propiedad.PropertyType.FullName, Propiedad.Name, Propiedad.GetValue(Row), DetalleColumna);
                            ComandoSQL.Parameters.Add(Param);

                        }

                        if (DetalleAtributos.Key == true)
                        {
                            Where.AppendFormat("Where {0} = @{0}", Propiedad.Name);
                            SqlParameter Param = new SqlParameter(string.Format("@{0}", Propiedad.Name), SqlDbType.Int);
                            Param.Value = Propiedad.GetValue(Row);
                            ComandoSQL.Parameters.Add(Param);
                        }
                    }
                }




            }
            char[] Caracteres = { ',' };
            ComandoSQL.CommandText = string.Format("{0} {1} {2} SELECT FilasAfectadas = @@ROWCOUNT ", Update.ToString().TrimEnd(Caracteres), Values.ToString().TrimEnd(Caracteres), Where.ToString());


            if (Sqlconexion.State != ConnectionState.Open)
            {
                Sqlconexion.Open();
            }
            try
            {
                Rows = Convert.ToInt32(ComandoSQL.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return Rows;
        }
        public static void UpdateBulk(DataTable dtDatos, string Table, List<string> CamposActualizar, string PrimaryKey, string Conexion, bool Conexionstring = false)
        {
            SqlConnection Sqlconexion;
            if (Conexionstring)
            {
                Sqlconexion = new SqlConnection(Conexion);
            }
            else
            {
                Sqlconexion = new SqlConnection(AppSettings.ConnectionStrings(Conexion));
            }
            SqlCommand ComandoSQL = new SqlCommand
            {
                Connection = Sqlconexion
            };
            if (Sqlconexion.State != ConnectionState.Open)
            {
                Sqlconexion.Open();
            }

            try
            {
                //Creating temp table on database
                StringBuilder QueryTablaTemporal = new StringBuilder();
                string IDTemp = Guid.NewGuid().ToString().Replace("-", "_");

                QueryTablaTemporal.AppendLine(string.Format("SELECT * INTO #TmpTable{1} FROM {0} WITH(NOLOCK) WHERE 1=0", Table, IDTemp));
                ComandoSQL.CommandText = QueryTablaTemporal.ToString();
                ComandoSQL.ExecuteNonQuery();

                QueryTablaTemporal.AppendLine(")");
                ComandoSQL.CommandText = QueryTablaTemporal.ToString();
                ComandoSQL.ExecuteNonQuery();


                SqlBulkCopy sbc = new SqlBulkCopy(Sqlconexion)
                {
                    DestinationTableName = string.Format("#TmpTable{0}", IDTemp)
                };
                sbc.WriteToServer(dtDatos);
                sbc.Close();


                // Updating destination table, and dropping temp table
                StringBuilder QueryUpdate = new StringBuilder();
                QueryUpdate.AppendLine("UPDATE T SET");

                foreach (string Campo in CamposActualizar)
                {
                    QueryUpdate.AppendFormat("T.[{0}] = Temp.[{0}],", Campo);
                }
                char[] Caracteres = { ',' };
                ComandoSQL.CommandText = string.Format("{2} FROM {0} T WITH(NOLOCK) INNER JOIN #TmpTable{3} Temp WITH(NOLOCK) ON T.{1} = Temp.{1} ", Table, PrimaryKey, QueryUpdate.ToString().TrimEnd(Caracteres), IDTemp);
                ComandoSQL.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Sqlconexion.Close();
            }
        }
        public static int Delete<T>(this T Row, string Conexion, bool Conexionstring = false) where T : IEntityType
        {
            SqlConnection Sqlconexion;
            if (Conexionstring)
            {
                Sqlconexion = new SqlConnection(Conexion);
            }
            else
            {
                Sqlconexion = new SqlConnection(AppSettings.ConnectionStrings(Conexion));
            }
            int Rows = 0;
            StringBuilder Delete = new StringBuilder();
            StringBuilder Where = new StringBuilder();
            SqlCommand ComandoSQL = new SqlCommand();
            DataTable DtResponseSQL = new DataTable();
            ComandoSQL.Connection = Sqlconexion;


            Delete.Append("/*Consulta Generada Automaticamente Desde TaxeiaFramework*/" + System.Environment.NewLine);

            System.Attribute[] AtributosEntidad = System.Attribute.GetCustomAttributes(typeof(T));
            foreach (System.Attribute Atributo in AtributosEntidad)
            {

                if (Atributo is EntityAtributes)
                {
                    EntityAtributes DetalleAtributosEntidad = (EntityAtributes)Atributo;

                    Delete.AppendFormat("DELETE  FROM {0}.{1}", DetalleAtributosEntidad.Schema, DetalleAtributosEntidad.Tablename);
                }

            }

            PropertyDescriptorCollection Propiedades = TypeDescriptor.GetProperties(typeof(T));
            foreach (PropertyDescriptor Propiedad in Propiedades)
            {

                AttributeCollection Atributos = Propiedad.Attributes;

                foreach (System.Attribute Atributo in Atributos)
                {

                    if (Atributo is PropEntityAtributes)
                    {
                        PropEntityAtributes DetalleAtributos = (PropEntityAtributes)Atributo;

                        if (DetalleAtributos.Key == true)
                        {
                            Where.AppendFormat("{0} = @{0}", Propiedad.Name);
                            SqlParameter Param = new SqlParameter(string.Format("@{0}", Propiedad.Name), SqlDbType.Int);
                            Param.Value = Propiedad.GetValue(Row);
                            ComandoSQL.Parameters.Add(Param);
                        }
                    }
                }
            }

            if (Sqlconexion.State != ConnectionState.Open)
            {
                Sqlconexion.Open();
            }

            ComandoSQL.CommandText = string.Format("{0} WHERE {1} SELECT FilasAfectadas = @@ROWCOUNT ", Delete.ToString(), Where.ToString());


            if (Sqlconexion.State != ConnectionState.Open)
            {
                Sqlconexion.Open();
            }
            try
            {
                Rows = Convert.ToInt32(ComandoSQL.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Sqlconexion.Close();
            }

            return Rows;
        }
        public static List<T> GetAll<T>(this T Row, string Conexion, bool Conexionstring = false) where T : class, IEntityType, new()
        {
            SqlConnection Sqlconexion;
            if (Conexionstring)
            {
                Sqlconexion = new SqlConnection(Conexion);
            }
            else
            {
                Sqlconexion = new SqlConnection(AppSettings.ConnectionStrings(Conexion));
            }

            StringBuilder Select = new StringBuilder();
            StringBuilder Where = new StringBuilder();
            SqlCommand ComandoSQL = new SqlCommand();
            SqlDataReader ReaderSQL;
            DataTable DtResponseSQL = new DataTable();
            ComandoSQL.Connection = Sqlconexion;


            Select.Append("/* Consulta Generada Automaticamente Por Sofmedic.DataAcces  */" + System.Environment.NewLine);


            System.Attribute[] AtributosEntidad = Attribute.GetCustomAttributes(typeof(T));
            foreach (System.Attribute Atributo in AtributosEntidad)
            {

                if (Atributo is EntityAtributes)
                {
                    EntityAtributes DetalleAtributosEntidad = (EntityAtributes)Atributo;
                    Select = new StringBuilder();
                    Select.AppendFormat("SELECT * FROM {0}.{1} WITH(NOLOCK)", DetalleAtributosEntidad.Schema, DetalleAtributosEntidad.Tablename);
                }
                if (Atributo is QueryEntityAtributes)
                {
                    QueryEntityAtributes DetalleAtributosEntidad = (QueryEntityAtributes)Atributo;
                    Select = new StringBuilder();
                    Select.AppendFormat(DetalleAtributosEntidad.Texto);
                }

            }



            ComandoSQL.CommandText = Select.ToString();
            if (Sqlconexion.State != ConnectionState.Open)
            {
                Sqlconexion.Open();
            }
            try
            {
                ReaderSQL = ComandoSQL.ExecuteReader();
                DtResponseSQL.Load(ReaderSQL);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Sqlconexion.Close();
            }


            IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            List<T> result = new List<T>();

            foreach (var row in DtResponseSQL.Rows)
            {
                result.Add(ConvertTo.CreateItemFromRow<T>((DataRow)row, properties));
            }

            return result;
        }
        public static List<T> GetByFilter<T>(this T Row, string Filter, List<SqlParam> Params, string Conexion, bool Conexionstring = false) where T : class, IEntityType, new()
        {
            SqlConnection Sqlconexion;
            if (Conexionstring)
            {
                Sqlconexion = new SqlConnection(Conexion);
            }
            else
            {
                Sqlconexion = new SqlConnection(AppSettings.ConnectionStrings(Conexion));
            }
            StringBuilder Select = new StringBuilder();
            StringBuilder Where = new StringBuilder();
            SqlCommand ComandoSQL = new SqlCommand();
            SqlDataReader ReaderSQL;
            DataTable DtResponseSQL = new DataTable();
            ComandoSQL.Connection = Sqlconexion;


            Select.Append("/* Consulta Generada Automaticamente Por Sofmedic.DataAcces */" + System.Environment.NewLine);
            Where.Append("1 = 1");

            System.Attribute[] AtributosEntidad = System.Attribute.GetCustomAttributes(typeof(T));
            foreach (System.Attribute Atributo in AtributosEntidad)
            {

                if (Atributo is EntityAtributes)
                {
                    EntityAtributes DetalleAtributosEntidad = (EntityAtributes)Atributo;
                    Select = new StringBuilder();
                    Select.AppendFormat("SELECT * FROM {0}.{1} WITH(NOLOCK)", DetalleAtributosEntidad.Schema, DetalleAtributosEntidad.Tablename);
                }
                if (Atributo is QueryEntityAtributes)
                {
                    QueryEntityAtributes DetalleAtributosEntidad = (QueryEntityAtributes)Atributo;
                    Select = new StringBuilder();
                    Select.AppendFormat(DetalleAtributosEntidad.Texto);
                }

            }
            if (Params != null)
            {
                foreach (SqlParam P in Params)
                {
                    P.Name = P.Name.Trim();
                    string TypeFullName = P.Value.GetType().FullName;
                    SqlParameter Param = new SqlParameter();
                    switch (TypeFullName)
                    {
                        case "System.DateTime":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.DateTime);
                            if (P.Value != null)
                            {
                                DateTime FechaIni = Convert.ToDateTime("1753-1-1 12:00:00 AM");
                                DateTime FechaFin = Convert.ToDateTime("9999-12-31 11:59:59 PM");
                                if (Convert.ToDateTime(P.Value) >= FechaFin || Convert.ToDateTime(P.Value) <= FechaIni)
                                {
                                    throw new ArgumentException(string.Format("El SqlParam {0} Debe estar entre {1} y {2}", P.Name, FechaIni, FechaFin));
                                }
                            }
                            break;
                        case "System.String":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.VarChar);
                            break;
                        case "System.Guid":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.UniqueIdentifier);
                            break;
                        case "System.Boolean":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Bit);
                            break;
                        case "System.Decimal":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Decimal);
                            break;
                        case "System.Double":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Float);
                            break;
                        case "System.Int16":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.SmallInt);
                            break;
                        case "System.Int32":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Int);
                            break;
                        case "System.Int64":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.BigInt);
                            break;
                        case "System.Object":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Variant);
                            break;
                        case "System.Single":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Real);
                            break;
                        default:
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.VarChar);
                            break;
                    }

                    Param.Value = (P.Value == null) ? DBNull.Value : P.Value;
                    ComandoSQL.Parameters.Add(Param);

                }
            }

            if (Filter.Length > 0)
            {
                Where = new StringBuilder();
                Where.Append(Filter);
            }

            ComandoSQL.CommandText = string.Format("{0} WHERE {1}", Select.ToString(), Where.ToString());
            if (Sqlconexion.State != ConnectionState.Open)
            {
                Sqlconexion.Open();
            }
            try
            {
                ReaderSQL = ComandoSQL.ExecuteReader();
                DtResponseSQL.Load(ReaderSQL);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Sqlconexion.Close();
            }


            IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            List<T> result = new List<T>();

            foreach (var row in DtResponseSQL.Rows)
            {
                var item = ConvertTo.CreateItemFromRow<T>((DataRow)row, properties);
                result.Add(item);
            }

            return result;
        }
        public static T GetById<T>(this T Row, string Conexion, bool Conexionstring = false) where T : class, IEntityType, new()
        {
            SqlConnection Sqlconexion;
            if (Conexionstring)
            {
                Sqlconexion = new SqlConnection(Conexion);
            }
            else
            {
                Sqlconexion = new SqlConnection(AppSettings.ConnectionStrings(Conexion));
            }
            StringBuilder Select = new StringBuilder();
            StringBuilder Where = new StringBuilder();
            SqlCommand ComandoSQL = new SqlCommand();
            SqlDataReader ReaderSQL;
            DataTable DtResponseSQL = new DataTable();
            ComandoSQL.Connection = Sqlconexion;


            Select.Append("/*Consulta Generada Automaticamente Desde TaxeiaFramework*/" + System.Environment.NewLine);

            System.Attribute[] AtributosEntidad = System.Attribute.GetCustomAttributes(typeof(T));
            foreach (System.Attribute Atributo in AtributosEntidad)
            {

                if (Atributo is EntityAtributes)
                {
                    EntityAtributes DetalleAtributosEntidad = (EntityAtributes)Atributo;

                    Select.AppendFormat("SELECT * FROM {0}.{1} WITH(NOLOCK)", DetalleAtributosEntidad.Schema, DetalleAtributosEntidad.Tablename);
                }

            }

            PropertyDescriptorCollection Propiedades = TypeDescriptor.GetProperties(typeof(T));
            foreach (PropertyDescriptor Propiedad in Propiedades)
            {

                AttributeCollection Atributos = Propiedad.Attributes;

                foreach (System.Attribute Atributo in Atributos)
                {

                    if (Atributo is PropEntityAtributes)
                    {
                        PropEntityAtributes DetalleAtributos = (PropEntityAtributes)Atributo;

                        if (DetalleAtributos.Key == true)
                        {
                            Where.AppendFormat("{0} = @{0}", Propiedad.Name);
                            SqlParameter Param = new SqlParameter(string.Format("@{0}", Propiedad.Name), SqlDbType.Int);
                            Param.Value = Propiedad.GetValue(Row);
                            ComandoSQL.Parameters.Add(Param);
                        }
                    }
                }
            }

            if (Sqlconexion.State != ConnectionState.Open)
            {
                Sqlconexion.Open();
            }

            try
            {
                ComandoSQL.CommandText = string.Format("{0} WHERE {1}", Select.ToString(), Where.ToString());
                ReaderSQL = ComandoSQL.ExecuteReader();
                DtResponseSQL.Load(ReaderSQL);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Sqlconexion.Close();
            }

            IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            List<T> result = new List<T>();

            foreach (var row in DtResponseSQL.Rows)
            {
                var item = ConvertTo.CreateItemFromRow<T>((DataRow)row, properties);
                result.Add(item);
            }

            return result.FirstOrDefault();
        }
        public static List<T> GetQuery<T>(this T Row, string Query, List<SqlParam> Params, string Conexion, bool Conexionstring = false) where T : class, IEntityType, new()
        {
            SqlConnection Sqlconexion;
            if (Conexionstring)
            {
                Sqlconexion = new SqlConnection(Conexion);
            }
            else
            {
                Sqlconexion = new SqlConnection(AppSettings.ConnectionStrings(Conexion));
            }
            StringBuilder SqlQuery = new StringBuilder();
            SqlCommand ComandoSQL = new SqlCommand();
            SqlDataReader ReaderSQL;
            DataTable DtResponseSQL = new DataTable();
            ComandoSQL.Connection = Sqlconexion;


            SqlQuery.Append("/*Consulta Generada Desde TaxeiaFramework*/" + System.Environment.NewLine);
            SqlQuery.Append(Query);

            if (Params != null)
            {
                foreach (SqlParam P in Params)
                {
                    P.Name = P.Name.Trim();
                    string TypeFullName = "System.String";
                    if (P.Value != null)
                    {
                        TypeFullName = P.Value.GetType().FullName;
                    }
                    SqlParameter Param = new SqlParameter();
                    switch (TypeFullName)
                    {
                        case "System.DateTime":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.DateTime);
                            if (P.Value != null)
                            {
                                DateTime FechaIni = Convert.ToDateTime("1753-1-1 12:00:00 AM");
                                DateTime FechaFin = Convert.ToDateTime("9999-12-31 11:59:59 PM");
                                if (Convert.ToDateTime(P.Value) >= FechaFin || Convert.ToDateTime(P.Value) <= FechaIni)
                                {
                                    throw new ArgumentException(string.Format("El SqlParam {0} Debe estar entre {1} y {2}", P.Name, FechaIni, FechaFin));
                                }
                            }
                            break;
                        case "System.String":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.VarChar);
                            break;
                        case "System.Boolean":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Bit);
                            break;
                        case "System.Decimal":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Decimal);
                            break;
                        case "System.Double":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Float);
                            break;
                        case "System.Int16":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.SmallInt);
                            break;
                        case "System.Int32":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Int);
                            break;
                        case "System.Int64":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.BigInt);
                            break;
                        case "System.Object":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Variant);
                            break;
                        case "System.Single":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Real);
                            break;
                        case "System.Guid":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.UniqueIdentifier);
                            break;
                        default:
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.VarChar);
                            break;
                    }

                    Param.Value = (P.Value == null) ? DBNull.Value : P.Value;
                    ComandoSQL.Parameters.Add(Param);

                }
            }

            if (Sqlconexion.State != ConnectionState.Open)
            {
                Sqlconexion.Open();
            }

            try
            {
                ComandoSQL.CommandText = SqlQuery.ToString();
                ReaderSQL = ComandoSQL.ExecuteReader();
                DtResponseSQL.Load(ReaderSQL);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Sqlconexion.Close();
            }

            IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            List<T> result = new List<T>();

            foreach (var row in DtResponseSQL.Rows)
            {
                var item = ConvertTo.CreateItemFromRow<T>((DataRow)row, properties);
                result.Add(item);
            }

            return result;
        }
        public static T GetScalar<T>(this T Row, string Query, List<SqlParam> Params, string Conexion, bool Conexionstring = false) where T : IEntityType
        {
            SqlConnection Sqlconexion;
            if (Conexionstring)
            {
                Sqlconexion = new SqlConnection(Conexion);
            }
            else
            {
                Sqlconexion = new SqlConnection(AppSettings.ConnectionStrings(Conexion));
            }
            StringBuilder SqlQuery = new StringBuilder();
            SqlCommand ComandoSQL = new SqlCommand();
            T Response;
            ComandoSQL.Connection = Sqlconexion;


            SqlQuery.Append("/*Consulta Generada Desde TaxeiaFramework*/" + System.Environment.NewLine);
            SqlQuery.Append(Query);

            if (Params != null)
            {
                foreach (SqlParam P in Params)
                {
                    P.Name = P.Name.Trim();
                    string TypeFullName = "System.String";
                    if (P.Value != null)
                    {
                        TypeFullName = P.Value.GetType().FullName;
                    }
                    SqlParameter Param = new SqlParameter();
                    switch (TypeFullName)
                    {
                        case "System.DateTime":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.DateTime);
                            if (P.Value != null)
                            {
                                DateTime FechaIni = Convert.ToDateTime("1753-1-1 12:00:00 AM");
                                DateTime FechaFin = Convert.ToDateTime("9999-12-31 11:59:59 PM");
                                if (Convert.ToDateTime(P.Value) >= FechaFin || Convert.ToDateTime(P.Value) <= FechaIni)
                                {
                                    throw new ArgumentException(string.Format("El SqlParam {0} Debe estar entre {1} y {2}", P.Name, FechaIni, FechaFin));
                                }
                            }
                            break;
                        case "System.String":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.VarChar);
                            break;
                        case "System.Guid":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.UniqueIdentifier);
                            break;
                        case "System.Boolean":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Bit);
                            break;
                        case "System.Decimal":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Decimal);
                            break;
                        case "System.Double":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Float);
                            break;
                        case "System.Int16":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.SmallInt);
                            break;
                        case "System.Int32":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Int);
                            break;
                        case "System.Int64":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.BigInt);
                            break;
                        case "System.Object":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Variant);
                            break;
                        case "System.Single":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Real);
                            break;
                        default:
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.VarChar);
                            break;
                    }

                    Param.Value = (P.Value == null) ? DBNull.Value : P.Value;
                    ComandoSQL.Parameters.Add(Param);

                }
            }

            if (Sqlconexion.State != ConnectionState.Open)
            {
                Sqlconexion.Open();
            }
            try
            {
                ComandoSQL.CommandText = SqlQuery.ToString();
                Response = (T)ComandoSQL.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Sqlconexion.Close();
            }


            return Response;
        }
        public static void GetNoReturn(string Query, List<SqlParam> Params, string Conexion, bool Conexionstring = false)
        {
            SqlConnection Sqlconexion;
            if (Conexionstring)
            {
                Sqlconexion = new SqlConnection(Conexion);
            }
            else
            {
                Sqlconexion = new SqlConnection(AppSettings.ConnectionStrings(Conexion));
            }
            StringBuilder SqlQuery = new StringBuilder();
            SqlCommand ComandoSQL = new SqlCommand();
            ComandoSQL.Connection = Sqlconexion;


            SqlQuery.Append("/*Consulta Generada Desde TaxeiaFramework*/" + System.Environment.NewLine);
            SqlQuery.Append(Query);

            if (Params != null)
            {
                foreach (SqlParam P in Params)
                {
                    P.Name = P.Name.Trim();
                    string TypeFullName = "System.String";
                    if (P.Value != null)
                    {
                        TypeFullName = P.Value.GetType().FullName;
                    }
                    SqlParameter Param = new SqlParameter();
                    switch (TypeFullName)
                    {
                        case "System.DateTime":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.DateTime);
                            if (P.Value != null)
                            {
                                DateTime FechaIni = Convert.ToDateTime("1753-1-1 12:00:00 AM");
                                DateTime FechaFin = Convert.ToDateTime("9999-12-31 11:59:59 PM");
                                if (Convert.ToDateTime(P.Value) >= FechaFin || Convert.ToDateTime(P.Value) <= FechaIni)
                                {
                                    throw new ArgumentException(string.Format("El SqlParam {0} Debe estar entre {1} y {2}", P.Name, FechaIni, FechaFin));
                                }
                            }
                            break;
                        case "System.String":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.VarChar);
                            break;
                        case "System.Guid":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.UniqueIdentifier);
                            break;
                        case "System.Boolean":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Bit);
                            break;
                        case "System.Decimal":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Decimal);
                            break;
                        case "System.Double":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Float);
                            break;
                        case "System.Int16":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.SmallInt);
                            break;
                        case "System.Int32":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Int);
                            break;
                        case "System.Int64":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.BigInt);
                            break;
                        case "System.Object":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Variant);
                            break;
                        case "System.Single":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Real);
                            break;
                        default:
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.VarChar);
                            break;
                    }

                    Param.Value = (P.Value == null) ? DBNull.Value : P.Value;
                    ComandoSQL.Parameters.Add(Param);

                }
            }

            if (Sqlconexion.State != ConnectionState.Open)
            {
                Sqlconexion.Open();
            }

            try
            {
                ComandoSQL.CommandText = SqlQuery.ToString();
                ComandoSQL.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Sqlconexion.Close();
            }
        }
        public static DataSet GetMultiData(string Query, List<SqlParam> Params, string Conexion, bool Conexionstring = false)
        {
            SqlConnection Sqlconexion;
            if (Conexionstring)
            {
                Sqlconexion = new SqlConnection(Conexion);
            }
            else
            {
                Sqlconexion = new SqlConnection(AppSettings.ConnectionStrings(Conexion));
            }
            StringBuilder SqlQuery = new StringBuilder();
            SqlCommand ComandoSQL = new SqlCommand();
            SqlDataAdapter AdapterSQL = new SqlDataAdapter();
            DataSet DtResponseSQL = new DataSet();
            ComandoSQL.Connection = Sqlconexion;


            SqlQuery.Append("/*Consulta Generada Desde TaxeiaFramework*/" + System.Environment.NewLine);
            SqlQuery.Append(Query);

            if (Params != null)
            {
                foreach (SqlParam P in Params)
                {
                    P.Name = P.Name.Trim();
                    string TypeFullName = "System.String";
                    if (P.Value != null)
                    {
                        TypeFullName = P.Value.GetType().FullName;
                    }
                    SqlParameter Param = new SqlParameter();
                    switch (TypeFullName)
                    {
                        case "System.DateTime":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.DateTime);
                            if (P.Value != null)
                            {
                                DateTime FechaIni = Convert.ToDateTime("1753-1-1 12:00:00 AM");
                                DateTime FechaFin = Convert.ToDateTime("9999-12-31 11:59:59 PM");
                                if (Convert.ToDateTime(P.Value) >= FechaFin || Convert.ToDateTime(P.Value) <= FechaIni)
                                {
                                    throw new ArgumentException(string.Format("El SqlParam {0} Debe estar entre {1} y {2}", P.Name, FechaIni, FechaFin));
                                }
                            }
                            break;
                        case "System.String":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.VarChar);
                            break;
                        case "System.Boolean":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Bit);
                            break;
                        case "System.Decimal":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Decimal);
                            break;
                        case "System.Double":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Float);
                            break;
                        case "System.Int16":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.SmallInt);
                            break;
                        case "System.Int32":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Int);
                            break;
                        case "System.Int64":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.BigInt);
                            break;
                        case "System.Object":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Variant);
                            break;
                        case "System.Single":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Real);
                            break;
                        case "System.Guid":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.UniqueIdentifier);
                            break;
                        default:
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.VarChar);
                            break;
                    }

                    Param.Value = (P.Value == null) ? DBNull.Value : P.Value;
                    ComandoSQL.Parameters.Add(Param);

                }
            }

            if (Sqlconexion.State != ConnectionState.Open)
            {
                Sqlconexion.Open();
            }

            try
            {
                ComandoSQL.CommandText = SqlQuery.ToString();
                AdapterSQL.SelectCommand = ComandoSQL;
                AdapterSQL.Fill(DtResponseSQL);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Sqlconexion.Close();
            }
            return DtResponseSQL;
        }
        #endregion


        #region With Transaction
        public static int Insert<T>(this T Row, SqlConnection Conexion, SqlTransaction Transaction) where T : IEntityType
        {
            int Identity = 0;
            StringBuilder insert = new StringBuilder();
            StringBuilder Values = new StringBuilder();
            SqlCommand ComandoSQL = new SqlCommand();
            ComandoSQL.Connection = Conexion;
            ComandoSQL.Transaction = Transaction;

            #region Proceso de Insercion de Registro

            insert.Append("/* Consulta Generada AutomaticamentePor TaxeiaFramework */" + System.Environment.NewLine);
            insert.Append("INSERT INTO ");
            Values.Append("VALUES (");

            Attribute[] AtributosEntidad = Attribute.GetCustomAttributes(typeof(T));


            #region Asignacion de Nombre de esquema y  Tabla
            foreach (System.Attribute Atributo in AtributosEntidad)
            {
                if (Atributo is EntityAtributes)
                {
                    EntityAtributes DetalleTabla = (EntityAtributes)Atributo;
                    insert.Append(DetalleTabla.Schema + ".[" + DetalleTabla.Tablename + "](");
                }
            }
            #endregion

            #region Recorremos cada Propiedad (Campo) de la clase (Tabla)
            PropertyDescriptorCollection Propiedades = TypeDescriptor.GetProperties(typeof(T));
            foreach (PropertyDescriptor Propiedad in Propiedades)
            {
                AttributeCollection Atributos = Propiedad.Attributes;

                foreach (System.Attribute Atributo in Atributos)
                {

                    if (Atributo is PropEntityAtributes)
                    {
                        PropEntityAtributes DetalleColumna = (PropEntityAtributes)Atributo;

                        if ((!DetalleColumna.Key && DetalleColumna.Insert==true) || Propiedad.PropertyType.FullName == "System.Guid")
                        {
                            insert.Append(string.Format("[{0}],", Propiedad.Name));
                            Values.AppendFormat("@{0} ,", Propiedad.Name);

                            SqlParameter Param = new SqlParameter();
                            Param = GetParameter(Propiedad.PropertyType.FullName, Propiedad.Name, Propiedad.GetValue(Row), DetalleColumna);
                            ComandoSQL.Parameters.Add(Param);
                        }
                    }
                }
            }
            #endregion
            char[] Caracteres = { ',' };
            ComandoSQL.CommandText = string.Format("{0}){1}) SELECT  ID = CONVERT(int,SCOPE_IDENTITY())", insert.ToString().TrimEnd(Caracteres), Values.ToString().TrimEnd(Caracteres));

            try
            {
                Identity = Convert.ToInt32(ComandoSQL.ExecuteScalar());
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            return Identity;
            #endregion
        }

        public static void InsertBulk(DataTable dtDatos, string Table, SqlConnection Conexion, SqlTransaction Transaction)
        {
            try
            {
                SqlBulkCopy sbc = new SqlBulkCopy(Conexion, SqlBulkCopyOptions.Default, Transaction);
                sbc.DestinationTableName = Table;
                sbc.WriteToServer(dtDatos);
                sbc.Close();
            }
            catch (SqlException ex)
            {
                throw ex;
            }

        }

        public static int Update<T>(this T Row, SqlConnection Conexion, SqlTransaction Transaction) where T : IEntityType
        {
            int Rows = 0;

            StringBuilder Update = new StringBuilder();
            StringBuilder Values = new StringBuilder();
            StringBuilder Where = new StringBuilder();
            SqlCommand ComandoSQL = new SqlCommand();
            ComandoSQL.Connection = Conexion;
            ComandoSQL.Transaction = Transaction;

            Attribute[] AtributosEntidad = System.Attribute.GetCustomAttributes(typeof(T));

            Update.Append("/*Consulta Generada Automaticamente Por TaxeiaFramework*/" + System.Environment.NewLine);
            Update.Append(" UPDATE ");
            Values.Append(" SET ");

            foreach (System.Attribute Atributo in AtributosEntidad)
            {

                if (Atributo is EntityAtributes)
                {
                    EntityAtributes DetalleAtributosEntidad = (EntityAtributes)Atributo;

                    Update.Append(DetalleAtributosEntidad.Schema + ".[" + DetalleAtributosEntidad.Tablename + "]");
                }

            }

            PropertyDescriptorCollection Propiedades = TypeDescriptor.GetProperties(typeof(T));
            foreach (PropertyDescriptor Propiedad in Propiedades)
            {
                AttributeCollection Atributos = Propiedad.Attributes;

                foreach (System.Attribute Atributo in Atributos)
                {

                    if (Atributo is PropEntityAtributes)
                    {
                        PropEntityAtributes DetalleAtributos = (PropEntityAtributes)Atributo;

                        if (DetalleAtributos.Update == true && DetalleAtributos.Key != true)
                        {

                            Values.AppendFormat("[{0}] = @{0} ,", Propiedad.Name);

                            SqlParameter Param = new SqlParameter();
                            PropEntityAtributes DetalleColumna = (PropEntityAtributes)Atributo;
                            Param = GetParameter(Propiedad.PropertyType.FullName, Propiedad.Name, Propiedad.GetValue(Row), DetalleColumna);
                            ComandoSQL.Parameters.Add(Param);

                        }

                        if (DetalleAtributos.Key == true)
                        {
                            Where.AppendFormat("Where {0} = @{0}", Propiedad.Name);
                            SqlParameter Param = new SqlParameter(string.Format("@{0}", Propiedad.Name), SqlDbType.Int);
                            Param.Value = Propiedad.GetValue(Row);
                            ComandoSQL.Parameters.Add(Param);
                        }
                    }
                }




            }
            char[] Caracteres = { ',' };
            ComandoSQL.CommandText = string.Format("{0} {1} {2} SELECT FilasAfectadas = @@ROWCOUNT ", Update.ToString().TrimEnd(Caracteres), Values.ToString().TrimEnd(Caracteres), Where.ToString());

            try
            {
                Rows = Convert.ToInt32(ComandoSQL.ExecuteScalar());
            }
            catch (SqlException ex)
            {
                throw ex;
            }


            return Rows;
        }

        public static void UpdateBulk(DataTable dtDatos, string Table, List<string> CamposActualizar, string PrimaryKey, SqlConnection Conexion, SqlTransaction Transaction)
        {
            SqlCommand ComandoSQL = new SqlCommand();
            ComandoSQL.Connection = Conexion;
            ComandoSQL.Transaction = Transaction;

            try
            {
                //Creating temp table on database
                StringBuilder QueryTablaTemporal = new StringBuilder();

                string IDTemp = Guid.NewGuid().ToString().Replace("-", "_");

                QueryTablaTemporal.AppendLine(string.Format("SELECT * INTO #TmpTable{1} FROM {0} WITH(NOLOCK) WHERE 1=0", Table, IDTemp));
                ComandoSQL.CommandText = QueryTablaTemporal.ToString();
                ComandoSQL.ExecuteNonQuery();


                SqlBulkCopy sbc = new SqlBulkCopy(Conexion, SqlBulkCopyOptions.Default, Transaction)
                {
                    DestinationTableName = string.Format("#TmpTable{0}", IDTemp)
                };
                sbc.WriteToServer(dtDatos);
                sbc.Close();


                // Updating destination table, and dropping temp table
                StringBuilder QueryUpdate = new StringBuilder();
                QueryUpdate.AppendLine("UPDATE T SET");

                foreach (string Campo in CamposActualizar)
                {
                    QueryUpdate.AppendFormat("T.[{0}] = Temp.[{0}],", Campo);
                }
                char[] Caracteres = { ',' };
                ComandoSQL.CommandText = string.Format("{2} FROM {0} T WITH(NOLOCK) INNER JOIN #TmpTable{3} Temp WITH(NOLOCK) ON T.{1} = Temp.{1} ", Table, PrimaryKey, QueryUpdate.ToString().TrimEnd(Caracteres), IDTemp);
                ComandoSQL.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw ex;
            }

        }

        public static int Delete<T>(this T Row, SqlConnection Conexion, SqlTransaction Transaction) where T : IEntityType
        {
            int Rows = 0;
            StringBuilder Delete = new StringBuilder();
            StringBuilder Where = new StringBuilder();
            SqlCommand ComandoSQL = new SqlCommand();
            DataTable DtResponseSQL = new DataTable();
            ComandoSQL.Connection = Conexion;
            ComandoSQL.Transaction = Transaction;


            Delete.Append("/*Consulta Generada Automaticamente Desde TaxeiaFramework*/" + System.Environment.NewLine);

            System.Attribute[] AtributosEntidad = System.Attribute.GetCustomAttributes(typeof(T));
            foreach (System.Attribute Atributo in AtributosEntidad)
            {

                if (Atributo is EntityAtributes)
                {
                    EntityAtributes DetalleAtributosEntidad = (EntityAtributes)Atributo;

                    Delete.AppendFormat("DELETE  FROM {0}.[{1}]", DetalleAtributosEntidad.Schema, DetalleAtributosEntidad.Tablename);
                }

            }

            PropertyDescriptorCollection Propiedades = TypeDescriptor.GetProperties(typeof(T));
            foreach (PropertyDescriptor Propiedad in Propiedades)
            {

                AttributeCollection Atributos = Propiedad.Attributes;

                foreach (System.Attribute Atributo in Atributos)
                {

                    if (Atributo is PropEntityAtributes)
                    {
                        PropEntityAtributes DetalleAtributos = (PropEntityAtributes)Atributo;

                        if (DetalleAtributos.Key == true)
                        {
                            Where.AppendFormat("{0} = @{0}", Propiedad.Name);
                            SqlParameter Param = new SqlParameter(string.Format("@{0}", Propiedad.Name), SqlDbType.Int);
                            Param.Value = Propiedad.GetValue(Row);
                            ComandoSQL.Parameters.Add(Param);
                        }
                    }
                }
            }
            ComandoSQL.CommandText = string.Format("{0} WHERE {1} SELECT FilasAfectadas = @@ROWCOUNT ", Delete.ToString(), Where.ToString());

            try
            {
                Rows = Convert.ToInt32(ComandoSQL.ExecuteScalar());
            }
            catch (SqlException ex)
            {
                throw ex;
            }

            return Rows;
        }

        public static List<T> GetAll<T>(this T Row, SqlConnection Conexion, SqlTransaction Transaction) where T : class, IEntityType, new()
        {
            StringBuilder Select = new StringBuilder();
            StringBuilder Where = new StringBuilder();
            SqlCommand ComandoSQL = new SqlCommand();
            SqlDataReader ReaderSQL;
            DataTable DtResponseSQL = new DataTable();
            ComandoSQL.Connection = Conexion;
            ComandoSQL.Transaction = Transaction;


            Select.Append("/* Consulta Generada Automaticamente Por Sofmedic.DataAcces  */" + System.Environment.NewLine);


            System.Attribute[] AtributosEntidad = Attribute.GetCustomAttributes(typeof(T));
            foreach (System.Attribute Atributo in AtributosEntidad)
            {

                if (Atributo is EntityAtributes)
                {
                    EntityAtributes DetalleAtributosEntidad = (EntityAtributes)Atributo;
                    Select = new StringBuilder();
                    Select.AppendFormat("SELECT * FROM {0}.[{1}] WITH(NOLOCK)", DetalleAtributosEntidad.Schema, DetalleAtributosEntidad.Tablename);
                }
                if (Atributo is QueryEntityAtributes)
                {
                    QueryEntityAtributes DetalleAtributosEntidad = (QueryEntityAtributes)Atributo;
                    Select = new StringBuilder();
                    Select.AppendFormat(DetalleAtributosEntidad.Texto);
                }

            }



            ComandoSQL.CommandText = Select.ToString();
            try
            {
                ReaderSQL = ComandoSQL.ExecuteReader();
                DtResponseSQL.Load(ReaderSQL);
            }
            catch (SqlException ex)
            {
                throw ex;
            }


            IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            List<T> result = new List<T>();

            foreach (var row in DtResponseSQL.Rows)
            {
                result.Add(ConvertTo.CreateItemFromRow<T>((DataRow)row, properties));
            }

            return result;
        }

        public static List<T> GetByFilter<T>(this T Row, string Filter, List<SqlParam> Params, SqlConnection Conexion, SqlTransaction Transaction) where T : class, IEntityType, new()
        {
            StringBuilder Select = new StringBuilder();
            StringBuilder Where = new StringBuilder();
            SqlCommand ComandoSQL = new SqlCommand();
            SqlDataReader ReaderSQL;
            DataTable DtResponseSQL = new DataTable();
            ComandoSQL.Connection = Conexion;
            ComandoSQL.Transaction = Transaction;


            Select.Append("/* Consulta Generada Automaticamente Por Sofmedic.DataAcces */" + System.Environment.NewLine);
            Where.Append("1 = 1");

            System.Attribute[] AtributosEntidad = System.Attribute.GetCustomAttributes(typeof(T));
            foreach (System.Attribute Atributo in AtributosEntidad)
            {

                if (Atributo is EntityAtributes)
                {
                    EntityAtributes DetalleAtributosEntidad = (EntityAtributes)Atributo;
                    Select = new StringBuilder();
                    Select.AppendFormat("SELECT * FROM {0}.[{1}] WITH(NOLOCK)", DetalleAtributosEntidad.Schema, DetalleAtributosEntidad.Tablename);
                }
                if (Atributo is QueryEntityAtributes)
                {
                    QueryEntityAtributes DetalleAtributosEntidad = (QueryEntityAtributes)Atributo;
                    Select = new StringBuilder();
                    Select.AppendFormat(DetalleAtributosEntidad.Texto);
                }

            }
            if (Params != null)
            {
                foreach (SqlParam P in Params)
                {
                    P.Name = P.Name.Trim();
                    string TypeFullName = P.Value.GetType().FullName;
                    SqlParameter Param = new SqlParameter();
                    switch (TypeFullName)
                    {
                        case "System.DateTime":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.DateTime);
                            if (P.Value != null)
                            {
                                DateTime FechaIni = Convert.ToDateTime("1753-1-1 12:00:00 AM");
                                DateTime FechaFin = Convert.ToDateTime("9999-12-31 11:59:59 PM");
                                if (Convert.ToDateTime(P.Value) >= FechaFin || Convert.ToDateTime(P.Value) <= FechaIni)
                                {
                                    throw new ArgumentException(string.Format("El SqlParam {0} Debe estar entre {1} y {2}", P.Name, FechaIni, FechaFin));
                                }
                            }
                            break;
                        case "System.String":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.VarChar);
                            break;
                        case "System.Guid":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.UniqueIdentifier);
                            break;
                        case "System.Boolean":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Bit);
                            break;
                        case "System.Decimal":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Decimal);
                            break;
                        case "System.Double":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Float);
                            break;
                        case "System.Int16":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.SmallInt);
                            break;
                        case "System.Int32":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Int);
                            break;
                        case "System.Int64":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.BigInt);
                            break;
                        case "System.Object":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Variant);
                            break;
                        case "System.Single":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Real);
                            break;
                        default:
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.VarChar);
                            break;
                    }

                    Param.Value = (P.Value == null) ? DBNull.Value : P.Value;
                    ComandoSQL.Parameters.Add(Param);

                }
            }

            if (Filter.Length > 0)
            {
                Where = new StringBuilder();
                Where.Append(Filter);
            }

            ComandoSQL.CommandText = string.Format("{0} WHERE {1}", Select.ToString(), Where.ToString());

            try
            {
                ReaderSQL = ComandoSQL.ExecuteReader();
                DtResponseSQL.Load(ReaderSQL);
            }
            catch (SqlException ex)
            {
                throw ex;
            }

            IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            List<T> result = new List<T>();

            foreach (var row in DtResponseSQL.Rows)
            {
                var item = ConvertTo.CreateItemFromRow<T>((DataRow)row, properties);
                result.Add(item);
            }

            return result;
        }

        public static T GetById<T>(this T Row, SqlConnection Conexion, SqlTransaction Transaction) where T : class, IEntityType, new()
        {
            StringBuilder Select = new StringBuilder();
            StringBuilder Where = new StringBuilder();
            SqlCommand ComandoSQL = new SqlCommand();
            SqlDataReader ReaderSQL;
            DataTable DtResponseSQL = new DataTable();
            ComandoSQL.Connection = Conexion;
            ComandoSQL.Transaction = Transaction;


            Select.Append("/*Consulta Generada Automaticamente Desde TaxeiaFramework*/" + System.Environment.NewLine);

            System.Attribute[] AtributosEntidad = System.Attribute.GetCustomAttributes(typeof(T));
            foreach (System.Attribute Atributo in AtributosEntidad)
            {

                if (Atributo is EntityAtributes)
                {
                    EntityAtributes DetalleAtributosEntidad = (EntityAtributes)Atributo;

                    Select.AppendFormat("SELECT * FROM {0}.[{1}] WITH(NOLOCK)", DetalleAtributosEntidad.Schema, DetalleAtributosEntidad.Tablename);
                }

            }

            PropertyDescriptorCollection Propiedades = TypeDescriptor.GetProperties(typeof(T));
            foreach (PropertyDescriptor Propiedad in Propiedades)
            {

                AttributeCollection Atributos = Propiedad.Attributes;

                foreach (System.Attribute Atributo in Atributos)
                {

                    if (Atributo is PropEntityAtributes)
                    {
                        PropEntityAtributes DetalleAtributos = (PropEntityAtributes)Atributo;

                        if (DetalleAtributos.Key == true)
                        {
                            Where.AppendFormat("{0} = @{0}", Propiedad.Name);
                            SqlParameter Param = new SqlParameter(string.Format("@{0}", Propiedad.Name), SqlDbType.Int);
                            Param.Value = Propiedad.GetValue(Row);
                            ComandoSQL.Parameters.Add(Param);
                        }
                    }
                }
            }

            try
            {
                ComandoSQL.CommandText = string.Format("{0} WHERE {1}", Select.ToString(), Where.ToString());
                ReaderSQL = ComandoSQL.ExecuteReader();
                DtResponseSQL.Load(ReaderSQL);
            }
            catch (SqlException ex)
            {
                throw ex;
            }

            IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            List<T> result = new List<T>();

            foreach (var row in DtResponseSQL.Rows)
            {
                var item = ConvertTo.CreateItemFromRow<T>((DataRow)row, properties);
                result.Add(item);
            }

            return result.FirstOrDefault();
        }

        public static List<T> GetQuery<T>(this T Row, string Query, List<SqlParam> Params, SqlConnection Conexion, SqlTransaction Transaction) where T : class, IEntityType, new()
        {
            StringBuilder SqlQuery = new StringBuilder();
            SqlCommand ComandoSQL = new SqlCommand();
            SqlDataReader ReaderSQL;
            DataTable DtResponseSQL = new DataTable();
            ComandoSQL.Connection = Conexion;
            ComandoSQL.Transaction = Transaction;


            SqlQuery.Append("/*Consulta Generada Desde TaxeiaFramework*/" + System.Environment.NewLine);
            SqlQuery.Append(Query);

            if (Params != null)
            {
                foreach (SqlParam P in Params)
                {
                    P.Name = P.Name.Trim();
                    string TypeFullName = "System.String";
                    if (P.Value != null)
                    {
                        TypeFullName = P.Value.GetType().FullName;
                    }
                    SqlParameter Param = new SqlParameter();
                    switch (TypeFullName)
                    {
                        case "System.DateTime":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.DateTime);
                            if (P.Value != null)
                            {
                                DateTime FechaIni = Convert.ToDateTime("1753-1-1 12:00:00 AM");
                                DateTime FechaFin = Convert.ToDateTime("9999-12-31 11:59:59 PM");
                                if (Convert.ToDateTime(P.Value) >= FechaFin || Convert.ToDateTime(P.Value) <= FechaIni)
                                {
                                    throw new ArgumentException(string.Format("El SqlParam {0} Debe estar entre {1} y {2}", P.Name, FechaIni, FechaFin));
                                }
                            }
                            break;
                        case "System.String":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.VarChar);
                            break;
                        case "System.Boolean":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Bit);
                            break;
                        case "System.Decimal":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Decimal);
                            break;
                        case "System.Double":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Float);
                            break;
                        case "System.Int16":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.SmallInt);
                            break;
                        case "System.Int32":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Int);
                            break;
                        case "System.Int64":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.BigInt);
                            break;
                        case "System.Object":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Variant);
                            break;
                        case "System.Single":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Real);
                            break;
                        case "System.Guid":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.UniqueIdentifier);
                            break;
                        default:
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.VarChar);
                            break;
                    }

                    Param.Value = (P.Value == null) ? DBNull.Value : P.Value;
                    ComandoSQL.Parameters.Add(Param);

                }
            }

            try
            {
                ComandoSQL.CommandText = SqlQuery.ToString();
                ReaderSQL = ComandoSQL.ExecuteReader();
                DtResponseSQL.Load(ReaderSQL);
            }
            catch (SqlException ex)
            {
                throw ex;
            }


            IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            List<T> result = new List<T>();

            foreach (var row in DtResponseSQL.Rows)
            {
                var item = ConvertTo.CreateItemFromRow<T>((DataRow)row, properties);
                result.Add(item);
            }

            return result;
        }

        //JGV ExecuteScalar regresa el valor de la primer columna del primer renglon, No es un objeto T
        //=> marca error de conversion,  revisar esta rutina
        public static T GetScalar<T>(this T Row, string Query, List<SqlParam> Params, SqlConnection Conexion, SqlTransaction Transaction) where T:IEntityType
        {
            StringBuilder SqlQuery = new StringBuilder();
            SqlCommand ComandoSQL = new SqlCommand();
            T Response;
            ComandoSQL.Connection = Conexion;
            ComandoSQL.Transaction = Transaction;


            SqlQuery.Append("/*Consulta Generada Desde TaxeiaFramework*/" + System.Environment.NewLine);
            SqlQuery.Append(Query);

            if (Params != null)
            {
                foreach (SqlParam P in Params)
                {
                    P.Name = P.Name.Trim();
                    string TypeFullName = "System.String";
                    if (P.Value != null)
                    {
                        TypeFullName = P.Value.GetType().FullName;
                    }
                    SqlParameter Param = new SqlParameter();
                    switch (TypeFullName)
                    {
                        case "System.DateTime":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.DateTime);
                            if (P.Value != null)
                            {
                                DateTime FechaIni = Convert.ToDateTime("1753-1-1 12:00:00 AM");
                                DateTime FechaFin = Convert.ToDateTime("9999-12-31 11:59:59 PM");
                                if (Convert.ToDateTime(P.Value) >= FechaFin || Convert.ToDateTime(P.Value) <= FechaIni)
                                {
                                    throw new ArgumentException(string.Format("El SqlParam {0} Debe estar entre {1} y {2}", P.Name, FechaIni, FechaFin));
                                }
                            }
                            break;
                        case "System.String":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.VarChar);
                            break;
                        case "System.Guid":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.UniqueIdentifier);
                            break;
                        case "System.Boolean":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Bit);
                            break;
                        case "System.Decimal":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Decimal);
                            break;
                        case "System.Double":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Float);
                            break;
                        case "System.Int16":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.SmallInt);
                            break;
                        case "System.Int32":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Int);
                            break;
                        case "System.Int64":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.BigInt);
                            break;
                        case "System.Object":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Variant);
                            break;
                        case "System.Single":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Real);
                            break;
                        default:
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.VarChar);
                            break;
                    }

                    Param.Value = (P.Value == null) ? DBNull.Value : P.Value;
                    ComandoSQL.Parameters.Add(Param);

                }
            }

            try
            {
                ComandoSQL.CommandText = SqlQuery.ToString();
                Response = (T)ComandoSQL.ExecuteScalar();
            }
            catch (SqlException ex)
            {
                throw ex;
            }


            return Response;
        }

        public static void GetNoReturn(string Query, List<SqlParam> Params, SqlConnection Conexion, SqlTransaction Transaction)
        {
            StringBuilder SqlQuery = new StringBuilder();
            SqlCommand ComandoSQL = new SqlCommand();
            ComandoSQL.Connection = Conexion;
            ComandoSQL.Transaction = Transaction;


            SqlQuery.Append("/*Consulta Generada Desde TaxeiaFramework*/" + System.Environment.NewLine);
            SqlQuery.Append(Query);

            if (Params != null)
            {
                foreach (SqlParam P in Params)
                {
                    P.Name = P.Name.Trim();
                    string TypeFullName = "System.String";
                    if (P.Value != null)
                    {
                        TypeFullName = P.Value.GetType().FullName;
                    }
                    SqlParameter Param = new SqlParameter();
                    switch (TypeFullName)
                    {
                        case "System.DateTime":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.DateTime);
                            if (P.Value != null)
                            {
                                DateTime FechaIni = Convert.ToDateTime("1753-1-1 12:00:00 AM");
                                DateTime FechaFin = Convert.ToDateTime("9999-12-31 11:59:59 PM");
                                if (Convert.ToDateTime(P.Value) >= FechaFin || Convert.ToDateTime(P.Value) <= FechaIni)
                                {
                                    throw new ArgumentException(string.Format("El SqlParam {0} Debe estar entre {1} y {2}", P.Name, FechaIni, FechaFin));
                                }
                            }
                            break;
                        case "System.String":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.VarChar);
                            break;
                        case "System.Guid":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.UniqueIdentifier);
                            break;
                        case "System.Boolean":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Bit);
                            break;
                        case "System.Decimal":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Decimal);
                            break;
                        case "System.Double":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Float);
                            break;
                        case "System.Int16":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.SmallInt);
                            break;
                        case "System.Int32":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Int);
                            break;
                        case "System.Int64":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.BigInt);
                            break;
                        case "System.Object":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Variant);
                            break;
                        case "System.Single":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Real);
                            break;
                        default:
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.VarChar);
                            break;
                    }

                    Param.Value = (P.Value == null) ? DBNull.Value : P.Value;
                    ComandoSQL.Parameters.Add(Param);

                }
            }

            try
            {
                ComandoSQL.CommandText = SqlQuery.ToString();
                ComandoSQL.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw ex;
            }
        }

        public static DataSet GetMultiData(string Query, List<SqlParam> Params, SqlConnection Conexion, SqlTransaction Transaction)
        {
            StringBuilder SqlQuery = new StringBuilder();
            SqlCommand ComandoSQL = new SqlCommand();
            SqlDataAdapter AdapterSQL = new SqlDataAdapter();
            DataSet DtResponseSQL = new DataSet();
            ComandoSQL.Connection = Conexion;
            ComandoSQL.Transaction = Transaction;


            SqlQuery.Append("/*Consulta Generada Desde TaxeiaFramework*/" + System.Environment.NewLine);
            SqlQuery.Append(Query);

            if (Params != null)
            {
                foreach (SqlParam P in Params)
                {
                    P.Name = P.Name.Trim();
                    string TypeFullName = "System.String";
                    if (P.Value != null)
                    {
                        TypeFullName = P.Value.GetType().FullName;
                    }
                    SqlParameter Param = new SqlParameter();
                    switch (TypeFullName)
                    {
                        case "System.DateTime":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.DateTime);
                            if (P.Value != null)
                            {
                                DateTime FechaIni = Convert.ToDateTime("1753-1-1 12:00:00 AM");
                                DateTime FechaFin = Convert.ToDateTime("9999-12-31 11:59:59 PM");
                                if (Convert.ToDateTime(P.Value) >= FechaFin || Convert.ToDateTime(P.Value) <= FechaIni)
                                {
                                    throw new ArgumentException(string.Format("El SqlParam {0} Debe estar entre {1} y {2}", P.Name, FechaIni, FechaFin));
                                }
                            }
                            break;
                        case "System.String":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.VarChar);
                            break;
                        case "System.Boolean":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Bit);
                            break;
                        case "System.Decimal":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Decimal);
                            break;
                        case "System.Double":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Float);
                            break;
                        case "System.Int16":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.SmallInt);
                            break;
                        case "System.Int32":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Int);
                            break;
                        case "System.Int64":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.BigInt);
                            break;
                        case "System.Object":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Variant);
                            break;
                        case "System.Single":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.Real);
                            break;
                        case "System.Guid":
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.UniqueIdentifier);
                            break;
                        default:
                            Param = new SqlParameter(string.Format("{0}", P.Name), SqlDbType.VarChar);
                            break;
                    }

                    Param.Value = (P.Value == null) ? DBNull.Value : P.Value;
                    ComandoSQL.Parameters.Add(Param);

                }
            }

            try
            {
                ComandoSQL.CommandText = SqlQuery.ToString();
                AdapterSQL.SelectCommand = ComandoSQL;
                AdapterSQL.Fill(DtResponseSQL);

            }
            catch (SqlException ex)
            {
                throw ex;
            }

            return DtResponseSQL;
        }

        //public static string GetFullTableName<T>(this T row)
        //{
        //    return "test";
        //}

        #endregion

        #region Auxiliar Metods
        private static SqlParameter GetParameter(string TypeFullName, string Name, object Value, PropEntityAtributes CampoAtribute)
        {
            SqlParameter Param;

            if (CampoAtribute.AllowNull)
            {
                switch (CampoAtribute.TipoDato)
                {
                    case Tipo.DateTime:
                        TypeFullName = "System.DateTime";
                        break;
                    case Tipo.String:
                        TypeFullName = "System.String";
                        break;
                    case Tipo.Boolean:
                        TypeFullName = "System.Boolean";
                        break;
                    case Tipo.Decimal:
                        TypeFullName = "System.Decimal";
                        break;
                    case Tipo.Double:
                        TypeFullName = "System.Double";
                        break;
                    case Tipo.Int16:
                        TypeFullName = "System.Int16";
                        break;
                    case Tipo.Int32:
                        TypeFullName = "System.Int32";
                        break;
                    case Tipo.Int64:
                        TypeFullName = "System.Int64";
                        break;
                    case Tipo.Object:
                        TypeFullName = "System.Object";
                        break;
                    case Tipo.TimeSpan:
                        TypeFullName = "System.TimeSpan";
                        break;
                    case Tipo.Guid:
                        TypeFullName = "System.Guid";
                        break;
                    case Tipo.Single:
                        TypeFullName = "System.Single";
                        break;
                    case Tipo.ByteArray:
                        TypeFullName = "System.Byte[]";
                        break;
                }
            }

            switch (TypeFullName)
            {
                case "System.DateTime":
                    Param = new SqlParameter(string.Format("@{0}", Name), SqlDbType.DateTime);
                    if (!CampoAtribute.AllowNull)
                    {
                        DateTime FechaIni = Convert.ToDateTime("1753-1-1 12:00:00 AM");
                        DateTime FechaFin = Convert.ToDateTime("9999-12-31 11:59:59 PM");
                        if (Convert.ToDateTime(Value) >= FechaFin || Convert.ToDateTime(Value) <= FechaIni)
                        {
                            throw new ArgumentException(string.Format("El SqlParam {0} Debe estar entre {1} y {2}", Name, FechaIni, FechaFin));
                        }
                    }
                    Param.Value = (Value == null) ? DBNull.Value : Value;
                    break;
                case "System.String":
                    Param = new SqlParameter(string.Format("@{0}", Name), SqlDbType.VarChar);
                    Param.Value = (Value == null) ? DBNull.Value : Value;
                    break;
                case "System.Boolean":
                    Param = new SqlParameter(string.Format("@{0}", Name), SqlDbType.Bit);
                    Param.Value = (Value == null) ? DBNull.Value : Value;
                    break;
                case "System.Decimal":
                    Param = new SqlParameter(string.Format("@{0}", Name), SqlDbType.Decimal);
                    Param.Value = (Value == null) ? DBNull.Value : Value;
                    break;
                case "System.Double":
                    Param = new SqlParameter(string.Format("@{0}", Name), SqlDbType.Float);
                    Param.Value = (Value == null) ? DBNull.Value : Value;
                    break;
                case "System.Int16":
                    Param = new SqlParameter(string.Format("@{0}", Name), SqlDbType.SmallInt);
                    Param.Value = (Value == null) ? DBNull.Value : Value;
                    break;
                case "System.Int32":
                    Param = new SqlParameter(string.Format("@{0}", Name), SqlDbType.Int);
                    Param.Value = (Value == null) ? DBNull.Value : Value;
                    break;
                case "System.Int64":
                    Param = new SqlParameter(string.Format("@{0}", Name), SqlDbType.BigInt);
                    Param.Value = (Value == null) ? DBNull.Value : Value;
                    break;
                case "System.Object":
                    Param = new SqlParameter(string.Format("@{0}", Name), SqlDbType.Variant);
                    Param.Value = (Value == null) ? DBNull.Value : Value;
                    break;
                case "System.TimeSpan":
                    Param = new SqlParameter(string.Format("@{0}", Name), SqlDbType.Time);
                    Param.Value = (Value == null) ? DBNull.Value : Value;
                    break;

                case "System.Guid":
                    Param = new SqlParameter(string.Format("@{0}", Name), SqlDbType.UniqueIdentifier);

                    Guid GuidValue = (Value == null) ? Guid.Empty : (Guid)Value;

                    if (GuidValue == Guid.Empty)
                    {
                        Param.Value = DBNull.Value;
                    }
                    else
                    {
                        Param.Value = GuidValue;
                    }
                    break;

                case "System.Single":
                    Param = new SqlParameter(string.Format("@{0}", Name), SqlDbType.Real);
                    Param.Value = (Value == null) ? DBNull.Value : Value;
                    break;
                case "System.Byte[]":
                    Param = new SqlParameter(string.Format("@{0}", Name), SqlDbType.VarBinary);
                    Param.Value = (Value == null) ? DBNull.Value : Value;
                    break;

                default:
                    Param = new SqlParameter(string.Format("@{0}", Name), SqlDbType.VarChar);
                    Param.Value = (Value == null) ? DBNull.Value : Value;
                    break;
            }

            return Param;
        }
        #endregion

    }
}
