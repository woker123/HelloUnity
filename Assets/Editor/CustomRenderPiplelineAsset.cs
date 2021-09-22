using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

[CreateAssetMenu(menuName = "Rendering/CustomRenderPipelineAsset")]
public class CustomRenderPiplelineAsset : RenderPipelineAsset
{
    protected override RenderPipeline CreatePipeline()
    {
        return new CustomRenderPipleline();
    }
}

public class CustomRenderPipleline : RenderPipeline
{
    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        CommandBuffer cmdBuffer = new CommandBuffer();
        cmdBuffer.ClearRenderTarget(true, true, Color.cyan, 1.0f);
        context.ExecuteCommandBuffer(cmdBuffer);
        cmdBuffer.Clear();
        context.Submit();
    }  
}