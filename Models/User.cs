using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ServiceBookingSystemAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace ServiceBookingSystemAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(50)]
        [EmailAddress]
        public string Email { get; set; }
        [StringLength(200)]
        public string Password { get; set; }
        public string Role { get; set; }
    }

    public class Service
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(200)]
        public string Name { get; set; }
        [Required]
        [StringLength(500)]
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int Duration { get; set; }
        [Required]
        public bool IsApproved { get; set; }
    }

    public class Booking
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ServiceId { get; set; }
        public DateTime BookingDate { get; set; }
        [Required]
        public string Status { get; set; }

        public User User { get; set; }

        public Service Service { get; set; }
    }
    public class BookingDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int ServiceId { get; set; }

        [Required]
        public DateTime BookingDate { get; set; }

        [Required]
        public string Status { get; set; }
    }

}

