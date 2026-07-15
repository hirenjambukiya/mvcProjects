using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELMS.Models.Entities;

namespace ELMS.Services.Interfaces
{
    public interface ILoging
    {
        mst_users GetUserByUserName(string userName);
        void InsertUser(mst_users user);
    }
}
