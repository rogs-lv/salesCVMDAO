using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace salesCVM.Models
{
    public class BusnessPartner
    {
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public int ListNum { get; set; }
        public string Currency { get; set; }
        public string VatGroup { get; set; }
    }
    public class Item 
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public char VATLiable { get; set; }
        public string TaxCodeAR { get; set; }
        public char IndirctTax { get; set; }
        public double Stock { get; set; }
        public string SalUnitMsr { get; set; }
        public decimal Price { get; set; }
        public int ItmsGrpCod { get; set; }
        public string ItmsGrpNam { get; set; }
    }
}
