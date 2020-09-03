using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using salesCVM.Models;

namespace salesCVM.SAP.Interface
{
    public interface ISAPConnect
    {
        bool Conectar(ref string msjCompany, Models.SAP sbo);
    }
}
