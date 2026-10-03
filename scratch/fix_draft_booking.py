import os
import re

# 1. Update BookingStatus.cs
status_file = r"c:\My working\HamidTech_Ventures\Clients\marquee-management-system\backend\src\AliRoyalMarquee.Domain\Enums\BookingStatus.cs"
with open(status_file, "r", encoding="utf-8") as f:
    status_code = f.read()

if "Draft" not in status_code:
    status_code = status_code.replace("Cancelled", "Cancelled,\n    Draft")
    with open(status_file, "w", encoding="utf-8") as f:
        f.write(status_code)

# 2. Update Booking.cs
booking_entity_file = r"c:\My working\HamidTech_Ventures\Clients\marquee-management-system\backend\src\AliRoyalMarquee.Domain\Entities\Booking.cs"
with open(booking_entity_file, "r", encoding="utf-8") as f:
    entity_code = f.read()

if "MarkAsDraft" not in entity_code:
    entity_code = entity_code.replace("Status != BookingStatus.Pending", "Status != BookingStatus.Pending && Status != BookingStatus.Draft")
    mark_as_draft_method = """
    public void MarkAsDraft()
    {
        Status = BookingStatus.Draft;
    }
"""
    entity_code = entity_code.replace("public void Confirm()", mark_as_draft_method + "\n    public void Confirm()")
    with open(booking_entity_file, "w", encoding="utf-8") as f:
        f.write(entity_code)

# 3. Update CreateBookingCommand.cs
create_cmd_file = r"c:\My working\HamidTech_Ventures\Clients\marquee-management-system\backend\src\AliRoyalMarquee.Application\Bookings\Commands\CreateBooking\CreateBookingCommand.cs"
with open(create_cmd_file, "r", encoding="utf-8") as f:
    cmd_code = f.read()

if "bool IsDraft = false" not in cmd_code:
    cmd_code = cmd_code.replace("string? EventTitle = null\n)", "string? EventTitle = null,\n    bool IsDraft = false\n)")
    with open(create_cmd_file, "w", encoding="utf-8") as f:
        f.write(cmd_code)

# 4. Update CreateBookingCommandHandler.cs
handler_file = r"c:\My working\HamidTech_Ventures\Clients\marquee-management-system\backend\src\AliRoyalMarquee.Application\Bookings\Commands\CreateBooking\CreateBookingCommandHandler.cs"
with open(handler_file, "r", encoding="utf-8") as f:
    handler_code = f.read()

if "request.IsDraft" not in handler_code:
    handler_code = handler_code.replace("_context.Bookings.Add(booking);", "if (request.IsDraft) booking.MarkAsDraft();\n\n        _context.Bookings.Add(booking);")
    with open(handler_file, "w", encoding="utf-8") as f:
        f.write(handler_code)


# 5. Update BookingForm.tsx
form_file = r"c:\My working\HamidTech_Ventures\Clients\marquee-management-system\frontend\src\features\bookings\BookingForm.tsx"
with open(form_file, "r", encoding="utf-8") as f:
    form_code = f.read()

if "isDraft = false" not in form_code:
    # Remove save draft from PageHeader actions
    form_code = re.sub(r'actions=\{[\s\S]*?\}\s*/>', '/>', form_code)
    
    # Update handleSubmit to take isDraft parameter
    form_code = form_code.replace("const handleSubmit = async (e: React.FormEvent) => {", "const handleSubmit = async (e?: React.FormEvent, isDraft = false) => {")
    form_code = form_code.replace("if (e) e.preventDefault();", "if (e) e.preventDefault();") # Already there
    
    # Add isDraft to bookingData
    form_code = form_code.replace("eventTitle: formData.eventTitle,", "eventTitle: formData.eventTitle,\n        isDraft,")
    
    # Update handleSaveDraft to call handleSubmit instead of localStorage
    save_draft_logic = """  const handleSaveDraft = () => {
    handleSubmit(undefined, true);
  };"""
    form_code = re.sub(r'const handleSaveDraft = \(\) => \{[\s\S]*?\};', save_draft_logic, form_code)
    
    # Move Save Draft button next to Confirm
    bottom_bar = """<div className="flex justify-between items-center bg-white p-4 md:p-6 border-t border-[#e8e4db] rounded-b-xl">
          <Button 
            variant="outline" 
            onClick={prevStep}
            disabled={currentStep === 1 || isSubmitting}
            className="w-24 md:w-32"
          >
            Back
          </Button>
          <div className="flex gap-4">
             {currentStep === steps.length && !isEditMode && (
                <Button variant="outline" icon="save" onClick={handleSaveDraft} disabled={isSubmitting}>Save Draft</Button>
             )}
             <Button 
               variant="primary" 
               onClick={currentStep === steps.length ? handleSubmit : nextStep}
               disabled={isSubmitting}
               className="w-32 md:w-40 bg-[#5C0A1E] text-white flex items-center justify-center gap-2"
             >
               {isSubmitting ? <span className="animate-spin material-symbols-outlined text-[18px]">autorenew</span> : null}
               {currentStep === steps.length ? (isEditMode ? 'Update' : 'Confirm') : 'Continue'}
             </Button>
          </div>
        </div>"""
    form_code = re.sub(r'<div className="flex justify-between items-center bg-white p-4 md:p-6 border-t border-\[#e8e4db\] rounded-b-xl">[\s\S]*?</div>\s*</div>\s*</div>', bottom_bar + "\n      </div>\n    </div>", form_code)

    with open(form_file, "w", encoding="utf-8") as f:
        f.write(form_code)

# 6. Update Bookings.tsx to show Draft badge correctly
bookings_file = r"c:\My working\HamidTech_Ventures\Clients\marquee-management-system\frontend\src\features\bookings\Bookings.tsx"
with open(bookings_file, "r", encoding="utf-8") as f:
    bookings_code = f.read()

bookings_code = bookings_code.replace("status === 'Pending' ? 'warning'", "status === 'Pending' ? 'warning' : status === 'Draft' ? 'outline'")
with open(bookings_file, "w", encoding="utf-8") as f:
    f.write(bookings_code)

print("Booking draft logic updated successfully.")
