import os

backend_src = r"c:\My working\HamidTech_Ventures\Clients\marquee-management-system\backend\src"

# 1. Update StaffMember.cs
staff_entity = os.path.join(backend_src, "AliRoyalMarquee.Domain", "Entities", "StaffMember.cs")
with open(staff_entity, "r") as f:
    code = f.read()
if "public string CNIC" not in code:
    code = code.replace("public decimal? Salary { get; set; }", 
                        "public decimal? Salary { get; set; }\n        public string CNIC { get; set; }\n        public string CompensationType { get; set; }")
    with open(staff_entity, "w") as f:
        f.write(code)

# 2. Update CreateStaffCommand.cs
create_cmd = os.path.join(backend_src, "AliRoyalMarquee.Application", "StaffMembers", "Commands", "CreateStaffCommand.cs")
with open(create_cmd, "r") as f:
    code = f.read()
if "public string CNIC" not in code:
    code = code.replace("public decimal? Salary { get; set; }", 
                        "public decimal? Salary { get; set; }\n        public string CNIC { get; set; }\n        public string CompensationType { get; set; }")
    code = code.replace("Salary = request.Salary", 
                        "Salary = request.Salary,\n                CNIC = request.CNIC,\n                CompensationType = request.CompensationType")
    with open(create_cmd, "w") as f:
        f.write(code)

# 3. Update UpdateStaffCommand.cs
update_cmd = os.path.join(backend_src, "AliRoyalMarquee.Application", "StaffMembers", "Commands", "UpdateStaffCommand.cs")
with open(update_cmd, "r") as f:
    code = f.read()
if "public string CNIC" not in code:
    code = code.replace("public decimal? Salary { get; set; }", 
                        "public decimal? Salary { get; set; }\n        public string CNIC { get; set; }\n        public string CompensationType { get; set; }")
    code = code.replace("entity.Salary = request.Salary;", 
                        "entity.Salary = request.Salary;\n            entity.CNIC = request.CNIC;\n            entity.CompensationType = request.CompensationType;")
    with open(update_cmd, "w") as f:
        f.write(code)

# 4. Update StaffDto.cs
dto = os.path.join(backend_src, "AliRoyalMarquee.Application", "StaffMembers", "DTOs", "StaffDto.cs")
with open(dto, "r") as f:
    code = f.read()
if "public string CNIC" not in code:
    code = code.replace("public decimal? Salary { get; set; }", 
                        "public decimal? Salary { get; set; }\n        public string CNIC { get; set; }\n        public string CompensationType { get; set; }")
    with open(dto, "w") as f:
        f.write(code)

# 5. Check GetStaffQuery mapping
get_query = os.path.join(backend_src, "AliRoyalMarquee.Application", "StaffMembers", "Queries", "GetStaffQuery.cs")
if os.path.exists(get_query):
    with open(get_query, "r") as f:
        code = f.read()
    if "CNIC = s.CNIC" not in code and "Salary = s.Salary" in code:
        code = code.replace("Salary = s.Salary", 
                            "Salary = s.Salary,\n                    CNIC = s.CNIC,\n                    CompensationType = s.CompensationType")
        with open(get_query, "w") as f:
            f.write(code)

print("Backend files updated.")
