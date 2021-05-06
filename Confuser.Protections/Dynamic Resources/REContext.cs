using System;
using Confuser.Core;
using Confuser.Core.Services;
using Confuser.DynCipher;
using Confuser.Renamer;
using dnlib.DotNet;

namespace Confuser.Protections.Resources2
{
    // Token: 0x02000053 RID: 83
    internal class REContext
    {
        // Token: 0x040000B2 RID: 178
        public ConfuserContext Context;

        // Token: 0x040000B3 RID: 179
        public FieldDef DataField;

        // Token: 0x040000B4 RID: 180
        public FieldDef DataField1;

        // Token: 0x040000B5 RID: 181
        public TypeDef DataType;

        // Token: 0x040000B6 RID: 182
        public IDynCipherService DynCipher;

        // Token: 0x040000B7 RID: 183
        public MethodDef InitMethod;

        // Token: 0x040000B8 RID: 184
        public IMarkerService Marker;

        // Token: 0x040000B9 RID: 185
        public Mode Mode;

        // Token: 0x040000BA RID: 186
        public IEncodeMode ModeHandler;

        // Token: 0x040000BB RID: 187
        public ModuleDef Module;

        // Token: 0x040000BC RID: 188
        public INameService Name;

        // Token: 0x040000BD RID: 189
        public RandomGenerator Random;
    }
}
