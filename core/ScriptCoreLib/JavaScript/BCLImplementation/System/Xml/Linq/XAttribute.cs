﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ScriptCoreLib.JavaScript.BCLImplementation.System.Xml.Linq
{
    // https://github.com/dotnet/corefx/blob/master/src/System.Xml.XDocument/System/Xml/Linq/XAttribute.cs

    [Script(Implements = typeof(XAttribute))]
    internal class __XAttribute : __XObject
    {
        internal __XContainer InternalElement;

        public XName Name { get; set; }

        public string InternalValue;



        public void Remove()
        {
            // X:\jsc.svn\examples\javascript\css\Test\CSSSearchUserFeedback\CSSSearchUserFeedback\Application.cs

            this.InternalElement.InternalElement.removeAttribute(this.Name.LocalName);
        }

        public string Value
        {
            get
            {
                if (this.InternalElement == null)
                    return InternalValue;

                return (string)this.InternalElement.InternalElement.getAttribute(this.Name.LocalName);
            }
            set
            {
                this.InternalElement.InternalElement.setAttribute(this.Name.LocalName, value);
            }
        }

        public __XAttribute(XName name, object value)
        {
            this.Name = name;
            this.InternalValue = value as string;

        }

        public static implicit operator XAttribute(__XAttribute a)
        {
            return (XAttribute)(object)a;
        }

        public static implicit operator __XAttribute(XAttribute a)
        {
            return (__XAttribute)(object)a;
        }


        // X:\jsc.svn\examples\javascript\p2p\RTCICELobby\RTCICELobby\Application.cs
        public static explicit operator int (__XAttribute attribute)
        {
            if (attribute == null)
                return default(int);


            return Convert.ToInt32(attribute.Value);
        }

        public static explicit operator bool (__XAttribute attribute)
        {
            // does jsc resolver also look at the retun type?

            // X:\jsc.svn\core\ScriptCoreLib.Windows.Forms\ScriptCoreLib.Windows.Forms\JavaScript\BCLImplementation\System\Windows\Forms\DataGridView\DataGridView..ctor.cs
            // X:\jsc.svn\examples\javascript\forms\FakeWindowsLoginExperiment\FakeWindowsLoginExperiment\Application.cs
            // X:\jsc.svn\examples\javascript\xml\test\TestXAttributeOp\TestXAttributeOp\Application.cs
            // X:\jsc.svn\examples\javascript\xml\test\TestBCLImplementationResolveByReturnType\TestBCLImplementationResolveByReturnType\Class1.cs

            return Convert.ToBoolean(attribute.Value);
        }

        public override string ToString()
        {
            // X:\jsc.svn\examples\javascript\CSS\CSSConditionalScroll\CSSConditionalScroll\Application.cs
            return this.Value;
        }
    }
}
