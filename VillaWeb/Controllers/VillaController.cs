using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.SD;
using WhiteLagoon.Application.Services.interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;
using WhiteLagoon.Infrastructure.Repository;

namespace WhiteLagoon.Web.Controllers
{
    [Authorize]
    public class VillaController : Controller
    {
        private readonly IVillaService _villaService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        public VillaController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment, IVillaService villaService)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
            _villaService = villaService;
        }
        public IActionResult Index()
        {
            var Villas = _villaService.GetAllVillas();
            return View(Villas);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Villa obj)
        {
            if (ModelState.IsValid)
            {
                _villaService.CreateVilla(obj);
                TempData["Success"] = "Villa created successfully";
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "Not valid";
            return View(obj);
        }
        public IActionResult Update(int id)
        {
            Villa? ObjFromDb = _villaService.GetVillaById(id);
            if(ObjFromDb == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(ObjFromDb);
        }
        [HttpPost]
        public  IActionResult Update(Villa obj)
        {
            if(obj.Name == obj.Description)
            {
                TempData["Error"] = "Name and Description cannot be the same";
            }
            if (ModelState.IsValid)
            {
                _villaService.UpdateVilla(obj);
                TempData["Success"] = "Villa updated successfully";
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "An error occurred";
            return View(obj);
        }
        public IActionResult Delete(int id)
        {
            Villa? objFromDb = _unitOfWork.VillaRepo.Get(v => v.Id == id);
            if(objFromDb == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View (objFromDb);
        }
        [HttpPost]
        public IActionResult Delete(Villa obj)
        {
            bool isDeleted = _villaService.DeleteVilla(obj.Id);
            if(!isDeleted)
            {
                TempData["Error"] = "Villa not found";
                return RedirectToAction("Error", "Home");
            }
            TempData["Success"] = "Villa deleted successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
