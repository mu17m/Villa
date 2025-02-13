using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;
using WhiteLagoon.Infrastructure.Repository;

namespace WhiteLagoon.Web.Controllers
{
    public class VillaController : Controller
    {
        private readonly IVillaRepository _VillaRepo;
        public VillaController(IVillaRepository villaRepo)
        {
            _VillaRepo = villaRepo;
        }
        public IActionResult Index()
        {
            var Villas = _VillaRepo.GetAll();
            return View(Villas);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Villa obj)
        {
            if(obj.Name == obj.Description)
            {
                TempData["Error"] = "Name and Description cannot be the same";
            }
            if (ModelState.IsValid)
            {
                _VillaRepo.Add(obj);
                _VillaRepo.Save();
                TempData["Success"] = "Villa created successfully";
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "An error occurred";
            return View(obj);
        }
        public IActionResult Update(int id)
        {
            Villa? ObjFromDb = _VillaRepo.Get(v => v.Id == id);
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
            if(ModelState.IsValid)
            {
                _VillaRepo.Update(obj);
                _VillaRepo.Save();
                TempData["Success"] = "Villa updated successfully";
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "An error occurred";
            return View(obj);
        }
        public IActionResult Delete(int id)
        {
            Villa? objFromDb = _VillaRepo.Get(v => v.Id == id);
            if(objFromDb == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View (objFromDb);
        }
        [HttpPost]
        public IActionResult Delete(Villa obj)
        {
            Villa? objFromDb = _VillaRepo.Get(v => v.Id == obj.Id);
            if(objFromDb == null)
            {
                TempData["Error"] = "Villa not found";
                return RedirectToAction("Error", "Home");
            }
            _VillaRepo.Remove(objFromDb);
            _VillaRepo.Save();
            TempData["Success"] = "Villa deleted successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
