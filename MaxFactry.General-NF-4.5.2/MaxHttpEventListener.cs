// <copyright file="MaxHttpEventListener.cs" company="Lakstins Family, LLC">
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
// <change date="5/18/2020" author="Brian A. Lakstins" description="Initial creation">
// <change date="5/18/2020" author="Brian A. Lakstins" description="from https://www.azurefromthetrenches.com/capturing-and-tracing-all-http-requests-in-c-and-net/">
// <change date="5/20/2020" author="Brian A. Lakstins" description="Update logging">
// <change date="5/24/2020" author="Brian A. Lakstins" description="Update logging">
// <change date="5/27/2020" author="Brian A. Lakstins" description="Don't check or log Azure Application Insights connections (circular logging possible).">
// <change date="1/13/2021" author="Brian A. Lakstins" description="Don't log events that take a long time as errors">
// </changelog>
#endregion

namespace MaxFactry.General
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Diagnostics.Tracing;
    using System.Globalization;
    using MaxFactry.Core;

    public class MaxHttpEventListener : EventListener
    {
        class HttpEvent
        {
            public Stopwatch Stopwatch { get; set; }

            public string Url { get; set; }

            public DateTimeOffset RequestedAt { get; set; }
        }

        private const int HttpBeginResponse = 140;
        private const int HttpEndResponse = 141;
        private const int HttpBeginGetRequestStream = 142;
        private const int HttpEndGetRequestStream = 143;

        private readonly ConcurrentDictionary<long, HttpEvent> _trackedEvents = new ConcurrentDictionary<long, HttpEvent>();

        protected override void OnEventSourceCreated(EventSource eventSource)
        {
            if (eventSource != null && eventSource.Name == "System.Diagnostics.Eventing.FrameworkEventSource")
            {
                EnableEvents(eventSource, EventLevel.Informational, (EventKeywords)4);
            }

            base.OnEventSourceCreated(eventSource);
        }

        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            if (eventData?.Payload == null)
                return;

            try
            {
                switch (eventData.EventId)
                {
                    case HttpBeginResponse:
                    case HttpBeginGetRequestStream:
                        OnBeginHttpResponse(eventData);
                        break;
                    case HttpEndResponse:
                    case HttpEndGetRequestStream:
                        OnEndHttpResponse(eventData);
                        break;
                }
            }
            catch (Exception)
            {
                // don't let the tracer break due to frailities underneath, you might want to consider unbinding it
            }
        }

        private void OnBeginHttpResponse(EventWrittenEventArgs httpEventData)
        {
            if (httpEventData.Payload.Count < 2)
            {
                return;
            }
#if NET46
            int indexOfId = httpEventData.PayloadNames.IndexOf("id");
            int indexOfUrl = httpEventData.PayloadNames.IndexOf("uri");
#else
            int indexOfId = 0;
            int indexOfUrl = 1;
#endif

            if (indexOfId == -1 || indexOfUrl == -1)
            {
                return;
            }
            long id = Convert.ToInt64(httpEventData.Payload[indexOfId], CultureInfo.InvariantCulture);
            string url = Convert.ToString(httpEventData.Payload[indexOfUrl], CultureInfo.InvariantCulture);
            string lsHost = new Uri(url).Host.ToLower();
            //// Don't track events reporting to visualstudio.com because of Azure Insights and endless loop of logging possibility
            if (!lsHost.Contains("visualstudio.com"))
            {
                _trackedEvents[id] = new HttpEvent
                {
                    Url = url,
                    Stopwatch = new Stopwatch(),
                    RequestedAt = DateTimeOffset.UtcNow
                };
                _trackedEvents[id].Stopwatch.Start();
            }
        }

        private void OnEndHttpResponse(EventWrittenEventArgs httpEventData)
        {
            if (httpEventData.Payload.Count < 1)
            {
                return;
            }
#if NET46
            int indexOfId = httpEventData.PayloadNames.IndexOf("id");
            if (indexOfId == -1)
            {
                return;
            }
#else
            int indexOfId = 0;
#endif
            long id = Convert.ToInt64(httpEventData.Payload[indexOfId], CultureInfo.InvariantCulture);
            HttpEvent trackedEvent;
            if (_trackedEvents.TryRemove(id, out trackedEvent))
            {
                trackedEvent.Stopwatch.Stop();
#if NET46
                int indexOfSuccess = httpEventData.PayloadNames.IndexOf("success");
                int indexOfSynchronous = httpEventData.PayloadNames.IndexOf("synchronous");
                int indexOfStatusCode = httpEventData.PayloadNames.IndexOf("statusCode");
#else
                int indexOfSuccess = httpEventData.Payload.Count > 1 ? 1 : -1;
                int indexOfSynchronous = httpEventData.Payload.Count > 2 ? 2 : -1;
                int indexOfStatusCode = httpEventData.Payload.Count > 3 ? 3 : -1;
#endif

                bool? success = indexOfSuccess > -1 ? new bool?(Convert.ToBoolean(httpEventData.Payload[indexOfSuccess])) : null;
                bool? synchronous = indexOfSynchronous > -1 ? new bool?(Convert.ToBoolean(httpEventData.Payload[indexOfSynchronous])) : null;
                int? statusCode = indexOfStatusCode > -1 ? new int?(Convert.ToInt32(httpEventData.Payload[indexOfStatusCode])) : null;

                //Trace.WriteLine($"Url: {trackedEvent.Url}\r\nElapsed Time: {trackedEvent.Stopwatch.ElapsedMilliseconds}ms\r\nSuccess: {success}\r\nStatus Code: {statusCode}\r\nSynchronus: {synchronous}");
                string lsHost = new Uri(trackedEvent.Url).Host;
                if (null == success || success == false)
                {
                    MaxLogLibrary.Log(new MaxLogEntryStructure(this.GetType(), "OnEndHttpResponse-" + lsHost, MaxEnumGroup.LogError, "ID: {id}\r\n, Url: {trackedEvent.Url}\r\nElapsed Time: {trackedEvent.Stopwatch.ElapsedMilliseconds}ms\r\nSuccess: {success}\r\nStatus Code: {statusCode}\r\nSynchronus: {synchronous}", id, trackedEvent.Url, trackedEvent.Stopwatch.ElapsedMilliseconds, success, statusCode, synchronous));
                }
                else if (trackedEvent.Stopwatch.ElapsedMilliseconds > 5000)
                {
                    MaxLogLibrary.Log(new MaxLogEntryStructure(this.GetType(), "OnEndHttpResponse-" + lsHost, MaxEnumGroup.LogWarning, "ID: {id}\r\n, Url: {trackedEvent.Url}\r\nElapsed Time: {trackedEvent.Stopwatch.ElapsedMilliseconds}ms\r\nSuccess: {success}\r\nStatus Code: {statusCode}\r\nSynchronus: {synchronous}", id, trackedEvent.Url, trackedEvent.Stopwatch.ElapsedMilliseconds, success, statusCode, synchronous));
                }
                else if (trackedEvent.Stopwatch.ElapsedMilliseconds > 1000)
                {
                    MaxLogLibrary.Log(new MaxLogEntryStructure(this.GetType(), "OnEndHttpResponse-" + lsHost, MaxEnumGroup.LogInfo, "ID: {id}\r\n, Url: {trackedEvent.Url}\r\nElapsed Time: {trackedEvent.Stopwatch.ElapsedMilliseconds}ms\r\nSuccess: {success}\r\nStatus Code: {statusCode}\r\nSynchronus: {synchronous}", id, trackedEvent.Url, trackedEvent.Stopwatch.ElapsedMilliseconds, success, statusCode, synchronous));
                }
                else
                {
                    MaxLogLibrary.Log(new MaxLogEntryStructure(this.GetType(), "OnEndHttpResponse-" + lsHost, MaxEnumGroup.LogDebug, "ID: {id}\r\n, Url: {trackedEvent.Url}\r\nElapsed Time: {trackedEvent.Stopwatch.ElapsedMilliseconds}ms\r\nSuccess: {success}\r\nStatus Code: {statusCode}\r\nSynchronus: {synchronous}", id, trackedEvent.Url, trackedEvent.Stopwatch.ElapsedMilliseconds, success, statusCode, synchronous));
                }

            }
        }
    }
}