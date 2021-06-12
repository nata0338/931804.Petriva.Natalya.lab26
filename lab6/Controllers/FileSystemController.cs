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
    public class FileSystemController : Controller
    {
        private readonly ForumContext _db;
        public FileSystemController(ForumContext context)
        {
            _db = context;        
        }

        public async Task<IActionResult> InitFileSystem()
        {
            _db.Folders.Add(
             new FolderModel { 
                 Name = "Root",
            });
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [Authorize]        
        public async Task<IActionResult> Index()
        {
            if(!await _db.Folders.AnyAsync())
            {
                return RedirectToAction("InitFileSystem");
            }

            return View(await _db.Folders.Include(x => x.Folders).FirstOrDefaultAsync(x => x.Name == "Root"));
        }


        [HttpGet]
        public async Task<IActionResult> OpenFolder(int id)
        {            
            if(id != _db.Folders.Where(x => x.Name == "Root").FirstOrDefault().Id) { 
            FolderModel folder = await _db.Folders
                .Include(x => x.Folders)
                .Include(x => x.Files)
                .FirstOrDefaultAsync(x => x.Id == id);

                ViewBag.Path = GetPath(folder.Id);
            return View(folder);
            }
            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Create(int? ParentId)
        {
            if(ParentId != null)
            {
                ViewBag.ParentId = ParentId;
                return View();
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Create(FolderModel model)
        {
            if (ModelState.IsValid)
            {
                FolderModel folder = new()
                {
                    Name = model.Name,
                    ParentFolderId = model.ParentFolderId
                };
                
                _db.Folders.Add(folder);

                FolderModel ParentFolder = await _db.Folders
                   .Include(x => x.Folders)
                   .FirstOrDefaultAsync(x => x.Id == model.ParentFolderId);
                ParentFolder.Folders.Add(folder);

                await _db.SaveChangesAsync();

                return RedirectToAction("OpenFolder", new { id = folder.ParentFolderId});
            }
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            FolderModel a = await _db.Folders.FirstOrDefaultAsync(x => x.Id == id);
            if (a.Name != "Root")
            {
                return View(await _db.Folders.FirstOrDefaultAsync(x => x.Id == id));
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Edit(FolderModel model)
        {
            if (ModelState.IsValid)
            {
                _db.Folders.Update(model);
                await _db.SaveChangesAsync();
                return RedirectToAction("OpenFolder", new { id = model.Id });
            }
            return View(model);
        }

        [HttpGet]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            FolderModel folder = await _db.Folders
                .Include(x => x.Folders)
                .Include(x => x.Files)
                .FirstOrDefaultAsync(x => x.Id == id);
            if(folder.Name == "Root")
            {
                return RedirectToAction("Index");
            }
            if (folder != null)
            {
                return View(folder);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            FolderModel folder = await _db.Folders
                .Include(x => x.Folders)
                .Include(x => x.Files)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (folder != null)
            {
                _db.DeleteFolderDirectory(folder);

                await _db.SaveChangesAsync();
                return RedirectToAction("OpenFolder", "FileSystem", new { id = folder.ParentFolderId });
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
