using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using salesCVM.Models;
using salesCVM.Utilities;
using System.Collections.Specialized;
using salesCVM.SAP.Interface;

namespace salesCVM.SAP
{
    public class SAPConnect : ISAPConnect
    {
        private Company oCompany;
        Log lg;
        public SAPConnect() {
            oCompany    = new Company();
            lg          = Log.getIntance();
        }

        public bool Conectar(ref string msjCompany, Models.SAP sbo) {
            try
            {
                oCompany.Server         = sbo.Server;
                oCompany.DbServerType   = ServerType(sbo.DbServerType);
                oCompany.LicenseServer  = sbo.SLDServer;
                oCompany.CompanyDB      = sbo.CompanyDB;
                oCompany.UserName       = sbo.UserName;//"manager";
                oCompany.Password       = sbo.Password;//"12345";
                oCompany.DbUserName     = sbo.DbUserName;// "sa";
                oCompany.DbPassword     = sbo.DbPassword;//"orosas$$";
                oCompany.language       = Lenguage(sbo.language);
                oCompany.UseTrusted     = false;

                if (oCompany.Connect() != 0)
                {
                    msjCompany = $"{oCompany.GetLastErrorCode()} {oCompany.GetLastErrorDescription()}";
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return false;
            }
        }
        public Company GetCompany() {
            return oCompany;
        }
        private BoDataServerTypes ServerType(string typeAppSetting) {
            switch (typeAppSetting)
            {
                case "2008":
                    return BoDataServerTypes.dst_MSSQL2008;
                case "2012":
                    return BoDataServerTypes.dst_MSSQL2012;
                case "2014":
                    return BoDataServerTypes.dst_MSSQL2014;
                case "2016":
                    return BoDataServerTypes.dst_MSSQL2016;
                default:
                    return BoDataServerTypes.dst_HANADB;
            }
        }
        private BoSuppLangs Lenguage(string lenguage = "") {
            switch (lenguage)
            {
                case "EN":
                    return BoSuppLangs.ln_English;
                default:
                    return BoSuppLangs.ln_Spanish;
            }
        }
    }
}
