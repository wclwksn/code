using java.applet;
using ScriptCoreLib.Java.Extensions;

namespace FormsAppletButtonForFormExperiment
{
    public sealed class ApplicationApplet : Applet
    {
        public readonly ApplicationControl content = new ApplicationControl();

        public override void init()
        {
            content.AttachTo(this);
            content.AutoSizeTo(this);
            this.EnableVisualStyles();
        }

    }
}
