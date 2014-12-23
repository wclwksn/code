﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScriptCoreLib.JavaScript.DOM
{
    // X:\jsc.svn\examples\javascript\Test\TestServiceWorker\TestServiceWorker\Application.cs
    // http://www.w3.org/TR/service-workers/#fetch-event-interface

    [Script(HasNoPrototype = true, ExternalTarget = "FetchEvent")]
    public class FetchEvent : IEvent
    {
        public readonly Request request;
        public readonly ServiceWorkerClient client;
        public readonly bool isReload;
    }

}
