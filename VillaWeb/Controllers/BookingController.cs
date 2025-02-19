using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Web.Controllers
{
    public class BookingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookingController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult FinalizeBooking(int nights, DateOnly CheckInDate, int VillaId)
        {
            Booking booking = new()
            {
                CheckInDate = CheckInDate,
                CheckOutDate = CheckInDate.AddDays(nights),
                Nights = nights,
                VillaId = VillaId,
                Villa = _unitOfWork.VillaRepo.Get(v => v.Id == VillaId)
            };
            booking.TotalCost = (int) booking.Villa.Price * nights;
            return View(booking);
        }
    }
}
