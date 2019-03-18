using System;

namespace Tapkey.Api.Models
{
    public class BoundLock
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public LockType LockType { get; set; }

        public DateTime BindDate { get; set; }
    }
}
