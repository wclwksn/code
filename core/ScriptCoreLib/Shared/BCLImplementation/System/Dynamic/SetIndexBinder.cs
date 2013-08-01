﻿using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScriptCoreLib.Shared.BCLImplementation.System.Dynamic
{
    [Script(Implements = typeof(global::System.Dynamic.SetIndexBinder))]
    public class __SetIndexBinder : __DynamicMetaObjectBinder
    {
        public CSharpBinderFlags flags;

      
        public Type context;
        public IEnumerable<CSharpArgumentInfo> argumentInfo;

        public override string ToString()
        {
            return "SetIndexBinder";
        }
    }
}