// <copyright file="MaxHtmlHelperExtension.cs" company="Lakstins Family, LLC">
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
// <change date="8/24/2015" author="Brian A. Lakstins" description="Initial creation">
// <change date="9/6/2020" author="Brian A. Lakstins" description="Updated client tool integration">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.IIS.Mvc4.PresentationLayer
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    using MaxFactry.Core;
    using MaxFactry.General.AspNet.BusinessLayer;
    using MaxFactry.General.AspNet.PresentationLayer;
    using MaxFactry.Base.BusinessLayer;
    using MaxFactry.Base.PresentationLayer;
    using MaxFactry.General.BusinessLayer;
    using MaxFactry.General.PresentationLayer;

    /// <summary>
    /// Helper library for producing HTML
    /// </summary>
    public static class MaxHtmlHelperCoreExtension
    {
        private static Dictionary<string, string> _oTrackingCodeIndex = new Dictionary<string, string>();

        private static object _oLock = new object();

        public static IHtmlString MaxGetLabelFor<T, TResult>(this HtmlHelper<T> helper, Expression<Func<T, TResult>> loLabelExpression, IDictionary<string, object> loAttributeDictionary, int lnColumnWidth)
        {
            MaxIndex loAttributeIndex = new MaxIndex();
            foreach (string lsKey in loAttributeDictionary.Keys)
            {
                loAttributeIndex.Add(lsKey, loAttributeDictionary[lsKey]);
            }

            MaxIndex loLabelIndex = MaxDesignLibrary.GetSubIndex("label", loAttributeIndex);
            if (loLabelIndex.Contains("class"))
            {
                List<string> loClassList = new List<string>(loLabelIndex["class"].ToString().Split(' '));
                if (!loClassList.Contains("control-label"))
                {
                    loLabelIndex["class"] = loLabelIndex["class"].ToString() + " control-label";
                }
            }
            else
            {
                loLabelIndex.Add("class", "control-label col-sm-" + lnColumnWidth.ToString());
            }

            return LabelExtensions.LabelFor(helper, loLabelExpression, loLabelIndex);
        }

        /// <summary>
        /// Gets the Html for an input form element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="helper"></param>
        /// <param name="loLabelExpression"></param>
        /// <param name="loAttributeIndex"></param>
        /// <returns></returns>
        public static IHtmlString MaxFormGroup<T, TResult>(this HtmlHelper<T> helper, Expression<Func<T, TResult>> loLabelExpression, IHtmlString lsInput, IDictionary<string, object> loAttributeDictionary, Expression<Func<T, bool>> lbIsOverride)
        {
            MaxIndex loAttributeIndex = new MaxIndex();
            foreach (string lsKey in loAttributeDictionary.Keys)
            {
                loAttributeIndex.Add(lsKey, loAttributeDictionary[lsKey]);
            }

            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(loLabelExpression, helper.ViewData);
            bool lbIsValid = helper.ViewData.ModelState.IsValidField(metadata.PropertyName);
            string lsValidationMessage = string.Empty;
            MvcHtmlString loValidationMessage = ValidationExtensions.ValidationMessageFor(helper, loLabelExpression);
            if (null != loValidationMessage)
            {
                lsValidationMessage = loValidationMessage.ToString();
            }

            string lsLabel = MaxGetLabelFor(helper, loLabelExpression, loAttributeDictionary, 2).ToString();

            MaxHtmlTagStructure loContainer = new MaxHtmlTagStructure("div");
            loContainer.Content = lsLabel;
            loContainer.Class = "form-group";
            if (!lbIsValid)
            {
                loContainer.Class += " has-error";
            }

            int lnInputColumnWidth = 10;
            if (null != lbIsOverride)
            {
                MaxHtmlTagStructure loOverrideColumn = new MaxHtmlTagStructure("div");
                MaxIndex loOverrideColumnIndex = MaxDesignLibrary.GetSubIndex("overridecolumn", loAttributeIndex);
                if (loOverrideColumnIndex.Contains("class"))
                {
                    loOverrideColumn.Class = loOverrideColumnIndex["class"].ToString();
                }
                else
                {
                    loOverrideColumn.Class = "col-sm-1";
                }

                MaxIndex loOverrideIndex = MaxDesignLibrary.GetSubIndex("override", loAttributeIndex);
                if (loOverrideIndex.Contains("class"))
                {
                    List<string> loClassList = new List<string>(loOverrideIndex["class"].ToString().Split(' '));
                    if (!loClassList.Contains("form-control"))
                    {
                        loOverrideIndex["class"] = loOverrideIndex["class"].ToString() + " form-control";
                    }
                }
                else
                {
                    loOverrideIndex.Add("class", "form-control");
                }

                if (loOverrideIndex.Contains("style"))
                {
                    List<string> loStyleList = new List<string>(loOverrideIndex["style"].ToString().Split(' '));
                    if (!loStyleList.Contains("width"))
                    {
                        loOverrideIndex["style"] = loOverrideIndex["style"].ToString() + ";width:34px;";
                    }
                }
                else
                {
                    loOverrideIndex.Add("style", "width:34px;");
                }

                string lsOverride = InputExtensions.CheckBoxFor(helper, lbIsOverride, loOverrideIndex).ToString();
                loOverrideColumn.Content = lsOverride;
                loContainer.Content += loOverrideColumn.ToString();
                lnInputColumnWidth -= 1;
            }

            MaxHtmlTagStructure loInputColumn = new MaxHtmlTagStructure("div");
            MaxIndex loColumnIndex = MaxDesignLibrary.GetSubIndex("inputcolumn", loAttributeIndex);
            if (loColumnIndex.Contains("class"))
            {
                loInputColumn.Class = loColumnIndex["class"].ToString();
            }
            else
            {
                loInputColumn.Class = "col-sm-" + lnInputColumnWidth.ToString();
            }

            loInputColumn.Content = lsInput.ToString();
            if (!string.IsNullOrEmpty(lsValidationMessage))
            {
                MaxHtmlTagStructure loHelpTag = new MaxHtmlTagStructure("span");
                loHelpTag.Class = "help-block";
                loHelpTag.Content = lsValidationMessage;
                loInputColumn.Content += loHelpTag.ToString();
            }

            loContainer.Content += loInputColumn.ToString();
            return new HtmlString(loContainer.ToString());
        }

        /// <summary>
        /// Gets the Html for an input form element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="helper"></param>
        /// <param name="expression"></param>
        /// <param name="loAttributeIndex"></param>
        /// <returns></returns>
        public static IHtmlString MaxFormInputGroup<T, TResult>(this HtmlHelper<T> helper, Expression<Func<T, TResult>> expression, IDictionary<string, object> loAttributeDictionary)
        {
            MaxIndex loAttributeIndex = new MaxIndex();
            foreach (string lsKey in loAttributeDictionary.Keys)
            {
                loAttributeIndex.Add(lsKey, loAttributeDictionary[lsKey]);
            }

            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            bool lbIsValid = helper.ViewData.ModelState.IsValidField(metadata.PropertyName);
            string lsValidationMessage = string.Empty;
            MvcHtmlString loValidationMessage = ValidationExtensions.ValidationMessageFor(helper, expression);
            if (null != loValidationMessage)
            {
                lsValidationMessage = loValidationMessage.ToString();
            }

            MaxIndex loInputIndex = MaxDesignLibrary.GetSubIndex("input", loAttributeIndex);
            if (loInputIndex.Contains("class"))
            {
                List<string> loClassList = new List<string>(loInputIndex["class"].ToString().Split(' '));
                if (!loClassList.Contains("form-control"))
                {
                    loInputIndex["class"] = loInputIndex["class"].ToString() + " form-control";
                }
            }
            else
            {
                loInputIndex.Add("class", "form-control");
            }

            MaxIndex loLabelIndex = MaxDesignLibrary.GetSubIndex("label", loAttributeIndex);
            if (loLabelIndex.Contains("class"))
            {
                List<string> loClassList = new List<string>(loLabelIndex["class"].ToString().Split(' '));
                if (!loClassList.Contains("control-label"))
                {
                    loLabelIndex["class"] = loLabelIndex["class"].ToString() + " control-label";
                }
            }
            else
            {
                loLabelIndex.Add("class", "control-label col-sm-2");
            }

            string lsInput = InputExtensions.TextBoxFor(helper, expression, loInputIndex).ToString();
            string lsLabel = LabelExtensions.LabelFor(helper, expression, loLabelIndex).ToString();

            MaxHtmlTagStructure loGroupDiv = new MaxHtmlTagStructure("div");
            loGroupDiv.Content = lsLabel;
            loGroupDiv.Class = "form-group";

            MaxHtmlTagStructure loInputColumnDiv = new MaxHtmlTagStructure("div");
            MaxIndex loColumnIndex = MaxDesignLibrary.GetSubIndex("inputcolumn", loAttributeIndex);
            if (loColumnIndex.Contains("class"))
            {
                loInputColumnDiv.Class = loColumnIndex["class"].ToString();
            }
            else
            {
                loInputColumnDiv.Class = "col-sm-10";
            }

            MaxHtmlTagStructure loInputGroupDiv = new MaxHtmlTagStructure("div");
            loInputGroupDiv.Content = lsInput;
            loInputGroupDiv.Class = "input-group";
            if (!lbIsValid)
            {
                loInputGroupDiv.Class += " has-error";
            }

            MaxIndex loToolTipIndex = MaxDesignLibrary.GetSubIndex("tooltip", loAttributeIndex);
            MaxHtmlTagStructure loToolTip = MaxDesignLibrary.GetToolTipTag(loToolTipIndex);

            MaxIndex loToolTipContentIndex = MaxDesignLibrary.GetSubIndex("tooltipcontent", loAttributeIndex);
            MaxHtmlTagStructure loToolTipContent = MaxDesignLibrary.GetToolTipContentTag(loToolTipContentIndex);

            loToolTip.Content = loToolTipContent.ToString();
            loInputGroupDiv.Content += loToolTip.ToString();
            loInputColumnDiv.Content += loInputGroupDiv.ToString();

            MaxHtmlTagStructure loHelpTag = new MaxHtmlTagStructure("span");
            loHelpTag.Class = "help-block";
            loHelpTag.Content = lsValidationMessage;
            loInputColumnDiv.Content += loHelpTag.ToString();

            loGroupDiv.Content += loInputColumnDiv.ToString();

            return new HtmlString(loGroupDiv.ToString());
        }

        /// <summary>
        /// Gets the Html for an input form element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="helper"></param>
        /// <param name="expression"></param>
        /// <param name="loAttributeIndex"></param>
        /// <param name="lsInput"></param>
        /// <returns></returns>
        public static IHtmlString MaxFormInputGroup<T, TResult>(this HtmlHelper<T> helper, Expression<Func<T, TResult>> expression, IDictionary<string, object> loAttributeDictionary, IHtmlString lsInput)
        {
            MaxIndex loAttributeIndex = new MaxIndex();
            foreach (string lsKey in loAttributeDictionary.Keys)
            {
                loAttributeIndex.Add(lsKey, loAttributeDictionary[lsKey]);
            }

            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            bool lbIsValid = helper.ViewData.ModelState.IsValidField(metadata.PropertyName);
            string lsValidationMessage = string.Empty;
            MvcHtmlString loValidationMessage = ValidationExtensions.ValidationMessageFor(helper, expression);
            if (null != loValidationMessage)
            {
                lsValidationMessage = loValidationMessage.ToString();
            }

            MaxIndex loInputIndex = MaxDesignLibrary.GetSubIndex("input", loAttributeIndex);
            if (loInputIndex.Contains("class"))
            {
                List<string> loClassList = new List<string>(loInputIndex["class"].ToString().Split(' '));
                if (!loClassList.Contains("form-control"))
                {
                    loInputIndex["class"] = loInputIndex["class"].ToString() + " form-control";
                }
            }
            else
            {
                loInputIndex.Add("class", "form-control");
            }

            MaxIndex loLabelIndex = MaxDesignLibrary.GetSubIndex("label", loAttributeIndex);
            if (loLabelIndex.Contains("class"))
            {
                List<string> loClassList = new List<string>(loLabelIndex["class"].ToString().Split(' '));
                if (!loClassList.Contains("control-label"))
                {
                    loLabelIndex["class"] = loLabelIndex["class"].ToString() + " control-label";
                }
            }
            else
            {
                loLabelIndex.Add("class", "control-label col-sm-2");
            }

            string lsLabel = LabelExtensions.LabelFor(helper, expression, loLabelIndex).ToString();

            MaxHtmlTagStructure loGroupDiv = new MaxHtmlTagStructure("div");
            loGroupDiv.Content = lsLabel;
            loGroupDiv.Class = "form-group";

            MaxHtmlTagStructure loInputColumnDiv = new MaxHtmlTagStructure("div");
            MaxIndex loColumnIndex = MaxDesignLibrary.GetSubIndex("inputcolumn", loAttributeIndex);
            if (loColumnIndex.Contains("class"))
            {
                loInputColumnDiv.Class = loColumnIndex["class"].ToString();
            }
            else
            {
                loInputColumnDiv.Class = "col-sm-10";
            }

            MaxHtmlTagStructure loInputGroupDiv = new MaxHtmlTagStructure("div");
            loInputGroupDiv.Content = lsInput.ToString();
            //loInputGroupDiv.Class = "input-group";
            if (!lbIsValid)
            {
                loInputGroupDiv.Class += " has-error";
            }

            loInputColumnDiv.Content += loInputGroupDiv.ToString();

            MaxHtmlTagStructure loHelpTag = new MaxHtmlTagStructure("span");
            loHelpTag.Class = "help-block";
            loHelpTag.Content = lsValidationMessage;
            loInputColumnDiv.Content += loHelpTag.ToString();

            loGroupDiv.Content += loInputColumnDiv.ToString();

            return new HtmlString(loGroupDiv.ToString());
        }

        /// <summary>
        /// Gets the Html for an input form element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="helper"></param>
        /// <param name="expression"></param>
        /// <param name="loAttributeIndex"></param>
        /// <returns></returns>
        public static IHtmlString MaxFormInputGroupOverride<T, TResult>(this HtmlHelper<T> helper, Expression<Func<T, TResult>> expression, Expression<Func<T, bool>> lbIsOverride, IDictionary<string, object> loAttributeDictionary)
        {
            MaxIndex loAttributeIndex = new MaxIndex();
            foreach (string lsKey in loAttributeDictionary.Keys)
            {
                loAttributeIndex.Add(lsKey, loAttributeDictionary[lsKey]);
            }

            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            bool lbIsValid = helper.ViewData.ModelState.IsValidField(metadata.PropertyName);
            string lsValidationMessage = string.Empty;
            MvcHtmlString loValidationMessage = ValidationExtensions.ValidationMessageFor(helper, expression);
            if (null != loValidationMessage)
            {
                lsValidationMessage = loValidationMessage.ToString();
            }

            MaxIndex loInputIndex = MaxDesignLibrary.GetSubIndex("input", loAttributeIndex);
            if (loInputIndex.Contains("class"))
            {
                List<string> loClassList = new List<string>(loInputIndex["class"].ToString().Split(' '));
                if (!loClassList.Contains("form-control"))
                {
                    loInputIndex["class"] = loInputIndex["class"].ToString() + " form-control";
                }
            }
            else
            {
                loInputIndex.Add("class", "form-control");
            }

            MaxIndex loLabelIndex = MaxDesignLibrary.GetSubIndex("label", loAttributeIndex);
            if (loLabelIndex.Contains("class"))
            {
                List<string> loClassList = new List<string>(loLabelIndex["class"].ToString().Split(' '));
                if (!loClassList.Contains("control-label"))
                {
                    loLabelIndex["class"] = loLabelIndex["class"].ToString() + " control-label";
                }
            }
            else
            {
                loLabelIndex.Add("class", "control-label col-sm-2");
            }

            MaxIndex loOverrideIndex = MaxDesignLibrary.GetSubIndex("override", loAttributeIndex);
            if (loOverrideIndex.Contains("class"))
            {
                List<string> loClassList = new List<string>(loOverrideIndex["class"].ToString().Split(' '));
                if (!loClassList.Contains("form-control"))
                {
                    loOverrideIndex["class"] = loOverrideIndex["class"].ToString() + " form-control";
                }
            }
            else
            {
                loOverrideIndex.Add("class", "form-control");
            }

            string lsLabel = LabelExtensions.LabelFor(helper, expression, loLabelIndex).ToString();

            MaxHtmlTagStructure loGroupDiv = new MaxHtmlTagStructure("div");
            loGroupDiv.Content = lsLabel;
            loGroupDiv.Class = "form-group";

            string lsOverride = InputExtensions.CheckBoxFor(helper, lbIsOverride, loOverrideIndex).ToString();
            MaxHtmlTagStructure loOverrideColumnDiv = new MaxHtmlTagStructure("div");
            MaxIndex loOverrideColumnIndex = MaxDesignLibrary.GetSubIndex("overridecolumn", loAttributeIndex);
            if (loOverrideColumnIndex.Contains("class"))
            {
                loOverrideColumnDiv.Class = loOverrideColumnIndex["class"].ToString();
            }
            else
            {
                loOverrideColumnDiv.Class = "col-sm-1";
            }

            loOverrideColumnDiv.Content = lsOverride;
            loGroupDiv.Content += loOverrideColumnDiv.ToString();

            MaxHtmlTagStructure loInputColumnDiv = new MaxHtmlTagStructure("div");
            MaxIndex loColumnIndex = MaxDesignLibrary.GetSubIndex("inputcolumn", loAttributeIndex);
            if (loColumnIndex.Contains("class"))
            {
                loInputColumnDiv.Class = loColumnIndex["class"].ToString();
            }
            else
            {
                loInputColumnDiv.Class = "col-sm-9";
            }

            string lsInput = InputExtensions.TextBoxFor(helper, expression, loInputIndex).ToString();
            MaxHtmlTagStructure loInputGroupDiv = new MaxHtmlTagStructure("div");
            loInputGroupDiv.Content = lsInput;
            loInputGroupDiv.Class = "input-group";
            if (!lbIsValid)
            {
                loInputGroupDiv.Class += " has-error";
            }

            MaxIndex loToolTipIndex = MaxDesignLibrary.GetSubIndex("tooltip", loAttributeIndex);
            MaxHtmlTagStructure loToolTip = MaxDesignLibrary.GetToolTipTag(loToolTipIndex);

            MaxIndex loToolTipContentIndex = MaxDesignLibrary.GetSubIndex("tooltipcontent", loAttributeIndex);
            MaxHtmlTagStructure loToolTipContent = MaxDesignLibrary.GetToolTipContentTag(loToolTipContentIndex);

            loToolTip.Content = loToolTipContent.ToString();
            loInputGroupDiv.Content += loToolTip.ToString();
            loInputColumnDiv.Content += loInputGroupDiv.ToString();

            MaxHtmlTagStructure loHelpTag = new MaxHtmlTagStructure("span");
            loHelpTag.Class = "help-block";
            loHelpTag.Content = lsValidationMessage;
            loInputColumnDiv.Content += loHelpTag.ToString();

            loGroupDiv.Content += loInputColumnDiv.ToString();

            return new HtmlString(loGroupDiv.ToString());
        }

        /// <summary>
        /// Gets the Html for a textarea form element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="helper"></param>
        /// <param name="expression"></param>
        /// <param name="loAttributeIndex"></param>
        /// <returns></returns>
        public static IHtmlString MaxFormTextAreaGroup<T, TResult>(this HtmlHelper<T> helper, Expression<Func<T, TResult>> expression, IDictionary<string, object> loAttributeDictionary)
        {
            MaxIndex loAttributeIndex = new MaxIndex();
            foreach (string lsKey in loAttributeDictionary.Keys)
            {
                loAttributeIndex.Add(lsKey, loAttributeDictionary[lsKey]);
            }

            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            bool lbIsValid = helper.ViewData.ModelState.IsValidField(metadata.PropertyName);
            string lsValidationMessage = string.Empty;
            MvcHtmlString loValidationMessage = ValidationExtensions.ValidationMessageFor(helper, expression);
            if (null != loValidationMessage)
            {
                lsValidationMessage = loValidationMessage.ToString();
            }

            MaxIndex loTextAreaIndex = MaxDesignLibrary.GetSubIndex("textarea", loAttributeIndex);
            if (loTextAreaIndex.Contains("class"))
            {
                List<string> loClassList = new List<string>(loTextAreaIndex["class"].ToString().Split(' '));
                if (!loClassList.Contains("form-control"))
                {
                    loTextAreaIndex["class"] = loTextAreaIndex["class"].ToString() + " form-control";
                }
            }
            else
            {
                loTextAreaIndex.Add("class", "form-control");
            }

            MaxIndex loLabelIndex = MaxDesignLibrary.GetSubIndex("label", loAttributeIndex);
            if (loLabelIndex.Contains("class"))
            {
                List<string> loClassList = new List<string>(loLabelIndex["class"].ToString().Split(' '));
                if (!loClassList.Contains("control-label"))
                {
                    loLabelIndex["class"] = loLabelIndex["class"].ToString() + " control-label";
                }
            }
            else
            {
                loLabelIndex.Add("class", "control-label col-sm-2");
            }

            string lsInput = TextAreaExtensions.TextAreaFor(helper, expression, loTextAreaIndex).ToString();
            string lsLabel = LabelExtensions.LabelFor(helper, expression, loLabelIndex).ToString();

            MaxHtmlTagStructure loGroupDiv = new MaxHtmlTagStructure("div");
            loGroupDiv.Content = lsLabel;
            loGroupDiv.Class = "form-group";

            if (!lbIsValid)
            {
                loGroupDiv.Class += " has-error";
            }

            MaxHtmlTagStructure loInputColumnDiv = new MaxHtmlTagStructure("div");
            MaxIndex loColumnIndex = MaxDesignLibrary.GetSubIndex("inputcolumn", loAttributeIndex);
            if (loColumnIndex.Contains("class"))
            {
                loInputColumnDiv.Class = loColumnIndex["class"].ToString();
            }
            else
            {
                loInputColumnDiv.Class = "col-sm-10";
            }

            MaxHtmlTagStructure loInputGroupDiv = new MaxHtmlTagStructure("div");
            loInputGroupDiv.Content = lsInput;
            loInputGroupDiv.Class = "input-group";

            MaxIndex loToolTipIndex = MaxDesignLibrary.GetSubIndex("tooltip", loAttributeIndex);
            MaxHtmlTagStructure loToolTip = MaxDesignLibrary.GetToolTipTag(loToolTipIndex);

            MaxIndex loToolTipContentIndex = MaxDesignLibrary.GetSubIndex("tooltipcontent", loAttributeIndex);
            MaxHtmlTagStructure loToolTipContent = MaxDesignLibrary.GetToolTipContentTag(loToolTipContentIndex);

            loToolTip.Content = loToolTipContent.ToString();
            loInputGroupDiv.Content += loToolTip.ToString();
            loInputColumnDiv.Content += loInputGroupDiv.ToString();

            MaxHtmlTagStructure loHelpTag = new MaxHtmlTagStructure("span");
            loHelpTag.Class = "help-block";
            loHelpTag.Content = lsValidationMessage;
            loInputColumnDiv.Content += loHelpTag.ToString();

            loGroupDiv.Content += loInputColumnDiv.ToString();

            return new HtmlString(loGroupDiv.ToString());
        }

        /// <summary>
        /// Gets the Html for an input form element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="helper"></param>
        /// <param name="expression"></param>
        /// <param name="loAttributeIndex"></param>
        /// <returns></returns>
        public static IHtmlString MaxGetHasError<T, TResult>(this HtmlHelper<T> helper, Expression<Func<T, TResult>> expression)
        {
            string lsR = string.Empty;
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            bool lbIsValid = helper.ViewData.ModelState.IsValidField(metadata.PropertyName);
            if (!lbIsValid)
            {
                lsR = "has-error";
            }

            return new HtmlString(lsR);
        }

        /// <summary>
        /// Gets the Html for a textarea form element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IHtmlString MaxGrid<T>(this HtmlHelper<T> helper, List<IHtmlString> loContentList, int lnGridColumnCount)
        {
            string lsR = string.Empty;
            int lnWidth = 12 / lnGridColumnCount;

            MaxHtmlTagStructure loRowDiv = new MaxHtmlTagStructure("div");
            loRowDiv.Class = "row";
            for (int lnC = 0; lnC < loContentList.Count; lnC++)
            {
                MaxHtmlTagStructure loColumnDiv = new MaxHtmlTagStructure("div");
                loColumnDiv.Class = "col-md-" + lnWidth.ToString();
                loColumnDiv.Content = loContentList[lnC].ToString();
                loRowDiv.Content += loColumnDiv.ToString();

                if (lnC > 0 && (lnC + 1) % lnGridColumnCount == 0)
                {
                    lsR += loRowDiv.ToString();
                    loRowDiv = new MaxHtmlTagStructure("div");
                    loRowDiv.Class = "row";
                }
            }

            if (!string.IsNullOrEmpty(loRowDiv.Content))
            {
                lsR += loRowDiv.ToString();
            }

            return new HtmlString(lsR);
        }

        /// <summary>
        /// Gets the Html for some named content.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string MaxGetController<T>(this HtmlHelper<T> helper, ViewContext loContext)
        {
            ViewContext loParent = loContext;
            while (null != loParent.ParentActionViewContext)
            {
                loParent = loParent.ParentActionViewContext;
            }

            return loParent.Controller.ControllerContext.RouteData.Values["controller"].ToString();
        }

        /// <summary>
        /// Gets the Html for some named content.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string MaxGetAction<T>(this HtmlHelper<T> helper, ViewContext loContext)
        {
            ViewContext loParent = loContext;
            while (null != loParent.ParentActionViewContext)
            {
                loParent = loParent.ParentActionViewContext;
            }

            return loParent.Controller.ControllerContext.RouteData.Values["action"].ToString();
        }

        /// <summary>
        /// Parses Html for Short Codes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string MaxGetContentShortCode<T>(this HtmlHelper<T> helper, string lsContent)
        {
            return MaxShortCodeLibrary.GetContentShortCode(lsContent);
        }

        /// <summary>
        /// Gets the Html for some named content.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IHtmlString MaxGetContent<T>(this HtmlHelper<T> helper, string lsContentName)
        {
            if (!string.IsNullOrEmpty(helper.ViewData[lsContentName] as string))
            {
                return new HtmlString(helper.ViewData[lsContentName] as string);
            }

            object loObject = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeProcess, "MaxHtmlContent-" + lsContentName);
            if (null != loObject)
            {
                return new HtmlString(loObject.ToString());
            }

            return new HtmlString(string.Empty);
        }

        /// <summary>
        /// Gets the Html for some named content.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IHtmlString GetContent<T>(this HtmlHelper<T> helper, string lsContentName)
        {
            return MaxGetContent<T>(helper, lsContentName);
        }


        public static IHtmlString MaxIdFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return new HtmlString(htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(ExpressionHelper.GetExpressionText(expression)));
        }

        public static IHtmlString MaxNameFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return new HtmlString(htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression)));
        }

        public static IHtmlString MaxErrorClassFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            string lsR = string.Empty;
            if (!htmlHelper.ViewData.ModelState.IsValidField(htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression))))
            {
                lsR = "has-error";

            }

            return new HtmlString(lsR);
        }

        public static ModelErrorCollection MaxGetErrorListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            ModelErrorCollection loR = new ModelErrorCollection();
            ModelState loState = htmlHelper.ViewData.ModelState[htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression))];
            if (null != loState)
            {
                foreach (var loError in loState.Errors)
                {
                    loR.Add(loError);
                }
            }

            return loR;
        }

        public static IHtmlString MaxToolTip<TModel>(this HtmlHelper<TModel> htmlHelper, string lsText, string lsGlyph)
        {
            MaxHtmlTagStructure loTip = new MaxHtmlTagStructure();
            loTip.Tag = "span";
            loTip.Class = "tooltipactive btn input-group-addon";
            loTip.AddToAttribute("data-placement", "left");
            loTip.AddToAttribute("data-toggle", "tooltip");
            loTip.AddToAttribute("title", lsText);

            MaxHtmlTagStructure loGlyph = new MaxHtmlTagStructure();
            loGlyph.Tag = "span";
            loGlyph.Class = "glyphicon glyphicon-" + lsGlyph;
            loTip.Content = loGlyph.ToString();
            return new HtmlString(loTip.ToString());
        }

        public static string MaxGetThemeView<T>(this HtmlHelper<T> helper, string lsView)
        {
            string lsBase = "~/Views/Shared/MaxTheme/";
            if (lsView.Contains("Partial"))
            {
                lsBase = "~/Views/MaxPartial/MaxTheme/";
            }

            string lsR = lsBase + "Default/" + lsView + ".cshtml";

            string lsTheme = MaxDesignLibrary.GetThemeName();
            string lsFile = lsBase + lsTheme + "/" + lsView + ".cshtml";
            if (System.Web.Hosting.HostingEnvironment.VirtualPathProvider.FileExists(lsFile))
            {
                lsR = lsFile;
            }

            return lsR;
        }

        public static string MaxGetThemeName<T>(this HtmlHelper<T> helper)
        {
            string lsR = string.Empty;
            lsR = MaxDesignLibrary.GetThemeName();
            return lsR;
        }


        public static void MaxSetThemeName<T>(this HtmlHelper<T> helper, string lsName)
        {
            MaxDesignLibrary.SetThemeName(lsName);
        }

        public static string MaxGetTitle<T>(this HtmlHelper<T> helper)
        {
            return MaxOwinLibrary.GetTitle();
        }

        public static void MaxIncludeClientTool<T>(this HtmlHelper<T> helper, string lsToolName)
        {
            MaxClientToolEntity.Include(lsToolName);
        }

        public static IHtmlString MaxGetClientToolHtml<T>(this HtmlHelper<T> helper, string lsLocation)
        {
            MaxClientToolViewModel loModel = new MaxClientToolViewModel();
            return new HtmlString(loModel.GetHtml(lsLocation));
        }

        public static bool MaxIsInRole<T>(this HtmlHelper<T> helper, string lsRoleList)
        {
            return MaxHtmlHelperLibrary.MaxIsInRole(lsRoleList);
        }

        public static bool MaxIsRecaptchVerified(string lsSecret, string lsResponse, string lsRemoteIP)
        {
            return MaxOwinLibrary.IsRecaptchaVerified(lsSecret, lsResponse, lsRemoteIP);
        }

        public static bool MaxIshCaptchVerified(string lsSecret, string lsResponse, string lsRemoteIP)
        {
            return MaxOwinLibrary.IshCaptchVerified(lsSecret, lsResponse, lsRemoteIP);
        }

        public static string MaxFileGetUrl<T>(this HtmlHelper<T> helper, string lsName)
        {
            return MaxHtmlHelperLibrary.MaxFileGetUrl(lsName);
        }

        public static string GetRandomSelection<T>(this HtmlHelper<T> helper, params string[] laParams)
        {
            return MaxHtmlHelperLibrary.GetRandomSelection(laParams);
        }
    }
}