﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
[assembly: Obfuscation(Feature = "script")]
namespace TestAsString
{
    public class Class1
    {
        public static void foo0int(object e)
        {
            var xx = e is int;


        }

        public static void foo0double(object e)
        {
            var xx = e is double;

        }

        public static void foo0bool(object e)
        {
            var xx = e is bool;

        }


        public static void foo0(object e)
        {
            var xx = e is string;

  
        }

        public static void foo1(object e)
        {

            var x = e as string;

        }

        public static void foo2(object e)
        {
 

            var y = e as Class1;
        }
    }
}
