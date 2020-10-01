
using System.Web.Mvc;

namespace AsuBlog.Controllers
{
    /// <summary>
    /// Contain actions to return views for different errors.
    /// </summary>
    public class ErrorsController : Controller
    {
        /// <summary>
        /// Return view for generic errors
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View("Index");
        }

        /// <summary>
        /// Return view for 404 errors.
        /// </summary>
        /// <returns></returns>
        public ActionResult NotFound()
        {
            return View("NotFound");
        }
    }
}