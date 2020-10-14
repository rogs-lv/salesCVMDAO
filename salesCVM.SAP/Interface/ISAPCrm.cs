using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace salesCVM.SAP.Interface
{
    public interface ISAPCrm
    {
        bool CreateOportunity();
        bool UpdateOportunity();
        bool CreateActivitie();
        bool UpdateActivitie();
    }
}
