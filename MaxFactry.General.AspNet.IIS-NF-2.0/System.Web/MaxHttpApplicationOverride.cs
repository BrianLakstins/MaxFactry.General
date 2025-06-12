// <copyright file="MaxHttpApplicationOverride.cs" company="Lakstins Family, LLC">
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
// <change date="6/22/2015" author="Brian A. Lakstins" description="Initial Release">
// <change date="7/29/2015" author="Brian A. Lakstins" description="Add sending emails at startup and shutdown.  Add writing response when error.">
// <change date="2/13/2016" author="Brian A. Lakstins" description="Update how version is determined.">
// <change date="7/19/2016" author="Brian A. Lakstins" description="Add user to custom parameters for caching.">
// <change date="7/19/2016" author="Brian A. Lakstins" description="Added reCAPTCHA as client tool.">
// <change date="7/22/2016" author="Brian A. Lakstins" description="Updates for changes to MaxConfigurationLibrary.">
// <change date="10/28/2016" author="Brian A. Lakstins" description="Updated to show blank page when an error occurs that does not have a description.">
// <change date="8/16/2018" author="Brian A. Lakstins" description="Updated start and stop notifications.">
// <change date="10/1/2018" author="Brian A. Lakstins" description="Add current URL to arguments to get storage key.">
// <change date="8/7/2019" author="Brian A. Lakstins" description="Updated error handling.">
// <change date="3/9/2020" author="Brian A. Lakstins" description="Updated error logging.">
// <change date="5/26/2020" author="Brian A. Lakstins" description="Updated error logging.">
// <change date="5/27/2020" author="Brian A. Lakstins" description="Updated error logging. Add checking to make sure client is still connected.">
// <change date="6/5/2020" author="Brian A. Lakstins" description="Add testing confiuration.">
// <change date="9/4/2020" author="Brian A. Lakstins" description="Remove client tool registration.">
// <change date="1/19/2021" author="Brian A. Lakstins" description="Update handling of site errors when HttpContext.Current is null.">
// <change date="7/20/2023" author="Brian A. Lakstins" description="Use constant instead of string for configuration name">
// <change date="7/25/2023" author="Brian A. Lakstins" description="Remove GetConfig.  Moved to MaxAppLibrary.">
// <change date="3/30/2024" author="Brian A. Lakstins" description="Update for change to dependent class.">
// <change date="6/11/2025" author="Brian A. Lakstins" description="Update for ApplicationKey">
// </changelog>
#endregion

namespace System.Web
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Reflection;
    using System.Web.Hosting;
    using MaxFactry.App.Base.AspNet.IIS;
    using MaxFactry.Core;
    using MaxFactry.General.BusinessLayer;
    using MaxFactry.General.AspNet.IIS;
    using MaxFactry.Base.DataLayer.Provider;

    /// <summary>
    /// MaxFactry implementation of HttpApplication
    /// </summary>
    public class MaxHttpApplicationOverride : System.Web.HttpApplication
    {
        private int _nErrorHttpContextCount = 0;

        protected string Name
        {
            get
            {
                string lsR = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeApplication, "MaxAssemblyProduct").ToString();
                string lsName = ConfigurationManager.AppSettings["MaxApplicationName"];
                if (!string.IsNullOrEmpty(lsName))
                {
                    lsR = lsName;
                }

                return lsR;
            }
        }

        public MaxHttpApplicationOverride()
        {
            MaxConfigurationLibrary.ExecutingType = this.GetType().BaseType;
        }

        /// <summary>
        /// Runs once for each application instance created.  Anything that updates the
        /// Application instance (like adding event handlers) should be included in Init().
        /// </summary>
        public override void Init()
        {
            base.Init();
            this.BeginRequest += (new EventHandler(MaxApplicationEventLibrary.ApplicationBeginRequest));
            this.AuthenticateRequest += (new EventHandler(MaxApplicationEventLibrary.ApplicationAuthenticateRequest));
            this.PostAuthenticateRequest += (new EventHandler(MaxApplicationEventLibrary.ApplicationPostAuthenticateRequest));
            this.AuthorizeRequest += (new EventHandler(MaxApplicationEventLibrary.ApplicationAuthorizeRequest));
            this.PostAuthorizeRequest += (new EventHandler(MaxApplicationEventLibrary.ApplicationPostAuthorizeRequest));
            this.ResolveRequestCache += (new EventHandler(MaxApplicationEventLibrary.ApplicationResolveRequestCache));
            this.PostResolveRequestCache += (new EventHandler(MaxApplicationEventLibrary.ApplicationPostResolveRequestCache));
            this.PostMapRequestHandler += (new EventHandler(MaxApplicationEventLibrary.ApplicationMapRequestHandler));
            this.PostMapRequestHandler += (new EventHandler(MaxApplicationEventLibrary.ApplicationPostMapRequestHandler));
            this.AcquireRequestState += (new EventHandler(MaxApplicationEventLibrary.ApplicationAcquireRequestState));
            this.PostAcquireRequestState += (new EventHandler(MaxApplicationEventLibrary.ApplicationPostAcquireRequestState));
            this.PreRequestHandlerExecute += (new EventHandler(MaxApplicationEventLibrary.ApplicationPreRequestHandlerExecute));
            this.PostRequestHandlerExecute += (new EventHandler(MaxApplicationEventLibrary.ApplicationPostRequestHandlerExecute));
            this.ReleaseRequestState += (new EventHandler(MaxApplicationEventLibrary.ApplicationReleaseRequestState));
            this.PostReleaseRequestState += (new EventHandler(MaxApplicationEventLibrary.ApplicationPostReleaseRequestState));
            this.UpdateRequestCache += (new EventHandler(MaxApplicationEventLibrary.ApplicationUpdateRequestCache));
            this.PostUpdateRequestCache += (new EventHandler(MaxApplicationEventLibrary.ApplicationPostUpdateRequestCache));
            this.LogRequest += (new EventHandler(MaxApplicationEventLibrary.ApplicationLogRequest));
            this.PostLogRequest += (new EventHandler(MaxApplicationEventLibrary.ApplicationPostLogRequest));
        }

        /// <summary>
        /// Runs once when the application is started
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void Application_Start_Handler(object sender, EventArgs e, MaxIndex loConfig)
        {
            MaxFactry.General.AspNet.IIS.MaxAppLibrary.Initialize(loConfig);
            this.FixAppDomainRestartWhenTouchingFiles();
            string lsVersion = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeApplication, "MaxAssemblyFileVersion").ToString();
            MaxFactry.Core.MaxLogLibrary.Log(new MaxLogEntryStructure(this.GetType(), "Application_Start_Handler", MaxFactry.Core.MaxEnumGroup.LogInfo, "Starting Application {Name} for instance {InstanceId} as {Version} in environment {Environment}", this.Name, MaxAppLibrary.GetApplicationRunId().ToString(), lsVersion, MaxFactry.Core.MaxLogLibrary.GetEnvironmentInformation()));
        }

        protected virtual void Session_Start_Handler(object sender, EventArgs e)
        {
            //// Access session to prevent error creating session - https://stackoverflow.com/questions/904952/whats-causing-session-state-has-created-a-session-id-but-cannot-save-it-becau
            string lsSessionId = Session.SessionID; 
            MaxFactry.Core.MaxLogLibrary.Log(new MaxLogEntryStructure("MaxHttpApplicationOverride", MaxFactry.Core.MaxEnumGroup.LogDebug, "Session_Start_Base {lsSessionId}", lsSessionId));
        }

        protected virtual void Session_End_Handler(object sender, EventArgs e)
        {
            string lsSessionId = Session.SessionID;
            MaxFactry.Core.MaxLogLibrary.Log(new MaxLogEntryStructure("MaxHttpApplicationOverride", MaxFactry.Core.MaxEnumGroup.LogDebug, "Session_End_Base {lsSessionId}", lsSessionId));
        }

        protected virtual void Application_End_Handler(object sender, EventArgs e)
        {
            string lsContent = "START\r\nUTC Time: " + MaxAppLibrary.GetApplicationStartDateTime().ToString() + "\r\n";
            MaxFactry.Core.MaxLogLibrary.Log(new MaxLogEntryStructure("Application_End_Handler", MaxFactry.Core.MaxEnumGroup.LogInfo, "Start Application End Handler"));
            try
            {
                lsContent += "STOP\r\nUTC Time: " + DateTime.UtcNow.ToString() + "\r\n";
                lsContent += "Application Instance [" + MaxAppLibrary.GetApplicationRunId().ToString() + "] end after " + MaxAppLibrary.GetTimeSinceApplicationStart().ToString() + " milliseconds\r\n";
                HttpRuntime runtime = (HttpRuntime)typeof(System.Web.HttpRuntime).InvokeMember("_theRuntime",
                                                                                        BindingFlags.NonPublic
                                                                                        | BindingFlags.Static
                                                                                        | BindingFlags.GetField,
                                                                                        null,
                                                                                        null,
                                                                                        null);

                if (runtime != null)
                {
                    string shutDownMessage = (string)runtime.GetType().InvokeMember("_shutDownMessage",
                                                                                     BindingFlags.NonPublic
                                                                                     | BindingFlags.Instance
                                                                                     | BindingFlags.GetField,
                                                                                     null,
                                                                                     runtime,
                                                                                     null);

                    string shutDownStack = (string)runtime.GetType().InvokeMember("_shutDownStack",
                                                                                   BindingFlags.NonPublic
                                                                                   | BindingFlags.Instance
                                                                                   | BindingFlags.GetField,
                                                                                   null,
                                                                                   runtime,
                                                                                   null);

                    MaxFactry.Core.MaxLogLibrary.Log(MaxFactry.Core.MaxEnumGroup.LogInfo, "Application_End message=[" + shutDownMessage + "]", "MaxHttpApplicationOverride");
                    MaxFactry.Core.MaxLogLibrary.Log(MaxFactry.Core.MaxEnumGroup.LogInfo, "Application_End stack=[" + shutDownStack + "]", "MaxHttpApplicationOverride");

                    lsContent += "Shutdown Message:\r\n" + shutDownMessage + "\r\n\r\n" + "Shutdown Stack:\r\n" + shutDownStack;
                }
                else
                {
                    lsContent = "Runtime is null";
                    MaxFactry.Core.MaxLogLibrary.Log(MaxFactry.Core.MaxEnumGroup.LogInfo, "Application_End runtime=null", "MaxHttpApplicationOverride");
                }

                //// Send an email about the application ending.
                lsContent += MaxFactry.Core.MaxLogLibrary.GetEnvironmentInformation() + "\r\n\r\n" + MaxAppLibrary.GetApplicationRunId().ToString();
                string lsVersion = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeApplication, "MaxAssemblyFileVersion").ToString();
                MaxFactry.Core.MaxLogLibrary.Log(new MaxLogEntryStructure(this.GetType(), "Application_End_Handler", MaxFactry.Core.MaxEnumGroup.LogInfo, "Ending Application {Name} for instance {InstanceId} as {Version} in environment {Environment} after {Time} milliseconds", this.Name, MaxAppLibrary.GetApplicationRunId().ToString(), lsVersion, MaxFactry.Core.MaxLogLibrary.GetEnvironmentInformation(), MaxAppLibrary.GetTimeSinceApplicationStart()));
            }
            catch (Exception loE)
            {
                MaxLogLibrary.Log(new MaxLogEntryStructure(this.GetType(), "Application_End_Handler", MaxEnumGroup.LogError, "Error in Application End Handler {ERROR}", loE));
            }
        }

        protected virtual void Application_Error_Handler(object sender, EventArgs e)
        {
            try
            {
                if (null != HttpContext.Current)
                {
                    if (null != HttpContext.Current.AllErrors && HttpContext.Current.AllErrors.Length > 1)
                    {
                        for (int lnE = 0; lnE < HttpContext.Current.AllErrors.Length; lnE++)
                        {
                            Exception loE = HttpContext.Current.AllErrors[lnE];
                            if (loE is HttpRequestValidationException || loE.Message.Contains("A potentially dangerous Request.Path value was detected"))
                            {
                                string lsDetail = MaxLogLibrary.GetExceptionDetail(HttpContext.Current.AllErrors[lnE]);
                                MaxLogLibrary.Log(new MaxLogEntryStructure("SiteErrorHandler", MaxEnumGroup.LogWarning, "Application Exception in AllErrors number {lnE] for Run Id: {MaxAppLibrary.GetApplicationRunId()}. {lsDetail}", lnE, MaxAppLibrary.GetApplicationRunId(), lsDetail));
                            }
                            else
                            {
                                MaxLogLibrary.Log(new MaxLogEntryStructure("SiteErrorHandler", MaxEnumGroup.LogError, "Application Exception in AllErrors number {lnE] for Run Id: {MaxAppLibrary.GetApplicationRunId()}.", HttpContext.Current.AllErrors[lnE], lnE, MaxAppLibrary.GetApplicationRunId()));
                            }
                        }

                        HttpContext.Current.Response.Write("Errors occurred. They have been logged.");
                        HttpContext.Current.ClearError();
                    }
                    else if (null != HttpContext.Current.Error)
                    {
                        Exception loE = HttpContext.Current.Error;
                        string lsDetail = MaxLogLibrary.GetExceptionDetail(loE);
                        string lsEnvironment = MaxLogLibrary.GetEnvironmentInformation();
                        if (loE is HttpRequestValidationException || loE.Message.Contains("A potentially dangerous Request.Path value was detected"))
                        {
                            MaxLogLibrary.Log(new MaxLogEntryStructure("SiteErrorHandler", MaxEnumGroup.LogWarning, "Application Exception in Current for Run Id: {ApplicationRunId}.  {lsDetail} {Environment}", MaxAppLibrary.GetApplicationRunId(), lsDetail, lsEnvironment));
                        }
                        else if (loE is System.Web.HttpException && loE.Message.Contains("The remote host closed the connection."))
                        {
                            MaxLogLibrary.Log(new MaxLogEntryStructure("SiteErrorHandler", MaxEnumGroup.LogWarning, "Application Exception in Current for Run Id: {ApplicationRunId}.  {lsDetail} {Environment}", loE, MaxAppLibrary.GetApplicationRunId(), lsDetail, lsEnvironment));
                        }
                        else
                        {
                            MaxLogLibrary.Log(new MaxLogEntryStructure("SiteErrorHandler", MaxEnumGroup.LogError, "Application Exception in Current for Run Id: {ApplicationRunId}.  {lsDetail} {Environment}", loE, MaxAppLibrary.GetApplicationRunId(), lsDetail, lsEnvironment));
                        }

                        HttpContext.Current.Response.Write("An error occurred. It has been logged.");
                        HttpContext.Current.ClearError();
                    }

                    if (HttpContext.Current.Response.IsClientConnected)
                    {
                        HttpContext.Current.Response.End();
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                    }
                }
                else
                {
                    _nErrorHttpContextCount++;
                    string lsEnvironment = MaxLogLibrary.GetEnvironmentInformation();
                    if (_nErrorHttpContextCount > 3)
                    {
                        MaxLogLibrary.Log(new MaxLogEntryStructure("SiteErrorHandler.3Null", MaxEnumGroup.LogError, "Application_Error_Handler ran, but HttpContext.Current was null at least 3 times.  {lsEnvironment}", new MaxException("Null HttpContext.Current"), lsEnvironment));
                    }

                    MaxLogLibrary.Log(new MaxLogEntryStructure("SiteErrorHandler.NoHttpContext", MaxEnumGroup.LogError, "{sender} {e}", sender, e));
                }
            }
            catch (Exception loE)
            {
                try
                {
                    MaxLogLibrary.Log(new MaxLogEntryStructure("SiteErrorHandler.Error", MaxEnumGroup.LogError, "Error in Application_Error_Handler", loE));
                }
                catch
                { }
            }
        }

        public override string GetVaryByCustomString(HttpContext context, string custom)
        {
            object loObject = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeProcess, "GetVaryByCustomString" + custom);
            if (null != loObject)
            {
                return loObject.ToString();
            }

            List<string> loCustom = new List<string>(custom.Split(';'));
            string lsR = string.Empty;

            if (loCustom.Contains("nocache") &&
                null != context &&
                null != context.Request &&
                !string.IsNullOrEmpty(context.Request.QueryString["nocache"]))
            {
                lsR = Guid.NewGuid().ToString();
            }
            else
            {
                if (null != context &&
                    null != context.Request &&
                    null != context.Request.Url)
                {
                    lsR += context.Request.Url.Host;
                }

                if (loCustom.Contains("user") &&
                    null != context &&
                    null != context.User &&
                    null != context.User.Identity &&
                    null != context.User.Identity.Name)
                {
                    lsR += ":" + context.User.Identity.Name;
                }

                //// Get the MaxStorageKey
                if (loCustom.Contains(MaxFactry.General.AspNet.IIS.MaxAppLibrary.MaxStorageKeyQueryName))
                {
                    string lsMaxStorageKey = MaxFactry.Base.DataLayer.Library.MaxDataLibrary.GetApplicationKey();
                    if (string.IsNullOrEmpty(lsMaxStorageKey))
                    {
                        //// Include a random storage key to make sure that nothing dependent on storage key is ever cached with Guid.Empty.
                        lsMaxStorageKey = Guid.NewGuid().ToString();
                    }

                    lsR += lsMaxStorageKey;
                }

                //// Get the Url
                if (loCustom.Contains("url"))
                {
                    if (null != context && null != context.Request && null != context.Request.Url)
                    {
                        lsR += context.Request.Url.AbsolutePath;
                    }
                }

                string lsBase = base.GetVaryByCustomString(context, custom);

                if (!string.IsNullOrEmpty(lsBase))
                {
                    lsR += lsBase;
                }
            }

            //lsR += Guid.NewGuid().ToString();
            MaxFactry.Core.MaxLogLibrary.Log(MaxFactry.Core.MaxEnumGroup.LogInfo, "GetVaryByCustomString [" + lsR + "]", "eFSWebApp");
            MaxConfigurationLibrary.SetValue(MaxEnumGroup.ScopeProcess, "GetVaryByCustomString" + custom, lsR);
            return lsR;
        }

        private void FixAppDomainRestartWhenTouchingFiles()
        {
            //FIX disable AppDomain restart when deleting subdirectory
            //This code will turn off monitoring from the root website directory.
            //Monitoring of Bin, App_Themes and other folders will still be operational, so updated DLLs will still auto deploy.
            System.Reflection.PropertyInfo loPropertyInfo = typeof(System.Web.HttpRuntime).GetProperty("FileChangesMonitor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            object loFileChangesMonitorValue = loPropertyInfo.GetValue(null, null);
            System.Reflection.FieldInfo loMonSubdirsFieldInfo = loFileChangesMonitorValue.GetType().GetField("_dirMonSubdirs", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.IgnoreCase);
            object loMonitorValue = loMonSubdirsFieldInfo.GetValue(loFileChangesMonitorValue);
            System.Reflection.MethodInfo loStopMonitoringMethod = loMonitorValue.GetType().GetMethod("StopMonitoring", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            loStopMonitoringMethod.Invoke(loMonitorValue, new object[] { });
        }
    }
}