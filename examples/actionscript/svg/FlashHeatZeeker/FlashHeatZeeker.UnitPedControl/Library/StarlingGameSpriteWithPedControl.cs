﻿using Box2D.Common.Math;
using Box2D.Dynamics;
using FlashHeatZeeker.Core.Library;
using FlashHeatZeeker.CorePhysics.Library;
using FlashHeatZeeker.StarlingSetup.Library;
using FlashHeatZeeker.UnitJeepControl.Library;
using FlashHeatZeeker.UnitPed.Library;
using ScriptCoreLib.ActionScript.flash.geom;
using starling.display;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FlashHeatZeeker.UnitPedControl.Library
{

    class StarlingGameSpriteWithPedControl : StarlingGameSpriteWithPhysics
    {

        public StarlingGameSpriteWithPedControl()
        {
            var textures_ped = new StarlingGameSpriteWithPedTextures(new_tex_crop);


            this.onbeforefirstframe += (stage, s) =>
            {
                var patrol1 = new PhysicalPed(textures_ped, this)
                {

                    speed = 10
                };

                var physical0 = new PhysicalPed(textures_ped, this);
                current = physical0;

                // 32x32 = 15FPS?
                // 24x24 35?

                #region others
                for (int ix = 0; ix < 4; ix++)
                    for (int iy = 0; iy < 4; iy++)
                    {
                        var p = new PhysicalPed(textures_ped, this);

                        p.SetPositionAndAngle(
                            8 * ix, 8 * iy
                        );


                    }
                #endregion


                #region __keyDown
                var __keyDown = new KeySample();

                stage.keyDown +=
                   e =>
                   {
                       // http://circlecube.com/2008/08/actionscript-key-listener-tutorial/
                       if (e.altKey)
                           __keyDown[Keys.Alt] = true;

                       __keyDown[(Keys)e.keyCode] = true;
                   };

                stage.keyUp +=
                 e =>
                 {
                     if (!e.altKey)
                         __keyDown[Keys.Alt] = false;

                     __keyDown[(Keys)e.keyCode] = false;
                 };

                #endregion

                bool mode_changepending = false;


                onsyncframe += delegate
                {

                    if (syncframeid % 300 == 100)
                    {
                        patrol1.body.SetAngle(
                            45.DegreesToRadians()
                        );

                        var partol_commands = new KeySample();

                        partol_commands[Keys.Up] = true;

                        patrol1.SetVelocityFromInput(partol_commands);
                    }

                    if (syncframeid % 300 == 150)
                    {
                        var partol_commands = new KeySample();

                        //partol_commands[Keys.Left] = true;

                        patrol1.SetVelocityFromInput(partol_commands);
                    }

                    if (syncframeid % 300 == 200)
                    {
                        patrol1.body.SetAngle(
                            (180 + 45).DegreesToRadians()
                        );

                        var partol_commands = new KeySample();

                        partol_commands[Keys.Up] = true;

                        patrol1.SetVelocityFromInput(partol_commands);
                    }


                    if (syncframeid % 300 == 250)
                    {
                        var partol_commands = new KeySample();

                        //partol_commands[Keys.Left] = true;

                        patrol1.SetVelocityFromInput(partol_commands);
                    }

                    current.SetVelocityFromInput(__keyDown);

                    #region simulate a weapone!
                    if (__keyDown[Keys.ControlKey])
                        if (frameid % 20 == 0)
                        {
                            var bodyDef = new b2BodyDef();

                            bodyDef.type = Box2D.Dynamics.b2Body.b2_dynamicBody;

                            // stop moving if legs stop walking!
                            bodyDef.linearDamping = 0;
                            bodyDef.angularDamping = 0;
                            //bodyDef.angle = 1.57079633;
                            bodyDef.fixedRotation = true;

                            var body = ground_b2world.CreateBody(bodyDef);
                            body.SetPosition(
                                new b2Vec2(
                                    current.body.GetPosition().x + 2,
                                    current.body.GetPosition().y + 2
                                )
                            );

                            body.SetLinearVelocity(
                                   new b2Vec2(
                                     100,
                                    100
                                )
                            );

                            var fixDef = new Box2D.Dynamics.b2FixtureDef();
                            fixDef.density = 0.1;
                            fixDef.friction = 0.01;
                            fixDef.restitution = 0;


                            fixDef.shape = new Box2D.Collision.Shapes.b2CircleShape(1.0);


                            var fix = body.CreateFixture(fixDef);

                            //body.SetPosition(
                            //    new b2Vec2(0, -100 * 16)
                            //);
                        }
                    #endregion


                    #region mode
                    if (!__keyDown[Keys.Space])
                    {
                        // space is not down.
                        mode_changepending = true;
                    }
                    else
                    {
                        if (mode_changepending)
                        {
                            if (physical0.visual.LayOnTheGround)
                                physical0.visual.LayOnTheGround = false;
                            else
                                physical0.visual.LayOnTheGround = true;

                            mode_changepending = false;



                        }
                    }
                    #endregion



                };


            };



        }
    }
}