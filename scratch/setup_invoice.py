import sys
import os

backend_path = r"c:\My working\HamidTech_Ventures\Clients\marquee-management-system\backend\src\AliRoyalMarquee.Application\Common\Interfaces\ICloudinaryService.cs"
with open(backend_path, 'w', encoding='utf-8') as f:
    f.write("""using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.Common.Interfaces
{
    public interface ICloudinaryService
    {
        Task<string> UploadPdfAsync(byte[] fileBytes, string fileName);
    }
}
""")

backend_path = r"c:\My working\HamidTech_Ventures\Clients\marquee-management-system\backend\src\AliRoyalMarquee.Infrastructure\Services\CloudinaryService.cs"
os.makedirs(os.path.dirname(backend_path), exist_ok=True)
with open(backend_path, 'w', encoding='utf-8') as f:
    f.write("""using CloudinaryDotNet;
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
                "YOUR_CLOUD_NAME",
                "YOUR_API_KEY",
                "YOUR_API_SECRET");

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
            return uploadResult.SecureUrl.ToString();
        }
    }
}
""")

backend_path = r"c:\My working\HamidTech_Ventures\Clients\marquee-management-system\backend\src\AliRoyalMarquee.Application\Bookings\Commands\GenerateInvoice\GenerateInvoiceCommand.cs"
os.makedirs(os.path.dirname(backend_path), exist_ok=True)
with open(backend_path, 'w', encoding='utf-8') as f:
    f.write("""using AliRoyalMarquee.Application.Common.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using QuestPDF.Fluent;

namespace AliRoyalMarquee.Application.Bookings.Commands.GenerateInvoice
{
    public class GenerateInvoiceCommand : IRequest<string>
    {
        public Guid BookingId { get; set; }
    }

    public class GenerateInvoiceCommandHandler : IRequestHandler<GenerateInvoiceCommand, string>
    {
        private readonly ICloudinaryService _cloudinary;

        public GenerateInvoiceCommandHandler(ICloudinaryService cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public async Task<string> Handle(GenerateInvoiceCommand request, CancellationToken cancellationToken)
        {
            // Note: In reality, we'd fetch the Booking from DB and pass data to the Document
            // var booking = await _context.Bookings.FindAsync(request.BookingId);
            
            var document = new AliRoyalMarquee.Infrastructure.Pdf.BookingInvoiceDocument();
            byte[] pdfBytes = document.GeneratePdf();

            string fileName = $"INV-{DateTime.Now.Year}-{request.BookingId.ToString().Substring(0,4)}";
            string url = await _cloudinary.UploadPdfAsync(pdfBytes, fileName);
            
            return url;
        }
    }
}
""")

print("Backend Cloudinary/Invoice setup completed")
