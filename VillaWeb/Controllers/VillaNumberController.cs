using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;
using WhiteLagoon.Web.ViewModels;
namespace WhiteLagoon.Web.Controllers
{
    public class VillaNumberController : Controller
    {
        private readonly ApplicationDbContext _Db;
        public VillaNumberController(ApplicationDbContext Db)
        {
            _Db = Db;
        }
        public IActionResult Index()
        {
            var VillaNumbers = _Db.VillaNumbers.Include(VNumber => VNumber.villa).ToList();
            return View(VillaNumbers);
        }
        public IActionResult Create()
        {
            VillaNumberVM createVM = new VillaNumberVM();
            createVM.list = _Db.Villas.ToList().Select(
                v => new SelectListItem
                {
                    Text = v.Name,
                    Value = v.Id.ToString()
                });
            createVM.villaNumber = new VillaNumber();
            return View(createVM);
        }
        [HttpPost]
        public IActionResult Create(VillaNumberVM objVM)
        {
            bool IsVillaNumberExists = _Db.VillaNumbers.Any(v => v.Villa_Number == objVM.villaNumber.Villa_Number);
            if(IsVillaNumberExists)
            {
                TempData["Error"] = "Villa Number is already exists";
                objVM.list = _Db.Villas.ToList().Select(
                    v => new SelectListItem
                    {
                        Text = v.Name,
                        Value = v.Id.ToString()
                    });
                return View(objVM);
            }
            if (ModelState.IsValid && !IsVillaNumberExists)
            {
                _Db.VillaNumbers.Add(objVM.villaNumber);
                _Db.SaveChanges();
                TempData["Success"] = "Villa Number created successfully";
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "An error occurred";
            objVM.list = _Db.Villas.ToList().Select(
                v => new SelectListItem
                {
                    Text = v.Name,
                    Value = v.Id.ToString()
                });
            return View(objVM);
        }
        public IActionResult Update(int id)
        {
            VillaNumberVM villaNumberVM = new VillaNumberVM();
            villaNumberVM.list = _Db.Villas.ToList().Select(
                v => new SelectListItem
                {
                    Text = v.Name,
                    Value = v.Id.ToString()
                });
            villaNumberVM.villaNumber = _Db.VillaNumbers.FirstOrDefault(v => v.Villa_Number == id);
            
            if(villaNumberVM.villaNumber == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(villaNumberVM);
        }
        [HttpPost]
        public  IActionResult Update(VillaNumberVM villaNumberVM)
        {
            if(ModelState.IsValid && villaNumberVM.villaNumber != null)
            {
                _Db.VillaNumbers.Update(villaNumberVM.villaNumber);
                _Db.SaveChanges();
                TempData["Success"] = "Villa Number updated successfully";
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "An error occurred";
            villaNumberVM.list = _Db.Villas.ToList().Select(
                    v => new SelectListItem
                    {
                        Text = v.Name,
                        Value = v.Id.ToString()
                    });
            return View(villaNumberVM);
        }

        public IActionResult Delete(int id)
        {
            VillaNumber? objFromDb = _Db.VillaNumbers.FirstOrDefault(v => v.Villa_Number == id);
            objFromDb.villa = _Db.Villas.FirstOrDefault(v => v.Id == objFromDb.VillaId);
            if(objFromDb == null || objFromDb.villa == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View (objFromDb);
        }
        [HttpPost]
        public IActionResult Delete(VillaNumber obj)
        {
            VillaNumber? objFromDb = _Db.VillaNumbers.FirstOrDefault(v => v.Villa_Number == obj.Villa_Number);
            if(objFromDb == null)
            {
                TempData["Error"] = "Villa Number not found";
                return View(obj);
            }
            _Db.VillaNumbers.Remove(objFromDb);
            _Db.SaveChanges();
            TempData["Success"] = "Villa Number deleted successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
