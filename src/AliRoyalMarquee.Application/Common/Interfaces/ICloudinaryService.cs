using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.Common.Interfaces
{
    public interface ICloudinaryService
    {
        Task<string> UploadPdfAsync(byte[] fileBytes, string fileName);
        Task<string> GenerateAndUploadInvoiceAsync(AliRoyalMarquee.Application.Bookings.DTOs.BookingDetailDto booking);
    }
}
