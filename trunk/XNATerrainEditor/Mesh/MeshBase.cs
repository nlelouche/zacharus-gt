//======================================================================
// XNA Terrain Editor
// Copyright (C) 2008 Eric Grossinger
// http://psycad007.spaces.live.com/
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace XNATerrainEditor
{
    public partial class MeshBase
    {
        public Model model;
        public Texture2D texture;
        
        public bool bHasAlpha = false;
        public float Alpha = 1.0f;

        public Vector3 position, rotation;

        public Matrix world, rotationMatrix;
        public float drawScale = 1.0f;

        /// <summary>
        /// Shader Settings
        /// </summary>
        private Effect effect;
        private EffectParameter worldViewProjParam;
        private EffectParameter worldParam;
        private EffectParameter textureParam;
        //private EffectParameter AmbientLightColorParam;
        //private EffectParameter AlphaParam;

        public Vector4 ambientLightColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);

        public DrawableBoundingBox boundingBox;

        /// <summary>
        /// Initialize Mesh
        /// </summary>
        public MeshBase(string modelName, string textureName, string shaderName, float scale, Vector3 spawnPosition, Vector3 spawnRotation)
        {
            model = Editor.content.Load<Model>(@"content\\models\\" + modelName);

            InitBoundingBox();

            if (textureName == string.Empty)
                texture = Editor.content.Load<Texture2D>(@"content\\textures\\null");
            else
            {
                if (File.Exists(Directory.GetCurrentDirectory() + "content\\textures\\" + textureName))
                    texture = Editor.content.Load<Texture2D>(@"content\\textures\\" + textureName);
                else if (File.Exists(Directory.GetCurrentDirectory() + "content\\textures\\terrain\\" + textureName))
                    texture = Editor.content.Load<Texture2D>(@"content\\textures\\terrain\\" + textureName);
            }
        
            position = spawnPosition;
            rotation = spawnRotation;
            drawScale = scale;

            if (shaderName != string.Empty)
                InitShader(shaderName);
            else
                InitShader("basic");

            Update();

            Editor.console.Add("Mesh (" + modelName + ") Initialized!");
        }

        private void InitBoundingBox()
        {
            if (model.Tag != null)
            {
                BoundingBox modelBounds = (BoundingBox)model.Tag;
                boundingBox = new DrawableBoundingBox(position, modelBounds.Min, modelBounds.Max);
            }
        }

        public virtual void InitShader(string shaderName)
        {
            effect = Editor.content.Load<Effect>(@"content\\shaders\\" + shaderName);
            
            worldViewProjParam = effect.Parameters["WorldViewProj"];
            worldParam = effect.Parameters["World"];
            textureParam = effect.Parameters["Material"];

            //System.Diagnostics.Debug.WriteLine("Shader Initialized");
        }

        /// <summary>
        /// Update Mesh
        /// </summary>
        public virtual void Update()
        {
            UpdateWorld();
        }

        public void UpdateWorld()
        {
            //Update world matrix (rotation.X - MathHelper.ToRadians(90.0f))
            if (rotationMatrix == Matrix.Identity)
                rotationMatrix = Matrix.CreateRotationX(rotation.X) * Matrix.CreateRotationY(rotation.Y) * Matrix.CreateRotationZ(rotation.Z);
            world = Matrix.CreateScale(drawScale) * rotationMatrix * Matrix.CreateTranslation(position);
        }

        /// <summary>
        /// Draw Mesh
        /// </summary>
        /// <param name="view">Camera view matrix</param>
        /// <param name="projection">Camera projection matrix</param>
        public virtual void Draw(GraphicsDevice graphicsDevice, Matrix view, Matrix projection)
        {
            graphicsDevice.RenderState.FogEnable = true;
            graphicsDevice.RenderState.DepthBufferEnable = true;
            graphicsDevice.RenderState.AlphaBlendEnable = false;
            graphicsDevice.RenderState.AlphaTestEnable = false;
            graphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
            graphicsDevice.SamplerStates[0].AddressU = TextureAddressMode.Wrap;
            graphicsDevice.SamplerStates[0].AddressV = TextureAddressMode.Wrap;
            graphicsDevice.SamplerStates[0].AddressW = TextureAddressMode.Wrap;

            if (model != null)
            {
                if (bHasAlpha)
                {
                    graphicsDevice.RenderState.AlphaBlendEnable = true;
                    graphicsDevice.RenderState.SourceBlend = Blend.One;
                    graphicsDevice.RenderState.DestinationBlend = Blend.One;
                }

                if (effect != null)
                {
                    #region Draw Model using Effect

                    effect.Begin();

                    foreach (EffectPass pass in effect.Techniques[0].Passes)
                    {
                        pass.Begin();

                        Matrix WorldViewProj = world * view * projection;
                        worldViewProjParam.SetValue(WorldViewProj);

                        if (worldParam != null)
                            worldParam.SetValue(world);

                        textureParam.SetValue(texture);

                        effect.CommitChanges();

                        DrawMesh();

                        pass.End();
                    }

                    effect.End();

                    if (boundingBox != null)
                        boundingBox.Draw(graphicsDevice, world, view, projection);

                    #endregion
                }
                else
                {
                    #region Draw Model using BasicEffect

                    foreach (ModelMesh mesh in model.Meshes)
                    {                        
                        foreach (BasicEffect basicEffect in mesh.Effects)
                        {
                            basicEffect.World = world;
                            basicEffect.View = view;
                            basicEffect.Projection = projection;
                            basicEffect.TextureEnabled = true;
                            basicEffect.Texture = texture;
                            basicEffect.EnableDefaultLighting();
                            basicEffect.VertexColorEnabled = false;
                            basicEffect.DiffuseColor = Vector3.One;
                            basicEffect.CommitChanges();
                        }

                        mesh.Draw();
                    }

                    if (boundingBox != null)
                        boundingBox.Draw(graphicsDevice, world, view, projection);

                    #endregion
                }

                if (bHasAlpha)
                    graphicsDevice.RenderState.AlphaBlendEnable = false;
            }
        }

        private void DrawMesh()
        {
            foreach (ModelMesh modelMesh in model.Meshes)
            {
                foreach (ModelMeshPart meshPart in modelMesh.MeshParts)
                {
                    VertexElement[] vertex = meshPart.VertexDeclaration.GetVertexElements();

                    meshPart.Effect = effect;

                    if (meshPart.PrimitiveCount > 0)
                    {
                        Editor.graphics.GraphicsDevice.VertexDeclaration = meshPart.VertexDeclaration;
                        Editor.graphics.GraphicsDevice.Vertices[0].SetSource(modelMesh.VertexBuffer, meshPart.StreamOffset, meshPart.VertexStride);
                        Editor.graphics.GraphicsDevice.Indices = modelMesh.IndexBuffer;
                        Editor.graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, meshPart.BaseVertex, 0, meshPart.NumVertices, meshPart.StartIndex, meshPart.PrimitiveCount);
                    }
                }
            }
        }
    }    
}
