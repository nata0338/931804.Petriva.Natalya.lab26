using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using lab6.Models;
using Microsoft.EntityFrameworkCore;

namespace lab6.Controllers
{
    public class ForumCategoryController : Controller
    {

        private readonly ForumContext _db;
        public ForumCategoryController(ForumContext context)
        {
            _db = context;
        }


        public async Task<IActionResult> Index()
        {
            return View(await _db.ForumCategories.ToListAsync());
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ForumCategoryModel model)
        {
            if (ModelState.IsValid)
            {
                _db.ForumCategories.Add(new ForumCategoryModel
                {
                    Name = model.Name,
                    Description = model.Description
                });
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }



        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id != null)
            {
                ForumCategoryModel model = await _db.ForumCategories.FirstOrDefaultAsync(p => p.Id == id);
                if (model != null)
                {
                    return View(model);
                }
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(ForumCategoryModel model)
        {
            if (ModelState.IsValid)
            {
                _db.ForumCategories.Update(model);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }




        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id != null)
            {
                ForumCategoryModel model = await _db.ForumCategories.FirstOrDefaultAsync(p => p.Id == id);
                return View(model);
            }
            return NotFound();
        }



        [HttpGet]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id != null)
            {
                ForumCategoryModel model = await _db.ForumCategories.FirstOrDefaultAsync(p => p.Id == id);
                if (model != null)
                    return View(model);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                ForumCategoryModel model = await _db.ForumCategories
                    .Include(t => (t.Topics as TopicModel).Replies)
                    .ThenInclude(t => t.Pictures)
                    .Include(t => (t.Topics as TopicModel).Pictures)
                    .Include(t => t.Topics)
                    .FirstOrDefaultAsync(p => p.Id == id);
                if (model != null)
                {
                    _db.DeleteForumCategory(model);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return NotFound();
        }
    }
}
