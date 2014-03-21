using jsgif;
using ScriptCoreLib;
using ScriptCoreLib.Delegates;
using ScriptCoreLib.Extensions;
using ScriptCoreLib.JavaScript;
using ScriptCoreLib.JavaScript.Components;
using ScriptCoreLib.JavaScript.DOM;
using ScriptCoreLib.JavaScript.DOM.HTML;
using ScriptCoreLib.JavaScript.Extensions;
using ScriptCoreLib.JavaScript.WebGL;
using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using WebGLSpiral.Shaders;
using WebGLToAnimatedGIFExperiment.Design;
using WebGLToAnimatedGIFExperiment.HTML.Images.FromAssets;
using WebGLToAnimatedGIFExperiment.HTML.Pages;
using System.Diagnostics;

namespace WebGLToAnimatedGIFExperiment
{
    using gl = ScriptCoreLib.JavaScript.WebGL.WebGLRenderingContext;
    using System.Collections.Generic;
    using System.Threading.Tasks;



    /// <summary>
    /// Your client side code running inside a web browser as JavaScript.
    /// </summary>
    public sealed class Application :
        // should jsc promote inheritance? would enable html component designer scenario
        //App.FromDocument,

        ISurface
    {
        //public readonly ApplicationWebService service = new ApplicationWebService();

        #region ISurface
        public event Action onframe;

        public event Action<int, int> onresize;

        public event Action<gl> onsurface;
        #endregion


        /// <summary>
        /// This is a javascript application.
        /// </summary>
        /// <param name="page">HTML document rendered by the web server which can now be enhanced.</param>
        public Application(IApp page)
        {
            //#region ChromeTCPServer
            //dynamic self = Native.self;
            //dynamic self_chrome = self.chrome;
            //object self_chrome_socket = self_chrome.socket;

            //if (self_chrome_socket != null)
            //{
            //    //chrome.Notification.DefaultIconUrl = new HTML.Images.FromAssets.Preview().src;
            //    chrome.Notification.DefaultTitle = "WebGLToAnimatedGIFExperiment";


            //    ChromeTCPServer.TheServerWithStyledForm.Invoke(
            //        AppSource.Text
            //    );

            //    return;
            //}
            //#endregion




            var ani6 = new WebGLEarthByBjorn.Application(null);

            ani6.canvas.AttachTo(page.e1);
            ani6.canvas.style.SetSize(96, 96);
            ani6.canvas.style.position = IStyle.PositionEnum.relative;



            var ani5 = new WebGLSVGAnonymous.Application(null);

            ani5.canvas.AttachTo(page.e1);

            // gif needs a bg?
            //ani4.canvas.style.backgroundColor = "yellow";

            ani5.canvas.style.SetSize(96, 96);
            ani5.canvas.style.position = IStyle.PositionEnum.relative;

            var ani4 = new WebGLColladaExperiment.Application(null);

            ani4.canvas.AttachTo(page.e1);

            // gif needs a bg?
            //ani4.canvas.style.backgroundColor = "yellow";

            ani4.canvas.style.SetSize(96, 96);
            ani4.canvas.style.position = IStyle.PositionEnum.relative;


            var ani3 = new WebGLTetrahedron.Application();

            ani3.gl.canvas.AttachTo(page.e1);

            // : ScriptComponent
            var ani2 = new WebGLEscherDrosteEffect.Application();

            ani2.gl.canvas.AttachTo(page.e1);





            var gl = new WebGLRenderingContext(alpha: false, preserveDrawingBuffer: true);

            // replace placeholder
            gl.canvas.id = page.canvas.id;
            page.canvas = gl.canvas;

            page.canvas.width = 96;
            page.canvas.height = 96;

            var s = new SpiralSurface(this);

            this.onsurface(gl);



            this.onresize(page.canvas.width, page.canvas.height);

            var st = new Stopwatch();
            st.Start();

            Native.window.onframe += delegate
            {
                s.ucolor_1 = (float)Math.Sin(st.ElapsedMilliseconds * 0.001) * 0.5f + 0.5f;

                this.onframe();
            };






            // jsc should link that js file once we reference it. for now its manual


            #region activate
            Action<IHTMLCanvas> activate =
                xcanvas =>
                {
                    var context = new { canvas = xcanvas };

                    context.canvas.style.border = "2px solid yellow";
                    context.canvas.style.cursor = IStyle.CursorEnum.pointer;

                    context.canvas.onclick +=
                         delegate
                         {
                             var c0 = new CanvasRenderingContext2D(96, 96);
                             c0.canvas.AttachToDocument();

                             var frames = new List<byte[]>();

                             // view-source:36524
                             //{ ToBase64String_while_timeout = 00:00:00.504, i = 911952, length = 1454096 } view-source:36524


                             //var framecount = 240;
                             //var delay = 1000 / 60;

                             var framecount = 16;
                             var delay = 1000 / 15;

                             new ScriptCoreLib.JavaScript.Runtime.Timer(
                                 async t =>
                                 {
                                     if (t.Counter == framecount)
                                     {
                                         Console.WriteLine("GIFEncoderWorker!");


                                         var src = await new GIFEncoderWorker(
                                                 96,
                                                 96,
                                                  delay: delay,
                                                 frames: frames
                                         );

                                         Console.WriteLine("done!");

                                         new IHTMLImage { src = src }.AttachToDocument();

                                         return;
                                     }

                                     if (t.Counter >= framecount)
                                     {
                                         c0.bytes = frames[t.Counter % frames.Count];

                                         return;
                                     }

                                     #region force redraw all
                                     s.ucolor_2 = t.Counter / 32.0f;

                                     // force redraw
                                     this.onframe();
                                     #endregion


                                     if (!t.IsAlive)
                                         return;

                                     c0.drawImage(context.canvas, 0, 0, 96, 96);

                                     frames.Add(c0.getImageData().data);



                                 }
                              ).StartInterval(1000 / 60);

                         };
                };
            #endregion


            activate(gl.canvas);
            activate(ani2.gl.canvas);
            activate(ani3.gl.canvas);
            activate(ani4.canvas);
            activate(ani5.canvas);
            activate(ani6.canvas);





        }

    }
}