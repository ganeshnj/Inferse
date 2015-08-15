using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace inferse.Controllers
{
    public class InfiniteScrollController : BaseController
    {
        //
        // GET: /InfiniteScroll/

        public ViewResult Index()
        {
            return View();
        }

        // Ajax Paging (cont'd)
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult AjaxPage(int? page)
        {
            var listPaged = GetPagedCodes(page); // GetPagedNames is found in BaseController
            //if (listPaged == null)
            //    return HttpNotFound();
            var  codes = listPaged.ToArray();

            //return Json(codes, JsonRequestBehavior.AllowGet);

            return Json(new
            {
                codes = listPaged.ToArray(),
                pager = listPaged.GetMetaData()
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
