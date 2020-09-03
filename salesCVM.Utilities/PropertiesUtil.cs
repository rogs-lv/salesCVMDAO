using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace salesCVM.Utilities
{
    public class PropertiesUtil
    {
        Log lg;
        public PropertiesUtil() {
            lg = Log.getIntance();
        }

        public NameValueCollection LoadAppSetting()
        {
            try
            {
                NameValueCollection appSettings = ConfigurationManager.AppSettings;
                return appSettings;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return null;
            }
        }
    }
}
