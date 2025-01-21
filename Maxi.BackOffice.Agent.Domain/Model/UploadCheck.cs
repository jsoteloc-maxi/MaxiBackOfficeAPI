using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxi.BackOffice.Agent.Domain.Model
{
    public class UploadCheck
    {
        public int IdCheck { get; set; }

        public int IdIssuer { get; set; }

        public Guid IdImage { get; set; }

        public int IdCheckPendingImage { get; set; }
    }
}
