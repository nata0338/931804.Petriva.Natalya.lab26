using lab6.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lab6.Controllers
{
    public class AccountController : Controller
    {

        private readonly ForumContext _db;
        public AccountController(ForumContext context)
        {
            _db = context;
        }


        [Authorize]
        public async Task<IActionResult> Profile()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                UserModel user = await _db.Users
                    .Include(p => p.Role)
                    .FirstOrDefaultAsync(u => u.Email == HttpContext.User.Identity.Name);
                return View(user);
            }

            return RedirectToAction("LogIn", "Auth");
        }

        [Authorize]
        public async Task<IActionResult> Blog()
        {
            List<TopicModel> MyTopics = new();

            int userId = Convert.ToInt32(HttpContext.User.Claims.First().Value);

            foreach (TopicModel topic in await _db.Topics
                .Include(x => x.Author) 
                .Include(x => x.Forum)
                .Include(x => x.Replies)
                .ThenInclude(x => x.Author)
                .ToListAsync())
            {
                if(topic.AuthorId == userId)
                {
                    MyTopics.Add(topic);
                }
            }

            return View(MyTopics);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Forums = await _db.ForumCategories.ToListAsync();
            return View();
        }
        [Authorize]
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
                TopicModel Topic = await _db.Topics.FirstOrDefaultAsync(x => x.Title == model.Title && x.DateCreated == model.DateCreated);
                return RedirectToAction("OpenTopic", "SingleForum", 
                    new { TopicId = Topic.Id, model.ForumId });
            }
            ViewBag.Forums = await _db.ForumCategories.ToListAsync();
            return View(model);
        }

    }
}


