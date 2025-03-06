using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Application.Common.Interfaces
{
    public interface IBookingRepository : IRepository<Booking>
    {
        public void Update(Booking entity);
        public void UpdateBookingStatus(int BookingId, string BookingStatus, int VillaNumber);
        public void UpdateStripePaymentIntentId(int BookingId,string SessionId, string PaymentIntentId);
    }
}
