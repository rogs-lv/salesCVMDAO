using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace salesCVM.Models
{
    public class Opportunity
    {
        public int OpprId           { get; set; }
        public string Name          { get; set; }
        public string CardCode      { get; set; }
        public string CardName      { get; set; }
        public double CloPrcnt      { get; set; }
        public int SlpCode          { get; set; }
        public string SlpName       { get; set; }
        public int CprCode          { get; set; }
        public int Territory        { get; set; }
        public DateTime OpenDate    { get; set; }
    }
    public class Potencial 
    {
        public DateTime PredDate    { get; set; }
        public double ClosePrev     { get; set; }
        public double MaxSumLoc     { get; set; }
    }
    public class General
    {
        public int PrjCode      { get; set; }
        public int Source       { get; set; }
        public int Industry     { get; set; }
        public string Memo      { get; set; }
    }
    public class Etapas
    {
        public int SlpCode              { get; set; }
        public string SlpName           { get; set; }
        public DateTime OpenDate        { get; set; }
        public DateTime CloseDate       { get; set; }
        public int Step_Id              { get; set; }
        public string Descript          { get; set; }
        public double ClosePrcnt        { get; set; }
        public double WtSumLoc          { get; set; }
        public int ObjType              { get; set; }
        public int DocNumber            { get; set;}
    }
    public class Partner
    {
        public int ParterId             { get; set; }
        public string Name              { get; set; }
        public int OrlCode              { get; set; }
        public string OrlDesc           { get; set; }
        public string RelatCard         { get; set; }
        public string Memo              { get; set; }
    }
    public class Competidores
    {
        public int CompetId         { get; set; }
        public string NameCompet    { get; set; }
        public string ThreatLevi    { get; set; }
        public string Name          { get; set; }
        public string Memo          { get; set; }
        public char Won             { get; set; }
    }
    public class Resumen
    {
        public string Status       { get; set; }
        public string ReasondId    { get; set; }
        public string Descript     { get; set; }
        public string DocType      { get; set; }
        public string Name         { get; set; }
        public string DocNum       { get; set; }
    }
    public class BussnessPartner
    {
        public string CardCode { get; set; }
        public string CardName { get; set; }
    }

    public class PersonaContacto
    {
        public int  CntctCode   { get; set; }
        public string Name      { get; set; }
    }
    public class Territorio
    {
        public int territryID { get; set; }
        public string descrpt { get; set; }
    }
    public class Vendedor
    {
        public int SlpCode { get; set; }
        public string SlpName { get; set; }
    }

    public class ProyectoSN
    {

    }
    public class FuenteInformacion
    {

    }
    public class Industria
    {

    }

    public class OptsEtapas
    {

    }
}
