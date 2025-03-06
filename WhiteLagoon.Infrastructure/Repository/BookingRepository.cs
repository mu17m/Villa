using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.SD;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Infrastructure.Repository
{
    public class BookingRepository : Repository<Booking>, IBookingRepository
    {
        private readonly ApplicationDbContext _Db;
        public BookingRepository(ApplicationDbContext Db) : base(Db)
        {
            _Db = Db;
        }
        public void Update(Booking entity)
        {
            _Db.Bookings.Update(entity);
        }

        public void UpdateBookingStatus(int BookingId, string BookingStatus, int VillaNumber=0)
        {
            var BookingFromDb = _Db.Bookings.FirstOrDefault(b => b.Id == BookingId);
            if(BookingFromDb != null)
            {
                BookingFromDb.Status = BookingStatus;
                BookingFromDb.VillaNumber = VillaNumber;
                if (BookingFromDb.Status == SD.StatusCheckedIn)
                {
                    BookingFromDb.AcutalCheckInDate = DateTime.Now;
                }
                if(BookingFromDb.Status == SD.StatusCompleted)
                {
                    BookingFromDb.AcutalCheckOutDate = DateTime.Now;
                }
            }
        }
        public void UpdateStripePaymentIntentId(int BookingId, string SessionId, string PaymentIntentId)
        {
            var BookingFromDb = _Db.Bookings.FirstOrDefault(b => b.Id == BookingId);
            if(BookingFromDb != null)
            {
                if(!string.IsNullOrEmpty(SessionId))
                {
                    BookingFromDb.StripSessionId = SessionId;
                }
                if(!string.IsNullOrEmpty(PaymentIntentId))
                {
                    BookingFromDb.StripPaymentIntentId = PaymentIntentId;
                    BookingFromDb.PaymentDate = DateTime.Now;
                    BookingFromDb.IsPaymentSuccessful = true;
                }
            }
        }
    }
}
