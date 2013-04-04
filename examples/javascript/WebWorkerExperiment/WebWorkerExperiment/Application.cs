using Abstractatech.ConsoleFormPackage.Library;
using ScriptCoreLib;
using ScriptCoreLib.Delegates;
using ScriptCoreLib.Extensions;
using ScriptCoreLib.JavaScript;
using ScriptCoreLib.JavaScript.Components;
using ScriptCoreLib.JavaScript.DOM;
using ScriptCoreLib.JavaScript.DOM.HTML;
using ScriptCoreLib.JavaScript.Extensions;
using ScriptCoreLib.JavaScript.Runtime;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using WebWorkerExperiment.Design;
using WebWorkerExperiment.HTML.Pages;

namespace WebWorkerExperiment
{
    /// <summary>
    /// Your client side code running inside a web browser as JavaScript.
    /// </summary>
    [Obsolete("JSC should not implement web workers unless, android webview has them and xml is supported.")]
    public sealed class Application
    {
        public readonly ApplicationWebService service = new ApplicationWebService();

        /// <summary>
        /// This is a javascript application.
        /// </summary>
        /// <param name="page">HTML document rendered by the web server which can now be enhanced.</param>
        public Application(IApp page)
        {
            new ConsoleForm().InitializeConsoleFormWriter().Show();

            @"Hello world".ToDocumentTitle();
            // Send data from JavaScript to the server tier
            service.WebMethod2(
                @"A string from JavaScript.",
                value => value.ToDocumentTitle()
            );

            Native.Window.onmessage += e =>
            {
                Console.WriteLine("Window onmessage: " + new { e.data });
            };

            //dynamic w = new IFunction("return new Worker('/w');").apply(null);



            // E/Web Console(30665): Uncaught ReferenceError: Worker is not defined at http://192.168.1.101:6612/view-source:48124
            // does not exist on android webview!
            var w = new Worker("/view-source/w");

            //            onmessage: { data = hello from worker? { self = [object global], constructor = function DedicatedWorkerContext() { [native code] }, prototype = , href = http://192.168.1.100:3054/w } }
            // view-source:26922
            //onmessage: { data = mirror: { data = from app to worker  } }


            w.onmessage = IFunction.OfDelegate(
                new Action<MessageEvent>(
                    e =>
                    {
                        Console.WriteLine("onmessage: " + new { e.data });
                        // onmessage: { data = hello from worker? 1 }
                    }
                )
            );


            w.postMessage("from app to worker ");

        }

        // ApplicationWorker
        public sealed class w
        {
            public readonly ApplicationWebService service = new ApplicationWebService();

            // for now jsc is only looking for HTML based apps
            public w(IApp page = null)
            {
                // DedicatedWorkerContext
                var self = (DedicatedWorkerGlobalScope)(object)Native.Window;

                // Uncaught ReferenceError: window is not defined 
                Console.WriteLine("hello from worker!");

                var counter = 0;


                Action<string> postMessage =
                    x => new IFunction("x", "return postMessage(x);").apply(null, x); ;



                var w = Expando.Of(Native.Window);

                // onmessage: { data = hello from worker? { Window = [object global], constructor = function DedicatedWorkerContext() { [native code] }, prototype =  } }


                self.postMessage(
                    "hello from worker? " + new { self, w.constructor, w.prototype, self.location.href }
                );

                // Uncaught Error: responseXML was null: { responseXML = , responseText = <document><y><obj>goo</obj></y></document> } 
                // XML is unavailable!
                //service.WebMethod2("goo",
                //    y =>
                //    {

                //        self.postMessage(
                //            "hello from worker! " + new { y }
                //        );
                //    }
                //);


                self.onmessage = IFunction.OfDelegate(
                    new Action<MessageEvent>(
                        e =>
                        {
                            self.postMessage(
                                "mirror: " + new { e.data }
                            );
                        }
                    )
                );

                //foreach (var MemberName in w.GetMemberNames())
                //{
                //    postMessage(
                //   "global " + new { MemberName }
                //   );
                //}

                //Native.Window.postMessage("hello from worker?");
            }
        }
    }
}