using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceBookingSystemAPI.Models;

namespace ServiceBookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public readonly ApplicationDbContext Context;

        public UserController(ApplicationDbContext dbContext)
        {
            this.Context = dbContext;
        }
        [HttpGet]
        public async Task<IActionResult> GetApprovedServices()
        {
            var services = await Context.Services
                .Where(s => s.IsApproved == true)
                .ToListAsync();

            return Ok(services);
        }

        //[Authorize(Roles = "User")]
        [HttpPost("bookings")]
        public async Task<IActionResult> BookService([FromBody] BookingDto bookingDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var booking = new Booking
            {
                UserId = bookingDto.UserId,
                ServiceId = bookingDto.ServiceId,
                BookingDate = bookingDto.BookingDate,
                Status = bookingDto.Status
            };

            Context.Bookings.Add(booking);
            await Context.SaveChangesAsync();

            return Ok(new { message = "Booking successful", booking });
        }


        [Authorize]
        [HttpGet("bookings/{userId}")]
        public async Task<IActionResult> GetUserBookings(int userId)
        {
            var bookings = await Context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.ServiceId)
                .ToListAsync();

            if (bookings == null || !bookings.Any())
            {
                return NotFound("No bookings found for this user.");
            }

            return Ok(bookings);
        }
    }
}
