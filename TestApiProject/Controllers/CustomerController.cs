using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestApiProject.DataContext;
using TestApiProject.ViewModel;

namespace TestApiProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<CustomersViewModel>>> GetAllCustomers()
        {
            var getCustomerQuery = await _context
                                                .Customers
                                                .Select(x => new CustomersViewModel
                                                {
                                                    UserName = x.UserName,
                                                    Email = x.Email,
                                                    PhoneNumber = x.PhoneNumber,
                                                }).ToListAsync();
            return Ok(getCustomerQuery);
        }
    }
}
