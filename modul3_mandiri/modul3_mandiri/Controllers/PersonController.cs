using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using modul3_mandiri.Models;

namespace modul3_mandiri.Controllers
{
    public class PersonController : Controller
    {
        private string __constr;
        private readonly IConfiguration _configuration;

        public PersonController(IConfiguration configuration)
        {
            __constr = configuration.GetConnectionString("WebApiDatabase");
            _configuration = configuration;
        }

        public IActionResult index()
        {
            return View();
        }

        [HttpPost("api/login")]
        public IActionResult LoginUser(string namaUser, string password)
        {
            var context = new PersonContext(_configuration.GetConnectionString("WebApiDatabase"));

            if (context.IsValidUser(namaUser, password))
            {
                return Ok(new { token = context.GenerateJwtToken(namaUser, _configuration) });
            }

            return Unauthorized();
        }

        [Authorize]

        [HttpGet("api/admin")]

        public ActionResult<Admin> ListPerson()
        {
            PersonContext context = new PersonContext(this.__constr);
            List<Admin> ListPerson = context.ListPerson();
            return Ok(ListPerson);
        }

        [HttpPost("api/admin/create")]
        public IActionResult CreatePerson([FromBody] Admin person)
        {
            PersonContext context = new PersonContext(this.__constr);
            context.AddPerson(person);
            return Ok("Person added successfully.");
        }

        [HttpPut("api/admin/update/{id}")]
        public IActionResult UpdatePerson(int id, [FromBody] Admin person)
        {
            person.id_person = id;
            PersonContext context = new PersonContext(this.__constr);
            context.UpdatePerson(person);
            return Ok("Person updated successfully.");
        }

        [HttpDelete("api/admin/delete/{id}")]
        public IActionResult DeletePerson(int id)
        {
            PersonContext context = new PersonContext(this.__constr);
            context.DeletePerson(id);
            return Ok("Person deleted successfully.");
        }

    }

}