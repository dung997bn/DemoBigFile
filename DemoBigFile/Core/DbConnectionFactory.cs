using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace DemoBigFile.Core
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly CommonConstants _constants;

        public DbConnectionFactory(IOptions<CommonConstants> constants)
        {
            _constants = constants.Value;
        }

        IDbConnection IDbConnectionFactory.CreateConnection()
        {
            var conn = new SqlConnection(_constants.ConnectionStr);
            conn.Open();
            return conn;
        }
    }
}
