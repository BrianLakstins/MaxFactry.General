// <copyright file="MaxBaseApiController.cs" company="Lakstins Family, LLC">
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
// <change date="12/19/2015" author="Brian A. Lakstins" description="Initial creation">
// <change date="9/9/2016" author="Brian A. Lakstins" description="Add method to configure string for sending back as response.">
// <change date="10/1/2018" author="Brian A. Lakstins" description="Add current URL to arguments to get storage key.">
// <change date="10/19/2018" author="Brian A. Lakstins" description="Fix current URL to be right call.">
// <change date="10/20/2018" author="Brian A. Lakstins" description="Add setting MaxStorageCookie">
// <change date="2/13/2020" author="Brian A. Lakstins" description="Add ability to get client ip address and some other information">
// <change date="2/18/2020" author="Brian A. Lakstins" description="Fix getting username and host name from request">
// <change date="10/12/2020" author="Brian A. Lakstins" description="Add some methods to help in running tasks async">
// <change date="12/1/2020" author="Brian A. Lakstins" description="Add methods to help with getting data from client and creating response.  Integrate default ViewModel for responses">
// <change date="12/10/2020" author="Brian A. Lakstins" description="Update to get data for Api Request">
// <change date="12/15/2020" author="Brian A. Lakstins" description="Update naming to be Request and Response">
// <change date="12/16/2020" author="Brian A. Lakstins" description="Add returning a status code with an object">
// <change date="12/19/2020" author="Brian A. Lakstins" description="Make GetRequest just use the data to map and not need an anonymous object. Add a method to get the current user that also works with tokens.">
// <change date="12/30/2020" author="Brian A. Lakstins" description="Turn sub objects of request into MaxIndex">
// <change date="12/31/2020" author="Brian A. Lakstins" description="Fix getting MaxIndex based on Json object">
// <change date="1/13/2021" author="Brian A. Lakstins" description="Add timeout to running http client processes">
// <change date="2/22/2021" author="Brian A. Lakstins" description="Trim input">
// <change date="2/25/2021" author="Brian A. Lakstins" description="Trim all input">
// <change date="3/15/2021" author="Brian A. Lakstins" description="Add handling of file uploads">
// <change date="3/16/2021" author="Brian A. Lakstins" description="Fix handling of file uploads">
// <change date="7/9/2021" author="Brian A. Lakstins" description="Add array handling">
// <change date="7/22/2021" author="Brian A. Lakstins" description="Add array handling when array of strings instead of object">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.IIS.Mvc4.PresentationLayer
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Reflection;
    using System.ServiceModel.Channels;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;
    using System.Web.Security;
    using Microsoft.Owin;
    using MaxFactry.Core;
    using MaxFactry.Base.BusinessLayer;
    using MaxFactry.General.AspNet.PresentationLayer;
    using MaxFactry.General.BusinessLayer;

    /// <summary>
    /// Base class for any Api controller.
    /// </summary>
    public abstract class MaxBaseApiController : ApiController
    {
        private const string _sHttpContext = "MS_HttpContext";
        private const string _sRemoteEndpointMessage = "System.ServiceModel.Channels.RemoteEndpointMessageProperty";
        private const string _sOwinContext = "MS_OwinContext";


        public MaxBaseApiController()
        {
            MaxLogLibrary.Log(MaxEnumGroup.LogInfo, "Created [" + this.GetType().ToString() + "] Api Controller", "MaxController");
        }

        public string Render(string lsView, object loModel, MaxIndex loMetaIndex)
        {
            string lsR = string.Empty;
            string lsName = this.GetType().Name.Substring(0, this.GetType().Name.Length - "Controller".Length);
            string lsViewPath = "/Views/" + lsName + "/" + lsView + ".cshtml";
            lsR = MaxHtmlHelperLibrary.GetHtml(lsViewPath, loModel, loMetaIndex);
            return lsR;
        }

        public static bool IsNextRunTime(string lsKey, TimeSpan loDuration)
        {
            bool lbR = false;
            DateTime ldLastUpdate = MaxConvertLibrary.ConvertToDateTime(typeof(object), MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopePersistent, lsKey));
            DateTime ldDateCheck = DateTime.UtcNow - loDuration;
            if (ldLastUpdate < ldDateCheck)
            {
                lbR = true;
            }

            return lbR;
        }

        public static void StartProcess(Uri loRequestUri, string lsProcess, string lsContent, TimeSpan loTimeout)
        {
            try
            {
                HttpClient loClient = new HttpClient();
                loClient.Timeout = loTimeout;
                loClient.BaseAddress = new Uri(loRequestUri.Scheme + "://" + loRequestUri.Authority + "/");
                loClient.DefaultRequestHeaders.Accept.Clear();
                Task<HttpResponseMessage> loTask = loClient.PostAsJsonAsync(lsProcess, lsContent);
                while (!loTask.IsCompleted && !loTask.IsCanceled && !loTask.IsFaulted)
                {
                    Thread.Sleep(1000);
                }

                if (loTask.IsFaulted)
                {
                    MaxLogLibrary.Log(new MaxLogEntryStructure("MaxBaseApiController.StartProcess", MaxEnumGroup.LogError, "MaxBaseApiController: Error running Process for [{Uri}][{lsProcess}][{lsContent}]", loTask.Exception, loRequestUri.ToString(), lsProcess, lsContent));
                }
            }
            catch (Exception loE)
            {
                MaxLogLibrary.Log(new MaxLogEntryStructure("MaxBaseApiController.StartProcess", MaxEnumGroup.LogError, "MaxBaseApiController.StartProcessEventHookLog: Error starting Process for [{Uri}][{lsProcess}][{lsContent}]", loE, loRequestUri.ToString(), lsProcess, lsContent));
            }
        }

        protected virtual HttpResponseMessage GetResponseMessage()
        {
            HttpResponseMessage loR = this.Request.CreateResponse();
            Collection<CookieHeaderValue> loCookieHeaderList = this.Request.Headers.GetCookies();
            bool lbHasStorageCookie = false;
            foreach (CookieHeaderValue loCookieHeader in loCookieHeaderList)
            {
                foreach (CookieState loCookie in loCookieHeader.Cookies)
                {
                    if (loCookie.Name == MaxFactry.General.AspNet.IIS.MaxAppLibrary.MaxStorageKeyCookieName)
                    {
                        lbHasStorageCookie = true;
                    }
                }
            }

            if (!lbHasStorageCookie)
            {
                string lsValue = MaxOwinLibrary.GetStorageKeyForRequest(this.Request.RequestUri);
                if (!string.IsNullOrEmpty(lsValue))
                {
                    CookieHeaderValue loCookie = new CookieHeaderValue(MaxFactry.General.AspNet.IIS.MaxAppLibrary.MaxStorageKeyCookieName, lsValue);
                    loCookie.Expires = DateTime.UtcNow.AddMonths(1);
                    loCookie.Domain = this.Request.RequestUri.Host;
                    try
                    {
                        loR.Headers.AddCookies(new CookieHeaderValue[] { loCookie });
                    }
                    catch (Exception loE)
                    {
                        MaxLogLibrary.Log(new MaxLogEntryStructure(MaxEnumGroup.LogError, "Error setting client cookie", loE));
                    }

                    MaxFactry.Core.MaxLogLibrary.Log(MaxFactry.Core.MaxEnumGroup.LogInfo, "GetResponseMessage()->Sending MaxStorageKey Cookie " + lsValue, "MaxHttpApplicationLibrary");
                }

            }

            return loR;
        }

        protected virtual HttpResponseMessage GetResponseMessage(string lsReturn)
        {
            HttpResponseMessage loR = this.GetResponseMessage();
            loR.Content = new StringContent(lsReturn);
            loR.Content.Headers.Remove("Content-Type");
            loR.Content.Headers.Add("Content-Type", "text/plain");
            MaxLogLibrary.ClearRecent();
            return loR;
        }

        protected virtual HttpResponseMessage GetResponseMessage(object loReturn)
        {
            return this.GetResponseMessage(loReturn, System.Net.HttpStatusCode.OK);
        }

        protected virtual HttpResponseMessage GetResponseMessage(object loReturn, System.Net.HttpStatusCode loStatus)
        {
            HttpResponseMessage loR = this.GetResponseMessage();
            loR.StatusCode = loStatus;
            loR.Content = new StringContent(MaxConvertLibrary.SerializeObjectToString(typeof(object), loReturn));
            loR.Content.Headers.Remove("Content-Type");
            loR.Content.Headers.Add("Content-Type", "text/json");
            MaxLogLibrary.ClearRecent();
            return loR;
        }

        protected virtual HttpResponseMessage GetResponseMessage(MaxApiResponseViewModel loReturn)
        {
            HttpResponseMessage loR = this.GetResponseMessage();
            loR.StatusCode = loReturn.Status;
            loR.Content = new StringContent(MaxConvertLibrary.SerializeObjectToString(typeof(object), loReturn));
            loR.Content.Headers.Remove("Content-Type");
            loR.Content.Headers.Add("Content-Type", "text/json");
            MaxLogLibrary.ClearRecent();
            return loR;
        }

        /// <summary>
        /// Gets the request in whatever format it is being supplied
        /// </summary>
        /// <typeparam name="T">Type of anonymous class</typeparam>
        /// <param name="loRequestItem">Anonymous item with expected field names for request</param>
        /// <returns>Request information</returns>
        protected virtual async Task<MaxApiRequestViewModel> GetRequest()
        {
            MaxApiRequestViewModel loR = new MaxApiRequestViewModel(this.Request);
            string lsContent = string.Empty;
            string lsMediaType = string.Empty;
            MaxIndex loItem = new MaxIndex();
            try
            {
                lsMediaType = string.Empty;
                if (null != this.Request.Content && null != this.Request.Content.Headers && null != this.Request.Content.Headers.ContentType)
                {
                    lsMediaType = this.Request.Content.Headers.ContentType.MediaType;
                }

                if (lsMediaType == "multipart/form-data")
                {
                    MultipartMemoryStreamProvider loMemoryProvider = await this.Request.Content.ReadAsMultipartAsync();
                    for (int lnC = 0; lnC < loMemoryProvider.Contents.Count; lnC++)
                    {
                        string lsName = Newtonsoft.Json.JsonConvert.DeserializeObject(loMemoryProvider.Contents[lnC].Headers.ContentDisposition.Name) as string;
                        if (null != loMemoryProvider.Contents[lnC].Headers.ContentDisposition.FileName)
                        {
                            string lsFileName = Newtonsoft.Json.JsonConvert.DeserializeObject(loMemoryProvider.Contents[lnC].Headers.ContentDisposition.FileName) as string;
                            byte[] laContent = await loMemoryProvider.Contents[lnC].ReadAsByteArrayAsync();
                            MaxIndex loFile = new MaxIndex();
                            loFile.Add("FileName", lsFileName);
                            loFile.Add("FileContent", laContent);
                            loFile.Add("MediaType", loMemoryProvider.Contents[lnC].Headers.ContentType.MediaType);
                            loItem.Add(lsName, loFile);
                        }
                        else
                        {
                            string lsFieldContent = await loMemoryProvider.Contents[lnC].ReadAsStringAsync();
                            loItem.Add(lsName, lsFieldContent);
                        }
                    }
                }
                else
                {
                    Stream loContent = await this.Request.Content.ReadAsStreamAsync();
                    StreamReader loReader = new StreamReader(loContent);
                    lsContent = loReader.ReadToEnd();
                    loReader.Close();
                }
            }
            catch (Exception loE)
            {
                MaxLogLibrary.Log(new MaxLogEntryStructure(MaxEnumGroup.LogError, "Error reading content from stream.", loE));
            }

            loR.Content = lsContent;

            if (!string.IsNullOrEmpty(lsContent) && lsMediaType == "application/json")
            {
                dynamic loData = Newtonsoft.Json.JsonConvert.DeserializeObject(lsContent);
                MaxIndex loIndex = this.GetIndex(loData);
                if (loIndex.Contains("access_token"))
                {
                    loR.AccessToken = loIndex.GetValueString("access_token");
                    loIndex.Remove("access_token");
                }

                if (loIndex.Contains("RequestFieldList"))
                {
                    loR.RequestFieldList = loIndex["RequestFieldList"] as string[];
                    loIndex.Remove("RequestFieldList");
                }

                if (loIndex.Contains("ResponseFieldList"))
                {
                    loR.ResponseFieldList = loIndex["ResponseFieldList"] as string[];
                    loIndex.Remove("ResponseFieldList");
                }

                loItem = loIndex;
            }
            else
            {
                NameValueCollection loData = HttpUtility.ParseQueryString(this.Request.RequestUri.Query);
                if (!string.IsNullOrEmpty(lsContent) && lsMediaType == "application/x-www-form-urlencoded")
                {
                    loData = HttpUtility.ParseQueryString(lsContent);
                }

                foreach (string lsName in loData.AllKeys)
                {
                    if (lsName == "access_token")
                    {
                        loR.AccessToken = MaxConvertLibrary.ConvertToString(typeof(object), loData[lsName]);
                    }
                    else if (lsName == "RequestFieldList")
                    {
                        loR.RequestFieldList = MaxConvertLibrary.ConvertToString(typeof(object), loData[lsName]).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    }
                    else if (lsName == "ResponseFieldList")
                    {
                        loR.ResponseFieldList = MaxConvertLibrary.ConvertToString(typeof(object), loData[lsName]).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    }
                    else
                    {
                        string lsValue = loData[lsName];
                        if (!string.IsNullOrEmpty(lsValue))
                        {
                            lsValue = lsValue.Trim();
                        }

                        loItem.Add(lsName, lsValue);
                    }
                }
            }

            loR.Item = loItem;
            return loR;
        }

        protected MaxIndex GetIndex(dynamic loData)
        {
            MaxIndex loR = new MaxIndex();
            foreach (Newtonsoft.Json.Linq.JProperty loProperty in loData.Properties())
            {
                string lsName = loProperty.Name;
                if (loProperty.Value.Type == Newtonsoft.Json.Linq.JTokenType.Object)
                {
                    loR.Add(lsName, GetIndex(loProperty.Value));
                }
                else if (loProperty.Value.Type == Newtonsoft.Json.Linq.JTokenType.Array)
                {
                    if (loProperty.Value.HasValues)
                    {
                        List<object> loList = new List<object>();
                        foreach (var loPropertyValue in ((Newtonsoft.Json.Linq.JToken)loProperty.Value).Children())
                        {
                            if (loPropertyValue.Type == Newtonsoft.Json.Linq.JTokenType.String)
                            {
                                loList.Add(loPropertyValue);
                            }
                            else
                            {
                                loList.Add(GetIndex(loPropertyValue));
                            }
                        }

                        loR.Add(lsName, loList.ToArray());
                    }
                }
                else
                { 
                    if (loProperty.Value.Type == Newtonsoft.Json.Linq.JTokenType.String)
                    {
                        string lsValue = MaxConvertLibrary.ConvertToString(typeof(object), loProperty.Value);
                        loR.Add(lsName, lsValue.Trim());
                    }
                    else
                    {
                        loR.Add(lsName, loProperty.Value);
                    }                    
                }
            }

            return loR;
        }

        /// <summary>
        /// Initializes a response with a blank item
        /// </summary>
        /// <typeparam name="T">Response Item is anonymous</typeparam>
        /// <param name="loResponseItem">Anonymouse item to be included with response</param>
        /// <returns>Initialized response item</returns>
        protected MaxApiResponseViewModel GetResponse<T>(T loResponseItem)
        {
            MaxIndex loItem = new MaxIndex();
            if (null != loResponseItem)
            {
                PropertyInfo[] laProperty = loResponseItem.GetType().GetProperties();
                foreach (PropertyInfo loProperty in laProperty)
                {
                    string lsValue = MaxConvertLibrary.ConvertToString(typeof(object), loProperty.GetValue(loResponseItem));
                    loItem.Add(lsValue, string.Empty);
                }
            }

            return new MaxApiResponseViewModel(loItem);
        }

        /// <summary>
        /// Initializes a response with a blank item
        /// </summary>
        /// <param name="loRequest">Request used to create response</param>
        /// <returns>Initialized response item</returns>
        protected MaxApiResponseViewModel GetResponse(MaxApiRequestViewModel loRequest)
        {
            MaxIndex loItem = new MaxIndex();
            if (null != loRequest && null != loRequest.ResponseFieldList)
            {
                foreach (string lsField in loRequest.ResponseFieldList)
                {
                    loItem.Add(lsField, string.Empty);
                }               
            }

            return new MaxApiResponseViewModel(loItem);
        }

        protected MaxIndex GetRequestInfoIndex()
        {
            MaxIndex loR = new MaxIndex();
            // Web-hosting. Needs reference to System.Web.dll
            if (Request.Properties.ContainsKey(_sHttpContext))
            {
                HttpContextWrapper loContext = Request.Properties[_sHttpContext] as HttpContextWrapper;
                if (loContext != null)
                {
                    loR.Add("Address", loContext.Request.UserHostAddress);
                    loR.Add("Agent", loContext.Request.UserAgent);
                    loR.Add("HostName", loContext.Request.Url.DnsSafeHost);
                    if (loContext.User.Identity.IsAuthenticated)
                    {
                        loR.Add("User", loContext.User.Identity.Name);
                    }
                }
            }

            // Self-hosting. Needs reference to System.ServiceModel.dll. 
            if (Request.Properties.ContainsKey(_sRemoteEndpointMessage))
            {
                RemoteEndpointMessageProperty loRemoteEndpoint = Request.Properties[_sRemoteEndpointMessage] as RemoteEndpointMessageProperty;
                if (loRemoteEndpoint != null)
                {
                    loR.Add("Address", loRemoteEndpoint.Address);
                }
            }

            // Self-hosting using Owin. Needs reference to Microsoft.Owin.dll. 
            if (Request.Properties.ContainsKey(_sOwinContext))
            {
                OwinContext loOwinContext = Request.Properties[_sOwinContext] as OwinContext;
                if (loOwinContext != null)
                {
                    loR.Add("Address", loOwinContext.Request.RemoteIpAddress);
                    loR.Add("HostName", loOwinContext.Request.Uri.DnsSafeHost);
                    if (loOwinContext.Request.User.Identity.IsAuthenticated)
                    {
                        loR.Add("User", loOwinContext.Request.User.Identity.Name);
                    }
                }
            }

            return loR;
        }

        /// <summary>
        /// Maps an Api request to an entity
        /// </summary>
        /// <param name="loEntity">Entity to map to</param>
        /// <param name="loRequest">Request data to use to map</param>
        /// <returns></returns>
        protected virtual MaxBaseIdEntity MapRequest(MaxBaseIdEntity loEntity, MaxApiRequestViewModel loRequest)
        {
            if (null != loRequest.RequestFieldList && loRequest.ResponseFieldList.Length > 0)
            {
                PropertyInfo[] laProperty = loEntity.GetType().GetProperties(BindingFlags.Public);
                foreach (string lsField in loRequest.RequestFieldList)
                {
                    foreach (PropertyInfo loProperty in laProperty)
                    {
                        if (lsField.Equals(loProperty.Name, StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (loRequest.Item.Contains(lsField))
                            {
                                if (loProperty.PropertyType == typeof(double))
                                {
                                    loProperty.SetValue(loEntity, MaxConvertLibrary.ConvertToDouble(typeof(object), loRequest.Item[lsField]));
                                }
                                else if (loProperty.PropertyType == typeof(int))
                                {
                                    loProperty.SetValue(loEntity, MaxConvertLibrary.ConvertToInt(typeof(object), loRequest.Item[lsField]));
                                }
                                else if (loProperty.PropertyType == typeof(bool))
                                {
                                    loProperty.SetValue(loEntity, MaxConvertLibrary.ConvertToBoolean(typeof(object), loRequest.Item[lsField]));
                                }
                                else if (loProperty.PropertyType == typeof(string))
                                {
                                    loProperty.SetValue(loEntity, MaxConvertLibrary.ConvertToString(typeof(object), loRequest.Item[lsField]));
                                }
                                else
                                {
                                    loProperty.SetValue(loEntity, loRequest.Item[lsField]);
                                }
                            }
                        }
                    }
                }
            }

            return loEntity;
        }

        /// <summary>
        /// Maps an entity and request to a response
        /// </summary>
        /// <param name="loEntity">Entity to use for response</param>
        /// <param name="loRequest">Requet to use for response</param>
        /// <returns></returns>
        protected virtual MaxIndex MapResponse(MaxBaseIdEntity loEntity, MaxApiRequestViewModel loRequest)
        {
            return loEntity.MapIndex(loRequest.ResponseFieldList);
        }

        /// <summary>
        /// Checks the permission
        /// </summary>
        /// <param name="loRequest">Current request</param>
        /// <param name="loEntity">Entity to check</param>
        /// <param name="loId">Id to check</param>
        /// <param name="lnPermission">Permission to make sure the user has</param>
        /// <returns></returns>
        protected virtual bool HasPermission(MaxApiRequestViewModel loRequest, MaxEntity loEntity, Guid loId, int lnPermission)
        {
            bool lbR = loRequest.RoleList.Contains("Admin - App") || loRequest.RoleList.Contains("Admin");
            return lbR;
        }

        /// <summary>
        /// Process an api call for an entity
        /// </summary>
        /// <param name="loEntity">The entity to process</param>
        /// <returns></returns>
        protected virtual async Task<MaxApiResponseViewModel> Process(MaxBaseIdEntity loEntity)
        {
            MaxApiResponseViewModel loR = null;
            if (this.Request.Method != HttpMethod.Options)
            {
                MaxApiRequestViewModel loRequest = await this.GetRequest();
                if (!this.HasPermission(loRequest, loEntity, loRequest.Id, (int)MaxEnumGroup.PermissionGroup))
                {
                    loR.Message.Error = "User does not have permission for this item type.";
                    loR.Status = HttpStatusCode.Forbidden;
                }
                else
                {
                    if (Request.Method == HttpMethod.Post)
                    {
                        //// Update
                        if (!this.HasPermission(loRequest, loEntity, loRequest.Id, (int)MaxEnumGroup.PermissionUpdate))
                        {
                            loR.Message.Error = "User does not have permission to update item.";
                            loR.Status = HttpStatusCode.Forbidden;
                        }
                        else if (!loEntity.LoadByIdCache(loRequest.Id))
                        {
                            loR.Message.Error = "Item with the provided Id cannot be loaded.";
                        }
                        else
                        {
                            loEntity = this.MapRequest(loEntity, loRequest);
                            loEntity.Update();
                            loR.Message.Success = "Item Updated";
                        }
                    }
                    else if (Request.Method == HttpMethod.Put)
                    {
                        //// Insert
                        if (!this.HasPermission(loRequest, loEntity, loRequest.Id, (int)MaxEnumGroup.PermissionInsert))
                        {
                            loR.Message.Error = "User does not have permission to add item.";
                            loR.Status = HttpStatusCode.Forbidden;
                        }
                        else
                        {
                            loEntity = this.MapRequest(loEntity, loRequest);
                            if (loEntity.Insert())
                            {
                                loR.Message.Success = "Item Added";
                            }
                            else
                            {
                                loR.Message.Error = "Item was not added";
                            }
                        }
                    }

                    if (Request.Method == HttpMethod.Delete)
                    {
                        if (!this.HasPermission(loRequest, loEntity, loRequest.Id, (int)MaxEnumGroup.PermissionDelete))
                        {
                            loR.Message.Error = "User does not have permission to delete item.";
                            loR.Status = HttpStatusCode.Forbidden;
                        }
                        else if (!loEntity.LoadByIdCache(loRequest.Id))
                        {
                            loR.Message.Error = "Item with the provided Id cannot be loaded.";
                        }
                        else
                        {
                            if (loEntity.Delete())
                            {
                                loR.Message.Success = "Item Deleted";
                            }
                        }
                    }
                    else if (Guid.Empty != loRequest.Id || Guid.Empty != loEntity.Id)
                    {
                        if (!this.HasPermission(loRequest, loEntity, loRequest.Id, (int)MaxEnumGroup.PermissionSelect))
                        {
                            loR.Message.Error = "User does not have permission for this item.";
                            loR.Status = HttpStatusCode.Forbidden;
                        }
                        else if (loEntity.Id == Guid.Empty && !loEntity.LoadByIdCache(loRequest.Id))
                        {
                            loR.Message.Error = "Item with the provided Id cannot be loaded.";
                        }
                        else
                        {
                            loR.Item = this.MapResponse(loEntity, loRequest);
                            if (string.IsNullOrEmpty(loR.Message.Success))
                            {
                                loR.Message.Success = "Got Item";
                            }
                        }
                    }

                    if (Guid.Empty == loRequest.Id || this.Request.Method == HttpMethod.Delete || this.Request.Method == HttpMethod.Put)
                    {
                        //// Load list
                        if (!this.HasPermission(loRequest, loEntity, loRequest.Id, (int)MaxEnumGroup.PermissionSelect))
                        {
                            loR.Message.Error = "User does not have permission for this list of items.";
                            loR.Status = HttpStatusCode.Forbidden;
                        }
                        else
                        {
                            MaxEntityList loList = loEntity.LoadAllCache(loRequest.ResponseFieldList);
                            SortedList<string, MaxBaseIdEntity> loSortedList = new SortedList<string, MaxBaseIdEntity>();
                            for (int lnE = 0; lnE < loList.Count; lnE++)
                            {
                                MaxBaseIdEntity loListEntity = loList[lnE] as MaxBaseIdEntity;
                                if (loListEntity.IsActive || this.HasPermission(loRequest, loEntity, loRequest.Id, (int)MaxEnumGroup.PermissionSelectInactive))
                                {
                                    loSortedList.Add(loListEntity.GetDefaultSortString(), loListEntity);
                                }
                            }

                            foreach (string lsKey in loSortedList.Keys)
                            {
                                MaxIndex loItem = loSortedList[lsKey].MapIndex(loRequest.ResponseFieldList);
                                loR.ItemList.Add(loItem);
                            }
                        }
                    }
                }
            }

            return loR;
        }

        /// <summary>
        /// Gets the user based on username
        /// </summary>
        /// <param name="loRequest"></param>
        /// <param name="lsUserName"></param>
        /// <returns></returns>
        protected MembershipUser GetUser(MaxApiRequestViewModel loRequest, object loUserIdentifier)
        {
            MembershipUser loR = null;
            MembershipUser loUser = loRequest.User;
            if (null != loUser)
            {
                if (loUserIdentifier is string && loUser.UserName == loUserIdentifier as string)
                {
                    loR = loUser;
                }
                else if (null != loUserIdentifier && loUser.ProviderUserKey == loUserIdentifier)
                {
                    loR = loUser;
                }
                else if (null != loUserIdentifier)
                {
                    List<string> loRoleList = new List<string>(Roles.GetRolesForUser(loUser.UserName));
                    if (loRoleList.Contains("Admin - App") || loRoleList.Contains("Admin") || loRoleList.Contains("User Manager"))
                    {
                        loR = Membership.GetUser(loUserIdentifier);
                    }
                }
                else
                {
                    loR = loUser;
                }
            }

            return loR;
        }

        /// <summary>
        /// Gets the currently logged in user
        /// </summary>
        /// <param name="loRequest"></param>
        /// <returns></returns>
        protected MembershipUser GetUser(MaxApiRequestViewModel loRequest)
        {
            return this.GetUser(loRequest, null);
        }

        protected List<string> GetRoles(MembershipUser loUser)
        {
            List<string> loR = new List<string>();
            if (null != loUser && !string.IsNullOrEmpty(loUser.UserName))
            {
                loR.AddRange(Roles.GetRolesForUser(loUser.UserName));
            }

            return loR;
        }

        /// <summary>
        /// Gets the Api data as a string that has been sanitized in some way
        /// </summary>
        /// <param name="loIndex"></param>
        /// <param name="lsName"></param>
        /// <returns></returns>
        protected virtual string GetSanitizedValue(MaxIndex loIndex, string lsName)
        {
            string lsR = loIndex.GetValueString(lsName).Trim();
            return lsR;
        }
    }
}
