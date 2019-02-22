using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MVCAppAuthorizationCodePKCE.NETCore.Models;
using Tapkey.Api.Models;

namespace MVCAppAuthorizationCodePKCE.NETCore.Controllers
{
    public class OwnerLocksController : Controller
    {
        private readonly ITapkeyApiClient _tapkeyApiClient;
        private readonly IMemoryCache _memoryCache;

        public OwnerLocksController(
            IMemoryCache memoryCache,
            ITapkeyApiClient tapkeyApiClient)
        {
            _memoryCache = memoryCache ?? throw new System.ArgumentNullException(nameof(memoryCache));
            _tapkeyApiClient = tapkeyApiClient;
        }

        public async Task<IActionResult> Index()
        {
            if (!_memoryCache.TryGetValue<string>(AppConstants.AccessTokenCacheKey, out var accessToken))
            {
                return Unauthorized();
            }

            var ownerAccounts = await _tapkeyApiClient.GetUserOwnerAccounts(accessToken);
            var ownerWithLocks = await GetOwnerLocks(accessToken, ownerAccounts);

            return View(ownerWithLocks);
        }

        private async Task<IEnumerable<OwnerWithLocksViewModel>> GetOwnerLocks(
            string accessToken,
            IEnumerable<OwnerAccount> ownerAccounts)
        {
            var ownerLocks = new List<OwnerWithLocksViewModel>();

            // Loop over user's Owner Accounts...
            foreach (var ownerAccount in ownerAccounts)
            {
                // ... and query for bound locks
                var boundLocks = await _tapkeyApiClient.GetOwnerBoundLocks(accessToken, ownerAccount.Id);

                var ownerWithLockViewModel = new OwnerWithLocksViewModel
                {
                    Owner = ownerAccount,
                    OwnerLocks = boundLocks.ToList()
                };

                ownerLocks.Add(ownerWithLockViewModel);
            }

            return ownerLocks;
        }
    }
}