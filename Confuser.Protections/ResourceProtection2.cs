using System;
using Confuser.Core;
using Confuser.Protections.Resources2;

namespace Confuser.Protections
{
    [BeforeProtection(new string[]
    {
        "Ki.ControlFlow",
        "Ki.ControlFlow"
    })]
    [AfterProtection(new string[]
    {
        "Ki.Constants"
    })]
    internal class ResourceProtection2 : Protection
    {
        protected override void Initialize(ConfuserContext context)
        {
        }
        
        protected override void PopulatePipeline(ProtectionPipeline pipeline)
        {
            pipeline.InsertPreStage(PipelineStage.ProcessModule, new InjectPhase(this));
        }
        
        public override string Description
        {
            get
            {
                return "This protection dynamically encodes and compresses the embedded resources.";
            }
        }
        
        public override string FullId
        {
            get
            {
                return "Ki.Resources2";
            }
        }
        
        public override string Id
        {
            get
            {
                return "dynamic resources";
            }
        }
        
        public override string Name
        {
            get
            {
                return "Dynamic Resource Protection";
            }
        }
        
        public override ProtectionPreset Preset
        {
            get
            {
                return ProtectionPreset.Normal;
            }
        }
        
        public const string _FullId = "Ki.Resources2";
        
        public const string _Id = "dy resources";
        
        public const string _ServiceId = "Ki.Resources2";
    }
}
