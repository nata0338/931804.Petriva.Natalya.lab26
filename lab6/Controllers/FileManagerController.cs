using lab6.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;


namespace lab6.Controllers
{
    public class FileManagerController : Controller
    {
        private readonly ForumContext _db;
        public FileManagerController(ForumContext context)
        {
            _db = context;
        }

        [HttpPost]
        [Route("/FileManager/AttachToTopic/{TopicId}")]
        public async Task<IActionResult> AttachToTopic(int TopicId, IFormFile file)
        {
            // загружаем картинку
            if (file != null)
            {
                TopicModel model = await _db.Topics
                    .Include(p => p.Pictures)
                    .FirstOrDefaultAsync(i => i.Id == TopicId);


                if (model.PictureCount < 3)
                {
                    byte[] img = null;
                    string picName = string.Concat(Guid.NewGuid().ToString());
                    using (var biReader = new BinaryReader(file.OpenReadStream()))
                    {
                        img = biReader.ReadBytes((int)file.Length);
                    }

                    model.Pictures.Add(new PictureModel
                    {
                        Name = picName,
                        Image = img
                    });
                    model.PictureCount++;
                    model.DateEdited = DateTime.Now;

                    await _db.SaveChangesAsync();

                    return RedirectToAction("OpenTopic", "SingleForum", new { TopicId });
                }
            }
            return RedirectToAction("OpenTopic", "SingleForum", new { TopicId });
        }

        [HttpPost]
        [Route("/FileManager/AttachToReply/{ReplyId}")]
        public async Task<IActionResult> AttachToReply(int ReplyId, IFormFile file)
        {
            int? TopicId = null;

            // загружаем картинку
            if (file != null)
            {
                ReplyModel model = await _db.Replies
                    .Include(p => p.Pictures)
                    .FirstOrDefaultAsync(i => i.Id == ReplyId);

                TopicId = model.TopicId;

                if (model.PictureCount < 3)
                {
                    byte[] img = null;
                    string picName = string.Concat(Guid.NewGuid().ToString());
                    using (var biReader = new BinaryReader(file.OpenReadStream()))
                    {
                        img = biReader.ReadBytes((int)file.Length);
                    }

                    model.Pictures.Add(new PictureModel
                    {
                        Name = picName,
                        Image = img
                    });
                    model.PictureCount++;
                    model.DateEdited = DateTime.Now;

                    await _db.SaveChangesAsync();

                    return RedirectToAction("OpenTopic", "SingleForum", new { model.TopicId });
                }
            }
            return RedirectToAction("OpenTopic", "SingleForum", new { TopicId });
        }

        public async Task<IActionResult> DetachFromTopic(int TopicId, string PictureName)
        {

            TopicModel topic = await _db.Topics.Include(p => p.Pictures).FirstOrDefaultAsync(i => i.Id == TopicId);
            PictureModel pic = await _db.Pictures.FirstOrDefaultAsync(p => p.Name == PictureName);
            topic.Pictures.Remove(pic);
            topic.PictureCount--;
            topic.DateEdited = DateTime.Now;

            _db.Pictures.Remove(pic);
            await _db.SaveChangesAsync();

            return RedirectToAction("OpenTopic", "SingleForum", routeValues: new { TopicId });
        }

        public async Task<IActionResult> DetachFromReply(int ReplyId, string PictureName)
        {

            ReplyModel reply = await _db.Replies.Include(p => p.Pictures).FirstOrDefaultAsync(i => i.Id == ReplyId);
            PictureModel pic = await _db.Pictures.FirstOrDefaultAsync(p => p.Name == PictureName);
            reply.Pictures.Remove(pic);
            reply.PictureCount--;
            reply.DateEdited = DateTime.Now;

            _db.Pictures.Remove(pic);
            await _db.SaveChangesAsync();

            return RedirectToAction("OpenTopic", "SingleForum", new { reply.TopicId });
        }
    }
}
