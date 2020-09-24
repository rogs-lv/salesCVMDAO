using salesCVM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace salesCVM.SAP.Interface
{
    public interface ISAPMasterData
    {
        bool CreateBusnessPartner(ref MensajesObj msjCreate, Models.SAP modelo, BP socio, string Usuario);
        bool UpdateBusnessPartner(ref MensajesObj msjUpd, Models.SAP modelo, BP socio, string Usuario);
        bool CreateItem(ref MensajesObj msjCreate, Models.SAP modelo, ItemData item, List<Propiedad> propiedades, List<Inventario> inventario);
        bool UpdateItem(ref MensajesObj msjCreate, Models.SAP modelo, ItemData item, Propiedad propiedades, Inventario inventario);
    }
}
