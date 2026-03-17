using BulkyWebRazor.Data;
using BulkyWebRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public List<Category> CategoryList { get; set; }
        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public void OnGet()
        {
            CategoryList = _db.Categories.ToList();
        }

        public IActionResult OnPostDelete(int? id)
        {
            Category CategoryObj = _db.Categories.FirstOrDefault(obj => obj.Id == id);
            if (CategoryObj == null)
            {
                return NotFound();
            }
            _db.Categories.Remove(CategoryObj);
            _db.SaveChanges();
            TempData["success"] = "Deleted Successfully";
            return RedirectToPage("Index");
        }
    }
}
