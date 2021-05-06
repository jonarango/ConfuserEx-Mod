using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using Confuser.Core;
using Confuser.Core.Helpers;
using Confuser.Core.Services;
using Confuser.Renamer;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Microsoft.CSharp;

namespace Confuser.Protections
{
    internal class NativePackerProtection : Protection
    {
        public const string _Id = "packer";
        public const string _FullId = "Ki.NativePacker";

        public override string Name
        {
            get { return "Packer Protection"; }
        }

        public override string Description
        {
            get { return "Packs your file. Use this as a single protection."; }
        }

        public override string Id
        {
            get { return _Id; }
        }

        public override string FullId
        {
            get { return _FullId; }
        }

        public override ProtectionPreset Preset
        {
            get { return ProtectionPreset.Maximum; }
        }

        protected override void Initialize(ConfuserContext context)
        {
            //
        }

        protected override void PopulatePipeline(ProtectionPipeline pipeline)
        {
            pipeline.InsertPreStage(PipelineStage.ProcessModule, new PackPhase(this));
        }

        class PackPhase : ProtectionPhase
        {
            public PackPhase(NativePackerProtection parent)
                : base(parent) { }

            public override ProtectionTargets Targets
            {
                get { return ProtectionTargets.Methods; }
            }

            public override string Name
            {
                get { return "Native Packer"; }
            }
            private static Random random = new Random();
            public static string RandomString(int length)
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789==abcdefghijklmnopqrstuvwxyz///+++";
                return new string(Enumerable.Repeat(chars, length)
                  .Select(s => s[random.Next(s.Length)]).ToArray());
            }

            protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
            {
                try
                {
                    string kappa = File.ReadAllText(Directory.GetCurrentDirectory() + "\\Packer.txt");
                    string FilePath = kappa.Split(';')[0];
                    string CleanPath = kappa.Split(';')[1];
                    string FileName = Path.GetFileName(FilePath);
                    byte[] assembly = File.ReadAllBytes(FilePath);
                    RijndaelManaged rijndaelManaged = new RijndaelManaged();
                    rijndaelManaged.KeySize = 256;
                    rijndaelManaged.GenerateIV();
                    rijndaelManaged.GenerateKey();
                    string newValue = Convert.ToBase64String(rijndaelManaged.Key);
                    string newValue2 = Convert.ToBase64String(rijndaelManaged.IV);
                    MemoryStream memoryStream = new MemoryStream();
                    rijndaelManaged.Padding = PaddingMode.ISO10126;
                    CryptoStream cryptoStream = new CryptoStream(memoryStream, rijndaelManaged.CreateEncryptor(), CryptoStreamMode.Write);
                    cryptoStream.Write(assembly, 0, assembly.Length);
                    cryptoStream.FlushFinalBlock();
                    cryptoStream.Flush();
                    memoryStream.Seek(0L, SeekOrigin.Begin);
                    string newValue3 = Convert.ToBase64String(memoryStream.ToArray());
                    cryptoStream.Close();
                    memoryStream.Close();
                    string text = Properties.Resources.StubCode;
                    text = text.Replace("%KEY%", newValue);
                    text = text.Replace("%IV%", newValue2);
                    text = text.Replace("%PROGRAM%", newValue3);
                    text = text.Replace("%lV%", RandomString(3000));
                    Assembly assembly2 = Assembly.Load(assembly);
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();
                    dictionary.Add("CompilerVersion", "v2.5");
                    goto IL_120;
                IL_120:
                    CSharpCodeProvider csharpCodeProvider = new CSharpCodeProvider(dictionary);
                    CompilerParameters compilerParameters = new CompilerParameters();
                    compilerParameters.CompilerOptions = "/target:winexe";
                    foreach (AssemblyName assemblyName in assembly2.GetReferencedAssemblies())
                    {
                        if (assemblyName.Name.Contains("System.") || assemblyName.Name.Contains("Microsoft."))
                        {
                            compilerParameters.ReferencedAssemblies.Add(assemblyName.Name + ".dll");
                        }
                    }
                    string outputAssembly = Path.GetTempPath() + Guid.NewGuid().ToString() + ".exe";
                    compilerParameters.GenerateExecutable = true;
                    compilerParameters.OutputAssembly = outputAssembly;
                    CompilerResults compilerResults = csharpCodeProvider.CompileAssemblyFromSource(compilerParameters, new string[]
                    {
                text
                    });
                    byte[] result;
                    try
                    {
                        FileStream fileStream = compilerResults.CompiledAssembly.GetFiles()[0];
                        byte[] array = new byte[fileStream.Length];
                        int num = fileStream.Read(array, 0, array.Length);
                        fileStream.Close();
                        string name = fileStream.Name;
                        fileStream.Dispose();
                        if (num == array.Length)
                        {
                            result = array;
                        }
                        else
                        {
                            result = null;
                        }
                    }
                    catch
                    {
                        result = null;
                    }
                    Directory.CreateDirectory(CleanPath + "\\Confused\\Packed");
                    File.WriteAllBytes(CleanPath + "\\Confused\\Packed\\" + FileName, result);
                    File.Delete(Directory.GetCurrentDirectory() + "\\Packer.txt");
                }
                catch
                {

                }
            }
        }
    }
}