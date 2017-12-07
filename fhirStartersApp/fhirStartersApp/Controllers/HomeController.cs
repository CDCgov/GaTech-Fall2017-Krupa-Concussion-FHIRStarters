using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using fhirStartersApp.Data;
using Microsoft.AspNetCore.Mvc;

namespace fhirStartersApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDatabaseContext _databaseContext;

        public HomeController(IDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<IActionResult> Index()
        {
            await TestConnectionAsync();
            return View();
        }

        private async Task TestConnectionAsync()
        {
            var patient = await _databaseContext.GetPatientByIdAsync(0);
        }

        public IActionResult Error()
        {
            ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }
    }
}
