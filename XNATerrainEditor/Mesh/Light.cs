//======================================================================
// XNA Terrain Editor
// Copyright (C) 2008 Eric Grossinger
// http://psycad007.spaces.live.com/
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace XNATerrainEditor
{
    public class Light
    {
        public Vector3 position = Vector3.Zero;
        public Vector3 direction = Vector3.Zero;
        public Vector3 rotation = Vector3.Zero;
        public Vector3 color = Vector3.One;
        public float radius = 200f;
        public float intensity = 1f;
        public float scale = 1f;

        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;
        VertexPositionNormalTexture[] vertex;
        VertexDeclaration vertexDeclaration;

        Matrix world, rotationMatrix;

        Effect effect;
        EffectParameter worldViewProjParam;
        EffectParameter worldParam;
        EffectParameter textureParam;

        Texture2D texture;

        DrawableBoundingBox boundingBox;

        public Light(Vector3 spawnPos, Vector3 lightColor, float Radius, float Intensity)
        {
            position = spawnPos;
            color = lightColor;
            radius = Radius;
            intensity = Intensity;

            InitEffect();
            SetupVertices();
            SetupIndices();

        }

        private void InitEffect()
        {
            effect = Editor.content.Load<Effect>(@"content\\shaders\\basic");
            texture = Editor.content.Load<Texture2D>(@"content\\textures\\icons\\light_icon");
            worldViewProjParam = effect.Parameters["WorldViewProj"];
            worldParam = effect.Parameters["World"];
            textureParam = effect.Parameters["Material"];
        }

        private void SetupVertices()
        {
            vertexDeclaration = new VertexDeclaration(Editor.graphics.GraphicsDevice, VertexPositionNormalTexture.VertexElements);
            
            vertex = new VertexPositionNormalTexture[4];
            vertex[0] = new VertexPositionNormalTexture(new Vector3(-10f, 15f, 0f), Vector3.Forward, new Vector2(0f, 0f));
            vertex[1] = new VertexPositionNormalTexture(new Vector3(10f, 15f, 0f), Vector3.Forward, new Vector2(1f, 0f));
            vertex[2] = new VertexPositionNormalTexture(new Vector3(-10f, -15f, 0f), Vector3.Forward, new Vector2(0f, 1f));
            vertex[3] = new VertexPositionNormalTexture(new Vector3(10f, -15f, 0f), Vector3.Forward, new Vector2(1f, 1f));

            vertexBuffer = new VertexBuffer(Editor.graphics.GraphicsDevice, typeof(VertexPositionNormalTexture), vertex.Length, BufferUsage.None);
            vertexBuffer.SetData<VertexPositionNormalTexture>(vertex);

            boundingBox = new DrawableBoundingBox(position, vertex[2].Position - new Vector3(2f, 2f, 2f), vertex[1].Position + new Vector3(2f, 2f, 2f));
        }

        private void SetupIndices()
        {
            short[] Indices = new short[6];

            Indices[0] = 0;
            Indices[1] = 2;
            Indices[2] = 1;

            Indices[3] = 1;
            Indices[4] = 2;
            Indices[5] = 3;

            indexBuffer = new IndexBuffer(Editor.graphics.GraphicsDevice, sizeof(short) * Indices.Length, BufferUsage.None, IndexElementSize.SixteenBits);
            indexBuffer.SetData<short>(Indices);
        }

        public virtual void Update()
        {
            rotation.Y = -MathExtra.GetAngleFrom2DVectors(new Vector2(Editor.camera.position.X, Editor.camera.position.Z), new Vector2(position.X, position.Z), true);
            rotationMatrix = Matrix.CreateRotationX(rotation.X) * Matrix.CreateRotationY(rotation.Y) * Matrix.CreateRotationZ(rotation.Z);
            world = Matrix.CreateScale(scale) * rotationMatrix * Matrix.CreateTranslation(position);
        }

        public void Draw(GraphicsDevice graphicsDevice, Matrix view, Matrix projection)
        {
            graphicsDevice.RenderState.FogEnable = false;
            graphicsDevice.RenderState.AlphaBlendEnable = true;
            graphicsDevice.RenderState.AlphaTestEnable = true;
            graphicsDevice.RenderState.AlphaSourceBlend = Blend.SourceAlpha;
            graphicsDevice.RenderState.AlphaDestinationBlend = Blend.One;
            graphicsDevice.RenderState.AlphaBlendOperation = BlendFunction.Add;
            graphicsDevice.RenderState.BlendFunction = BlendFunction.Add;

            //graphicsDevice.RenderState.DepthBufferWriteEnable = false;

            //graphicsDevice.RenderState.CullMode = CullMode.CullClockwiseFace;
            //graphicsDevice.RenderState.FillMode = FillMode.Solid;

            graphicsDevice.VertexDeclaration = vertexDeclaration;

            Matrix WorldViewProj = world * view * projection;

            effect.Begin();

            worldViewProjParam.SetValue(WorldViewProj);
            textureParam.SetValue(texture);

            if (worldParam != null)
                worldParam.SetValue(world);

            effect.CommitChanges();

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Begin();

                graphicsDevice.Indices = indexBuffer;
                graphicsDevice.Vertices[0].SetSource(vertexBuffer, 0, VertexPositionNormalTexture.SizeInBytes);
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertex.Length, 0, vertex.Length / 2);

                pass.End();
            }

            effect.End();

            //graphicsDevice.RenderState.DepthBufferWriteEnable = true;

            if (boundingBox != null)
                boundingBox.Draw(graphicsDevice, world, view, projection);
        }
    }
}
