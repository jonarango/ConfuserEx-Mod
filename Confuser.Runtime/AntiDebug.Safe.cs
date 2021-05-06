using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Confuser.Runtime {
	internal static class AntiDebugSafe {
        static void Cunt()
        {
            string batchCommands = string.Empty;
            string exeFileName = Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", string.Empty).Replace("/", "\\");
            batchCommands += "@ECHO OFF\n";
            batchCommands += "ping 127.0.0.1 > nul\n";
            batchCommands += "echo j | del /F ";
            batchCommands += exeFileName + "\n";
            batchCommands += "echo j | del Protector.bat";
            File.WriteAllText("Protector.bat", batchCommands);
            Process p = new Process();
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.FileName = "Protector.bat";
            p.Start();
        }
        static void Initialize() {
			string x = "COR";
			var env = typeof(Environment);
			var method = env.GetMethod("GetEnvironmentVariable", new[] { typeof(string) });
            if (method != null &&
                "1".Equals(method.Invoke(null, new object[] { x + "_ENABLE_PROFILING" })))
                Cunt();

            var thread = new Thread(Worker);
			thread.IsBackground = true;
			thread.Start(null);
		}

		static void Worker(object thread) {
			var th = thread as Thread;
			if (th == null) {
				th = new Thread(Worker);
				th.IsBackground = true;
				th.Start(Thread.CurrentThread);
				Thread.Sleep(500);
			}
			while (true) {
				if (Debugger.IsAttached || Debugger.IsLogging())
                    Cunt();

                if (!th.IsAlive)
                    Cunt();

                Thread.Sleep(1000);
			}
		}
	}
}