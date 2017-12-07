using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fhirStartersApp.Data
{
    public class ManagementPlan
    {
        public ManagementPlan(byte[] fileContents, string fileExtension)
        {
            FileContents = fileContents;
            FileExtension = fileExtension;
        }

        public byte[] FileContents { get; }
        public string FileExtension { get; }
    }
}
