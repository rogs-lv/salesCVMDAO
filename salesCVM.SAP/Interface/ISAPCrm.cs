using salesCVM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace salesCVM.SAP.Interface
{
    public interface ISAPCrm
    {
        bool CreateOportunity(ref MensajesObj msjCreate, Models.SAP modelo, OpportunitySAP _opp, string Usuario);
        bool UpdateOportunity(ref MensajesObj msjUpd, Models.SAP modelo, OpportunitySAP _opp, string Usuario);
        bool CreateActivity(ref MensajesObj msjCreate, Models.SAP modelo, ActivitySap _act, string Usuario);
        bool UpdateActivity(ref MensajesObj msjUpdate, Models.SAP modelo, ActivitySap _act, string Usuario);
    }
}
