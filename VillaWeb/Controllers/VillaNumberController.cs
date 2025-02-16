using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;
using WhiteLagoon.Web.ViewModels;
namespace WhiteLagoon.Web.Controllers
{
    [Authorize]
    public class VillaNumberController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public VillaNumberController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var VillaNumbers = _unitOfWork.VillaNumberRepo.GetAll(includeProperties:"villa");
            return View(VillaNumbers);
        }
        public IActionResult Create()
        {
            VillaNumberVM createVM = new VillaNumberVM();
            createVM.list = _unitOfWork.VillaRepo.GetAll().Select(
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
            bool IsVillaNumberExists = _unitOfWork.VillaNumberRepo.Any(v => v.Villa_Number == objVM.villaNumber.Villa_Number);
            if(IsVillaNumberExists)
            {
                TempData["Error"] = "Villa Number is already exists";
                objVM.list = _unitOfWork.VillaRepo.GetAll().Select(
                    v => new SelectListItem
                    {
                        Text = v.Name,
                        Value = v.Id.ToString()
                    });
                return View(objVM);
            }
            if (ModelState.IsValid && !IsVillaNumberExists)
            {
                _unitOfWork.VillaNumberRepo.Add(objVM.villaNumber);
                _unitOfWork.Save();
                TempData["Success"] = "Villa Number created successfully";
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
            VillaNumberVM villaNumberVM = new VillaNumberVM();
            villaNumberVM.list = _unitOfWork.VillaRepo.GetAll().Select(
                v => new SelectListItem
                {
                    Text = v.Name,
                    Value = v.Id.ToString()
                });
            villaNumberVM.villaNumber = _unitOfWork.VillaNumberRepo.Get(v => v.Villa_Number == id);
            
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
                _unitOfWork.VillaNumberRepo.Update(villaNumberVM.villaNumber);
                _unitOfWork.Save();
                TempData["Success"] = "Villa Number updated successfully";
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
            VillaNumber? objFromDb = _unitOfWork.VillaNumberRepo.Get(v => v.Villa_Number == id);
            objFromDb.villa = _unitOfWork.VillaRepo.Get(v => v.Id == objFromDb.VillaId);
            if(objFromDb == null || objFromDb.villa == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View (objFromDb);
        }
        [HttpPost]
        public IActionResult Delete(VillaNumber obj)
        {
            VillaNumber? objFromDb = _unitOfWork.VillaNumberRepo.Get(v => v.Villa_Number == obj.Villa_Number);
            if(objFromDb == null)
            {
                TempData["Error"] = "Villa Number not found";
                return View(obj);
            }
            _unitOfWork.VillaNumberRepo.Remove(objFromDb);
            _unitOfWork.Save();
            TempData["Success"] = "Villa Number deleted successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
