using FlashHeatZeeker.PromotionPreloader;
using FlashHeatZeeker.TestDrivers.Library;
using FlashHeatZeeker.TestDriversWithAudio.Library;
using ScriptCoreLib.ActionScript;
using ScriptCoreLib.ActionScript.Extensions;
using ScriptCoreLib.ActionScript.flash.display;
using ScriptCoreLib.Extensions;
using ScriptCoreLib.Shared;
using System;
using System.Windows;
using System.Windows.Forms;

namespace FlashHeatZeeker.TestDriversTouch
{
    public class ApplicationSpritePreloader : FlashHeatZeeker.PromotionPreloader.ApplicationSpritePreloader
    {
        [TypeOfByNameOverride]
        public override Type GetTargetType()
        {
            return typeof(ApplicationSprite);
        }
    }

    [Frame(typeof(ApplicationSpritePreloader))]
    [SWF(backgroundColor = 0xB27D51)]
    public sealed class ApplicationSprite : Sprite, IAlternator
    {
        public string Alternate { get; set; }

        public ApplicationSprite()
        {
            this.InvokeWhenPromotionIsReady(
              delegate
              {
                  ApplicationCanvas content1 = new ApplicationCanvas();




                  FlashHeatZeeker.TestDriversWithAudio.Library.StarlingGameSpriteWithTestDriversWithAudio.HudPadding =
               4 + content1.fingersize + 4 + content1.fingersize + 4;

                  var content0 = new FlashHeatZeeker.TestDriversWithAudio.ApplicationSpriteContent();


                  content0.AttachTo(this);

                  content1.r.Opacity = 0;

                  var InactiveOpaciy = 0.07;


                  #region bind
                  Action<UIElement, Keys> bind =
                      (ui, key) =>
                      {
                          //                    Implementation not found for type import :
                          //type: System.Windows.UIElement
                          //method: Void add_MouseDown(System.Windows.Input.MouseButtonEventHandler)

                          //ui.MouseDown +=
                          ui.Opacity = InactiveOpaciy;

                          ui.MouseLeftButtonDown +=
                              (sender, e) =>
                              {
                                  ui.Opacity = 1;
                                  e.Handled = true;
                                  StarlingGameSpriteWithTestDriversWithAudio.__keyDown[key] = true;
                              };

                          //ui.MouseUp +=
                          ui.MouseLeftButtonUp +=
                             (sender, e) =>
                             {
                                 ui.Opacity = InactiveOpaciy;
                                 e.Handled = true;
                                 StarlingGameSpriteWithTestDriversWithAudio.__keyDown[key] = false;
                             };

                          ui.TouchDown +=
                            (sender, e) =>
                            {
                                ui.Opacity = 1;
                                e.Handled = true;
                                StarlingGameSpriteWithTestDriversWithAudio.__keyDown[key] = true;
                            };

                          ui.TouchUp +=
                             (sender, e) =>
                             {
                                 ui.Opacity = InactiveOpaciy;
                                 e.Handled = true;
                                 StarlingGameSpriteWithTestDriversWithAudio.__keyDown[key] = false;
                             };
                      };
                  #endregion
                  bind(content1.up, Keys.Up);
                  bind(content1.down, Keys.Down);
                  bind(content1.left, Keys.Left);
                  bind(content1.right, Keys.Right);
                  bind(content1.space, Keys.Space);
                  bind(content1.control, Keys.ControlKey);
                  bind(content1.enter, Keys.Enter);


                  this.InvokeWhenStageIsReady(
                      () =>
                      {
                          content1.AttachToContainer(this);
                          content1.AutoSizeTo(this.stage);
                      }
                  );
              }
            );
        }
    }


}
