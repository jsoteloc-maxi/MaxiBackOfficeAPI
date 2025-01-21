using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxi.BackOffice.CrossCutting.Common.Configurations
{
    public class AppSettings
    {

        public static string ConexionSeguridad
        {
            get { return Parameters.GetKeyValue("ConexionDBSeguridad", false); }
        }

        public static string ConexionOperaciones
        {
            get { return Parameters.GetKeyValue("ConexionDBOperaciones", false); }
        }

        public static string ConnectionString_DbOper
        {
            get { return ConnectionStrings(Parameters.GetKeyValue("ConexionDBOperaciones", false)); }
        }
        public static string ConnectionString_DbLogs
        {
            get { return ConnectionStrings(Parameters.GetKeyValue("ConexionDBLogs", false)); }
        }

        public static string GiactApiUsername
        {
            get { return Parameters.GetKeyValue("GiactApiUsername", false); }
        }
        public static string GiactApiPassword
        {
            get { return Parameters.GetKeyValue("GiactApiPassword", false); }
        }

        public static string CronExpressionJobDiario
        {
            get { return Parameters.GetKeyValue("CronExpressionJobDiario", false); }
        }

        public static string ConnectionStrings(string ConnectionName)
        {
            return Parameters.GetConnectionStrings(ConnectionName);
        }

        public static string ReportMapPath
        {
            get { return Parameters.GetKeyValue("ReportMapPath", false); }
        }

        public static string Lenguaje
        {
            get { return Parameters.GetKeyValue("Lenguaje", false); }
        }
        public static bool Debug
        {
            get { return Convert.ToBoolean(Parameters.GetKeyValue("TSIDebug", false)); }
        }

        public static int CommandTimeout
        {
            get { return Convert.ToInt32(Parameters.GetKeyValue("CommandTimeout", false)); }
        }

        public static bool FacturaOnline
        {
            get { return Convert.ToBoolean(Parameters.GetKeyValue("FacturaOnline", false)); }
        }

    }
}
