using inferse.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace inferse.Controllers
{
    public class BaseController : Controller
    {
        private UsersContext db = new UsersContext();

        protected IPagedList<Code> GetPagedCodes(int? page)
        {
            // return a 404 if user browses to before the first page
            if (page.HasValue && page < 1)
                return null;

            // retrieve list from database/whereverand
            var listUnpaged = GetStuffFromDatabase();

            // page the list
            const int pageSize = 10;
            var listPaged = listUnpaged.ToPagedList(page ?? 1, pageSize);

            // return a 404 if user browses to pages beyond last page. special case first page if no items exist
            if (listPaged.PageNumber != 1 && page.HasValue && page > listPaged.PageCount)
                return null;

            return listPaged;
        }

        protected IPagedList<Code> GetPagedCodes(int? page, int id)
        {
            // return a 404 if user browses to before the first page
            if (page.HasValue && page < 1)
                return null;

            // retrieve list from database/whereverand
            var listUnpaged = GetStuffFromDatabase(id);

            // page the list
            const int pageSize = 10;
            var listPaged = listUnpaged.ToPagedList(page ?? 1, pageSize);

            // return a 404 if user browses to pages beyond last page. special case first page if no items exist
            if (listPaged.PageNumber != 1 && page.HasValue && page > listPaged.PageCount)
                return null;

            return listPaged;
        }

        protected IPagedList<Code> GetPagedCodes(int? page, string username)
        {
            // return a 404 if user browses to before the first page
            if (page.HasValue && page < 1)
                return null;

            // retrieve list from database/whereverand
            var listUnpaged = GetStuffFromDatabase(username);

            // page the list
            const int pageSize = 10;
            var listPaged = listUnpaged.ToPagedList(page ?? 1, pageSize);

            // return a 404 if user browses to pages beyond last page. special case first page if no items exist
            if (listPaged.PageNumber != 1 && page.HasValue && page > listPaged.PageCount)
                return null;

            return listPaged;
        }
        protected IPagedList<UserProfile> GetPagedFollower(int? page, int id)
        {
            // return a 404 if user browses to before the first page
            if (page.HasValue && page < 1)
                return null;

            // retrieve list from database/whereverand
            var listUnpaged = GetFollowers(id);

            // page the list
            const int pageSize = 10;
            var listPaged = listUnpaged.ToPagedList(page ?? 1, pageSize);

            // return a 404 if user browses to pages beyond last page. special case first page if no items exist
            if (listPaged.PageNumber != 1 && page.HasValue && page > listPaged.PageCount)
                return null;

            return listPaged;
        }

        protected IPagedList<UserProfile> GetPagedFollowing(int? page, int id)
        {
            // return a 404 if user browses to before the first page
            if (page.HasValue && page < 1)
                return null;

            // retrieve list from database/whereverand
            var listUnpaged = GetFollowing(id);

            // page the list
            const int pageSize = 10;
            var listPaged = listUnpaged.ToPagedList(page ?? 1, pageSize);

            // return a 404 if user browses to pages beyond last page. special case first page if no items exist
            if (listPaged.PageNumber != 1 && page.HasValue && page > listPaged.PageCount)
                return null;

            return listPaged;
        }

        protected IEnumerable<UserProfile> GetFollowers(int id)
        {
            // var sampleData = new StreamReader(Server.MapPath("~/App_Data/Names.txt")).ReadToEnd();
            //return sampleData.Split('\n');
            UserProfile user = db.UserProfiles.Find(id);
            return user.Followers;
        }

        protected IEnumerable<UserProfile> GetFollowing(int id)
        {
            // var sampleData = new StreamReader(Server.MapPath("~/App_Data/Names.txt")).ReadToEnd();
            //return sampleData.Split('\n');
            UserProfile user = db.UserProfiles.Find(id);
            return user.Following;
        }

        // in this case we return IEnumerable<string>, but in most
        // - DB situations you'll want to return IQueryable<string>
        protected IEnumerable<Code> GetStuffFromDatabase()
        {
           // var sampleData = new StreamReader(Server.MapPath("~/App_Data/Names.txt")).ReadToEnd();
            //return sampleData.Split('\n');
            int userId = WebMatrix.WebData.WebSecurity.GetUserId(User.Identity.Name);
            IEnumerable<int> followings = db.UserProfiles.Find(userId).Following.Select(x=>x.UserId);
            var codes = db.Codes.Where(u => u.UserId == userId || followings.Contains(u.UserId)).OrderByDescending(c=>c.PostedOn);
            return codes;
        }

        protected IEnumerable<Code> GetStuffFromDatabase(int id)
        {
            // var sampleData = new StreamReader(Server.MapPath("~/App_Data/Names.txt")).ReadToEnd();
            //return sampleData.Split('\n');
            var codes = db.Codes.Where(u => u.UserId == id).OrderByDescending(c => c.PostedOn);
            return codes;
        }

        protected IEnumerable<Code> GetStuffFromDatabase(string username)
        {
            // var sampleData = new StreamReader(Server.MapPath("~/App_Data/Names.txt")).ReadToEnd();
            //return sampleData.Split('\n');
            var codes = db.Codes.Where(u => u.Post.Contains("@" + username)).OrderByDescending(c => c.PostedOn);
            return codes;
        }

    }
}
