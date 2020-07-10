using Store.DAL.Entities;
using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace AsuBlog
{
    public static class ActionLinkExtensions
    {
        public static MvcHtmlString PostLink(this HtmlHelper helper, Post post)
        {
            return helper.ActionLink(post.Title, "Post", "Blog", new { year = post.PostedOn.Year, month = post.PostedOn.Month, title = post.UrlSlug }, new { title = post.Title });
        }

        public static MvcHtmlString TagLink(this HtmlHelper helper, Tag tag)
        {
            return helper.ActionLink(tag.Name, "Tag", "Blog", new { tag = tag.UrlSlug }, new { title = String.Format("Все статьи, связанные с тегом {0}", tag.Name) });
        }
    }
}