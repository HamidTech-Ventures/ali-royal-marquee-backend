using AliRoyalMarquee.Application.Enquiries.DTOs;
using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.Enquiries.Interfaces;

public interface IQuotationPdfGenerator
{
    byte[] GeneratePdf(EnquiryDetailDto enquiry, EnquiryQuotationDto quotation);
}
