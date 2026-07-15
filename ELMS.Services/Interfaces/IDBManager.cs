using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELMS.Services.Interfaces
{
    public interface IDBManager
    {
        IDbConnection CreateConnection(string ConKey);

    }
}
