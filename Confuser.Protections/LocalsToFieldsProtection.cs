using System;
using System.Collections.Generic;
using System.Linq;
using Confuser.Core;
using Confuser.Core.Helpers;
using Confuser.Core.Services;
using Confuser.Renamer;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections
{
    [BeforeProtection("Ki.ControlFlow")]
    internal class LocalsToFieldsProtection : Protection
    {
        public const string _Id = "lcltofield";
        public const string _FullId = "Ki.LocalsToField";

        public override string Name
        {
            get { return "Locals-to-Field Protection"; }
        }

        public override string Description
        {
            get { return "This protection converts all locals to fields."; }
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
            pipeline.InsertPreStage(PipelineStage.ProcessModule, new ConvertionPhase(this));
        }

        class ConvertionPhase : ProtectionPhase
        {
            public ConvertionPhase(LocalsToFieldsProtection parent)
                : base(parent) { }

            public override ProtectionTargets Targets
            {
                get { return ProtectionTargets.Methods; }
            }

            public override string Name
            {
                get { return "l-to-f convertion"; }
            }

            Dictionary<Local, FieldDef> convertedLocals = new Dictionary<Local, FieldDef>();
            protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
            {
                foreach (var method in parameters.Targets.OfType<MethodDef>())
                {
                    if (!method.HasBody) continue;
                    method.Body.SimplifyMacros(method.Parameters);
                    var instructions = method.Body.Instructions;
                    for (int i = 0; i < instructions.Count; i++)
                    {
                        if (instructions[i].Operand is Local local)
                        {
                            FieldDef def = null;
                            if (!convertedLocals.ContainsKey(local))
                            {
                                def = new FieldDefUser(/*NameService.GenName()*/"[ xnoobx.cc ]", new FieldSig(local.Type), FieldAttributes.Public | FieldAttributes.Static);
                                context.CurrentModule.GlobalType.Fields.Add(def);
                                convertedLocals.Add(local, def);
                            }
                            else
                                def = convertedLocals[local];


                            OpCode eq = null;
                            switch (instructions[i].OpCode.Code)
                            {
                                case Code.Ldloc:
                                    eq = OpCodes.Ldsfld;
                                    break;
                                case Code.Ldloca:
                                    eq = OpCodes.Ldsflda;
                                    break;
                                case Code.Stloc:
                                    eq = OpCodes.Stsfld;
                                    break;
                            }
                            instructions[i].OpCode = eq;
                            instructions[i].Operand = def;

                        }
                    }
                    convertedLocals.ToList().ForEach(x => method.Body.Variables.Remove(x.Key));
                    convertedLocals = new Dictionary<Local, FieldDef>();
                }
            }
        }
    }
}