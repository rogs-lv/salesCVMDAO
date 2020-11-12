﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace salesCVM.Models
{
    public class OpportunitySAP
    {
        public Opportunity Header { get; set; }
        public TabsOpportunity Detail { get; set; }
    }
    public class Opportunity
    {
        public int      OpprId        { get; set; }
        public string   Name          { get; set; }
        public string   CardCode      { get; set; }
        public string   CardName      { get; set; }
        public double   CloPrcnt      { get; set; }
        public int      SlpCode       { get; set; }
        public string   SlpName       { get; set; }
        public int      CprCode       { get; set; }
        public int      Territory     { get; set; }
        public DateTime OpenDate      { get; set; }
    }
    public class TabsOpportunity
    {
        public Potencial            TabPotencial { get; set; }
        public General              TabGeneral { get; set; }
        public Etapas               TabEtapa { get; set; }
        public Partner              TabPartner { get; set; }
        public Competidores         TabCompetidor { get; set; }
        public Resumen              TabResumen { get; set; }
        public List<Etapas>         TableEtapas { get; set; }
        public List<Partner>        TablePartner { get; set; }
        public List<Competidores>   TableCompet { get; set; }
        public TabsOpportunity() {
            this.TabPotencial    = new Potencial();
            this.TabGeneral      = new General();
            this.TabEtapa        = new Etapas();
            this.TabPartner      = new Partner();
            this.TabCompetidor   = new Competidores();
            this.TabResumen      = new Resumen();
            this.TableEtapas     = new List<Etapas>();
            this.TablePartner    = new List<Partner>();
            this.TableCompet     = new List<Competidores>();
        }
    }
    public class OptionsHeaderOpp {
        public List<PersonaContacto>    ListPrsContacto { get; set; }
        public List<Territorio>         ListTerritorio  { get; set; }
        public List<Vendedor>           ListVendedor    { get; set; }

        public OptionsHeaderOpp() {
            this.ListPrsContacto    = new List<PersonaContacto>();
            this.ListTerritorio     = new List<Territorio>();
            this.ListVendedor       = new List<Vendedor>();
        }
    }
    public class OptionsTabsDetail
    {
        public List<ProyectoSN>         ListProyectoSN { get; set; }
        public List<FuenteInformacion>  ListaInformacion { get; set; }
        public List<Industria>          ListIndustria { get; set; }
        public List<Vendedor>           ListVendedor { get; set; }
        public List<OpcEtapas>          ListEtapa { get; set; }
        public List<Relacion>           ListRelacion { get; set; }
        public List<Competidores>       ListCompetidor { get; set; }
    }
    public class Potencial 
    {
        public DateTime PredDate    { get; set; }
        public double ClosePrev     { get; set; }
        public double MaxSumLoc     { get; set; }
    }
    public class General
    {
        public string PrjCode      { get; set; }
        public int Source       { get; set; }
        public int Industry     { get; set; }
        public string Memo      { get; set; }
    }
    public class Etapas
    {
        public int          SlpCode         { get; set; }
        public string       SlpName         { get; set; }
        public DateTime     OpenDate        { get; set; }
        public DateTime     CloseDate       { get; set; }
        public int          Step_Id         { get; set; }
        public string       Descript        { get; set; }
        public double       ClosePrcnt      { get; set; }
        public double       WtSumLoc        { get; set; }
        public int          ObjType         { get; set; }
        public int          DocNumber       { get; set;}
        public int          LineNum         { get; set; }
        public Etapas() {
            this.SlpName    = "";
            this.Descript   = "";
        }
    }
    public class OpcEtapas
    {
        public int      Num         { get; set; }
        public string   Descript    { get; set; }
        public int      StepId      { get; set; }
        public double   CloPrcnt    { get; set; }
    }
    public class Partner
    {
        public int ParterId             { get; set; }
        public string Name              { get; set; }
        public int OrlCode              { get; set; }
        public string OrlDesc           { get; set; }
        public string RelatCard         { get; set; }
        public string Memo              { get; set; }
        public Partner() {
            this.ParterId   = 0;
            this.Name       = "";
            this.OrlCode    = 0;
            this.RelatCard  = "";
            this.Memo       = "";
        }
    }
    public class Competidores
    {
        public int CompetId         { get; set; }
        public string NameCompet    { get; set; }
        public string ThreatLevi    { get; set; }
        public string Name          { get; set; }
        public string Memo          { get; set; }
        public char Won             { get; set; }
        public Competidores() {
            this.CompetId   = 0;
            this.NameCompet = "";
            this.ThreatLevi = "";
            this.Name       = "";
            this.Memo       = "";
            this.Won        = ' ';
        }
    }
    public class Resumen
    {
        public char Status       { get; set; }
        public int ReasondId    { get; set; }
        public string Descript     { get; set; }
        public string DocType      { get; set; }
        public string Name         { get; set; }
        public string DocNum       { get; set; }
    }
    public class BusinessP
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
        public string descript { get; set; }
    }
    public class Vendedor
    {
        public int SlpCode { get; set; }
        public string SlpName { get; set; }
    }
    public class ProyectoSN
    {
        public string PrjCode { get; set; }
        public string PrjName { get; set; }
    }
    public class FuenteInformacion
    {
        public int Num { get; set; }
        public string Descript { get; set; }
    }
    public class Industria
    {
        public int IndCode { get; set; }
        public string IndName { get; set; }
        public string IndDesc { get; set; }
    }
    public class OptsEtapas
    {
        public int Num { get; set; }
        public string Descript { get; set; }
        public int StepId { get; set; }
        public double CloPrcnt { get; set; }
    }
    public class Relacion
    {
        public int OrlCode { get; set; }
        public string OrlDesc { get; set; }
    }
    public class Razones
    {
        public int Num { get; set; }
        public string Descript { get; set; }
    }

    public class ActivitySap
    {
        public int                  IdOpp { get; set; }
        public int                  LineOpp { get; set; }
        public int                  ClgCode { get; set; }
        public Action               Actividad { get; set; }
        public Tipo                 Tipo { get; set; }
        public Asunto               Asunto { get; set; }
        public PersonaContacto      Contacto { get; set; }
        public BusinessP            Socio { get; set; }
        public string               Telefono { get; set; }
        public TabGeneral           TabGeneral { get; set; }
        public TabContenido         TabContenido { get; set; }
    }
    public class OptionsHeadActivity 
    {
        public List<Tipo>               ListTipo { get; set; }
        public List<Asunto>             ListAsunto { get; set; }
        public List<PersonaContacto>    ListContacto { get; set; }
        public List<OpcEtapas>             ListEtapa { get; set; }
        public OptionsHeadActivity() {
            this.ListTipo       = new List<Tipo>();
            this.ListAsunto     = new List<Asunto>();
            this.ListContacto   = new List<PersonaContacto>();
            this.ListEtapa      = new List<OpcEtapas>();
        }
    }
    public class OptionsTabsActivity
    {
        public List<Localidad>      ListLocalidad { get; set; }
        public OptionsTabsActivity() {
            this.ListLocalidad  = new List<Localidad>();
        }
    }
    public class TabGeneral
    {
        public DateTime     Recontact { get; set; }
        public DateTime     EndDate { get; set; }
        public string       Priority { get; set; }
        public string       Location { get; set; }
    }
    public class TabContenido
    {
        public string       Notes { get; set; }
    }
    public class Tipo
    {
        public int      Code { get; set; }
        public string   Name { get; set; }
    }
    public class Asunto
    {
        public int      Code { get; set; }
        public string   Name { get; set; }
    }
    public class Localidad
    {
        public int      Code { get; set; }
        public string   Name { get; set; }
    }
    public class Action
    {
        public char     Code        { get; set; }
        public string   Description { get; set; }
    }
}
