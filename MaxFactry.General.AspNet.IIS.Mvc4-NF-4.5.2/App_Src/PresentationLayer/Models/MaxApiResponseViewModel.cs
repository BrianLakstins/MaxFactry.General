﻿// <copyright file="MaxApiResponseViewModel.cs" company="Lakstins Family, LLC">
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
// <change date="12/1/2020" author="Brian A. Lakstins" description="Initial creation">
// <change date="12/15/2020" author="Brian A. Lakstins" description="Change array to list to make easier to add items">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.IIS.Mvc4.PresentationLayer
{
    using MaxFactry.Core;
    using System.Collections.Generic;

	/// <summary>
	/// View model for responding to any api call
	/// </summary>
	public class MaxApiResponseViewModel
    {
        public MaxApiResponseViewModel()
        {
            this.Item = new MaxIndex();
            this.ItemList = new List<MaxIndex>();
            this.Message = new MaxApiMessageViewModel();
            this.Page = new MaxIndex();
        }

        public MaxApiResponseViewModel(MaxIndex loItem)
        {
            this.Item = loItem;
            this.ItemList = new List<MaxIndex>();
            this.Message = new MaxApiMessageViewModel();
            this.Page = new MaxIndex();
        }

        public MaxApiMessageViewModel Message { get; set; }

        public MaxIndex Item { get; set; }

        public List<MaxIndex> ItemList { get; set; }

        public MaxIndex Page { get; set; }
    }
}
