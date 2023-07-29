using Dapper;
using DemoBigFile.Core;
using DemoBigFile.Models;
using DemoBigFile.Models.RelationalModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DemoBigFile.Extensions;

namespace DemoBigFile.Repository
{
    public class Repo : IRepository
    {
        IDbConnectionFactory _dbConnection;

        public Repo(IDbConnectionFactory dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public void Create(DonationViewModel model)
        {
            try
            {
                var sql = @"SET IDENTITY_INSERT donation ON
                        INSERT INTO donation( id, parent_id, elec_code, senator_id, amount, representative_name,
                                             postal_code, address, occupation_id, receipt_no)
	                    VALUES (@id, @parent_id, @elec_code, @senator_id, @amount, @representative_name,
                                             @postal_code, @address, @occupation_id, @receipt_no)";
                using (var conn = _dbConnection.CreateConnection())
                {
                    var res = conn.Execute(sql, new
                    {
                        id = model.id,
                        parent_id = model.parent_id,
                        elec_code = model.elec_code,
                        senator_id = model.senator_id,
                        amount = model.amount,
                        representative_name = model.representative_name,
                        postal_code = model.postal_code,
                        address = model.address,
                        occupation_id = model.occupation_id,
                        receipt_no = model.receipt_no
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public void MergeTable()
        {
            try
            {
                using (var conn = _dbConnection.CreateConnection())
                {
                    var res = conn.Execute("MergeTableDonation", commandType: System.Data.CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task DemoRelationalDataInsert(DataTable product, DataTable tableProductVariant)
        {
            try
            {
                using (var connection = new SqlConnection("Server=HALO-KIRIN;Database=test_bigfile;User Id=sa;Password=cntt;MultipleActiveResultSets=true"))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("sp_demo_relational_Data", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    //Pass table Valued parameter to Store Procedure
                    SqlParameter sqlParam1 = cmd.Parameters.AddWithValue("@ProductTable", product);
                    sqlParam1.SqlDbType = SqlDbType.Structured;
                    SqlParameter sqlParam2 = cmd.Parameters.AddWithValue("@ProductVariantTable", tableProductVariant);
                    sqlParam2.SqlDbType = SqlDbType.Structured;
                   await  cmd.ExecuteNonQueryAsync();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }
    }
}
