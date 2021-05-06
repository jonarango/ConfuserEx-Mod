using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Confuser.Runtime {
	static partial class AntiDebugAntinet {
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
			if (!InitializeAntiDebugger())
                Cunt();
            InitializeAntiProfiler();
			if (IsProfilerAttached) {
                Cunt();
                PreventActiveProfilerFromReceivingProfilingMessages();
			}
		}
	}
}