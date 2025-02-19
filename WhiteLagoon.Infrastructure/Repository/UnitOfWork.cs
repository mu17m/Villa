using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public IVillaRepository VillaRepo { get; private set; }

        public IVillaNumberRepository VillaNumberRepo { get; private set; }

        public IAmenityRepository AmenityRepo { get; private set; }

        public IBookingRepository BookingRepo { get; private set; }

        private readonly ApplicationDbContext _Db;
        public UnitOfWork(ApplicationDbContext Db)
        {
            _Db = Db;
            VillaRepo = new VillaRepository(_Db);
            VillaNumberRepo = new VillaNumberRepository(_Db);
            AmenityRepo = new AmenityRepository(_Db);
            BookingRepo = new BookingRepository(_Db);
        }

        public void Save()
        {
            _Db.SaveChanges();
        }
    }
}
