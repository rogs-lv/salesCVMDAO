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
        public double   Rate { get; set; }
        public char     IndirctTax { get; set; }
        public double   Stock { get; set; }
        public string   SalUnitMsr { get; set; }
        public decimal  Price { get; set; }
        public int      ItmsGrpCod { get; set; }
        public string   ItmsGrpNam { get; set; }
        public string   WhsCode { get; set; }
    }

    public class Socios {
        public string   Serie { get; set; }
        public string   CardCode { get; set; }
        public string   CardName { get; set; }
        public char     CardType { get; set; }
        public string   LicTradNum { get; set; }
        public string   Currency { get; set; }
        public string   E_Mail { get; set; }
        public double   Balance { get; set; }
        public string   IntrntSite { get; set; }
        public string   FormaPago { get; set; }
        public string   MetodoPago { get; set; }
        public string   TaxCode { get; set; }
        public double   Rate { get; set; }
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
        public string      ZipCode { get; set; }
    }
    public class Contacto
    {
        public int      CntctCode { get; set; }
        public string   Name { get; set; }
        public string   FirstName { get; set; }
        public string   Title { get; set; }
        public string   MiddleName { get; set; }
        public string   Position { get; set; }
        public string   LastName { get; set; }
        public string   Address { get; set; }
        public string   Tel1 { get; set; }
        public string   Cellolar { get; set; }
        public string   E_MailL { get; set; }
    }
    public class BP
    {
        public Socios               Header { get; set; }
        public List<Direcciones>    TabDireccion { get; set; }
        public List<Contacto>       TabContacto { get; set; }
    }

    public class ItemSAP
    {
        public ItemData Header { get; set; }
        public List<Propiedad> TabsProps { get; set; }
    }
    public class ItemData
    {
        public string       ItemCode { get; set; }
        public string       ItemName { get; set; }
        public int          ItmsGrpCod { get; set; }
        public int          UgpEntry { get; set; }
        public int          ListNum { get; set; }
        public decimal      Price { get; set; }
        public bool         InvntItem { get; set; }
        public bool         SellItem { get; set; }
        public bool         PrchseItem { get; set; }
        public bool         WTLiable { get; set; }
        public bool         VATLiable { get; set; }
        public bool         validFor { get; set; }
        public string       CodeBars { get; set; }
        public string       PicturName { get; set; }
    }
    public class Propiedad
    {
        public int          ItmsTypCod { get; set; }
        public string       ItmsGrpNam { get; set; }
        public bool         Status { get; set; }
    }
    public class Inventario
    {
        public string       WhsCode { get; set; }
        public string       WhsName { get; set; }
        public char         Locked { get; set; }
        public double       OnHand { get; set; }
        public double       IsCommited { get; set; }
        public double       OnOrder { get; set; }
        public double       Disponible { get; set; }
    }
    
    public class Monedas
    {
        public string       CurrCode { get; set; }
        public string       CurrName { get; set; }
    }
    public class PriceList
    {
        public int          ListNum { get; set; }
        public string       ListName { get; set; }
    }
    public class Country
    {
        public string       Code { get; set; }
        public string       Name { get; set; }
    }
    public class State
    {
        public string       Code { get; set; }
        public string       Name { get; set; }
        public string       Country { get; set; }
    }
    public class PrecioArticulo {
        public decimal      Price { get; set; }
        public string       ItemCode { get; set; }
    }

    public class UoM {
        public int UgpEntry { get; set; }
        public string UgpCode { get; set; }
        public string UgpName { get; set; }
    }
    public class GrupoArticulos
    {
        public int ItmsGrpCod { get; set; }
        public string ItmsGrpNam { get; set; }
    }

    public class DocumentNumbering
    {
        public int      Series { get; set; }
        public string   SeriesName { get; set; }
        public int      NextNumber { get; set; }
        public string   Code { get; set; }
        public int      NumSize { get; set; }
    }

    public class FormaPago
    {
        public string IdFormaPago { get; set; }
        public string DescFormaPago { get; set; }
    }

    public class MetodoPago {
        public string IdMetodoPago { get; set; }
        public string DescMetodoPago { get; set; }
    }
}
