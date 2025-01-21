using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxi.BackOffice.Agent.Domain.Model
{
    //Todo: Renombrar ValidateCheckImageRequest

    public class ObjetoRequestItem
    {
        public string Metodo { get; set; }//Se debe utilizar un ENUM para definir si es un batch o individual
        public string ImageName { get; set; }
        public byte[] ImageBytes { get; set; }

        public int IdcheckImagePending { get; set; }
        public byte[] ImageBytesRear { get; set; }
        public string Lang { get; set; }

        //Todo: agregar campos para mas datos, ej para IRD que no requiere orbo

        //public string Micr { get; set; }
        //public string Routing { get; set; }
        //public string Account { get; set; }
        //public string CheckNum { get; set; }

        //public string ObjectType { get; set; }
    }
}
