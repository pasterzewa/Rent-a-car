using Microsoft.AspNetCore.Http;

namespace Rent_a_Car.Messenge.FromCustomer
{
    public class FileUploadForm
    {
        public bool isModal()
        {
            return File != null && RentID != null && RentID != "" && (IsPhoto ^ IsProtocol);
        }
        public IFormFile File { get; set; }
        public string RentID { get; set; }

        public bool IsPhoto { get; set; } = false;
        public bool IsProtocol { get; set; } = false;
    }
}
