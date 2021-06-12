using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace lab6.Models
{
    public class ForumContext : DbContext
    {
        public DbSet<UserModel> Users { get;set; }
        public DbSet<ForumCategoryModel> ForumCategories { get; set; }
        public DbSet<TopicModel> Topics { get; set; }
        public DbSet<ReplyModel> Replies { get; set; }
        public DbSet<RoleModel> Roles { get; set; }
        public DbSet<PictureModel> Pictures { get; set; }
        public DbSet<FolderModel> Folders { get; set; }
        public DbSet<FileModel> Files { get; set; }


        public ForumContext(DbContextOptions<ForumContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RoleModel>().HasData(
                new RoleModel { Id = 1, Name = "admin" },
                new RoleModel { Id = 2, Name = "user" } );

            modelBuilder.Entity<UserModel>().HasData(
                new UserModel
                {
                    Id = 1,
                    Email = "admin@mail.ru",
                    Password = "admin",
                    RoleId = 1
                },
                new UserModel
                {
                    Id = 2,
                    Email = "user@mail.ru",
                    Password = "user",
                    RoleId = 2
                });

            modelBuilder.Entity<ForumCategoryModel>().HasData(
                new ForumCategoryModel
                {
                    Id = 1,
                    Name = "People",
                    Description = "About famous people."
                },
                new ForumCategoryModel 
                {
                    Id = 2,
                    Name = "Сountries",
                    Description = "About"
                },
                new ForumCategoryModel
                {
                    Id = 3,
                    Name = "Aninals",
                    Description = "Dog, cat"
                });

            modelBuilder.Entity<PictureModel>().HasData(
                new PictureModel
                {
                    Id = 1,
                    Name = "flags",
                    Image = ReadFile("wwwroot/img/flags.jpg"),
                    ReplyModelId = 2
                },
                new PictureModel
                {
                    Id = 2,
                    Name = "p1",
                    Image = ReadFile("wwwroot/img/p1.jpg"),
                    ReplyModelId = 1
                },
                new PictureModel
                {
                    Id = 3,
                    Name = "unnamed",
                    Image = ReadFile("wwwroot/img/unnamed.jpg"),
                    TopicModelId = 1
                });

            modelBuilder.Entity<TopicModel>().HasData(
                new TopicModel
                {
                    Id = 1,
                    Title = "Michael Jackson",
                    Description = "Michael Jackson was an American singer, songwriter and legendary dancer. He was and is still known as the king of the pop scene. He became the idol of millions and was a grandee who had hundreds of different prizes among which there were 15 Grammy awards.",
                    DateCreated = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss"),
                    DateEdited = DateTime.Now,
                    ReplyCount = 1,
                    ForumId = 1,
                    AuthorId = 1,
                    LastReplyId = 2,
                    PictureCount = 2
                },
                new TopicModel
                {
                    Id = 2,
                    Title = "France",
                    Description = "France is typically associated with great culture and rich history. There are many marvelous chateaux and ancient buildings in the country. Apart from Paris, there are some remarkable places to visit: Arles (an ancient city on the Rhone river), Versailles, Provence, Lille, etc.",
                    DateCreated = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss"),
                    DateEdited = DateTime.Now,
                    ReplyCount = 2,
                    ForumId = 2,
                    AuthorId = 2,
                    LastReplyId = 1,
                    PictureCount = 1
                });

            modelBuilder.Entity<ReplyModel>().HasData(
                new ReplyModel
                {
                    Id = 1,
                    Text = "My favorite ",
                    DateCreated = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss"),
                    DateEdited = DateTime.Now,
                    TopicId = 1,
                    AuthorId = 2,
                    PictureCount = 3
                },
                new ReplyModel
                {
                    Id = 2,
                    Text = "I like it",
                    DateCreated = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss"),
                    DateEdited = DateTime.Now,
                    TopicId = 2,
                    AuthorId = 1,
                    PictureCount = 0
                });           

            modelBuilder.Entity<FolderModel>().HasData(
                new FolderModel
                {
                    Id = 1,
                    Name = "Root"
                });
        }


       
        public ReplyModel DeleteAllPicturesInReply(ReplyModel reply)
        {
            if (reply.PictureCount != 0)
            {
                foreach (PictureModel pic in reply.Pictures.ToList())
                {
                    reply.Pictures.Remove(pic);
                    this.Pictures.Remove(pic);
                }
            }
            return reply;
        }
        public TopicModel DeleteAllPicturesInTopic(TopicModel topic)
        {
            if (topic.PictureCount != 0)
            {
                foreach (PictureModel pic in topic.Pictures.ToList())
                {
                    topic.Pictures.Remove(pic);
                    this.Pictures.Remove(pic);
                }
            }
            return topic;
        }
        public TopicModel DeleteAllRepliesInTopic(TopicModel topic)
        {
            if(topic.ReplyCount != 0)
            {
                foreach (ReplyModel reply in topic.Replies.ToList())
                {
                    DeleteReply(reply, topic);
                }
            }
            return topic;
        }
        public ForumCategoryModel DeleteAllTopicsInCategory(ForumCategoryModel forumCategory)
        {
            if (forumCategory.Topics.Any())
            {
                foreach (TopicModel topic in forumCategory.Topics.ToList())
                {
                    DeleteTopic(topic);
                }
            }
            return forumCategory;
        }

        public void DeleteForumCategory(ForumCategoryModel forumCategory)
        {
            forumCategory = DeleteAllTopicsInCategory(forumCategory);
            this.ForumCategories.Remove(forumCategory);
        }
        public void DeleteReply(ReplyModel reply, TopicModel topic)
        {
            reply = DeleteAllPicturesInReply(reply);
            topic.Replies.Remove(reply);
            topic.ReplyCount--;
            this.Replies.Remove(reply);
        }
        public void DeleteTopic(TopicModel topic)
        {
            topic = DeleteAllRepliesInTopic(topic);
            topic = DeleteAllPicturesInTopic(topic);
            this.Topics.Remove(topic);
        }
        public void DeleteFile(FileModel file)
        {
           FolderModel folder = this.Folders.FirstOrDefault(x => x.Id == file.FolderId);
           folder.Files.Remove(file);
           this.Files.Remove(file);
        }
        public void DeleteFolder(FolderModel folder)
        {
            FolderModel ParentFolder = this.Folders.FirstOrDefault(x => x.Id == folder.ParentFolderId);
            ParentFolder.Folders.Remove(folder);
            this.Folders.Remove(folder);
        }

        public void DeleteFolderDirectory(FolderModel folder)
        {
            if (folder.Files.Any())
            {
                foreach(FileModel file in folder.Files.ToList())
                {
                    DeleteFile(file);
                }
            }

            if (folder.Folders.Any())
            {
                foreach (FolderModel fld in folder.Folders.ToList())
                {
                    FolderModel fld_temp = this.Folders
                        .Include(x => x.Folders)
                        .Include(x => x.Files)
                        .FirstOrDefault(x => x.Id == fld.Id);

                    if (fld.Folders.Any())
                    {
                        DeleteFolderDirectory(fld);
                    }
                    DeleteFolder(fld);
                } 
            }
            DeleteFolder(folder);
        }



        // method for seeding img data
        public byte[] ReadFile(string path)
        {
            FileInfo fInfo = new(path);
            long numBytes = fInfo.Length;

            FileStream fStream = new(path, FileMode.Open, FileAccess.Read);

            BinaryReader br = new(fStream);

            byte[] data = br.ReadBytes((int)numBytes);
            return data;
        }
    }
}