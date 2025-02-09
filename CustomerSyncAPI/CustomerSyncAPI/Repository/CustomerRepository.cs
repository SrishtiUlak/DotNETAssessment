using CustomerSyncAPI.Data;
using CustomerSyncAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerSyncAPI.Repository
{
    public class CustomerRepository
    {
        private readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            return await _context.Customer
                .Include(c => c.Location) // Include the related Location entity
                .ToListAsync(); // Asynchronously retrieve all customers
        }

        public async Task<List<Location>> GetCustomerLocationsAsync(int customerId)
        {
            return await _context.Location.Where(l => l.CustomerID == customerId).ToListAsync();
        }
    }

}
