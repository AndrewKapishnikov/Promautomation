using Store.DAL.Entities;
using System;
using System.Configuration;
using System.Web.Mvc;

namespace AsuBlog
{

    public static class Extensions
    {
        public static string Href(this Post post, UrlHelper helper)
        {
            return helper.RouteUrl(new
            {
                controller = "Blog",
                action = "Post",
                year = post.PostedOn.Year,
                month = post.PostedOn.Month,
                title = post.UrlSlug
            });
        }

        /// <summary>
        /// Convert the passed datetime from UTC timezone to configured timezone in web.config.
        /// </summary>
        /// <param name="utcDT"></param>
        /// <returns></returns>
        public static string ToConfigLocalTime(this DateTime utcDT)
        {
            var istTZ = TimeZoneInfo.FindSystemTimeZoneById(ConfigurationManager.AppSettings["Timezone"]);
            return TimeZoneInfo.ConvertTimeFromUtc(utcDT, istTZ).ToString();
        }

        public static string ToShortConfigLocalTime(this DateTime utcDT)
        {
            var istTZ = TimeZoneInfo.FindSystemTimeZoneById(ConfigurationManager.AppSettings["Timezone"]);
            return TimeZoneInfo.ConvertTimeFromUtc(utcDT, istTZ).ToShortTimeString();
        }
    }
}