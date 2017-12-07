using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace fhirStartersApp.Data
{
    public interface IDatabaseContext
    {
        Task<object> GetPatientByIdAsync(int id);
    }
}
