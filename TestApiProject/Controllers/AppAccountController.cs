using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TestApiProject.DataContext;
using TestApiProject.Entities;
using TestApiProject.ViewModel;
using TestAspNetProject.Models;

namespace TestApiProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppAccountController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<Customer> _userManager;
        private readonly SignInManager<Customer> _signInManager;

        private readonly ApplicationDbContext _context;
        public AppAccountController(IConfiguration configuration,
                                    UserManager<Customer> userManger,
                                    SignInManager<Customer> signInManager,
                                    ApplicationDbContext context)
        {
            _configuration = configuration;
            _userManager = userManger;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpPost("/api/customer")]
        [AllowAnonymous]
        public async Task<ActionResult<string>> CreateCustomer(CustomersViewModel user)
        {
            var applicationUser = new Customer
            {
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
            };


            IdentityResult result = await _userManager.CreateAsync(applicationUser, user.Password);

            if (!result.Succeeded)
                return BadRequest();

            return Ok(user.UserName);
        }

        [HttpPost("/api/customer/authenticate")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateUser request)
        {
            var identityUser = await _userManager.FindByNameAsync(request.UserName);

            if (identityUser == null)
                return Unauthorized();


            var result = await _signInManager.CheckPasswordSignInAsync(identityUser, request.Password, lockoutOnFailure: false);

            if (!result.Succeeded)
                return Unauthorized();

            List<Claim> userClaims = ConstructUserClaims(identityUser);

            JwtSecurityToken token = GenerateJwtToken(userClaims);

            var tokenResult = new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            };

            return Ok(tokenResult);
        }

        private List<Claim> ConstructUserClaims(Customer identityUser)
        {

            var userClaims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, identityUser.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, identityUser.UserName),
                new Claim(JwtRegisteredClaimNames.Email, identityUser.Email),
            };

            return userClaims;
        }
        private JwtSecurityToken GenerateJwtToken(List<Claim> userClaims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:JwtKey"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(issuer: _configuration["Tokens:JwtIssuer"],
                                             audience: _configuration["Tokens:JwtAudience"],
                                             claims: userClaims,
                                             expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Tokens:JwtValidMinutes"])),
                                             signingCredentials: creds);

            return token;
        }
    }
}
