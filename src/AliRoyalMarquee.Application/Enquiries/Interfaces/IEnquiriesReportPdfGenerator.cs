using AliRoyalMarquee.Application.Enquiries.DTOs;
using AliRoyalMarquee.Application.Enquiries.Queries.GetEnquiryStats;
using AliRoyalMarquee.Application.Enquiries.Queries.GetEnquiryLifecycle;
using System.Collections.Generic;

namespace AliRoyalMarquee.Application.Enquiries.Interfaces;

public interface IEnquiriesReportPdfGenerator
{
    byte[] GeneratePdf(
        EnquiryStatsDto stats, 
        EnquiryLifecycleDto lifecycle, 
        List<EnquiryDto> enquiries, 
        string appliedFilters);
}
