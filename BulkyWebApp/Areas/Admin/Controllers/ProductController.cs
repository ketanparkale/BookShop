using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModel;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.FileProviders;
using System.Collections;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BulkyWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnviroment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnviroment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnviroment = webHostEnviroment;

        }
        public IActionResult Index()
        {
            List<Product> products = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
            return View(products);
        }

        public IActionResult Upsert(int? id)
        {
             ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll()
                .Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),

                product = new Product()
            };

            if(id==null || id == 0)
            {
                return View(productVM);
            }
            else
            {
                productVM.product = _unitOfWork.Product.Get(u => u.Id == id);
                return View(productVM);
            }
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM obj, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnviroment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"Images\Product\");

                    if(!string.IsNullOrEmpty(obj.product.ImageUrl))
                    {
                        var oldImgPath = Path.Combine(wwwRootPath, obj.product.ImageUrl.TrimStart('\\'));
                        if(System.IO.File.Exists(oldImgPath))
                        {
                            System.IO.File.Delete(oldImgPath);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(productPath + fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    obj.product.ImageUrl = @"\Images\Product\" + fileName;
                }
                if(obj.product.Id == 0)
                {
                    _unitOfWork.Product.Add(obj.product);
                }
                else
                {
                    _unitOfWork.Product.Update(obj.product);
                }

                _unitOfWork.Save();
                TempData["success"] = "Product Created Successfully";
                return RedirectToAction("Index", "Product");
            }
            else
            {
                obj.CategoryList = _unitOfWork.Category.GetAll()
                .Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(obj);
            }
        }

        
        //public IActionResult Delete(int? id)
        //{
        //    Product? productToDel = _unitOfWork.Product.Get(u => u.Id == id);
        //    if(productToDel == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(productToDel);
        //}
        //[HttpPost , ActionName("Delete")]
        //public IActionResult DeletePost(int? id)
        //{

        //    Product? productToDel = _unitOfWork.Product.Get(u => u.Id == id);
        //    if(productToDel == null)
        //    {
        //        return NotFound();
        //    }
        //    _unitOfWork.Product.Remove(productToDel);
        //    _unitOfWork.Save();
        //    TempData["success"] = "Product deleted successfully";
        //    return RedirectToAction("Index", "Product");
        //}

        #region API CALL

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> products = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = products });
        }
        [HttpDelete]
        public IActionResult Delete(int?id)
        {
            var productToBeDel = _unitOfWork.Product.Get(u => u.Id == id);
            if(productToBeDel == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            string oldImgPath = Path.Combine(_webHostEnviroment.ContentRootPath,
                productToBeDel.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImgPath))
            {
                System.IO.File.Delete(oldImgPath);
            }
            _unitOfWork.Product.Remove(productToBeDel);
            _unitOfWork.Save();

            return Json(new { success = "true" , message = "Deleted Successfully"});
        }

        #endregion 
    }
}
