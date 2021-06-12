using lab6.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace lab6.Controllers
{
    public class FileController : Controller
    {
        private readonly ForumContext _db;
        public FileController(ForumContext context)
        {
            _db = context;
        }


        [HttpGet]
        public IActionResult FileUpload(int FolderId)
        {
            FileModel model = new()
            {
                FolderId = FolderId
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> FileUpload(FileModel model, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                if(file.Length > 10000000)
                {
                    ModelState.AddModelError(String.Empty, "The file size is too big. (max 10 mb)");
                    return View(model);
                }

                byte[] f = null;

                using (var biReader = new BinaryReader(file.OpenReadStream()))
                {
                    f = biReader.ReadBytes((int)file.Length);
                }

                FileModel NewFile = new()
                {
                    Name = model.Name,
                    Extension = Path.GetExtension(file.FileName),
                    Size = file.Length,
                    File = f,
                    Type = file.ContentType,
                    FolderId = model.FolderId
                };

                FolderModel folder = await _db.Folders.FirstOrDefaultAsync(x => x.Id == model.FolderId);
                _db.Files.Add(NewFile);
                folder.Files.Add(NewFile);

                await _db.SaveChangesAsync();

                return RedirectToAction("OpenFolder", "FileSystem", new { id = model.FolderId });
            }
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> OpenFile(int id, int FolderId)
        {
            FileModel file = await _db.Files.FirstOrDefaultAsync(x => x.Id == id);

            ViewBag.Path = GetPath(FolderId);

            return View(file);
        }

        public async Task<IActionResult> DownloadFile(int id)
        {
            FileModel file = await _db.Files.FirstOrDefaultAsync(x => x.Id == id);

            return File(file.File, file.Type, file.Name+file.Extension);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            return View(await _db.Files.FirstOrDefaultAsync(x => x.Id == id));
        }
        [HttpPost]
        public async Task<IActionResult> Edit(FileModel model)
        {
            if (ModelState.IsValid)
            {
                FileModel file = await _db.Files
                    .Where(x => x.Id == model.Id)
                    .FirstOrDefaultAsync();

                file.Name = model.Name;

                await _db.SaveChangesAsync();

                return RedirectToAction("OpenFile", new { id = file.Id, file.FolderId });
            }
            return View(model);
        }


        [HttpGet]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            FileModel file = await _db.Files.FirstOrDefaultAsync(x => x.Id == id);
            if (file != null)
            {
                return View(file);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            FileModel file = await _db.Files.FirstOrDefaultAsync(x => x.Id == id);

            if (file != null)
            {
                _db.DeleteFile(file);

                await _db.SaveChangesAsync();
                return RedirectToAction("OpenFolder", "FileSystem", new { id = file.FolderId });
            }
            return NotFound();
        }

        private List<Tuple<int, string>> GetPath(int? id)
        {
            List<Tuple<int, string>> path = new();
            FolderModel folder = _db.Folders.FirstOrDefault(x => x.Id == id);
            path.Add(new Tuple<int, string>(folder.Id, folder.Name));
            while (folder.ParentFolderId != null)
            {
                folder = _db.Folders.FirstOrDefault(x => x.Id == folder.ParentFolderId);
                path.Add(new Tuple<int, string>(folder.Id, folder.Name));
            }
            path.Reverse();
            return path;
        }
    }
}
