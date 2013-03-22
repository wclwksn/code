using ScriptCoreLib;
using ScriptCoreLib.Delegates;
using ScriptCoreLib.Extensions;
using ScriptCoreLib.JavaScript;
using ScriptCoreLib.JavaScript.Components;
using ScriptCoreLib.JavaScript.DOM;
using ScriptCoreLib.JavaScript.DOM.HTML;
using ScriptCoreLib.JavaScript.Extensions;
using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using com.abstractatech.appmanager.Design;
using com.abstractatech.appmanager.HTML.Pages;
using com.abstractatech.appmanager.windows;
using System.Windows.Forms;

namespace com.abstractatech.appmanager
{
    /// <summary>
    /// Your client side code running inside a web browser as JavaScript.
    /// </summary>
    public sealed class Application
    {
        public readonly ApplicationWebService service = new ApplicationWebService();

        /// <summary>
        /// This is a javascript application.
        /// </summary>
        /// <param name="page">HTML document rendered by the web server which can now be enhanced.</param>
        public Application(IBeforeLogin page)
        {
            // http://stackoverflow.com/questions/2279978/webview-showing-white-bar-on-right-side
            // webview.setScrollBarStyle(WebView.SCROLLBARS_OUTSIDE_OVERLAY);



            page.LoginButton.onclick +=
                    delegate
                    {

                        // ask for credentials for new ui

                        var s = new IHTMLScript { src = "/a" };

                        // http://stackoverflow.com/questions/538745/how-to-tell-if-a-script-tag-failed-to-load
                        s.onload +=
                            delegate
                            {
                                page.LoginButton.Orphanize();
                            };

                        s.AttachToDocument();

                    };

            "App Mngr".ToDocumentTitle();
        }

        public sealed class a
        {
            public readonly ApplicationWebService service = new ApplicationWebService();


            public a(IBeforeLogin ee)
            {
                var page = new App();

                page.ScrollArea.AttachToDocument();

                var count = 0;

                #region yield
                yield_ACTION_MAIN yield = (
                            packageName,
                            name,
                            icon_base64,
                            label
                        ) =>
                {
                    count++;

                    var a = new AppPreview();

                    a.Icon.src = "data:image/png;base64," + icon_base64;
                    a.Label.innerText = label;

                    a.Container.AttachTo(page.ScrollArea);


                    a.Clickable.onclick +=
                        e =>
                        {
                            Console.WriteLine(new { label });
                            e.preventDefault();



                            var content = new ApplicationControl();


                            var f = new Form { Text = label };

                            f.ClientSize = content.Size;

                            f.Controls.Add(content);
                            content.Dock = DockStyle.Fill;

                            f.Show();


                            //f.PopupInsteadOfClosing();

                            content.Label.Text = label;
                            content.Package.Text = packageName;

                            a.Icon.cloneNode(true).AttachTo(

                                ScriptCoreLib.JavaScript.Windows.Forms.Extensions.GetHTMLTargetContainer(content.Icon)

                            );


                            content.NavigateTo.Click +=
                                delegate
                                {
                                    var uri = Native.Window.prompt("Where to?", "http://" + Native.Document.location.host + ":", label);

                                    if (string.IsNullOrEmpty(uri))
                                        return;

                                    var w = new WebBrowser();

                                    f.Controls.Add(w);
                                    w.Dock = DockStyle.Fill;
                                    w.Navigate(uri);

                                    f.ClientSize = content.Size;

                                };
                            content.Launch.Click +=
                                delegate
                                {
                                    // level 1
                                    // run on android

                                    // level 2
                                    // run as float

                                    // level 3 
                                    // run here as iframe

                                    // level 4
                                    // run here as js import

                                    service.Launch(
                                        packageName,
                                        name
                                    );

                                };

                            //f.Icon.toh
                        };
                };
                #endregion


                #region more
                var skip = 0;
                var take = 10;

                var getmore = "Scroll down for more...";

                new IHTMLButton { innerText = getmore }.AttachToDocument().With(
                  more =>
                  {
                      more.style.position = IStyle.PositionEnum.@fixed;
                      more.style.left = "2px";
                      more.style.bottom = "2px";

                      Action done = delegate { };


                      Action MoveNext = delegate
                      {
                          more.disabled = true;
                          more.innerText = "checking for more...";

                          Console.WriteLine("MoveNext: " + new { skip, take });

                          service.queryIntentActivities(
                              yield,
                              skip: "" + skip,
                              take: "" + take,
                              yield_done: done

                          );


                          //service.File_list("",
                          //    ydirectory: ydirectory,
                          //    yfile: yfile,
                          //    sskip: skip.ToString(),
                          //    stake: take.ToString(),
                          //    done: done
                          //);

                          skip += take;

                      };

                      done = delegate
                      {
                          more.innerText = getmore;
                          more.disabled = false;

                          if (count == skip)
                          {
                              Native.Document.body.With(
                                   body =>
                                   {
                                       if (more.disabled)
                                           return;

                                       if (body.scrollHeight - 1 <= Native.Window.Height + body.scrollTop)
                                       {
                                           MoveNext();
                                       }

                                   }
                             );
                          }
                      };



                      MoveNext();

                      more.onclick += delegate
                      {
                          MoveNext();
                      };



                      Native.Window.onscroll +=
                            e =>
                            {

                                Native.Document.body.With(
                                    body =>
                                    {
                                        if (more.disabled)
                                            return;

                                        if (body.scrollHeight - 1 <= Native.Window.Height + body.scrollTop)
                                        {
                                            MoveNext();
                                        }

                                    }
                              );

                            };
                  }
              );
                #endregion


            }
        }
    }
}