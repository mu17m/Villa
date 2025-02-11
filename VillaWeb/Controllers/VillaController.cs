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
                return RedirectToAction("index");
            }
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
    }
}
