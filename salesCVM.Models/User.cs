﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace salesCVM.Models
{
    public class User
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string U_CardCode { get; set; }
        public string U_CardName { get; set; }
        public string U_SlpCode { get; set; }
        public string U_SlpName { get; set; }
        public char U_CambioPrecio { get; set; }
        public char U_CambioSN { get; set; }
        public decimal U_PrcntjDescMax { get; set; }
        public string TaxCode { get; set; }
        public int ListNum { get; set; }
        public string WhsCode { get; set; }
        public double Rate { get; set; }
        public string U_Sucursal { get; set; }
        public string Token { get; set; }
    }

    public class UserLogin {
        public string IdUser { get; set; }
        public string Password { get; set; }
    }

}
