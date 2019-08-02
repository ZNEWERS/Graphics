using System;
using UnityEditor.Graphing;
using UnityEngine;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    class SamplerStateMaterialSlot : MaterialSlot
    {
        [SerializeField]
        private int m_TextureSlot = -1;

        public int textureSlotId
        {
            get => m_TextureSlot;
            set => m_TextureSlot = value;
        }

        public SamplerStateMaterialSlot()
        {
        }

        public SamplerStateMaterialSlot(
            int slotId,
            string displayName,
            string shaderOutputName,
            SlotType slotType,
            ShaderStageCapability stageCapability = ShaderStageCapability.All,
            bool hidden = false)
            : base(slotId, displayName, shaderOutputName, slotType, stageCapability, hidden)
        {
        }

        public override string GetDefaultValue(GenerationMode generationMode)
        {
            var matOwner = owner as AbstractMaterialNode;
            if (matOwner == null)
                throw new Exception(string.Format("Slot {0} either has no owner, or the owner is not a {1}", this, typeof(AbstractMaterialNode)));

            var textureSlot = matOwner.FindInputSlot<MaterialSlot>(textureSlotId);
            return textureSlot != null && (textureSlot.isConnected
                || (textureSlot is Texture2DInputMaterialSlot texture2DSlot && texture2DSlot.texture != null)
                || (textureSlot is Texture2DArrayInputMaterialSlot texture2DArraySlot && texture2DArraySlot.textureArray != null)
                || (textureSlot is Texture3DInputMaterialSlot texture3DSlot && texture3DSlot.texture != null)
                || (textureSlot is CubemapInputMaterialSlot cubemapSlot && cubemapSlot.cubemap != null))
                ? $"sampler{matOwner.GetSlotValue(textureSlotId, generationMode)}"
                : SamplerStateShaderProperty.GetBuiltinSamplerName(TextureSamplerState.FilterMode.Linear, TextureSamplerState.WrapMode.Repeat);
        }

        public override SlotValueType valueType { get { return SlotValueType.SamplerState; } }
        public override ConcreteSlotValueType concreteValueType { get { return ConcreteSlotValueType.SamplerState; } }
        public override void AddDefaultProperty(PropertyCollector properties, GenerationMode generationMode)
        {
            var matOwner = owner as AbstractMaterialNode;
            if (matOwner == null)
                throw new Exception(string.Format("Slot {0} either has no owner, or the owner is not a {1}", this, typeof(AbstractMaterialNode)));

            properties.AddShaderProperty(new SamplerStateShaderProperty()
            {
                overrideReferenceName = GetDefaultValue(generationMode),
                generatePropertyBlock = false,
            });
        }

        public override void CopyValuesFrom(MaterialSlot foundSlot)
        {
        }
    }
}
