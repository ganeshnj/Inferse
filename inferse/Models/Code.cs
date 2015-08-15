using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace inferse.Models
{
    public class Code
    {
        public int CodeId { get; set; }
        public string Post { get; set; }
        public System.DateTime PostedOn { get; set; }
        public bool IsDeleted { get; set; }
        public System.DateTime LastModified { get; set; }
        public int UserId { get; set; }

        public virtual UserProfile User { get; set; }
    }
}