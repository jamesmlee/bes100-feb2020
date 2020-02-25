using LibraryApi.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryApi.Controllers
{
    public class StatusController : Controller
    {
        [HttpGet("status")]
        public ActionResult<StatusResponse> GetTheStatus()
        {
            var response = new StatusResponse
            {
                Status = "looks good up here, captain",
                CreatedAt = DateTime.Now
            };
            return Ok(response);
        }

        // Resource Archetypes
        // 1. Collection (usually plural, a set of things) /employees
        // 2. Document (singular ... a single thingy)
        // 3. Store (server-maintained cache of client data
        // 4. Controller

        // Getting information INTO the API
        // 1. URL
        //    a. Just plain ole' routing.
        //    b. Embed some variables in the route itself. e.g. GET /employees/657/salary
        
        // employeeId: int ... route constraint. without it, GET /employees/bob/salary will get a response of 0, without it 404 status code
        // employeeId:int:min(1) ... preferred over if(employeeId < 1) { return NotFound() };
        [HttpGet("employees/{employeeId:int}/salary")]
        public ActionResult GetEmployeeSalary(int employeeId)
        {
            //if (employeeId < 1)
            //{
            //    return NotFound();
            //}
            return Ok($"Employee {employeeId} has a salary of $65,000");
        }

        //    c. Add some data to the query string at the end of the URL e.g. GET /shoes?color=blue

        // can't use constraints with query string parameters
        [HttpGet("shoes")]
        public ActionResult GetSomeShoes([FromQuery] string color = "All")
        {
            return Ok($"Getting you shoes of color {color}");
        }

        [HttpGet("whoami")]
        public ActionResult WhoAmI([FromHeader(Name="User-Agent")] string userAgent)
        {
            return Ok($"You are using {userAgent}");
        }

        [HttpGet("employees")]
        public ActionResult GetAllEmployees()
        {
            return Ok("All the employees...");
        }

        [HttpPost("employees")]
        public ActionResult AddAnEmployee([FromBody] NewEmployee employee, [FromServices] IGenerateEmployeeIds idGenerator)
        {
            // new keyword means tightly coupled to a service
            //var idGenerator = new EmployeeIdGenerator();
            // instead, add IGenerateEmployeeIds to Startup.cs in Configure Services
            var id = idGenerator.GetNewEmployeeId();
            return Ok($"Hiring {employee.Name} starting at {employee.StartingSalary.ToString("c")} with id of {id.ToString()}");
        }
        /* Body
         {
	        "name": "Bob Smith",
	        "startingSalary": 52000
         } 
        */
    }

    // after copying JSON, go to Edit --> Paste Special --> Paste JSON As Classes
    public class NewEmployee
    {
        public string Name { get; set; }
        public decimal StartingSalary { get; set; }
    }

    public class StatusResponse
    {
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }


}
