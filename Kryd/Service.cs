﻿/*
 * Copyright (c) 2013 Philipp Wolfer <ph.wolfer@gmail.com>
 * Licensed under the MIT license.
 * See the file LICENSE.txt for copying permission.
 */

using System;
using System.Collections.Generic;
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
            var response = await HttpClient.SendAsync(request);
            return response;
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