using ScriptCoreLib.ActionScript.Extensions;
using ScriptCoreLib.ActionScript.flash.display;
using ScriptCoreLib.Extensions;

namespace FlashHeatZeeker.Core
{
    public sealed class ApplicationSprite : Sprite
    {

        public readonly ApplicationCanvas content = new ApplicationCanvas();

        public ApplicationSprite()
        {
            this.InvokeWhenStageIsReady(
                () =>
                {
                    content.AttachToContainer(this);
                    content.AutoSizeTo(this.stage);
                }
            );
        }
    }
}