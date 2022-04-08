using Microsoft.AspNetCore.Http;

namespace Rent_a_Car.Messenge.FromCustomer
{

    public class ReturnFormMessege
    {

        public string ReturnFileID { get; set; }
        public int RentedCarID { get; set; }
        public System.DateTime ReturnDate { get; set; }
        public string CarConditon { get; set; }
        public int OdometerReading { get; set; }
        public IFormFile Photo { get; set; }
        public IFormFile ReturnProocol { get; set; }

    }
}
