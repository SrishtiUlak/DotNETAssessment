namespace CustomerSyncAPI.Models
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public List<Location> Location { get; set; } = new List<Location>();
    }
}
