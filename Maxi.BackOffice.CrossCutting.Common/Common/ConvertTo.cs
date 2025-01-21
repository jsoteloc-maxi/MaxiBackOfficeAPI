using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Maxi.BackOffice.CrossCutting.Common.Common
{
    public static class ConvertTo
    {
 
        public static object Datatype(object dato, int Tipo)
        {
            switch (Tipo)
            {
                case 1:
                    string Texto = "";
                    if (dato != null)
                        Texto = dato.ToString();

                    return Texto;
                case 2:
                    int Numerico = 0;
                    if (dato != null)
                    {
                        if (dato.ToString() != "")
                            Numerico = Convert.ToInt32(dato);
                    }
                    return Numerico;
                case 3:
                    DateTime Fecha = DateTime.Now;
                    if (dato != null)
                    {
                        if (dato.ToString() != "")
                            Fecha = Convert.ToDateTime(dato);
                    }
                    return Fecha;
                case 4:
                    int SINO = 0;
                    if (dato != null)
                    {
                        if (dato.ToString() == "true")
                            SINO = 1;
                        else
                            SINO = 0;
                    }
                    return SINO;
                default:
                    string TextoDefaul = "";
                    if (dato != null)
                        TextoDefaul = dato.ToString();
                    return TextoDefaul;
            }

        }

        
        public static string Tobool(object Dato)
        {
            if (Dato.ToString() == "1")
                return "true";
            else
                return "false";

        }

    
        public static List<T> ToList<T>(this DataTable table) where T : class, new()
        {
            IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            List<T> result = new List<T>();

            foreach (var row in table.Rows)
            {
                var item = CreateItemFromRow<T>((DataRow)row, properties);
                result.Add(item);
            }

            return result;
        }
        public static List<T> DatableToList<T>(this T Entidad, DataTable table) where T : class, new()
        {
            IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            List<T> result = new List<T>();

            foreach (var row in table.Rows)
            {
                var item = CreateItemFromRow<T>((DataRow)row, properties);
                result.Add(item);
            }

            return result;
        }

    
        public static T ToClass<T>(this T Entidad, DataRow row) where T : class, new()
        {
            IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();

            T item = new T();
            foreach (var property in properties)
            {
                if (row.Table.Columns.Contains(property.Name))
                {
                    if (property.PropertyType == typeof(System.DayOfWeek))
                    {
                        DayOfWeek day = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), row[property.Name].ToString());
                        property.SetValue(item, day, null);
                    }
                    else
                    {
                        property.SetValue(item, row[property.Name], null);
                    }
                }
            }
            return item;
        }

    
        public static T CreateItemFromRow<T>(DataRow row, IList<PropertyInfo> properties) where T : class, new()
        {
            T item = new T();
            foreach (var property in properties)
            {
                if (row.Table.Columns.Contains(property.Name))
                {
                    if (row[property.Name] != System.DBNull.Value && row[property.Name] != null)
                    {
                        property.SetValue(item, row[property.Name], null);
                    }
                }
            }
            return item;
        }
        public static T CreateItemFromRow2<T>(DataRow row, IList<PropertyInfo> properties) where T : class, new()
        {
            T item = new T();
            foreach (var property in properties)
            {
                if (row.Table.Columns.Contains(property.Name))
                {
                    if (row[property.Name] != System.DBNull.Value && row[property.Name] != null)
                    {
                        switch (row.Table.Columns[property.Name].DataType.FullName)
                        {
                            case "System.DateTime":
                                property.SetValue(item, Convert.ToDateTime(row[property.Name]), null);
                                break;
                            case "System.String":
                                property.SetValue(item, Convert.ToString(row[property.Name]), null);
                                break;
                            case "System.Boolean":
                                property.SetValue(item, Convert.ToBoolean(row[property.Name]), null);
                                break;
                            case "System.Decimal":
                                property.SetValue(item, Convert.ToDecimal(row[property.Name]), null);
                                break;
                            case "System.Double":
                                property.SetValue(item, Convert.ToDouble(row[property.Name]), null);
                                break;
                            case "System.Int16":
                                property.SetValue(item, Convert.ToInt16(row[property.Name]), null);
                                break;
                            case "System.Int32":
                                property.SetValue(item, Convert.ToInt32(row[property.Name]), null);
                                break;
                            case "System.Int64":
                                property.SetValue(item, Convert.ToInt64(row[property.Name]), null);
                                break;
                            default:
                                property.SetValue(item, row[property.Name], null);
                                break;
                        }


                    }
                }
            }
            return item;
        }

        public static DataTable ListToDataTable<T>(this IList<T> data, string TableName, string DataSetName, List<string> FieldsInclude)
        {
            PropertyDescriptorCollection props =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            table.TableName = TableName;
            table.Namespace = DataSetName;
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                if (FieldsInclude.Contains(prop.Name))
                {
                    table.Columns.Add(prop.Name, prop.PropertyType);
                }
            }

            foreach (T item in data)
            {
                DataRow Registro = table.NewRow();
                for (int i = 0; i < props.Count; i++)
                {
                    PropertyDescriptor prop = props[i];
                    if (FieldsInclude.Contains(prop.Name))
                    {
                        Registro[prop.Name] = props[i].GetValue(item);
                    }
                }
                table.Rows.Add(Registro);
            }
            return table;
        }

        public static DataTable ListToDataTable<T>(this IList<T> data, string TableName, string DataSetName)
        {
            PropertyDescriptorCollection props =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            table.TableName = TableName;
            table.Namespace = DataSetName;
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];

                table.Columns.Add(prop.Name, prop.PropertyType);

            }

            foreach (T item in data)
            {
                DataRow Registro = table.NewRow();
                for (int i = 0; i < props.Count; i++)
                {
                    PropertyDescriptor prop = props[i];

                    Registro[prop.Name] = props[i].GetValue(item);

                }
                table.Rows.Add(Registro);
            }
            return table;
        }
        public static DataTable ToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
               TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;

        }
        public static DataTable ToDataTable<T>(this T data, string TableName, string DataSetName, List<string> FieldsInclude)
        {
            PropertyDescriptorCollection props =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            table.TableName = TableName;
            table.Namespace = DataSetName;
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                if (FieldsInclude.Contains(prop.Name))
                {
                    table.Columns.Add(prop.Name, prop.PropertyType);
                }
            }
            DataRow Registro = table.NewRow();

            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                if (FieldsInclude.Contains(prop.Name))
                {
                    Registro[prop.Name] = props[i].GetValue(data);
                }
            }
            table.Rows.Add(Registro);

            return table;
        }

        public static DataTable ToDataTable<T>(this T data, string TableName, string DataSetName)
        {
            PropertyDescriptorCollection props =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            table.TableName = TableName;
            table.Namespace = DataSetName;
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }
            DataRow Registro = table.NewRow();

            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                Registro[prop.Name] = props[i].GetValue(data);
            }
            table.Rows.Add(Registro);

            return table;
        }

        public static DataTable CreateDataTable<T>(IEnumerable<T> list)
        {
            Type type = typeof(T);
            var properties = type.GetProperties();

            DataTable dataTable = new DataTable();
            foreach (PropertyInfo info in properties)
            {
                dataTable.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
            }

            foreach (T entity in list)
            {
                object[] values = new object[properties.Length];
                for (int i = 0; i < properties.Length; i++)
                {
                    values[i] = properties[i].GetValue(entity);
                }

                dataTable.Rows.Add(values);
            }
            return dataTable;
        }

        /// <summary>
        /// Serializa un objeto a JSON
        /// </summary>
        /// <typeparam name="T">Tipo de objeto</typeparam>
        /// <param name="obj">objeto</param>
        /// <returns>JSON del objeto serializado</returns>
        public static string ToJSON<T>(T obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, obj);
            string retVal = Encoding.UTF8.GetString(ms.ToArray());
            return retVal;
        }

        /// <summary>
        /// Deserializa un objeto desde JSON
        /// </summary>
        /// <typeparam name="T">Tipo de objeto</typeparam>
        /// <param name="json">JSON a deserializar</param>
        /// <returns>Objeto deserializado</returns>
        public static T FromJSON<T>(string json)
        {
            T obj = Activator.CreateInstance<T>();
            MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            obj = (T)serializer.ReadObject(ms);
            ms.Close();
            return obj;
        }
    }
}
