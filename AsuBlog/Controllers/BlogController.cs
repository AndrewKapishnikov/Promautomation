using AsuBlog.Models;
using PagedList;
using Store.DAL.Entities;
using Store.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.ServiceModel.Syndication;

namespace AsuBlog.Controllers
{

    public class BlogController : Controller
    {
        private readonly IUnitOfWork store;

        public BlogController(IUnitOfWork store)
        {
            this.store = store;
        }
        public ActionResult Index()
        {
            IList<Post> posts = store.Posts.GetAll().OrderByDescending(p => p.NumberVisits).Take(9).ToList();
                   
            return View("Index",posts);
        }
   
        public ActionResult Catalog(string topic, string subtopic, string theme, string subtheme, int? page)
        {                    
            int pageSize = 7;
            int pageNumber = (page ?? 1);
            ICollection<Post> posts;
            var routeCategory = new RouteCategoryModel();
            if(subtheme != null)
            {
                routeCategory.SubthemeCategory = store.Categorys.Get(subtheme);
            }
            if (theme != null)
            {
                routeCategory.ThemeCategory = store.Categorys.Get(theme);
            }
            if (subtopic != null)
            {
                routeCategory.SubtopicCategory = store.Categorys.Get(subtopic);
            }
            if (topic != null)
            {
                routeCategory.TopicCategory = store.Categorys.Get(topic);
            }
           
            ViewBag.RouteCategory = routeCategory;

            try
            {
                if (routeCategory.SubthemeCategory != null && routeCategory.ThemeCategory != null &&
                     routeCategory.SubtopicCategory != null && routeCategory.TopicCategory != null)
                {
                    posts = store.Posts.GetPublishedPostsForCategory(subtheme);
                    return View("Catalog", posts.ToPagedList(pageNumber, pageSize));
                }
                else if (routeCategory.TopicCategory != null && routeCategory.SubtopicCategory != null
                        && routeCategory.ThemeCategory != null && subtheme == null && routeCategory.ThemeCategory.Level == 3)
                {
                    posts = store.Posts.GetPublishedPostsForCategory(theme);
                    return View("Catalog", posts.ToPagedList(pageNumber, pageSize));
                }
                else if (routeCategory.TopicCategory != null && routeCategory.SubtopicCategory != null
                         && theme == null && subtheme == null && routeCategory.SubtopicCategory.Level == 2)
                {
                    posts = store.Posts.GetPublishedPostsForCategory(subtopic);
                    return View("Catalog", posts.ToPagedList(pageNumber, pageSize));
                }
                else if (routeCategory.TopicCategory != null && subtopic == null && theme == null 
                        && subtheme == null && routeCategory.TopicCategory.Level == 1 )
                {
                    posts = store.Posts.GetPublishedPostsForCategory(topic);
                    return View("Catalog", posts.ToPagedList(pageNumber, pageSize));
                 }
                else
                {
                    if (topic == null && subtopic == null && theme == null && subtheme == null)
                    {
                        posts = store.Posts.GetPublishedPosts();
                        return View("Catalog", posts.ToPagedList(pageNumber, pageSize));
                    }
                    else
                    {
                        return View("UnexistingCatalog");
                    }
                }

            }
            catch(Exception ex)
            {
                return View("ExceptionCatalog");
            }
           
        }

        [ChildActionOnly]
        public PartialViewResult SubCatalog(Category topicCategory, Category subtopicCategory, Category themeCategory)
        {
            WidgetForSubCatalog subCatalog = new WidgetForSubCatalog();
           

            if (topicCategory != null && subtopicCategory != null && themeCategory != null )
            {
               subCatalog.Categories = store.Categorys.GetAll().Where<Category>(p => p.ParentId == themeCategory.Id && p.BoolArticle == false ).ToList();
               subCatalog.TopicUrlSlug = topicCategory.UrlSlug;
               subCatalog.SubTopicUrlSlug = subtopicCategory.UrlSlug;
               subCatalog.ThemeUrlSlug = themeCategory.UrlSlug;
            }
            else if (topicCategory != null && subtopicCategory != null)
            {
               subCatalog.Categories = store.Categorys.GetAll().Where<Category>(p => p.ParentId == subtopicCategory.Id && p.BoolArticle == false).ToList();
               subCatalog.TopicUrlSlug = topicCategory.UrlSlug;
               subCatalog.SubTopicUrlSlug = subtopicCategory.UrlSlug;
            }
            else if (topicCategory != null)
            {
               subCatalog.Categories = store.Categorys.GetAll().Where<Category>(p => p.ParentId == topicCategory.Id && p.BoolArticle == false).ToList();
               subCatalog.TopicUrlSlug = topicCategory.UrlSlug;
            }
            else
            {
               subCatalog.Categories = store.Categorys.GetAll().Where(p => p.ParentId == null).ToList();
            }

            return PartialView("_SubCatalog", subCatalog);
        }

        [ChildActionOnly]
        public PartialViewResult ShowTreeView()
        {
            List<Category> categorys = store.Categorys.GetAll().ToList();
            return PartialView("_TreeView",categorys);
        }

        [ChildActionOnly]
        public PartialViewResult ShowMenu()
        {
            List<Category> categorys = store.Categorys.GetAll().Where( p => (p.Level == 1 || p.Level == 2) && p.BoolArticle == false).ToList();
            return PartialView("_MenuView", categorys);
        }

        [ChildActionOnly]
        public PartialViewResult ShowSidePanel()
        {
            WidgetForSidePanel widget = new WidgetForSidePanel();
            widget.LastPosts = store.Posts.GetPublishedPostsByPageNoPageSize(0, 5);
            widget.Tags = store.Tags.GetAll().ToList<Tag>();
              
            return PartialView("_SidePanelView", widget);
        }


        public ActionResult Post(int? year, int? month, string title)
        {
            if(year == null || month == null || title == null)
            {
                return View("PostNotFound");
            }
            try
            {
                Post post = store.Posts.GetPostByYearMonthTitleSlug((int)year, (int)month, title);
             
                if (post == null)
                    return View("PostNotFound");

                if (post.Published == false && User.Identity.Name != "Andrew")
                    return View("PostNotPublished");

                post.NumberVisits = post.NumberVisits + 1;
                store.Posts.Update(post);

                WidgetForPost postWidget = new WidgetForPost();
                postWidget.TopicCategory = post.Categorys.Where(p => p.Level == 1).FirstOrDefault();
                postWidget.SubtopicCategory = post.Categorys.Where(p => p.Level == 2).FirstOrDefault();
                postWidget.ThemeCategory = post.Categorys.Where(p => p.Level == 3).FirstOrDefault();
                postWidget.SubthemeCategory = post.Categorys.Where(p => p.Level == 4).FirstOrDefault();
                postWidget.Post = post;

              

                return View("Post",postWidget);
            }
            catch(Exception ex)
            {
                return View("PostNotFound");
            }
        }
        /// <summary>
        /// Listing all articles containing the passed tag
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ViewResult Tag(string tag, int? page)
        {
            int pageSize = 7;
            int pageNumber = (page ?? 1);

            if (page < 1) return View("Index");
            
            Tag foundtag = store.Tags.Get(tag);

            if (foundtag == null)
                return View("UnexistingTag");

            ViewBag.Title = String.Format(@"Статьи, связанные с тегом ""{0}""", foundtag.Name);
            ViewBag.Tag = tag;

            IPagedList<Post> listpost = foundtag.Posts.ToPagedList(pageNumber, pageSize);

            if (listpost.PageNumber > listpost.PageCount && listpost.PageNumber > 1)
                return View("Index");

            return View("TagPosts", listpost);

        }

        /// <summary>
        /// Displaying a list of articles matching the search criteria
        /// </summary>
        /// <param name="s"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ViewResult Search(string s, int? page)
        {
            int pageSize = 7;
            int pageNumber = (page ?? 1);

            if (page < 1) return View("Index");

            ICollection<Post> posts = store.Posts.GetPublishedPostsForSearch(s);
            ViewBag.Title = String.Format(@"Статьи по запросу ""{0}""", s);
            ViewBag.s = s;

            IPagedList<Post> listpost = posts.ToPagedList(pageNumber, pageSize);

            if(listpost.Count < 1 )
                ViewBag.Title = String.Format(@"По запросу ""{0}"" ничего не найдено...", s);

            if (listpost.PageNumber > listpost.PageCount && listpost.PageNumber > 1)
                return View("Index");

            return View("SearchPost",listpost);
        }

        /// <summary>
        /// Rich Site Summary 
        /// </summary>
        /// <returns></returns>
        public ActionResult GetFeed()
        {
            var blogTitle = ConfigurationManager.AppSettings["BlogTitle"];
            var blogDescription = ConfigurationManager.AppSettings["BlogDescription"];
            var blogUrl = ConfigurationManager.AppSettings["BlogUrl"];

            var posts = store.Posts.GetPublishedPostsByPageNoPageSize(0,25).Select
            (
                p => new SyndicationItem
                    (
                        p.Title,
                        p.Description,
                        new Uri(string.Concat(blogUrl, p.Href(Url)))
                    )
            );

            var feed = new SyndicationFeed(blogTitle, blogDescription, new Uri(blogUrl), posts)
            {
                Copyright = new TextSyndicationContent(String.Format("Copyright © {0}", blogTitle)),
                Language = "ru-RU"
            };

            return new RssResult(new Rss20FeedFormatter(feed));
        }

    }
}