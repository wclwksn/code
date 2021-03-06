﻿using System;
using System.Collections.Generic;
using System.Text;
using ScriptCoreLib;

namespace ScriptCoreLibJava.BCLImplementation.System.Threading
{
    // http://referencesource.microsoft.com/#mscorlib/system/threading/thread.cs
    // https://github.com/dotnet/coreclr/blob/master/src/mscorlib/src/System/Threading/Thread.cs
    // https://github.com/dotnet/corefx/blob/master/src/System.Diagnostics.Process/src/System/Diagnostics/ProcessThread.cs
    // https://github.com/mono/mono/blob/master/mcs/class/corlib/System.Threading/Thread.cs

    // https://github.com/Reactive-Extensions/IL2JS/blob/master/mscorlib/System/Threading/Thread.cs
    // https://github.com/kswoll/WootzJs/blob/master/WootzJs.Runtime/Activator.cs
    // https://github.com/konsoletyper/teavm/blob/master/teavm-classlib/src/main/java/org/teavm/classlib/java/lang/TThread.java

    // X:\jsc.svn\core\ScriptCoreLibJava\BCLImplementation\System\Threading\Thread.cs
    // X:\jsc.svn\core\ScriptCoreLib\ActionScript\BCLImplementation\System\Threading\Thread.cs
    // X:\jsc.svn\core\ScriptCoreLib\JavaScript\BCLImplementation\System\Threading\Thread.cs
    // X:\jsc.svn\core\ScriptCoreLibNative\ScriptCoreLibNative\BCLImplementation\System\Threading\Thread.cs

    [Script(Implements = typeof(global::System.Threading.Thread))]
    public class __Thread
    {
        // X:\jsc.svn\examples\java\appengine\Test\TestThreadManager\TestThreadManager\ApplicationWebService.cs
        // would this enable async queries?

        // if we are to target appengine
        // Create a thread factory by calling com.google.appengine.api.ThreadManager.currentRequestThreadFactory()
        //call the factory's newRequestThread method, passing in the Runnable, newRequestThread(runnable)
        // https://cloud.google.com/appengine/docs/java/

        // https://software.intel.com/en-us/blogs/2014/07/23/will-my-android-app-still-run-with-art-instead-of-dalvik
        // Watch the size of the ART unified Thread stack which should be about equal to the two Dalvik stacks (by default a 32 KB Java stack and a 1 MB native stack).
        // http://stackoverflow.com/questions/23739261/does-android-art-support-runtime-dynamic-class-loading-just-like-dalvik

        // X:\jsc.svn\examples\c\Test\TestThreadStart\TestThreadStart\BCLImplementation.cs

        // what about hoping to UI thread on android?
        // X:\jsc.svn\examples\java\hybrid\JVMCLRHopToThreadPool\JVMCLRHopToThreadPool\Program.cs
        // X:\jsc.svn\examples\javascript\Test\TestHopToThreadPoolAwaitable\TestHopToThreadPoolAwaitable\Application.cs
        // X:\jsc.svn\examples\javascript\android\CallSetContentView\CallSetContentView\ApplicationWebService.cs
        // how can we hop back?
        // X:\jsc.svn\core\ScriptCoreLibAndroid\ScriptCoreLibAndroid\Extensions\ActivityExtensions.cs
        // X:\jsc.svn\examples\javascript\android\AndroidPINForm\AndroidPINForm\ApplicationWebService.cs

        public java.lang.Thread InternalValue;

        [Script]
        class RunnableHandler : java.lang.Runnable
        {
            public Action Handler;

            public void run()
            {
                Handler();
            }
        }


        // X:\jsc.svn\core\ScriptCoreLibJava.AppEngine\ScriptCoreLibJava.AppEngine\Extensions\ThreadManagerExtensions.cs
        public static Func<java.lang.Runnable, java.lang.Thread> new_java_lang_Thread = yy => new java.lang.Thread(yy);


        #region ctor
        internal __Thread()
        {

        }

        public __Thread(global::System.Threading.ThreadStart t)
        {
            InternalValue = new_java_lang_Thread(
                new RunnableHandler
            {
                Handler =
                        delegate
                {
                    t();
                }
            }
            );
        }

        public __Thread(global::System.Threading.ParameterizedThreadStart t)
        {
            InternalValue = new_java_lang_Thread(
                      new RunnableHandler
            {
                Handler =
                              delegate
                {
                    t(InternalParameterizedThreadStart);
                }
            }
                  );
        }
        #endregion


        public static implicit operator global::System.Threading.Thread(__Thread e)
        {
            return (global::System.Threading.Thread)(object)e;
        }

        public int ManagedThreadId
        {
            get
            {
                return (int)this.InternalValue.getId();
            }
        }


        public static global::System.Threading.Thread CurrentThread
        {
            get
            {
                return new __Thread { InternalValue = java.lang.Thread.currentThread() };

            }
        }

        public static void Sleep(int millisecondsTimeout)
        {
            try
            {
                java.lang.Thread.sleep(millisecondsTimeout);
            }
            catch
            {

                throw;
            }
        }

        public object InternalParameterizedThreadStart;

        public void Start(object parameter)
        {
            InternalParameterizedThreadStart = parameter;
            InternalValue.start();
        }

        public void Start()
        {
            InternalValue.start();
        }

        public string Name
        {
            get
            {
                return InternalValue.getName();
            }
            set
            {
                InternalValue.setName(value);
            }
        }

        public bool IsAlive
        {
            get
            {
                return InternalValue.isAlive();
            }
        }

        public bool IsBackground { get { return InternalValue.isDaemon(); } set { InternalValue.setDaemon(value); } }

        public void Join()
        {
            try
            {
                InternalValue.join();
            }
            catch
            {
                throw;
            }
        }

        public bool Join(int ms)
        {
            try
            {
                InternalValue.join(ms);
            }
            catch
            {
                throw;
            }

            return !InternalValue.isAlive();
        }

        public void Abort()
        {
            InternalValue.stop();
        }
    }
}
