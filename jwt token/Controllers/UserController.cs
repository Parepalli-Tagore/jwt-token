using jwt_token.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace jwt_token.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IConfiguration _config;
        private List<user> users = new List<user>()
        {
            new user()
            {
                Id = 1,
                name = "Test",
                phone = "1234",
                password = "1234"
            },
            new user()
            {
                Id = 2,
                name = "Test2",
                phone = "1111",
                password = "1111"
            },
            new user()
            {
                Id = 3,
                name = "Test3",
                phone="2222",
                password="2222"
            },
        };
        public UserController(IConfiguration config)
        {
            _config = config;
        }   
        [HttpPost("login")]
        public ActionResult login(user u)
        {   
            var user1 = users.FirstOrDefault(us => us.phone == u.phone && us.password == u.password);
            if(user1 == null)
            {
                return NotFound();
            }
            var token = GenerateToken(user1.Id, "User");
            return Ok(token);
        }

        [HttpGet("GetUser")]
        [Authorize]
        public user GetUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var claims = identity.Claims;
            int Id = int.Parse(claims.FirstOrDefault(o => o.Type == ClaimTypes.Name).Value);
            string role = claims.FirstOrDefault(o => o.Type == ClaimTypes.Role).Value;
            return users.FirstOrDefault(u => u.Id == Id);
        }

        private string GenerateToken(int id, string role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);



            var claims = new[]
            {
                new Claim(ClaimTypes.Name,id.ToString()),
                new Claim(ClaimTypes.Role,role)
            };
            var token = new JwtSecurityToken(
                    _config["Jwt:Issuer"],
                    _config["Jwt:Audience"],
                    claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        // GET: api/<UserController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<UserController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
