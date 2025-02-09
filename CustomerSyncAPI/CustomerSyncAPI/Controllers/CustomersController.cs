using CustomerSyncAPI.Data;
using CustomerSyncAPI.Models;
using CustomerSyncAPI.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CustomerSyncAPI.Controllers
{

[Route("api/[controller]")]
[ApiController]
public class CustomersController : ControllerBase
{
    private readonly AppDbContext _context;

    public CustomersController(AppDbContext context)
    {
        _context = context;
    }

        // GET: api/customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            var customers = await _context.Customer
                .Include(c => c.Location)
                .Select(c => new Customer
                {
                    CustomerID = c.CustomerID,
                    Name = c.Name,
                    Email = c.Email,
                    Phone = c.Phone,
                    Location = c.Location.Select(l => new Location
                    {
                        LocationID = l.LocationID,
                        CustomerID = l.CustomerID,
                        Address = l.Address
                    }).ToList()
                })
                .ToListAsync();

            return customers;
        }
        // GET: api/customers/{CustomerID}/locations
        [HttpGet("{CustomerID}/locations")]
    public async Task<ActionResult<IEnumerable<Location>>> GetCustomerLocations(int CustomerID)
    {
        var locations = await _context.Location
            .Where(l => l.CustomerID == CustomerID)
            .ToListAsync();

        if (locations == null || !locations.Any())
        {
            return NotFound();
        }

        return locations;
    }
}
}
