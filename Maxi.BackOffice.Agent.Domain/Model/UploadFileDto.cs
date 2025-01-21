using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxi.BackOffice.Agent.Domain.Model
{
    public class UploadFileDto
    {
        #region Fields

        #endregion

        public int Id { get; set; }

        public int IdReference { get; set; }

        public int IdDocumentType { get; set; }

        public string DocumentTypeName { get; set; }

        public string FileName { get; set; }

        public string FileGuid { get; set; }

        public string Extension { get; set; }

        public int IdStatus { get; set; }

        public int IdUser { get; set; }

        public DateTime LastChangeDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public DateTime CreationDate { get; set; }

        public int idImgType { get; set; }

        public int? IdCountry { get; set; }

        public int? IdState { get; set; }
    }
}
