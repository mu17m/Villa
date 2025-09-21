using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.SD;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Web.ViewModels;

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

            var villaNumbers = _unitOfWork.VillaNumberRepo.GetAll().ToList();
            var bookings = _unitOfWork.BookingRepo.GetAll(b => b.Status == SD.StatusApproved || 
            b.Status == SD.StatusCheckedIn).ToList();
            
            int AvailableRooms = SD.VillaRoomsAvailableCount(villa.Id, villaNumbers, bookings, booking.Nights, booking.CheckInDate);
            villa.IsAvilable = AvailableRooms > 0 ? true : false;
            
            if(villa.IsAvilable == false)
            {
                TempData["Error"] = "Booking Failed: Villa is not avilable for the selected dates";
                return RedirectToAction(nameof(FinalizeBooking),
                    new{
                        Villas = booking.Villa,
                        CheckInDate = booking.CheckInDate,
                        Nights = booking.Nights
                    });
            
            }

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
                    _unitOfWork.BookingRepo.UpdateBookingStatus(BookingId, SD.StatusApproved, 0);
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
            if(bookingFromDb.VillaNumber == 0)
            {
                // returns the villa numbers as int
                var avilableVillaNumbers = AvilableVillaNumbers(bookingFromDb.VillaId);
                // assign VillaNumbers member of Booking model
                bookingFromDb.villaNumbers = _unitOfWork.VillaNumberRepo.GetAll(vn => vn.VillaId == bookingFromDb.VillaId
                && avilableVillaNumbers.Any(avilableVillaNumbers => avilableVillaNumbers == vn.Villa_Number)).ToList();
            }
            return View(bookingFromDb);
        }
        [HttpPost]
        [Authorize(Roles =SD.Role_Admin)]
        public IActionResult CheckIn(Booking booking)
        {
            _unitOfWork.BookingRepo.UpdateBookingStatus(booking.Id, SD.StatusCheckedIn, booking.VillaNumber);
            _unitOfWork.Save();
            TempData["Success"] = "CheckIn Successful";
            return RedirectToAction(nameof(Details), new {Id = booking.Id});
        }
        public IActionResult CheckOut(Booking booking)
        {
            _unitOfWork.BookingRepo.UpdateBookingStatus(booking.Id, SD.StatusCompleted, booking.VillaNumber);
            _unitOfWork.Save();
            TempData["Success"] = "Booking Completed Successful";
            return RedirectToAction(nameof(Details), new {Id = booking.Id});
        }
        public IActionResult Cancel(Booking booking)
        {
            _unitOfWork.BookingRepo.UpdateBookingStatus(booking.Id, SD.StatusCancelled, 0);
            _unitOfWork.Save();
            TempData["Success"] = "Booking Canclled Successful";
            return RedirectToAction(nameof(Details), new {Id = booking.Id});
        }
        private List<int> AvilableVillaNumbers(int VillaId)
        {
            List<int> avilableVillaNumbers = new();
            var villaNumbers = _unitOfWork.VillaNumberRepo.GetAll(vn => vn.VillaId == VillaId);
            // returns all the villa numbers that are booked and checked in and if the villa number is not in the list of booked villa numbers then add it to the avilableVillaNumbers list
            var AllBookedVillaNumbers = _unitOfWork.BookingRepo.GetAll(b => b.VillaId == VillaId && b.Status == SD.StatusCheckedIn)
                .Select(b => b.VillaNumber);

            foreach(var villaNumber in villaNumbers)
            {
                if (!AllBookedVillaNumbers.Contains(villaNumber.Villa_Number))
                {
                    avilableVillaNumbers.Add(villaNumber.Villa_Number);
                }
            }
            return avilableVillaNumbers;
        }
        public IActionResult ExportBooking(int id)
        {
            Booking booking = _unitOfWork.BookingRepo.Get(b => b.Id == id, includeProperties:"User");
            booking.Villa = _unitOfWork.VillaRepo.Get(v => v.Id == booking.VillaId);
            if(booking == null)
            {
                return RedirectToAction("Error", "Home");
            }
            string templatePath = "wwwroot/templates/BookingDetails.docx"; // Ensure this file exists

            if (string.IsNullOrEmpty(booking.Phone))
            {
                booking.Phone = "";
            }
            var replacements = new Dictionary<string, string>
    {
        { "xx_booking_Number", "Booking ID - " + booking.Id.ToString()},
        { "xx_BOOKING_Date", booking.BookingDate.ToShortDateString() },
        { "xx_customer_name", booking.User.Name },
        { "xx_customer_phone", booking.Phone },
        { "xx_customer_email", booking.Email },
        { "xx_payment_date", booking.PaymentDate.ToShortDateString() },
        { "xx_checkin_date", booking.CheckInDate.ToShortDateString() },
        { "xx_checkout_date", booking.CheckOutDate.ToShortDateString() },
        { "xx_booking_total", booking.TotalCost.ToString("c") }
    };

            // Sample table data
            var tableData = new List<List<string>>
    {
        new List<string> { "NIGHTS", "VILLA", "PRICE PER NIGHT", "TOTAL" },
        new List<string> { booking.Nights.ToString(), booking.Villa.Name, (booking.TotalCost/booking.Nights).ToString("c"), booking.TotalCost.ToString("c") }
    };

            WordExportService wordExportService = new WordExportService();
            byte[] fileContents = wordExportService.GenerateBookingDocument(templatePath, replacements, tableData);

            return File(fileContents, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "BookingDetails.docx");
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
