using System;
using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.Resources2
{
    // Token: 0x02000049 RID: 73
    internal interface IEncodeMode
    {
        // Token: 0x06000157 RID: 343
        IEnumerable<Instruction> EmitDecrypt(MethodDef init, REContext ctx, Local block, Local key);

        // Token: 0x06000158 RID: 344
        uint[] Encrypt(uint[] data, int offset, uint[] key);
    }
}
