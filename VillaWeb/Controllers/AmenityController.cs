using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.SD;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;
using WhiteLagoon.Web.ViewModels;
namespace WhiteLagoon.Web.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    public class AmenityController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public AmenityController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var Amenitys = _unitOfWork.AmenityRepo.GetAll(includeProperties:"Villa");
            return View(Amenitys);
        }
        public IActionResult Create()
        {
            AmenityVM createVM = new AmenityVM();
            createVM.list = _unitOfWork.VillaRepo.GetAll().Select(
                v => new SelectListItem
                {
                    Text = v.Name,
                    Value = v.Id.ToString()
                });
            return View(createVM);
        }
        [HttpPost]
        public IActionResult Create(AmenityVM objVM)
        {
            //bool IsAmenityExists = _unitOfWork.AmenityRepo.Any(v => v.Id == objVM.amenity.Id);
            //if(IsAmenityExists)
            //{
            //    TempData["Error"] = "Villa Number is already exists";
            //    objVM.list = _unitOfWork.VillaRepo.GetAll().Select(
            //        v => new SelectListItem
            //        {
            //            Text = v.Name,
            //            Value = v.Id.ToString()
            //        });
            //    return View(objVM);
            //}
            if (ModelState.IsValid )
            {
                _unitOfWork.AmenityRepo.Add(objVM.amenity);
                _unitOfWork.Save();
                TempData["Success"] = "Amenity created successfully";
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "An error occurred";
            objVM.list = _unitOfWork.VillaRepo.GetAll().Select(
                v => new SelectListItem
                {
                    Text = v.Name,
                    Value = v.Id.ToString()
                });
            return View(objVM);
        }
        public IActionResult Update(int id)
        {
            AmenityVM villaNumberVM = new AmenityVM();
            villaNumberVM.list = _unitOfWork.VillaRepo.GetAll().Select(
                v => new SelectListItem
                {
                    Text = v.Name,
                    Value = v.Id.ToString()
                });
            villaNumberVM.amenity = _unitOfWork.AmenityRepo.Get(v => v.Id == id);
            
            if(villaNumberVM.amenity == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(villaNumberVM);
        }
        [HttpPost]
        public  IActionResult Update(AmenityVM villaNumberVM)
        {
            if(ModelState.IsValid && villaNumberVM.amenity != null)
            {
                _unitOfWork.AmenityRepo.Update(villaNumberVM.amenity);
                _unitOfWork.Save();
                TempData["Success"] = "Amenity updated successfully";
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "An error occurred";
            villaNumberVM.list = _unitOfWork.VillaRepo.GetAll().Select(
                    v => new SelectListItem
                    {
                        Text = v.Name,
                        Value = v.Id.ToString()
                    });
            return View(villaNumberVM);
        }
        public IActionResult Delete(int id)
        {
            Amenity? objFromDb = _unitOfWork.AmenityRepo.Get(v => v.Id == id);
            objFromDb.Villa = _unitOfWork.VillaRepo.Get(v => v.Id == objFromDb.VillaId);
            if(objFromDb == null || objFromDb.Villa == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View (objFromDb);
        }
        [HttpPost]
        public IActionResult Delete(Amenity obj)
        {
            Amenity? objFromDb = _unitOfWork.AmenityRepo.Get(v => v.Id == obj.Id);
            if(objFromDb == null)
            {
                TempData["Error"] = "Amenity not found";
                return View(obj);
            }
            _unitOfWork.AmenityRepo.Remove(objFromDb);
            _unitOfWork.Save();
            TempData["Success"] = "Amenity deleted successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
