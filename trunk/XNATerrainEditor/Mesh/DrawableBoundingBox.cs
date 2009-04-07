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

namespace XNATerrainEditor
{
    public class DrawableBoundingBox
    {
        public BoundingBox boundingBox;
        Vector4 color = Color.White.ToVector4();

        public Plane[] plane;
        bool bPlaneTransformed = false;

        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;
        VertexPositionColor[] vertex;
        VertexDeclaration vertexDeclaration;

        Effect effect;

        public DrawableBoundingBox(Vector3 position, Vector3 min, Vector3 max)
        {            
            boundingBox = new BoundingBox(min + position, max + position);
            effect = Editor.content.Load<Effect>(@"content\\shaders\\color");
            SetupVertices();
            SetupIndices();
        }

        private void SetupVertices()
        {
            vertexDeclaration = new VertexDeclaration(Editor.graphics.GraphicsDevice, VertexPositionColor.VertexElements);

            vertex = new VertexPositionColor[8];

            float MinX = boundingBox.Min.X;
            float MinY = boundingBox.Min.Y;
            float MinZ = boundingBox.Min.Z;
            float MaxX = boundingBox.Max.X;
            float MaxY = boundingBox.Max.Y;
            float MaxZ = boundingBox.Max.Z;

            float SizeX = MaxX - MinX;
            float SizeY = MaxY - MinY;
            float SizeZ = MaxZ - MinZ;

            vertex[0] = new VertexPositionColor(new Vector3(MinX, MinY, MinZ), Color.White);
            vertex[1] = new VertexPositionColor(new Vector3(MinX + SizeX, MinY, MinZ), Color.White);
            vertex[2] = new VertexPositionColor(new Vector3(MinX, MinY, MinZ + SizeZ), Color.White);
            vertex[3] = new VertexPositionColor(new Vector3(MinX + SizeX, MinY, MinZ + SizeZ), Color.White);

            vertex[4] = new VertexPositionColor(new Vector3(MinX, MinY + SizeY, MinZ), Color.White);
            vertex[5] = new VertexPositionColor(new Vector3(MinX + SizeX, MinY + SizeY, MinZ), Color.White);
            vertex[6] = new VertexPositionColor(new Vector3(MinX, MinY + SizeY, MinZ + SizeZ), Color.White);
            vertex[7] = new VertexPositionColor(new Vector3(MinX + SizeX, MinY + SizeY, MinZ + SizeZ), Color.White);

            plane = new Plane[5];
            plane[0] = new Plane(vertex[0].Position, vertex[1].Position, vertex[4].Position);
            plane[1] = new Plane(vertex[1].Position, vertex[3].Position, vertex[5].Position);
            plane[2] = new Plane(vertex[3].Position, vertex[2].Position, vertex[7].Position);
            plane[3] = new Plane(vertex[2].Position, vertex[0].Position, vertex[6].Position);
            plane[4] = new Plane(vertex[4].Position, vertex[5].Position, vertex[6].Position);

            vertexBuffer = new VertexBuffer(Editor.graphics.GraphicsDevice, typeof(VertexPositionColor), vertex.Length, BufferUsage.None);
            vertexBuffer.SetData<VertexPositionColor>(vertex);
        }

        private void SetupIndices()
        {
            short[] Indices = new short[24];

            Indices[0] = 0; Indices[1] = 1;
            Indices[2] = 0; Indices[3] = 2;
            Indices[4] = 3; Indices[5] = 1;
            Indices[6] = 3; Indices[7] = 2;
            Indices[8] = 4; Indices[9] = 5;
            Indices[10] = 4; Indices[11] = 6;
            Indices[12] = 7; Indices[13] = 5;
            Indices[14] = 7; Indices[15] = 6;
            Indices[16] = 0; Indices[17] = 4;
            Indices[18] = 1; Indices[19] = 5;
            Indices[20] = 3; Indices[21] = 7;
            Indices[22] = 2; Indices[23] = 6;

            indexBuffer = new IndexBuffer(Editor.graphics.GraphicsDevice, sizeof(short) * Indices.Length, BufferUsage.None, IndexElementSize.SixteenBits);
            indexBuffer.SetData<short>(Indices);
        }

        public void Draw(GraphicsDevice graphicsDevice, Matrix world, Matrix view, Matrix projection)
        {
            graphicsDevice.RenderState.DepthBufferEnable = true;
            graphicsDevice.VertexDeclaration = vertexDeclaration;

            Matrix WorldViewProj = world * view * projection;

            if (!bPlaneTransformed)
            {
                plane[0] = MathExtra.TransformPlane(ref plane[0], ref world);
                plane[1] = MathExtra.TransformPlane(ref plane[1], ref world);
                plane[2] = MathExtra.TransformPlane(ref plane[2], ref world);
                plane[3] = MathExtra.TransformPlane(ref plane[3], ref world);
                plane[4] = MathExtra.TransformPlane(ref plane[4], ref world);
                bPlaneTransformed = true;
            }

            effect.Begin();

            effect.Parameters["WorldViewProj"].SetValue(WorldViewProj);
            effect.Parameters["color"].SetValue(color);

            effect.CommitChanges();

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Begin();

                graphicsDevice.Indices = indexBuffer;
                graphicsDevice.Vertices[0].SetSource(vertexBuffer, 0, VertexPositionColor.SizeInBytes);
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0, vertex.Length, 0, 12);

                pass.End();
            }

            effect.End();
        }
    }
}
