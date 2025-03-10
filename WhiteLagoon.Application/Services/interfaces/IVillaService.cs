using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Application.Services.interfaces
{
    public interface IVillaService
    {
        public void CreateVilla(Villa villa);
        public IEnumerable<Villa> GetAllVillas();
        public Villa GetVillaById(int id);
        public void UpdateVilla(Villa villa);
        public bool DeleteVilla(int id);
    }
}
