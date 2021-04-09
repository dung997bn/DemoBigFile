using Dapper;
using DemoBigFile.Core;
using DemoBigFile.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var sql = @"INSERT INTO donation( id, type_id, class_id, source_id, parent_id, elec_code, senator_id, state, no_history,
                                             image_url, year, month,day ,amount, total_number,company_name,representative_name,
                                             postal_code,address,occupation_id,deduction, missed_message, created_at, created_user_id, 
                                             created_process, updated_at, updated_user_id, updated_process ,deleted_at, 
                                             deleted_user_id, deleted_process, times_object_last_batch_process, batch_processing, receipt_no)
	                    VALUES (@id, @type_id, @class_id, @source_id, @parent_id, @elec_code, @senator_id, @state, @no_history,
                                             @image_url, @year, @month, @day , @amount, @total_number, @company_name, @representative_name,
                                             @postal_code,address, @occupation_id, @deduction, @missed_message, @created_at, @created_user_id, 
                                             @created_process, @updated_at, @updated_user_id, @updated_process , @deleted_at, 
                                             @deleted_user_id, @deleted_process, @times_object_last_batch_process, @batch_processing, @receipt_no)";
            using (var conn = _dbConnection.CreateConnection())
            {
                var res = conn.Execute(sql, new
                {
                    id = model.id,
                    type_id = model.type_id,
                    class_id = model.class_id,
                    source_id = model.source_id,
                    parent_id = model.parent_id,
                    elec_code = model.elec_code,
                    senator_id = model.senator_id,
                    state = model.state,
                    no_history = model.no_history,
                    image_url = model.image_url,
                    year = model.year,
                    month = model.month,
                    day = model.day,
                    amount = model.amount,
                    total_number = model.total_number,
                    company_name = model.company_name,
                    representative_name = model.representative_name,
                    postal_code = model.postal_code,
                    address = model.address,
                    occupation_id = model.occupation_id,
                    deduction = model.deduction,
                    missed_message = model.missed_message,
                    created_at = model.created_at,
                    created_user_id = model.created_user_id,
                    created_process = model.created_process,
                    updated_at = model.updated_at,
                    updated_user_id = model.updated_user_id,
                    updated_process = model.updated_process,
                    deleted_at = model.deleted_at,
                    deleted_user_id = model.deleted_user_id,
                    deleted_process = model.deleted_process,
                    times_object_last_batch_process = model.times_object_last_batch_process,
                    batch_processing = model.batch_processing,
                    receipt_no = model.receipt_no
                });
            }
        }
    }
}
