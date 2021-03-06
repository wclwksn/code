using ScriptCoreLib.ActionScript;
using ScriptCoreLib.ActionScript.Extensions;
using ScriptCoreLib.Extensions;
using ScriptCoreLib.Shared.BCLImplementation.GLSL;
using ScriptCoreLib.Shared.Lambda;
using starling.core;
using starling.display;
using starling.text;
using starling.textures;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using FlashHeatZeekerWithStarlingT18.ActionScript.Images;
using ScriptCoreLib.ActionScript.flash.geom;
using starling.filters;
using System.Xml.Linq;
using starling.utils;
using Box2D.Common.Math;
using Box2D.Dynamics;
using FlashHeatZeekerWithStarlingT18.Library;

namespace FlashHeatZeekerWithStarlingT18
{
    static class X
    {
        public static double GetLength(this __vec2 p)
        {
            return Math.Sqrt(p.x * p.x + p.y * p.y);
        }

        public static double DegreesToRadians(this double Degrees)
        {
            return (Math.PI * 2) * Degrees / 360;
        }

        public static double DegreesToRadians(this int Degrees)
        {
            return (Math.PI * 2) * Degrees / 360;
        }

        public static int RadiansToDegrees(this double Arc)
        {
            return (int)(360 * Arc / (Math.PI * 2));
        }

        public static double GetRotation(this __vec2 p)
        {
            var x = p.x;
            var y = p.y;

            const double _180 = System.Math.PI;
            const double _90 = System.Math.PI / 2;
            const double _270 = System.Math.PI * 3 / 2;

            if (x == 0)
                if (y < 0)
                    return _270;
                else if (y == 0)
                    return 0;
                else
                    return _90;

            if (y == 0)
                if (x < 0)
                    return _180;
                else
                    return 0;

            var a = System.Math.Atan(y / x);

            if (x < 0)
                a += _180;
            else if (y < 0)
                a += System.Math.PI * 2;


            return a;
        }
    }

    public class ApplicationSpriteContent : ScriptCoreLib.ActionScript.flash.display.Sprite
    {
        public ApplicationSpriteContent()
        {
            this.InvokeWhenStageIsReady(
                delegate
                {
                    __stage = this.stage;

                    // there can only be one in this VM. :)
                    __sprite = this;

                    var loc_b2stage = default(__b2debug_viewport);


                    this.stage.stage3Ds[0].context3DCreate +=
                        delegate
                        {
                            Console.WriteLine("context3DCreate");

                        };

                    // http://gamua.com/starling/first-steps/
                    // http://forum.starling-framework.org/topic/starling-air-desktop-extendeddesktop-fullscreen-issue
                    Starling.handleLostContext = true;

                    var s = new Starling(
                        typeof(Game).ToClassToken(),
                        this.stage
                    );


                    #region b2stage

                    Action b2stage_centerize = delegate
                    {
                        if (loc_b2stage == null)
                            return;

                        loc_b2stage.loc.x = ApplicationSprite.__stage.stageWidth * 0.5;
                        loc_b2stage.loc.y = ApplicationSprite.__stage.stageHeight * 0.7;
                    };

                    get_b2debug_viewport =
                        delegate
                        {
                            Console.WriteLine("b2stage");


                            if (loc_b2stage == null)
                            {
                                // http://doc.starling-framework.org/core/starling/core/Starling.html
                                // should use Native overlay ?

                                // http://www.ilike2flash.com/2012/04/as3-box2d-debug-mode-in-starling.html
                                // http://mariamdholkawala.com/mobile/?p=1051
                                //  A quick Google search revealed that you had to add the debug mode into a flash sprite so it can be added on top of the starling stage.

                                // our physics overlay

                                loc_b2stage = new __b2debug_viewport();

                                loc_b2stage.AtDispose +=
                                    delegate
                                    {
                                        ScriptCoreLib.ActionScript.Extensions.CommonExtensions.Orphanize(loc_b2stage.loc);

                                        loc_b2stage = null;
                                    };

                                loc_b2stage.loc = new ScriptCoreLib.ActionScript.flash.display.Sprite().AttachTo(s.nativeOverlay);
                                loc_b2stage.rot = new ScriptCoreLib.ActionScript.flash.display.Sprite().AttachTo(loc_b2stage.loc);

                                // can be redrawn or cleared!
                                loc_b2stage.content = new ScriptCoreLib.ActionScript.flash.display.Sprite().AttachTo(loc_b2stage.rot);
                                loc_b2stage.content_layer0 = new ScriptCoreLib.ActionScript.flash.display.Sprite().AttachTo(loc_b2stage.content);

                                loc_b2stage.content.graphics.beginFill(0xff);
                                // ae we on top of the starling?
                                loc_b2stage.content.graphics.drawRect(0, 0, 100, 100);
                                loc_b2stage.content.alpha = 0.5;

                                b2stage_centerize();

                            }

                            return loc_b2stage;
                        };
                    #endregion


                    s.start();

                    #region resize
                    // http://forum.starling-framework.org/topic/starling-stage-resizing
                    this.stage.resize += delegate
                    {
                        // http://forum.starling-framework.org/topic/starling-stage-resizing

                        s.viewPort = new ScriptCoreLib.ActionScript.flash.geom.Rectangle(
                            0, 0, this.stage.stageWidth, this.stage.stageHeight
                        );

                        s.stage.stageWidth = this.stage.stageWidth;
                        s.stage.stageHeight = this.stage.stageHeight;


                        b2stage_centerize();
                    };

                    s.viewPort = new ScriptCoreLib.ActionScript.flash.geom.Rectangle(
                         0, 0, this.stage.stageWidth, this.stage.stageHeight
                     );

                    s.stage.stageWidth = this.stage.stageWidth;
                    s.stage.stageHeight = this.stage.stageHeight;
                    #endregion


                    this.stage.keyUp +=
                        e =>
                        {
                            if (e.keyCode == (uint)System.Windows.Forms.Keys.F11)
                            {
                                this.stage.displayState = ScriptCoreLib.ActionScript.flash.display.StageDisplayState.FULL_SCREEN_INTERACTIVE;
                                //this.stage.SetFullscreen(true);
                            }
                        };
                }
            );


        }

        public Action<string> __raise_fps = delegate { };
        public Action<IRemoteGame> __raise_context_new_remotegame = delegate { };
        public Action<XElement> __context_postMessage = delegate { };
        public Action<XElement> __game_onmessage = delegate { };
        public Action<XElement> __game_postMessage = delegate { };

        public static Func<__b2debug_viewport> get_b2debug_viewport;


        public static ApplicationSpriteContent __sprite;
        public static ScriptCoreLib.ActionScript.flash.display.Stage __stage;


    }

    public class __b2debug_viewport
    {
        // box2d thinks in metric while we think in pixels
        // this is a scale magic to keep both sides happy and functional
        public const double b2scale = 16;

        public ScriptCoreLib.ActionScript.flash.display.Sprite loc;
        public ScriptCoreLib.ActionScript.flash.display.Sprite rot;
        public ScriptCoreLib.ActionScript.flash.display.Sprite content;
        public ScriptCoreLib.ActionScript.flash.display.Sprite content_layer0;

        public event Action AtDispose;
        public void Dispose()
        {
            if (AtDispose != null)
                AtDispose();
        }
    }


    // HD
    [SWF(frameRate = 120, width = 1280, height = 720)]
    public sealed class ApplicationSprite : ApplicationSpriteContent
    {
        public ApplicationSprite()
        {
            this.__raise_fps = this.raise_fps;
            this.__raise_context_new_remotegame = this.raise_context_new_remotegame;
            this.__context_postMessage = this.context_postMessage;
            this.game_onmessage += e => this.__game_onmessage(e);
            this.__game_postMessage = this.game_postMessage;

            #region AtInitializeConsoleFormWriter

            var w = new __OutWriter();
            var o = Console.Out;
            var __reentry = false;

            var __buffer = new StringBuilder();

            w.AtWrite =
                x =>
                {
                    __buffer.Append(x);
                };

            w.AtWriteLine =
                x =>
                {
                    __buffer.AppendLine(x);
                };

            Console.SetOut(w);

            this.AtInitializeConsoleFormWriter = (
                Action<string> Console_Write,
                Action<string> Console_WriteLine
            ) =>
            {

                try
                {


                    w.AtWrite =
                        x =>
                        {
                            o.Write(x);

                            if (!__reentry)
                            {
                                __reentry = true;
                                Console_Write(x);
                                __reentry = false;
                            }
                        };

                    w.AtWriteLine =
                        x =>
                        {
                            o.WriteLine(x);

                            if (!__reentry)
                            {
                                __reentry = true;
                                Console_WriteLine(x);
                                __reentry = false;
                            }
                        };

                    Console.WriteLine("flash Console.WriteLine should now appear in JavaScript form!");
                    Console.WriteLine(__buffer.ToString());
                }
                catch
                {

                }
            };
            #endregion

        }

        public event Action<string> fps;

        public void raise_fps(string e)
        {
            if (fps != null)
                fps(e);
        }

        Action<Action<string>, Action<string>> AtInitializeConsoleFormWriter;


        #region InitializeConsoleFormWriter
        class __OutWriter : TextWriter
        {
            public Action<string> AtWrite;
            public Action<string> AtWriteLine;

            public override void Write(string value)
            {
                AtWrite(value);
            }

            public override void WriteLine(string value)
            {
                AtWriteLine(value);
            }

            public override Encoding Encoding
            {
                get { return Encoding.UTF8; }
            }
        }

        public void InitializeConsoleFormWriter(
            Action<string> Console_Write,
            Action<string> Console_WriteLine
        )
        {
            AtInitializeConsoleFormWriter(Console_Write, Console_WriteLine);
        }
        #endregion



        public event Action<XElement> context_onmessage;
        public void context_postMessage(XElement e)
        {
            if (context_onmessage != null)
                context_onmessage(e);
        }

        public event Action<IRemoteGame> context_new_remotegame;
        public void raise_context_new_remotegame(IRemoteGame e)
        {
            if (context_new_remotegame != null)
                context_new_remotegame(e);
        }



        public event Action<XElement> game_onmessage;
        public void game_postMessage(XElement e)
        {
            if (game_onmessage != null)
                game_onmessage(e);
        }
    }

    // for bullets?
    class KineticEnergy
    {
        public DisplayObject Target;

        public __vec2 Energy;

        public int TTL;
    }


    static class StarlingExtensions
    {
        public static T AttachTo<T>(this T e, DisplayObjectContainer x) where T : DisplayObject
        {
            if (e == null)
                return e;

            x.addChild(e);

            return e;
        }

        public static T Orphanize<T>(this T e) where T : DisplayObject
        {
            if (e != null)
                if (e.parent != null)
                    e.removeFromParent();


            return e;
        }

        public static T MoveTo<T>(this T e, double x, double y) where T : DisplayObject
        {
            if (e == null)
                return e;

            e.x = x;
            e.y = y;


            return e;
        }


    }


    class GameMap
    {
        //    BBB GGG
        //  CCC AAA FFF
        //    DDD EEE

        public Sprite ground = new Sprite();

        public Image ground_color;

        public Action<DisplayObject> rtex_draw;
        public RenderTexture rtex;
        public Image ground_tracks;


        public GameMap()
        {
            Console.WriteLine("GameMap ctor");
            var bmd = new ScriptCoreLib.ActionScript.flash.display.BitmapData(4, 4, false, 0xB27D51);
            //var bmd = new ScriptCoreLib.ActionScript.flash.display.BitmapData(2048, 2048, false, 0xB27D51);

            //var offset = new Matrix();

            //offset.translate(96, 0);
            //bmd.draw(new white_jsc(), offset);

            // http://doc.starling-framework.org/core/starling/textures/RenderTexture.html
            // Beware that render textures can't be restored when the Starling's render context is lost.
            var tex = RenderTexture.fromBitmapData(bmd);


            // does not work??
            //var tex = Texture.fromColor(4, 4, 0xffB27D51u, false, 512);
            this.ground_color = new Image(tex).AttachTo(ground);
            this.ground_color.scaleX = 512;
            this.ground_color.scaleY = 512;

            //        Error: Error #3691: Resource limit for this resource type exceeded.
            //at flash.display3D::Context3D/createTexture()
            //at starling.textures::Texture$/empty()[Y:\opensource\github.com\Starling-Framework\src\starling\textures\Texture.as:259]
            //at starling.textures::RenderTexture()[Y:\opensource\github.com\Starling-Framework\src\starling\textures\RenderTexture.as:91]
            //at FlashHeatZeekerWithStarlingT18::GameMap()[T:\web\FlashHeatZeekerWithStarlingT18\GameMap.as:33]
            //at FlashHeatZeekerWithStarlingT18::Game()[T:\web\FlashHeatZeekerWithStarlingT18\Game.as:181]
            //at starling.core::Starling/initializeRoot()[Y:\opensource\github.com\Starling-Framework\src\starling\core\Starling.as:338]
            //at starling.core::Starling/initialize()[Y:\opensource\github.com\Starling-Framework\src\starling\core\Starling.as:314]
            //at starling.core::Starling/onContextCreated()[Y:\opensource\github.com\Starling-Framework\src\starling\core\Starling.as:519]


            // http://forum.starling-framework.org/topic/resource-limit-for-this-resource-type-exceeded
            this.rtex = new RenderTexture(512, 512, true, 1);
            this.ground_tracks = new Image(rtex).AttachTo(ground);

            var rtex_draw_commands = new List<DisplayObject>();

            this.rtex_draw = e =>
            {
                rtex_draw_commands.Add(e);

                rtex.draw(e);
            };



            var skipone = 0;

            ApplicationSprite.__stage.stage3Ds[0].context3DCreate +=
                delegate
                {
                    skipone++;

                    if (skipone == 1)
                        return;

                    var sw = new Stopwatch();
                    sw.Start();

                    Console.WriteLine("will redraw " + new { rtex_draw_commands.Count });

                    // tested at X:\jsc.svn\examples\javascript\StarlingRenderTextureExperiment\StarlingRenderTextureExperiment\ApplicationSprite.cs

                    this.rtex = new RenderTexture(512, 512, true, 1);
                    this.ground_tracks.texture = this.rtex;


                    this.rtex.drawBundled(
                        new Action(
                            delegate
                            {
                                rtex_draw_commands.WithEach(e => rtex.draw(e));
                            }
                        ).ToFunction()
                    );

                    sw.Stop();
                    Console.WriteLine("will redraw " + new { rtex_draw_commands.Count } + " done in " + sw.ElapsedMilliseconds + "ms");

                };

            var logo = new Image(Game.LogoTexture);
            logo.scaleX = 0.1;
            logo.scaleY = 0.1;

            rtex_draw(logo);



            this.ground_tracks.scaleX = 4;
            this.ground_tracks.scaleY = 4;
            //350 MB is the absolute limit for textures, including the texture memory required for mipmaps. However, many devices do not support this much texture memory. For maximum compatibility, limit texture memory use to 128 MB, or less.


            //offset.translate(96, 0);



            //// http://forum.starling-framework.org/topic/starling-drawing-api-and-animations

            //tex.up

            Console.WriteLine("GameMap ctor done");
        }

        public List<DisplayObject> doodads = new List<DisplayObject>();
        public List<GameUnit> static_units = new List<GameUnit>();
        public string Name;


        public Point Location = new Point();
        public List<Point> VirtualLocations = new List<Point>();

        public bool hitTest(double hx, double hy)
        {
            var cx = ground.x + 2048;
            var cy = ground.y + 2048;

            //Console.WriteLine("hitTest " + new { this.ground.x, this.ground.y, cx, cy, hx, hy });


            //allow_unit_teleport, hittesting... { x = 880.6350114443436, y = -2026.816097834212 }
            //hitTest { x = 0, y = -4096, cx = 2048, cy = -2048, hx = 880.6350114443436, hy = -2026.816097834212 }
            //hy > cy where { hy = -2026.816097834212, cy = -2048 }
            //will NOT teleport
            //map teleport done in 1ms
            //allow_unit_teleport, hittesting... { x = 880.6350114443436, y = -2026.816097834212 }
            //hitTest { x = -2048, y = -4096, cx = 0, cy = -2048, hx = 880.6350114443436, hy = -2026.816097834212 }
            //hx > cx where { hx = 880.6350114443436, cx = 0 }
            //will NOT teleport



            if (hx < ground.x)
            {
                //Console.WriteLine("hx < ground.x where " + new { hx, ground.x });
                return false;
            }

            if (hy < ground.y)
            {
                //Console.WriteLine("hy < ground.y where " + new { hy, ground.y });
                return false;
            }

            if (hx >= cx)
            {
                //Console.WriteLine("hx > cx where " + new { hx, cx });
                return false;
            }

            if (hy >= cy)
            {
                //Console.WriteLine("hy > cy where " + new { hy, cy });
                return false;
            }

            return true;

        }
    }


    class GameUnit
    {
        public bool isjeepengine;

        public Func<double, double> zoomer_default = y => 1 + (1 - y) * 0.2;


        public double move_forward = 0.0;
        public double move_backward = 0.0;

        public double rot_left = 0.0;
        public double rot_right = 0.0;


        public Sprite loc;
        public Sprite rot;

        public Image shape;

        // hit F2 to see the box2d physics
        public Car physics;

        public b2Body physics_body;

        // special shadow if any
        public DisplayObject shadow_loc;
        public Sprite shadow_rot;

        public __vec2 prevframe_loc = new __vec2();
        public double prevframe_rot = 0;

        public Queue<Sprite> tracks = new Queue<Sprite>();

        public double rotation
        {
            get { return this.rot.rotation; }
            set
            {
                this.rot.rotation = value;

                if (this.physics_body != null)
                    this.physics_body.SetAngle(value);

                if (this.shadow_rot != null)
                    this.shadow_rot.rotation = value;
            }
        }

        // make th unit look like team lead
        public Action AddRank;

        public Action<double> ScrollTracks = delegate { };

        public DisplayObject guntower;
        public bool RemoteControlEnabled;

        public Action RenewTracks = delegate { };

        // not all units can be manned.
        public DriverSeat driverseat;

        // pedesterians can man the driverseat
        public bool isdriver;

        public class DriverSeat
        {
            public GameUnit driver;
        }

        public GameUnit TeleportTo(GameUnit r, double dx, double dy)
        {
            TeleportTo(
                r.loc.x - dx,
                r.loc.y - dy
            );




            return this;
        }

        public GameUnit TeleportTo(double dx, double dy)
        {
            if (this.physics_body != null)
            {
                this.physics_body.SetPosition(
                    new b2Vec2(
                        (dx) / __b2debug_viewport.b2scale,
                        (dy) / __b2debug_viewport.b2scale
                    )
                );


            }

            this.loc.x = (dx);
            this.loc.y = (dy);

            if (shadow_loc != null)
            {
                shadow_loc.x = this.loc.x;
                shadow_loc.y = this.loc.y;
            }

            return this;
        }

        public void TeleportBy(double dx, double dy)
        {
            if (this.physics_body != null)
            {

                this.physics_body.GetPosition().With(
                    pp =>
                    {

                        this.physics_body.SetPosition(
                            new b2Vec2(
                                pp.x + dx / __b2debug_viewport.b2scale,
                               pp.y + dy / __b2debug_viewport.b2scale
                            )
                        );
                    }
                );
            }

            if (this.physics != null)
            {

                this.physics.body.GetPosition().With(
                    pp =>
                    {

                        this.physics.body.SetPosition(
                            new b2Vec2(
                                pp.x + dx / __b2debug_viewport.b2scale,
                               pp.y + dy / __b2debug_viewport.b2scale
                            )
                        );
                    }
                );

                this.physics.wheels.WithEach(
                    w =>
                    {
                        w.body.GetPosition().With(
                           pp =>
                           {

                               w.body.SetPosition(
                                   new b2Vec2(
                                       pp.x + dx / __b2debug_viewport.b2scale,
                                      pp.y + dy / __b2debug_viewport.b2scale
                                   )
                               );
                           }
                       );
                    }
                );

            }


            this.loc.x += dx;
            this.loc.y += dy;



        }
    }

    public class Game : Sprite
    {
        public static Texture LogoTexture;

        public Game()
        {
            var r = new Random();
            var networkid = r.Next();

            var __bmd = new ScriptCoreLib.ActionScript.flash.display.BitmapData(96, 96, true, 0x00000000);
            __bmd.draw(new white_jsc());
            LogoTexture = Texture.fromBitmapData(__bmd);

            #region screen bg
            {
                var bmd = new ScriptCoreLib.ActionScript.flash.display.BitmapData(4, 4, false, 0xA26D41);
                var tex = Texture.fromBitmapData(bmd);


                var img = new Image(tex);

                img.scaleX = 512;
                img.scaleY = 512;
                addChild(img);
            }
            #endregion

            var viewport_loc = new Sprite().AttachTo(this);
            var viewport_rot = new Sprite().AttachTo(viewport_loc);
            var viewport_content = new Sprite().AttachTo(viewport_rot);

            var viewport_content_layer0_ground = new Sprite().AttachTo(viewport_content);
            var viewport_content_layer1_tracks = new Sprite().AttachTo(viewport_content);
            var viewport_content_layer2_units = new Sprite().AttachTo(viewport_content);

            var viewport_content_layer3_trees_shadow = new Sprite().AttachTo(viewport_content);
            var viewport_content_layer3_trees = new Sprite().AttachTo(viewport_content);

            var viewport_content_layer4_clouds = new Sprite().AttachTo(viewport_content);

            viewport_rot.scaleX = 2.0;
            viewport_rot.scaleY = 2.0;

            var mapA = new GameMap { Name = "mapA" };

            // our map A
            {
                // ArgumentError: Error #3683: Texture too big (max is 2048x2048).
                //var bmd = new ScriptCoreLib.ActionScript.flash.display.BitmapData(2048, 2048, false, 0xB27D51);

                //var tex = Texture.fromBitmapData(bmd);
                //var img = new Image(tex);

                mapA.ground.AttachTo(viewport_content_layer0_ground);
            }

            //var mapB_offset_x = -2048 / 2;
            //var mapB_offset_y = -2048;
            var mapB = new GameMap { Name = "mapB" };

            mapB.Location.x = -2048 / 2;
            mapB.Location.y = -2048;

            mapB.VirtualLocations.Add(new Point(-2048, 2048 * 2));
            mapB.VirtualLocations.Add(new Point(2048 * 1.5, 2048));



            // our map B
            {
                // ArgumentError: Error #3683: Texture too big (max is 2048x2048).
                //var bmd = new ScriptCoreLib.ActionScript.flash.display.BitmapData(2048, 2048, false, 0xB27D51);


                mapB.ground.AttachTo(viewport_content_layer0_ground);
                mapB.ground.MoveTo(mapB.Location.x, mapB.Location.y);
            }









            var mapC = new GameMap { Name = "mapC" };
            mapC.Location = new Point(-2048, 0);



            mapC.VirtualLocations.Add(new Point(2048, 2048 * 2));
            mapC.VirtualLocations.Add(new Point(2048 * 1.5, -2048));
            // our map C
            {
                mapC.ground.AttachTo(viewport_content_layer0_ground);
                mapC.ground.MoveTo(mapC.Location.x, mapC.Location.y);
            }


            var mapD = new GameMap { Name = "mapD" };

            mapD.Location = new Point(-2048 / 2, 2048);
            mapD.VirtualLocations.Add(new Point(0, -2048 * 2));
            mapD.VirtualLocations.Add(new Point(2048 * 2, 0));




            // our map C
            {
                mapD.ground.AttachTo(viewport_content_layer0_ground);
                mapD.ground.MoveTo(mapD.Location.x, mapD.Location.y);
            }


            var mapE = new GameMap { Name = "mapE" };

            mapE.Location = new Point(2048 / 2, 2048);
            mapE.VirtualLocations.Add(new Point(2048 * -1.5, -2048));
            mapE.VirtualLocations.Add(new Point(2048, -2048 * 2));


            // our map C
            {
                mapE.ground.AttachTo(viewport_content_layer0_ground);
                mapE.ground.MoveTo(mapE.Location.x, mapE.Location.y);
            }

            var mapF = new GameMap { Name = "mapF" };

            mapF.Location = new Point(2048, 0);
            mapF.VirtualLocations.Add(new Point(-2048, -2048 * 2));
            mapF.VirtualLocations.Add(new Point(-2048 * 1.5, 2048));


            {
                mapF.ground.AttachTo(viewport_content_layer0_ground);
                mapF.ground.MoveTo(mapF.Location.x, mapF.Location.y);
            }

            var mapG = new GameMap { Name = "mapG" };


            mapG.Location = new Point(2048 / 2, -2048);
            mapG.VirtualLocations.Add(new Point(0, 2048 * 2));
            mapG.VirtualLocations.Add(new Point(2048 * -2, 0));




            {
                mapG.ground.AttachTo(viewport_content_layer0_ground);
                mapG.ground.MoveTo(mapG.Location.x, mapG.Location.y);
            }


            var maps = new[] { mapA, mapB, mapC, mapD, mapE, mapF, mapG };


            #region new_tex_400
            Func<string, Texture> new_tex_400 =
               asset =>
               {
                   var shape = KnownEmbeddedResources.Default[asset].ToSprite();
                   var bmd = new ScriptCoreLib.ActionScript.flash.display.BitmapData(400, 400, true, 0x00000000);
                   bmd.draw(shape);
                   return Texture.fromBitmapData(bmd);
               };
            #endregion

            #region new_tex_512
            Func<string, Texture> new_tex_512 =
               asset =>
               {
                   var shape = KnownEmbeddedResources.Default[asset].ToSprite();


                   var bmd = new ScriptCoreLib.ActionScript.flash.display.BitmapData(512, 512, true, 0x00000000);

                   var m = new Matrix();
                   m.scale(512 / 400.0, 512 / 400.0);

                   bmd.draw(shape, m);
                   return Texture.fromBitmapData(bmd);
               };
            #endregion




            var textures_road0 = new_tex_512("assets/FlashHeatZeekerWithStarlingT18/road0.svg");
            var textures_touchdown = new_tex_512("assets/FlashHeatZeekerWithStarlingT18/touchdown.svg");

            {
                var img = new Image(textures_touchdown);
                img.x = 128;
                img.y = 128;

                img.scaleX = 0.5;
                img.scaleY = 0.5;

                img.AttachTo(viewport_content_layer1_tracks);
            }








            var textures_hill0 = new_tex_512("assets/FlashHeatZeekerWithStarlingT18/hill0.svg");

            {
                var img = new Image(textures_hill0);
                img.x = 400;
                img.y = 400;

                img.scaleX = 0.4;
                img.scaleY = 0.4;

                img.AttachTo(viewport_content_layer1_tracks);
            }

            var textures_hill1 = new_tex_512("assets/FlashHeatZeekerWithStarlingT18/hill1.svg");

            {
                var img = new Image(textures_hill1);
                img.x = 800;
                img.y = 400;

                img.scaleX = 0.4;
                img.scaleY = 0.4;

                img.AttachTo(viewport_content_layer1_tracks);
            }

            var textures_watertower0 = new_tex_512("assets/FlashHeatZeekerWithStarlingT18/watertower0.svg");

            var textures_tree0 = new_tex_512("assets/FlashHeatZeekerWithStarlingT18/tree0.svg");
            var textures_tree0_shadow = new_tex_512("assets/FlashHeatZeekerWithStarlingT18/tree0_shadow.svg");


            #region new_tree
            Func<GameUnit> new_tree =
                delegate
                {
                    var scale = 0.12 + 0.2 * (1 - r.NextDouble() * r.NextDouble());


                    var unit_loc = new Sprite().AttachTo(viewport_content_layer3_trees);
                    {
                        var unit_scale = new Sprite().AttachTo(unit_loc);
                        var img = new Image(textures_tree0);
                        img.x = -256;
                        img.y = -256;

                        unit_scale.scaleX = scale;
                        unit_scale.scaleY = scale;

                        img.AttachTo(unit_scale);
                    }

                    var shadow_loc = new Sprite().AttachTo(viewport_content_layer3_trees_shadow);
                    {
                        var unit_scale = new Sprite().AttachTo(shadow_loc);
                        var img = new Image(textures_tree0_shadow);
                        img.x = -256;
                        img.y = -256;

                        unit_scale.scaleX = scale;
                        unit_scale.scaleY = scale;

                        img.AttachTo(unit_scale);
                        img.alpha = 0.7;
                    }

                    return new GameUnit
                    {

                        loc = unit_loc,
                        shadow_loc = shadow_loc

                    };
                };
            #endregion

            #region new_hill1
            Func<Sprite> new_hill1 =
                delegate
                {
                    var unit_loc = new Sprite().AttachTo(viewport_content_layer1_tracks);
                    var unit_scale = new Sprite().AttachTo(unit_loc);
                    var img = new Image(textures_hill1);
                    img.x = -256;
                    img.y = -256;

                    unit_scale.scaleX = 0.15;
                    unit_scale.scaleY = 0.15;

                    img.AttachTo(unit_scale);

                    return unit_loc;
                };
            #endregion


            #region new_watertower
            Func<Sprite> new_watertower =
                delegate
                {
                    var unit_loc = new Sprite().AttachTo(viewport_content_layer3_trees);
                    var unit_scale = new Sprite().AttachTo(unit_loc);
                    var img = new Image(textures_watertower0);
                    img.x = -256;
                    img.y = -256;

                    unit_scale.scaleX = 0.3;
                    unit_scale.scaleY = 0.3;

                    img.AttachTo(unit_scale);

                    return unit_loc;
                };
            #endregion



            new_tree().TeleportTo(128 * 1, 0);
            //new_tree().MoveTo(128 * 2, 0);
            //new_tree().MoveTo(128 * 3, 0);
            new_tree().TeleportTo(128 * 4, 0);
            //new_tree().MoveTo(128 * 5, 0);
            //new_tree().MoveTo(128 * 6, 0);
            new_tree().TeleportTo(128 * 7, 0);






            #region new_road
            Func<GameUnit> new_road = delegate
            {
                var _loc = new Sprite().AttachTo(viewport_content_layer1_tracks);
                var _rot = new Sprite().AttachTo(_loc);
                var img = new Image(textures_road0);
                img.x = -256;
                img.y = -256;
                _rot.scaleX = 0.5;
                _rot.scaleY = 0.5;

                img.AttachTo(_rot);

                return new GameUnit { loc = _loc, rot = _rot };
            };
            #endregion

            #region build_vroad
            Action<double, double, GameMap> build_vroad = (mapx, mapy, map) =>
            {
                for (int i = 0; i < 2048; i += 256)
                {
                    var rr = new_road();

                    rr.rotation = 90.DegreesToRadians();

                    var rrx = mapx + 2024 - 128;
                    var rry = mapy + 128 + (i);

                    rr.loc.MoveTo(rrx, rry);

                    if (map != null)
                        map.doodads.Add(rr.loc);
                }
            };
            #endregion

            #region build_yroad
            Action<double, double, GameMap> build_yroad = (mapx, mapy, map) =>
            {
                for (int i = 0; i < 2048; i += 256)
                {
                    var rr = new_road();

                    rr.rotation = 90.DegreesToRadians();

                    var rrx = mapx + 2024 - 128 - i / 2;
                    var rry = mapy + 128 + (i);

                    rr.loc.MoveTo(rrx, rry);

                    if (map != null)
                        map.doodads.Add(rr.loc);
                }
            };
            #endregion


            #region build_hroad
            Action<double, double, GameMap> build_hroad = (mapx, mapy, map) =>
            {
                for (int i = 0; i < 2048; i += 256)
                {
                    var rr = new_road();

                    //rr.rotation = 90.DegreesToRadians();

                    var rry = mapy + 2024 - 128;
                    var rrx = mapx + 128 + (i);

                    rr.loc.MoveTo(rrx, rry);

                    if (map != null)
                        map.doodads.Add(rr.loc);
                }
            };
            #endregion


            #region fill_empty_map
            Action<double, double, GameMap> fill_empty_map =
                (mapx, mapy, map) =>
                {

                    #region map c grow trees in the center
                    for (int i = 0; i < 256; i++)
                    {


                        var tree = new_tree().TeleportTo(
                           mapx + r.NextDouble() * (2048 - 512) + 128,
                           mapy + r.NextDouble() * (2048 - 512) + 128
                           );

                        if (map != null)
                        {
                            map.static_units.Add(tree);
                        }

                    }
                    #endregion

                    #region paint hills


                    map.rtex.drawBundled(
                        new Action(
                            delegate
                            {
                                for (int i = 0; i < 64; i++)
                                {
                                    var img = new Image(textures_hill1);

                                    img.scaleX = 0.15 / 4.0;
                                    img.scaleY = 0.15 / 4.0;

                                    var hill = img.MoveTo(
                                        //mapx + (r.NextDouble() * 0.8 + 0.1) * 2048,
                                        //mapy + (r.NextDouble() * 0.8 + 0.1) * 2048

                                        0 + (r.NextDouble() * 0.8 + 0.1) * 512,
                                        0 + (r.NextDouble() * 0.8 + 0.1) * 512
                                     );

                                    map.rtex_draw(hill);
                                }
                            }
                        ).ToFunction()
                    );
                    #endregion


                };
            #endregion


            fill_empty_map(mapB.Location.x, mapB.Location.y, mapB);
            build_vroad(mapB.Location.x, mapB.Location.y, mapB);
            build_hroad(mapB.Location.x, mapB.Location.y, mapB);

            fill_empty_map(mapC.Location.x, mapC.Location.y, mapC);

            fill_empty_map(mapD.Location.x, mapD.Location.y, mapD);
            build_yroad(mapD.Location.x, mapD.Location.y, mapD);


            fill_empty_map(mapE.Location.x, mapE.Location.y, mapE);
            fill_empty_map(mapF.Location.x, mapF.Location.y, mapF);
            fill_empty_map(mapG.Location.x, mapG.Location.y, mapG);

            #region pin_draw
            Action<DisplayObject> pin_draw = e =>
            {
                // should draw on neighbours too?
                maps.Where(k => k.hitTest(e.x, e.y)).WithEach(
                    k =>
                    {
                        // give it a new offset and scale

                        e.x = (e.x - k.ground.x) / 4.0;
                        e.y = (e.y - k.ground.y) / 4.0;

                        e.scaleX = 1 / 4.0;
                        e.scaleY = 1 / 4.0;


                        k.rtex_draw(e);
                    }
                );
            };
            #endregion

            #region pin_doodad
            Action<DisplayObject> pin_doodad = cloc =>
                {
                    maps.Where(k => k.hitTest(cloc.x, cloc.y)).WithEach(
                        k =>
                        {
                            k.doodads.Add(cloc);
                        }
                    );
                };
            #endregion


            pin_doodad(new_watertower().MoveTo(2048 / 2, 0));
            pin_doodad(new_watertower().MoveTo(mapB.Location.x, mapB.Location.y));
            pin_doodad(new_watertower().MoveTo(mapB.Location.x + 2048, mapB.Location.y));
            pin_doodad(new_watertower().MoveTo(mapB.Location.x, mapB.Location.y + 2048));

            pin_doodad(new_watertower().MoveTo(400, 800));
            pin_doodad(new_watertower());

            var textures_tanktrackpattern = new_tex_512("assets/FlashHeatZeekerWithStarlingT18/tanktrackpattern.svg");

            #region pin_letter
            Action<string, double, double> pin_letter =
                (texasset, texx, texy) =>
                {
                    new Image(new_tex_512(texasset)).With(
                       img =>
                       {
                           img.MoveTo(texx + 512, texy + 512);

                           pin_draw(img);
                       }
                   );
                };
            #endregion


            pin_letter("assets/FlashHeatZeekerWithStarlingT18/letterB.svg", mapB.Location.x, mapB.Location.y);
            pin_letter("assets/FlashHeatZeekerWithStarlingT18/letterC.svg", mapC.Location.x, mapC.Location.y);
            pin_letter("assets/FlashHeatZeekerWithStarlingT18/letterD.svg", mapD.Location.x, mapD.Location.y);
            pin_letter("assets/FlashHeatZeekerWithStarlingT18/letterE.svg", mapE.Location.x, mapE.Location.y);
            pin_letter("assets/FlashHeatZeekerWithStarlingT18/letterF.svg", mapF.Location.x, mapF.Location.y);
            pin_letter("assets/FlashHeatZeekerWithStarlingT18/letterG.svg", mapG.Location.x, mapG.Location.y);



            textures_tanktrackpattern.repeat = true;

            var textures_bullet = new_tex_400("assets/FlashHeatZeekerWithStarlingT18/bullet.svg");
            var textures_tracks0 = new_tex_400("assets/FlashHeatZeekerWithStarlingT18/tracks0.svg");

            var textures_jeep = new_tex_400("assets/FlashHeatZeekerWithStarlingT18/jeep.svg");
            var textures_jeep_shadow = new_tex_400("assets/FlashHeatZeekerWithStarlingT18/jeep_shadow.svg");
            var textures_jeep_trackpattern = new_tex_400("assets/FlashHeatZeekerWithStarlingT18/jeep_trackpattern.svg");

            var textures_ped_stand = new_tex_400("assets/FlashHeatZeekerWithStarlingT18/ped_stand.svg");

            var textures_greentank = new_tex_400("assets/FlashHeatZeekerWithStarlingT18/greentank.svg");
            var textures_greentank_guntower = new_tex_400("assets/FlashHeatZeekerWithStarlingT18/greentank_guntower.svg");
            var textures_greentank_guntower_rank = new_tex_400("assets/FlashHeatZeekerWithStarlingT18/greentank_guntower_rank.svg");
            var textures_greentank_shadow = new_tex_400("assets/FlashHeatZeekerWithStarlingT18/greentank_shadow.svg");

            GameUnit current = null;



            var b2world = new b2World(new b2Vec2(0, 0), false);


            #region get_b2debug_viewport
            var b2debug_viewport = default(__b2debug_viewport);


            Action get_b2debug_viewport = delegate
            {
                b2debug_viewport = ApplicationSpriteContent.get_b2debug_viewport();

                var b2debugDraw = new b2DebugDraw();
                b2debugDraw.SetSprite(b2debug_viewport.content_layer0);
                // textures are 512 pixels, while our svgs are 400px
                // so how big is a meter in our game world? :)
                b2debugDraw.SetDrawScale(__b2debug_viewport.b2scale);
                b2debugDraw.SetFillAlpha(0.5);
                b2debugDraw.SetLineThickness(1.0);
                b2debugDraw.SetFlags(b2DebugDraw.e_shapeBit);

                b2world.SetDebugDraw(b2debugDraw);
            };

            //get_b2debug_viewport();
            #endregion

            Func<double, double, double[]> ff = (a, b) => { return new double[] { a, b }; };




            #region new_gameunit

            Func<GameUnit> new_ped =
                delegate
                {
                    var unit_loc = new Sprite().AttachTo(viewport_content_layer2_units);
                    var unit_shadow_loc = new Sprite().AttachTo(unit_loc).MoveTo(8, 8);

                    var unit_rot = new Sprite().AttachTo(unit_loc);
                    var unit_shadow_rot = new Sprite().AttachTo(unit_shadow_loc);

                    // can we have wheels?



                    //var shadow_shape = new Image(textures_greentank_shadow) { x = -200, y = -200 }.AttachTo(unit_shadow_rot);
                    //shadow_shape.alpha = 0.2;



                    var shape = new Image(textures_ped_stand) { x = -200, y = -200 }.AttachTo(unit_rot);

                    // art is too big!
                    unit_rot.scaleY = 0.7;
                    unit_rot.scaleX = 0.7;

                    var bodyDef = new b2BodyDef();

                    bodyDef.type = Box2D.Dynamics.b2Body.b2_dynamicBody;
                    bodyDef.linearDamping = 0.15;
                    bodyDef.angularDamping = 0.3;
                    //bodyDef.angle = 1.57079633;
                    bodyDef.fixedRotation = true;

                    var body = b2world.CreateBody(bodyDef);



                    var fixDef = new Box2D.Dynamics.b2FixtureDef();
                    fixDef.density = 0.1;
                    fixDef.friction = 0.01;
                    fixDef.restitution = 0;


                    fixDef.shape = new Box2D.Collision.Shapes.b2CircleShape(0.7);
                    body.CreateFixture(fixDef);

                    var u = new GameUnit
                    {
                        loc = unit_loc,
                        rot = unit_rot,

                        shape = shape,

                        shadow_rot = unit_shadow_rot,

                        //physics = unit4_physics,
                        physics_body = body,

                        isdriver = true,

                        zoomer_default = y => 1.7 + (1 - y) * 0.1

                    };

                    u.RenewTracks =
                        delegate
                        {

                        };

                    return u;
                };


            #region new_tank
            Func<GameUnit> new_tank =
           delegate
           {
               var unit_loc = new Sprite().AttachTo(viewport_content_layer2_units);
               var unit_shadow_loc = new Sprite().AttachTo(unit_loc).MoveTo(8, 8);

               var unit_rot = new Sprite().AttachTo(unit_loc);
               var unit_shadow_rot = new Sprite().AttachTo(unit_shadow_loc);

               // can we have wheels?



               var shadow_shape = new Image(textures_greentank_shadow) { x = -200, y = -200 }.AttachTo(unit_shadow_rot);
               shadow_shape.alpha = 0.2;





               var xwheels = new[] { 
                        //top left
                        new Wheel { b2world = b2world, x = -1.1, y = -1.2, width = 0.4, length = 0.8, revolving = true, powered = true },

                        //top right
                        new Wheel{b2world= b2world, x =1.1,  y =-1.2,  width =0.4,  length =0.8,  revolving =true,  powered =true},


                        //back left
                        new Wheel{b2world= b2world, x =-1.1,  y =1.2,  width =0.4,  length =0.8,  revolving =false,  powered =false},

                        //back right
                        new Wheel{b2world= b2world, x =1.1,  y =1.2,  width =0.4,  length =0.8,  revolving =false,  powered =false},
                    };


               //xwheels.WithEach(build_wheel);

               var shape = new Image(textures_greentank) { x = -200, y = -200 }.AttachTo(unit_rot);





               ////initialize car
               var unit4_physics = new Car(
                   b2world: b2world,
                   width: 4,
                   length: 6,
                   position: ff(0, 0),
                   angle: 180,
                   power: 200,

                   max_steer_angle: 20,
                   //max_steer_angle: 40,

                   max_speed: 60,
                   wheels: xwheels
               );

               var u = new GameUnit
               {
                   loc = unit_loc,
                   rot = unit_rot,

                   shape = shape,

                   shadow_rot = unit_shadow_rot,

                   physics = unit4_physics,

                   driverseat = new GameUnit.DriverSeat()
               };

               var RenewTracks_previous_position_empty = true;
               var RenewTracks_previous_position_x = 0.0;
               var RenewTracks_previous_position_y = 0.0;

               u.RenewTracks =
                   delegate
                   {
                       if (RenewTracks_previous_position_empty)
                       {
                           RenewTracks_previous_position_x = u.loc.x;
                           RenewTracks_previous_position_y = u.loc.y;

                           RenewTracks_previous_position_empty = false;

                           return;
                       }

                       var dx = new __vec2(
                           (float)(u.loc.x - RenewTracks_previous_position_x),
                           (float)(u.loc.y - RenewTracks_previous_position_y)
                       );

                       var dxlen = dx.GetLength() / 10.0;


                       if (dxlen < 1.0)
                       {
                           //Console.WriteLine(new { dxlen, p.x, p.y });
                           return;
                       }

                       RenewTracks_previous_position_x = u.loc.x;
                       RenewTracks_previous_position_y = u.loc.y;

                       //RenewTracks_previous_position = p;

                       #region track_unit_loc
                       var track_unit_loc = new Sprite();
                       var track_unit_rot = new Sprite().AttachTo(track_unit_loc);

                       xwheels.WithEach(
                           w =>
                           {
                               var img = new Image(textures_jeep_trackpattern);
                               img.x = -200;
                               img.y = -200;
                               //img.alpha = 0.1;

                               var track_wheel_loc = new Sprite().AttachTo(track_unit_rot);
                               var track_wheel_rot = new Sprite().AttachTo(track_wheel_loc);

                               img.AttachTo(track_wheel_rot);

                               track_wheel_loc.x = w.x * __b2debug_viewport.b2scale;
                               track_wheel_loc.y = w.y * __b2debug_viewport.b2scale;

                               track_wheel_rot.rotation = w.rotation;
                               //track_wheel_rot.scaleY = dxlen;

                               //+90.DegreesToRadians();

                               if (u.physics.steer_left == Car.STEER_NONE
                                   && u.physics.steer_right == Car.STEER_NONE)
                               {
                                   track_wheel_loc.alpha = 0.1;
                               }
                               else
                               {
                                   track_wheel_loc.alpha = 0.2;
                               }
                           }
                       );


                       track_unit_loc.x = u.loc.x;
                       track_unit_loc.y = u.loc.y;

                       track_unit_rot.rotation = u.rotation;

                       pin_draw(track_unit_loc);
                       #endregion

                   };

               return u;
           };
            #endregion


            #region new_jeep
            Func<GameUnit> new_jeep =
              delegate
              {
                  var unit_loc = new Sprite().AttachTo(viewport_content_layer2_units);
                  var unit_shadow_loc = new Sprite().AttachTo(unit_loc).MoveTo(8, 8);

                  var unit_rot = new Sprite().AttachTo(unit_loc);
                  var unit_shadow_rot = new Sprite().AttachTo(unit_shadow_loc);

                  // can we have wheels?



                  var shadow_shape = new Image(textures_jeep_shadow) { x = -200, y = -200 }.AttachTo(unit_shadow_rot);
                  shadow_shape.alpha = 0.2;

                  Action<Wheel> build_wheel =
                      w =>
                      {
                          var wheel_loc = new Sprite().AttachTo(unit_rot);
                          var wheel_rot = new Sprite().AttachTo(wheel_loc);

                          var wheel_bmd = new ScriptCoreLib.ActionScript.flash.display.BitmapData(1, 1, false, 0);
                          var wheel_tex = Texture.fromBitmapData(wheel_bmd);
                          var wheel_img = new Image(wheel_tex).AttachTo(wheel_rot);

                          wheel_img.scaleX = w.width * __b2debug_viewport.b2scale;
                          wheel_img.scaleY = w.length * __b2debug_viewport.b2scale;

                          wheel_img.x = -0.5 * w.width * __b2debug_viewport.b2scale;
                          wheel_img.y = -0.5 * w.length * __b2debug_viewport.b2scale;

                          wheel_loc.x = w.x * __b2debug_viewport.b2scale;
                          wheel_loc.y = w.y * __b2debug_viewport.b2scale;

                          w.setAngle += a =>
                          {
                              wheel_rot.rotation = a.DegreesToRadians();
                          };
                      };



                  var xwheels = new[] { 
                        //top left
                        new Wheel { b2world = b2world, x = -1.1, y = -1.2, width = 0.4, length = 0.8, revolving = true, powered = true },

                        //top right
                        new Wheel{b2world= b2world, x =1.1,  y =-1.2,  width =0.4,  length =0.8,  revolving =true,  powered =true},


                        //back left
                        new Wheel{b2world= b2world, x =-1.1,  y =1.2,  width =0.4,  length =0.8,  revolving =false,  powered =false},

                        //back right
                        new Wheel{b2world= b2world, x =1.1,  y =1.2,  width =0.4,  length =0.8,  revolving =false,  powered =false},
                    };


                  xwheels.WithEach(build_wheel);

                  var shape = new Image(textures_jeep) { x = -200, y = -200 }.AttachTo(unit_rot);





                  ////initialize car
                  var unit4_physics = new Car(
                      b2world: b2world,
                      width: 2,
                      length: 4,
                      position: ff(0, 0),
                      angle: 180,
                      power: 60,

                      max_steer_angle: 20,
                      //max_steer_angle: 40,

                      max_speed: 60,
                      wheels: xwheels
                  );

                  var u = new GameUnit
                  {
                      isjeepengine = true,

                      loc = unit_loc,
                      rot = unit_rot,

                      shape = shape,

                      shadow_rot = unit_shadow_rot,

                      physics = unit4_physics,

                      // this unit can be manned!
                      driverseat = new GameUnit.DriverSeat()
                  };

                  var RenewTracks_previous_position_empty = true;
                  var RenewTracks_previous_position_x = 0.0;
                  var RenewTracks_previous_position_y = 0.0;

                  u.RenewTracks =
                      delegate
                      {
                          if (RenewTracks_previous_position_empty)
                          {
                              RenewTracks_previous_position_x = u.loc.x;
                              RenewTracks_previous_position_y = u.loc.y;

                              RenewTracks_previous_position_empty = false;

                              return;
                          }

                          var dx = new __vec2(
                              (float)(u.loc.x - RenewTracks_previous_position_x),
                              (float)(u.loc.y - RenewTracks_previous_position_y)
                          );

                          var dxlen = dx.GetLength() / 10.0;


                          if (dxlen < 1.0)
                          {
                              //Console.WriteLine(new { dxlen, p.x, p.y });
                              return;
                          }

                          RenewTracks_previous_position_x = u.loc.x;
                          RenewTracks_previous_position_y = u.loc.y;

                          //RenewTracks_previous_position = p;

                          #region track_unit_loc
                          var track_unit_loc = new Sprite();
                          var track_unit_rot = new Sprite().AttachTo(track_unit_loc);

                          xwheels.WithEach(
                              w =>
                              {
                                  var img = new Image(textures_jeep_trackpattern);
                                  img.x = -200;
                                  img.y = -200;
                                  //img.alpha = 0.1;

                                  var track_wheel_loc = new Sprite().AttachTo(track_unit_rot);
                                  var track_wheel_rot = new Sprite().AttachTo(track_wheel_loc);

                                  img.AttachTo(track_wheel_rot);

                                  track_wheel_loc.x = w.x * __b2debug_viewport.b2scale;
                                  track_wheel_loc.y = w.y * __b2debug_viewport.b2scale;

                                  track_wheel_rot.rotation = w.rotation;
                                  //track_wheel_rot.scaleY = dxlen;

                                  //+90.DegreesToRadians();

                                  if (u.physics.steer_left == Car.STEER_NONE
                                      && u.physics.steer_right == Car.STEER_NONE)
                                  {
                                      track_wheel_loc.alpha = 0.1;
                                  }
                                  else
                                  {
                                      track_wheel_loc.alpha = 0.2;
                                  }
                              }
                          );


                          track_unit_loc.x = u.loc.x;
                          track_unit_loc.y = u.loc.y;

                          track_unit_rot.rotation = u.rotation;

                          pin_draw(track_unit_loc);
                          #endregion

                      };

                  return u;
              };
            #endregion


            #region new_greentank
            Func<GameUnit> new_greentank =
                delegate
                {
                    var unit_loc = new Sprite().AttachTo(viewport_content_layer2_units);
                    var unit_shadow_loc = new Sprite().AttachTo(unit_loc).MoveTo(8, 8);

                    var unit_rot = new Sprite().AttachTo(unit_loc);
                    var unit_shadow_rot = new Sprite().AttachTo(unit_shadow_loc);

                    // can we have wheels?



                    var shadow_shape = new Image(textures_greentank_shadow) { x = -200, y = -200 }.AttachTo(unit_shadow_rot);
                    shadow_shape.alpha = 0.2;


                    var trackpattern = new Sprite().AttachTo(unit_rot);
                    var trackpattern_img = new Image(textures_tanktrackpattern)
                    {
                        x = -512 / 2 + 6, // got a centering issue?
                        y = -512 / 2
                    }.AttachTo(trackpattern);
                    var trackpattern_x = 0.0;

                    trackpattern.scaleX = 0.13;
                    trackpattern.scaleY = 0.18;

                    #region setOffset
                    var hRatio = 1.0;
                    var vRatio = 1.0;

                    Action<Image, double, double> setOffset = (image, xx, yy) =>
                    {
                        yy = ((yy / image.height % 1) + 1);
                        xx = ((xx / image.width % 1) + 1);
                        image.setTexCoords(0, new Point(xx, yy));
                        image.setTexCoords(1, new Point(xx + hRatio, yy));
                        image.setTexCoords(2, new Point(xx, yy + vRatio));
                        image.setTexCoords(3, new Point(xx + hRatio, yy + vRatio));

                    };
                    #endregion

                    var shape = new Image(textures_greentank) { x = -200, y = -200 }.AttachTo(unit_rot);


                    var guntower = new Sprite().AttachTo(unit_rot);


                    new Image(textures_greentank_guntower) { x = -200, y = -200 }.AttachTo(guntower);


                    return new GameUnit
                    {
                        loc = unit_loc,
                        rot = unit_rot,

                        shape = shape,

                        shadow_rot = unit_shadow_rot,

                        AddRank = delegate
                        {

                            new Image(textures_greentank_guntower_rank) { x = -200, y = -200 }.AttachTo(guntower);

                        },

                        guntower = guntower,

                        ScrollTracks = dx =>
                        {
                            trackpattern_x += dx;

                            setOffset(trackpattern_img, 0, trackpattern_x * 1.2);
                        },

                        driverseat = new GameUnit.DriverSeat()
                    };
                };
            #endregion

            #endregion

            #region robo1
            // this is where unit 1 is based on network intel

            // each unit needs a recon shadow
            var unit1_recon = new_greentank();

            // there is a man in robo1, NPC man
            // lets remove his weapon
            unit1_recon.guntower.Orphanize();
            {
                var filter = new ColorMatrixFilter();
                filter.adjustSaturation(-1);
                unit1_recon.shape.filter = filter;
            }


            unit1_recon.loc.MoveTo(
                256, 256
            );

            #endregion

            var units = new List<GameUnit>();

            var unit1 = new_greentank();
            unit1.loc.MoveTo(256, 256);
            unit1.AddRank();
            units.Add(unit1);

            var unit3 = new_greentank();
            unit3.loc.MoveTo(200 + 400 + 200, 200 + 400);
            units.Add(unit3);





            var props = new List<object>();

            // size behaves like radius!!
            props.Add(new CircleProp(b2world, size: ff(50 / __b2debug_viewport.b2scale, 50 / __b2debug_viewport.b2scale), position: ff(0, 0)));
            props.Add(new BoxProp(b2world, size: ff(100 / __b2debug_viewport.b2scale, 100 / __b2debug_viewport.b2scale), position: ff(100 / __b2debug_viewport.b2scale, 100 / __b2debug_viewport.b2scale)));





            //Func<double, double, double[]> ff = (a, b) => { return new double[] { a, b }; };

            //var unit4 = new_jeep();
            //unit4.loc.MoveTo(-200, 0);



            var unit2_jeep = new_jeep();
            unit2_jeep.TeleportBy(100, -200);
            units.Add(unit2_jeep);

            var unit9_tank = new_tank();
            unit9_tank.TeleportBy(200, -200);
            units.Add(unit9_tank);

            var unit8_ped = new_ped();
            unit8_ped.TeleportBy(200, 0);
            units.Add(unit8_ped);

            // 2 jeeps : 40fps
            // 10 jeeps : 32fps
            // 20 jeeps : 26fps

            for (int iy = 0; iy < 1; iy++)
                for (int ix = 0; ix < 3; ix++)
                {
                    var unit5 = new_jeep();
                    unit5.TeleportBy(-50 * ix, -200 - iy * 50);
                    units.Add(unit5);


                }

            for (int iy = 0; iy < 8; iy++)
                for (int ix = 0; ix < 8; ix++)
                {


                    var unit8 = new_ped();
                    unit8.TeleportBy(-25 * ix, 200 + iy * 25);
                    units.Add(unit8);
                }



            #region robo1
            var robo1 = new_greentank();

            // there is a man in robo1, NPC man
            // lets remove his weapon
            robo1.guntower.Orphanize();
            {
                var filter = new ColorMatrixFilter();
                filter.adjustHue(0.5);
                robo1.shape.filter = filter;
            }
            robo1.loc.MoveTo(
                mapB.Location.x + 2048 - 128,
                mapB.Location.y + 2048 - 128
            );

            robo1.rotation = 270.DegreesToRadians();



            #endregion


            var controllable = new[] { unit1, unit3 }.Concat(units);

            current = unit1;

            #region tree0

            for (int iy = 0; iy < 128; iy++)
            {
                {
                    var svg = new Image(textures_tree0);
                    svg.x = 400;
                    svg.y = 0;
                    svg.AttachTo(viewport_content);

                    svg.scaleX = 0.15;
                    svg.scaleY = 0.15;

                    if (iy % 3 == 0)
                        svg.y += 50;

                    if (iy % 3 == 1)
                        svg.y += 100;


                    svg.x += 15 * iy;
                }


                {
                    var svg = new Image(textures_tree0);
                    svg.x = 0;
                    svg.y = 400;
                    svg.AttachTo(viewport_content);

                    svg.scaleX = 0.15;
                    svg.scaleY = 0.15;

                    if (iy % 3 == 0)
                        svg.x += 50;

                    if (iy % 3 == 1)
                        svg.x += 100;


                    svg.y += 15 * iy;
                }
            }
            #endregion

            var frameid = 0L;


            var move_speed_default = 0.09;
            var move_speed = move_speed_default;



            var rot_sw = new Stopwatch();
            rot_sw.Start();

            var move_zoom = 1.0;

            var diesel2 = KnownEmbeddedResources.Default["assets/FlashHeatZeekerWithStarlingT18/diesel4.mp3"].ToSoundAsset().ToMP3PitchLoop();
            var sand_run = KnownEmbeddedResources.Default["assets/FlashHeatZeekerWithStarlingT18/sand_run.mp3"].ToSoundAsset().ToMP3PitchLoop();
            var jeepengine = KnownEmbeddedResources.Default["assets/FlashHeatZeekerWithStarlingT18/jeepengine.mp3"].ToSoundAsset().ToMP3PitchLoop();


            //diesel2.Sound.vol
            var profile_map_teleportcheck = 0L;

            Func<double, double> zoomer_default = y => 1 + (1 - y) * 0.2;
            Func<double, double> current_zoomer = zoomer_default;








            #region lookat
            // F3 to disable camera follow.
            bool lookat_disabled = false;
            Action<double, double, double> lookat = null;

            lookat = (rot, x, y) =>
            {
                if (!lookat_disabled)
                {
                    if (b2debug_viewport != null)
                    {
                        b2debug_viewport.rot.rotation = -rot.RadiansToDegrees();
                        b2debug_viewport.content.x = -x;
                        b2debug_viewport.content.y = -y;
                    }

                    viewport_rot.rotation = -rot;

                    viewport_content.x = -x;
                    viewport_content.y = -y;
                }

                var __profile_map_teleportcheck = new Stopwatch();
                __profile_map_teleportcheck.Start();

                Action later = delegate { };

                #region map_teleport
                Action<GameMap, double, double, bool> map_teleport =
                    (map, mapx, mapy, allow_unit_teleport) =>
                    {
                        var dx = mapx - map.ground.x;
                        var dy = mapy - map.ground.y;

                        if (dx == 0)
                            if (dy == 0)
                                return;

                        if (allow_unit_teleport)
                        {
                            //Console.WriteLine("allow_unit_teleport, hittesting... " + new { locx = current.loc.x, locy = current.loc.y, x, y });

                            if (map.hitTest(x, y))
                            {
                                Console.WriteLine("will teleport to " + map.Name + " by " + new { dx, dy });

                                // ourself and everybody around us?

                                current.TeleportBy(dx, dy);

                                later = delegate
                                {
                                    // infinite loop?
                                    lookat(current.rotation, current.loc.x, current.loc.y);
                                };
                            }
                            else
                            {
                                //Console.WriteLine("will NOT teleport");
                            }
                        }

                        map.ground.MoveTo(
                          map.ground.x + dx,
                          map.ground.y + dy
                        );

                        var teleportsw = new Stopwatch();

                        teleportsw.Start();

                        map.doodads.WithEach(
                            k => k.MoveTo(k.x + dx, k.y + dy)
                        );

                        map.static_units.WithEach(
                            k => k.TeleportBy(dx, dy)
                        );

                        teleportsw.Stop();

                        Console.WriteLine(map.Name + " teleport done in " + teleportsw.ElapsedMilliseconds + "ms");

                    };
                #endregion


                // am I in a virtual position? teleport to original
                // am I near a virtual position? show it
                // otherwise revert


                maps.WithEach(
                    map =>
                    {
                        #region in_virtual
                        var in_virtual = map.VirtualLocations.Where(
                            v =>
                            {
                                if (x < v.x)
                                    return false;

                                if (y < v.y)
                                    return false;

                                if (x >= v.x + 2048)
                                    return false;

                                if (y >= v.y + 2048)
                                    return false;

                                return true;
                            }
                        ).FirstOrDefault();

                        if (in_virtual != null)
                        {
                            map_teleport(map,
                                map.Location.x,
                                map.Location.y,
                                true
                            );
                            return;
                        }
                        #endregion

                        #region near_virtual
                        var near_virtual = map.VirtualLocations.Where(
                                v =>
                                {
                                    if (x < v.x - 1024)
                                        return false;

                                    if (y < v.y - 1024)
                                        return false;

                                    if (x >= v.x + 2048 + 1024)
                                        return false;

                                    if (y >= v.y + 2048 + 1024)
                                        return false;

                                    return true;
                                }
                            ).FirstOrDefault();

                        if (near_virtual != null)
                        {
                            map_teleport(map,
                                near_virtual.x,
                                near_virtual.y,
                                false
                            );
                            return;
                        }
                        #endregion

                        map_teleport(map,
                            map.Location.x,
                            map.Location.y,
                            false
                        );

                    }
                );

                __profile_map_teleportcheck.Stop();
                profile_map_teleportcheck += __profile_map_teleportcheck.ElapsedMilliseconds;

                later();

            };
            #endregion

            bool user_pause = false;




            var KineticEnergy = new List<KineticEnergy>();

            var physicstime = new Stopwatch();

            physicstime.Start();

            ApplicationSprite.__stage.enterFrame +=
                delegate
                {
                    physicstime.Stop();

                    if (user_pause)
                        return;

                    var physicstime_elapsed = physicstime.ElapsedMilliseconds;

                    physicstime.Restart();

                    rot_sw.Stop();



                    // first thing do physics?

                    //TypeError: Error #1009: Cannot access a property or method of a null object reference.
                    //    at Box2D.Dynamics::b2World/DrawDebugData()[Y:\opensource\sourceforge\box2dflash\Box2D\Dynamics\b2World.as:656]
                    //    at FlashHeatZeekerWithStarlingT18::Game___c__DisplayClass46/__ctor_b__3b_100663971()[V:\web\FlashHeatZeekerWithStarlingT18\Game___c__DisplayClass46.as:163]

                    // can jsc tell us about timing?



                    foreach (var item in units)
                    {

                        item.physics_body.With(
                            body =>
                            {

                                var a = item.physics_body.GetAngle();

                                var ped_drot = rot_sw.ElapsedMilliseconds
                                     * (0.005)
                                     * ((item.rot_left + item.rot_right));

                                a += ped_drot;

                                item.physics_body.SetAngle(a);

                                item.physics_body.SetLinearVelocity(
                                    new b2Vec2(
                                       6 * Math.Cos(a + 270.DegreesToRadians()) * (item.move_forward + item.move_backward),
                                       6 * Math.Sin(a + 270.DegreesToRadians()) * (item.move_forward + item.move_backward)
                                    )
                                );

                            }
                        );

                        if (item.physics != null)
                            item.physics.update(physicstime_elapsed);
                    }



                    //update physics world
                    b2world.Step(physicstime_elapsed / 1000.0, 10, 8);

                    //clear applied forces, so they don't stack from each update
                    b2world.ClearForces();

                    if (b2debug_viewport != null)
                        b2world.DrawDebugData();


                    foreach (var item in units)
                    {
                        if (item.physics_body != null)
                            item.physics_body.GetPosition().With(
                                 p =>
                                 {
                                     item.loc.x = p.x * __b2debug_viewport.b2scale;
                                     item.loc.y = p.y * __b2debug_viewport.b2scale;

                                     item.rotation = item.physics_body.GetAngle();

                                     item.RenewTracks();
                                 }
                             );

                        if (item.physics != null)
                            item.physics.body.GetPosition().With(
                                 p =>
                                 {
                                     item.loc.x = p.x * __b2debug_viewport.b2scale;
                                     item.loc.y = p.y * __b2debug_viewport.b2scale;

                                     item.rotation = item.physics.body.GetAngle();

                                     item.RenewTracks();
                                 }
                             );
                    }




                    // which is it, do we need to zoom out or in?

                    #region KineticEnergy
                    foreach (var item in KineticEnergy)
                    {
                        item.Target.With(
                            t =>
                            {
                                if (item.TTL == 0)
                                {
                                    t.Orphanize();

                                    item.Target = null;
                                    return;
                                }

                                t.x += rot_sw.ElapsedMilliseconds * item.Energy.x;
                                t.y += rot_sw.ElapsedMilliseconds * item.Energy.y;

                                item.TTL--;


                            }
                        );
                    }
                    #endregion


                    var any_movement = Math.Sign(
                        Math.Abs(current.move_forward)
                        + Math.Abs(current.move_backward)
                        + Math.Abs(current.rot_left)
                        + Math.Abs(current.rot_right)
                    ) - 0.5;

                    move_zoom +=
                        any_movement *
                          rot_sw.ElapsedMilliseconds * 0.004;

                    move_zoom = move_zoom.Max(0.0).Min(1.0);

                    if (current.isdriver)
                    {
                        diesel2.LeftVolume = 0;
                        diesel2.RightVolume = 0;

                        jeepengine.LeftVolume = 0;
                        jeepengine.RightVolume = 0;

                        sand_run.LeftVolume = 0.0 + move_zoom * 0.9;
                        sand_run.RightVolume = 0.0 + move_zoom * 0.9;
                        sand_run.Rate = 0.9 + move_zoom * 0.1;
                    }
                    else if (current.isjeepengine)
                    {
                        diesel2.LeftVolume = 0;
                        diesel2.RightVolume = 0;


                        sand_run.LeftVolume = 0;
                        sand_run.RightVolume = 0;

                        jeepengine.LeftVolume = 0.4 + move_zoom * 0.7;
                        jeepengine.RightVolume = 1;
                        jeepengine.Rate = 1;
                        //jeepengine.Rate = 0.9 + move_zoom * 0.1;
                    }
                    else
                    {
                        jeepengine.LeftVolume = 0;
                        jeepengine.RightVolume = 0;

                        sand_run.LeftVolume = 0;
                        sand_run.RightVolume = 0;

                        diesel2.LeftVolume = 0.3 + move_zoom * 0.7;
                        diesel2.RightVolume = 1;
                        diesel2.Rate = 0.9 + move_zoom;
                    }
                    // show only % of the zoom/speed boost
                    //if (lookat_disabled)
                    //{

                    //}
                    //else
                    //{
                    viewport_rot.scaleX = current_zoomer(move_zoom);
                    viewport_rot.scaleY = current_zoomer(move_zoom);

                    if (b2debug_viewport != null)
                    {
                        b2debug_viewport.rot.scaleX = current_zoomer(move_zoom);
                        b2debug_viewport.rot.scaleY = current_zoomer(move_zoom);
                    }

                    var drot = rot_sw.ElapsedMilliseconds
                        * (1 + move_zoom)
                        * (current.rot_left + current.rot_right)
                        * (Math.Abs(current.move_forward + current.move_backward).Max(0.5) * 0.09).DegreesToRadians();



                    #region remotecontrol
                    Action<GameUnit> remotecontrol =
                        c =>
                        {
                            c.rotation += drot;

                            var dx = rot_sw.ElapsedMilliseconds
                                * (1 + move_zoom)
                                * (current.move_forward + current.move_backward)
                                * move_speed
                                * Math.Cos(c.rotation + (270).DegreesToRadians());

                            var dy = rot_sw.ElapsedMilliseconds
                               * (1 + move_zoom)
                               * (current.move_forward + current.move_backward)
                               * move_speed
                               * Math.Sin(c.rotation + (270).DegreesToRadians());


                            c.ScrollTracks(
                                rot_sw.ElapsedMilliseconds
                                * (1 + move_zoom)
                               * (current.move_forward + current.move_backward)
                               * move_speed
                            );
                            var prevframe_loc = new __vec2();

                            prevframe_loc.x = (float)c.loc.x;
                            prevframe_loc.y = (float)c.loc.y;

                            c.loc.x += dx;
                            c.loc.y += dy;

                            var prevframe_loc_length = new __vec2(
                                c.prevframe_loc.x - (float)c.loc.x,
                                c.prevframe_loc.y - (float)c.loc.y
                            ).GetLength();

                            var changed_prevframe_rot =
                                Math.Abs(c.rot.rotation - c.prevframe_rot) > 25.DegreesToRadians();

                            if (prevframe_loc_length > 80 || changed_prevframe_rot)
                            {
                                // unit draws tracks..
                                c.prevframe_loc = prevframe_loc;
                                c.prevframe_rot = c.rot.rotation;

                                var unit_loc = new Sprite();
                                var unit_rot = new Sprite().AttachTo(unit_loc);

                                var img = new Image(textures_tracks0);
                                img.x = -200;
                                img.y = -200;
                                img.alpha = 0.2;

                                img.AttachTo(unit_rot);


                                unit_rot.rotation = c.rotation;
                                unit_loc.MoveTo(c.loc.x, c.loc.y);
                                // lets add our new tracks to the map, to teleport them later

                                //pin_doodad(unit_loc);

                                //c.tracks.Enqueue(unit_loc);

                                //if (c.tracks.Count > 256)
                                //    c.tracks.Dequeue().Orphanize();

                                pin_draw(unit_loc);

                            }
                        };
                    #endregion

                    if (current.physics_body != null)
                    {


                        lookat(current.rot.rotation, current.loc.x, current.loc.y);


                    }
                    else if (current.physics != null)
                    {



                        lookat(current.rot.rotation, current.loc.x, current.loc.y);


                    }
                    else
                    {
                        if (robo1.RemoteControlEnabled)
                        {
                            remotecontrol(robo1);


                            lookat(
                                current.rot.rotation,
                                (current.loc.x + (robo1.loc.x - current.loc.x) / 2),
                                (current.loc.y + (robo1.loc.y - current.loc.y) / 2)
                                );

                        }
                        else
                        {
                            remotecontrol(current);

                            lookat(current.rot.rotation, current.loc.x, current.loc.y);
                        }
                    }

                    rot_sw.Restart();



                };



            #region switchto
            Action<GameUnit> switchto =
                nextunit =>
                {
                    KnownEmbeddedResources.Default["assets/FlashHeatZeekerWithStarlingT18/letsgo.mp3"].ToSoundAsset().play();

                    move_zoom = 1;
                    current_zoomer = nextunit.zoomer_default;


                    current = nextunit;
                    lookat(current.rotation, current.loc.x, current.loc.y);
                };
            #endregion

            var postMessage_tracker = 0;
            var postMessage_xtracker = 1000;

            var sync_postMessage_for_context = new List<XElement>();

            #region postMessage
            Action<XElement> sync_postMessage =
                e =>
                {
                    postMessage_tracker++;
                    postMessage_xtracker++;

                    var ee = XElement.Parse(e.ToString());
                    ee.Add(new XAttribute("t", "" + postMessage_tracker));

                    // send it to outside world
                    //ApplicationSprite.__sprite.context_postMessage(ee);
                    sync_postMessage_for_context.Add(ee);

                    var eee = XElement.Parse(e.ToString());
                    eee.Add(new XAttribute("t", "" + postMessage_xtracker));

                    // send it to ourselves
                    ApplicationSprite.__sprite.__game_postMessage(eee);
                };
            #endregion

            // script: error JSC1000: ActionScript : failure at starling.display.Stage.add_keyDown : Object reference not set to an instance of an object.
            // there is something fron with flash natives gen. need to fix that.





            var disable_keyDown_Up = false;
            var disable_keyDown_Down = false;
            var disable_keyDown_Left = false;
            var disable_keyDown_Right = false;

            #region keyDown
            ApplicationSprite.__stage.keyDown +=
                e =>
                {
                    Console.WriteLine("keyDown " + new { e.keyCode });

                    if (e.keyCode == (uint)System.Windows.Forms.Keys.Up)
                    {

                        if (!disable_keyDown_Up)
                        {
                            disable_keyDown_Up = true;

                            current.move_forward = 1;

                            sync_postMessage(
                                  new XElement("move_forward",
                                      new XAttribute("i", "" + networkid),
                                      new XAttribute("f", "" + (networkframe + 2))
                                  )
                            );

                            if (current != null)
                                if (current.physics != null)
                                    current.physics.accelerate = Car.ACC_ACCELERATE;
                        }
                    }

                    if (e.keyCode == (uint)System.Windows.Forms.Keys.Down)
                    {


                        if (!disable_keyDown_Down)
                        {
                            disable_keyDown_Down = true;

                            // move slower while backwards?
                            current.move_backward = -0.5;
                            current.move_forward = 0;

                            sync_postMessage(
                                  new XElement("move_backward",
                                      new XAttribute("i", "" + networkid),
                                      new XAttribute("f", "" + (networkframe + 2))
                                  )
                            );
                            if (current != null)
                                if (current.physics != null)
                                    current.physics.accelerate = Car.ACC_BRAKE;
                        }
                    }

                    if (e.keyCode == (uint)System.Windows.Forms.Keys.Left)
                    {
                        if (!disable_keyDown_Left)
                        {
                            disable_keyDown_Left = true;

                            current.rot_left = -1;

                            sync_postMessage(
                                  new XElement("rot_left",
                                      new XAttribute("i", "" + networkid),
                                      new XAttribute("f", "" + (networkframe + 2))
                                  )
                            );

                            if (current != null)
                                if (current.physics != null)
                                    current.physics.steer_left = Car.STEER_LEFT;
                        }
                    }

                    if (e.keyCode == (uint)System.Windows.Forms.Keys.Right)
                    {

                        if (!disable_keyDown_Right)
                        {
                            disable_keyDown_Right = true;


                            sync_postMessage(
                                  new XElement("rot_right",
                                      new XAttribute("i", "" + networkid),
                                      new XAttribute("f", "" + (networkframe + 2))
                                  )
                            );

                            if (current != null)
                            {
                                current.rot_right = 1;

                                if (current.physics != null)
                                    current.physics.steer_right = Car.STEER_RIGHT;
                            }
                        }
                    }


                };
            #endregion


            #region keyUp
            ApplicationSprite.__stage.keyUp +=
              e =>
              {
                  Console.WriteLine("keyUp " + new { e.keyCode });

                  if (e.keyCode == (uint)System.Windows.Forms.Keys.CapsLock)
                  {
                      robo1.RemoteControlEnabled = !robo1.RemoteControlEnabled;
                  }


                  if (e.keyCode == (uint)System.Windows.Forms.Keys.Up)
                  {
                      current.move_forward = 0.0;

                      disable_keyDown_Up = false;

                      sync_postMessage(
                           new XElement("move_forward",
                               new XAttribute("i", "" + networkid),
                               new XAttribute("f", "" + (networkframe + 2))
                           )
                     );

                      if (current != null)
                          if (current.physics != null)
                              current.physics.accelerate = Car.ACC_NONE;

                  }

                  if (e.keyCode == (uint)System.Windows.Forms.Keys.Down)
                  {
                      current.move_backward = 0;

                      disable_keyDown_Down = false;

                      sync_postMessage(
                           new XElement("move_backward",
                               new XAttribute("i", "" + networkid),
                               new XAttribute("f", "" + (networkframe + 2))
                           )
                     );
                      if (current != null)
                          if (current.physics != null)
                              current.physics.accelerate = Car.ACC_NONE;
                  }

                  if (e.keyCode == (uint)System.Windows.Forms.Keys.Left)
                  {
                      current.rot_left = 0;
                      disable_keyDown_Left = false;

                      sync_postMessage(
                            new XElement("rot_left",
                                new XAttribute("i", "" + networkid),
                                new XAttribute("f", "" + (networkframe + 2))
                            )
                      );
                      if (current != null)
                          if (current.physics != null)
                              current.physics.steer_left = Car.STEER_NONE;
                  }

                  if (e.keyCode == (uint)System.Windows.Forms.Keys.Right)
                  {
                      disable_keyDown_Right = false;

                      sync_postMessage(
                           new XElement("rot_right",
                               new XAttribute("i", "" + networkid),
                               new XAttribute("f", "" + (networkframe + 2))
                           )
                     );
                      if (current != null)
                      {
                          current.rot_right = 0;

                          if (current.physics != null)
                              current.physics.steer_right = Car.STEER_NONE;
                      }
                  }



                  if (e.keyCode == (uint)System.Windows.Forms.Keys.D1)
                  {
                      switchto(unit1);
                  }

                  if (e.keyCode == (uint)System.Windows.Forms.Keys.D2)
                  {
                      switchto(unit2_jeep);
                  }

                  if (e.keyCode == (uint)System.Windows.Forms.Keys.D3)
                  {
                      switchto(unit3);
                  }

                  if (e.keyCode == (uint)System.Windows.Forms.Keys.P)
                  {
                      user_pause = !user_pause;
                  }

                  if (e.keyCode == (uint)System.Windows.Forms.Keys.Tab)
                  {
                      //                      System.Linq.Enumerable for System.Collections.Generic.IEnumerable`1[FlashHeatZeekerWithStarlingT18.GameUnit] Skip[GameUnit](System.Collections.Generic.IEnumerable`1[FlashHeatZeekerWithStarlingT18.GameUnit], Int32) used at
                      //FlashHeatZeekerWithStarlingT18.Game+<>c__DisplayClass10.<.ctor>b__a at offset 014b.
                      //If the use of this method is intended, an implementation should be provided with the attribute [Script(Implements=typeof(...)] set. You may have mistyped it.



                      var nextunit = controllable.AsCyclicEnumerable().SkipWhile(k => k != current).Take(2).Last();
                      switchto(nextunit);
                  }

                  #region Enter
                  if (e.keyCode == (uint)System.Windows.Forms.Keys.Enter)
                  {
                      // could also build/ man a standalone guntowner
                      // or jump out a flying chopper?

                      if (current.isdriver)
                      {
                          Console.WriteLine("lets enter a vehicle!");

                          var target =
                              from x in units
                              where x.driverseat != null

                              // can enter if the seat is full.
                              // unless we kick them out before ofcourse
                              where x.driverseat.driver == null

                              let distance = new __vec2(
                                  (float)(current.loc.x - x.loc.x),
                                  (float)(current.loc.y - x.loc.y)
                              ).GetLength()

                              where distance < 60

                              orderby distance descending
                              select new { x, distance };

                          target.FirstOrDefault().With(
                              x =>
                              {
                                  Console.WriteLine(new { x.distance });

                                  current.loc.visible = false;
                                  current.physics_body.SetActive(false);

                                  x.x.driverseat.driver = current;
                                  switchto(x.x);
                              }
                          );

                      }
                      else if (current.driverseat != null)
                      {
                          Console.WriteLine("lets exit a vehicle!");

                          current.driverseat.driver.With(
                              driver =>
                              {
                                  current.driverseat.driver = null;

                                  driver.loc.visible = true;
                                  driver.physics_body.SetActive(true);


                                  // are we jumping out of the car?
                                  // where should we appear? at what impulse?

                                  driver.TeleportTo(
                                       current,

                                       // where is the door?
                                       Math.Cos(current.rotation) * 30,
                                       Math.Sin(current.rotation) * 30
                                   );


                                  driver.rotation = current.rotation;

                                  switchto(driver);


                                  //unit8_ped.physics_body.ApplyLinearImpulse(
                                  //    unit2_jeep.physics.body.get
                              }
                          );

                      }
                  }
                  #endregion

                  // flash fullscreen allows space, tab and arrows!
                  if (e.keyCode == (uint)System.Windows.Forms.Keys.Space)
                  {
                      Console.WriteLine("fire!");
                      // http://www.sounddogs.com/results.asp?Type=1&CategoryID=1027&SubcategoryID=11
                      KnownEmbeddedResources.Default["assets/FlashHeatZeekerWithStarlingT18/cannon1.mp3"].ToSoundAsset().play();

                      var unit_bullet = new Sprite().AttachTo(viewport_content);

                      var shape = new Image(textures_bullet) { x = -200, y = -200 }.AttachTo(unit_bullet);

                      unit_bullet.MoveTo(
                          current.loc.x + 100 * Math.Cos(current.rotation + 270.DegreesToRadians()),
                          current.loc.y + 100 * Math.Sin(current.rotation + 270.DegreesToRadians())
                      );

                      KineticEnergy.Add(
                          new FlashHeatZeekerWithStarlingT18.KineticEnergy
                          {
                              Target = unit_bullet,
                              Energy = new __vec2(
                                  (float)(2 * Math.Cos(current.rotation + 270.DegreesToRadians())),
                                  (float)(2 * Math.Sin(current.rotation + 270.DegreesToRadians()))
                              ),
                              TTL = 30
                          }
                        );

                      unit_bullet.scaleX = 0.8;
                      unit_bullet.scaleY = 0.8;
                  }

                  if (e.keyCode == (uint)System.Windows.Forms.Keys.F6)
                  {

                      current.TeleportBy(200, 200);

                  }

                  if (e.keyCode == (uint)System.Windows.Forms.Keys.F4)
                  {

                  }

                  // disable camera follow
                  if (e.keyCode == (uint)System.Windows.Forms.Keys.F3)
                  {
                      lookat_disabled = !lookat_disabled;
                  }

                  // toggle physics view
                  if (e.keyCode == (uint)System.Windows.Forms.Keys.F2)
                  {
                      // faster?
                      // jsc flash natives gen / fiels public vs protected
                      //b2debugDraw.m_sprite.alpha = 0;

                      if (b2debug_viewport == null)
                      {
                          get_b2debug_viewport();
                      }
                      else
                      {
                          b2world.SetDebugDraw(null);

                          b2debug_viewport.Dispose();

                          b2debug_viewport = null;
                          //b2debugDraw = null;
                      }
                  }

                  // orbit mode
                  if (e.keyCode == (uint)System.Windows.Forms.Keys.F1)
                  {
                      //                      cript: error JSC1000: ActionScript :
                      //BCL needs another method, please define it.
                      //Cannot call type without script attribute :
                      //System.Delegate for Boolean op_Equality(System.Delegate, System.Delegate) used at
                      //FlashHeatZeekerWithStarlingT18.Game+<>c__DisplayClass22.<.ctor>b__19 at offset 035c.
                      //If the use of this method is intended, an implementation should be provided with the attribute [Script(Implements=typeof(...)] set. You may have mistyped it.

                      if ((object)current_zoomer == (object)zoomer_default)
                      {
                          current_zoomer = y => 0.10 + (1 - y) * 0.02;
                          move_speed = move_speed_default * 10;
                      }
                      else
                      {
                          current_zoomer = zoomer_default;
                          move_speed = move_speed_default;
                      }
                  }
              };
            #endregion



            // where is our ego? center of touchdown?
            //switchto(unit1);
            switchto(unit8_ped);

            var info = new TextField(
                800,
                400,
                "Welcome to Starling!"
            ) { hAlign = HAlign.LEFT, vAlign = VAlign.TOP };

            info.AttachTo(this).MoveTo(8, 8);


            var logo = new Image(LogoTexture) { alpha = 0.3 }.AttachTo(this);

            #region viewport_loc, resize all you want
            Action centerize = delegate
            {
                logo.MoveTo(
                    ApplicationSprite.__stage.stageWidth - 96,
                    ApplicationSprite.__stage.stageHeight - 96
                );

                viewport_loc.x = ApplicationSprite.__stage.stageWidth * 0.5;
                viewport_loc.y = ApplicationSprite.__stage.stageHeight * 0.7;
            };

            ApplicationSprite.__stage.resize +=
                delegate
                {
                    centerize();
                };

            centerize();
            #endregion


            var maxframe = new Stopwatch();
            var maxframe_elapsed = 0.0;


            #region fps
            var sw = new Stopwatch();

            sw.Start();

            var fps = 1;

            var ii = 0;

            maxframe.Start();


            var network_rx_last_second = 0;


            ApplicationSprite.__stage.enterFrame +=
                delegate
                {
                    frameid++;

                    maxframe.Stop();

                    //                    System.TimeSpan for Boolean op_GreaterThan(System.TimeSpan, System.TimeSpan) used at
                    //FlashHeatZeeker.ApplicationSprite+<>c__DisplayClass11.<.ctor>b__d at offset 001e.

                    //                TypeError: Error #1009: Cannot access a property or method of a null object reference.
                    //at FlashHeatZeeker::ApplicationSprite___c__DisplayClass11/__ctor_b__d_100663322()[U:\web\FlashHeatZeeker\ApplicationSprite___c__DisplayClass11.as:141]

                    if (maxframe.Elapsed.TotalMilliseconds > maxframe_elapsed)
                        maxframe_elapsed = maxframe.Elapsed.TotalMilliseconds;

                    ii++;

                    var now = DateTime.Now;
                    var network_rx_per_second = network_rx - network_rx_last_second;

                    var SPEED = "-";

                    if (current != null)
                        if (current.physics != null)
                            SPEED = Math.Ceiling(current.physics.getSpeedKMH()) + " km/h";

                    // memory 538 - 187
                    info.text = "F1 overview F2 physics F3 camera F4 clear\n" + new
                    {
                        fps,
                        SPEED,
                        networkid,
                        networkframe,
                        network_rx,
                        network_rx_per_second,
                        frameid,
                        maxframe_elapsed,
                        now
                        //, 
                        //profile_map_teleportcheck 
                    }.ToString().Replace(", ", ",\n");

                    if (fps < 25)
                        info.color = 0xff0000;
                    else if (fps < 30)
                        info.color = 0xaf0000;
                    else if (fps < 40)
                        info.color = 0x7f0000;
                    else
                        info.color = 0x0;

                    if (sw.ElapsedMilliseconds < 1000)
                    {
                        maxframe.Restart();
                        return;
                    }

                    fps = ii;

                    ApplicationSprite.__sprite.__raise_fps("" + fps);

                    network_rx_last_second = network_rx;

                    ii = 0;
                    maxframe_elapsed = 0;
                    sw.Restart();
                };
            #endregion

            #region networktimer
            var networktimer = new ScriptCoreLib.ActionScript.flash.utils.Timer(1000 / 15);
            networktimer.timer +=
                delegate
                {
                    if (user_pause)
                        return;

                    networkframe++;




                    //what did we plan to do today, at this frame?
                    // lets look our calendar

                    networksync_actions[networkframe].With(
                        tasks =>
                        {
                            tasks();
                        }
                    );

                    // garbage collect
                    networksync_actions[networkframe] = null;

                    // how much bandwidth can we use? are we throttled if we exceed a limit per sec?

                    #region context_postMessage
                    var message = new XElement("s",
                            new XAttribute("i", "" + networkid),
                            new XAttribute("f", "" + networkframe)
                        );


                    // if we send it at once is it better?
                    foreach (var item in sync_postMessage_for_context)
                    {
                        message.Add(item);
                    }
                    sync_postMessage_for_context.Clear();

                    ApplicationSprite.__sprite.__context_postMessage(message);
                    #endregion


                    this.remotegames.WithEach(
                        remotegame =>
                        {
                            if (remotegame.networkid == networkid)
                                return;

                            // if dx is more than 4 we might need to wait for the other client!
                            // they mght be on pause!

                            remotegame.RaiseTitleChange(
                                remotegame.networkid + " " + new
                                {
                                    remotegame.networkframe,
                                    remotegame.networkframe_dx,
                                    remotegame.rx_messagecount
                                    //, remotegame.initial_networkframe_delta
                                }.ToString());
                        }
                    );
                };
            networktimer.start();
            #endregion

            Action<XElement> __sprite_game_onmessage = null;

            __sprite_game_onmessage =
                data =>
                {
                    network_rx += data.ToString().Length;

                    var __networkid = data.Attribute("networkid");
                    if (__networkid == null)
                        __networkid = data.Attribute("i");

                    var __networkframe = data.Attribute("networkframe");
                    if (__networkframe == null)
                        __networkframe = data.Attribute("f");


                    var remotegame_networkid = int.Parse(__networkid.Value);
                    var remotegame_networkframe = int.Parse(__networkframe.Value);

                    #region remotegame networkid


                    // do we know this remote game already?

                    var remotegame = remotegames.FirstOrDefault(k => k.networkid == remotegame_networkid);

                    if (remotegame == null)
                    {
                        remotegame = new RemoteGame
                        {
                            Context = this,

                            networkid = remotegame_networkid,
                            initial_networkframe_delta = remotegame_networkframe - this.networkframe
                        };

                        remotegames.Add(remotegame);

                        // 
                        var remotegame_info = new TextField(800, 100, "") { hAlign = HAlign.LEFT }.MoveTo(
                            8, remotegames.Count * 32 + 72
                        ).AttachTo(this);


                        remotegame.AtTitleChange +=
                            e =>
                            {
                                if (remotegame.networkid == networkid)
                                {
                                    remotegame_info.color = 0xff;
                                }
                                else if (Math.Abs(remotegame.networkframe_dx) > 4)
                                {
                                    remotegame_info.color = 0xff0000;
                                }
                                else
                                {
                                    remotegame_info.color = 0;
                                }

                                remotegame_info.text = e;
                            };


                        // let context (html app) know about what we know
                        ApplicationSprite.__sprite.__raise_context_new_remotegame(remotegame);
                        remotegame.RaiseTitleChange(new { remotegame.networkid }.ToString());
                    }
                    #endregion


                    remotegame.rx_messagecount++;

                    //if (sync_networkid == networkid)
                    //{
                    // we are looking at a message from ourself!

                    //if (sync.Name.LocalName == "recon_keyCode_Left")
                    //{

                    //    var tasks = networksync_actions[sync_networkframe];

                    //    tasks += delegate
                    //    {
                    //        unit1_recon.rotation -= 5.DegreesToRadians();
                    //    };

                    //    networksync_actions[sync_networkframe] = tasks;

                    //    if (sync_networkid == networkid)
                    //        return;
                    //}
                    //}

                    #region sync
                    if (data.Name.LocalName == "s")
                    {

                        // this tells us the remote game has completed this frame
                        remotegame.networkframe = remotegame_networkframe;

                        data.Elements().WithEach(
                            submessage =>
                            {
                                __sprite_game_onmessage(submessage);
                            }
                        );

                        return;
                    }
                    #endregion


                    if (data.Name.LocalName == "move_forward")
                    {
                        if (remotegame.networkid == networkid)
                        {
                            // move ghost instead
                        }
                        else
                        {
                            if (current.move_forward == 0)
                                current.move_forward = 1;
                            else
                                current.move_forward = 0;
                        }
                    }

                    if (data.Name.LocalName == "move_backward")
                    {
                        if (remotegame.networkid == networkid)
                        {
                            // move ghost instead
                        }
                        else
                        {
                            if (current.move_backward == 0)
                                current.move_backward = -0.5;
                            else
                                current.move_backward = 0;
                        }
                    }

                    if (data.Name.LocalName == "rot_left")
                    {
                        if (remotegame.networkid == networkid)
                        {
                            // move ghost instead
                        }
                        else
                        {
                            if (current.rot_left == 0)
                                current.rot_left = -1;
                            else
                                current.rot_left = 0;
                        }
                    }

                    if (data.Name.LocalName == "rot_right")
                    {
                        if (remotegame.networkid == networkid)
                        {
                            // move ghost instead
                        }
                        else
                        {
                            if (current.rot_right == 0)
                                current.rot_right = 1;
                            else
                                current.rot_right = 0;
                        }
                    }

                    // show an event we did not process and when the remote client scheduled it

                    remotegame.RaiseWriteLine(
                         data.Name.LocalName + " " + data
                     );
                };


            // preventing a bug?
            ApplicationSprite.__sprite.__game_onmessage += __sprite_game_onmessage;

        }

        public int network_rx = 0;

        public int networkframe = 0;

        // world stops at frame 0xffff! should wrap actually!
        public Action[] networksync_actions = new Action[0xffff];

        public List<RemoteGame> remotegames = new List<RemoteGame>();
    }

    public interface IRemoteGame
    {
        event Action<string> AtTitleChange;
        event Action<string> AtWriteLine;
    }

    public class RemoteGame : IRemoteGame
    {
        public Game Context;

        public int networkframe_dx
        {
            get
            {
                return Context.networkframe - this.networkframe + this.initial_networkframe_delta;
            }
        }

        public int rx_messagecount;

        public int networkid { get; set; }
        public int networkframe { get; set; }

        public int initial_networkframe_delta { get; set; }

        public event Action<string> AtTitleChange;
        public void RaiseTitleChange(string e)
        {
            if (AtTitleChange != null)
                AtTitleChange(e);
        }

        public event Action<string> AtWriteLine;
        public void RaiseWriteLine(string e)
        {
            if (AtWriteLine != null)
                AtWriteLine(e);
        }
    }
}
