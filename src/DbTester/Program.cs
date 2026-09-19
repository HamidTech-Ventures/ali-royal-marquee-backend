using Npgsql;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        string connStr = "Host=localhost;Port=5434;Database=aliroyal;Username=postgres;Password=postgres";
        await using var conn = new NpgsqlConnection(connStr);
        await conn.OpenAsync();

        // Setup test data
        await Execute(conn, "DELETE FROM \"Bookings\"");
        await Execute(conn, "DELETE FROM \"Venues\"");
        await Execute(conn, "DELETE FROM \"Customers\"");
        
        Guid venueA = Guid.NewGuid();
        Guid venueB = Guid.NewGuid();
        Guid cust = Guid.NewGuid();
        
        await Execute(conn, $"INSERT INTO \"Customers\" (\"Id\", \"Name\", \"Phone\", \"Tier\", \"CreatedAt\") VALUES ('{cust}', 'Test', '123', 0, NOW())");
        await Execute(conn, $"INSERT INTO \"Venues\" (\"Id\", \"Name\", \"Capacity\", \"IsActive\", \"CreatedAt\") VALUES ('{venueA}', 'Venue A', 100, true, NOW())");
        await Execute(conn, $"INSERT INTO \"Venues\" (\"Id\", \"Name\", \"Capacity\", \"IsActive\", \"CreatedAt\") VALUES ('{venueB}', 'Venue B', 100, true, NOW())");

        Console.WriteLine("--- Test A: Allowed (Venue A, Non-overlapping) ---");
        try {
            await InsertBooking(conn, venueA, cust, "12:00", "16:00", "BK-1");
            await InsertBooking(conn, venueA, cust, "19:00", "23:30", "BK-2");
            Console.WriteLine("Test A passed.");
        } catch(Exception e) { Console.WriteLine("Test A failed: " + e.Message); }

        Console.WriteLine("--- Test B: Rejected (Venue A, Overlapping) ---");
        try {
            await InsertBooking(conn, venueA, cust, "15:00", "20:00", "BK-3");
            Console.WriteLine("Test B failed (Should have rejected)");
        } catch (PostgresException e) when (e.SqlState == "23P01") {
            Console.WriteLine("Test B passed (Constraint violation detected).");
        } catch(Exception e) { Console.WriteLine("Test B failed with unexpected error: " + e.Message); }

        Console.WriteLine("--- Test C: Different Venue ---");
        try {
            await InsertBooking(conn, venueB, cust, "15:00", "20:00", "BK-4");
            Console.WriteLine("Test C passed.");
        } catch(Exception e) { Console.WriteLine("Test C failed: " + e.Message); }

        Console.WriteLine("--- Test D: Boundary ---");
        try {
            await InsertBooking(conn, venueA, cust, "16:00", "18:00", "BK-5");
            Console.WriteLine("Test D passed (Boundary allowed).");
        } catch (PostgresException e) when (e.SqlState == "23P01") {
            Console.WriteLine("Test D failed (Constraint violation on exact boundary).");
        } catch(Exception e) { Console.WriteLine("Test D failed with unexpected error: " + e.Message); }

        Console.WriteLine("--- Test E: Concurrent Requests ---");
        try {
            var task1 = InsertBooking(conn, venueB, cust, "21:00", "23:00", "BK-6");
            
            await using var conn2 = new NpgsqlConnection(connStr);
            await conn2.OpenAsync();
            var task2 = InsertBooking(conn2, venueB, cust, "21:00", "23:00", "BK-7");
            
            await Task.WhenAll(task1, task2);
            Console.WriteLine("Test E failed: Both concurrent bookings succeeded!");
        } catch (Exception e) when (e is PostgresException pe && pe.SqlState == "23P01") {
            Console.WriteLine("Test E passed: Concurrent overlap rejected by DB engine!");
        } catch (Exception e) {
            if (e.InnerException is PostgresException pe2 && pe2.SqlState == "23P01")
                Console.WriteLine("Test E passed: Concurrent overlap rejected by DB engine (AggregateException).");
            else
                Console.WriteLine("Test E failed with unexpected error: " + e);
        }

        Console.WriteLine("--- Test F: Cancelled vs Confirmed ---");
        try
        {
            var testFBookingId1 = Guid.NewGuid();
            var testFBookingId2 = Guid.NewGuid();
            await using var cmdF1 = new NpgsqlCommand(@"
                INSERT INTO ""Bookings"" (""Id"", ""ReferenceNumber"", ""CustomerId"", ""VenueId"", ""BookingDate"", ""StartTime"", ""EndTime"", ""Status"", ""GuestCount"", ""TotalAmount"", ""CreatedAt"")
                VALUES (@id, 'F-1', @customerId, @venueId, NOW(), '2026-11-01 12:00:00+00', '2026-11-01 16:00:00+00', 3, 100, 50000, NOW());", conn);
            cmdF1.Parameters.AddWithValue("id", testFBookingId1);
            cmdF1.Parameters.AddWithValue("customerId", cust);
            cmdF1.Parameters.AddWithValue("venueId", venueA);
            await cmdF1.ExecuteNonQueryAsync();

            await using var cmdF2 = new NpgsqlCommand(@"
                INSERT INTO ""Bookings"" (""Id"", ""ReferenceNumber"", ""CustomerId"", ""VenueId"", ""BookingDate"", ""StartTime"", ""EndTime"", ""Status"", ""GuestCount"", ""TotalAmount"", ""CreatedAt"")
                VALUES (@id, 'F-2', @customerId, @venueId, NOW(), '2026-11-01 12:00:00+00', '2026-11-01 16:00:00+00', 1, 100, 50000, NOW());", conn);
            cmdF2.Parameters.AddWithValue("id", testFBookingId2);
            cmdF2.Parameters.AddWithValue("customerId", cust);
            cmdF2.Parameters.AddWithValue("venueId", venueA);
            await cmdF2.ExecuteNonQueryAsync();
            Console.WriteLine("Test F passed: Cancelled booking did not block a confirmed one!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Test F failed: {ex.Message}");
        }
    }

    static async Task Execute(NpgsqlConnection conn, string sql)
    {
        await using var cmd = new NpgsqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync();
    }
    
    static async Task InsertBooking(NpgsqlConnection conn, Guid venue, Guid cust, string start, string end, string refNum)
    {
        string sql = $@"INSERT INTO ""Bookings"" (""Id"", ""CustomerId"", ""VenueId"", ""ReferenceNumber"", ""BookingDate"", ""StartTime"", ""EndTime"", ""Status"", ""GuestCount"", ""TotalAmount"", ""CreatedAt"") 
                        VALUES ('{Guid.NewGuid()}', '{cust}', '{venue}', '{refNum}', NOW(), '2026-10-01 {start}:00Z', '2026-10-01 {end}:00Z', 0, 100, 1000, NOW())";
        await Execute(conn, sql);
    }
}
