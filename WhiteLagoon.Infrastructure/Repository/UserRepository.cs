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
    public class UserRepository : Repository<ApplicationUser>, IUserRepository
    {
        private readonly ApplicationDbContext _Db;
        public UserRepository(ApplicationDbContext Db) : base(Db)
        {
            _Db = Db;
        }
        public void Update(ApplicationUser user)
        {
            _Db.ApplicationUsers.Update(user);
        }
    }
}
