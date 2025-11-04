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
// <change date="3/30/2024" author="Brian A. Lakstins" description="Update for change to dependent class.  Updated to use DataKey.">
// <change date="6/28/2024" author="Brian A. Lakstins" description="Add generic permission checking.">
// <change date="7/2/2024" author="Brian A. Lakstins" description="Add method to get permission based on object and display name.">
// <change date="7/10/2024" author="Brian A. Lakstins" description="Add ability for any type of permission.">
// <change date="7/29/2024" author="Brian A. Lakstins" description="Seperate out single item and list handling.">
// <change date="7/30/2024" author="Brian A. Lakstins" description="Fix property name issue.">
// <change date="7/31/2024" author="Brian A. Lakstins" description="Fix not loading by DataKey.  Update property name mapping.  Fix including common value with list.">
// <change date="8/26/2024" author="Brian A. Lakstins" description="Updated request processing to have both original and mapped entities.  Removed unneeded methods.">
// <change date="8/29/2024" author="Brian A. Lakstins" description="Updated exception logging for processing api requests">
// <change date="9/16/2024" author="Brian A. Lakstins" description="Add a way to update attributeindex values">
// <change date="9/24/2024" author="Brian A. Lakstins" description="Update sorting and filtering">
// <change date="11/13/2024" author="Brian A. Lakstins" description="Add Entity as an arguement for getting the response">
// <change date="3/4/2025" author="Brian A. Lakstins" description="Handle null data key different from empty data key">
// <change date="4/14/2025" author="Brian A. Lakstins" description="Load through POST without also updating.">
// <change date="4/29/2025" author="Brian A. Lakstins" description="Consider and update successful if any updates on a list succeed.">
// <change date="6/17/2025" author="Brian A. Lakstins" description="Update logging.">
// <change date="7/10/2025" author="Brian A. Lakstins" description="Add returning of ItemListTotal with page. Return lists for change operations that include DataKey.">
// <change date="7/15/2025" author="Brian A. Lakstins" description="Send some properties to load list instead of having it determine them from request.">
// <change date="9/9/2025" author="Brian A. Lakstins" description="Don't require ResponsePropertyList for updates.">
// <change date="9/29/2025" author="Brian A. Lakstins" description="Allow for AttributeIndex to be readonly">
// <change date="10/17/2025" author="Brian A. Lakstins" description="Fix null error">
// <change date="11/4/2025" author="Brian A. Lakstins" description="Swap POST and PUT.  Add shorter variable names for request.">
// <change date="11/4/2025" author="Brian A. Lakstins" description="Add standard variable names for request">
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
        protected const string _sDateRangeNameText = "This Week,Last 7 days,Last week,Last 30 days,Last month,This month,Last Quarter,This Quarter,Custom,All";


        public MaxBaseApiController()
        {
            MaxLogLibrary.Log(new MaxLogEntryStructure(this.GetType(), "MaxBaseApiController", MaxEnumGroup.LogInfo, "Created"));
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
                    MaxLogLibrary.Log(new MaxLogEntryStructure(typeof(MaxBaseApiController), "MaxBaseApiController.StartProcess", MaxEnumGroup.LogError, "MaxBaseApiController: Error running Process for [{Uri}][{lsProcess}][{lsContent}]", loTask.Exception, loRequestUri.ToString(), lsProcess, lsContent));
                }
            }
            catch (Exception loE)
            {
                MaxLogLibrary.Log(new MaxLogEntryStructure(typeof(MaxBaseApiController), "MaxBaseApiController.StartProcess", MaxEnumGroup.LogError, "MaxBaseApiController.StartProcessEventHookLog: Error starting Process for [{Uri}][{lsProcess}][{lsContent}]", loE, loRequestUri.ToString(), lsProcess, lsContent));
            }
        }

        [HttpGet]
        [HttpOptions]
        [ActionName("permission")]
        public async Task<HttpResponseMessage> Permission()
        {
            HttpStatusCode loStatus = HttpStatusCode.Unauthorized;
            var loResponseItem = new
            {
                DataKey = "DataKey",
                Name = "Name",
                DisplayName = "DisplayName"
            };

            MaxApiResponseViewModel loR = GetResponse(loResponseItem);
            if (this.Request.Method != HttpMethod.Options)
            {
                MaxApiRequestViewModel loRequest = await this.GetRequest();
                if (null != loRequest.User)
                {
                    loStatus = HttpStatusCode.OK;
                    try
                    {
                        MaxIndex loPermissionList = this.GetPermissionList();
                        string[] laKey = loPermissionList.GetSortedKeyList();
                        foreach (string lsKey in laKey)
                        {
                            MaxIndex loPermission = loPermissionList[lsKey] as MaxIndex;
                            if (null != loPermission)
                            {
                                loR.ItemList.Add(loPermission);
                            }
                        }

                        loR.Message.Success = "Got permission list";
                    }
                    catch (Exception loE)
                    {
                        loR.Message.Error = "Exception getting permission list: " + loE.Message;
                        MaxLogLibrary.Log(new MaxLogEntryStructure(this.GetType(), "MaxBaseApi", MaxEnumGroup.LogError, "Exception getting permission list", loE));
                    }
                }
            }
            else
            {
                loStatus = HttpStatusCode.OK;
            }

            return this.GetResponseMessage(loR, loStatus);
        }

        [HttpGet]
        [HttpOptions]
        [ActionName("permissiontype")]
        public async Task<HttpResponseMessage> PermissionType()
        {
            HttpStatusCode loStatus = HttpStatusCode.Unauthorized;
            var loResponseItem = new
            {
                Name = "Name",
                Permission = "Permission"
            };

            MaxApiResponseViewModel loR = GetResponse(loResponseItem);
            if (this.Request.Method != HttpMethod.Options)
            {
                MaxApiRequestViewModel loRequest = await this.GetRequest();
                if (null != loRequest.User)
                {
                    loStatus = HttpStatusCode.OK;
                    try
                    {
                        int[] laPermssionType = new int[6] {
                            (int)MaxEnumGroup.PermissionGroup,
                            (int)MaxEnumGroup.PermissionSelect,
                            (int)MaxEnumGroup.PermissionInsert,
                            (int)MaxEnumGroup.PermissionUpdate,
                            (int)MaxEnumGroup.PermissionDelete,
                            (int)MaxEnumGroup.PermissionSelectInactive};

                        foreach (int lnPermissionType in laPermssionType)
                        {
                            string lsName = MaxEnumGroup.GetName(typeof(MaxEnumGroup), lnPermissionType);
                            MaxIndex loPermissionType = new MaxIndex();
                            loPermissionType.Add(loResponseItem.Name, lsName);
                            loPermissionType.Add(loResponseItem.Permission, lnPermissionType);
                            loR.ItemList.Add(loPermissionType);
                        }

                        loR.Message.Success = "Got permission type list";
                    }
                    catch (Exception loE)
                    {
                        loR.Message.Error = "Exception getting permission type list: " + loE.Message;
                        MaxLogLibrary.Log(new MaxLogEntryStructure(this.GetType(), "MaxBaseApi", MaxEnumGroup.LogError, "Exception getting permission type list", loE));
                    }
                }
            }
            else
            {
                loStatus = HttpStatusCode.OK;
            }

            return this.GetResponseMessage(loR, loStatus);
        }

        /// <summary>
        /// Gets the list of permissions related to this api
        ///            var loResponseItem = new
        ///            {
        ///                DataKey = "DataKey",
        ///                Name = "Name",
        ///                DisplayName = "DisplayName"
        ///            };
        /// </summary>
        /// <returns></returns>
        protected virtual MaxIndex GetPermissionList()
        {
            MaxIndex loR = new MaxIndex();
            return loR;
        }

        /// <summary>
        /// Checks the permission
        /// </summary>
        /// <param name="loRequest">Current request</param>
        /// <param name="loEntity">Entity to check</param>
        /// <param name="loId">Id to check</param>
        /// <param name="lnPermission">Permission to make sure the user has</param>
        /// <returns></returns>
        protected virtual bool HasPermission(MaxApiRequestViewModel loRequest, MaxEntity loEntity, int lnPermission)
        {
            bool lbR = loRequest.RoleList.Contains("Admin - App") || loRequest.RoleList.Contains("Admin");
            if (!lbR)
            {
                try
                {
                    if (null != loRequest.User && null != loRequest.RoleList && loRequest.RoleList.Count > 0)
                    {
                        Guid loPermissionId = MaxRoleRelationPermissionEntity.GetPermissionId(loEntity.GetType().ToString());
                        for (int lnR = 0; lnR < loRequest.RoleList.Count && !lbR; lnR++)
                        {
                            MaxEntityList loRoleRelationPermissionList = MaxRoleRelationPermissionEntity.Create().LoadAllByRoleNameCache(loRequest.RoleList[lnR]);
                            if (null != loRoleRelationPermissionList && loRoleRelationPermissionList.Count > 0)
                            {
                                for (int lnE = 0; lnE < loRoleRelationPermissionList.Count && !lbR; lnE++)
                                {
                                    MaxRoleRelationPermissionEntity loRoleRelationPermissionEntity = loRoleRelationPermissionList[lnE] as MaxRoleRelationPermissionEntity;
                                    if (loRoleRelationPermissionEntity.PermissionId == loPermissionId)
                                    {
                                        if ((loRoleRelationPermissionEntity.Permission & lnPermission) > 0)
                                        {
                                            lbR = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception loE)
                {
                    MaxLogLibrary.Log(new MaxLogEntryStructure(this.GetType(), "HasPermission", MaxEnumGroup.LogError, "Exception checking permission", loE));
                }

            }

            return lbR;
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
            if (loReturn is MaxApiResponseViewModel && null != ((MaxApiResponseViewModel)loReturn).ItemList && ((MaxApiResponseViewModel)loReturn).ItemList.Count > 0)
            {
                ((MaxApiResponseViewModel)loReturn).Page.Add("ItemListTotal", ((MaxApiResponseViewModel)loReturn).ItemList.Count);
            }

            return this.GetResponseMessage(loReturn, System.Net.HttpStatusCode.OK);
        }

        protected virtual HttpResponseMessage GetResponseMessage(object loReturn, System.Net.HttpStatusCode loStatus)
        {
            if (loReturn is MaxApiResponseViewModel && null != ((MaxApiResponseViewModel)loReturn).ItemList && ((MaxApiResponseViewModel)loReturn).ItemList.Count > 0)
            {
                ((MaxApiResponseViewModel)loReturn).Page.Add("ItemListTotal", ((MaxApiResponseViewModel)loReturn).ItemList.Count);
            }

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
            if (null != loReturn.ItemList && loReturn.ItemList.Count > 0)
            {
                loReturn.Page.Add("ItemListTotal", loReturn.ItemList.Count);
            }

            HttpResponseMessage loR = this.GetResponseMessage();
            loR.StatusCode = loReturn.Status;
            string lsContent = MaxConvertLibrary.SerializeObjectToString(typeof(object), loReturn);
            loR.Content = new StringContent(lsContent);
            loR.Content.Headers.Remove("Content-Type");
            loR.Content.Headers.Add("Content-Type", "text/json");
            MaxLogLibrary.ClearRecent();
            return loR;
        }

        protected virtual HttpResponseMessage GetResponseMessage(MaxApiResponseViewModel loReturn, MaxEntity loEntity)
        {
            if (null != loReturn.ItemList && loReturn.ItemList.Count > 0)
            {
                loReturn.Page.Add("ItemListTotal", loReturn.ItemList.Count);
            }

            HttpResponseMessage loR = this.GetResponseMessage();
            if (loReturn.Status == HttpStatusCode.OK && 
                this.Request.Method == HttpMethod.Get && 
                loReturn.Item.Contains("ContentName") &&
                loReturn.Item.Contains("MimeType") &&
                loEntity is MaxBaseIdFileEntity &&
                ((MaxBaseIdFileEntity)loEntity).Id != Guid.Empty &&
                null != ((MaxBaseIdFileEntity)loEntity).Content)
            {
                loR = new HttpResponseMessage(HttpStatusCode.OK);
                loR.Content = new StreamContent(((MaxBaseIdFileEntity)loEntity).Content);
                loR.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("inline");
                loR.Content.Headers.ContentDisposition.FileName = ((MaxBaseIdFileEntity)loEntity).ContentName;
                loR.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(((MaxBaseIdFileEntity)loEntity).MimeType);
            }
            else
            {
                loR.StatusCode = loReturn.Status;
                string lsContent = MaxConvertLibrary.SerializeObjectToString(typeof(object), loReturn);
                loR.Content = new StringContent(lsContent);
                loR.Content.Headers.Remove("Content-Type");
                loR.Content.Headers.Add("Content-Type", "text/json");
            }

            MaxLogLibrary.ClearRecent();
            return loR;
        }

        /// <summary>
        /// Gets the request in whatever format it is being supplied (QueryString, JSON, Form Data, Multipart Form Data)
        /// </summary>
        /// <returns>Request information</returns>
        protected virtual async Task<MaxApiRequestViewModel> GetRequest()
        {
            MaxApiRequestViewModel loR = new MaxApiRequestViewModel(this.Request);
            string lsContent = string.Empty;
            string lsMediaType = string.Empty;
            MaxIndex loItem = new MaxIndex();
            List<MaxIndex> loItemList = new List<MaxIndex>();
            try
            {
                NameValueCollection loQSData = HttpUtility.ParseQueryString(this.Request.RequestUri.Query);
                foreach (string lsName in loQSData.AllKeys)
                {
                    string lsValue = loQSData[lsName];
                    if (!string.IsNullOrEmpty(lsValue))
                    {
                        lsValue = lsValue.Trim();
                    }

                    loItem.Add(lsName, lsValue);
                }

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
                //// JSON content
                dynamic loData = Newtonsoft.Json.JsonConvert.DeserializeObject(lsContent);
                MaxIndex loJSData = this.GetIndex(loData);
                string[] laKey = loJSData.GetSortedKeyList();
                foreach (string lsKey in laKey)
                {
                    loItem.Add(lsKey, loJSData[lsKey]);
                }
            }
            else if (!string.IsNullOrEmpty(lsContent) && lsMediaType == "application/x-www-form-urlencoded")
            {
                //// Url encoded content
                NameValueCollection loData = HttpUtility.ParseQueryString(lsContent);
                foreach (string lsName in loData.AllKeys)
                {
                    string lsValue = loData[lsName];
                    if (!string.IsNullOrEmpty(lsValue))
                    {
                        lsValue = lsValue.Trim();
                    }

                    loItem.Add(lsName, lsValue);
                }
            }


            if (loItem.Contains("access_token"))
            {
                loR.AccessToken = loItem.GetValueString("access_token");
                loItem.Remove("access_token");
            }

            var loProperty = new
            {
                RequestPropertyList = "RequestPropertyList",
                dataFields = "dataFields",
                ResponsePropertyList = "ResponsePropertyList",
                fields = "fields",
                ResponseFilterList = "ResponseFilterList",
                filter = "filter",
                SearchText = "SearchText",
                search = "search"
            };

            //// RequestPropertyList can be an array of strings or a comma delimited string
            //// It could be in the RequestPropertyList or RQ property
            object[] laRequestPropertyList = loItem[loProperty.RequestPropertyList] as object[];
            if (null == laRequestPropertyList && loItem.Contains(loProperty.RequestPropertyList))
            {
                laRequestPropertyList = loItem.GetValueString(loProperty.RequestPropertyList).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }

            if (null == laRequestPropertyList)
            {
                laRequestPropertyList = loItem[loProperty.dataFields] as object[];
                if (null == laRequestPropertyList && loItem.Contains(loProperty.dataFields))
                {
                    laRequestPropertyList = loItem.GetValueString(loProperty.dataFields).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                }
            }

            //// Set the RequestPropertyList
            if (null != laRequestPropertyList)
            {
                loR.RequestPropertyList = new string[laRequestPropertyList.Length];
                for (int lnF = 0; lnF < laRequestPropertyList.Length; lnF++)
                {
                    loR.RequestPropertyList[lnF] = laRequestPropertyList[lnF].ToString();
                }

                if (loItem.Contains(loProperty.RequestPropertyList))
                {
                    loItem.Remove(loProperty.RequestPropertyList);
                }
                else if (loItem.Contains(loProperty.dataFields))
                {
                    loItem.Remove(loProperty.dataFields);
                }
            }

            //// ResponsePropertyList can be an array of strings or a comma delimited string
            //// It could be in the ResponsePropertyList or RS property
            object[] laResponsePropertyList = loItem[loProperty.ResponsePropertyList] as object[];
            if (null == laResponsePropertyList && loItem.Contains(loProperty.ResponsePropertyList))
            {
                laResponsePropertyList = loItem.GetValueString(loProperty.ResponsePropertyList).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }

            if (null == laResponsePropertyList)
            {
                laResponsePropertyList = loItem[loProperty.fields] as object[];
                if (null == laResponsePropertyList && loItem.Contains(loProperty.fields))
                {
                    laResponsePropertyList = loItem.GetValueString(loProperty.fields).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                }
            }

            /// Set the ResponsePropertyList
            if (null != laResponsePropertyList)
            {
                loR.ResponsePropertyList = new string[laResponsePropertyList.Length];
                for (int lnF = 0; lnF < laResponsePropertyList.Length; lnF++)
                {
                    loR.ResponsePropertyList[lnF] = laResponsePropertyList[lnF].ToString();
                }

                if (loItem.Contains(loProperty.ResponsePropertyList))
                {
                    loItem.Remove(loProperty.ResponsePropertyList);
                }
                else if (loItem.Contains(loProperty.fields))
                {
                    loItem.Remove(loProperty.fields);
                }
            }

            //// ResponseFilterList can be an array of strings or a tab delimited string
            //// It could be in the ResponseFilterList or RSF property
            object[] laResponseFilterList = loItem[loProperty.ResponseFilterList] as object[];
            if (null == laResponseFilterList && loItem.Contains(loProperty.ResponseFilterList))
            {
                laResponseFilterList = loItem.GetValueString(loProperty.ResponseFilterList).Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
            }

            if (null == laResponseFilterList)
            {
                laResponseFilterList = loItem[loProperty.filter] as object[];
                if (null == laResponseFilterList && loItem.Contains(loProperty.filter))
                {
                    laResponseFilterList = loItem.GetValueString(loProperty.filter).Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                }
            }

            if (null != laResponseFilterList)
            {
                loR.ResponseFilterList = new string[laResponseFilterList.Length];
                for (int lnF = 0; lnF < laResponseFilterList.Length; lnF++)
                {
                    loR.ResponseFilterList[lnF] = laResponseFilterList[lnF].ToString();
                }

                if (loItem.Contains(loProperty.ResponseFilterList))
                {
                    loItem.Remove(loProperty.ResponseFilterList);
                }
                else if (loItem.Contains(loProperty.filter))
                {
                    loItem.Remove(loProperty.filter);
                }
            }

            var loRequestPage = new
            {
                Page = "Page",
                page = "page",
                PageLength = "PageLength",
                limit = "limit",
                PropertySort = "PropertySort",
                sort = "sort",
                SearchText = "SearchText",
                q = "q"
            };

            loR.Page = MaxConvertLibrary.ConvertToInt(typeof(object), loItem.GetValueString(loRequestPage.Page));
            if (loR.Page == int.MinValue)
            {
                loR.Page = MaxConvertLibrary.ConvertToInt(typeof(object), loItem.GetValueString(loRequestPage.page));
            }

            if (loItem.Contains(loRequestPage.Page))
            {
                loItem.Remove(loRequestPage.Page);
            }
            else if (loItem.Contains(loRequestPage.page))
            {
                loItem.Remove(loRequestPage.page);
            }

            loR.PageLength = MaxConvertLibrary.ConvertToInt(typeof(object), loItem.GetValueString(loRequestPage.PageLength));
            if (loR.PageLength == int.MinValue)
            {
                loR.PageLength = MaxConvertLibrary.ConvertToInt(typeof(object), loItem.GetValueString(loRequestPage.limit));
            }

            if (loItem.Contains(loRequestPage.PageLength))
            {
                loItem.Remove(loRequestPage.PageLength);
            }
            else if (loItem.Contains(loRequestPage.limit))
            {
                loItem.Remove(loRequestPage.limit);
            }

            if (loR.Page == int.MinValue || loR.PageLength == int.MinValue)
            {
                loR.Page = 1;
                loR.PageLength = 1000;
            }

            loR.PropertySort = loItem.GetValueString(loRequestPage.PropertySort);
            if (string.IsNullOrEmpty(loR.PropertySort))
            {
                loR.PropertySort = loItem.GetValueString(loRequestPage.sort);
            }

            if (loItem.Contains(loRequestPage.PropertySort))
            {
                loItem.Remove(loRequestPage.PropertySort);
            }
            else if (loItem.Contains(loRequestPage.sort))
            {
                loItem.Remove(loRequestPage.sort);
            }

            loR.SearchText = loItem.GetValueString(loRequestPage.SearchText);
            if (string.IsNullOrEmpty(loR.SearchText))
            {
                loR.SearchText = loItem.GetValueString(loRequestPage.q);
            }

            if (loItem.Contains(loRequestPage.SearchText))
            {
                loItem.Remove(loRequestPage.SearchText);
            }
            else if (loItem.Contains(loRequestPage.q))
            {
                loItem.Remove(loRequestPage.q);
            }

            loR.Item = loItem;
            return loR;
        }

        protected virtual MaxIndex GetIndex(dynamic loData)
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
            if (null != loRequest && null != loRequest.ResponsePropertyList)
            {
                foreach (string lsField in loRequest.ResponsePropertyList)
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
        /// Process an api call for an entity
        /// </summary>
        /// <param name="loEntity">The entity to process</param>
        /// <returns></returns>
        protected virtual async Task<MaxApiResponseViewModel> Process(MaxEntity loEntity)
        {
            MaxApiResponseViewModel loR = new MaxApiResponseViewModel();
            if (this.Request.Method != HttpMethod.Options)
            {
                try
                {
                    MaxApiRequestViewModel loRequest = await this.GetRequest();
                    try
                    {
                        loR = this.ProcessRequest(loRequest, loEntity);
                    }
                    catch (Exception loE)
                    {
                        MaxLogEntryStructure loLogEntry = new MaxLogEntryStructure(this.GetType(), "Process", MaxEnumGroup.LogCritical, "Exception during processing request {Request}", loE, loRequest);
                        MaxLogLibrary.Log(loLogEntry);
                        loR.Message.Error = "Exception during processing request (" + loE.Message + ")";
                    }

                    if (!string.IsNullOrEmpty(loR.Message.Error))
                    {
                        string lsUserName = "Unknown";
                        if (loRequest.User != null)
                        {
                            lsUserName = loRequest.User.UserName + "<" + loRequest.User.Email + ">";
                        }

                        MaxLogLibrary.Log(new MaxLogEntryStructure(this.GetType(), "Process", MaxEnumGroup.LogError, "Error message for processing {Request} from user {UserName} for entity type {Type}.  This is the error: {Error}", loRequest, lsUserName, loEntity.GetType(), loR.Message.Error));
                    }
                }
                catch (Exception loE)
                {
                    MaxLogEntryStructure loLogEntry = new MaxLogEntryStructure(this.GetType(), "Process", MaxEnumGroup.LogCritical, "Exception during getting request", loE);                    
                    MaxLogLibrary.Log(loLogEntry);
                    loR.Message.Error = "Exception during getting request (" + loE.Message + ")";
                }
            }

            return loR;
        }

        protected virtual MaxApiResponseViewModel ProcessRequest(MaxApiRequestViewModel loRequest, MaxEntity loEntity)
        {
            MaxApiResponseViewModel loR = new MaxApiResponseViewModel();
            if (!this.HasPermission(loRequest, loEntity, (int)MaxEnumGroup.PermissionGroup))
            {
                loR.Message.Error = "User does not have permission for this item type.";
                loR.Status = HttpStatusCode.Forbidden;
                if (loRequest.User == null)
                {
                    loR.Message.Error = string.Empty;
                    loR.Message.Warning = "User needs to be logged in to access this item type.";
                    loR.Status = HttpStatusCode.Unauthorized;
                }
            }
            else if (null == loEntity)
            {
                loR.Message.Error = "Unable to process null entity";
            }
            else
            {
                if ((Request.Method == HttpMethod.Get || Request.Method == HttpMethod.Post || Request.Method == HttpMethod.Delete) && !this.HasPermission(loRequest, loEntity, (int)MaxEnumGroup.PermissionSelect))
                {
                    loR.Message.Error = "User does not have permission for this item.";
                    loR.Status = HttpStatusCode.Forbidden;
                }
                else if (Request.Method == HttpMethod.Put && !this.HasPermission(loRequest, loEntity, (int)MaxEnumGroup.PermissionInsert))
                {
                    loR.Message.Error = "User does not have permission to add item.";
                    loR.Status = HttpStatusCode.Forbidden;
                }
                else if (Request.Method == HttpMethod.Delete && !this.HasPermission(loRequest, loEntity, (int)MaxEnumGroup.PermissionDelete))
                {
                    loR.Message.Error = "User does not have permission to delete item.";
                    loR.Status = HttpStatusCode.Forbidden;
                }
                else
                {
                    string lsDataKey = loRequest.GetDataKey(-1);
                    if (!string.IsNullOrEmpty(lsDataKey) && !loEntity.LoadByDataKeyCache(lsDataKey, loRequest.ResponsePropertyList))
                    {
                        loR.Message.Error = "Entity could not be loaded by data key";
                    }
                    else
                    {
                        if (Request.Method == HttpMethod.Post && null != loRequest.RequestPropertyList && !this.HasPermission(loRequest, loEntity, (int)MaxEnumGroup.PermissionUpdate))
                        {
                            loR.Message.Error = "User does not have permission to update this item.";
                            loR.Status = HttpStatusCode.Forbidden;
                        }
                        else
                        {
                            MaxEntityList loMappedList = this.MapListRequest(loEntity, loRequest);
                            MaxEntity loMappedEntity = this.MapRequest(loEntity, loRequest);
                            if (Request.Method == HttpMethod.Post)
                            {
                                loR = this.ProcessPost(loRequest, loMappedEntity, loMappedList, loR);
                            }
                            else if (Request.Method == HttpMethod.Put)
                            {
                                loR = this.ProcessPut(loRequest, loEntity, loMappedEntity, loMappedList, loR);
                            }
                            else if (Request.Method == HttpMethod.Delete)
                            {
                                loR = this.ProcessDelete(loRequest, loMappedEntity, loMappedList, loR);
                            }

                            if (null != loMappedEntity && !string.IsNullOrEmpty(loMappedEntity.DataKey) && Request.Method != HttpMethod.Delete)
                            {
                                MaxIndex loEntityIndex = loMappedEntity.MapIndex(loRequest.ResponsePropertyList);
                                string[] laKey = loEntityIndex.GetSortedKeyList();
                                foreach (string lsKey in laKey)
                                {
                                    object loValue = loEntityIndex[lsKey];
                                    loR.Item[lsKey] = loValue;
                                }
                            }

                            if (null == lsDataKey || 
                                (this.Request.Method != HttpMethod.Get && loR.ItemList.Count == 0 && 
                                ((null != loRequest.ResponseFilterList && loRequest.ResponseFilterList.Length > 0) ||
                                loRequest.PageLength > 0)))
                            {
                                //// Load list
                                if (!this.HasPermission(loRequest, loEntity, (int)MaxEnumGroup.PermissionSelect))
                                {
                                    loR.Message.Error = "User does not have permission for this list of items.";
                                    loR.Status = HttpStatusCode.Forbidden;
                                }
                                else
                                {
                                    MaxApiResponseViewModel loLoadListResponse = this.ProcessLoadList(loRequest, loEntity);
                                    loR.Page = loLoadListResponse.Page;
                                    loR.ItemList = loLoadListResponse.ItemList;
                                }
                            }
                            else if (loR.ItemList.Count == 0)
                            {
                                loR.ItemList = null;
                            }
                        }
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
        protected virtual MaxEntityList MapListRequest(MaxEntity loEntity, MaxApiRequestViewModel loRequest)
        {
            MaxEntityList loR = new MaxEntityList(loEntity.GetType());
            if (null != loRequest.RequestPropertyList && loRequest.RequestPropertyList.Length > 0)
            {
                Dictionary<string, string> loRequestPropertyIndex = new Dictionary<string, string>();
                foreach (string lsRequestPropertyName in loRequest.RequestPropertyList)
                {
                    loRequestPropertyIndex.Add(lsRequestPropertyName.ToLowerInvariant(), lsRequestPropertyName);
                }


                MaxEntity loEntityCopy = loEntity.CreateNew();
                PropertyInfo[] laEntityProperty = loEntity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                if (null != loEntityCopy)
                {
                    bool lbValueFound = true;
                    int lnEntityNum = 0;
                    List<MaxIndex> loEntityIndexList = new List<MaxIndex>();
                    while (lbValueFound)
                    {
                        MaxIndex loEntityIndex = new MaxIndex();
                        lbValueFound = false;
                        foreach (PropertyInfo loEntityProperty in laEntityProperty)
                        {
                            string lsPropertyNameKey = loEntityProperty.Name.ToLowerInvariant();
                            if (loRequestPropertyIndex.ContainsKey(lsPropertyNameKey))
                            {
                                string lsRequestFieldName = loRequestPropertyIndex[lsPropertyNameKey];
                                bool lbHasValueCommon = null != loRequest.Item[lsRequestFieldName];

                                string lsValueCommon = loRequest.Item.GetValueString(lsRequestFieldName);
                                lsRequestFieldName += "[" + lnEntityNum.ToString() + "]";
                                bool lbHasValue = null != loRequest.Item[lsRequestFieldName];
                                string lsValue = loRequest.Item.GetValueString(lsRequestFieldName);
                                if (lbHasValue)
                                {
                                    lbValueFound = true;
                                }
                                else if (lbHasValueCommon)
                                {
                                    lsValue = lsValueCommon;
                                }

                                if (lbHasValueCommon || lbHasValue)
                                {
                                    loEntityIndex.Add(loEntityProperty.Name, lsValue);
                                }
                            }
                        }                        

                        if (lbValueFound)
                        {
                            loEntityIndexList.Add(loEntityIndex);
                        }
                     
                        lnEntityNum++;
                    }

                    string lsListKey = "MaxList";
                    if (loRequest.Item.Contains("MaxListKeyName"))
                    {
                        lsListKey = loRequest.Item.GetValueString("MaxListKeyName");
                    }

                    if (loRequest.Item.Contains(lsListKey))
                    {
                        try
                        {
                            MaxIndex[] laListIndex = MaxConvertLibrary.DeserializeObject(loRequest.Item.GetValueString(lsListKey), typeof(MaxIndex[])) as MaxIndex[];
                            if (null != laListIndex)
                            {
                                foreach (MaxIndex loListEntityIndex in laListIndex)
                                {
                                    MaxIndex loEntityIndex = new MaxIndex();
                                    lbValueFound = false;
                                    foreach (PropertyInfo loEntityProperty in laEntityProperty)
                                    {
                                        string lsPropertyNameKey = loEntityProperty.Name.ToLowerInvariant();
                                        if (loRequestPropertyIndex.ContainsKey(lsPropertyNameKey))
                                        {
                                            string lsRequestFieldName = loRequestPropertyIndex[lsPropertyNameKey];
                                            bool lbHasValueCommon = null != loRequest.Item[lsRequestFieldName];

                                            string lsValueCommon = loRequest.Item.GetValueString(lsRequestFieldName);
                                            bool lbHasValue = null != loListEntityIndex[lsRequestFieldName];
                                            string lsValue = loListEntityIndex.GetValueString(lsRequestFieldName);
                                            if (lbHasValue)
                                            {
                                                lbValueFound = true;
                                            }
                                            else if (lbHasValueCommon)
                                            {
                                                lsValue = lsValueCommon;
                                            }

                                            if (lbHasValueCommon || lbHasValue)
                                            {
                                                loEntityIndex.Add(loEntityProperty.Name, lsValue);
                                            }
                                        }
                                    }

                                    if (lbValueFound)
                                    {
                                        loEntityIndexList.Add(loEntityIndex);
                                    }
                                }
                            }
                        }
                        catch (Exception loE)
                        {
                            MaxLogLibrary.Log(new MaxLogEntryStructure(this.GetType(), "MapListRequest", MaxEnumGroup.LogError, "Exception deserilizing list property", loE));
                        }
                    }

                    MaxIndex[] laEntityIndex = loEntityIndexList.ToArray();
                    foreach (MaxIndex loEntityIndex in laEntityIndex)
                    {
                        loEntityCopy = loEntity.CreateNew();
                        bool lbMapProperties = true;
                        lbValueFound = false;
                        string lsDataKey = loEntityIndex.GetValueString(loEntity.GetPropertyName(() => loEntity.DataKey));
                        if (!string.IsNullOrEmpty(lsDataKey))
                        {
                            lbMapProperties = false;
                            if (loEntityCopy.LoadByDataKeyCache(lsDataKey))
                            {
                                lbMapProperties = true;
                                lbValueFound = true;
                            }
                        }

                        if (lbMapProperties)
                        {
                            foreach (PropertyInfo loProperty in laEntityProperty)
                            {
                                if (loEntityIndex.Contains(loProperty.Name))
                                {
                                    lbValueFound = true;
                                    string lsValue = loEntityIndex.GetValueString(loProperty.Name);
                                    loEntityCopy.SetValue(loProperty, lsValue);
                                }
                            }
                        }

                        if (lbValueFound)
                        {
                            loR.Add(loEntityCopy);
                        }
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
        protected virtual MaxEntity MapRequest(MaxEntity loEntity, MaxApiRequestViewModel loRequest)
        {
            MaxEntity loR = loEntity.Clone();
            if (null != loRequest.RequestPropertyList && loRequest.RequestPropertyList.Length > 0)
            {
                Dictionary<string, string> loEntityPropertyIndex = new Dictionary<string, string>();
                foreach (string lsProperty in loRequest.RequestPropertyList)
                {
                    string lsPropertyKey = lsProperty.ToLowerInvariant();
                    loEntityPropertyIndex.Add(lsPropertyKey, lsProperty);
                }

                PropertyInfo[] laProperty = loEntity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo loProperty in laProperty)
                {
                    string lsPropertyNameKey = loProperty.Name.ToLowerInvariant();
                    if (loEntityPropertyIndex.ContainsKey(lsPropertyNameKey))
                    {
                        string lsRequestFieldName = loEntityPropertyIndex[lsPropertyNameKey];
                        bool lbHasValueCommon = null != loRequest.Item[lsRequestFieldName];

                        string lsValueCommon = loRequest.Item.GetValueString(lsRequestFieldName);
                        bool lbHasValue = null != loRequest.Item[lsRequestFieldName];

                        string lsValue = loRequest.Item.GetValueString(lsRequestFieldName);
                        if (lbHasValueCommon && !lbHasValue)
                        {
                            lsValue = lsValueCommon;
                        }

                        if ((lbHasValueCommon || lbHasValue))
                        {
                            if (loProperty.CanWrite)
                            {
                                loR.SetValue(loProperty, lsValue);
                            }
                            else if (loR is MaxBaseEntity && loProperty.Name == "AttributeIndex")
                            {
                                MaxIndex loAttributeIndex = MaxConvertLibrary.DeserializeObject(lsValue, typeof(MaxIndex)) as MaxIndex;
                                if (loAttributeIndex != null)
                                {
                                    string[] laKey = loAttributeIndex.GetSortedKeyList();
                                    foreach (string lsKey in laKey)
                                    {
                                        if (!lsKey.StartsWith("_"))
                                        {
                                            ((MaxBaseEntity)loR).SetAttribute(lsKey, loAttributeIndex.GetValueString(lsKey));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return loR;
        }

        /// <summary>
        /// Create or replace a resource
        /// </summary>
        /// <param name="loRequest"></param>
        /// <param name="loOriginalEntity"></param>
        /// <param name="loMappedEntity"></param>
        /// <param name="loMappedEntityList"></param>
        /// <param name="loResponse"></param>
        /// <returns></returns>
        protected virtual MaxApiResponseViewModel ProcessPut(MaxApiRequestViewModel loRequest, MaxEntity loOriginalEntity, MaxEntity loMappedEntity, MaxEntityList loMappedEntityList, MaxApiResponseViewModel loResponse)
        {
            MaxApiResponseViewModel loR = loResponse;
            bool lbIsSuccess = false;
            if (loMappedEntityList.Count > 0)
            {
                lbIsSuccess = false;
                for (int lnE = 0; lnE < loMappedEntityList.Count; lnE++)
                {
                    if (!string.IsNullOrEmpty(loMappedEntityList[lnE].DataKey))
                    {
                        if (loMappedEntityList[lnE].Update())
                        {
                            lbIsSuccess = true;
                        }
                    }
                    else if (loMappedEntityList[lnE] is MaxBaseIdEntity)
                    {
                        MaxBaseIdEntity loBaseIdEntity = loMappedEntityList[lnE] as MaxBaseIdEntity;
                        if (Guid.Empty == loBaseIdEntity.Id)
                        {
                            if (loBaseIdEntity.Insert())
                            {
                                lbIsSuccess = true;
                            }
                        }
                        else
                        {
                            if (loBaseIdEntity.Update())
                            {
                                lbIsSuccess = true;
                            }
                        }
                    }
                    else if (string.IsNullOrEmpty(loMappedEntityList[lnE].DataKey))
                    {
                        if (loMappedEntityList[lnE].Insert())
                        {
                            lbIsSuccess = true;
                        }
                    }

                    loR.ItemList.Add(loMappedEntityList[lnE].MapIndex(loRequest.ResponsePropertyList));
                }
            }
            else
            {
                lbIsSuccess = false;
                if (null != loRequest.RequestPropertyList)
                {
                    if (!string.IsNullOrEmpty(loMappedEntity.DataKey))
                    {
                        lbIsSuccess = loMappedEntity.Update();
                    }
                    else if (loMappedEntity is MaxBaseIdEntity)
                    {
                        MaxBaseIdEntity loBaseIdEntity = loMappedEntity as MaxBaseIdEntity;
                        if (Guid.Empty == loBaseIdEntity.Id)
                        {
                            lbIsSuccess = loBaseIdEntity.Insert();
                        }
                        else
                        {
                            lbIsSuccess = loBaseIdEntity.Update();
                        }
                    }
                    else if (string.IsNullOrEmpty(loMappedEntity.DataKey))
                    {
                        lbIsSuccess = loMappedEntity.Insert();
                    }
                }
            }

            if (lbIsSuccess)
            {
                loR.Message.Success = "Item Updated";
                if (loMappedEntityList.Count > 0)
                {
                    loR.Message.Success = "Items Updated";
                }
            }
            else if (null == loRequest.RequestPropertyList || (null == loRequest.ResponsePropertyList || loRequest.ResponsePropertyList.Length == 0))
            {
                loR.Message.Success = "Item Loaded";
            }
            else
            {
                loR.Message.Error = "There was a problem updating";
            }

            return loR;
        }

        /// <summary>
        /// Create a new resource
        /// </summary>
        /// <param name="loRequest"></param>
        /// <param name="loMappedEntity"></param>
        /// <param name="loMappedList"></param>
        /// <param name="loResponse"></param>
        /// <returns></returns>
        protected virtual MaxApiResponseViewModel ProcessPost(MaxApiRequestViewModel loRequest, MaxEntity loMappedEntity, MaxEntityList loMappedList, MaxApiResponseViewModel loResponse)
        {
            MaxApiResponseViewModel loR = loResponse;
            bool lbIsSuccess = false;
            if (loMappedList.Count > 0)
            {
                lbIsSuccess = true;
                for (int lnE = 0; lnE < loMappedList.Count; lnE++)
                {
                    if (!loMappedList[lnE].Insert())
                    {
                        lbIsSuccess = false;
                    }
                    else
                    {
                        loR.ItemList.Add(loMappedList[lnE].MapIndex(loRequest.ResponsePropertyList));
                    }
                }
            }
            else if (null != loMappedEntity)
            {
                lbIsSuccess = loMappedEntity.Insert();
                if (lbIsSuccess)
                {
                    loR.Item = loMappedEntity.MapIndex(loRequest.ResponsePropertyList);
                }
            }

            if (lbIsSuccess)
            {
                loR.Message.Success = "Item Added";
                if (loMappedList.Count > 0)
                {
                    loR.Message.Success = "Items Added";
                }
            }
            else
            {
                loR.Message.Error = "There was a problem adding";
            }            

            return loR;
        }

        protected virtual MaxApiResponseViewModel ProcessDelete(MaxApiRequestViewModel loRequest, MaxEntity loMappedEntity, MaxEntityList loMappedEntityList, MaxApiResponseViewModel loResponse)
        {
            MaxApiResponseViewModel loR = loResponse;
            //// Delete
            bool lbIsDeleted = false;
            if (loMappedEntityList.Count > 0)
            {
                lbIsDeleted = true;
                for (int lnE = 0; lnE < loMappedEntityList.Count; lnE++)
                {
                    if (!loMappedEntityList[lnE].Delete())
                    {
                        lbIsDeleted = false;
                    }
                }
            }
            else if (null != loMappedEntity)
            {
                lbIsDeleted = loMappedEntity.Delete();
            }

            if (lbIsDeleted)
            {
                loR.Message.Success = "Item Deleted";
                if (loMappedEntityList.Count > 0)
                {
                   loR.Message.Success = "Items Deleted";
                }
            }
            else
            {
                loR.Message.Error = "Item with the provided Data Key cannot be deleted.";
                if (loMappedEntityList.Count > 0)
                {
                    loR.Message.Success = "At least one of the requested items cannot be deleted";
                }
            }

            return loR;
        }

        protected virtual MaxIndex GetFilter(MaxApiRequestViewModel loRequest, MaxEntity loEntity)
        {
            MaxIndex loR = new MaxIndex();
            if (loRequest.ResponseFilterList != null && loRequest.ResponseFilterList.Length > 0)
            {
                for (int lnF = 0; lnF < loRequest.ResponseFilterList.Length; lnF++)
                {
                    string lsFilter = loRequest.ResponseFilterList[lnF];
                    NameValueCollection loQuery = HttpUtility.ParseQueryString(lsFilter);
                    foreach (string lsName in loQuery.Keys)
                    {
                        string[] laValue = loQuery.GetValues(lsName);
                        foreach (string lsValue in laValue)
                        {
                            if (!string.IsNullOrEmpty(lsValue))
                            {
                                if (loR.Count > 0)
                                {
                                    MaxIndex loFilterPartPrevious = loR[loR.Count - 1] as MaxIndex;
                                    if (!loFilterPartPrevious.Contains("Condition"))
                                    {
                                        loFilterPartPrevious.Add("EndGroup", 1);
                                    }
                                }

                                if (lsValue.Contains("\t"))
                                {
                                    string[] laPartValue = lsValue.Split(new char[] { '\t' });
                                    for (int lnPV = 0; lnPV < laPartValue.Length; lnPV++)
                                    {
                                        MaxIndex loFilterPart = new MaxIndex();
                                        loFilterPart.Add("Name", lsName);
                                        loFilterPart.Add("Operator", "=");
                                        loFilterPart.Add("Value", laPartValue[lnPV]);
                                        loFilterPart.Add("Condition", "OR");
                                        if (lnPV == 0)
                                        {
                                            loFilterPart.Add("StartGroup", 1);
                                        }
                                        else if (lnPV == laPartValue.Length - 1)
                                        {
                                            if (lsName == loQuery.Keys[loQuery.Keys.Count - 1])
                                            {
                                                loFilterPart.Add("Condition", "");
                                            }
                                            else
                                            {
                                                loFilterPart.Add("Condition", "AND");
                                                loFilterPart.Add("EndGroup", 1);
                                            }
                                        }

                                        loR.Add(loFilterPart);
                                    }
                                }
                                else
                                {
                                    MaxIndex loFilterPart = new MaxIndex();
                                    loFilterPart.Add("StartGroup", 1);
                                    loFilterPart.Add("Name", lsName);
                                    loFilterPart.Add("Operator", "=");
                                    loFilterPart.Add("Value", lsValue);
                                    loR.Add(loFilterPart);
                                }
                            }
                        }
                    }

                    for (int lnFP = 0; lnFP < loR.Count; lnFP++)
                    {
                        MaxIndex loFilterPart = loR[lnFP] as MaxIndex;
                        if (lnFP == loR.Count - 1 && !loFilterPart.Contains("EndGroup"))
                        {
                            loFilterPart.Add("EndGroup", 1);
                        }
                        else if (!loFilterPart.Contains("Condition"))
                        {
                            loFilterPart.Add("Condition", "AND");
                        }
                    }
                }
            }

            return loR;
        }

        protected virtual MaxApiResponseViewModel ProcessLoadList(MaxApiRequestViewModel loRequest, MaxEntity loEntity)
        {
            MaxApiResponseViewModel loR = new MaxApiResponseViewModel();
            var loResponsePage = new
            {
                Total = "Total",
                ItemList = "ItemList"
            };

            MaxIndex loFilter = this.GetFilter(loRequest, loEntity);
            MaxEntity loEntityCopy = loEntity.CreateNew();
            if (null != loEntityCopy)
            {
                string[] laPropertyNameList = loRequest.ResponsePropertyList;
                if ((null == loRequest.ResponsePropertyList || loRequest.ResponsePropertyList.Length == 0) &&
                    null != loRequest.RequestPropertyList && loRequest.RequestPropertyList.Length > 0)
                {
                    laPropertyNameList = loRequest.RequestPropertyList;
                }

                if ((!string.IsNullOrEmpty(loRequest.PropertySort) && loRequest.Page > 0 && loRequest.PageLength > 0) || loFilter.Count > 0)
                {
                    MaxEntityList loList = loEntityCopy.LoadAllByPageFilter(loRequest.Page, loRequest.PageLength, loRequest.PropertySort, loFilter, laPropertyNameList);
                    loR.Page.Add(loResponsePage.Total, loList.Total);
                    List<string> loDataNameList = new List<string>(loEntity.GetData().DataModel.DataNameList);
                    if (!loDataNameList.Contains(loRequest.PropertySort))
                    {
                        //// TODO: Take into account multiple properties separated by commas and "asc" and "desc" suffixes
                        PropertyInfo loProperty = loEntityCopy.GetType().GetProperty(loRequest.PropertySort);
                        if (null != loProperty && loList.Total > loRequest.PageLength && (loRequest.Page > 1 || loList.Count > loRequest.PageLength))
                        {
                            SortedList<string, MaxEntity> loSortedList = new SortedList<string, MaxEntity>();
                            for (int lnE = 0; lnE < loList.Count; lnE++)
                            {
                                MaxEntity loListEntity = loList[lnE];
                                string lsValue = MaxConvertLibrary.ConvertToSortString(typeof(object), loProperty.GetValue(loListEntity));
                                loSortedList.Add(lsValue + '-' + loListEntity.DataKey, loListEntity);
                            }

                            loList = new MaxEntityList(loEntityCopy.GetType());
                            for (int lnL = 0; lnL < loSortedList.Values.Count; lnL++)
                            {
                                if (loRequest.PageLength <= 0 || ((loRequest.Page - 1) * loRequest.PageLength <= lnL && lnL < loRequest.Page * loRequest.PageLength))
                                {
                                    loList.Add(loSortedList.Values[lnL]);
                                }
                            }
                        }
                    }

                    for (int lnE = 0; lnE < loList.Count; lnE++)
                    {
                        MaxEntity loListEntity = loList[lnE];
                        MaxIndex loItem = loListEntity.MapIndex(laPropertyNameList);
                        loR.ItemList.Add(loItem);
                    }
                }
                else
                {
                    MaxEntityList loList = loEntityCopy.LoadAllCache(laPropertyNameList);
                    loR.Page.Add(loResponsePage.Total, loList.Total);
                    SortedList<string, MaxEntity> loSortedList = new SortedList<string, MaxEntity>();
                    for (int lnE = 0; lnE < loList.Count; lnE++)
                    {
                        MaxEntity loListEntity = loList[lnE];
                        if (loListEntity is MaxBaseIdEntity)
                        {
                            if (((MaxBaseIdEntity)loListEntity).IsActive || this.HasPermission(loRequest, loEntity, (int)MaxEnumGroup.PermissionSelectInactive))
                            {
                                loSortedList.Add(loListEntity.GetDefaultSortString(), loListEntity);
                            }
                        }
                        else
                        {
                            loSortedList.Add(loListEntity.GetDefaultSortString(), loListEntity);
                        }
                    }

                    foreach (string lsKey in loSortedList.Keys)
                    {
                        MaxIndex loItem = loSortedList[lsKey].MapIndex(laPropertyNameList);
                        loR.ItemList.Add(loItem);
                    }                    
                }
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

        protected static MaxIndex GetDateFilter(int lnDateRangeIndex, DateTime ldStartDate, DateTime ldEndDate)
        {
            MaxIndex loR = new MaxIndex();
            string[] laDateRangeName = _sDateRangeNameText.Split(new char[] { ',' });
            if (lnDateRangeIndex >= 0 && lnDateRangeIndex < laDateRangeName.Length)
            {
                loR.Add("Name", laDateRangeName[lnDateRangeIndex]);
                loR.Add("Index", lnDateRangeIndex);
                // "This Week,Last 7 days,Last week,Last 30 days,Last month,This month,Last Quarter,This Quarter,Custom,All"
                if (laDateRangeName[lnDateRangeIndex] == "All")
                {
                    ldStartDate = DateTime.MinValue;
                    ldEndDate = DateTime.MaxValue;
                }
                else if (laDateRangeName[lnDateRangeIndex] == "This Week")
                {
                    // This week
                    ldStartDate = DateTime.UtcNow.Date.AddDays(-1 * (int)DateTime.UtcNow.Date.DayOfWeek);
                    ldEndDate = ldStartDate.AddDays(7);
                }
                else if (laDateRangeName[lnDateRangeIndex] == "Last 7 days")
                {
                    // Last 7 days
                    ldEndDate = DateTime.UtcNow.Date;
                    ldStartDate = ldEndDate.AddDays(-7);
                }
                else if (laDateRangeName[lnDateRangeIndex] == "Last week")
                {
                    // Last week
                    ldStartDate = DateTime.UtcNow.Date.AddDays(-1 * (int)DateTime.UtcNow.Date.DayOfWeek).AddDays(-7);
                    ldEndDate = ldStartDate.AddDays(7);
                }
                else if (laDateRangeName[lnDateRangeIndex] == "Last 30 days")
                {
                    // Last 30 days
                    ldEndDate = DateTime.UtcNow.Date;
                    ldStartDate = ldEndDate.AddDays(-30);
                }
                else if (laDateRangeName[lnDateRangeIndex] == "Last month")
                {
                    // Last month
                    ldStartDate = DateTime.UtcNow.Date.AddMonths(-1);
                    ldStartDate = new DateTime(ldStartDate.Year, ldStartDate.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                    ldEndDate = ldStartDate.AddMonths(1);
                }
                else if (laDateRangeName[lnDateRangeIndex] == "This month")
                {
                    // This month
                    ldStartDate = new DateTime(DateTime.UtcNow.Date.Year, DateTime.UtcNow.Date.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                    ldEndDate = ldStartDate.AddMonths(1);
                }
                else if (laDateRangeName[lnDateRangeIndex] == "Last Quarter")
                {
                    // Last Quarter
                    ldStartDate = new DateTime(DateTime.UtcNow.Date.Year, DateTime.UtcNow.Date.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                    int lnCount = 0;
                    while ((ldStartDate.Month != 1 && ldStartDate.Month != 4 && ldStartDate.Month != 7 && ldStartDate.Month != 10) || lnCount == 0)
                    {
                        if (ldStartDate.Month == 1 || ldStartDate.Month == 4 || ldStartDate.Month == 7 || ldStartDate.Month == 10)
                        {
                            lnCount++;
                        }

                        ldStartDate = ldStartDate.AddMonths(-1);
                    }

                    ldEndDate = ldStartDate.AddMonths(3);
                }
                else if (laDateRangeName[lnDateRangeIndex] == "This Quarter")
                {
                    // This Quarter
                    ldStartDate = new DateTime(DateTime.UtcNow.Date.Year, DateTime.UtcNow.Date.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                    while (ldStartDate.Month != 1 && ldStartDate.Month != 4 && ldStartDate.Month != 7 && ldStartDate.Month != 10)
                    {
                        ldStartDate = ldStartDate.AddMonths(-1);
                    }

                    ldEndDate = ldStartDate.AddMonths(3);
                }
            }
            else if (ldEndDate == DateTime.MinValue)
            {
                ldStartDate = DateTime.Now.Date;
                ldEndDate = DateTime.MaxValue;
            }

            loR.Add("StartDate", ldStartDate);
            loR.Add("EndDate", ldEndDate);
            return loR;
        }

        protected static MaxIndex GetDateFilter(MaxIndex loRequestIndex)
        {
            MaxIndex loR = new MaxIndex();
            var loRequestItem = new
            {
                StartDate = "StartDate",
                EndDate = "EndDate",
                DateRangeIndex = "DateRangeIndex"
            };

            DateTime ldStartDate = MaxConvertLibrary.ConvertToDateTime(typeof(object), loRequestIndex.GetValueString(loRequestItem.StartDate));
            DateTime ldEndDate = MaxConvertLibrary.ConvertToDateTime(typeof(object), loRequestIndex.GetValueString(loRequestItem.EndDate));
            int lnDateRangeIndex = MaxConvertLibrary.ConvertToInt(typeof(object), loRequestIndex.GetValueString(loRequestItem.DateRangeIndex));
            loR = GetDateFilter(lnDateRangeIndex, ldStartDate, ldEndDate);
            return loR;
        }
    }
}
