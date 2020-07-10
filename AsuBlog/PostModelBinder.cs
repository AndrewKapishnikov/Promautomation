using Ninject;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Store.DAL.Entities;
using Store.DAL.Interfaces;

namespace AsuBlog
{
    /// <summary>
    /// Bind POST model to actions.
    /// </summary>
    public class PostModelBinder:  DefaultModelBinder
    {
        private readonly IKernel kernel;

        public PostModelBinder(IKernel kernel)
        {
            this.kernel = kernel;
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var post = (Post)base.BindModel(controllerContext, bindingContext);  //Обнаружено потенциально опасное значение Request.Form, полученное от 
                                                                                 //клиента (ShortDescription="<p>a</p>").' если не установлено ValidateInput(false)


            using (var blogRepository = kernel.Get<IUnitOfWork>())
            {

                var categoryId = bindingContext.ValueProvider.GetValue("Category for Post").AttemptedValue;

                post.Categorys = new List<Category>();
                post.Categorys.Add(blogRepository.Categorys.Get(Int32.Parse(categoryId)));

                var tags = bindingContext.ValueProvider.GetValue("Tags").AttemptedValue.Split(',');

                if (tags.Length > 0)
                {
                    post.Tags = new List<Tag>();

                    foreach (var tag in tags)
                    {
                        post.Tags.Add(blogRepository.Tags.Get(int.Parse(tag.Trim())));
                    }
                }

                if (bindingContext.ValueProvider.GetValue("oper").AttemptedValue.Equals("edit"))
                    post.Modified = DateTime.UtcNow;
                else
                    post.PostedOn = DateTime.UtcNow;
            }
                       

            return post;
        }
    }
}