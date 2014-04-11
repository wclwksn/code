﻿using ScriptCoreLib;
using ScriptCoreLib.Shared.BCLImplementation.System.Collections.Specialized;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace ScriptCoreLibJava.BCLImplementation.System.Web
{
    [Script(Implements = typeof(global::System.Web.HttpFileCollection))]
    internal sealed class __HttpFileCollection : __NameObjectCollectionBase
    {
        public Dictionary<string, __HttpPostedFile> InternalDictionary = new Dictionary<string, __HttpPostedFile>();

        public __HttpFileCollection()
        {

        }
        public string[] AllKeys { get { return InternalDictionary.Keys.AsEnumerable().ToArray(); } }

        public HttpPostedFile this[string name] { get { return (HttpPostedFile)(object)InternalDictionary[name]; } }
    }
}