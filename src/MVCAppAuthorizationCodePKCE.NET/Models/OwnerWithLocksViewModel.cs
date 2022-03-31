using Tapkey.Api.Models;

namespace MVCAppAuthorizationCodePKCE.NET.Models
{
    public class OwnerWithLocksViewModel
    {
        public OwnerAccount Owner { get; set; }

        public List<BoundLock> OwnerLocks { get; set; }

        public OwnerWithLocksViewModel()
        {
            OwnerLocks = new List<BoundLock>();
        }
    }
}
