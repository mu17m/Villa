using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Web.ViewModels
{
    public class AmenityVM
    {
        public IEnumerable<SelectListItem>? list { get; set; }
        public Amenity? amenity{ get; set; }
    }
}
