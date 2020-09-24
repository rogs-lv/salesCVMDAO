using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace salesCVM.Models
{
    public class BusnessPartner
    {
        public string   CardCode { get; set; }
        public string   CardName { get; set; }
        public int      ListNum { get; set; }
        public string   Currency { get; set; }
        public string   VatGroup { get; set; }
    }
    public class Item 
    {
        public string   ItemCode { get; set; }
        public string   ItemName { get; set; }
        public char     VATLiable { get; set; }
        public string   TaxCodeAR { get; set; }
        public char     IndirctTax { get; set; }
        public double   Stock { get; set; }
        public string   SalUnitMsr { get; set; }
        public decimal  Price { get; set; }
        public int      ItmsGrpCod { get; set; }
        public string   ItmsGrpNam { get; set; }
        public string   WhsCode { get; set; }
    }

    public class Socios {
        public string   Series { get; set; }
        public string   CardCode { get; set; }
        public string   CardName { get; set; }
        public char     CardType { get; set; }
        public string   LicTradNum { get; set; }
        public string   Currency { get; set; }
        public string   E_Mail { get; set; }
    }
    public class Direcciones {
        public int      LineNum { get; set; }
        public string   Address { get; set; }
        public char     AdresType { get; set; }
        public string   Country { get; set; }
        public string   County { get; set; }
        public string   City { get; set; }
        public string   State { get; set; }
        public string   Block { get; set; }
        public string   Street { get; set; }
        public string   StreetNo { get; set; }
        public int      ZipCode { get; set; }
    }
    public class BP
    {
        public Socios               Header { get; set; }
        public List<Direcciones>    TabDireccion { get; set; }
    }

    public class ItemData
    {
        public string       ItemCode { get; set; }
        public string       ItemName { get; set; }
        public int          ItmsGrpCod { get; set; }
        public int          UgpEntry { get; set; }
        public int          ListNum { get; set; }
        public decimal      Price { get; set; }
        public char         InvntItem { get; set; }
        public char         SellItem { get; set; }
        public char         PrchseItem { get; set; }
        public char         WTLiable { get; set; }
        public char         VATLiable { get; set; }
        public char         validFor { get; set; }
    }
    public class Propiedad
    {
        public int ItmsTypCod { get; set; }
        public string ItmsGrpNam { get; set; }
        public bool Status { get; set; }
    }
    public class Inventario
    {
        public string WhsCode { get; set; }
        public string WhsName { get; set; }
        public char Locked { get; set; }
        public double OnHand { get; set; }
    }
    
    public class Monedas
    {
        public string CurrCode { get; set; }
        public string CurrName { get; set; }
    }
    public class PriceList
    {
        public int ListNum { get; set; }
        public string ListName { get; set; }
    }
    public class Country
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
    public class State
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
    }
}
