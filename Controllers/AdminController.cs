using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceBookingSystem.Repository;
using ServiceBookingSystemAPI.Models;

namespace ServiceBookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]  
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly DBAccessLayer _dBAccessLayer;
        public AdminController(ApplicationDbContext context, DBAccessLayer dBAccessLayer)
        {
            _context = context;
            _dBAccessLayer = dBAccessLayer;
        }

        [HttpPost("Addservices")]
        public async Task<IActionResult> AddService([FromBody] Service service)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                _context.Services.Add(service);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Service added successfully, pending approval.", service });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
        [HttpGet("GetServices")]
        public async Task<IActionResult> GetServices()
        {
            var services = await _context.Services
                .ToListAsync();

            return Ok(services);
        }

        [HttpPut("services/{id}")]
        public async Task<IActionResult> ServiceStatus([FromRoute] int id, [FromBody] bool isApproved)
        {
            try
            {
                var service = await _context.Services.FindAsync(id);
                if (service == null)
                {
                    return NotFound("Service not found.");
                }

                service.IsApproved = isApproved;
                _context.Services.Update(service);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Service approval status updated.", service });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpGet("ServiceBookings")]
        public async Task<IActionResult> GetPendingBookings()
        {
            var data = await _dBAccessLayer.GetBookedOrdersAsync();

            return Ok(data);
        }
        [HttpPut("bookings/{id}")]
        public async Task<IActionResult> UpdateBookingStatus(int id, [FromBody] string status)
        {
            try
            {
                var booking = await _context.Bookings.FindAsync(id);
                if (booking == null)
                {
                    return NotFound("Booking not found.");
                }

                if (status != "Approved" && status != "Rejected")
                {
                    return BadRequest("Invalid status.");
                }

                booking.Status = status;
                await _context.SaveChangesAsync();

                return Ok(new { message = $"Booking has been {status.ToLower()}." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpDelete("services/{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            try
            {
                var service = await _context.Services.FindAsync(id);
                if (service == null)
                {
                    return NotFound("Service not found.");
                }

                _context.Services.Remove(service);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Service deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpDelete("bookings/{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            try
            {
                var booking = await _context.Bookings.FindAsync(id);
                if (booking == null)
                {
                    return NotFound("Booking not found.");
                }

                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Booking deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
    }
}
