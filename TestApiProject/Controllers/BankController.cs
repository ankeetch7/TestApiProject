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
    public class BankController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public BankController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<BankViewModel>>> GetAllBanks()
        {
            var bankQuery = await _context.Banks
                                    .Select(x => new BankViewModel
                                    {
                                        BankCode = x.BankCode,
                                        BankName = x.BankName,
                                        Address = x.Address,
                                        ContactPerson = x.ContactPerson,
                                        ContactNo = x.ContactNo,
                                    }).ToListAsync();
            return Ok();
        }
    }
}
