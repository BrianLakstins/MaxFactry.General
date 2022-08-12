// <copyright file="MaxDesignLibraryASPNetIISMvc4Provider.cs" company="Lakstins Family, LLC">
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
// <change date="10/8/2015" author="Brian A. Lakstins" description="Initial creation">
// <change date="12/4/2020" author="Brian A. Lakstins" description="Add ability to GetHtml from a Razor file or raw Razor View text">
// </changelog>
#endregion

namespace MaxFactry.General.PresentationLayer.Provider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Web.Hosting;
    using MaxFactry.Core;
    using MaxFactry.General.BusinessLayer;
    using Fizzler.Systems.HtmlAgilityPack;
    using HtmlAgilityPack;
    using RazorEngine.Templating;


    /// <summary>
    /// Default provider for MaxShortCodeLibrary
    /// </summary>
    public class MaxDesignLibraryASPNetIISMvc4Provider : MaxDesignLibraryDefaultProvider
    {
        private static MaxIndex _oRazorCacheIndex = new MaxIndex();

        public override string GetThemeView(string lsView)
        {
            string lsBase = "~/Views/Shared/MaxTheme/";
            if (lsView.Contains("Partial"))
            {
                lsBase = "~/Views/MaxPartial/MaxTheme/";
            }

            string lsR = lsBase + "Default/" + lsView + ".cshtml";

            string lsTheme = MaxDesignLibrary.GetThemeName();
            string lsTest = lsBase + lsTheme + "/" + lsView + ".cshtml";
            if (System.Web.Hosting.HostingEnvironment.VirtualPathProvider.FileExists(lsTest))
            {
                lsR = lsTest;
            }

            return lsR;
        }

        public override string ConvertToInlineCSS(string lsHtml, string lsCSS)
        {
            string lsR = lsHtml;
            if (ContainsHtml(lsR))
            {
                HtmlDocument loHtml = new HtmlDocument();
                loHtml.OptionOutputAsXml = true;
                loHtml.LoadHtml(lsHtml);
                HtmlNode loDoc = loHtml.DocumentNode;
                List<HtmlNode> loStyleNodeList = loDoc.QuerySelectorAll("style").ToList();
                List<string> loStyleTextList = new List<string>();
                foreach (HtmlNode loStyleNode in loStyleNodeList)
                {
                    loStyleTextList.Add(loStyleNode.InnerText);
                    loStyleNode.ParentNode.RemoveChild(loStyleNode, false);
                }

                if (!String.IsNullOrEmpty(lsCSS))
                {
                    loStyleTextList.Add(lsCSS);
                }

                SortedList<string, MaxCSSStyleStructure> loCSSList = new SortedList<string, MaxCSSStyleStructure>();
                foreach (string lsStyleText in loStyleTextList)
                {
                    MaxIndex loList = GetListFromText(lsStyleText);
                    string[] laListKey = loList.GetSortedKeyList();
                    foreach (string lsListKey in laListKey)
                    {
                        MaxCSSStyleStructure loCSSStyle = loList[lsListKey] as MaxCSSStyleStructure;
                        //// Update any previous definitions of this style element
                        if (loCSSList.ContainsKey(loCSSStyle.Name))
                        {
                            string[] laKey = loCSSStyle.AttributeIndex.GetSortedKeyList();
                            foreach (string lsKey in laKey)
                            {
                                //// Override any attributes that already exist for this
                                if (loCSSList[loCSSStyle.Name].AttributeIndex.Contains(lsKey))
                                {
                                    loCSSList[loCSSStyle.Name].AttributeIndex.Remove(lsKey);
                                }

                                loCSSList[loCSSStyle.Name].AttributeIndex.Add(lsKey, loCSSStyle.AttributeIndex[lsKey]);
                            }
                        }
                        else
                        {
                            loCSSList.Add(loCSSStyle.Name, loCSSStyle);
                        }
                    }
                }

                foreach (KeyValuePair<String, MaxCSSStyleStructure> loCSSStyle in loCSSList)
                {
                    if (loCSSStyle.Key.Contains(":hover") || loCSSStyle.Key.Contains(":before") || loCSSStyle.Key.Contains(":") || loCSSStyle.Key.Contains("..") || loCSSStyle.Key.Contains("@"))
                    {

                    }
                    else
                    {
                        List<HtmlNode> loNodeList = loDoc.QuerySelectorAll(loCSSStyle.Key).ToList();

                        foreach (HtmlNode loNode in loNodeList)
                        {
                            if (loNode.Attributes["style"] == null)
                            {
                                loNode.SetAttributeValue("style", loCSSStyle.Value.ToString());
                            }
                            else
                            {
                                loNode.SetAttributeValue("style", loNode.Attributes["style"].Value + loCSSStyle.Value.ToString());
                            }
                        }
                    }
                }

                lsR = loHtml.DocumentNode.OuterHtml;
                lsR = lsR.Replace(@"<?xml version=""1.0"" encoding=""iso-8859-1""?>", "").Replace("&amp;", "&");
                lsR = lsR.Replace(@"<?xml version=""1.0"" encoding=""utf-8""?>", "").Replace("&amp;", "&");
            }

            return lsR;
        }

        public override string RemoveComments(string lsCode)
        {
            Regex loRegex = new Regex(@"/\*([^*]|[\r\n]|(\*+([^*/]|[\r\n])))*\*+/");
            return loRegex.Replace(lsCode, string.Empty);
        }

        public override bool ContainsHtml(string lsHtml)
        {
            if (lsHtml.Contains("<"))
            {
                if (lsHtml.Substring(lsHtml.IndexOf("<")).Contains(">"))
                {
                    return true;
                }
            }

            return false;
        }

        public override string RemoveHtml(string lsHtml)
        {
            string lsR = lsHtml;
            if (ContainsHtml(lsR))
            {
                HtmlDocument loDoc = new HtmlDocument();
                loDoc.LoadHtml(lsR);
                lsR = loDoc.DocumentNode.InnerText;
            }

            return lsR;
        }

        public override MaxIndex GetListFromText(string lsCSS)
        {
            MaxIndex loList = new MaxIndex();
            string[] laStyleLine = lsCSS.Split('}'); ;
            foreach (string lsStyle in laStyleLine)
            {
                string lsStyleValue = RemoveComments(lsStyle).Replace("\r", string.Empty).Replace("\n", string.Empty);

                if (lsStyleValue.IndexOf('{') > -1)
                {
                    loList.Add(new MaxCSSStyleStructure(lsStyleValue));
                }
            }

            return loList;
        }

        public override string GetHtml(string lsView, object loModel, MaxIndex loMetaIndex)
        {
            string lsR = string.Empty;
            DynamicViewBag loViewBag = new DynamicViewBag();
            if (null != loMetaIndex)
            {
                string[] laKey = loMetaIndex.GetSortedKeyList();
                foreach (string lsKey in laKey)
                {
                    loViewBag.AddValue(lsKey, loMetaIndex[lsKey]);
                }
            }

            Type loModelType = null;
            if (null != loModel)
            {
                loModelType = loModel.GetType();
            }

            string lsViewText = lsView;
            if (lsView.StartsWith("/") && lsView.EndsWith(".cshtml") && HostingEnvironment.VirtualPathProvider.FileExists(lsView))
            {
                VirtualFile loFile = HostingEnvironment.VirtualPathProvider.GetFile(lsView);
                Stream loStream = loFile.Open();
                try
                {
                    StreamReader loReader = new StreamReader(loStream);
                    lsViewText = loReader.ReadToEnd();
                    loReader.Close();
                }
                finally
                {
                    loStream.Dispose();
                }
            }

            string lsCacheKey = MaxFactry.Base.DataLayer.MaxDataLibrary.GetStorageKey(null) + MaxEncryptionLibrary.GetHash(typeof(object), MaxEncryptionLibrary.MD5Hash, lsViewText);
            if (_oRazorCacheIndex.Contains(lsCacheKey))
            {
                lsR = RazorEngine.Razor.Run(lsCacheKey, loModel, loViewBag);
            }
            else 
            {
                RazorEngine.Razor.Compile(lsViewText, loModelType, lsCacheKey);
                _oRazorCacheIndex.Add(lsCacheKey, true);
                lsR = RazorEngine.Razor.Run(lsCacheKey, loModel, loViewBag);
            }

            return lsR;
        }
    }
}