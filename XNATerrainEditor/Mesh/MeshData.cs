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
    public class MeshData
    {
        public VertexPositionNormalTexture[] Vertices;
        public int[] Indices;
        public Vector3[] FaceNormals;
        
        public BoundingBox collisionBox;
        
        public MeshData(VertexPositionNormalTexture[] Vertices, int[] Indices, Vector3[] pFaceNormals, BoundingBox boundingBox)
        {
            this.Vertices = Vertices;
            this.Indices = Indices;
            this.FaceNormals = pFaceNormals;
            this.collisionBox = boundingBox;
        }
    }
}
