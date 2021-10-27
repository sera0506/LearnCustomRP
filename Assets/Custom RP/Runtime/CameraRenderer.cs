using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public partial class CameraRenderer
{
    const string bufferName = "Render Camera";
    private static ShaderTagId unlitShaderTag = new ShaderTagId("SRPDefaultUnlit");

    private CommandBuffer commandBuffer = new CommandBuffer { name = bufferName };
    private ScriptableRenderContext context;
    private Camera camera;
    private CullingResults cullingResults;

    public void Render(ScriptableRenderContext _context, Camera _camera)
    {
        context = _context;
        camera = _camera;

        PrepareBuffer();
        PrepareForSceneWindow();
        if (!Cull())
        {
            return;
        }

        Setup();
        DrawVisibleGeometry();
        DrawUnsupportedShaders();
        DrawGizmos();
        Submit();
    }

    private bool Cull()
    {
        if(camera.TryGetCullingParameters(out ScriptableCullingParameters scriptableCullingParameters))
        {
            cullingResults = context.Cull(ref scriptableCullingParameters);
            return true;
        }

        return false;
    }

    private void Setup()
    {
        context.SetupCameraProperties(camera);
        CameraClearFlags flags = camera.clearFlags;
        commandBuffer.ClearRenderTarget(flags <= CameraClearFlags.Depth, 
            flags == CameraClearFlags.Color, 
            flags == CameraClearFlags.Color ? camera.backgroundColor.linear : Color.clear);
        commandBuffer.BeginSample(SampleName);
        ExecuteBuffer();
    }

    private void Submit()
    {
        commandBuffer.EndSample(SampleName);
        ExecuteBuffer();
        context.Submit();
    }

    private void ExecuteBuffer()
    {
        context.ExecuteCommandBuffer(commandBuffer);
        commandBuffer.Clear();
    }

    private void DrawVisibleGeometry()
    {

        //不透明物件
        var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);

        SortingSettings sortingSettings = 
            new SortingSettings(camera)
        {
            criteria = SortingCriteria.CommonOpaque
        };
        DrawingSettings drawingSettings = new DrawingSettings(unlitShaderTag, sortingSettings);
        context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);

        // 天空盒
        context.DrawSkybox(camera);


        // 透明物件
        sortingSettings.criteria = SortingCriteria.CommonTransparent;
        drawingSettings.sortingSettings = sortingSettings;
        filteringSettings.renderQueueRange = RenderQueueRange.transparent;

        context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);

    }
}
