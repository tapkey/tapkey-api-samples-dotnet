﻿using System.Net.Http.Headers;
using System.Text.Json;
using Tapkey.Api.Models;

namespace MVCAppAuthorizationCodePKCE.NET
{
    /// <summary>
    /// A typed HttpClient that knows how to talk with the Tapkey API
    /// </summary>
    public interface ITapkeyApiClient
    {
        /// <summary>
        /// Get the all the owner accounts of the current user.
        /// </summary>
        /// <param name="accessToken">The access token obtained from the authorization_code flow</param>
        /// <returns></returns>
        Task<IEnumerable<OwnerAccount>> GetUserOwnerAccounts(string accessToken);

        /// <summary>
        /// Get all the bound locks that belongs to the specified owner account
        /// </summary>
        /// <param name="accessToken">The access token obtained from the authorization_code flow</param>
        /// <param name="ownerAccountId">The owner account id</param>
        /// <returns></returns>
        Task<IEnumerable<BoundLock>> GetOwnerBoundLocks(string accessToken, string ownerAccountId);
    }

    public class TapkeyApiClient : ITapkeyApiClient
    {
        private readonly HttpClient _httpClient;
        private const string TapkeyApiVersion = "api/v1";
        private JsonSerializerOptions jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public TapkeyApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<IEnumerable<OwnerAccount>> GetUserOwnerAccounts(string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            // Get the owner accounts of the authorized user
            var ownersJsonResponse = await _httpClient.GetStringAsync($"{TapkeyApiVersion}/owners");
            var ownerAccounts = JsonSerializer.Deserialize<List<OwnerAccount>>(ownersJsonResponse, jsonOptions);

            return ownerAccounts;
        }

        public async Task<IEnumerable<BoundLock>> GetOwnerBoundLocks(string accessToken, string ownerAccountId)
        {
            if (string.IsNullOrEmpty(accessToken))
                throw new ArgumentException("accessToken cannot be null", nameof(accessToken));

            if (string.IsNullOrEmpty(ownerAccountId))
                throw new ArgumentException("Owner Account Id cannot be null", nameof(ownerAccountId));

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            var boundLocksJson = await _httpClient.GetStringAsync($"{TapkeyApiVersion}/owners/{ownerAccountId}/boundlocks");
            var boundLocks = JsonSerializer.Deserialize<List<BoundLock>>(boundLocksJson, jsonOptions);

            return boundLocks;
        }
    }
}
