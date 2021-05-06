using System;
using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.Resources2
{
    // Token: 0x02000051 RID: 81
    internal class NormalMode : IEncodeMode
    {
        // Token: 0x0600016B RID: 363 RVA: 0x00005101 File Offset: 0x00003301
        public IEnumerable<Instruction> EmitDecrypt(MethodDef init, REContext ctx, Local block, Local key)
        {
            int num;
            for (int i = 0; i < 16; i = num + 1)
            {
                yield return Instruction.Create(OpCodes.Ldloc, block);
                yield return Instruction.Create(OpCodes.Ldc_I4, i);
                yield return Instruction.Create(OpCodes.Ldloc, block);
                yield return Instruction.Create(OpCodes.Ldc_I4, i);
                yield return Instruction.Create(OpCodes.Ldelem_U4);
                yield return Instruction.Create(OpCodes.Ldloc, key);
                yield return Instruction.Create(OpCodes.Ldc_I4, i);
                yield return Instruction.Create(OpCodes.Ldelem_U4);
                yield return Instruction.Create(OpCodes.Xor);
                yield return Instruction.Create(OpCodes.Stelem_I4);
                num = i;
            }
            yield break;
        }

        // Token: 0x0600016C RID: 364 RVA: 0x0000B5A0 File Offset: 0x000097A0
        public uint[] Encrypt(uint[] data, int offset, uint[] key)
        {
            uint[] array = new uint[key.Length];
            for (int i = 0; i < key.Length; i++)
            {
                array[i] = (data[i + offset] ^ key[i]);
            }
            return array;
        }
    }
}
