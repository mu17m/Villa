using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.SD;
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
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult FinalizeBooking(int VillaId, int Year, int Month, int Day, int nights)
        // I Used 3 parameters of date rather than one parameter of DateOnly because I was getting an error 
        //public IActionResult FinalizeBooking(int VillaId, DateOnly checkInDate, int nights)
        {
            var ClaimIdentity = (ClaimsIdentity)User.Identity;
            var UserId = ClaimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ApplicationUser user = _unitOfWork.UserRepo.Get(u => u.Id == UserId);
            Booking booking = new()
            {
                VillaId = VillaId,
                Villa = _unitOfWork.VillaRepo.Get(v => v.Id == VillaId),
                UserId = UserId,
                User = user,
                Nights = nights,
                CheckInDate = new DateOnly(Year, Month, Day),
                //CheckInDate = checkInDate,
                //CheckInDate = checkInDate.AddDays(nights)
                Name = user.Name,
                Email = user.Email,
                Phone = user.PhoneNumber
            };
            booking.CheckOutDate = booking.CheckInDate.AddDays(nights);
            booking.TotalCost = (int) booking.Villa.Price * nights;
            return View(booking);
        }
        [Authorize]
        [HttpPost]
        public IActionResult FinalizeBooking(Booking booking)
        {
            var villa = _unitOfWork.VillaRepo.Get(v => v.Id == booking.VillaId);
            booking.Status = SD.StatusPending;
            booking.BookingDate = DateOnly.FromDateTime(DateTime.Now);
            booking.Villa = villa;
            _unitOfWork.BookingRepo.Add(booking);
            _unitOfWork.Save();

            var domin = Request.Scheme + "://" + Request.Host.Value + "/";
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domin + $"Booking/BookingConfirmation?BookingId={booking.Id}",
                CancelUrl = domin + $"Booking/FinalizeBooking?VillaId={booking.VillaId}&Year={booking.CheckInDate.Year}&Month={booking.CheckInDate.Month}&Day={booking.CheckInDate.Day}&nights={booking.Nights}",
            };
                options.LineItems.Add(new SessionLineItemOptions
                {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)booking.TotalCost * 100,
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = booking.Villa.Name
                    }
                },
                    Quantity = 1
                });
            
            var service = new SessionService();
            Session session = service.Create(options);
            _unitOfWork.BookingRepo.UpdateStripePaymentIntentId(booking.Id, session.Id,session.PaymentIntentId);
            _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);

            return new StatusCodeResult(303);
        }
        public IActionResult BookingConfirmation(int BookingId)
        {
            Booking bookingFromDb = _unitOfWork.BookingRepo.Get(b => b.Id == BookingId);
            if(bookingFromDb.Status == SD.StatusPending)
            {
                //we should make status approved
                var service = new SessionService();
                Session session = service.Get(bookingFromDb.StripSessionId);
                if(session.PaymentStatus == "paid")
                {
                    _unitOfWork.BookingRepo.UpdateBookingStatus(BookingId, SD.StatusApproved);
                    _unitOfWork.BookingRepo.UpdateStripePaymentIntentId(BookingId, session.Id, session.PaymentIntentId);
                    _unitOfWork.Save();
                }
            }
            return View(BookingId);
        }
        [Authorize]
        public IActionResult Details(int Id)
        {
            Booking bookingFromDb = _unitOfWork.BookingRepo.Get(b => b.Id == Id, includeProperties: "Villa,User");
            return View(bookingFromDb);
        }

        #region API calls
        [HttpGet]
        [Authorize]
        public IActionResult GetAllBookings(string? status)
        {
            IEnumerable<Booking> bookings;

            if(User.IsInRole(SD.Role_Admin))
            {
                bookings = _unitOfWork.BookingRepo.GetAll(includeProperties: "Villa");

            }
            else
            {
                var ClaimIdentity = (ClaimsIdentity)User.Identity;
                var UserId = ClaimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                bookings = _unitOfWork.BookingRepo.GetAll(b => b.UserId==UserId, includeProperties: "Villa");
            }
            if(status != null && status != "All")
            {
                bookings = bookings.Where(b => b.Status == status);
            }
            return Json(new { data = bookings });
        }
        #endregion

    }
}
