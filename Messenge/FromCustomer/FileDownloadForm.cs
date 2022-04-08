using Microsoft.AspNetCore.Http;

namespace Rent_a_Car.Messenge.FromCustomer
{
    public class FileDownloadForm
    {
        public bool isModal()
        {
            return RentID != null && RentID != "" && (IsPhoto ^ IsProtocol);
        }
        public string RentID { get; set; }

        public bool IsPhoto { get; set; } = false;
        public bool IsProtocol { get; set; } = false;

    }
}
