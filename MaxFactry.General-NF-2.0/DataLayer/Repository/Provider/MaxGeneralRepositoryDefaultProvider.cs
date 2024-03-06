// <copyright file="MaxWebFileRepositoryDefaultProvider.cs" company="Lakstins Family, LLC">
// Copyright (c) Brian A. Lakstins (http://www.lakstins.com/brian/)
// </copyright>

#region License
// <license>
// This software is provided 'as-is', without any express or implied warranty. In no 
// event will the author be held liable for any damages arising from the use of this 
// software.
//  
// Permission is granted to anyone to use this software for any purpose, including 
// commercial applications, and to alter it and redistribute it freely, subject to the 
// following restrictions:
// 
// 1. The origin of this software must not be misrepresented; you must not claim that 
// you wrote the original software. If you use this software in a product, an 
// acknowledgment (see the following) in the product documentation is required.
// 
// Portions Copyright (c) Brian A. Lakstins (http://www.lakstins.com/brian/)
// 
// 2. Altered source versions must be plainly marked as such, and must not be 
// misrepresented as being the original software.
// 
// 3. This notice may not be removed or altered from any source distribution.
// </license>
#endregion

#region Change Log
// <changelog>
// <change date="6/4/2015" author="Brian A. Lakstins" description="Initial creation">
// <change date="6/19/2019" author="Brian A. Lakstins" description="Remove handling of configuration and exception logging information.">
// <change date="8/1/2023" author="Brian A. Lakstins" description="Added selecting remote HTTP process">
// </changelog>
#endregion

namespace MaxFactry.General.DataLayer.Provider
{
    using System;
    using System.IO;
    using System.Net;
#if net4_52 || netcore2 || netstandard1_2
    using System.Threading.Tasks;
    using System.Net.Http;
    using System.Net.Http.Headers;
#endif
    using MaxFactry.Core;
    using MaxFactry.Base.DataLayer;
    using MaxFactry.Base.DataLayer.Provider;
    using MaxFactry.General.DataLayer;

    /// <summary>
    /// Provider for MaxApplicationInternetRepository
    /// </summary>
    public class MaxGeneralRepositoryDefaultProvider : MaxBaseIdRepositoryDefaultProvider, IMaxGeneralRepositoryProvider
    {
        private static object _oLock = new object();

        public override MaxDataList Select(MaxData loData, MaxDataQuery loDataQuery, int lnPageIndex, int lnPageSize, string lsOrderBy, out int lnTotal, params string[] laDataNameList)
        {
            if (loData.DataModel is MaxHttpClientDataModel)
            {
                MaxHttpClientDataModel loDataModel = new MaxHttpClientDataModel();
                string lsAlternateId = this.GetValue(loDataQuery, loDataModel.AlternateId) as string;
                if (!string.IsNullOrEmpty(lsAlternateId) && lsAlternateId == "Remote")
                {
                    return this.SelectRemoteHttp(loData, loDataQuery, lnPageIndex, lnPageSize, out lnTotal, laDataNameList);
                }
            }
            else
            {
                MaxData loRemoteData = loData.Get("RemoteData") as MaxData;
                if (loRemoteData != null && loRemoteData.DataModel is MaxHttpClientDataModel)
                {
                    return this.SelectRemoteHttp(loRemoteData, loDataQuery, lnPageIndex, lnPageSize, out lnTotal, laDataNameList);                    
                }
            }

            return base.Select(loData, loDataQuery, lnPageIndex, lnPageSize, lsOrderBy, out lnTotal, laDataNameList);
        }

        /// <summary>
        /// Downloads data from an external url.
        /// </summary>
        /// <param name="loDataModel">The content data model.</param>
        /// <param name="lsUrl">The url to the file.</param>
        /// <returns>Data from download of the file..</returns>
        public virtual MaxData Download(MaxFileDownloadDataModel loDataModel, string lsUrl)
        {
            return this.DownloadConditional(loDataModel, lsUrl);
        }

        public virtual MaxDataList SelectRemoteHttp(MaxData loData, MaxDataQuery loDataQuery, int lnPageIndex, int lnPageSize, out int lnTotal, params string[] laDataNameList)
        {
            return this.SelectRemoteHttpConditional(loData, loDataQuery, lnPageIndex, lnPageSize, out lnTotal, laDataNameList);
        }

#if net2 || netcore2 || netstandard1_2
        /// <summary>
        /// Downloads data from an external url.
        /// </summary>
        /// <param name="loDataModel">The content data model.</param>
        /// <param name="lsUrl">The url to the file.</param>
        /// <returns>Data from download of the file..</returns>
        public virtual MaxData DownloadConditional(MaxFileDownloadDataModel loDataModel, string lsUrl)
        {
            MaxData loR = new MaxData(loDataModel);
            loR.Set(loDataModel.RequestUrl, lsUrl);
            loR.Set(loDataModel.Name, lsUrl);
            WebRequest loRequest = HttpWebRequest.Create(new Uri(lsUrl));
            try
            {
                HttpWebResponse loResponse = (HttpWebResponse)loRequest.GetResponse();
                Stream loResponseStream = loResponse.GetResponseStream();
                loR.Set(loDataModel.Content, loResponseStream);
                loR.Set(loDataModel.ContentLength, loResponse.ContentLength);
                loR.Set(loDataModel.ContentType, loResponse.ContentType);
                loR.Set(loDataModel.ResponseUrl, loResponse.ResponseUri.ToString());
                string lsHeader = string.Empty;
                for (int lnW = 0; lnW < loResponse.Headers.Keys.Count; lnW++)
                {
                    lsHeader += loResponse.Headers.Keys[lnW] + "=" + loResponse.Headers[loResponse.Headers.Keys[lnW]] + "\r\n";
                }

                loR.Set(loDataModel.ResponseHeader, lsHeader);

                string lsCookie = string.Empty;
                for (int lnW = 0; lnW < loResponse.Cookies.Count; lnW++)
                {
                    lsCookie += loResponse.Cookies[lnW].Name + "=" + loResponse.Cookies[lnW].Value + "\r\n";
                }

                loR.Set(loDataModel.ResponseCookie, lsCookie);
                loR.Set(loDataModel.StatusCode, loResponse.StatusCode.ToString());
                loR.Set(loDataModel.ResponseHost, loResponse.ResponseUri.Host.ToLower());
            }
            catch (Exception loE)
            {
                loR.Set(loDataModel.ContentType, "ERROR");
                MemoryStream loStream = new MemoryStream(System.Text.Encoding.Unicode.GetBytes(loE.ToString()));
                loR.Set(loDataModel.Content, loStream);
            }

            return loR;
        }

#else
        /// <summary>
        /// Downloads data from an external url.
        /// </summary>
        /// <param name="loDataModel">The content data model.</param>
        /// <param name="lsUrl">The url to the file.</param>
        /// <returns>Data from download of the file..</returns>
        public virtual MaxData DownloadConditional(MaxFileDownloadDataModel loDataModel, string lsUrl)
        {
            throw new NotImplementedException();
        }

#endif

#if net4_52 || netcore2 || netstandard1_2
        private HttpClient _oHttpClient = null;

        protected HttpClient HttpClient
        {
            get
            {
                if (null == _oHttpClient)
                {
                    lock (_oLock)
                    {
                        if (null == _oHttpClient)
                        {
                            _oHttpClient = GetMaxClient();
                        }
                    }
                }

                return _oHttpClient;
            }
        }

        protected HttpClient GetMaxClient()
        {
            HttpClient loR = null;
            System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;
            HttpClientHandler loHandler = new System.Net.Http.HttpClientHandler();
            if (loHandler.SupportsAutomaticDecompression)
            {
                loHandler.AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate;
            }

            loHandler.CookieContainer = new System.Net.CookieContainer();
            loR = new HttpClient(loHandler);
            loR.Timeout = new TimeSpan(0, 0, 10);
            loR.DefaultRequestHeaders.Add("User-Agent", "Mozilla /5.0 (MaxFactry .NET Framework)");
            loR.DefaultRequestHeaders.Add("DNT", "1");
            loR.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
            loR.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return loR;
        }

        /// <summary>
        /// Selects data from the database.
        /// </summary>
        /// <param name="loData">Element with data used in the filter.</param>
        /// <param name="loDataQuery">Query information to filter results.</param>
        /// <param name="lnPageIndex">Page to return.</param>
        /// <param name="lnPageSize">Items per page.</param>
        /// <param name="lnTotal">Total items found.</param>
        /// <param name="laDataNameList">list of fields to return from select.</param>
        /// <returns>List of data from select.</returns>
        public virtual MaxDataList SelectRemoteHttpConditional(MaxData loData, MaxDataQuery loDataQuery, int lnPageIndex, int lnPageSize, out int lnTotal, params string[] laDataNameList)
        {
            MaxLogLibrary.Log(MaxEnumGroup.LogDebug, "Select [" + loData.DataModel.DataStorageName + "] start", "MaxDataContextHttpClientProvider");
            MaxHttpClientDataModel loDataModel = loData.DataModel as MaxHttpClientDataModel;
            if (null == loDataModel)
            {
                throw new MaxException("MaxHttpClientDataModel is expected by MaxDataContextHttpClientProvider");
            }

            MaxDataList loR = new MaxDataList(loDataModel);
            MaxData loDataReturn = new MaxData(loData.DataModel);
            lnTotal = 0;
            DateTime ldRequestStart = DateTime.UtcNow;
            Uri loReqestUrl = loData.Get(loDataModel.RequestUrl) as Uri;
            string lsRequestUrl = this.GetValue(loDataQuery, loDataModel.RequestUrl) as string;
            if (null == lsRequestUrl)
            {
                lsRequestUrl = loData.Get(loDataModel.RequestUrl) as string;
            }

            if (null != lsRequestUrl)
            {
                loReqestUrl = new Uri(lsRequestUrl);
            }

            object loRequestContent = this.GetValue(loDataQuery, loDataModel.RequestContent);
            if (null == loRequestContent)
            {
                loRequestContent = loData.Get(loDataModel.RequestContent);
            }
            
            try
            {
                HttpClient loClient = HttpClient;
                System.Collections.Generic.Dictionary<string, string> loContentDictionary = new System.Collections.Generic.Dictionary<string, string>();
                bool lbStringContent = false;
                string lsClientId = this.GetValue(loDataQuery, loDataModel.ClientId) as string;
                string lsClientSecret = this.GetValue(loDataQuery, loDataModel.ClientSecret) as string;
                string lsToken = this.GetValue(loDataQuery, loDataModel.Token) as string;

                HttpContent loContent = null;
                if (loRequestContent is HttpContent)
                {
                    loContent = (HttpContent)loRequestContent;
                }
                else if (loRequestContent is string)
                {
                    loContent = new StringContent((string)loRequestContent);
                }
                else if (loRequestContent is MaxIndex)
                {
                    string[] laKey = ((MaxIndex)loRequestContent).GetSortedKeyList();
                    foreach (string lsKey in laKey)
                    {
                        if (lsKey == "BasicAuthClientId")
                        {
                            lsClientId = ((MaxIndex)loRequestContent).GetValueString(lsKey);
                        }
                        else if (lsKey == "BasicAuthClientSecret")
                        {
                            lsClientSecret = ((MaxIndex)loRequestContent).GetValueString(lsKey);
                        }
                        else if (lsKey == "BearerAuthToken")
                        {
                            lsToken = ((MaxIndex)loRequestContent).GetValueString(lsKey);
                        }
                        else if (lsKey == "StringContent")
                        {
                            lbStringContent = MaxConvertLibrary.ConvertToBoolean(typeof(object), ((MaxIndex)loRequestContent)[lsKey]);
                        }
                        else
                        {
                            loContentDictionary.Add(lsKey, ((MaxIndex)loRequestContent).GetValueString(lsKey));
                        }
                    }
                }
                else if (null != loRequestContent)
                {
                    loContent = new StringContent(MaxConvertLibrary.SerializeObjectToString(loRequestContent));
                }



                //// Access the URL in a secure manner, so create a new client to use
                if ((!string.IsNullOrEmpty(lsClientId) && !string.IsNullOrEmpty(lsClientSecret)) || !string.IsNullOrEmpty(lsToken))
                {
                    loClient = GetMaxClient();
                    if (!string.IsNullOrEmpty(lsClientId) && !string.IsNullOrEmpty(lsClientSecret))
                    {
                        string lsAuth = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(string.Format("{0}:{1}", lsClientId, lsClientSecret)));
                        loClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", lsAuth);
                        if (!loContentDictionary.ContainsKey("grant_type"))
                        {
                            loContentDictionary.Add("grant_type", "client_credentials");
                        }
                        string lsScope = this.GetValue(loDataQuery, loDataModel.Scope) as string;
                        if (null == lsScope)
                        {
                            lsScope = loData.Get(loDataModel.Scope) as string;
                        }

                        if (!loContentDictionary.ContainsKey("scope") && null != lsScope)
                        {
                            loContentDictionary.Add("scope", lsScope);
                        }
                    }
                    else if (!string.IsNullOrEmpty(lsToken))
                    {
                        loClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", lsToken);
                    }

                    CacheControlHeaderValue loCache = new CacheControlHeaderValue();
                    loCache.NoCache = true;
                    loClient.DefaultRequestHeaders.CacheControl = loCache;
                }

                if (loContentDictionary.Count > 0)
                {
                    if (lbStringContent)
                    {
                        loContent = new System.Net.Http.StringContent(MaxConvertLibrary.SerializeObjectToString(loContentDictionary));
                    }
                    else
                    {
                        loContent = new System.Net.Http.FormUrlEncodedContent(loContentDictionary);
                    }
                }

                HttpResponseMessage loHttpClientResponse = null;
                object loResponseContent = null;
                Task<System.Net.Http.HttpResponseMessage> loTask = null;
                if (null != loContent)
                {
                    loTask = loClient.PostAsync(loReqestUrl, loContent);
                }
                else
                {
                    loTask = loClient.GetAsync(loReqestUrl);
                }

                while (!loTask.IsCompleted)
                {
                    System.Threading.Thread.Sleep(10);
                }

                if (loTask.Status == TaskStatus.RanToCompletion)
                {
                    loHttpClientResponse = loTask.Result;

                    if (loHttpClientResponse.Content != null)
                    {
                        Task loContentTask = null;
                        if (loHttpClientResponse.Content.GetType() == typeof(System.Net.Http.StreamContent))
                        {
                            loContentTask = loHttpClientResponse.Content.ReadAsStreamAsync();
                        }
                        else if (loHttpClientResponse.Content.GetType() == typeof(string))
                        {
                            loContentTask = loHttpClientResponse.Content.ReadAsStringAsync();
                        }
                        else if (loHttpClientResponse.Content.GetType() == typeof(byte[]))
                        {
                            loContentTask = loHttpClientResponse.Content.ReadAsByteArrayAsync();
                        }

                        while (!loContentTask.IsCompleted)
                        {
                            System.Threading.Thread.Sleep(10);
                        }

                        if (loContentTask.Status == TaskStatus.RanToCompletion)
                        {
                            if (loContentTask is Task<Stream>)
                            {
                                loResponseContent = ((Task<Stream>)loContentTask).Result;
                            }
                            else if (loContentTask is Task<string>)
                            {
                                loResponseContent = ((Task<string>)loContentTask).Result;
                            }
                            else if (loContentTask is Task<byte[]>)
                            {
                                loResponseContent = ((Task<byte[]>)loContentTask).Result;
                            }
                        }
                        else
                        {
                            throw new MaxException("Read content task to " + loReqestUrl + " completed with status " + loTask.Status.ToString());
                        }
                    }
                }
                else
                {
                    throw new MaxException("Task to " + loReqestUrl + " completed with status " + loTask.Status.ToString());
                }

                DateTime ldResponseEnd = DateTime.UtcNow;
                loDataReturn.Set(loDataModel.RequestTime, ldRequestStart);
                loDataReturn.Set(loDataModel.ResponseTime, ldResponseEnd);
                loDataReturn.Set(loDataModel.Response, loHttpClientResponse);
                loDataReturn.Set(loDataModel.ResponseContent, loResponseContent);
                loDataReturn.Set(loDataModel.RequestUrl, loReqestUrl);
                loDataReturn.Set(loDataModel.RequestContent, loContent);
            }
            catch (Exception loE)
            {
                loDataReturn.Set(loDataModel.Exception, loE);
            }

            loR.Add(loDataReturn);
            lnTotal = loR.Count;
            MaxLogLibrary.Log(MaxEnumGroup.LogDebug, "Select [" + loData.DataModel.DataStorageName + "] end", "MaxDataContextHttpClientProvider");
            return loR;
        }

#else

        public virtual MaxDataList SelectRemoteHttpConditional(MaxData loData, MaxDataQuery loDataQuery, int lnPageIndex, int lnPageSize, out int lnTotal, params string[] laDataNameList)
        {
            throw new NotImplementedException();
        }

#endif
    }
}
