using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Web.Controllers
{
    public class VillaController : Controller
    {
        private readonly ApplicationDbContext _Db;
        public VillaController(ApplicationDbContext Db)
        {
            _Db = Db;
        }
        public IActionResult Index()
        {
            var Villas = _Db.Villas.ToList();
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
                ModelState.AddModelError("Description", "Name and Description cannot be the same");
            }
            if (ModelState.IsValid)
            {
                _Db.Villas.Add(obj);
                _Db.SaveChanges();
                TempData["Success"] = "Villa created successfully";
                return RedirectToAction("index");
            }
            TempData["Error"] = "An error occurred";
            return View(obj);
        }
        public IActionResult Update(int id)
        {
            Villa? ObjFromDb = _Db.Villas.FirstOrDefault(v => v.Id == id);
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
                ModelState.AddModelError("Description", "Name and Description cannot be the same");
            }
            if(ModelState.IsValid)
            {
                _Db.Villas.Update(obj);
                _Db.SaveChanges();
                TempData["Success"] = "Villa updated successfully";
                return RedirectToAction("index");
            }
            TempData["Error"] = "An error occurred";
            return View(obj);
        }

        public IActionResult Delete(int id)
        {
            Villa? objFromDb = _Db.Villas.FirstOrDefault(v => v.Id == id);
            if(objFromDb == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View (objFromDb);
        }
        [HttpPost]
        public IActionResult Delete(Villa obj)
        {
            Villa? objFromDb = _Db.Villas.FirstOrDefault(v => v.Id == obj.Id);
            if(objFromDb == null)
            {
                TempData["Error"] = "Villa not found";
                return RedirectToAction("Error", "Home");
            }
            _Db.Villas.Remove(objFromDb);
            _Db.SaveChanges();
            TempData["Success"] = "Villa deleted successfully";
            return RedirectToAction("index");
        }
    }
}
