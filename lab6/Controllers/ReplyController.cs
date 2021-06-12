using lab6.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace lab6.Controllers
{
    public class ReplyController : Controller
    {
        private readonly ForumContext _db;
        public ReplyController(ForumContext context)
        {
            _db = context;
        }


        [Authorize]
        [HttpGet]
        public IActionResult Create(int TopicId, int UserId)
        {
            ViewBag.tid = TopicId;
            ViewBag.uid = UserId;
            return View();
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(ReplyModel model)
        {
            if (ModelState.IsValid)
            {
                ReplyModel reply = new()
                {
                    Text = model.Text,
                    DateCreated = model.DateCreated,
                    DateEdited = model.DateEdited,
                    TopicId = model.TopicId,
                    AuthorId = model.AuthorId,
                    PictureCount = model.PictureCount
                };
                TopicModel topic = await _db.Topics.FirstOrDefaultAsync(i => i.Id == model.TopicId);

                topic.Replies.Add(reply);
                topic.ReplyCount++;
                await _db.SaveChangesAsync();

                topic.LastReplyId = reply.Id;

                await _db.SaveChangesAsync();
                return RedirectToAction("OpenTopic", "SingleForum", new { model.TopicId });
            }
            return View(model);
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int? ReplyId)
        {
            if (ReplyId != null)
            {
                ReplyModel reply = await _db.Replies.FirstOrDefaultAsync(p => p.Id == ReplyId);
                if (reply != null)
                {
                    return View(reply);
                }
            }
            return NotFound();
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(ReplyModel model)
        {
            if (ModelState.IsValid)
            {
                _db.Replies.Update(model);
                await _db.SaveChangesAsync();
                return RedirectToAction("OpenTopic", "SingleForum", new { model.TopicId });
            }
            return View(model);
        }


        [Authorize]
        [HttpGet]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int? ReplyId)
        {
            if (ReplyId != null)
            {
                ReplyModel model = await _db.Replies
                    .Include(t => t.Topic)
                    .Include(o => o.Author)
                    .FirstOrDefaultAsync(p => p.Id == ReplyId);
                if (model != null)
                    return View(model);
            }
            return NotFound();
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(int? ReplyId)
        {
            if (ReplyId != null)
            {         
                ReplyModel reply = await _db.Replies.Include(p => p.Pictures).FirstOrDefaultAsync(i => i.Id == ReplyId);
                TopicModel topic = await _db.Topics.FirstOrDefaultAsync(i => i.Id == reply.TopicId);


                _db.DeleteReply(reply, topic);

                await _db.SaveChangesAsync();

                return RedirectToAction("OpenTopic", "SingleForum", new { TopicId = topic.Id });
            }
            return NotFound();
        }
    }
}
