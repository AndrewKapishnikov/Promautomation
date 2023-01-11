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
using Store.DAL.Repositories;

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
            return View("Index", posts);
        }
   
        public ActionResult Catalog(string topic, string subtopic, string theme, string subtheme, int? page)
        {
            try
            {
                int pageSize = 7;
                int pageNumber = (page ?? 1);
                PostRepository postRepository = store.Posts as PostRepository;
                var routeCategory = new RouteCategoryModel();
           
                bool topicIsNull = topic is null;
                bool subtopicIsNull = subtopic is null;
                bool themeIsNull = theme is null;
                bool subthemeIsNull = subtheme is null;
                ViewBag.RouteCategory = routeCategory;
                if (topicIsNull && subtopicIsNull && themeIsNull && subthemeIsNull)
                {
                    var posts = postRepository.GetPublishedPosts();
                    return View("Catalog", posts.ToPagedList(pageNumber, pageSize));
                }
               
                if (!subthemeIsNull) routeCategory.SubthemeCategory = store.Categorys.Get(subtheme);
                bool subthemeCategoryIsNull = routeCategory.SubthemeCategory is null;

                if (!themeIsNull) routeCategory.ThemeCategory = store.Categorys.Get(theme);
                bool themeCategoryIsNull = routeCategory.ThemeCategory is null;

                if (!subtopicIsNull) routeCategory.SubtopicCategory = store.Categorys.Get(subtopic);
                bool subtopicCategoryIsNull = routeCategory.SubtopicCategory is null;

                if (!topicIsNull) routeCategory.TopicCategory = store.Categorys.Get(topic);
                bool topicCategoryIsNull = routeCategory.TopicCategory is null;

                if (!topicCategoryIsNull && !subtopicCategoryIsNull && !themeCategoryIsNull && !subthemeCategoryIsNull)
                    return GetActionResultForCatalogView(postRepository, subtheme, pageNumber, pageSize);
                else if (!topicCategoryIsNull && !subtopicCategoryIsNull && !themeCategoryIsNull && subthemeIsNull && routeCategory.ThemeCategory.Level == 3)
                    return GetActionResultForCatalogView(postRepository, theme, pageNumber, pageSize);
                else if (!topicCategoryIsNull && !subtopicCategoryIsNull && themeIsNull && subthemeIsNull && routeCategory.SubtopicCategory.Level == 2)
                    return GetActionResultForCatalogView(postRepository, subtopic, pageNumber, pageSize);
                else if (!topicCategoryIsNull && subtopicIsNull && themeIsNull && subthemeIsNull && routeCategory.TopicCategory.Level == 1)
                    return GetActionResultForCatalogView(postRepository, topic, pageNumber, pageSize);
                else
                    return View("UnexistingCatalog");
            }
            catch
            {
                return View("ExceptionCatalog");
            }
           
        }

        private ActionResult GetActionResultForCatalogView(PostRepository postRepository, string catalog, int pageNumber, int pageSize)
        {
            ICollection<Post> posts = postRepository.GetPublishedPostsForCategory(catalog);
            return View("Catalog", posts.ToPagedList(pageNumber, pageSize));
        }

        [ChildActionOnly]
        public PartialViewResult SubCatalog(RouteCategoryModel routeCategory)
        {
            WidgetForSubCatalog subCatalog = new WidgetForSubCatalog();
            bool topicCategoryIsNull = routeCategory.TopicCategory is null;
            bool subtopicCategoryIsNull = routeCategory.SubtopicCategory is null;
            bool themeCategoryIsNull = routeCategory.ThemeCategory is null;

            subCatalog.TopicUrlSlug = routeCategory.TopicCategory?.UrlSlug;
            subCatalog.SubTopicUrlSlug = routeCategory.SubtopicCategory?.UrlSlug;
            subCatalog.ThemeUrlSlug = routeCategory.ThemeCategory?.UrlSlug;

            if (!topicCategoryIsNull && !subtopicCategoryIsNull && !themeCategoryIsNull)
                subCatalog.Categories = store.Categorys.GetAll().Where(p => p.ParentId == routeCategory.ThemeCategory.Id && p.BoolArticle == false ).ToList();
            else if (!topicCategoryIsNull && !subtopicCategoryIsNull)
                subCatalog.Categories = store.Categorys.GetAll().Where(p => p.ParentId == routeCategory.SubtopicCategory.Id && p.BoolArticle == false).ToList();
            else if (!topicCategoryIsNull)
                subCatalog.Categories = store.Categorys.GetAll().Where(p => p.ParentId == routeCategory.TopicCategory.Id && p.BoolArticle == false).ToList();
            else
                subCatalog.Categories = store.Categorys.GetAll().Where(p => p.ParentId is null).ToList();
  
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
            PostRepository postRepository = store.Posts as PostRepository;
            WidgetForSidePanel widget = new WidgetForSidePanel();
            widget.LastPosts = postRepository.GetPublishedPostsByPageNoPageSize(0, 5);
            widget.Tags = store.Tags.GetAll().ToList<Tag>();
              
            return PartialView("_SidePanelView", widget);
        }


        public ActionResult Post(int? year, int? month, string title)
        {
            if(year is null || month is null || title is null)
            {
                return View("PostNotFound");
            }
            try
            {
                PostRepository postRepository = store.Posts as PostRepository;
                Post post = postRepository.GetPostByYearMonthTitleSlug((int)year, (int)month, title);
             
                if (post is null)
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

                return View("Post", postWidget);
            }
            catch
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

            if (foundtag is null)
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

            PostRepository postRepository = store.Posts as PostRepository;
            ICollection<Post> posts = postRepository.GetPublishedPostsForSearch(s);
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

            PostRepository postRepository = store.Posts as PostRepository;
            var posts = postRepository.GetPublishedPostsByPageNoPageSize(0,25).Select
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