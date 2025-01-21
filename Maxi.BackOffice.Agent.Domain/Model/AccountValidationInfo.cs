using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxi.BackOffice.Agent.Domain.Model
{
    public class AccountValidationInfo
    {
        //constructor
        public AccountValidationInfo()
        {
            Notes = new List<AccountCautionNote>();
        }



        public DateTime? CreatedDate { get; set; }

        public string BankName { get; set; } = "";

        public DateTime? AccountAddedDate { get; set; }
        public DateTime? AccountClosedDate { get; set; }
        public DateTime? AccountLastUpdatedDate { get; set; }

        //response.ItemReferenceId = giactResult.ItemReferenceId;
        //response.CreatedDate = giactResult.CreatedDate;
        //response.ErrorMessage = giactResult.ErrorMessage;


        /// <summary>
        /// Obtiene el estatus con mayor peso encontrado en las Notas
        /// </summary>
        public string Status
        {
            get
            {
                string s = "INFO";
                int i = 0;
                foreach (var item in Notes)
                    if (item.StatusInt > i) { s = item.Status; i = item.StatusInt; }
                return s;
            }
            set { } //importante para que serielice a json
        }

        /// <summary>
        /// Lista de notas de revision de cuenta
        /// </summary>
        public List<AccountCautionNote> Notes { get; }


        
        //Para ordenar las notes de mayor a menor severidad
        public void SortNotes()
        {
            Notes.Sort((x, y) =>
            {
                int i;
                int r;

                if (x.SourceInt > y.SourceInt) i = -1; else i = 1;
                if (x.SourceInt == y.SourceInt) i = 0;

                if (x.StatusInt > y.StatusInt) r = -1; else r = 1;
                if (x.StatusInt == y.StatusInt) r = 0;

                return Math.Max(i, r);
            });

            //Notes.Sort((x, y) => y.StatusInt - x.StatusInt);
        }

    }
}
