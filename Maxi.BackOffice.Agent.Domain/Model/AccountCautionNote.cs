using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxi.BackOffice.Agent.Domain.Model
{
    public class AccountCautionNote
    {
        /// <summary>
        /// Status: INFO, WARN, ERROR
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// Texto de la nota
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// Origen de la nota: MAXI, GIACT
        /// </summary>
        public string Source { get; set; }
        public string SourceText { get; set; }

        public string ResType { get; set; }
        public string ResName { get; set; }

        /// <summary>
        /// Para giact, la fecha de la revision de esta nota
        /// </summary>
        public DateTime? ReviewDate { get; set; }

        /// <summary>
        /// Ayuda a ordenamiento, la prioridad del estatus en numero
        /// </summary>
        public int StatusInt
        {
            get
            {
                int r = 0;
                if (Status == "INFO") r = 0;
                if (Status == "WARN") r = 1;
                if (Status == "ERROR") r = 2;
                return r;
            }
            set { } //importante para que serielice a json
        }

        /// <summary>
        /// Ayuda a ordenamiento, peso del source
        /// </summary>
        public int SourceInt
        {
            get
            {
                int r = 0;
                if (Source.ToUpper() == "GIACT") r = 1;
                return r;
            }
            set { } //importante para que serielice a json
        }

        //Para uso en el front
        public string StatusIcon { get; set; }

        //para guardar el tag del mensaje de texto
        public string TextTag { get; set; }
    }
}
