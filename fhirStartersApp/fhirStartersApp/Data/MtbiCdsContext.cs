using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace fhirStartersApp.Data
{
    public class MtbiCdsContext : IDatabaseContext
    {
        private readonly string _connectionString;

        public MtbiCdsContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        private async Task<MySqlConnection> GetConnectionAsync()
        {
            var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }

        public async Task<object> GetPatientByIdAsync(int id)
        {
            using (var connection = await GetConnectionAsync())
            {
                connection.Ping(); //TODO: this is just for testing the database connection...remove
            }
            return new object(); // return a patient
        }
    }
}
