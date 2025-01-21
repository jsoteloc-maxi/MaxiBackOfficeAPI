using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxi.BackOffice.CrossCutting.Common.Attributes
{
    public class EntityAtributes : System.Attribute
    {
        private string tablename;
        private string schema;
        private string conexionName;



        public EntityAtributes()
        {
            schema = "dbo";
            tablename = "NoTablename";
            conexionName = "DefultDB";
        }

        /// <summary>
        /// Nombre de Tabla En la Base  de Datos
        /// </summary>
        public string Tablename
        {
            get { return tablename; }
            set { tablename = value; }
        }

        /// <summary>
        /// Nombre del Eschema al cual pertenece la Tabla  por defecto se asigna dbo
        /// </summary>
        public string Schema
        {
            get { return schema; }
            set { schema = value; }
        }

        /// <summary>
        /// Nombre de La conexion Ubicada en el Archivo de Configuracion
        /// </summary>
        public string ConexionName
        {
            get { return conexionName; }
            set { conexionName = value; }
        }

    }
}
