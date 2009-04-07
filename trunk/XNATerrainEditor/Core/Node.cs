using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace XNATerrainEditor
{
    public class Node
    {
        public NodeType Type;
        public BoundingSquare BoundingCoordinates = new BoundingSquare();
        public int[] Branches = new int[4];
        public BoundingBox boundingBox;
        public List<int> TriangleIDs = new List<int>();
        public int ID;
        public int ParentID;


        public Node()
        {
            
        }
    }
}
