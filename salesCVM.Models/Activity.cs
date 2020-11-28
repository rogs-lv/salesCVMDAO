using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace salesCVM.Models
{
    #region Drop Downs
    public class DropDownActivity {
        public int      Code { get; set; }
        public string   Name { get; set; }

        public DropDownActivity() {
            this.Code = -1;
            this.Name = "";
        }
    }
    public class Contactos
    {
        public int CntctCode { get; set; }
        public string Name { get; set; }
        public string Tel1 { get; set; }
    }
    #endregion

    #region Listas
    public class OportunidadAct {
        public int      OpprId      { get; set; }
        public string   Name        { get; set; }
        public string   CardCode    { get; set; }
    }
    public class SocioNegocioAct {
        public string CardCode { get; set; }
        public string CardName { get; set; }
    }
    public class Actividad {
        public int      ClgCode     { get; set; }
        public string   CardCode    { get; set; }
        public string   CardName    { get; set; }
        public string   Action      { get; set; }
    }
    public class Stage
    {
        public int      Line        { get; set; }
        public string   SlpName     { get; set; }
        public string   Descript    { get; set; }
        public double   ClosePrcnt  { get; set; }
    }
    #endregion

    #region document
    public class ActivitySAP
    {
        public int          OprId       { get; set; }
        public int          OprLine     { get; set; }
        public int          ClgCode     { get; set; }
        public string       Action      { get; set; }
        public int          CntctType   { get; set; }
        public int          CntctSbjct  { get; set; }
        public int          CntctCode   { get; set; }
        public string       CardCode    { get; set; }
        public string       CardName { get; set; }
        public string       Tel         { get; set; }
        public string       Recontact   { get; set; }
        public string       endDate     { get; set; }
        public string       BeginTime   { get; set; }
        public string       ENDTime     { get; set; }
        public int          Priority    { get; set; }
        public int          Location    { get; set; }
        public string       Notes       { get; set; }
        
        public ActivitySAP() {
            this.OprId = -1;
            this.OprLine = -1;
            this.ClgCode = -1;
            this.Action = "";
            this.CntctType = -1;
            this.CntctSbjct = -1;
            this.CntctCode = -1;
            this.CardCode = "";
            this.CardName = "";
            this.Tel = "";
            this.Recontact = "";
            this.endDate = "";
            this.BeginTime = "";
            this.ENDTime = "";
            this.Priority = -1;
            this.Location = -1;
            this.Notes = "";
        }
    }
    #endregion
}
