using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DemoBigFile.Core
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
