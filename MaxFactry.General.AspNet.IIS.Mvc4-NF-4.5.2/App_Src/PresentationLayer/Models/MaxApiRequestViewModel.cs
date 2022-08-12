// <copyright file="MaxApiRequestViewModel.cs" company="Lakstins Family, LLC">
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
// <change date="12/10/2020" author="Brian A. Lakstins" description="Initial creation">
// <change date="12/19/2020" author="Brian A. Lakstins" description="Add accesstoken to help in checking security for a user">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.IIS.Mvc4.PresentationLayer
{
    using MaxFactry.Core;

	/// <summary>
	/// View model for parsing a request to any api call
	/// </summary>
	public class MaxApiRequestViewModel
    {
        public MaxApiRequestViewModel()
        {
            this.Item = new MaxIndex();
        }

        public MaxApiRequestViewModel(MaxIndex loItem)
        {
            this.Item = loItem;
        }

        public string Content { get; set; }

        public string AccessToken { get; set; }

        public MaxIndex Item { get; set; }

        public MaxIndex[] ItemList { get; set; }
    }
}
