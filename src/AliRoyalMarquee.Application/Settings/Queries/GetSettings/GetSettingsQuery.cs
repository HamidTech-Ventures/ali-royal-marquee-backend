using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Application.Settings.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AliRoyalMarquee.Application.Settings.Queries.GetSettings;

public class GetSettingsQuery : IRequest<List<SettingDto>>
{
}

public class GetSettingsQueryHandler : IRequestHandler<GetSettingsQuery, List<SettingDto>>
{
    private readonly IAppDbContext _context;

    public GetSettingsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SettingDto>> Handle(GetSettingsQuery request, CancellationToken cancellationToken)
    {
        var settings = await _context.SystemSettings
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return settings.Select(s => new SettingDto
        {
            Id = s.Id,
            Key = s.Key,
            Value = s.Value,
            Category = s.Category
        }).ToList();
    }
}
