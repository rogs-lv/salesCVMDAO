using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using salesCVM.Models;

namespace salesCVM.SAP.Interface
{
    public interface ISAPMarketing
    {
        bool CreateDocument(ref Mensajes msjCreate, DocSAP document, Models.SAP modelo, int type);

    }
}
