// <copyright file="MaxLogLibraryAspNetIISProvider.cs" company="Lakstins Family, LLC">
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
// <change date="7/8/2019" author="Brian A. Lakstins" description="Initial creation">
// <change date="4/28/2020" author="Brian A. Lakstins" description="Update process to get environment information to get as much as possible when some is not available.">
// <change date="4/24/2020" author="Brian A. Lakstins" description="Remove performance counter logging.">
// </changelog>
#endregion

namespace MaxFactry.Core.Provider
{
    using System;
    using System.Web;

    /// <summary>
    /// Default provider for the MaxFactory class to manage logging.
    /// </summary>
	public class MaxLogLibraryAspNetIISProvider : MaxLogLibraryBaseProvider, IMaxLogLibraryProvider
    {
        /// <summary>
        /// Adds the first 10000 messages to the MaxFactry.GetValue list.
        /// </summary>
        /// <param name="loLogEntry">Log entry to process.</param>
        public override void Log(MaxLogEntryStructure loLogEntry)
        {
        }

        /// <summary>
        /// Gets detail about an exception
        /// </summary>
        /// <param name="loException">The exception to get details</param>
        /// <returns>text information about the exception</returns>
        public string GetExceptionDetail(Exception loException)
        {
            HttpContext loContext = HttpContext.Current;
            Exception loE = loException;
            if (null == loE && null != loContext && null != loContext.AllErrors && loContext.AllErrors.Length > 0)
            {
                loE = loContext.AllErrors[loContext.AllErrors.Length - 1];

                if (null != loContext && null != loContext.Server && null != loContext.Server.GetLastError())
                {
                    ////loContext.Server.ClearError();
                }
            }

            string lsR = string.Empty;
            if (loE is HttpException)
            {
                if (loE.Message.Contains("A potentially dangerous Request.Path value was detected from the client"))
                {
                    return lsR;
                }
            }

            try
            {
                if (null != loContext && null != loContext.Request)
                {
                    lsR += "\r\nUrl: " + loContext.Request.Url.ToString();
                }
            }
            catch (Exception loERawUrl)
            {
                lsR += "\r\nUrl Error: " + loERawUrl.Message;
            }

            return lsR;
        }

        /// <summary>
        /// Gets information about the current environment
        /// </summary>
        /// <returns>Text based message about the current environment</returns>
        public string GetEnvironmentInformation()
        {
            string lsR = string.Empty;

            lsR += "\r\n\r\nHttpRuntime Information\r\n";
            try
            {
                lsR += string.Format("{0} = {1}\r\n", "identification of the application domain", HttpRuntime.AppDomainAppId);
                lsR += string.Format("{0} = {1}\r\n", "physical drive path", HttpRuntime.AppDomainAppPath);
                lsR += string.Format("{0} = {1}\r\n", "virtual path", HttpRuntime.AppDomainAppVirtualPath);
                lsR += string.Format("{0} = {1}\r\n", "domain identification of the application domain", HttpRuntime.AppDomainId);
                lsR += string.Format("{0} = {1}\r\n", "folder path for the ASP.NET client script files", HttpRuntime.AspClientScriptPhysicalPath);
                lsR += string.Format("{0} = {1}\r\n", "virtual path for the ASP.NET client script files", HttpRuntime.AspClientScriptVirtualPath);
                lsR += string.Format("{0} = {1}\r\n", "physical path of the directory where the ASP.NET executable files are installed", HttpRuntime.AspInstallDirectory);
                lsR += string.Format("{0} = {1}\r\n", "physical path to the /bin directory", HttpRuntime.BinDirectory);
                lsR += string.Format("{0} = {1}\r\n", "physical path to the directory where the common language runtime executable files are installed", HttpRuntime.ClrInstallDirectory);
                lsR += string.Format("{0} = {1}\r\n", "Gets the physical path to the directory where ASP.NET stores temporary files", HttpRuntime.CodegenDir);
                lsR += string.Format("{0} = {1}\r\n", "indicates whether the application is mapped to a (UNC) share", HttpRuntime.IsOnUNCShare);
                lsR += string.Format("{0} = {1}\r\n", "physical path to the directory where the Machine.config file", HttpRuntime.MachineConfigurationDirectory);
                lsR += string.Format("{0} = {1}\r\n", "indicates whether the current application is running in the integrated-pipeline mode", HttpRuntime.UsingIntegratedPipeline);
            }
            catch (Exception loEHttpRuntime)
            {
                lsR += "Error getting HttpRuntime information\r\n" + loEHttpRuntime.ToString() + "\r\n";
            }

            HttpContext loContext = HttpContext.Current;
            lsR += "\r\n\r\nHttpContext.Current Information\r\n";
            if (null != loContext)
            {
                try
                {
                    if (null != loContext.User)
                    {
                        try
                        {
                            lsR += "Context.Authenticated: " + loContext.User.Identity.IsAuthenticated + "\r\n";
                            if (loContext.User.Identity.IsAuthenticated)
                            {
                                lsR += "Context.UserName: " + loContext.User.Identity.Name + "\r\n";
                            }
                        }
                        catch (Exception loEUser)
                        {
                            lsR += "Context.User: Error: " + loEUser.Message + "\r\n";
                        }
                    }
                }
                catch (Exception loEUser)
                {
                    lsR += "Accessing loContext.User caused an exception:\r\n" + loEUser.ToString() + "\r\n";
                }

                try
                {
                    if (null != loContext.Request)
                    {
                        try
                        {
                            lsR += "Request.RawUrl: " + loContext.Request.RawUrl + "\r\n";
                        }
                        catch (Exception loERawUrl)
                        {
                            lsR += "Request.RawUrl: Error: " + loERawUrl.Message + "\r\n";
                        }

                        try
                        {
                            lsR += "\r\n\r\nCookies Information\r\n";
                            if (null != loContext.Request && null != loContext.Request.Cookies)
                            {
                                for (int lnV = 0; lnV < loContext.Request.Cookies.Keys.Count; lnV++)
                                {
                                    lsR += string.Format("[{0}] {1} = {2}\r\n", lnV, loContext.Request.Cookies.Keys[lnV], loContext.Request.Cookies[lnV].Value);
                                }
                            }
                        }
                        catch (Exception loECookies)
                        {
                            lsR += "Error getting Cookies information\r\n" + loECookies.ToString() + "\r\n";
                        }

                        try
                        {
                            lsR += "\r\n\r\nServer Variables\r\n";
                            if (null != loContext.Request && null != loContext.Request.ServerVariables)
                            {
                                for (int lnV = 0; lnV < loContext.Request.ServerVariables.Keys.Count; lnV++)
                                {
                                    lsR += string.Format("[{0}] {1} = {2}\r\n", lnV, loContext.Request.ServerVariables.Keys[lnV], loContext.Request.ServerVariables[lnV]);
                                }
                            }
                        }
                        catch (Exception loEServer)
                        {
                            lsR += "Error getting Server Variables information\r\n" + loEServer.ToString() + "\r\n";
                        }

                        try
                        {
                            lsR += "\r\n\r\nQuery String Variables\r\n";
                            if (null != loContext.Request && null != loContext.Request.QueryString)
                            {
                                for (int lnV = 0; lnV < loContext.Request.QueryString.Keys.Count; lnV++)
                                {
                                    lsR += string.Format("[{0}] {1} = {2}\r\n", lnV, loContext.Request.QueryString.Keys[lnV], loContext.Request.QueryString[lnV]);
                                }
                            }
                        }
                        catch (Exception loEQueryString)
                        {
                            lsR += "Error getting Query String Variables information\r\n" + loEQueryString.ToString() + "\r\n";
                        }

                        try
                        {
                            lsR += "\r\n\r\nForm Variables\r\n";
                            if (null != loContext.Request && null != loContext.Request.Form)
                            {
                                for (int lnV = 0; lnV < HttpContext.Current.Request.Form.Keys.Count; lnV++)
                                {
                                    lsR += string.Format("[{0}] {1} = {2}\r\n", lnV, loContext.Request.Form.Keys[lnV], loContext.Request.Form[lnV]);
                                }
                            }
                        }
                        catch (Exception loEForm)
                        {
                            lsR += "Error getting Form Variables information\r\n" + loEForm.ToString() + "\r\n";
                        }
                    }
                }
                catch (Exception loERequest)
                {
                    lsR += "Accessing loContext.Request caused an exception:\r\n" + loERequest.ToString() + "\r\n";
                }

                try
                {
                    lsR += "\r\n\r\nSession Information\r\n";
                    if (null != loContext.Session)
                    {
                        for (int lnV = 0; lnV < loContext.Session.Keys.Count; lnV++)
                        {
                            lsR += string.Format("[{0}] {1} = {2}\r\n", lnV, loContext.Session.Keys[lnV], loContext.Session[lnV]);
                        }

                        if (loContext.Session.Keys.Count == 0)
                        {
                            lsR += "Context.Session: No keys\r\n";
                        }
                    }
                }
                catch (Exception loESession)
                {
                    lsR += "Error getting Session information\r\n" + loESession.ToString() + "\r\n";
                }
           }

            return lsR;
        }
    }
}
