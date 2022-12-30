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
            int pageSize = 7;
            int pageNumber = (page ?? 1);
            ICollection<Post> posts;
            PostRepository postRepository = store.Posts as PostRepository;
            var routeCategory = new RouteCategoryModel();
            try
            {
                bool topicIsNull = topic is null;
                bool subtopicIsNull = subtopic is null;
                bool themeIsNull = theme is null;
                bool subthemeIsNull = subtheme is null;
                ViewBag.RouteCategory = routeCategory;
                if (topicIsNull && subtopicIsNull && themeIsNull && subthemeIsNull)
                {
                    posts = postRepository.GetPublishedPosts();
                    return View("Catalog", posts.ToPagedList(pageNumber, pageSize));
                }

                SetRouteCategoryModel(routeCategory, (topicIsNull, topic), (subtopicIsNull, subtopic), (themeIsNull, theme), (subthemeIsNull, subtheme),
                                                      out bool topicCategoryIsNull, 
                                                      out bool subtopicCategoryIsNull,
                                                      out bool themeCategoryIsNull,
                                                      out bool subthemeCategoryIsNull);

                if (!subthemeCategoryIsNull && !themeCategoryIsNull && !subtopicCategoryIsNull && !topicCategoryIsNull)
                {
                    posts = postRepository.GetPublishedPostsForCategory(subtheme);
                    return View("Catalog", posts.ToPagedList(pageNumber, pageSize));
                }
                else if (!topicCategoryIsNull && !subtopicCategoryIsNull && !themeCategoryIsNull && subthemeIsNull && routeCategory.ThemeCategory.Level == 3)
                {
                    posts = postRepository.GetPublishedPostsForCategory(theme);
                    return View("Catalog", posts.ToPagedList(pageNumber, pageSize));
                }
                else if (!topicCategoryIsNull && !subtopicCategoryIsNull && themeIsNull && subthemeIsNull && routeCategory.SubtopicCategory.Level == 2)
                {
                    posts = postRepository.GetPublishedPostsForCategory(subtopic);
                    return View("Catalog", posts.ToPagedList(pageNumber, pageSize));
                }
                else if (!topicCategoryIsNull && subtopicIsNull && themeIsNull && subthemeIsNull && routeCategory.TopicCategory.Level == 1 )
                {
                    posts = postRepository.GetPublishedPostsForCategory(topic);
                    return View("Catalog", posts.ToPagedList(pageNumber, pageSize));
                }
                else
                {
                     return View("UnexistingCatalog");
                }
            }
            catch
            {
                return View("ExceptionCatalog");
            }
           
        }


        private void SetRouteCategoryModel(RouteCategoryModel routeCategory, (bool topicIsNull, string topicName) topic, 
                                                                             (bool subtopicIsNull, string subtopicName) subtopic, 
                                                                             (bool themeIsNulll, string themeName) theme, 
                                                                             (bool subthemeIsNull, string subthemeName) subtheme,
                                                                              out bool topicCategoryIsNull, out bool subtopicCategoryIsNull, 
                                                                              out bool themeCategoryIsNull, out bool subthemeCategoryIsNull)
        {
            if (!subtheme.subthemeIsNull) routeCategory.SubthemeCategory = store.Categorys.Get(subtheme.subthemeName);
            subthemeCategoryIsNull = routeCategory.SubthemeCategory is null;
            
            if (!theme.themeIsNulll) routeCategory.ThemeCategory = store.Categorys.Get(theme.themeName);
            themeCategoryIsNull = routeCategory.ThemeCategory is null;
    
            if (!subtopic.subtopicIsNull) routeCategory.SubtopicCategory = store.Categorys.Get(subtopic.subtopicName);
            subtopicCategoryIsNull = routeCategory.SubtopicCategory is null;
       
            if (!topic.topicIsNull) routeCategory.TopicCategory = store.Categorys.Get(topic.topicName);
            topicCategoryIsNull = routeCategory.TopicCategory is null;
          
        }

        [ChildActionOnly]
        public PartialViewResult SubCatalog(Category topicCategory, Category subtopicCategory, Category themeCategory)
        {
            WidgetForSubCatalog subCatalog = new WidgetForSubCatalog();
            bool topicCategoryIsNull = topicCategory is null;
            bool subtopicCategoryIsNull = subtopicCategory is null;
            bool themeCategoryIsNull = themeCategory is null;

            if (!topicCategoryIsNull && !subtopicCategoryIsNull && !themeCategoryIsNull)
            {
               subCatalog.Categories = store.Categorys.GetAll().Where<Category>(p => p.ParentId == themeCategory.Id && p.BoolArticle == false ).ToList();
               subCatalog.TopicUrlSlug = topicCategory.UrlSlug;
               subCatalog.SubTopicUrlSlug = subtopicCategory.UrlSlug;
               subCatalog.ThemeUrlSlug = themeCategory.UrlSlug;
            }
            else if (!topicCategoryIsNull && !subtopicCategoryIsNull)
            {
               subCatalog.Categories = store.Categorys.GetAll().Where<Category>(p => p.ParentId == subtopicCategory.Id && p.BoolArticle == false).ToList();
               subCatalog.TopicUrlSlug = topicCategory.UrlSlug;
               subCatalog.SubTopicUrlSlug = subtopicCategory.UrlSlug;
            }
            else if (!topicCategoryIsNull)
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