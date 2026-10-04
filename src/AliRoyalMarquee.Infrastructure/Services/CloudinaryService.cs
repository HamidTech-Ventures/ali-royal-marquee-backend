using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.IO;
using System.Threading.Tasks;
using AliRoyalMarquee.Application.Common.Interfaces;

namespace AliRoyalMarquee.Infrastructure.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService()
        {
            // PLACEHOLDER: USER MUST REPLACE THESE WITH ACTUAL CREDENTIALS
            Account account = new Account(
                "dbg4uwv2b",
                "944311492752433",
                "hH_kfI39-lgJQ-Zh5YlcOPAIdDo");

            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadPdfAsync(byte[] fileBytes, string fileName)
        {
            using var stream = new MemoryStream(fileBytes);
            var uploadParams = new RawUploadParams()
            {
                File = new FileDescription(fileName, stream),
                PublicId = $"invoices/{fileName}",
                Overwrite = true
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            
            if (uploadResult.Error != null)
            {
                throw new System.Exception($"Cloudinary Error: {uploadResult.Error.Message}");
            }
            
            return uploadResult.SecureUrl?.ToString() ?? uploadResult.Url?.ToString() ?? "No URL Returned";
        }

        public async Task<string> GenerateAndUploadInvoiceAsync(AliRoyalMarquee.Application.Bookings.DTOs.BookingDetailDto booking)
        {
            var document = new AliRoyalMarquee.Infrastructure.Pdf.BookingInvoiceDocument(booking);
            byte[] pdfBytes = QuestPDF.Fluent.GenerateExtensions.GeneratePdf(document);

            string fileName = $"INV-{System.DateTime.Now.Year}-{booking.Id.ToString().Substring(0,4)}.pdf";
            return await UploadPdfAsync(pdfBytes, fileName);
        }
    }
}
