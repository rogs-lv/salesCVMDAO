using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace salesCVM.DAO.Implements
{
    public class HanaAdapter : IDBAdapter
    {
        public IDbConnection GetConnection() {
            return null;
        }
    }
}
