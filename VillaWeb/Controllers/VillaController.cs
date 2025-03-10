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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        public VillaController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            var Villas = _unitOfWork.VillaRepo.GetAll();
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
                if (obj.Image != null)
                {
                    // FIleName is the Name of Image
                    string FileName = Guid.NewGuid().ToString() + Path.GetExtension(obj.Image.FileName);
                    // path is the image location
                    string path = Path.Combine(_hostEnvironment.WebRootPath, @"Images\VillaImage");
                    // fileStream is the image file created and then we should copy the image from obj.Image to the fileStream
                    using (var fileStream = new FileStream(Path.Combine(path, FileName), FileMode.Create))
                    {
                        obj.Image.CopyTo(fileStream);
                        obj.ImageUrl = @"\Images\VillaImage\" + FileName;
                    }
                }
                else
                {
                    obj.ImageUrl = "https://placeholder.co/600x400";
                }
                _unitOfWork.VillaRepo.Add(obj);
                _unitOfWork.Save();
                TempData["Success"] = "Villa created successfully";
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "Not valid";
            return View(obj);
        }
        public IActionResult Update(int id)
        {
            Villa? ObjFromDb = _unitOfWork.VillaRepo.Get(v => v.Id == id);
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
                if (obj.Image != null)
                {
                    string FileName = Guid.NewGuid().ToString() + Path.GetExtension(obj.Image.FileName);
                    string path = Path.Combine(_hostEnvironment.WebRootPath, @"Images\VillaImage");

                    if (!string.IsNullOrEmpty(obj.ImageUrl))
                    {
                        string oldPath = Path.Combine(_hostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(path, FileName), FileMode.Create))
                    {
                        obj.Image.CopyTo(fileStream);
                        obj.ImageUrl = @"\Images\VillaImage\" + FileName;
                    }
                }

                _unitOfWork.VillaRepo.Update(obj);
                _unitOfWork.Save();
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
            Villa? objFromDb = _unitOfWork.VillaRepo.Get(v => v.Id == obj.Id);
            if(objFromDb == null)
            {
                TempData["Error"] = "Villa not found";
                return RedirectToAction("Error", "Home");
            }
            
            if (!string.IsNullOrEmpty(objFromDb.ImageUrl))
            {
                string oldPath = Path.Combine(_hostEnvironment.WebRootPath, objFromDb.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
            }
            _unitOfWork.VillaRepo.Remove(objFromDb);
            _unitOfWork.Save();
            TempData["Success"] = "Villa deleted successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
