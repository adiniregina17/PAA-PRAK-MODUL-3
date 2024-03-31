using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tugaspr_modul3.Models;

namespace tugaspr_modul3.Controllers
{
    public class PersonController : Controller
    {
        private string __constr;

        public PersonController(IConfiguration configuration)
        {
            __constr = configuration.GetConnectionString("WebApiDatabase");
        }

        public IActionResult index()
        {
            return View();
        }


        [HttpGet("api/person")]

        public ActionResult<Person> ListPerson()
        {
            PersonContext context = new PersonContext(__constr);
            List<Person> ListPerson = context.ListPerson();
            return Ok(ListPerson);
        }

        [HttpPost("api/person_auth"), Authorize]

        public ActionResult<Person> ListPersonWithAuth()
        {
            PersonContext context = new PersonContext(this.__constr);
            List<Person> ListPerson = context.ListPerson();
            return Ok(ListPerson);
        }
    }

}