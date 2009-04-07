//======================================================================
// XNA Terrain Editor
// Copyright (C) 2008 Eric Grossinger
// http://psycad007.spaces.live.com/
//======================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace XNATerrainEditor
{
    public partial class PaintTools : Form
    {
        public Tool currentTool = Tool.Select;
        public enum Tool
        {
            Select,
            Fill,
            Unfill,
        }

        Heightmap heightmap;
        MouseState ms;

        int layerID = -1;

        public PaintTools(Heightmap myHeightmap)
        {
            heightmap = myHeightmap;
            InitializeComponent();
        }

        private void ResetToolSelection(Tool tool)
        {
            if (tool != Tool.Select)
                checkBox1.Checked = false;
            if (tool != Tool.Fill)
                checkBox2.Checked = false;
            if (tool != Tool.Unfill)
                checkBox3.Checked = false;
        }

        private void SelectTool(Tool tool)
        {
            ResetToolSelection(tool);

            switch (tool)
            {
                case Tool.Select:
                    currentTool = Tool.Select;
                    break;
                case Tool.Fill:
                    currentTool = Tool.Fill;
                    break;
                case Tool.Unfill:
                    currentTool = Tool.Unfill;
                    break;
            }
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            heightmap.groundCursorSize = hScrollBar1.Value;
        }

        private void hScrollBar2_Scroll(object sender, ScrollEventArgs e)
        {
            heightmap.groundCursorStrength = hScrollBar2.Value;
        }

        private void PaintTools_Load(object sender, EventArgs e)
        {
            GetHeightmapData();
            ResetToolSelection(Tool.Select);
            hScrollBar1.Value = heightmap.groundCursorSize;
            hScrollBar2.Value = heightmap.groundCursorStrength;
            heightmap.bShowCursor = true;
            timer1.Start();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
                SelectTool(Tool.Select);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
                SelectTool(Tool.Fill);
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked == true)
                SelectTool(Tool.Unfill);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ms = Mouse.GetState();

            Ray pickRay = Editor.heightmap.GetPickRay();
            float rayLength = 0f;

            if (Editor.camera.state == Camera.State.Fixed && layerID != -1 && currentTool != Tool.Select)
            {
                heightmap.bShowCursor = true;

                for (int i = 0; i < heightmap.triangle.Length; i++)
                {
                    Heightmap.Tri thisTri = heightmap.triangle[i];
                    if (MathExtra.Intersects(pickRay, thisTri.p1, thisTri.p3, thisTri.p2, thisTri.normal, false, true, out rayLength))
                    {
                        heightmap.testTriangle[2].SetNewCoordinates(thisTri.p1, thisTri.p2, thisTri.p3, Microsoft.Xna.Framework.Graphics.Color.White);

                        Vector3 rayTarget = pickRay.Position + pickRay.Direction * rayLength;
                        heightmap.groundCursorPosition.X = rayTarget.X / (heightmap.size.X * heightmap.cellSize.X);
                        heightmap.groundCursorPosition.Y = rayTarget.Y;
                        heightmap.groundCursorPosition.Z = rayTarget.Z / (heightmap.size.Y * heightmap.cellSize.Y);

                        Point pixel = new Point((int)(rayTarget.X / heightmap.cellSize.X), (int)(rayTarget.Z / heightmap.cellSize.Y));

                        if (rayLength > 0f)
                        {
                            if (ms.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                            {
                                switch (currentTool)
                                {
                                    case Tool.Fill:
                                        heightmap.Paint(pixel.X, pixel.Y, heightmap.groundCursorStrength / 100f, layerID - 1);
                                        break;
                                    case Tool.Unfill:
                                        heightmap.Paint(pixel.X, pixel.Y, -heightmap.groundCursorStrength / 100f, layerID - 1);
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            else if (heightmap.bShowCursor)
                heightmap.bShowCursor = false;
        }

        public void GetHeightmapData()
        {
            lstTexture1.Items.Clear();

            string currentDir = Application.StartupPath;
            string[] textureFiles = Directory.GetFiles(currentDir + "\\content\\textures\\terrain");
            
            foreach (string thisFile in textureFiles)
                lstTexture1.Items.Add(thisFile.Substring(currentDir.Length + "\\content\\textures\\terrain\\".Length, thisFile.Length - 4 - currentDir.Length - "\\content\\textures\\terrain\\".Length));
            
            foreach (string customTex in heightmap.customTexture)
                lstTexture1.Items.Add(customTex);

            lstTexture1.Text = "";


            listView1.Items.Clear();

            //Get current layers settings
            for (int i = 0; i < Editor.heightmap.textureLayer.Length; i++)
            {
                string[] layerItems = new string[3];

                if (i == 0)
                    layerItems[0] = "detail";
                else
                    layerItems[0] = "layer" + i;

                layerItems[1] = Editor.heightmap.textureLayer[i].layerTex;
                layerItems[2] = Editor.heightmap.textureLayer[i].scale.ToString();
                
                ListViewItem item = new ListViewItem(layerItems);
                listView1.Items.Add(item);
            }
        }

        private void lstTexture1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (layerID != -1)
            {
                Editor.heightmap.textureLayer[layerID].layerTex = lstTexture1.Text;
                listView1.Items[layerID].SubItems[1].Text = Editor.heightmap.textureLayer[layerID].layerTex;
                Editor.heightmap.LoadTextures();
                Editor.heightmap.UpdateTextures();
            }
        }

        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            if (layerID != -1)
            {
                Editor.heightmap.textureLayer[layerID].scale = trackBar4.Value;
                listView1.Items[layerID].SubItems[2].Text = Editor.heightmap.textureLayer[layerID].scale.ToString();
                Editor.heightmap.UpdateEffect();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            heightmap.bShowCursor = false;
            base.OnClosed(e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.FileName = "";
            openFileDialog.Filter = "All Images|*.bmp;*.jpg;*.dds;*.tga|Bitmap (*.bmp)|*.bmp|JPEG (*.jpg)|*.jpg|DirectDraw Surface (*.dds)|*.dds|Truevision TGA (*.tga)|*.tga|*.*|*.*";
            openFileDialog.ShowDialog();

            if (openFileDialog.FileName != "")
            {
                //Add to custom textures list (Texture2D.FromFile)
                if (!FoundInList(openFileDialog.FileName))
                    Editor.heightmap.customTexture.Add(openFileDialog.FileName);

                if (listView1.SelectedItems.Count > 0)
                {
                    Editor.heightmap.textureLayer[layerID].layerTex = openFileDialog.FileName;
                    Editor.heightmap.LoadTextures();
                    Editor.heightmap.UpdateTextures();
                }
                else
                    GetHeightmapData();
            }
        }

        private bool FoundInList(string s)
        {
            for (int i = 0; i < Editor.heightmap.customTexture.Count; i++)
            {
                if (Editor.heightmap.customTexture[i] == s)
                    return true;
            }

            return false;
        }

        private void lstTexture1_KeyUp(object sender, KeyEventArgs e)
        {
            lstTexture1.Text = listView1.FindItemWithText(listView1.Items[layerID].Text).SubItems[1].Text;
        }

        private void lstTexture1_KeyDown(object sender, KeyEventArgs e)
        {
            lstTexture1.Text = listView1.FindItemWithText(listView1.Items[layerID].Text).SubItems[1].Text;
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                layerID = listView1.SelectedItems[0].Index;
                Editor.heightmap.currentLayer = layerID;
                lstTexture1.Text = listView1.FindItemWithText(listView1.Items[layerID].Text).SubItems[1].Text;
                trackBar4.Value = (int)Editor.heightmap.textureLayer[layerID].scale;
                Editor.console.Add("LayerID : " + layerID + " selected");
            }
            else
                layerID = -1;
        }
    }
}