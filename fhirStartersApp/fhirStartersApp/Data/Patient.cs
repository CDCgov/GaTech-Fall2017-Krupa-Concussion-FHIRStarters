using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fhirStartersApp.Data
{
    public class Patient
    {
        public Age Age { get; set; }
        public Observations Data { get; set; }
    }
}
