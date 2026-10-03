using System;
using System.Threading;
using System.Threading.Tasks;
using AliRoyalMarquee.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AliRoyalMarquee.Application.Enquiries.Commands.DeleteEnquiry;

public record DeleteEnquiryCommand(Guid Id) : IRequest;

public class DeleteEnquiryCommandHandler : IRequestHandler<DeleteEnquiryCommand>
{
    private readonly IAppDbContext _context;

    public DeleteEnquiryCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteEnquiryCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Enquiries
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            return;

        _context.Enquiries.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
