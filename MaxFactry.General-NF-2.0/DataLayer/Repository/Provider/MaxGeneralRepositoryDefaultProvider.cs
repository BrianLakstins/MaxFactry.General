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
// </changelog>
#endregion

namespace MaxFactry.General.DataLayer.Provider
{
    using System;
    using System.IO;
    using System.Net;
    using MaxFactry.Core;
    using MaxFactry.Base.DataLayer;
    using MaxFactry.Base.DataLayer.Provider;
    using MaxFactry.General.DataLayer;

    /// <summary>
    /// Provider for MaxApplicationInternetRepository
    /// </summary>
    public class MaxGeneralRepositoryDefaultProvider : MaxBaseIdRepositoryDefaultProvider, IMaxGeneralRepositoryProvider
    {
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
    }
}
