using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhiteLagoon.Domain.Entities
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
        [ForeignKey("Villa")]
        public int VillaId { get; set; }
        public Villa Villa { get; set; } = null!;
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }

        [Required]
        public double TotalCost { get; set; }
        public int Nights { get; set; }
        public string? Status { get; set; }
        [Required]
        public DateOnly BookingDate { get; set; }
        
        [Required]
        public DateOnly CheckInDate { get; set; }
        [Required]
        public DateOnly CheckOutDate { get; set; }

        public bool IsPaymentSuccessful { get; set; } = false;
        public DateTime PaymentDate { get; set; }
        
        public string? StripSessionId { get; set; }
        public string? StripPaymentIntentId { get; set; }

        public DateTime AcutalCheckInDate { get; set; }
        public DateTime AcutalCheckOutDate { get; set; }
        public int VillaNumber { get; set; }
        [NotMapped]
        public List<VillaNumber>? villaNumbers { get; set; }
    }
}
