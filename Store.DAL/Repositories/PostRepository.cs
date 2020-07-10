using Store.DAL.EF;
using Store.DAL.Entities;
using Store.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;


namespace Store.DAL.Repositories
{
    public class PostRepository : IRepository<Post>
    {
        private ApplicationContext db;

        public PostRepository(ApplicationContext context)
        {
            this.db = context;
        }
        public IEnumerable<Post> GetAll()
        {
           //IQueryable<Post> posts = db.Posts; //Это будет отложенный запрос
            return db.Posts; //Нужно посмотреть необходим ли здесь .ToList();
        }
        public IList<Post> GetPublishedPosts()
        {
            return db.Posts.Where<Post>(p => p.Published)
                     .OrderByDescending<Post, DateTime>(p => p.PostedOn)
                     .ToList<Post>();
        }
        public Post Get(int id)
        {
            return db.Posts.Find(id);
        }
        public Post Get(string slug)
        {
            return db.Posts.FirstOrDefault<Post>(p => p.UrlSlug.Equals(slug));
        }
        public Post GetPostByYearMonthTitleSlug(int year, int month, string titleSlug)
        {
            var post = db.Posts.Where(p => p.PostedOn.Year == year && p.PostedOn.Month == month && p.UrlSlug.Equals(titleSlug))
                           .FirstOrDefault<Post>();
            return post;
        }
        /// <summary>
        /// Return collection of posts based on pagination parameters.
        /// </summary>
        /// <param name="pageNo">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns></returns>
        public IList<Post> GetPublishedPostsByPageNoPageSize(int pageNo, int pageSize)
        {
            return db.Posts.Where<Post>(p => p.Published)
                           .OrderByDescending<Post, DateTime>(p => p.PostedOn)
                           .Skip<Post>(pageNo * pageSize)
                           .Take<Post>(pageSize)
                           .ToList<Post>();
        }

        /// <summary>
        /// Return collection of posts belongs to a particular tag.
        /// </summary>
        /// <param name="tagSlug">Tag's url slug</param>
        /// <param name="pageNo">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns></returns>
        public IList<Post> GetPublishedPostsForTagByPageNoPageSize(string tagSlug, int pageNo, int pageSize)
        {
            return db.Posts.Where<Post>(p => p.Published && p.Tags.Any<Tag>(t => t.UrlSlug.Equals(tagSlug)))
                           .OrderByDescending<Post, DateTime>(p => p.PostedOn)
                           .Skip<Post>(pageNo * pageSize)
                           .Take<Post>(pageSize)
                           .ToList<Post>();
        }
        public IList<Post> GetPublishedPostsForTag(string tagSlug)
        {
            return db.Posts.Where<Post>(p => p.Published && p.Tags.Any<Tag>(t => t.UrlSlug.Equals(tagSlug)))
                           .OrderByDescending<Post, DateTime>(p => p.PostedOn)
                           .ToList<Post>();
        }
        /// <summary>
        /// Return collection of posts belongs to a particular category.
        /// </summary>
        /// <param name="categorySlug">Category's url slug</param>
        /// <param name="pageNo">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns></returns>
        public IList<Post> GetPublishedPostsForCategoryByPageNoPageSize(string categorySlug, int pageNo, int pageSize)
        {
            return db.Posts.Where<Post>(p => p.Published && p.Categorys.Any<Category>(t => t.UrlSlug.Equals(categorySlug)))
                           .OrderByDescending<Post, DateTime>(p => p.PostedOn)
                           .Skip<Post>(pageNo * pageSize)
                           .Take<Post>(pageSize)
                           .ToList<Post>();
        }
        public IList<Post> GetPublishedPostsForCategory(string categorySlug)
        {
            return db.Posts.Where<Post>(p => p.Published && p.Categorys.Any<Category>(t => t.UrlSlug.Equals(categorySlug)))
                           .OrderByDescending<Post, DateTime>(p => p.PostedOn)
                           .ToList<Post>();
        }

        /// <summary>
        /// Return collection of posts that matches the search text.
        /// </summary>
        /// <param name="search">search text</param>
        /// <param name="pageNo">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns></returns>
        public IList<Post> GetPublishedPostsForSearchByPageNoPageSize(string search, int pageNo, int pageSize)
        {
             return db.Posts.Where<Post>(p => p.Published && (p.Title.Contains(search) 
                            || p.Categorys.Any<Category>(t => t.Name.Equals(search)) 
                            || p.Tags.Any<Tag>(t => t.Name.Equals(search))) )
                            .OrderByDescending<Post, DateTime>(p => p.PostedOn)
                            .Skip<Post>(pageNo * pageSize)
                            .Take<Post>(pageSize)
                            .ToList<Post>();
        }
        public IList<Post> GetPublishedPostsForSearch(string search)
        {
            return db.Posts.Where<Post>(p => p.Published && (p.Title.Contains(search)
                           || p.Categorys.Any<Category>(t => t.Name.Equals(search))
                           || p.Tags.Any<Tag>(t => t.Name.Equals(search))))
                           .OrderByDescending<Post, DateTime>(p => p.PostedOn)
                           .ToList<Post>();
        }

        /// <summary>
        /// Return posts based on pagination and sorting parameters.
        /// </summary>
        /// <param name="pageNo">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="sortColumn">Sort column name</param>
        /// <param name="sortByAscending">True to sort by ascending</param>
        /// <returns></returns>
        public IList<Post> GetPostsForAdminPanel(int pageNo, int pageSize, string sortColumn, bool sortByAscending)
        {
            IList<Post> posts;
           
            switch (sortColumn)
            {
                case "Title":
                    if (sortByAscending)
                    {
                        posts = db.Posts.OrderBy(p => p.Title)
                                        .Skip(pageNo * pageSize)
                                        .Take(pageSize)
                                        .ToList();

                    }
                    else
                    {
                        posts = db.Posts.OrderByDescending(p => p.Title)
                                        .Skip(pageNo * pageSize)
                                        .Take(pageSize)
                                        .ToList();
                    }
                    break;
                case "Published":
                    if (sortByAscending)
                    {
                        posts = db.Posts.OrderBy(p => p.Published)
                                        .Skip(pageNo * pageSize)
                                        .Take(pageSize)
                                        .ToList();
                    }
                    else
                    {
                        posts = db.Posts.OrderByDescending(p => p.Published)
                                        .Skip(pageNo * pageSize)
                                        .Take(pageSize)
                                        .ToList();
                    }
                    break;
                case "PostedOn":
                    if (sortByAscending)
                    {
                        posts = db.Posts.OrderBy(p => p.PostedOn)
                                        .Skip(pageNo * pageSize)
                                        .Take(pageSize)
                                        .ToList();
                    }
                    else
                    {
                        posts = db.Posts.OrderByDescending(p => p.PostedOn)
                                        .Skip(pageNo * pageSize)
                                        .Take(pageSize)
                                        .ToList();
                    }
                    break;
                case "Modified":
                    if (sortByAscending)
                    {
                        posts = db.Posts.OrderBy(p => p.Modified)
                                        .Skip(pageNo * pageSize)
                                        .Take(pageSize)
                                        .ToList();
                    }
                    else
                    {
                        posts = db.Posts.OrderByDescending(p => p.Modified)
                                        .Skip(pageNo * pageSize)
                                        .Take(pageSize)
                                        .ToList();
                    }
                    break;
                
                default:
                    posts = db.Posts.OrderByDescending(p => p.PostedOn)
                                    .Skip(pageNo * pageSize)
                                    .Take(pageSize)
                                    .ToList();
                   
                    break;
            }

            return posts;
        }

        public int TotalItems()
        {
            return db.Posts.Count<Post>();
        }
        public int TotalPublishedPosts()
        {
            return db.Posts.Where<Post>(p => p.Published == true).Count<Post>();
        }

        /// <summary>
        /// Return total no. of posts belongs to a particular category.
        /// </summary>
        /// <param name="categorySlug">Category's url slug</param>
        /// <returns></returns>
        public int TotalPublishedPostsForCategory(string categorySlug)
        {
            return  db.Posts.Where(p => p.Published && p.Categorys.Any<Category>(t => t.UrlSlug.Equals(categorySlug)) )
                            .Count<Post>();
        }

        /// <summary>
        /// Return total no. of posts belongs to a particular tag.
        /// </summary>
        /// <param name="tagSlug">Tag's url slug</param>
        /// <returns></returns>
        public int TotalPublishedPostsForTag(string tagSlug)
        {
            return db.Posts.Where(p => p.Published && p.Tags.Any<Tag>(t => t.UrlSlug.Equals(tagSlug)) )
                           .Count<Post>();
        }

        /// <summary>
        /// Return total no. of posts that matches the search text.
        /// </summary>
        /// <param name="search">search text</param>
        /// <returns></returns>
        public int TotalPublishedPostsForSearch(string search)
        {
            return db.Posts.Where(p => p.Published && (p.Title.Contains(search) 
                           || p.Categorys.Any<Category>(t => t.Name.Contains(search)) 
                           || p.Tags.Any<Tag>(t => t.Name.Equals(search))) )
                           .Count();
        }

        public int Create(Post item)
        {
            Post post = db.Posts.Add(item);
            db.SaveChanges();
            return post.Id;
        }
         
        public void Update(Post item)
        {
            //var local = db.Set<Post>()
            //             .Local
            //             .FirstOrDefault(f => f.Id == item.Id);
            //if (local != null)
            //{
            //    db.Entry(local).State = EntityState.Detached;
            //}
                
            db.Entry<Post>(item).State = EntityState.Modified;
           
            db.SaveChanges();
        }
        public void Delete(int id)
        {
            Post post = db.Posts.Find(id);
            if (post != null)
            {
                db.Posts.Remove(post);
                db.SaveChanges();
            }
        }

    }
}
