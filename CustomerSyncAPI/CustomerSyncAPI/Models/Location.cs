namespace CustomerSyncAPI.Models
{
    public class Location
    {
        public int LocationID { get; set; }
        public int CustomerID { get; set; }
        public string Address { get; set; }

        public Customer Customer { get; set; }
    }
}
