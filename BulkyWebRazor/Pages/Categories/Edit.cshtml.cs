using BulkyWebRazor.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BulkyWebRazor.Models;

namespace BulkyWebRazor.Pages.Categories
{
    [BindProperties]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        
        public Category? CategoryObj { get; set; }
        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult OnGet(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            CategoryObj = _db.Categories.FirstOrDefault(obj => obj.Id == id);
            if(CategoryObj ==null)
            {
                return BadRequest();
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            
            if (!ModelState.IsValid)
            {
                return Page();
            }
            _db.Categories.Update(CategoryObj);
            _db.SaveChanges();
            TempData["success"] = "Category updated successfully";
            return RedirectToPage("Index");
        }


    }
}
