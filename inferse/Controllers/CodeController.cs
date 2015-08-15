using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using inferse.Models;
using inferse.Filters;
using inferse.Helpers;

namespace inferse.Controllers
{
    public class CodeController : BaseController
    {
        private UsersContext db = new UsersContext();

        //
        // GET: /Default1/

        public ActionResult Index()
        {
            var codes = db.Codes.Include(c => c.User);
            return View(codes.ToList());
        }

        //
        // GET: /Default1/Details/5

        public ActionResult Details(int id = 0)
        {
            Code code = db.Codes.Find(id);
            if (code == null)
            {
                return HttpNotFound();
            }
            return View(code);
        }

        //
        // GET: /Default1/Create

        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.UserProfiles, "UserId", "UserName");
            return View();
        }

        //
        // POST: /Default1/Create

        [HttpPost]
        public ActionResult Create(Code code)
        {
            if (ModelState.IsValid)
            {
                db.Codes.Add(code);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.UserProfiles, "UserId", "UserName", code.UserId);
            return View(code);
        }

        //
        // GET: /Default1/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Code code = db.Codes.Find(id);
            if (code == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.UserProfiles, "UserId", "UserName", code.UserId);
            return View(code);
        }

        //
        // POST: /Default1/Edit/5

        [HttpPost]
        public ActionResult Edit(Code code)
        {
            if (ModelState.IsValid)
            {
                db.Entry(code).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.UserProfiles, "UserId", "UserName", code.UserId);
            return View(code);
        }

        //
        // GET: /Default1/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Code code = db.Codes.Find(id);
            if (code == null)
            {
                return HttpNotFound();
            }
            return View(code);
        }

        //
        // POST: /Default1/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Code code = db.Codes.Find(id);
            db.Codes.Remove(code);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [InitializeSimpleMembership]
        public ActionResult Home(int? page, int id = 0)
        {
            if (WebMatrix.WebData.WebSecurity.IsAuthenticated)
            {
                UserProfile user = db.UserProfiles.Find(WebMatrix.WebData.WebSecurity.GetUserId(User.Identity.Name));

                if (!Extensions.IsMyWall(id, user.UserId) && id != 0)
                {
                    UserProfile friend = db.UserProfiles.Find(id);
                    if (user.Following.Contains(friend))
                    {
                        ViewBag.Followship = "Following";
                    }
                    else
                    {
                        ViewBag.Followship = "Follow";
                    }

                    ViewBag.UserProfile = friend;
                    ViewBag.PostsCount = db.Codes.Where(c => c.UserId == friend.UserId).Count().ToString();
                    ViewBag.FollowerCount = friend.Followers.Count.ToString();
                    ViewBag.FollowingCount = friend.Following.Count.ToString();

                    return View(GetPagedCodes(page, friend.UserId));
                }
                else
                {
                    ViewBag.UserProfile = user;
                    ViewBag.PostsCount = db.Codes.Where(c => c.UserId == user.UserId).Count().ToString();
                    ViewBag.FollowerCount = user.Followers.Count.ToString();
                    ViewBag.FollowingCount = user.Following.Count.ToString();

                    return View(GetPagedCodes(page));
                }
            }
            else
            {
                UserProfile user = db.UserProfiles.Find(id);
                ViewBag.UserProfile = user;
                ViewBag.PostsCount = db.Codes.Where(c => c.UserId == user.UserId).Count().ToString();
                ViewBag.FollowerCount = user.Followers.Count.ToString();
                ViewBag.FollowingCount = user.Following.Count.ToString();

                return View(GetPagedCodes(page, db.UserProfiles.Find(id).UserId));
            }
        }

        [InitializeSimpleMembership]
        [HttpPost, ActionName("CodeBox")]
        public ActionResult CodeBox(Code code)
        {
            code.IsDeleted = false;
            code.LastModified = DateTime.Now;
            code.PostedOn = DateTime.Now;

            int userId = WebMatrix.WebData.WebSecurity.GetUserId(User.Identity.Name);
            code.User = db.UserProfiles.Find(userId);

            if (ModelState.IsValid)
            {
                db.Codes.Add(code);
                db.SaveChanges();
            }
            ModelState.Clear();

            return Content("You post has been publsihed", "text/html");
        }

        [InitializeSimpleMembership]
        [HttpPost, ActionName("Home")]
        public ActionResult Home(int? page, Code code)
        {
            code.IsDeleted = false;
            code.LastModified = DateTime.Now;
            code.PostedOn = DateTime.Now;

            int userId = WebMatrix.WebData.WebSecurity.GetUserId(User.Identity.Name);
            code.User = db.UserProfiles.Find(userId);

            if (ModelState.IsValid)
            {
                db.Codes.Add(code);
                db.SaveChanges();
            }
            UserProfile user = db.UserProfiles.Find(userId);
            ViewBag.UserProfile = user;

            var codes = GetPagedCodes(page);

            if (codes == null)
                return HttpNotFound();

            //  ViewBag.Codes = codes;

            return View(codes);
        }

    
        public PartialViewResult Followship(int id=0)
        {
            var user = db.UserProfiles.Find(WebMatrix.WebData.WebSecurity.GetUserId(User.Identity.Name));

            UserProfile friend = db.UserProfiles.Find(id);

            if (user.Following.Contains(friend))
            {
                user.Following.Remove(friend);
                ViewBag.Followship = "Follow";
            }
            else
            {
                user.Following.Add(friend);
                ViewBag.Followship = "Following";
            }
            db.SaveChanges();

            ViewBag.UserProfile = friend;

            var codes = db.Codes.Where(c => c.UserId == friend.UserId || c.Post.Contains("@" + friend.UserName)).OrderByDescending(c => c.PostedOn);
            return PartialView("Followship");
            //return  Content((string)ViewBag.Followship);
        }
        [InitializeSimpleMembership]
        public ActionResult Profile(int? page, int id = 0)
        {
            if (WebMatrix.WebData.WebSecurity.IsAuthenticated)
            {
                UserProfile user = db.UserProfiles.Find(WebMatrix.WebData.WebSecurity.GetUserId(User.Identity.Name));

                if (!Extensions.IsMyWall(id, user.UserId) && id != 0)
                {
                    UserProfile friend = db.UserProfiles.Find(id);
                    if (user.Following.Contains(friend))
                    {
                        ViewBag.Followship = "Following";
                    }
                    else
                    {
                        ViewBag.Followship = "Follow";
                    }

                    ViewBag.UserProfile = friend;
                    ViewBag.PostsCount = db.Codes.Where(c => c.UserId == friend.UserId).Count().ToString();
                    ViewBag.FollowerCount = friend.Followers.Count.ToString();
                    ViewBag.FollowingCount = friend.Following.Count.ToString();

                    return View(GetPagedCodes(page, friend.UserId));
                }
                else
                {
                    ViewBag.UserProfile = user;
                    ViewBag.PostsCount = db.Codes.Where(c => c.UserId == user.UserId).Count().ToString();
                    ViewBag.FollowerCount = user.Followers.Count.ToString();
                    ViewBag.FollowingCount = user.Following.Count.ToString();

                    return View(GetPagedCodes(page,user.UserId));
                }
            }
            else
            {
                UserProfile user = db.UserProfiles.Find(id);
                ViewBag.UserProfile = user;
                ViewBag.PostsCount = db.Codes.Where(c => c.UserId == user.UserId).Count().ToString();
                ViewBag.FollowerCount = user.Followers.Count.ToString();
                ViewBag.FollowingCount = user.Following.Count.ToString();

                return View(GetPagedCodes(page, db.UserProfiles.Find(id).UserId));
            }
        }

        [InitializeSimpleMembership]
        public ActionResult Mention(int? page)
        {
                int loginUserId = WebMatrix.WebData.WebSecurity.GetUserId(User.Identity.Name);
                UserProfile user = db.UserProfiles.Where(x => x.UserId == loginUserId).FirstOrDefault();

                ViewBag.UserProfile = user;

                // var codes = db.Codes.Where(c => c.UserId == user.UserId || c.Post.Contains("@" + user.UserName)).OrderByDescending(c => c.PostedOn);         

                ViewBag.PostsCount = db.Codes.Where(c => c.UserId == user.UserId).Count().ToString();
                ViewBag.FollowerCount = user.Followers.Count.ToString();
                ViewBag.FollowingCount = user.Following.Count.ToString();

                var codes = GetPagedCodes(page, user.UserName);

                if (codes == null)
                    return HttpNotFound();

                //  ViewBag.Codes = codes;

                return View(codes);
        }

        public ActionResult FollowingSearch(string term)
        {
            UserProfile user = db.UserProfiles.Find(WebMatrix.WebData.WebSecurity.GetUserId(User.Identity.Name));
            var followings = user.Following.Select(x=>x.UserName);
            return this.Json(followings.Where(t => t.StartsWith(term)), JsonRequestBehavior.AllowGet);
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }


    }
}