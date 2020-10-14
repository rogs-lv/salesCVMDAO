using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace salesCVM.Models
{
    public class DocSAP {
        public Document Header { get; set; }
        public List<DocumentLines> Detail { get; set; }
    }

    public class Document {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public DateTime DocDate { get; set; }
        public string Reference { get; set; }
        public string Comments { get; set; }
        public char Status { get; set; }
        public int DocEntrySAP { get; set; }
        public int DocNumSAP { get; set; }
    }

    public class DocumentLines {
        public int DocEntry { get; set; }
        public int LineNum { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public double Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal UnitePrice { get; set; }
        public double Discount { get; set; }
        public string Currency { get; set; }
        public string TaxCode { get; set; }
        public double Rate { get; set; }
        public string WhsCode { get; set; }
    }
}
