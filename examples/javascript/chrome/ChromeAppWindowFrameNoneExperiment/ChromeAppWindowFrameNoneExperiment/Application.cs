using ChromeAppWindowFrameNoneExperiment.Design;
using ChromeAppWindowFrameNoneExperiment.HTML.Pages;
using ScriptCoreLib;
using ScriptCoreLib.Delegates;
using ScriptCoreLib.Extensions;
using ScriptCoreLib.JavaScript;
using ScriptCoreLib.JavaScript.BCLImplementation.System.Windows.Forms;
using ScriptCoreLib.JavaScript.Components;
using ScriptCoreLib.JavaScript.DOM;
using ScriptCoreLib.JavaScript.DOM.HTML;
using ScriptCoreLib.JavaScript.Extensions;
using ScriptCoreLib.JavaScript.Runtime;
using ScriptCoreLib.JavaScript.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace ChromeAppWindowFrameNoneExperiment
{
    /// <summary>
    /// Your client side code running inside a web browser as JavaScript.
    /// </summary>
    public sealed class Application
    {
        public readonly ApplicationWebService service = new ApplicationWebService();

        public readonly ApplicationControl content = new ApplicationControl();

        /// <summary>
        /// This is a javascript application.
        /// </summary>
        /// <param name="page">HTML document rendered by the web server which can now be enhanced.</param>
        public Application(IApp page)
        {
            //Console.WriteLine("Application loading...");

            #region do InternalHTMLTargetAttachToDocument
            if (Expando.InternalIsMember(Native.Window, "chrome"))
                if (chrome.app.runtime != null)
                {
                    // X:\jsc.svn\examples\javascript\chrome\ChromeAppWindowFrameNoneExperiment\ChromeAppWindowFrameNoneExperiment\Application.cs

                    //The JavaScript context calling chrome.app.window.current() has no associated AppWindow. 
                    //Console.WriteLine("appwindow loading... " + new { current = chrome.app.window.current() });
                    // no HTML layout yet

                    if (!(Native.Window.opener == null && Native.Window.parent == Native.Window.self))
                    {
                        Console.WriteLine("i am about:blank");
                        return;
                    }

                    //Console.WriteLine("Application wait for onLaunched");


                    chrome.runtime.onSuspend.addListener(
                        new Action(
                            delegate
                            {
                                Console.WriteLine("suspend!");

                            }
                        )
                    );


                    Action later = delegate { };

                    var windows = new List<AppWindow>();


                    #region InternalHTMLTargetAttachToDocument
                    Action<__Form, Action> InternalHTMLTargetAttachToDocument =
                       (that, yield) =>
                       {

                           //Error in event handler for app.runtime.onLaunched: Error: Invalid value for argument 2. Property 'transparentBackground': Expected 'boolean' but got 'integer'.
                           var transparentBackground = true;


                           // http://src.chromium.org/viewvc/chrome/trunk/src/chrome/common/extensions/api/app_window.idl
                           chrome.app.window.create(
                                 Native.Document.location.pathname,
                                 new
                                 {
                                     frame = "none"
                                     //,transparentBackground
                                 },
                                 new Action<AppWindow>(
                                     appwindow =>
                                     {
                                         // Uncaught TypeError: Cannot read property 'contentWindow' of undefined 

                                         Console.WriteLine("appwindow loading... " + new { appwindow });
                                         Console.WriteLine("appwindow loading... " + new { appwindow.contentWindow });


                                         appwindow.contentWindow.onload +=
                                             delegate
                                             {
                                                 var c = that;
                                                 var f = (Form)that;
                                                 var ff = c;

                                                 windows.Add(appwindow);

                                                 // http://sandipchitale.blogspot.com/2013/03/tip-webkit-app-region-css-property.html

                                                 (ff.CaptionForeground.style as dynamic).webkitAppRegion = "drag";

                                                 //(ff.ResizeGripElement.style as dynamic).webkitAppRegion = "drag";
                                                 // cant have it yet
                                                 ff.ResizeGripElement.Orphanize();

                                                 f.StartPosition = FormStartPosition.Manual;
                                                 f.MoveTo(0, 0);

                                                 f.FormClosing +=
                                                     delegate
                                                     {
                                                         Console.WriteLine("FormClosing");
                                                         appwindow.close();
                                                     };

                                                 appwindow.onRestored.addListener(
                                                       new Action(
                                                            delegate
                                                            {
                                                                that.CaptionShadow.Hide();

                                                            }
                                                       )
                                                    );

                                                 appwindow.onMaximized.addListener(
                                                    new Action(
                                                         delegate
                                                         {
                                                             that.CaptionShadow.Show();

                                                         }
                                                    )
                                                 );

                                                 appwindow.onClosed.addListener(
                                                     new Action(
                                                         delegate
                                                         {
                                                             Console.WriteLine("onClosed");
                                                             windows.Remove(appwindow);

                                                             f.Close();
                                                         }
                                                    )
                                                 );

                                                 // wont fire yet
                                                 appwindow.contentWindow.onbeforeunload +=
                                                     delegate
                                                     {
                                                         Console.WriteLine("onbeforeunload");
                                                     };

                                                 appwindow.contentWindow.onresize +=
                                                     //appwindow.onBoundsChanged.addListener(
                                                     //    new Action(
                                                         delegate
                                                         {
                                                             f.SizeTo(
                                                                 appwindow.contentWindow.Width,
                                                                  appwindow.contentWindow.Height
                                                             );
                                                         }
                                                     //)
                                                     //)
                                                 ;

                                                 f.SizeTo(
                                                       appwindow.contentWindow.Width,
                                                       appwindow.contentWindow.Height
                                                   );


                                                 //Console.WriteLine("appwindow contentWindow onload");


                                                 that.HTMLTarget.AttachTo(
                                                     appwindow.contentWindow.document.body
                                                 );

                                                 yield();
                                                 //Console.WriteLine("appwindow contentWindow onload done");
                                             };

                                         //Uncaught TypeError: Cannot read property 'contentWindow' of undefined 





                                     }
                                 )
                             );

                       };
                    #endregion


                    __Form.InternalHTMLTargetAttachToDocument =
                        (that, yield) =>
                        {
                            Console.WriteLine("Application wait for onLaunched for InternalHTMLTargetAttachToDocument");

                            later +=
                                delegate
                                {

                                    InternalHTMLTargetAttachToDocument(that, yield);
                                };
                        };


                    // why wait?
                    chrome.app.runtime.onLaunched.addListener(
                        new Action(
                            delegate
                            {
                                if (later == null)
                                {
                                    if (windows.Count == 0)
                                    {
                                        Console.WriteLine("chrome.runtime.reload");
                                        chrome.runtime.reload();
                                        return;
                                    }

                                    Console.WriteLine("drawAttention");
                                    windows.First().drawAttention();


                                    return;
                                }

                                Console.WriteLine("Application onLaunched!");
                                // signal any pending Show commands?

                                __Form.InternalHTMLTargetAttachToDocument = InternalHTMLTargetAttachToDocument;

                                later();
                                later = null;

                            }
                        )
                    );


                }
            #endregion


            //FormStyler.AtFormCreated = FormStylerLikeAero.LikeAero;
            //FormStyler.AtFormCreated = FormStylerLikeFloat.LikeFloat;
            //FormStyler.AtFormCreated = FormStyler.LikeWindows3;
            FormStyler.AtFormCreated = FormStyler.LikeVisualStudioMetro;

            var xf = new Form();
            content.BackColor = System.Drawing.Color.Transparent;
            xf.Controls.Add(content);
            xf.Show();

            //            The webpage at http://192.168.1.100:6669/ might be temporarily down or it may have moved permanently to a new web address.
            //Error code: ERR_UNSAFE_PORT
        }

    }
}