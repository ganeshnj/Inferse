using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace inferse.Helpers
{
    public static class Extensions
    {
        static public Boolean IsMyWall(int RoutedUserId, int LoginedUserId)
        {
            if (RoutedUserId == LoginedUserId)
                return true;
            else
                return false;
        }
    }
}