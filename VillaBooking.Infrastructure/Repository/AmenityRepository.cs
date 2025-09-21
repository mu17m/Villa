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
    public class AmenityRepository : Repository<Amenity>, IAmenityRepository
    {
        private readonly ApplicationDbContext _Db;
        public AmenityRepository(ApplicationDbContext Db) : base(Db)
        {
            _Db = Db;
        }
        void IAmenityRepository.Update(Amenity entity)
        {
            _Db.Amenities.Update(entity);
        }
    }
}
