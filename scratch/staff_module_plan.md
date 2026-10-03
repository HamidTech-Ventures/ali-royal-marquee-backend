# Staff Module Overhaul Plan

Based on a deep review of your requirements, here is the architectural and implementation plan to adapt the Staff Module to the realities of Pakistani marquee operations.

## 1. Backend Data Structure (C# / PostgreSQL)
- **Database Schema**: Add `CNIC` (string) and `CompensationType` (string or enum) to the `StaffMember` entity.
- **API DTOs & Commands**: Update `CreateStaffCommand`, `UpdateStaffCommand`, and `StaffMemberDto` so the API can accept and return the CNIC and Compensation Type (`Fixed Monthly` vs `Per-Event/Daily Wage`).
- **Migrations**: Generate and apply an Entity Framework migration to safely add these columns to the database without losing existing staff data.

## 2. Frontend Structure (React / TypeScript)
- **Types**: Update the global `Staff` interface to include `cnic`, `compensationType`, and adapt the Shift type strictly to `'Afternoon (Lunch)' | 'Evening (Dinner)' | 'Night (Cleanup)'`.
- **Add/Edit Form (`StaffForm.tsx`)**: 
  - Make CNIC a mandatory field in the Personal Info section.
  - Replace the static Salary field with a dynamic Compensation Type selector.
  - Update Shift options to align with the standard event lifecycle.
- **Staff Dashboard (`Staff.tsx`)**: Update the quick-filter shift buttons at the top of the grid to mirror the new shift terminologies.
- **Employee Profile (`StaffDetails.tsx`)**:
  - **Remove**: Strip out the overly complex corporate HR sections (Attendance, Schedule, Leave, Performance).
  - **Event Assignment**: Wire this section up to pull real historical data showing exactly which events this staff member worked from their join date to today.
  - **Payroll**: Redesign this block to clearly display either their Fixed Monthly salary or their accrued Per-Event wages based on their assigned events.

## 3. Financial Integration (Future-proofing)
By officially separating "Fixed Monthly" vs "Per-Event/Daily Wage", we lay the groundwork for the Finance module. When an event concludes, the system will dynamically sum up the wages of all daily-wagers assigned to that shift and automatically post it as an "Operating Expense" for that specific Booking.

---
**Status**: Ready to execute. If you approve this plan, I will immediately begin rewriting the backend files, generate the database migration, and overhaul the React components.
