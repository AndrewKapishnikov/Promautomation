﻿using System;
using System.ServiceModel.Syndication;
using System.Text;
using System.Web.Mvc;
using System.Xml;

namespace AsuBlog
{
   
    public class RssResult : ActionResult
    {
        private readonly SyndicationFeedFormatter feed;

        public Encoding ContentEncoding { get; set; }

        public string ContentType { get; set; }

        public SyndicationFeedFormatter Feed
        {
            get { return this.feed; }
        }

        public RssResult(SyndicationFeedFormatter feed)
        {
            this.feed = feed;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var response = context.HttpContext.Response;
            response.ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : "application/rss+xml";

            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;

            if (this.feed != null)
                using (var xmlWriter = new XmlTextWriter(response.Output))
                {
                    xmlWriter.Formatting = Formatting.Indented;
                    this.feed.WriteTo(xmlWriter);
                }
        }
    }
}