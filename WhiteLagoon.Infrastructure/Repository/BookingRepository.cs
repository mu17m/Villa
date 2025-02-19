using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Application.Common.Interfaces;
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
    }
}
