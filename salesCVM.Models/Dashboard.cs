using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace salesCVM.Models
{
    public class Anuncio
    {
        public int Code { get; set; }
        public int DocEntry { get; set; }
        public string U_Anuncio { get; set; }
        public char U_Tipo { get; set; }
    }

    public class Grafica
    {
        public int U_Mes { get; set; }
        public decimal U_Cuota { get; set; }
        public decimal Ventas { get; set; }
    }
    public class GraficaY
    {

    }
}
