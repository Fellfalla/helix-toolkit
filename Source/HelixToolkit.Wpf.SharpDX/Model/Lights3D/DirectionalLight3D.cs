﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectionalLight3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using HelixToolkit.Wpf.SharpDX.Extensions;

    public sealed class DirectionalLight3D : Light3D
    {
        public DirectionalLight3D()
        {
            this.Color = global::SharpDX.Color.White;
            this.LightType = LightType.Directional;
        }

        public override void Attach(IRenderHost host)
        {
            /// --- attach
            base.Attach(host);

            /// --- light constant params            
            this.vLightDir = this.effect.GetVariableByName("vLightDir").AsVector();
            this.vLightColor = this.effect.GetVariableByName("vLightColor").AsVector();
            this.iLightType = this.effect.GetVariableByName("iLightType").AsScalar();

            /// --- Set light type
            Light3DSceneShared.LightTypes[lightIndex] = (int)Light3D.Type.Directional;

            /// --- flush
            //this.Device.ImmediateContext.Flush();
        }

        public override void Detach()
        {
            Disposer.RemoveAndDispose(ref this.vLightDir);
            Disposer.RemoveAndDispose(ref this.vLightColor);
            Disposer.RemoveAndDispose(ref this.iLightType);
            base.Detach();
        }

        public override void Render(RenderContext context)
        {
            var manager = renderHost.RenderTechniquesManager;
            if (renderHost.RenderTechnique == manager.RenderTechniques.Get(DeferredRenderTechniqueNames.Deferred) ||
                renderHost.RenderTechnique == manager.RenderTechniques.Get(DeferredRenderTechniqueNames.GBuffer))
            {
                return;
            }

            if (this.IsRendering)
            {
                /// --- set lighting parameters

                Light3DSceneShared.LightColors[lightIndex] = this.Color;
            }
            else
            {
                // --- turn-off the light
                Light3DSceneShared.LightColors[lightIndex] = new global::SharpDX.Color4(0, 0, 0, 0);
            }

            /// --- set lighting parameters
            Light3DSceneShared.LightDirections[lightIndex] = -this.Direction.ToVector4();

            /// --- update lighting variables               
            this.vLightDir.Set(Light3DSceneShared.LightDirections);
            this.vLightColor.Set(Light3DSceneShared.LightColors);
            this.iLightType.Set(Light3DSceneShared.LightTypes);


            /// --- if shadow-map enabled
            if (this.renderHost.IsShadowMapEnabled)
            {
                /// update shader
                this.mLightView.SetMatrix(Light3DSceneShared.LightViewMatrices);
                this.mLightProj.SetMatrix(Light3DSceneShared.LightProjMatrices);
            }
        }
    }
}
