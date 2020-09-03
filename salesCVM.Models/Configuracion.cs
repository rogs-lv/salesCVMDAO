using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace salesCVM.Models
{
    public class Configuracion
    {
        public class Menu {
            public char U_Pedido { get; set; }
            public char U_Cotizacion { get; set; }
            public char U_SN { get; set; }
            public char U_Remision { get; set; }
            public char U_CRM { get; set; }
        }

        public class Adicional
        {
            public char U_CambioPrecio { get; set; }
        }
    }
}
