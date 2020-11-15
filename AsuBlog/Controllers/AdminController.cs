using AsuBlog.Models;
using Newtonsoft.Json;
using Store.DAL.Entities;
using Store.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.IO;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;

namespace AsuBlog.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly IUnitOfWork store;

        public AdminController(IUnitOfWork store)
        {
            this.store = store;
        }

        public ActionResult Manage()
        {
            return View("Manage");
        }

        #region Posts

        /// <summary>
        /// {  page: current page no,               //JSON
        ///    records: total no.of records,
        ///    rows: records,
        ///    total: no.of records returned now  }
        ///     $page = $_POST['page'];  // текущая страница
        ///     $limit = $_POST['rows']; // количество строк отображаемое в гриде
        ///     $sidx = $_POST['sidx'];  // сортировка по полю грида
        ///     $sord = $_POST['sord'];  // get the direction
        /// </summary>
        /// <param name="jqParams"></param>
        /// <returns></returns>
        public ContentResult Posts(JqInViewModel jqParams)
       {
                var posts = store.Posts.GetPostsForAdminPanel(jqParams.page - 1, jqParams.rows,
                                                              jqParams.sidx, jqParams.sord == "asc");
            
                var totalPosts = store.Posts.TotalItems();

            return Content(JsonConvert.SerializeObject(new
            {
                page = jqParams.page,
                records = totalPosts,
                rows = posts,
                total = Math.Ceiling(Convert.ToDouble(totalPosts) / jqParams.rows)
            }, new CustomDateTimeConverter()), "application/json");
        }


        /// <summary>
        /// Go to post view from control panel
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ShowPost(int id)
        {
            var post = store.Posts.Get(id);
            return RedirectToRoute(new { controller = "Blog", action = "Post", year = post.PostedOn.Year, month = post.PostedOn.Month, title = post.UrlSlug });
        }

        // Potentially dangerous Request.Form value received from client(ShortDescription = "<p> a </p>") 
        // was detected. ' if not set ValidateInput (false)
        [HttpPost, ValidateInput(false)]
        public ContentResult AddPost(Post post)
        {
            string json;

            ModelState.Clear();

            if (TryValidateModel(post))
            {
                Post postNew = new Post();
                postNew.Title = post.Title;
                postNew.UrlSlug = post.UrlSlug;
                postNew.PostedOn = post.PostedOn;
                postNew.Published = post.Published;
                postNew.ShortDescription = post.ShortDescription;
                postNew.Description = post.Description;
                postNew.Modified = post.Modified;
                postNew.Topic = post.Topic;
                postNew.Subtopic = post.Subtopic;
                postNew.Theme = post.Theme;
                postNew.Subtheme = post.Subtheme;
                postNew.Meta = post.Meta;
                postNew.NumberVisits = 0;
            
                foreach(Tag tag in post.Tags)
                {
                    Tag tagNew = store.Tags.Get(tag.Id);
                    postNew.Tags.Add(tagNew);
                }
                
                postNew.Categorys.Add(store.Categorys.Get(post.Categorys.FirstOrDefault().Id));
                IEnumerable<Category> categoryCollection = store.Categorys.GetAll();

                Post returnPost = AddCategorysToPost(postNew, categoryCollection);

                var id = store.Posts.Create(returnPost);

                Directory.CreateDirectory(Server.MapPath("~/Images/") + returnPost.UrlSlug);

                json = JsonConvert.SerializeObject(new
                {
                    id = id,
                    success = true,
                    message = "Статья успешно добавлена"
                });
            }
            else
            {
                json = JsonConvert.SerializeObject(new
                {
                    id = 0,
                    success = false,
                    message = "Произошла ошибка при добавлении статьи!"
                });
            }

            return Content(json, "application/json");
        }

        private Post AddCategorysToPost(Post post, IEnumerable<Category> categoryCollection)
        {
            var upcategory1 = post.Categorys.FirstOrDefault();
            if (upcategory1.ParentId != null)
            {
                var upcategory2 = categoryCollection.Where(p => p.Id == upcategory1.ParentId).FirstOrDefault();
                if (upcategory2.ParentId != null)
                {
                    var upcategory3 = categoryCollection.Where(p => p.Id == upcategory2.ParentId).FirstOrDefault();
                    if (upcategory3.ParentId != null)
                    {
                        var upcategory4 = categoryCollection.Where(p => p.Id == upcategory3.ParentId).FirstOrDefault();

                        if (upcategory4 != null)
                            post.Categorys.Add(upcategory4);
                    }
                    if (upcategory3 != null)
                        post.Categorys.Add(upcategory3);
                }
                if (upcategory2 != null)
                    post.Categorys.Add(upcategory2);
            }

            Category nameAsPost = new Category();
            nameAsPost.Name = post.Title;
            nameAsPost.Level = upcategory1.Level + 1;
            nameAsPost.UrlSlug = post.UrlSlug;
            nameAsPost.ParentId = upcategory1.Id;
            nameAsPost.BoolArticle = true;
            nameAsPost.Description = post.ShortDescription;

            var newCategoryId = store.Categorys.Create(nameAsPost);

            Category category = store.Categorys.Get(newCategoryId);

            post.Categorys.Add(category);

            return post;
        }

        /// <summary>
        /// Edit selected post
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        [HttpPost, ValidateInput(false)]
        public ContentResult EditPost(Post post)
        {
            string json;

            ModelState.Clear();

            if (TryValidateModel(post))
            {
                IList<Category> categoriesForRemove = store.Categorys.GetAll().Where(p => p.BoolArticle == true).ToList();
                var categForRemove = categoriesForRemove.Where(p => p.Posts.Where(s => s.Id == post.Id).FirstOrDefault() != null).FirstOrDefault();
                if (categForRemove != null)
                {
                    store.Categorys.Delete(categForRemove.Id);
                }

                Post postNew = store.Posts.Get(post.Id);

                if (post.UrlSlug != postNew.UrlSlug)
                {
                    string oldDirectory = Server.MapPath("~/Images/") + postNew.UrlSlug;
                    string newDirectory = Server.MapPath("~/Images/") + post.UrlSlug;
                    Directory.Move(oldDirectory, newDirectory);
                    postNew.ImagePath = postNew.ImagePath.Replace(postNew.UrlSlug, post.UrlSlug);
                    postNew.UrlSlug = post.UrlSlug;
                }
               
                if(post.Title != postNew.Title)
                    postNew.Title = post.Title;

                if (post.PostedOn != postNew.PostedOn)
                    postNew.PostedOn = post.PostedOn;

                if (post.Published != postNew.Published)
                    postNew.Published = post.Published;

                if (post.ShortDescription != postNew.ShortDescription)
                    postNew.ShortDescription = post.ShortDescription;

                if (post.Description != postNew.Description)
                    postNew.Description = post.Description;

                if (post.Modified != postNew.Modified)
                    postNew.Modified = post.Modified;

                if (post.NumberVisits != postNew.NumberVisits)
                    postNew.NumberVisits = post.NumberVisits;

                if (post.Topic != postNew.Topic)
                    postNew.Topic = post.Topic;

                if (post.Subtopic != postNew.Subtopic)
                    postNew.Subtopic = post.Subtopic;

                if (post.Theme != postNew.Theme)
                    postNew.Theme = post.Theme;

                if (post.Subtheme != postNew.Subtheme)
                    postNew.Subtheme = post.Subtheme;

                if (post.Meta != postNew.Meta)
                    postNew.Meta = post.Meta;

                postNew.Tags.Clear();
                postNew.Categorys.Clear();

                foreach (Tag tag in post.Tags)
                {
                    Tag tagNew = store.Tags.Get(tag.Id);
                    postNew.Tags.Add(tagNew);
                }
                                
                postNew.Categorys.Add(store.Categorys.Get(post.Categorys.FirstOrDefault().Id));
                IEnumerable<Category> categoryCollection = store.Categorys.GetAll();

                Post returnPost = AddCategorysToPost(postNew, categoryCollection);

                store.Posts.Update(returnPost);
                
                json = JsonConvert.SerializeObject(new
                {
                    id = post.Id,
                    success = true,
                    message = "Статья успешно добавлена"
                });
            }
            else
            {
                json = JsonConvert.SerializeObject(new
                {
                    id = 0,
                    success = false,
                    message = "Возникла ошибка при сохранении изменений!"
                });
            }

            return Content(json, "application/json");
        }

        /// <summary>
        /// Delete selected post
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ContentResult DeletePost(int id)
        {
            IList<Category> categoriesForRemove = store.Categorys.GetAll().Where(p => p.BoolArticle == true).ToList();
            var categForRemove = categoriesForRemove.Where(p => p.Posts.Where(s => s.Id == id).FirstOrDefault() != null).FirstOrDefault();
            if(categForRemove!=null)
            {
                store.Categorys.Delete(categForRemove.Id);
            }

            store.Posts.Delete(id);

            Directory.Delete(Server.MapPath("~/Images/")+categForRemove.UrlSlug, true);

            var json = JsonConvert.SerializeObject(new
            {
                success = true,
                message = "Пост удалён успешно"
            });

            return Content(json, "application/json");
        }

        #endregion

        #region Categories
        /// <summary>
        /// Return all the categories.
        /// </summary>
        /// <returns></returns>
        public ContentResult Categories(JqInViewModel jqParams)
        {
            var categories = store.Categorys.GetAll().OrderBy(p => p.Level)
                                    .Skip((jqParams.page - 1)* jqParams.rows)
                                    .Take(jqParams.rows)
                                    .ToList();
            var totalCategorys = store.Categorys.TotalItems();

            return Content(JsonConvert.SerializeObject(new
            {
                page = jqParams.page,
                records = totalCategorys,
                rows = categories,
                total = Math.Ceiling(Convert.ToDouble(totalCategorys) / jqParams.rows)
            }), "application/json");
        }

        /// <summary>
        /// Create a new category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpPost, ValidateInput(false)]
        public ContentResult AddCategory(Category category)
        {
            string json;
            ModelState.Clear();

            if (TryValidateModel(category))
            {
                if (category.ParentId == 0)
                {
                    category.Level = 1;
                    category.ParentId = null;
                }
                else
                {
                  category.Level = store.Categorys.Get((int)(category.ParentId)).Level + 1;
                }

                category.BoolArticle = false;

                store.Categorys.Create(category);

                json = JsonConvert.SerializeObject(new
                {
                    id = category.Id,
                    success = true,
                    message = "Изменения сохранены успешно"
                });
            }
            else
            {
                json = JsonConvert.SerializeObject(new
                {
                    id = 0,
                    success = false,
                    message = "Возникла ошибка при сохранении категории."
                });
            }

            return Content(json, "application/json");
        }


        /// <summary>
        /// Edit selected category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpPost, ValidateInput(false)]
        public ContentResult EditCategory(Category category)
        {
            string json;
            ModelState.Clear();

            if (TryValidateModel(category))
            {
                if (category.BoolArticle == false)
                {
                    Category categoryNew = store.Categorys.Get(category.Id);
                    categoryNew.FullUrl = category.FullUrl;
                    categoryNew.Name = category.Name;
                    categoryNew.BoolArticle = category.BoolArticle;
                    categoryNew.Description = category.Description;
                    categoryNew.UrlSlug = category.UrlSlug;


                    if (category.ParentId == 0)
                    {
                        categoryNew.Level = 1;
                        categoryNew.ParentId = null;
                    }
                    else
                    {
                        categoryNew.Level = store.Categorys.Get((int)(category.ParentId)).Level + 1;
                        categoryNew.ParentId = category.ParentId;
                    }

                    store.Categorys.Update(categoryNew);

                    json = JsonConvert.SerializeObject(new
                    {
                        id = category.Id,
                        success = true,
                        message = "Изменения сохранены успешно"
                    });
                }
                else
                {
                    json = JsonConvert.SerializeObject(new
                    {
                        id = 0,
                        success = false,
                        message = "Нельзя изменить категорию с полем BoolArticle = true.Эти категории появляются автоматически при добавлении поста.Их редактировать не надо. "
                    });

                }
             }
            else
            {
                json = JsonConvert.SerializeObject(new
                {
                    id = 0,
                    success = false,
                    message = "Возникла ошибка при сохранении категории."
                });
            }

            return Content(json, "application/json");
        }

        /// <summary>
        /// Delete selected category
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ContentResult DeleteCategory(int id)
        {
            
            store.Categorys.Delete(id);

            var json = JsonConvert.SerializeObject(new
            {
                success = true,
                message = "Category deleted successfully."
            });

            return Content(json, "application/json");
        }

        #endregion

        #region Tags

        /// <summary>
        /// Return all the tags as JSON.
        /// </summary>
        /// <returns></returns>
        public ContentResult Tags(JqInViewModel jqParams)
        {
            var tags = store.Tags.GetAll().OrderBy(p => p.Id)
                                   .Skip((jqParams.page - 1) * jqParams.rows)
                                   .Take(jqParams.rows)
                                   .ToList();
            var totalTags = store.Tags.TotalItems();

            return Content(JsonConvert.SerializeObject(new
            {
                page = jqParams.page,
                records = totalTags,
                rows = tags,
                total = Math.Ceiling(Convert.ToDouble(totalTags) / jqParams.rows)
            }), "application/json");
        }

        /// <summary>
        /// Создать новый тег
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        [HttpPost, ValidateInput(false)]
        public ContentResult AddTag(Tag tag)
        {
            string json;
            ModelState.Clear();

            if (TryValidateModel(tag))
            {
                store.Tags.Create(tag);

                json = JsonConvert.SerializeObject(new
                {
                    id = tag.Id,
                    success = true,
                    message = "Изменения сохранены успешно"
                });
            }
            else
            {
                json = JsonConvert.SerializeObject(new
                {
                    id = 0,
                    success = false,
                    message = "Возникла ошибка при сохранении категории."
                });
            }
            
            return Content(json, "application/json");
        }

        /// <summary>
        /// Edit the selected tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></return>
        [HttpPost, ValidateInput(false)]
        public ContentResult EditTag(Tag tag)
        {
            string json;
            ModelState.Clear();

            if (TryValidateModel(tag))
            {
                store.Tags.Update(tag);

                json = JsonConvert.SerializeObject(new
                {
                    id = tag.Id,
                    success = true,
                    message = "Изменения сохранены успешно"
                });
            }
            else
            {
                json = JsonConvert.SerializeObject(new
                {
                    id = 0,
                    success = false,
                    message = "Возникла ошибка при сохранении категории."
                });
            }

            return Content(json, "application/json");
        }

        /// <summary>
        /// Delete selected category
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ContentResult DeleteTag(int id)
        {
            store.Tags.Delete(id);

            var json = JsonConvert.SerializeObject(new
            {
                success = true,
                message = "Category deleted successfully."
            });

            return Content(json, "application/json");
        }

        #endregion

        #region Get Items for DropDown
        /// <summary>
        /// Return html required to create category dropdown in jQGrid popup.
        /// </summary>
        /// <returns></returns>
        public ContentResult GetCategoriesForPostsGrid()
        {
            var categories = store.Categorys.GetAll().Where(p=>p.BoolArticle==false).OrderBy(s => s.Level);

            var sb = new StringBuilder();
            sb.AppendLine("<select>");

            foreach (var category in categories)
            {
                //sb.AppendLine(string.Format(@"<option value=""{0}"">{1}</option>", category.Id, category.Name));
                sb.AppendLine(string.Format("<option value=\"{0}\">{1}</option>", category.Id, category.Name));
            }

            sb.AppendLine("</select>");
            return Content(sb.ToString(), "text/html");
        }

             
        /// <summary>
        /// Return html required to create tag dropdown in jQGrid popup.
        /// </summary>
        /// <returns></returns>
        public ContentResult GetTagsForPostsGrid()
        {
            var tags = store.Tags.GetAll().OrderBy(s => s.Name);

            var str = new StringBuilder();
            str.AppendLine(@"<select multiple=""multiple"">");

            foreach (var tag in tags)
            {
                str.AppendLine(string.Format(@"<option value=""{0}"">{1}</option>", tag.Id, tag.Name));
            }

            str.AppendLine("<select>");
            return Content(str.ToString(), "text/html");
        }
        
        /// <summary>
        /// Return html required to create parentId dropdown in jQGrid popup category.
        /// </summary>
        /// <returns></returns>
        public ContentResult GetCategoriesForParentCategoryGrid()
        {
            var parentCategory = store.Categorys.GetAll().Where(p => p.BoolArticle == false && p.Level != 4).OrderBy(p => p.Level);

            var str = new StringBuilder();
            str.AppendLine(@"<select>");
            str.AppendLine(string.Format(@"<option value=""0"">null</option>"));

            foreach (var category in parentCategory)
            {
                str.AppendLine(string.Format(@"<option value=""{0}"">{1}</option>", category.Id, category.Name));
            }

            str.AppendLine("<select>");
            return Content(str.ToString(), "text/html");
        }

        #endregion

        #region Image     
        [ChildActionOnly]
        public ActionResult GiveImageForPost()
        {
            SelectList posts = new SelectList(store.Posts.GetAll(), "Id", "Title");
          
            return PartialView("_ImageSendForm", posts);
        }


        [HttpPost]
        public ActionResult UploadImage(string postId, string mainImage, HttpPostedFileBase uploadImage)
        {
            if (ModelState.IsValid && uploadImage != null)
            {
                Post post = store.Posts.Get(Int32.Parse(postId));
                if (post!=null)
                {
                    string pathForImage = Server.MapPath("~/Images/"+post.UrlSlug+"/");

                    string fileName = Path.GetFileName(uploadImage.FileName);
      
                    uploadImage.SaveAs(Path.Combine(pathForImage,fileName));

                    if (mainImage == "true")
                    {
                        post.ImagePath = "/Images/" + post.UrlSlug + "/" + fileName;
                        store.Posts.Update(post);
                    }
                }


               return RedirectToAction("Manage","Admin");
            }
            return View();
        }

        [HttpPost]
        public ActionResult DeleteImage(string postId, string image)
        {
            if (ModelState.IsValid && image != null)
            {
                Post post = store.Posts.Get(Int32.Parse(postId));
                if (post != null)
                {
                    string pathForImage = Server.MapPath("~/Images/" + post.UrlSlug + "/");
                    string fullPath = Path.Combine(pathForImage, image);
                 
                    FileInfo fileInfo = new FileInfo(fullPath);
                    if (fileInfo.Exists)
                    {
                        fileInfo.Delete();
                        // альтернатива с помощью класса File
                        // File.Delete(path);
                        return View("SuccessDelete");
                    }

                }
               
            }
            return RedirectToAction("Manage", "Admin");
        }

        [HttpPost]
        public ActionResult LookImages(string postId)
        {
            List<string> filesName = null;
            Dictionary<string, string> model = new Dictionary<string, string>();
            Post post = store.Posts.Get(Int32.Parse(postId));
            if (post != null)
            {
                string str;
                string path = "/Images/" + post.UrlSlug + "/";
                string pathForImages = Server.MapPath(path);
                filesName = (from a in Directory.GetFiles(pathForImages)
                                         select Path.GetFileName(a))
                                         .ToList();
             
                for (int i = 0; i < filesName.Count; i++)
                {
                    str = path + filesName[i];
                    model.Add(filesName[i], str);
                }

            }

            return PartialView("_LookImages", model);
        }

        #endregion

        #region Users
        /// <summary>
        /// {  page: current page no,               //JSON
        ///    records: total no.of records,
        ///    rows: records,
        ///    total: no.of records returned now  }
        ///     $page = $_POST['page'];     // текущая страница
        ///     $limit = $_POST['rows'];    // количество строк отображаемое в гриде
        ///     $sidx = $_POST['sidx'];     // сортировка по полю грида
        ///     $sord = $_POST['sord'];     // get the direction
        /// </summary>
        /// <param name="jqParams"></param>
        /// <returns></returns>
        public ContentResult Users(JqInViewModel jqParams)
        {
            var users = store.UserManager.GetUsersForAdminPanel(jqParams.page - 1, jqParams.rows,
                                                          jqParams.sidx, jqParams.sord == "asc");

            var totalUsers = store.UserManager.Users.Count();

            
            return Content(JsonConvert.SerializeObject(new
            {
                page = jqParams.page,
                records = totalUsers,
                rows = users,
                total = Math.Ceiling(Convert.ToDouble(totalUsers) / jqParams.rows)
            }), "application/json");
        }


        /// <summary>
        /// Delete selected user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ContentResult> DeleteUser(string Id)
        {
            string json = null;
            ApplicationUser user = await store.UserManager.FindByIdAsync(Id);
            if (user.UserName == "Andrew" && user.Email == "kapishnikovaa@yandex.ru")
                user = null;

            if (user != null)
            {
                IdentityResult result = await store.UserManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    json = JsonConvert.SerializeObject(new
                    {
                        success = true,
                        message = "Category deleted successfully."
                    });
                  
                }
            }
            else
            {
                json = JsonConvert.SerializeObject(new
                {
                    success = false,
                    message = "Category deleted successfully."
                });
            }

            return Content(json, "application/json");

        }
        #endregion

        #region Chat
        /// <summary>
        /// Return all the messages as JSON.
        /// </summary>
        /// <returns></returns>
        public ContentResult Chats(JqInViewModel jqParams)
        {
            var chats = store.Chats.GetAll().OrderByDescending(p => p.DateMessage)
                                   .Skip((jqParams.page - 1) * jqParams.rows)
                                   .Take(jqParams.rows)
                                   .ToList();
            var totalChats = store.Chats.TotalItems();

            return Content(JsonConvert.SerializeObject(new
            {
                page = jqParams.page,
                records = totalChats,
                rows = chats,
                total = Math.Ceiling(Convert.ToDouble(totalChats) / jqParams.rows)
            }, new CustomDateTimeConverter()), "application/json");
        }


        /// <summary>
        /// Delete selected message
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ContentResult DeleteChat(int id)
        {
            store.Chats.Delete(id);

            var json = JsonConvert.SerializeObject(new
            {
                success = true,
                message = "Message deleted successfully."
            });

            return Content(json, "application/json");
        }

        #endregion

      
    }

}