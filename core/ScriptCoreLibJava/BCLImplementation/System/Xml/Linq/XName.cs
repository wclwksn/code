﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptCoreLib;
using System.Xml.Linq;

namespace ScriptCoreLibJava.BCLImplementation.System.Xml.Linq
{
	[Script(Implements = typeof(global::System.Xml.Linq.XName))]
	internal class __XName
	{
		public string InternalValue;

		public string LocalName
		{
			get
			{
				return InternalValue;
			}
		}
		public override string ToString()
		{
			return LocalName;
		}

		public static implicit operator __XName(string e)
		{
            return __XName.Get(e);
		}

        public static __XName Get(string e)
        {
            return new __XName { InternalValue = e };
        }

		public static XName Get(string localName, string namespaceName)
		{
			return (XName)(object)new __XName { InternalValue = localName };
		}
	}
}
