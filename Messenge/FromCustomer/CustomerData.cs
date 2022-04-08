using Rent_a_Car.Models;

namespace Rent_a_Car.Messenge.FromCustomer
{
    public class CustomerData
    {
        public CustomerData (Customer customer)
        {
            if (customer == null) return;
            Name = customer.Name;
            Surname = customer.Surname;
            Email = customer.Email;
            BecoamingDriverDate = customer.BecoamingDriverDate;
            BirtheDate = customer.BirtheDate;
            City = customer.City;
            Street = customer.Street;
            StreetNumber = customer.StreetNumber;
            Poste_Code = customer.Poste_Code; 
        }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }

        public System.DateTime BecoamingDriverDate { get; set; }
        public System.DateTime BirtheDate { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public int StreetNumber { get; set; }
        public string Poste_Code { get; set; }
        public int carDetalisID { get; set; }
    }
}
