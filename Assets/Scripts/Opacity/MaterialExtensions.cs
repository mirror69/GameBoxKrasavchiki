using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MaterialExtensions
{
    //private const float MinOffset = -1.13f;
    //private const float MaxOffset = 1.28f;
    private const float HighestDissolveOffset = float.MaxValue;

    public static void SetZWriteEnabled(this Material material, bool enabled)
    {
        if (material.HasProperty("_ZWrite"))
        {
            material.SetInt("_ZWrite", enabled ? 1 : 0);
        }
    }

    public static void SetOpacity(this Material material, float value)
    {
        if (material.HasProperty("_Opacity"))
        {
            material.SetFloat("_Opacity", value);
        }
        else if (material.HasProperty("_BaseColor"))
        {
            Color color = material.GetColor("_BaseColor");
            color.a = value;
            material.SetColor("_BaseColor", color);
        }
        else if (material.HasProperty("_Dissolve"))
        {
            material.SetFloat("_Dissolve", 1 - value);
        }
    }
    public static void SetDirectionDissolve(this Material material, float value, float minOffset,
        float maxOffset)
    {
        if (material.HasProperty("_DissolveOffset"))
        {
            Vector4 vector = Vector4.zero;
            vector.y = value >= 1 ? HighestDissolveOffset : minOffset + (maxOffset - minOffset) * value;
            //if (value >= 1)
            //{
            //    vector.y = MinOffset + (MaxOffset - MinOffset) * value;
            //}
            //Vector4 vector = new Vector4(0, MinOffset + (MaxOffset - MinOffset) * value, 0);
            material.SetVector("_DissolveOffset", vector);
        }
    }


    public static void ToOpaqueMode(this Material material)
    {
        material.SetOverrideTag("RenderType", "");
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        material.SetInt("_ZWrite", 1);
        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = -1;
    }

    public static void ToFadeMode(this Material material)
    {
        material.SetOverrideTag("RenderType", "Transparent");
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 1);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }
}
