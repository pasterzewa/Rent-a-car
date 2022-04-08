using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rent_a_Car.Data
{
    public class DatabaseFiller
    {
        public static void FillDataIfEmpty(ApplicationDbContext context)
        {
            if (context.Car.Count() > 0) return;
            FillCars(context);
            FillCompanies(context);
            FillEmployers(context);
            FillCustomers(context);
            FillReturnFile(context);
            FillRentCar(context);
            FillCarDetails(context);
            
        }

        private static void FillCars(ApplicationDbContext context)
        {
            context.Car.Add(PrepCar("Mercedes-Benz", "CLS-Klasa", 306));
            context.Car.Add(PrepCar("Alfa Romeo", "Giulia", 280));
            context.Car.Add(PrepCar("Opel", "Astra", 160));
            context.Car.Add(PrepCar("Opel", "Vectra", 122));
            context.Car.Add(PrepCar("Volkswagen", "Golf", 105));
            context.SaveChanges();
        }

        private static Models.Car PrepCar(string brand, string model, int horsePower)
        {
            var car1 = new Models.Car();
            car1.Brand = brand;
            car1.Model = model;
            car1.HorsePower = horsePower;
            return car1;
        }
        private static void FillCompanies(ApplicationDbContext context)
        {
            context.Company.Add(PrepCompany("Całkowicie normalna firma", 123123132));
            context.Company.Add(PrepCompany("Studenci Politechniki", 321321321));
            context.Company.Add(PrepCompany("Wypożyczalnia u Stefka", 111222333));
            context.SaveChanges();
        }

        private static Models.Company PrepCompany(string name, int phone)
        {
            var company = new Models.Company();
            company.Name = name;
            company.Phone = phone;
            return company;
        }
        private static void FillEmployers(ApplicationDbContext context)
        {
            context.Employer.Add(PrepEmployer("Michał", "Michałowski"));
            context.Employer.Add(PrepEmployer("Robert", "Rpbertowicz"));
            context.Employer.Add(PrepEmployer("Stefan", "Stefański"));
            context.SaveChanges();

        }

        private static Models.Employer PrepEmployer(string name, string surname)
        {
            var employer = new Models.Employer();
            employer.Name = name;
            employer.Surname = surname;
            return employer;
        }

        private static void FillCustomers(ApplicationDbContext context)
        {
            context.Customer.Add(PrepCustomer("Maciej", "Maciejowicz", "maciej@mac.pl", new DateTime(2000, 7, 7), "Warszawa", "98-358", new DateTime(2018, 09, 05), "maciejj", "redacted123", 0));
            context.Customer.Add(PrepCustomer("Mariusz", "Mariuszewski", "mariu@mac.pl", new DateTime(1992, 1, 12), "Warszawa", "32-123", new DateTime(2012, 01, 02), "mariuszz", "redacted123", 0));
            context.Customer.Add(PrepCustomer("Sebastian", "Sebastiański", "seba@mac.pl", new DateTime(1997, 2, 13), "Warszawa", "12-432", new DateTime(2018, 09, 05), "sebekk", "redacted123", 0));
            context.Customer.Add(PrepCustomer("Marzena", "Marzennowska", "marzena@mac.pl", new DateTime(1987, 7, 7), "Warszawa", "12-321", new DateTime(2016, 07, 14), "marzenna", "redacted123", 0));
            context.Customer.Add(PrepCustomer("Izabela", "Izabelowska", "iza@mac.pl", new DateTime(1990, 6, 12), "Warszawa", "53-123", new DateTime(2017, 01, 23), "izabella", "redacted123", 0));
            context.SaveChanges();
        }

        private static Models.Customer PrepCustomer(string name, string surname, string email, System.DateTime dateOfBirth, string city, string postalCode, System.DateTime driversLicenseDate, string login, string password, int numberOfRentedCars)
        {
            var customer = new Models.Customer();
            customer.Name = name;
            customer.Surname = surname;
            customer.Email = email;
            customer.BirtheDate = dateOfBirth;
            customer.City = city;
            customer.Poste_Code = postalCode;
            return customer;
        }
        private static void FillReturnFile(ApplicationDbContext context)
        {
            context.SaveChanges();
        }

        private static void FillRentCar(ApplicationDbContext context)
        {
            context.SaveChanges();
        }

        private static void FillCarDetails(ApplicationDbContext context)
        {
            context.CarDetails.Add(PrepCarDetail(1,1,2000,new DateTime(2000,1,1),"Niezły"));
            context.CarDetails.Add(PrepCarDetail(2, 1, 2001, new DateTime(1999, 1, 1), "Niezły"));
            context.CarDetails.Add(PrepCarDetail(3, 1, 2002, new DateTime(1998, 1, 1), "Niezły"));
            context.CarDetails.Add(PrepCarDetail(4, 1, 2003, new DateTime(1997, 1, 1), "Niezły"));
            context.CarDetails.Add(PrepCarDetail(5, 1, 2004, new DateTime(1996, 1, 1), "Niezły"));
            context.SaveChanges();
        }

        private static Models.CarDetails PrepCarDetail(int carID, int companyID, decimal price, DateTime yearOfProduction, string description)
        {
            Models.CarDetails carDetails = new();
            carDetails.CarID = carID;
            carDetails.CompanyID = companyID;
            carDetails.Price = price;
            carDetails.YearOfProduction = yearOfProduction;
            carDetails.Description = description;
            carDetails.IsAvailable = true;
            return carDetails;
        }
    }
}
