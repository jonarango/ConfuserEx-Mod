using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Confuser.Core;
using Confuser.Core.Helpers;
using Confuser.Core.Services;
using Confuser.Renamer;
using dnlib.DotNet;
using dnlib.DotNet.MD;
using dnlib.DotNet.Writer;

namespace Confuser.Protections.Resources2
{
    // Token: 0x0200004E RID: 78
    internal class MDPhase
    {
        // Token: 0x06000164 RID: 356 RVA: 0x00005095 File Offset: 0x00003295
        public MDPhase(REContext ctx)
        {
            this.ctx = ctx;
        }

        // Token: 0x06000165 RID: 357 RVA: 0x000050A6 File Offset: 0x000032A6
        public void Hook()
        {
            this.ctx.Context.CurrentModuleWriterListener.OnWriterEvent += this.OnWriterEvent;
        }

        // Token: 0x06000166 RID: 358 RVA: 0x0000C158 File Offset: 0x0000A358
        private void OnWriterEvent(object sender, ModuleWriterListenerEventArgs e)
        {
            ModuleWriterBase moduleWriterBase = (ModuleWriterBase)sender;
            bool flag = e.WriterEvent == ModuleWriterEvent.MDBeginAddResources;
            if (flag)
            {
                this.ctx.Context.CheckCancellation();
                this.ctx.Context.Logger.Debug("Encrypting resources...");
                bool flag2 = this.ctx.Context.Packer != null;
                List<EmbeddedResource> list = this.ctx.Module.Resources.OfType<EmbeddedResource>().ToList<EmbeddedResource>();
                bool flag3 = !flag2;
                if (flag3)
                {
                    this.ctx.Module.Resources.RemoveWhere((Resource res) => res is EmbeddedResource);
                }
                string text = this.ctx.Name.RandomName(RenameMode.Letters);
                PublicKey publicKey = null;
                bool flag4 = moduleWriterBase.TheOptions.StrongNameKey != null;
                if (flag4)
                {
                    publicKey = PublicKeyBase.CreatePublicKey(moduleWriterBase.TheOptions.StrongNameKey.PublicKey);
                }
                AssemblyDefUser assemblyDefUser = new AssemblyDefUser(text, new Version(0, 0), publicKey);
                assemblyDefUser.Modules.Add(new ModuleDefUser(text + ".dll"));
                ModuleDef manifestModule = assemblyDefUser.ManifestModule;
                assemblyDefUser.ManifestModule.Kind = ModuleKind.Dll;
                AssemblyRefUser asmRef = new AssemblyRefUser(manifestModule.Assembly);
                bool flag5 = !flag2;
                if (flag5)
                {
                    foreach (EmbeddedResource embeddedResource in list)
                    {
                        embeddedResource.Attributes = ManifestResourceAttributes.Public;
                        manifestModule.Resources.Add(embeddedResource);
                        this.ctx.Module.Resources.Add(new AssemblyLinkedResource(embeddedResource.Name, asmRef, embeddedResource.Attributes));
                    }
                }
                byte[] array;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    manifestModule.Write(memoryStream, new ModuleWriterOptions
                    {
                        StrongNameKey = moduleWriterBase.TheOptions.StrongNameKey
                    });
                    array = memoryStream.ToArray();
                }
                array = this.ctx.Context.Registry.GetService<ICompressionService>().Compress(array, delegate (double progress)
                {
                    this.ctx.Context.Logger.Progress((int)(progress * 10000.0), 10000);
                });
                this.ctx.Context.Logger.EndProgress();
                this.ctx.Context.CheckCancellation();
                uint num = (uint)((array.Length + 3) / 4);
                num = (num + 15u & 4294967280u);
                uint[] array2 = new uint[num];
                Buffer.BlockCopy(array, 0, array2, 0, array.Length);
                uint num2 = this.ctx.Random.NextUInt32() | 16u;
                uint[] array3 = new uint[16];
                uint num3 = num2;
                for (int i = 0; i < 16; i++)
                {
                    num3 ^= num3 >> 13;
                    num3 ^= num3 << 25;
                    num3 ^= num3 >> 27;
                    array3[i] = num3;
                }
                byte[] array4 = new byte[array2.Length * 4];
                for (int j = 0; j < array2.Length; j += 16)
                {
                    uint[] src = this.ctx.ModeHandler.Encrypt(array2, j, array3);
                    for (int k = 0; k < 16; k++)
                    {
                        array3[k] ^= array2[j + k];
                    }
                    Buffer.BlockCopy(src, 0, array4, j * 4, 64);
                }
                uint num4 = (uint)array4.Length;
                TablesHeap tablesHeap = moduleWriterBase.MetaData.TablesHeap;
                tablesHeap.ClassLayoutTable[moduleWriterBase.MetaData.GetClassLayoutRid(this.ctx.DataType)].ClassSize = num4;
                RawFieldRow rawFieldRow = tablesHeap.FieldTable[moduleWriterBase.MetaData.GetRid(this.ctx.DataField)];
                RawFieldRow rawFieldRow2 = rawFieldRow;
                rawFieldRow2.Flags |= 256;
                this.encryptedResource = moduleWriterBase.Constants.Add(new ByteArrayChunk(array4), 8u);
                MutationHelper.InjectKeys(this.ctx.InitMethod, new int[]
                {
                    0,
                    1
                }, new int[]
                {
                    (int)(num4 / 4u),
                    (int)num2
                });
            }
            else
            {
                bool flag6 = e.WriterEvent == ModuleWriterEvent.EndCalculateRvasAndFileOffsets;
                if (flag6)
                {
                    TablesHeap tablesHeap2 = moduleWriterBase.MetaData.TablesHeap;
                    tablesHeap2.FieldRVATable[moduleWriterBase.MetaData.GetFieldRVARid(this.ctx.DataField)].RVA = (uint)this.encryptedResource.RVA;
                }
            }
        }

        // Token: 0x0400009E RID: 158
        private readonly REContext ctx;

        // Token: 0x0400009F RID: 159
        private ByteArrayChunk encryptedResource;
    }
}
