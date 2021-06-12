using lab6.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace lab6.Controllers
{
    public class SingleForumController : Controller
    {

        private readonly ForumContext _db;
        public SingleForumController(ForumContext context)
        {
            _db = context;
        }

        public IActionResult PreIndex(int FId)
        {
            HttpContext.Session.SetInt32("fid", FId);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Index()
        {
            int? fid = HttpContext.Session.GetInt32("fid");

            if (fid != null)
            {
                return View(await _db.ForumCategories
                    .Include(t => ((t.Topics as TopicModel).Author))
                    .Include(t => ((t.Topics as TopicModel).Replies as ReplyModel).Author)
                    .FirstOrDefaultAsync(m => m.Id == fid));
            }
            return NotFound();
        }

        
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.fid = HttpContext.Session.GetInt32("fid");
            return View();
        }  
        [HttpPost]
        public async Task<IActionResult> Create(TopicModel model)
        {
            if (ModelState.IsValid)
            {
                _db.Topics.Add(new TopicModel
                {
                    Title = model.Title,
                    Description = model.Description,
                    ForumId = model.ForumId,
                    AuthorId = model.AuthorId,
                    DateCreated = model.DateCreated,
                    ReplyCount = model.ReplyCount
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
                TopicModel topic = await _db.Topics.FirstOrDefaultAsync(p => p.Id == id);
                if (topic != null)
                {
                    return View(topic);
                }
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(TopicModel model)
        {
            if (ModelState.IsValid)
            {
                _db.Topics.Update(model);
                await _db.SaveChangesAsync();
                return RedirectToAction("OpenTopic", new { TopicId = model.Id });
            }
            return View(model);
        }


        [HttpGet]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id != null)
            {
                TopicModel model = await _db.Topics
                    .Include(t => t.Forum)
                    .Include(o => o.Author)
                    .FirstOrDefaultAsync(p => p.Id == id);
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
                TopicModel model = await _db.Topics
                    .Include(r => r.Replies)
                    .Include(r => (r.Replies as ReplyModel).Pictures)
                    .Include(p => p.Pictures)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (model != null)
                {
                    _db.DeleteTopic(model);

                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return NotFound();
        }


        [HttpGet]
        public async Task<IActionResult> OpenTopic(int TopicId, int? ForumId)
        {
            if(ForumId != null)
            {
                HttpContext.Session.SetInt32("fid", (int)ForumId);
            }

            TopicModel topic = await _db.Topics
            .Include(r => (r.Replies as ReplyModel).Author)
            .Include(r => (r.Replies as ReplyModel).Pictures)
            .Include(a => a.Author)
            .Include(f => f.Forum)
            .Include(p => p.Pictures)
            .FirstOrDefaultAsync(i => i.Id == TopicId);
            if (topic != null)
            {
                return View(topic);
            }
            return NotFound();
        }

    }
}
