using AliRoyalMarquee.Application.Enquiries.Commands.CompleteEnquiryFollowUp;
using AliRoyalMarquee.Application.Enquiries.Commands.ConvertEnquiryToBooking;
using AliRoyalMarquee.Application.Enquiries.Commands.CreateEnquiry;
using AliRoyalMarquee.Application.Enquiries.Commands.CreateEnquiryActivity;
using AliRoyalMarquee.Application.Enquiries.Commands.CreateEnquiryFollowUp;
using AliRoyalMarquee.Application.Enquiries.Commands.CreateEnquiryQuotation;
using AliRoyalMarquee.Application.Enquiries.Commands.MarkEnquiryLost;
using AliRoyalMarquee.Application.Enquiries.Commands.UpdateEnquiry;
using AliRoyalMarquee.Application.Enquiries.Commands.UpdateEnquiryStatus;
using AliRoyalMarquee.Application.Enquiries.Commands.DeleteEnquiry;
using AliRoyalMarquee.Application.Enquiries.Queries.GetEnquiries;
using AliRoyalMarquee.Application.Enquiries.Queries.GetEnquiryById;
using AliRoyalMarquee.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AliRoyalMarquee.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Enforce authorization
public class EnquiriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public EnquiriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private Guid GetUserId()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) 
            ?? User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)
            ?? User.FindFirstValue("id")
            ?? User.FindFirstValue("userId");
        return Guid.TryParse(userIdString, out var userId) ? userId : Guid.Empty;
    }

    [HttpGet]
    public async Task<IActionResult> GetEnquiries([FromQuery] GetEnquiriesQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var result = await _mediator.Send(new AliRoyalMarquee.Application.Enquiries.Queries.GetEnquiryStats.GetEnquiryStatsQuery());
        return Ok(result);
    }

    [HttpGet("lifecycle")]
    public async Task<IActionResult> GetLifecycle([FromQuery] string scope = "all")
    {
        var result = await _mediator.Send(new AliRoyalMarquee.Application.Enquiries.Queries.GetEnquiryLifecycle.GetEnquiryLifecycleQuery(scope));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetEnquiryById(Guid id)
    {
        var result = await _mediator.Send(new GetEnquiryByIdQuery(id));
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateEnquiry([FromBody] CreateEnquiryCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetEnquiryById), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateEnquiry(Guid id, [FromBody] UpdateEnquiryCommand command)
    {
        if (id != command.Id) return BadRequest();
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteEnquiry(Guid id)
    {
        await _mediator.Send(new DeleteEnquiryCommand(id));
        return NoContent();
    }

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateEnquiryStatusRequest request)
    {
        await _mediator.Send(new UpdateEnquiryStatusCommand(id, request.Status, GetUserId()));
        return NoContent();
    }

    [HttpPost("{id:guid}/mark-lost")]
    public async Task<IActionResult> MarkLost(Guid id, [FromBody] MarkEnquiryLostRequest request)
    {
        await _mediator.Send(new MarkEnquiryLostCommand(id, request.Reason, GetUserId()));
        return NoContent();
    }

    [HttpPost("{id:guid}/convert-to-booking")]
    public async Task<IActionResult> ConvertToBooking(Guid id, [FromBody] ConvertEnquiryToBookingRequest request)
    {
        var command = new ConvertEnquiryToBookingCommand(
            id, request.VenueId, request.Date, request.StartTime, request.EndTime, request.GuestCount, request.TotalAmount, GetUserId(), request.PackageId);
        var bookingId = await _mediator.Send(command);
        return Ok(new { bookingId });
    }

    [HttpPost("{id:guid}/followups")]
    public async Task<IActionResult> CreateFollowUp(Guid id, [FromBody] CreateEnquiryFollowUpRequest request)
    {
        var command = new CreateEnquiryFollowUpCommand(id, request.DueDate, request.DueTime, request.Type, request.Notes, request.AssignedToId);
        var followUpId = await _mediator.Send(command);
        return Ok(new { followUpId });
    }

    [HttpPost("{id:guid}/followups/{followUpId:guid}/complete")]
    public async Task<IActionResult> CompleteFollowUp(Guid id, Guid followUpId, [FromBody] CompleteEnquiryFollowUpRequest request)
    {
        await _mediator.Send(new CompleteEnquiryFollowUpCommand(id, followUpId, request.Result, GetUserId()));
        return NoContent();
    }

    [HttpPut("{id:guid}/followups/{followUpId:guid}")]
    public async Task<IActionResult> UpdateFollowUp(Guid id, Guid followUpId, [FromBody] UpdateEnquiryFollowUpRequest request)
    {
        await _mediator.Send(new AliRoyalMarquee.Application.Enquiries.Commands.UpdateEnquiryFollowUp.UpdateEnquiryFollowUpCommand(
            id, followUpId, request.DueDate, request.DueTime, request.Type, request.Notes, request.AssignedToId, GetUserId()));
        return NoContent();
    }

    [HttpDelete("{id:guid}/followups/{followUpId:guid}")]
    public async Task<IActionResult> CancelFollowUp(Guid id, Guid followUpId)
    {
        await _mediator.Send(new AliRoyalMarquee.Application.Enquiries.Commands.CancelEnquiryFollowUp.CancelEnquiryFollowUpCommand(id, followUpId, GetUserId()));
        return NoContent();
    }

    [HttpPost("{id:guid}/activities")]
    public async Task<IActionResult> CreateActivity(Guid id, [FromBody] CreateEnquiryActivityRequest request)
    {
        var command = new CreateEnquiryActivityCommand(id, request.Type, request.Description, GetUserId());
        var activityId = await _mediator.Send(command);
        return Ok(new { activityId });
    }

    [HttpPost("{id:guid}/quotations")]
    public async Task<IActionResult> CreateQuotation(Guid id, [FromBody] CreateEnquiryQuotationRequest request)
    {
        var command = new CreateEnquiryQuotationCommand(
            id, 
            request.LineItems, 
            request.DiscountAmount, 
            request.ServiceChargeAmount, 
            request.PRATaxAmount, 
            request.TokenMoney,
            request.AdvancePayment,
            request.ValidUntil, 
            request.Notes, 
            GetUserId());
        var quotationId = await _mediator.Send(command);
        return Ok(new { quotationId });
    }

    [HttpPost("{id:guid}/quotations/{quotationId:guid}/revision")]
    public async Task<IActionResult> CreateQuotationRevision(Guid id, Guid quotationId)
    {
        var newQuotationId = await _mediator.Send(new AliRoyalMarquee.Application.Enquiries.Commands.CreateQuotationRevision.CreateQuotationRevisionCommand(id, quotationId, GetUserId()));
        return Ok(new { quotationId = newQuotationId });
    }

    [HttpDelete("{id:guid}/quotations/{quotationId:guid}")]
    [Authorize(Policy = "Enquiries.ManageQuotations")]
    public async Task<IActionResult> DeleteQuotation(Guid id, Guid quotationId)
    {
        await _mediator.Send(new AliRoyalMarquee.Application.Enquiries.Commands.DeleteEnquiryQuotation.DeleteEnquiryQuotationCommand(id, quotationId));
        return NoContent();
    }

    [HttpGet("{id:guid}/quotations/{quotationId:guid}/pdf")]
    [Authorize(Policy = "Enquiries.ManageQuotations")]
    public async Task<IActionResult> GetQuotationPdf(
        Guid id, 
        Guid quotationId, 
        [FromServices] AliRoyalMarquee.Application.Enquiries.Interfaces.IQuotationPdfGenerator pdfGenerator)
    {
        var enquiry = await _mediator.Send(new GetEnquiryByIdQuery(id));
        if (enquiry == null) return NotFound();

        var quotation = enquiry.Quotations.FirstOrDefault(q => q.Id == quotationId);
        if (quotation == null) return NotFound();

        var pdfBytes = pdfGenerator.GeneratePdf(enquiry, quotation);
        var filename = $"Ali-Royal-Marquee-Quotation-{quotation.QuotationReference}.pdf";

        // Log Activity
        await _mediator.Send(new CreateEnquiryActivityCommand(id, EnquiryActivityType.NoteAdded, $"Downloaded PDF for Quotation {quotation.QuotationReference}", GetUserId()));

        return File(pdfBytes, "application/pdf", filename);
    }

    [HttpGet("export/pdf")]
    [Authorize(Policy = "Enquiries.Export")]
    public async Task<IActionResult> ExportEnquiriesPdf(
        [FromQuery] GetEnquiriesQuery query,
        [FromServices] AliRoyalMarquee.Application.Enquiries.Interfaces.IEnquiriesReportPdfGenerator pdfGenerator)
    {
        // Execute the identical query but without pagination (max 1000 records for safety)
        var exportQuery = query with { PageNumber = 1, PageSize = 1000 };
        var paginatedEnquiries = await _mediator.Send(exportQuery);

        // We also need the stats and lifecycle for the report.
        var stats = await _mediator.Send(new AliRoyalMarquee.Application.Enquiries.Queries.GetEnquiryStats.GetEnquiryStatsQuery());
        var lifecycle = await _mediator.Send(new AliRoyalMarquee.Application.Enquiries.Queries.GetEnquiryLifecycle.GetEnquiryLifecycleQuery("all"));

        var appliedFilters = $"Status: {query.Status?.ToString() ?? "All"}";

        var pdfBytes = pdfGenerator.GeneratePdf(stats, lifecycle, paginatedEnquiries.Items.ToList(), appliedFilters);
        var filename = $"Ali-Royal-Marquee-Enquiries-Report-{DateTime.UtcNow:yyyy-MM-dd}.pdf";

        // Audit Log (this requires AuditLogCommand which we should create, but for now we skip or just use an Application Service)
        // For now just return the file
        return File(pdfBytes, "application/pdf", filename);
    }
}

// Request models for endpoints
public class UpdateEnquiryStatusRequest { public EnquiryStatus Status { get; set; } }
public class MarkEnquiryLostRequest { public string Reason { get; set; } = default!; }
public class ConvertEnquiryToBookingRequest { 
    public Guid VenueId { get; set; }
    public DateTime Date { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int GuestCount { get; set; }
    public decimal TotalAmount { get; set; }
    public Guid? PackageId { get; set; }
}
public class CreateEnquiryFollowUpRequest { 
    public DateTime DueDate { get; set; }
    public TimeSpan? DueTime { get; set; }
    public FollowUpType Type { get; set; }
    public string? Notes { get; set; }
    public Guid? AssignedToId { get; set; }
}
public class UpdateEnquiryFollowUpRequest { 
    public DateTime DueDate { get; set; }
    public TimeSpan? DueTime { get; set; }
    public FollowUpType Type { get; set; }
    public string? Notes { get; set; }
    public Guid? AssignedToId { get; set; }
}
public class CompleteEnquiryFollowUpRequest { public string? Result { get; set; } }
public class CreateEnquiryActivityRequest { 
    public EnquiryActivityType Type { get; set; }
    public string Description { get; set; } = default!;
}
public class CreateEnquiryQuotationRequest { 
    public System.Collections.Generic.List<AliRoyalMarquee.Application.Enquiries.Commands.CreateEnquiryQuotation.CreateQuotationLineItemDto> LineItems { get; set; } = new();
    public decimal DiscountAmount { get; set; }
    public decimal ServiceChargeAmount { get; set; }
    public decimal PRATaxAmount { get; set; }
    public decimal TokenMoney { get; set; }
    public decimal AdvancePayment { get; set; }
    public DateTime? ValidUntil { get; set; }
    public string? Notes { get; set; }
}
