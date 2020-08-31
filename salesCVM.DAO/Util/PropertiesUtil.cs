using Authlete.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using salesCVM.Utilities;

namespace salesCVM.DAO.Util
{
    public class PropertiesUtil
    {
        public static IDictionary<string, string> LoadProperty(string URL) {
            try
            {
                IDictionary<string, string> prop = null;
                using (TextReader reader = new StreamReader(URL))
                {
                    prop = PropertiesLoader.Load(reader);
                }
                return prop;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
