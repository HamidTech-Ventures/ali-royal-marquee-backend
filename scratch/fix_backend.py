import os
import re

backend_src = r"c:\My working\HamidTech_Ventures\Clients\marquee-management-system\backend\src"

def replace_in_file(path, old, new):
    if not os.path.exists(path): return
    with open(path, 'r', encoding='utf-8') as f:
        content = f.read()
    content = content.replace(old, new)
    with open(path, 'w', encoding='utf-8') as f:
        f.write(content)

# 1. Remove ProfitMarginTarget
package_cs = os.path.join(backend_src, r"AliRoyalMarquee.Domain\Entities\Package.cs")
replace_in_file(package_cs, "public int? ProfitMarginTarget { get; set; }", "")

create_pkg = os.path.join(backend_src, r"AliRoyalMarquee.Application\Packages\Commands\CreatePackageCommand.cs")
replace_in_file(create_pkg, "public int? ProfitMarginTarget { get; set; }", "")
replace_in_file(create_pkg, "ProfitMarginTarget = request.ProfitMarginTarget,", "")

update_pkg = os.path.join(backend_src, r"AliRoyalMarquee.Application\Packages\Commands\UpdatePackageCommand.cs")
replace_in_file(update_pkg, "public int? ProfitMarginTarget { get; init; }", "")
replace_in_file(update_pkg, "package.ProfitMarginTarget = request.ProfitMarginTarget;", "")

# 2. Remove Cost from MenuItem
menuitem_cs = os.path.join(backend_src, r"AliRoyalMarquee.Domain\Entities\MenuItem.cs")
replace_in_file(menuitem_cs, "public decimal Cost { get; set; }", "")

create_menu = os.path.join(backend_src, r"AliRoyalMarquee.Application\MenuItems\Commands\CreateMenuItemCommand.cs")
replace_in_file(create_menu, "public decimal Cost { get; init; }", "")
replace_in_file(create_menu, "Cost = request.Cost", "")

# 3. Create UpdateMenuItemCommand
update_menu_content = """using AliRoyalMarquee.Application.Common.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.MenuItems.Commands;

public class UpdateMenuItemCommand : IRequest
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string? Description { get; init; }
}

public class UpdateMenuItemCommandHandler : IRequestHandler<UpdateMenuItemCommand>
{
    private readonly IAppDbContext _context;
    public UpdateMenuItemCommandHandler(IAppDbContext context) => _context = context;

    public async Task Handle(UpdateMenuItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.MenuItems.FindAsync(new object[] { request.Id }, cancellationToken);
        if (entity == null) throw new Exception("Not found");
        entity.Name = request.Name;
        entity.Category = request.Category;
        entity.Description = request.Description;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
"""
os.makedirs(os.path.join(backend_src, r"AliRoyalMarquee.Application\MenuItems\Commands"), exist_ok=True)
with open(os.path.join(backend_src, r"AliRoyalMarquee.Application\MenuItems\Commands\UpdateMenuItemCommand.cs"), 'w') as f:
    f.write(update_menu_content)

# 4. Create UpdateAddonCommand
update_addon_content = """using AliRoyalMarquee.Application.Common.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.Addons.Commands;

public class UpdateAddonCommand : IRequest
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string? Description { get; init; }
    public string Unit { get; init; } = string.Empty;
}

public class UpdateAddonCommandHandler : IRequestHandler<UpdateAddonCommand>
{
    private readonly IAppDbContext _context;
    public UpdateAddonCommandHandler(IAppDbContext context) => _context = context;

    public async Task Handle(UpdateAddonCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Addons.FindAsync(new object[] { request.Id }, cancellationToken);
        if (entity == null) throw new Exception("Not found");
        entity.Name = request.Name;
        entity.Category = request.Category;
        entity.Price = request.Price;
        entity.Description = request.Description;
        entity.Unit = request.Unit;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
"""
os.makedirs(os.path.join(backend_src, r"AliRoyalMarquee.Application\Addons\Commands"), exist_ok=True)
with open(os.path.join(backend_src, r"AliRoyalMarquee.Application\Addons\Commands\UpdateAddonCommand.cs"), 'w') as f:
    f.write(update_addon_content)

# 5. Create UpdatePricingRuleCommand
update_rule_content = """using AliRoyalMarquee.Application.Common.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.PricingRules.Commands;

public class UpdatePricingRuleCommand : IRequest
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string RuleType { get; init; } = string.Empty;
    public decimal? FlatAmount { get; init; }
    public decimal? PercentageAmount { get; init; }
}

public class UpdatePricingRuleCommandHandler : IRequestHandler<UpdatePricingRuleCommand>
{
    private readonly IAppDbContext _context;
    public UpdatePricingRuleCommandHandler(IAppDbContext context) => _context = context;

    public async Task Handle(UpdatePricingRuleCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.PricingRules.FindAsync(new object[] { request.Id }, cancellationToken);
        if (entity == null) throw new Exception("Not found");
        entity.Name = request.Name;
        entity.RuleType = request.RuleType;
        entity.FlatAmount = request.FlatAmount;
        entity.PercentageAmount = request.PercentageAmount;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
"""
os.makedirs(os.path.join(backend_src, r"AliRoyalMarquee.Application\PricingRules\Commands"), exist_ok=True)
with open(os.path.join(backend_src, r"AliRoyalMarquee.Application\PricingRules\Commands\UpdatePricingRuleCommand.cs"), 'w') as f:
    f.write(update_rule_content)
