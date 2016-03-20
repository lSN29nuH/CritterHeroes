﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CritterHeroes.Web.Contracts;
using CritterHeroes.Web.Contracts.Configuration;
using CritterHeroes.Web.Contracts.Events;
using CritterHeroes.Web.Contracts.Storage;
using CritterHeroes.Web.DataProviders.RescueGroups.Models;
using CritterHeroes.Web.DataProviders.RescueGroups.Responses;
using CritterHeroes.Web.Models.LogEvents;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using TOTD.Utility.EnumerableHelpers;
using TOTD.Utility.ExceptionHelpers;
using TOTD.Utility.StringHelpers;

namespace CritterHeroes.Web.DataProviders.RescueGroups.Storage
{
    public abstract class RescueGroupsStorage<T> : IRescueGroupsStorageContext<T> where T : class
    {
        private IRescueGroupsConfiguration _configuration;
        private IHttpClient _client;
        private IAppEventPublisher _publisher;

        private string _token;
        private string _tokenHash;

        private JsonSerializer _serializer;
        private List<T> _entityTracker;

        public RescueGroupsStorage(IRescueGroupsConfiguration configuration, IHttpClient client, IAppEventPublisher publisher)
        {
            ThrowIf.Argument.IsNull(configuration, nameof(configuration));

            this._configuration = configuration;
            this._client = client;
            this._publisher = publisher;

            this.ResultLimit = 100;

            this._serializer = new JsonSerializer();
            this._serializer.ContractResolver = new CamelCasePropertyNamesContractResolver();

            this._entityTracker = new List<T>();
        }

        public abstract string ObjectType
        {
            get;
        }

        public virtual string ObjectAction
        {
            get;
            protected set;
        }

        public abstract bool IsPrivate
        {
            get;
        }

        public abstract IEnumerable<SearchField> Fields
        {
            get;
        }

        protected abstract string SortField
        {
            get;
        }

        protected abstract string KeyField
        {
            get;
        }

        public IEnumerable<SearchFilter> Filters
        {
            get;
            set;
        }

        protected int ResultStart
        {
            get;
            set;
        }

        public int ResultLimit
        {
            get;
            set;
        }

        public string FilterProcessing
        {
            get;
            set;
        }

        public virtual async Task<T> GetAsync(string entityID)
        {
            SearchFilter filter = new SearchFilter()
            {
                FieldName = KeyField,
                Operation = SearchFilterOperation.Equal,
                Criteria = entityID
            };

            IEnumerable<T> result = await GetAllAsync(filter);
            return result.SingleOrDefault();
        }

        public async Task<IEnumerable<T>> GetAllAsync(params SearchFilter[] searchFilters)
        {
            Filters = searchFilters;
            ObjectAction = ObjectActions.Search;

            SearchModel search = new SearchModel()
            {
                ResultStart = ResultStart,
                ResultLimit = ResultLimit,
                ResultSort = SortField,
                Filters = Filters,
                FilterProcessing = FilterProcessing,
                Fields = Fields.Where(x => x.IsSelected).SelectMany(x => x.FieldNames)
            };

            RequestData requestData = new RequestData("search", search);

            return await GetEntitiesAsync(requestData);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            ObjectAction = ObjectActions.List;
            return await GetEntitiesAsync();
        }

        protected virtual async Task<IEnumerable<T>> GetEntitiesAsync(RequestData requestData = null)
        {
            List<T> result = new List<T>();
            ResultStart = 0;

            JObject request = await CreateRequest(requestData);

            JObject response = await SendRequestAsync<JObject>(request);

            JObject data;
            IEnumerable<T> batch;

            if (response["data"].HasValues)
            {
                data = response.Value<JObject>("data");
                batch = FromStorage(data.Properties());
                result.AddRange(batch);
            }

            int foundRows = response.Value<int>("foundRows");
            ResultStart += ResultLimit;
            while (ResultStart < foundRows)
            {
                ResultStart += ResultLimit;
                request = await CreateRequest(requestData);
                response = await SendRequestAsync<JObject>(request);
                if (response["data"].HasValues)
                {
                    data = response.Value<JObject>("data");
                    batch = FromStorage(data.Properties());
                    result.AddRange(batch);
                }
            }

            _entityTracker.AddRange(result);

            return result;
        }

        public virtual Task SaveAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public virtual Task SaveAsync(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }

        public virtual Task DeleteAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public virtual Task DeleteAllAsync()
        {
            throw new NotImplementedException();
        }

        public abstract IEnumerable<T> FromStorage(IEnumerable<JProperty> tokens);

        protected virtual async Task<JObject> CreateRequest(RequestData requestData = null)
        {
            JObject request = new JObject();

            if (IsPrivate || ObjectAction == ObjectActions.Search)
            {
                IEnumerable<JProperty> loginResult = await LoginAsync();
                foreach (JProperty property in loginResult)
                {
                    request.Add(property);
                }
            }
            else
            {
                // API key is needed for all public requests
                request.Add(new JProperty("apikey", _configuration.APIKey));
            }

            ThrowIf.Argument.IsNullOrEmpty(ObjectAction, nameof(ObjectAction));

            request.Add(new JProperty("objectType", ObjectType));
            request.Add(new JProperty("objectAction", ObjectAction));

            if (requestData != null)
            {
                JProperty dataProperty = new JProperty(requestData.Key, JToken.FromObject(requestData.Data, _serializer));
                request.Add(dataProperty);
            }

            return request;
        }

        protected async Task<IEnumerable<JProperty>> LoginAsync()
        {
            if (_token.IsNullOrEmpty() && _tokenHash.IsNullOrEmpty())
            {
                JObject request = new JObject(
                    new JProperty("username", _configuration.Username),
                    new JProperty("password", _configuration.Password),
                    new JProperty("accountNumber", _configuration.AccountNumber),
                    new JProperty("action", ObjectActions.Login)
                );

                DataResponseModel<LoginResponseData> response;
                try
                {
                    response = await SendRequestAsync<DataResponseModel<LoginResponseData>>(request);
                }
                catch (RescueGroupsException ex)
                {
                    throw new RescueGroupsException("Login", ex);
                }

                _token = response.Data.Token;
                _tokenHash = response.Data.TokenHash;
            }

            return new JProperty[]
            {
                new JProperty("token", _token),
                new JProperty("tokenHash", _tokenHash)
            };
        }

        protected void ValidateResponse(JObject response)
        {
            string status = response.Value<string>("status");
            if (!status.SafeEquals("ok"))
            {
                JToken property = response["messages"]["generalMessages"];
                string errorMessage;
                if (property != null && property.HasValues)
                {
                    errorMessage = property[0]["messageText"].Value<string>();
                }
                else
                {
                    errorMessage = "Unable to parse error response";
                }
                throw new RescueGroupsException(errorMessage);
            }
        }

        protected async Task<TResponse> SendRequestAsync<TResponse>(JObject request) where TResponse : class
        {
            string jsonRequest = request.ToString();
            HttpResponseMessage response = await _client.PostAsync(_configuration.Url, new StringContent(jsonRequest, Encoding.UTF8, "application/json"));

            string content = await response.Content.ReadAsStringAsync();

            RescueGroupsLogEvent logEvent;
            if (request["password"] != null)
            {
                logEvent = RescueGroupsLogEvent.Create(_configuration.Url, "Login", content, response.StatusCode);
            }
            else
            {
                logEvent = RescueGroupsLogEvent.Create(_configuration.Url, jsonRequest, content, response.StatusCode);
            }
            _publisher.Publish(logEvent);

            if (!response.IsSuccessStatusCode)
            {
                throw new RescueGroupsException("Unsuccesful status code: {0} - {1}; URL: {2}", (int)response.StatusCode, response.StatusCode, _configuration.Url);
            }

            JObject result = JObject.Parse(content);
            ValidateResponse(result);

            if (typeof(TResponse) == typeof(JObject))
            {
                return result as TResponse;
            }
            else
            {
                TResponse responseModel = JsonConvert.DeserializeObject<TResponse>(content);
                return responseModel;
            }
        }
    }
}
