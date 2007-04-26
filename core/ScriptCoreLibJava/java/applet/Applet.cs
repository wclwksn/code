using ScriptCoreLib;

using java.awt;
using java.net;

namespace java.applet
{
    [Script(IsNative = true)]
    public class Applet : Panel
    {
        public virtual void init()
        {

        }

        public void showStatus(string e)
        {
        }

        public void resize(int w, int h)
        {

        }

        public AppletContext getAppletContext()
        {
            return default(AppletContext);
        }

        /// <summary>
        /// Gets the URL of the document in which this applet is embedded.
        /// </summary>
        public URL DocumentBase
        {
            [Script(ExternalTarget = "get*")]
            get { return default(URL); }
        }

        public URL CodeBase
        {
            [Script(ExternalTarget = "get*")]
            get { return default(URL); }
        }

        /// <summary>
        /// Returns the value of the named parameter in the HTML tag.
        /// </summary>
        public string getParameter(string name)
        {
            return default(string);
        }

    }
}
