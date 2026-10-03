import os

backend_src = r"c:\My working\HamidTech_Ventures\Clients\marquee-management-system\backend\src"

# 1. Update CreateInventoryItemCommand.cs
create_cmd = os.path.join(backend_src, r"AliRoyalMarquee.Application\Inventory\Commands\CreateInventoryItemCommand.cs")
with open(create_cmd, "r") as f:
    code = f.read()
code = code.replace("public string Unit { get; set; }", "public string Unit { get; set; }\n        public string ItemType { get; set; }")
code = code.replace("Unit = request.Unit", "Unit = request.Unit,\n                ItemType = request.ItemType")
with open(create_cmd, "w") as f:
    f.write(code)

# 2. Update UpdateInventoryItemCommand.cs
update_cmd = os.path.join(backend_src, r"AliRoyalMarquee.Application\Inventory\Commands\UpdateInventoryItemCommand.cs")
with open(update_cmd, "r") as f:
    code = f.read()
code = code.replace("public string Unit { get; set; }", "public string Unit { get; set; }\n        public string ItemType { get; set; }")
code = code.replace("entity.Unit = request.Unit;", "entity.Unit = request.Unit;\n            entity.ItemType = request.ItemType;")
with open(update_cmd, "w") as f:
    f.write(code)

print("C# commands updated")
