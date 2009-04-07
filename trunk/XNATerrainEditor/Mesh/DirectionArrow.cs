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
    class DirectionArrow
    {
        private VertexPositionColor[] vertex;

        public DirectionArrow(Vector3 direction, Color color)
        {
            InitVertices(direction, color);
        }

        private void InitVertices(Vector3 direction, Color color)
        {
            vertex = new VertexPositionColor[10];

            if (direction == Vector3.Up)
            {
                vertex[0] = new VertexPositionColor(new Vector3(0f, -10f, 0f), color);
                vertex[1] = new VertexPositionColor(new Vector3(0f, 10f, 0f), color);
                vertex[2] = new VertexPositionColor(new Vector3(0f, 10f, 0f), color);
                vertex[3] = new VertexPositionColor(new Vector3(1f, 4f, 1f), color);
                vertex[4] = new VertexPositionColor(new Vector3(0f, 10f, 0f), color);
                vertex[5] = new VertexPositionColor(new Vector3(-1f, 4f, 1f), color);
                vertex[6] = new VertexPositionColor(new Vector3(0f, 10f, 0f), color);
                vertex[7] = new VertexPositionColor(new Vector3(1f, 4f, -1f), color);
                vertex[8] = new VertexPositionColor(new Vector3(0f, 10f, 0f), color);
                vertex[9] = new VertexPositionColor(new Vector3(-1f, 4f, -1f), color);
            }
            else if (direction == Vector3.Forward)
            {
                vertex[0] = new VertexPositionColor(new Vector3(0f, 0f, -10f), color);
                vertex[1] = new VertexPositionColor(new Vector3(0f, 0f, 10f), color);
                vertex[2] = new VertexPositionColor(new Vector3(0f, 0f, 10f), color);
                vertex[3] = new VertexPositionColor(new Vector3(1f, 1f, 4f), color);
                vertex[4] = new VertexPositionColor(new Vector3(0f, 0f, 10f), color);
                vertex[5] = new VertexPositionColor(new Vector3(-1f, 1f, 4f), color);
                vertex[6] = new VertexPositionColor(new Vector3(0f, 0f, 10f), color);
                vertex[7] = new VertexPositionColor(new Vector3(1f, -1f, 4f), color);
                vertex[8] = new VertexPositionColor(new Vector3(0f, 0f, 10f), color);
                vertex[9] = new VertexPositionColor(new Vector3(-1f, -1f, 4f), color);
            }
            else if (direction == Vector3.Right)
            {
                vertex[0] = new VertexPositionColor(new Vector3(-10f, 0f, 0f), color);
                vertex[1] = new VertexPositionColor(new Vector3(10f, 0f, 0f), color);
                vertex[2] = new VertexPositionColor(new Vector3(10f, 0f, 0f), color);
                vertex[3] = new VertexPositionColor(new Vector3(4f, 1f, 1f), color);
                vertex[4] = new VertexPositionColor(new Vector3(10f, 0f, 0f), color);
                vertex[5] = new VertexPositionColor(new Vector3(4f, -1f, 1f), color);
                vertex[6] = new VertexPositionColor(new Vector3(10f, 0f, 0f), color);
                vertex[7] = new VertexPositionColor(new Vector3(4f, 1f, -1f), color);
                vertex[8] = new VertexPositionColor(new Vector3(10f, 0f, 0f), color);
                vertex[9] = new VertexPositionColor(new Vector3(4f, -1f, -1f), color);
            }
        }        

        public void Draw(Matrix world, Matrix view, Matrix projection)
        {
            Editor.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertex, 0, vertex.Length / 2);
        }
    }
}
