using System;
using System.Collections.Generic;
using Confuser.DynCipher.AST;
using Confuser.DynCipher.Generation;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.Resources2
{
    // Token: 0x02000047 RID: 71
    internal class DynamicMode : IEncodeMode
    {
        // Token: 0x06000152 RID: 338 RVA: 0x0000B870 File Offset: 0x00009A70
        public IEnumerable<Instruction> EmitDecrypt(MethodDef init, REContext ctx, Local block, Local key)
        {
            StatementBlock statement;
            StatementBlock statement2;
            ctx.DynCipher.GenerateCipherPair(ctx.Random, out statement, out statement2);
            List<Instruction> list = new List<Instruction>();
            DynamicMode.CodeGen codeGen = new DynamicMode.CodeGen(block, key, init, list);
            codeGen.GenerateCIL(statement2);
            codeGen.Commit(init.Body);
            DMCodeGen dmcodeGen = new DMCodeGen(typeof(void), new Tuple<string, Type>[]
            {
                Tuple.Create<string, Type>("{BUFFER}", typeof(uint[])),
                Tuple.Create<string, Type>("{KEY}", typeof(uint[]))
            });
            dmcodeGen.GenerateCIL(statement);
            this.encryptFunc = dmcodeGen.Compile<Action<uint[], uint[]>>();
            return list;
        }

        // Token: 0x06000153 RID: 339 RVA: 0x0000B91C File Offset: 0x00009B1C
        public uint[] Encrypt(uint[] data, int offset, uint[] key)
        {
            uint[] array = new uint[key.Length];
            Buffer.BlockCopy(data, offset * 4, array, 0, key.Length * 4);
            this.encryptFunc(array, key);
            return array;
        }

        // Token: 0x04000095 RID: 149
        private Action<uint[], uint[]> encryptFunc;

        // Token: 0x02000048 RID: 72
        private class CodeGen : CILCodeGen
        {
            // Token: 0x06000155 RID: 341 RVA: 0x0000507A File Offset: 0x0000327A
            public CodeGen(Local block, Local key, MethodDef init, IList<Instruction> instrs) : base(init, instrs)
            {
                this.block = block;
                this.key = key;
            }

            // Token: 0x06000156 RID: 342 RVA: 0x0000B958 File Offset: 0x00009B58
            protected override Local Var(Variable var)
            {
                bool flag = var.Name == "{BUFFER}";
                Local result;
                if (flag)
                {
                    result = this.block;
                }
                else
                {
                    bool flag2 = var.Name == "{KEY}";
                    if (flag2)
                    {
                        result = this.key;
                    }
                    else
                    {
                        result = base.Var(var);
                    }
                }
                return result;
            }

            // Token: 0x04000096 RID: 150
            private readonly Local block;

            // Token: 0x04000097 RID: 151
            private readonly Local key;
        }
    }
}
