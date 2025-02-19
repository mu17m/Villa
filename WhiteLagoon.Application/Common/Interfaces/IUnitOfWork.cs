using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhiteLagoon.Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        public IVillaRepository VillaRepo { get;  }
        public IVillaNumberRepository VillaNumberRepo { get; }
        public IAmenityRepository AmenityRepo { get; }
        public IBookingRepository BookingRepo { get; }
        public void Save();
    }
}
