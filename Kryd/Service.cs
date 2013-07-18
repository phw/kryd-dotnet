/*
 * Copyright (c) 2013 Philipp Wolfer <ph.wolfer@gmail.com>
 * Licensed under the MIT license.
 * See the file LICENSE.txt for copying permission.
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Kryd
{
    public class Service
    {
        private static Uri endpointUrl = new Uri("https://api.kryd.com/event.lua");

        private string accountId;
        private string apiKey;
        private string sessionId;

        private HttpClient httpClient;
        protected HttpClient HttpClient
        {
            get
            {
                if (this.httpClient == null)
                {
                    this.httpClient = new HttpClient();
                }

                return this.httpClient;
            }
        }

        public Service(string accountId, string apiKey, string sessionId)
        {
            this.accountId = accountId;
            this.apiKey = apiKey;
            this.sessionId = sessionId;
        }

        public async Task<HttpResponseMessage> SendEvent(string eventType, IDictionary<string, string> options) 
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, endpointUrl);
            request.Content = GetRequestContent(eventType, options);
            var response = await HttpClient.SendAsync(request).ConfigureAwait(false);
            return response;
        }

        public async Task<HttpResponseMessage> SendLoginEvent()
        {
            return await this.SendEvent("login", null).ConfigureAwait(false);
        }

        public async Task<HttpResponseMessage> SendRegisterEvent()
        {
            return await this.SendEvent("register", null).ConfigureAwait(false);
        }

        public async Task<HttpResponseMessage> SendIdentifyEvent(string firstname, string lastname, string email, string salutation)
        {
            var options = new Dictionary<string, string>
            {
                { "firstname", firstname },
                { "lastname", lastname },
                { "email", email },
                { "salutation", salutation.ToUpper() == "M" ? "M" : "F" },
            };

            return await this.SendEvent("identify", options).ConfigureAwait(false);
        }

        public async Task<HttpResponseMessage> SendItemviewEvent(string itemid)
        {
            var options = new Dictionary<string, string>
            {
                { "itemview", itemid },
            };

            return await this.SendEvent("basket", options).ConfigureAwait(false);
        }

        public async Task<HttpResponseMessage> SendBasketEvent(params string[] itemids)
        {
            IDictionary<string, string> options = null;
            if (itemids != null && itemids.Length > 0)
            {
                options = new Dictionary<string, string>
                {
                    { "itemids", string.Join(";", itemids.Distinct()) },
                };
            }

            return await this.SendEvent("basket", options).ConfigureAwait(false);
        }

        public async Task<HttpResponseMessage> SendCompleteEvent(decimal? orderValue = null)
        {
            IDictionary<string, string> options = null;
            if (orderValue.HasValue)
            {
                options = new Dictionary<string, string>
                {
                    { "order_value", orderValue.Value.ToString("#.00", CultureInfo.InvariantCulture) },
                };
            }

            return await this.SendEvent("complete", options).ConfigureAwait(false);
        }

        private FormUrlEncodedContent GetRequestContent(string eventType, IDictionary<string, string> options)
        {
            var parameters = new List<KeyValuePair<string, string>>();
            AddParameter(parameters, "accountid", this.accountId);
            AddParameter(parameters, "sessionid", this.sessionId);
            AddParameter(parameters, "eventtype", eventType);
            AddParameter(parameters, "key", this.apiKey);
            if (options != null)
            {
                AddParameter(parameters, "options", JsonConvert.SerializeObject(options));
            }

            return new FormUrlEncodedContent(parameters);
        }

        private void AddParameter(List<KeyValuePair<string, string>> parameters, string key, string value)
        {
            parameters.Add(new KeyValuePair<string, string>(key, value));
        }
    }
}
