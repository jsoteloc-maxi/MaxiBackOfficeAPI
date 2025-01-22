using System.Configuration;
using Microsoft.Data.SqlClient;


namespace Maxi.BackOffice.CrossCutting.Common.Configurations
{
    public class Parameters
    {
        /// <summary>
        /// Get value of the key in the AppSettings
        /// </summary>
        /// <param name="key">name of the key</param>
        /// <param name="throwException">if set as true,an exception is generated</param>
        /// <returns></returns>

        public static string GetKeyValue(string key, bool throwException = true)
        { 
            string keyValue = GetValue(ConfigurationManager.AppSettings[key]);
            if (string.IsNullOrEmpty(keyValue) && throwException)
                throw new Exception(string.Format("No found key {0}", key));
            return keyValue;
        }

        /// <summary>
        /// Get value of the object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>

        public static string GetValue(object obj)
        {
            if (obj != null)
                return obj.ToString();
            else
                return string.Empty;
        }


        public static string GetConnectionStrings(string Name)
        {
            try
            {
                string connstr = ConfigurationManager.ConnectionStrings[Name].ConnectionString;
                var connnectionStringBuilder = new SqlConnectionStringBuilder(connstr);

                return connnectionStringBuilder.ToString();
            }
            catch (Exception ex)
            { throw new Exception("Parameters.GetConnectionStrings: ", ex); }

            //return Encript.Decrypt(Conexion, "8AAAC2BF-0F0E-459D-8289-2DF52216B8F3");
        }


    }
}
