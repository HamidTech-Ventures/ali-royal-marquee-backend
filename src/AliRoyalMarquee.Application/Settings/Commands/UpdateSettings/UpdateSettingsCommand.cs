using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Application.Settings.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AliRoyalMarquee.Application.Settings.Commands.UpdateSettings;

public class UpdateSettingsCommand : IRequest<bool>
{
    public List<SettingDto> Settings { get; set; } = new();
}

public class UpdateSettingsCommandHandler : IRequestHandler<UpdateSettingsCommand, bool>
{
    private readonly IAppDbContext _context;

    public UpdateSettingsCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateSettingsCommand request, CancellationToken cancellationToken)
    {
        var existingSettings = await _context.SystemSettings.ToListAsync(cancellationToken);

        foreach (var newSetting in request.Settings)
        {
            var existing = existingSettings.FirstOrDefault(s => s.Key == newSetting.Key);
            if (existing != null)
            {
                existing.Value = newSetting.Value;
                existing.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
