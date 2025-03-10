using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Services.interfaces;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Application.Services.Implementations
{
    public class VillaService : IVillaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        public VillaService(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }
        public void CreateVilla(Villa obj)
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
        }         
        public bool DeleteVilla(int id)
        {
            Villa? objFromDb = _unitOfWork.VillaRepo.Get(v => v.Id == id);
            if (objFromDb == null)
            {
                return false;
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
            return true;
        }
        public IEnumerable<Villa> GetAllVillas()
        {
            return _unitOfWork.VillaRepo.GetAll();
        }
        public Villa GetVillaById(int id)
        {
            return _unitOfWork.VillaRepo.Get(v => v.Id == id);
        }
        public void UpdateVilla(Villa obj)
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
        }
    }
}
