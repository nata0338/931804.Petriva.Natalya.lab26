using lab6.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace lab6.Controllers
{
    public class HomeController : Controller
    {
        private readonly ForumContext _db;
        public HomeController(ForumContext context)
        {
            _db = context;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Mockups()
        {
            return View();
        }

        public async Task<IActionResult> RandomSingleForum()
        {
            List<int> ids = new();
            foreach (ForumCategoryModel forum in await _db.ForumCategories.ToListAsync())
            {
                ids.Add(forum.Id);
            }

            Random Random = new();

            int ForumIndex = Random.Next(0, ids.ToArray().Length);

            return RedirectToAction("PreIndex", "SingleForum", new { FId = ids[ForumIndex]});
        }

        public async Task<IActionResult> RandomSingleTopic()
        {
            Dictionary<int,int> ids = new();
            int counter = 0;
            foreach (TopicModel topic in await _db.Topics.ToListAsync())
            {
                ids.Add(++counter, topic.Id);
            }

            Random Random = new();

            int Index = Random.Next(1, counter + 1);
            int TopicId = ids[Index];

            TopicModel t = await _db.Topics.FirstOrDefaultAsync(o => o.Id == TopicId);

            HttpContext.Session.SetInt32("fid", (int)t.ForumId);

            return RedirectToAction("OpenTopic", "SingleForum", new { TopicId = t.Id });
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
