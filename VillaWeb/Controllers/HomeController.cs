using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Web.ViewModels;
using WhiteLagoonWeb.Models;

namespace WhiteLagoonWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            HomeVM homeVM = new HomeVM()
            {
                Villas = _unitOfWork.VillaRepo.GetAll(includeProperties:"VillaAmenities"),
                CheckInDate = DateOnly.FromDateTime(DateTime.Now),
                Nights = 1
            };
            return View(homeVM);
        }
        [HttpPost]
        public IActionResult GetVillasByDate(DateOnly CheckInDate, int Nights)
        {
            IEnumerable<Villa> villas = _unitOfWork.VillaRepo.GetAll(includeProperties: "VillaAmenities");
            foreach(var villa in villas)
            {
                if (villa.Id % 2 == 0)
                {
                    villa.IsAvilable = false;
                }
            }
            HomeVM homeVM = new HomeVM()
            {
                Villas = villas,
                CheckInDate = CheckInDate,
                Nights = Nights
            };
            return PartialView("_VillaList", homeVM);

        }
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Error()
        {
            return View();
        }
    }
}
