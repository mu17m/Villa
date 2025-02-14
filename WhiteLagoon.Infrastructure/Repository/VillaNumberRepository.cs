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
    public class VillaNumberRepository : Repository<VillaNumber>, IVillaNumberRepository
    {
        private readonly ApplicationDbContext _Db;
        public VillaNumberRepository(ApplicationDbContext Db) : base(Db)
        {
            _Db = Db;
        }
        public void Update(VillaNumber entity)
        {
            _Db.VillaNumbers.Update(entity);
        }
    }
}
