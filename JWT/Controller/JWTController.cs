using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JWT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace JWT.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class JWTController : ControllerBase
    {
        private readonly WebapicoreContext _context;
        private readonly Jwtset jwt;
        public JWTController(WebapicoreContext context, IOptions<Jwtset> options)
        {
            _context = context;
            jwt = options.Value;
        }
        [HttpGet]
        [Route("details-get")]
        [Authorize]
        public IActionResult get()
        {
            var details = _context.Firstapitbs.ToArray();
            return Ok(new
            {
                data = details.ToArray()
            });
        }
        [HttpPost("Authentication")]
        public IActionResult Authenticate([FromBody] Jwtcredentials uc)
        {
            var user = validateuser(uc.username, uc.Password);

            if (user == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            var token = generatetoken(user.Username);
            return Ok(token);
        }
        [NonAction]
        public Userlogin validateuser(string username, string password)
        {
            var user = _context.Userlogins.FirstOrDefault(s =>s.Username== username && s.Password==password);

            return user;
        }

[NonAction]
        public string generatetoken(string username)
        {
            var tokenhandler = new JwtSecurityTokenHandler();

            var tokenkey = Encoding.UTF8.GetBytes(this.jwt.securitykey);
            if (tokenkey.Length < 16)
            {
                return "Key length is insufficient.";
            }
            var tokendesc = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new Claim[] { new Claim(ClaimTypes.Name, username) }),
                Expires = DateTime.Now.AddSeconds(120),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenkey), SecurityAlgorithms.HmacSha256)
            };
            var token = tokenhandler.CreateToken(tokendesc);
            string finaltoken = tokenhandler.WriteToken(token);
            return finaltoken;

        }
        // [HttpPost("Authentication")]
        // public IActionResult Authenticate([FromBody]Jwtcredentials uc )
        // {
        //     var user =_context.Userlogins.ToList().FirstOrDefault(s => string.Equals(s.Username,uc.username,StringComparison.OrdinalIgnoreCase) && string.Equals(s.Password,uc.Password,StringComparison.OrdinalIgnoreCase));
        //     if (user == null)
        //     {
        //         return Unauthorized();
        //     }
        //     var tokenhandler = new JwtSecurityTokenHandler();
            
        //     var tokenkey = Encoding.UTF8.GetBytes(this.jwt.securitykey);
        //     if (tokenkey.Length < 16)
        //     {
        //         return BadRequest("Key length is insufficient.");
        //     }
        //     var tokendesc = new SecurityTokenDescriptor
        //     {
        //         Subject = new ClaimsIdentity(
        //             new Claim[] { new Claim(ClaimTypes.Name, user.Username) }),
        //         Expires = DateTime.Now.AddSeconds(120),
        //         SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenkey), SecurityAlgorithms.HmacSha256)
        //     };
        //     var token = tokenhandler.CreateToken(tokendesc);
        //     string finaltoken = tokenhandler.WriteToken(token);
        //     return Ok(finaltoken);

        // }




    }
}