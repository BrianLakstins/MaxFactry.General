// <copyright file="MaxApiMessageViewModel.cs" company="Lakstins Family, LLC">
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
// <change date="12/27/2020" author="Brian A. Lakstins" description="Initial creation">
// <change date="10/11/2021" author="Brian A. Lakstins" description="Add log list from current thread">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.IIS.Mvc4.PresentationLayer
{
    using System;
    using System.Collections.Generic;
    using MaxFactry.Core;

    /// <summary>
    /// View model for the message part of an api call response
    /// </summary>
    public class MaxApiMessageViewModel
    {
        public MaxApiMessageViewModel()
        {
            this.Success = string.Empty;
            this.Log = string.Empty;
            this.Error = string.Empty;
            this.Warning = string.Empty;
        }

        public string Success { get; set; }

        public string Warning { get; set; }

        public string Log { get; set; }

        public string Error { get; set; }

        public MaxIndex[] LogList
        {
            get
            {
                List<MaxIndex> loR = new List<MaxIndex>();
                MaxLogEntryStructure[] laLog = MaxLogLibrary.GetRecent(null, null, MaxEnumGroup.LogNotice, MaxEnumGroup.LogError, MaxEnumGroup.LogCritical);
                foreach (MaxLogEntryStructure loLog in laLog)
                {
                    MaxIndex loLogIndex = new MaxIndex();
                    loLogIndex.Add("Timestamp", loLog.Timestamp.ToString());
                    loLogIndex.Add("Message", loLog.Message);
                    loLogIndex.Add("Level", loLog.Level.ToString().Replace("Log", string.Empty));
                    loLogIndex.Add("Name", loLog.Name);
                    loR.Add(loLogIndex);
                }

                return loR.ToArray();
            }
        }

    }
}
