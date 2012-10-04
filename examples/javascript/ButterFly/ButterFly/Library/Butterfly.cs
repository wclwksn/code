using ScriptCoreLib.JavaScript;
using ScriptCoreLib.JavaScript.Extensions;
using ScriptCoreLib.JavaScript.DOM;
using ScriptCoreLib.JavaScript.DOM.HTML;
using ScriptCoreLib.JavaScript.Runtime;
using ScriptCoreLib;
using ScriptCoreLib.Shared;
using ScriptCoreLib.Shared.Drawing;
using System;
using System.Threading.Tasks;

namespace ButterFly.source.js
{


    public class Butterfly
    {
        IHTMLDiv Control = new IHTMLDiv();



        public Butterfly(IHTMLElement e)
        {


            e.insertNextSibling(Control);

            Control.appendChild(@"
There is a buttrfly under your mouse. 
Can you see it? :)
Click to capture pointer lock and see how the butterfly warps from left to right.
");

            Spawn(Control);


        }

        //async Task __buttryfly()
        //{ 
        //    //Task.Factory.
        //}

        //async 
            void Spawn(IHTMLElement e)
        {


            Native.Document.body.style.margin = "0px";
            Native.Document.body.style.padding = "0px";
            Native.Document.body.style.overflow = IStyle.OverflowEnum.hidden;

            e.style.position = IStyle.PositionEnum.absolute;
            e.style.left = "0px";
            e.style.top = "0px";
            e.style.right = "0px";
            e.style.bottom = "0px";

            e.style.backgroundColor = Color.FromRGB(209, 245, 245);

            IHTMLElement loading = new IHTMLElement(IHTMLElement.HTMLElementEnum.code, "loading...");

            loading.style.SetLocation(64, 64, 200, 64);

            e.appendChild(loading);

            //await __buttryfly();

            new global::ButterFly.HTML.Images.FromAssets.buttryfly().InvokeOnComplete(
                img =>
                {
                    loading.FadeOut();

                    try
                    {
                        //IStyleSheet.Default.AddRule("*", "cursor: none, url('" + new global::ButterFly.HTML.Images.FromAssets.nocursor().src + "'), auto;", 0);
                        IStyleSheet.Default.AddRule("*", "cursor: none;", 0);
                    }
                    catch (Exception exc)
                    {
                        new IHTMLElement(IHTMLElement.HTMLElementEnum.pre, exc.Message).AttachToDocument();
                    }


                    e.style.backgroundImage = "url(" + img.src + ")";
                    e.style.backgroundRepeat = "no-repeat";



                    e.DisableContextMenu();


                    var x = 0;
                    var y = 0;
                    e.onmousemove +=
                        delegate(IEvent i)
                        {



                            if (Native.Document.pointerLockElement == e)
                            {
                                x += i.movementX;
                                y += i.movementY;
                            }
                            else
                            {
                                x = i.CursorX;
                                y = i.CursorY;
                            }

                            if (x < -img.width / 2)
                                x += Native.Window.Width;

                            if (y < -img.height / 2)
                                y += Native.Window.Height;

                            x = x % Native.Window.Width;
                            y = y % Native.Window.Height;

                            e.style.backgroundPosition = x + "px " + y + "px";


                        };

                    e.onclick +=
                        delegate
                        {
                            e.requestPointerLock();
                        };
                });

        }

    }
}
