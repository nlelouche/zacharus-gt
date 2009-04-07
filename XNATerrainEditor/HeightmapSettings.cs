//======================================================================
// XNA Terrain Editor
// Copyright (C) 2008 Eric Grossinger
// http://psycad007.spaces.live.com/
//======================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace XNATerrainEditor
{
    public partial class HeightmapSettings : Form
    {        
        List<string> skybox;
        string selectedSky = string.Empty;

        public CustomSize customSize;

        public Vector2 mapSize = new Vector2(64f, 64f);
        public Vector2 tileSize = new Vector2(50f, 50f);
        public float mapHeight = 500f;

        public HeightmapSettings()
        {
            InitializeComponent();
        }

        private void GetSkyboxes()
        {
            //Retreive Available Skyboxes
            skybox = new List<string>();
            string[] skyboxes = Directory.GetFiles(Editor.AppPath + "\\content\\textures\\skybox");

            lstSkies.Items.Clear();

            for (int i = 0; i < skyboxes.Length; i++)
            {
                string finalName = string.Empty;
                string[] skyName = skyboxes[i].Split('\\');
                finalName = skyName[skyName.Length - 1];
                finalName = finalName.Substring(0, finalName.Length - 4);
                char[] trimmers = { '_' };
                string[] strSplit = finalName.Split('_');
                finalName = strSplit[0];

                if (skybox.Count > 0)
                {
                    bool bFoundSimilar = false;
                    for (int j = 0; j < skybox.Count; j++)
                    {
                        if (skybox[j] == finalName)
                        {
                            bFoundSimilar = true;
                        }
                    }
                    if (!bFoundSimilar)
                    {
                        skybox.Add(finalName);
                        lstSkies.Items.Add(finalName);
                    }
                }
                else
                {
                    skybox.Add(finalName);
                    lstSkies.Items.Add(finalName);
                }
            }

            //lstSkybox.Text = Game1.skybox.name;
            lstSkies.SelectedIndex = lstSkies.FindString(Editor.skybox.name);
            selectedSky = lstSkies.SelectedItem.ToString();
        }

        public void GetHeightmapData()
        {
            if (Editor.heightmap != null)
            {
                mapSmoothing.Checked = Editor.heightmap.bSmooth;

                trackBar1.Value = (int)Editor.heightmap.cellSize.X;
                trackBar4.Value = (int)Editor.heightmap.cellSize.Y;
                trackBar2.Value = (int)Editor.heightmap.maxHeight;

                numericUpDown3.Value = trackBar1.Value;
                numericUpDown4.Value = trackBar4.Value;
                numericUpDown5.Value = trackBar2.Value;

                //lblHeightmap.Text = heightmap.heightmapFile;

                string currentDir = Directory.GetCurrentDirectory();

                //Get current fog settings
                lblFogColor.ForeColor = System.Drawing.Color.FromArgb((int)(Editor.graphics.GraphicsDevice.RenderState.FogColor.ToVector4().W * 255f), (int)(Editor.graphics.GraphicsDevice.RenderState.FogColor.ToVector4().X * 255f), (int)(Editor.graphics.GraphicsDevice.RenderState.FogColor.ToVector4().Y * 255f), (int)(Editor.graphics.GraphicsDevice.RenderState.FogColor.ToVector4().Z * 255f));
                lblFogColor.BackColor = lblFogColor.ForeColor;
                trackBar9.Value = (int)Editor.graphics.GraphicsDevice.RenderState.FogStart;
                trackBar10.Value = (int)Editor.graphics.GraphicsDevice.RenderState.FogEnd;
                trackBar11.Value = (int)Editor.graphics.GraphicsDevice.RenderState.FogDensity;

                trackBar3.Value = (int)(Editor.waterHeight / Editor.heightmap.maxHeight * 1000f);

                checkBox1.Checked = Editor.bDrawSkybox;
                checkBox2.Checked = Editor.bDrawWater;
                checkBox3.Checked = Editor.bUseFog;

                //comboBox1.Text = Game1.fogMode.ToString();

                //comboBox1.Items.Add("None");
                //comboBox1.Items.Add("Linear");
                //comboBox1.Items.Add("Exponent");
                //comboBox1.Items.Add("ExponentSquared");

                drawDetail.Checked = Editor.heightmap.bDrawDetail;

                trackBar8.Value = (int)(Editor.sun.lightPower * 10f);
                
                //checkBox4.Checked = Game1.water.bFollowCamera;

                trackBar7.Value = (int)(Editor.heightmap.ambientLight.Length() / 2f * 100f);
            }
        }

        private void GetSunData()
        {
            if (Editor.sun != null)
            {
                trackBar5.Value = (int)MathHelper.ToDegrees(Editor.sun.rotation.X);
                trackBar6.Value = (int)MathHelper.ToDegrees(Editor.sun.rotation.Y);
                checkBox5.Checked = Editor.bDrawSun;

                numericUpDown1.Value = (decimal)((float)Editor.sun.LongitudeSpeed * 100f);
                numericUpDown2.Value = (decimal)((float)Editor.sun.LatitudeSpeed * 100f);

                checkBox4.Checked = Editor.sun.bCheckTerrainCollision;
            }
        }

        private void HeightmapSettings_Load(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = Editor.AppPath;
            saveFileDialog1.InitialDirectory = Editor.AppPath;

            trackBar1.Value = (int)tileSize.X;
            trackBar4.Value = (int)tileSize.Y;
            trackBar2.Value = (int)mapHeight;
            numericUpDown3.Value = trackBar1.Value;
            numericUpDown4.Value = trackBar4.Value;
            numericUpDown5.Value = trackBar2.Value;

            GetSkyboxes();
            GetHeightmapData();
            GetSunData();
        }

        private void lstSkybox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Editor.skybox.LoadTextures(selectedSky);
        }

        private void trackBar1_Scroll_1(object sender, EventArgs e)
        {
            numericUpDown3.Value = trackBar1.Value;

            if (Editor.heightmap != null)
            {
                Editor.heightmap.cellSize.X = trackBar1.Value;
                ResetHeightmap();
            }
        }

        private void trackBar2_Scroll_1(object sender, EventArgs e)
        {
            numericUpDown5.Value = trackBar2.Value;

            if (Editor.heightmap != null)
            {
                Editor.heightmap.maxHeight = trackBar2.Value;
                Editor.waterHeight = trackBar3.Value / 1000f * Editor.heightmap.maxHeight;
                ResetHeightmap();
            }
        }

        private void mapSmoothing_CheckedChanged(object sender, EventArgs e)
        {
            if (Editor.heightmap != null)
            {
                Editor.heightmap.bSmooth = mapSmoothing.Checked;
                ResetHeightmap();
            }
        }

        private void ResetHeightmap()
        {
            if (Editor.heightmap != null)
            {
                Editor.heightmap.quadTree = null;
                Editor.heightmap.CalculateHeightmap();
            }
        }

        private void lblFogColor_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();

            lblFogColor.ForeColor = colorDialog1.Color;
            lblFogColor.BackColor = colorDialog1.Color;

            Editor.fogColor = new Microsoft.Xna.Framework.Graphics.Color(new Microsoft.Xna.Framework.Vector4((float)colorDialog1.Color.R / 255, (float)colorDialog1.Color.G / 255, (float)colorDialog1.Color.B / 255, 1.0f));
            Editor.graphics.GraphicsDevice.RenderState.FogColor = Editor.fogColor;
        }

        private void trackBar9_Scroll(object sender, EventArgs e)
        {
            if (trackBar9.Value > trackBar10.Value)
                trackBar10.Value = trackBar9.Value + 1;

            Editor.fogStart = trackBar9.Value;
            Editor.graphics.GraphicsDevice.RenderState.FogStart = Editor.fogStart;
        }

        private void trackBar10_Scroll(object sender, EventArgs e)
        {
            if (trackBar10.Value < trackBar9.Value)
                trackBar9.Value = trackBar10.Value - 1;

            Editor.fogEnd = trackBar10.Value;
            Editor.graphics.GraphicsDevice.RenderState.FogEnd = Editor.fogEnd;
        }

        private void trackBar11_Scroll(object sender, EventArgs e)
        {
            Editor.fogDensity = (float)trackBar11.Value / 100f;
            Editor.graphics.GraphicsDevice.RenderState.FogDensity = Editor.fogDensity;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveTerrain(Editor.mapName);
        }        

        private void closeSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        #region NewMap menu
        private void xsmall64x64ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Editor.paintTools != null)
                Editor.paintTools.Close();
            if (Editor.heightTools != null)
                Editor.heightTools.Close();
            Editor.heightmap = new Heightmap(new Vector2((float)trackBar1.Value, (float)trackBar4.Value));
            Editor.heightmap.maxHeight = (float)trackBar2.Value;
            Editor.heightmap.CreateNewHeightmap(Heightmap.MapSize.XSmall, null);
            GetHeightmapData();
            Editor.mapName = string.Empty;

            trackBar1.Enabled = false;
            trackBar4.Enabled = false;
            numericUpDown3.Enabled = false;
            numericUpDown4.Enabled = false;

            //ResetCamera();
        }
        private void small128x128ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Editor.paintTools != null)
                Editor.paintTools.Close();
            if (Editor.heightTools != null)
                Editor.heightTools.Close();
            Editor.heightmap = new Heightmap(new Vector2((float)trackBar1.Value, (float)trackBar4.Value));
            Editor.heightmap.maxHeight = (float)trackBar2.Value;
            Editor.heightmap.CreateNewHeightmap(Heightmap.MapSize.Small, null);
            GetHeightmapData();
            Editor.mapName = string.Empty;
            //ResetCamera();

            trackBar1.Enabled = false;
            trackBar4.Enabled = false;
            numericUpDown3.Enabled = false;
            numericUpDown4.Enabled = false;
        }
        private void medium256x256ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Editor.paintTools != null)
                Editor.paintTools.Close();
            if (Editor.heightTools != null)
                Editor.heightTools.Close();
            Editor.heightmap = new Heightmap(new Vector2((float)trackBar1.Value, (float)trackBar4.Value));
            Editor.heightmap.maxHeight = (float)trackBar2.Value;
            Editor.heightmap.CreateNewHeightmap(Heightmap.MapSize.Medium, null);
            GetHeightmapData();
            Editor.mapName = string.Empty;
            //ResetCamera();

            trackBar1.Enabled = false;
            trackBar4.Enabled = false;
            numericUpDown3.Enabled = false;
            numericUpDown4.Enabled = false;
        }

        private void large512x512ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Editor.paintTools != null)
                Editor.paintTools.Close();
            if (Editor.heightTools != null)
                Editor.heightTools.Close();
            Editor.heightmap = new Heightmap(new Vector2((float)trackBar1.Value, (float)trackBar4.Value));
            Editor.heightmap.maxHeight = (float)trackBar2.Value;
            Editor.heightmap.CreateNewHeightmap(Heightmap.MapSize.Large, null);
            GetHeightmapData();
            Editor.mapName = string.Empty;
            //ResetCamera();
        }

        private void xlarge1024x1024ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Editor.paintTools != null)
                Editor.paintTools.Close();
            if (Editor.heightTools != null)
                Editor.heightTools.Close();
            Editor.heightmap = new Heightmap(new Vector2((float)trackBar1.Value, (float)trackBar4.Value));
            Editor.heightmap.maxHeight = (float)trackBar2.Value;
            Editor.heightmap.CreateNewHeightmap(Heightmap.MapSize.XLarge, null);
            GetHeightmapData();
            Editor.mapName = string.Empty;
            //ResetCamera();
        }

        private void ResetCamera()
        {
            Vector3 center = new Vector3(Editor.heightmap.center.X, 0f, Editor.heightmap.center.Z);
            Vector3 centerNormal = Vector3.Zero;
            Editor.heightmap.GetGroundHeight(Editor.heightmap.center, out center.Y, out centerNormal);
            center.Y += 300f;
            Editor.camera.position = center;
        }

        #endregion

        private void customSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            customSize = new CustomSize(this);
            customSize.Visible = true;
        }

        private void heightToolsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Editor.heightmap != null)
            {
                if (Editor.paintTools != null)
                    Editor.paintTools.Close();

                if (Editor.heightTools == null || !Editor.heightTools.Visible)
                {
                    Editor.heightTools = new HeightTools(Editor.heightmap);
                    Editor.heightTools.Visible = true;
                }
            }
        }

        private void textureToolsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Editor.heightmap != null)
            {
                if (Editor.heightTools != null)
                    Editor.heightTools.Close();

                if (Editor.paintTools == null || !Editor.paintTools.Visible)
                {
                    Editor.paintTools = new PaintTools(Editor.heightmap);
                    Editor.paintTools.Visible = true;
                }
            }
        }

        private void solidToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Editor.heightmap != null)
            {
                Editor.heightmap.currentTechnique = "TransformTexture";
                wireframeToolStripMenuItem.Checked = false;
                solidWireframeToolStripMenuItem.Checked = false;
                solidToolStripMenuItem.Checked = true;
            }
        }

        private void smoothMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Editor.heightmap != null)
                Editor.heightmap.SmoothHeightmap();
        }

        private void randomNoiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Editor.heightmap != null)
                Editor.heightmap.RandomizeHeight(Heightmap.GenerationType.Random, 3, false);
        }

        private void perlinNoiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Editor.heightmap != null)
                Editor.heightmap.RandomizeHeight(Heightmap.GenerationType.PerlinNoise, 5, false);
        }

        private void randomNoiseToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (Editor.heightmap != null)
                Editor.heightmap.RandomizeHeight(Heightmap.GenerationType.Random, 3, true);
        }

        private void perlinNoiseToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (Editor.heightmap != null)
                Editor.heightmap.RandomizeHeight(Heightmap.GenerationType.PerlinNoise, 5, true);
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            if (Editor.water != null)
            {
                if (Editor.heightmap != null)
                    Editor.waterHeight = trackBar3.Value / 1000f * Editor.heightmap.maxHeight;
                else
                    Editor.waterHeight = trackBar3.Value / 1000f * 500f;
            }
        }

        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            numericUpDown4.Value = trackBar4.Value;

            if (Editor.heightmap != null)
            {
                Editor.heightmap.cellSize.Y = trackBar4.Value;
                ResetHeightmap();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Editor.bDrawSkybox = checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Editor.bDrawWater = checkBox2.Checked;
        }

        /*
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.Text)
            {
                case "None":
                    Game1.fogMode = FogMode.None;
                    checkBox3.Checked = false;
                    break;
                case "Linear":
                    Game1.fogMode = FogMode.Linear;
                    break;
                case "Exponent":
                    Game1.fogMode = FogMode.Exponent;
                    break;
                case "ExponentSquared":
                    Game1.fogMode = FogMode.ExponentSquared;
                    break;
            }
        }
        */

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            Editor.bUseFog = checkBox3.Checked;
        }

        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "*.xml|*.xml";
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName != "")
                LoadTerrain(openFileDialog1.FileName);
        }

        private void heightmapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "All Images|*.bmp;*.jpg;*.dds;*.tga|Bitmap (*.bmp)|*.bmp|JPEG (*.jpg)|*.jpg|DirectDraw Surface (*.dds)|*.dds|Truevision TGA (*.tga)|*.tga|*.*|*.*";
            openFileDialog1.ShowDialog();

            if (openFileDialog1.FileName != "")
            {
                if (Editor.heightmap == null)
                    Editor.heightmap = new Heightmap(new Vector2(trackBar1.Value, trackBar4.Value));

                Editor.heightmap.LoadHeightMap(openFileDialog1.FileName);
            }
        }                

        private void wireframeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Editor.heightmap != null)
            {
                solidToolStripMenuItem.Checked = false;
                solidWireframeToolStripMenuItem.Checked = false;
                wireframeToolStripMenuItem.Checked = true;
                Editor.heightmap.currentTechnique = "TransformWireframe";
            }
        }

        private void solidWireframeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Editor.heightmap != null)
            {
                solidToolStripMenuItem.Checked = false;
                wireframeToolStripMenuItem.Checked = false;
                solidWireframeToolStripMenuItem.Checked = true;
                Editor.heightmap.currentTechnique = "TransformTextureWireframe";
            }
        }

        private void trackBar5_Scroll(object sender, EventArgs e)
        {
            Editor.sun.rotation.Y = MathHelper.ToRadians(trackBar5.Value);
        }

        private void trackBar6_Scroll(object sender, EventArgs e)
        {
            Editor.sun.rotation.X = MathHelper.ToRadians(trackBar6.Value);
        }

        private void createIslandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Editor.heightmap != null)
                Editor.heightmap.CreateIsland();
        }

        private void colormapGenerationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Editor.heightmap != null)
            {
                Editor.heightmap.LoadDefaultTextures();
                Editor.heightmap.GenerateColorMap();
            }
        }

        private void heightmapToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ShowSaveDialog();
            Editor.console.Add("Saving heightmap as : " + saveFileDialog1.FileName);
            Editor.heightmap.UpdateHeightFile();
            SaveTexture(Editor.heightmap.heightmap, saveFileDialog1.FileName);
        }

        private void colormapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowSaveDialog();
            Editor.console.Add("Saving texture distribution map as : " + saveFileDialog1.FileName);
            SaveTexture(Editor.heightmap.colormap, saveFileDialog1.FileName);
        }

        private void ShowSaveDialog()
        {
            saveFileDialog1.FileName = string.Empty;
            saveFileDialog1.Filter = "All Images|*.bmp;*.jpg;*.dds;*.tga|Bitmap (*.bmp)|*.bmp|JPEG (*.jpg)|*.jpg|DirectDraw Surface (*.dds)|*.dds|Truevision TGA (*.tga)|*.tga|*.*|*.*";
            saveFileDialog1.ShowDialog();
        }

        private void SaveTexture(Texture2D texture, string filename)
        {
            string format = filename.Substring(filename.Length - 3, 3).ToLower().Trim();

            try
            {
                switch (format)
                {
                    case "bmp":
                        texture.Save(filename, ImageFileFormat.Bmp);
                        break;
                    case "dds":
                        texture.Save(filename, ImageFileFormat.Dds);
                        break;
                    case "jpg":
                        texture.Save(filename, ImageFileFormat.Jpg);
                        break;
                    case "tga":
                        texture.Save(filename, ImageFileFormat.Tga);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (Game1.heightmap != null)
            //{
            saveFileDialog1.FileName = "";
            saveFileDialog1.Filter = "*.xml|*.xml";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != string.Empty)
            {
                Editor.mapName = saveFileDialog1.FileName;
                SaveTerrain(saveFileDialog1.FileName);
            }
            //}
        }

        private void LoadTerrain(string filename)
        {
            if (!File.Exists(filename))
                return;

            XMLReader xmlReader = new XMLReader();
            xmlReader.Open(filename);

            Editor.bDrawSkybox = Convert.ToBoolean(xmlReader.GetElementValue("drawskybox"));

            //Skybox
            if (Editor.skybox == null)
                Editor.skybox = new Skybox();
            Editor.skybox.LoadTextures(xmlReader.GetElementValue("skybox"));

            //Sun Settings
            Editor.bDrawSun = Convert.ToBoolean(xmlReader.GetElementValue("drawsun"));
            Editor.sun = new Sun(Convert.ToDouble(xmlReader.GetElementValue("angle")), Convert.ToDouble(xmlReader.GetElementValue("elevation")));
            string[] sColor = xmlReader.GetElementValue("suncolor").Split(';');
            Editor.sun.color = new Vector3((float)Convert.ToDouble(sColor[0]), (float)Convert.ToDouble(sColor[1]), (float)Convert.ToDouble(sColor[2]));
            Editor.sun.LongitudeSpeed = Convert.ToDouble(xmlReader.GetElementValue("longitudespeed"));
            Editor.sun.LatitudeSpeed = Convert.ToDouble(xmlReader.GetElementValue("latitudespeed"));
            Editor.sun.lightPower = (float)Convert.ToDouble(xmlReader.GetElementValue("intensity"));
            Editor.sun.bCheckTerrainCollision = Convert.ToBoolean(xmlReader.GetElementValue("sunraycollision"));

            //Fog Settings
            Editor.bUseFog = Convert.ToBoolean(xmlReader.GetElementValue("usefog"));
            string[] fColor = xmlReader.GetElementValue("fogcolor").Split(';');
            Microsoft.Xna.Framework.Graphics.Color fogColor = new Microsoft.Xna.Framework.Graphics.Color(new Vector3((float)Convert.ToDouble(fColor[0]), (float)Convert.ToDouble(fColor[1]), (float)Convert.ToDouble(fColor[2])));
            Console.WriteLine("FogColor: " + fogColor.ToString());
            Editor.fogColor = fogColor;
            Editor.fogStart = (float)Convert.ToDouble(xmlReader.GetElementValue("fogstart"));
            Editor.fogEnd = (float)Convert.ToDouble(xmlReader.GetElementValue("fogend"));
            Editor.fogDensity = (float)Convert.ToDouble(xmlReader.GetElementValue("fogdensity"));

            Editor.InitFog();

            //Heightmap
            if (xmlReader.GetElementValue("heightmap") != string.Empty)
            {
                string[] cSize = xmlReader.GetElementValue("cellsize").Split(';');
                Editor.heightmap = new Heightmap(new Vector2((float)Convert.ToDouble(cSize[0]), (float)Convert.ToDouble(cSize[1])));
                
                Editor.heightmap.LoadHeightMap(xmlReader.GetElementValue("heightmap"));
                Editor.heightmap.LoadColormap(xmlReader.GetElementValue("colormap"));

                Editor.heightmap.maxHeight = (float)Convert.ToDouble(xmlReader.GetElementValue("maxheight"));
                Editor.heightmap.bSmooth = Convert.ToBoolean(xmlReader.GetElementValue("smooth"));
                Editor.heightmap.bDrawDetail = Convert.ToBoolean(xmlReader.GetElementValue("drawdetail"));
                string[] aLight = xmlReader.GetElementValue("ambientlight").Split(';');
                Editor.heightmap.ambientLight = new Vector3();
                Editor.heightmap.ambientLight.X = (float)Convert.ToDouble(aLight[0]);
                Editor.heightmap.ambientLight.Y = (float)Convert.ToDouble(aLight[1]);
                Editor.heightmap.ambientLight.Z = (float)Convert.ToDouble(aLight[2]);

                //Texture Layers
                for (int i = 0; i < 4; i++)
                {
                    Editor.heightmap.textureLayer[i].layerTex = xmlReader.GetElementValue("layer" + i + "tex");
                    Editor.heightmap.textureLayer[i].scale = (float)Convert.ToDouble(xmlReader.GetElementValue("layer" + i + "texScale"));
                }

                Editor.heightmap.Init();
            }

            GetSkyboxes();
            GetSunData();
            GetHeightmapData();

        }

        private void SaveTerrain(string filename)
        {
            if (File.Exists(filename))
                File.Delete(filename);

            XMLReader xmlReader = new XMLReader();
            xmlReader.CreateDocument();
            xmlReader.SetRoot("Terrain");

            xmlReader.AddElement("drawskybox", Editor.bDrawSkybox.ToString());
            xmlReader.AddElement("skybox", selectedSky);

            if (Editor.heightmap != null)
            {
                Editor.heightmap.UpdateHeightFile();

                //Heightmap Texture
                string title = Path.GetFileNameWithoutExtension(filename);
                Editor.heightmap.heightmap.Save(title + "_height.bmp", ImageFileFormat.Bmp);
                xmlReader.AddElement("heightmap", title + "_height.bmp");

                //Colormap Texture
                Editor.heightmap.colormap.Save(title + "_color.tga", ImageFileFormat.Tga);
                xmlReader.AddElement("colormap", title + "_color.tga");

                //Heightmap Settings
                xmlReader.AddElement("cellsize", Editor.heightmap.cellSize.X + ";" + Editor.heightmap.cellSize.Y);
                xmlReader.AddElement("maxheight", Editor.heightmap.maxHeight.ToString());
                xmlReader.AddElement("smooth", Editor.heightmap.bSmooth.ToString());
                xmlReader.AddElement("drawdetail", Editor.heightmap.bDrawDetail.ToString());
                xmlReader.AddElement("ambientlight", Editor.heightmap.ambientLight.X + ";" + Editor.heightmap.ambientLight.Y + ";" + Editor.heightmap.ambientLight.Z);

                //Texture Layers Settings
                if (Editor.heightmap.textureLayer != null)
                {
                    for (int i = 0; i < Editor.heightmap.textureLayer.Length; i++)
                    {
                        xmlReader.AddElement("layer" + i + "tex", Editor.heightmap.textureLayer[i].layerTex);
                        xmlReader.AddElement("layer" + i + "texScale", Editor.heightmap.textureLayer[i].scale.ToString().Replace('.', ';'));
                    }
                }
            }

            //Sun Settings
            xmlReader.AddElement("drawsun", Editor.bDrawSun.ToString());
            xmlReader.AddElement("suncolor", Editor.sun.color.X + ";" + Editor.sun.color.Y + ";" + Editor.sun.color.Z);
            xmlReader.AddElement("angle", MathHelper.ToDegrees(Editor.sun.rotation.Y).ToString());
            xmlReader.AddElement("elevation", MathHelper.ToDegrees(Editor.sun.rotation.X).ToString());
            xmlReader.AddElement("longitudespeed", Editor.sun.LongitudeSpeed.ToString());
            xmlReader.AddElement("latitudespeed", Editor.sun.LatitudeSpeed.ToString());
            xmlReader.AddElement("intensity", Editor.sun.lightPower.ToString());
            xmlReader.AddElement("sunraycollision", checkBox4.Checked.ToString());

            //Fog Settings
            xmlReader.AddElement("usefog", Editor.bUseFog.ToString());
            xmlReader.AddElement("fogcolor", Editor.fogColor.ToVector3().X + ";" + Editor.fogColor.ToVector3().Y + ";" + Editor.fogColor.ToVector3().Z);
            xmlReader.AddElement("fogstart", Editor.fogStart.ToString());
            xmlReader.AddElement("fogend", Editor.fogEnd.ToString());
            xmlReader.AddElement("fogdensity", Editor.fogDensity.ToString());

            xmlReader.Save(filename);

            Editor.console.Add("File saved successfuly!");
        }

        private void drawDetail_CheckedChanged(object sender, EventArgs e)
        {
            if (Editor.heightmap != null)
                Editor.heightmap.bDrawDetail = drawDetail.Checked;
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            Editor.bDrawSun = checkBox5.Checked;
        }        

        private void trackBar8_Scroll(object sender, EventArgs e)
        {
            if (Editor.sun != null)
                Editor.sun.lightPower = trackBar8.Value * 0.1f;
        }        

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (Editor.sun != null)
                Editor.sun.LongitudeSpeed = (float)numericUpDown1.Value * 0.01f;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (Editor.sun != null)
                Editor.sun.LatitudeSpeed = (float)numericUpDown2.Value * 0.01f;
        }

        private void lstSkies_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedSky = lstSkies.SelectedItem.ToString();
            Editor.skybox.LoadTextures(selectedSky);
        }

        private void HeightmapSettings_FormClosed(object sender, FormClosedEventArgs e)
        {
            Editor.settings = null;
        }

        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            trackBar2.Value = (int)numericUpDown5.Value;
            this.trackBar2_Scroll_1(sender, e);
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            trackBar4.Value = (int)numericUpDown4.Value;
            this.trackBar4_Scroll(sender, e);
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            trackBar1.Value = (int)numericUpDown3.Value;
            this.trackBar1_Scroll_1(sender, e);
        }

        private void colormapToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (Editor.heightmap != null)
            {
                openFileDialog1.FileName = "";
                openFileDialog1.Filter = "All Images|*.bmp;*.jpg;*.dds;*.tga|Bitmap (*.bmp)|*.bmp|JPEG (*.jpg)|*.jpg|DirectDraw Surface (*.dds)|*.dds|Truevision TGA (*.tga)|*.tga|*.*|*.*";
                openFileDialog1.ShowDialog();

                if (openFileDialog1.FileName != "")
                    Editor.heightmap.LoadColormap(openFileDialog1.FileName);
            }
        }

        private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            if (Editor.heightmap == null)
            {
                //saveToolStripMenuItem.Enabled = false;
                //saveAsToolStripMenuItem.Enabled = false;                
                heightmapToolStripMenuItem1.Enabled = false;
                colormapToolStripMenuItem.Enabled = false;
                colormapToolStripMenuItem1.Enabled = false;
            }
            else
            {
                //saveToolStripMenuItem.Enabled = true;
                //saveAsToolStripMenuItem.Enabled = true;
                heightmapToolStripMenuItem1.Enabled = true;
                colormapToolStripMenuItem.Enabled = true;
                colormapToolStripMenuItem1.Enabled = true;
            }

            if (Editor.mapName == string.Empty)
                saveToolStripMenuItem.Enabled = false;
            else
                saveToolStripMenuItem.Enabled = true;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Editor.bExit = true;
        }

        private void toolsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            if (Editor.heightmap == null)
            {
                heightToolsToolStripMenuItem.Enabled = false;
                textureToolsToolStripMenuItem.Enabled = false;
                randomNoiseToolStripMenuItem.Enabled = false;
                perlinNoiseToolStripMenuItem.Enabled = false;
                randomNoiseToolStripMenuItem1.Enabled = false;
                perlinNoiseToolStripMenuItem1.Enabled = false;
                createIslandToolStripMenuItem.Enabled = false;
                colormapGenerationToolStripMenuItem.Enabled = false;
                smoothMapToolStripMenuItem.Enabled = false;
            }
            else
            {
                heightToolsToolStripMenuItem.Enabled = true;
                textureToolsToolStripMenuItem.Enabled = true;
                randomNoiseToolStripMenuItem.Enabled = true;
                perlinNoiseToolStripMenuItem.Enabled = true;
                randomNoiseToolStripMenuItem1.Enabled = true;
                perlinNoiseToolStripMenuItem1.Enabled = true;
                createIslandToolStripMenuItem.Enabled = true;
                colormapGenerationToolStripMenuItem.Enabled = true;
                smoothMapToolStripMenuItem.Enabled = true;
            }
        }

        public void CreateMap()
        {
            Editor.heightmap = new Heightmap(tileSize);
            Editor.heightmap.maxHeight = mapHeight;
            Editor.heightmap.CreateNewHeightmap(null, new Microsoft.Xna.Framework.Point((int)mapSize.X, (int)mapSize.Y));
        }

        private void trackBar7_Scroll(object sender, EventArgs e)
        {
            if (Editor.heightmap != null)
                Editor.heightmap.ambientLight = Vector3.One * (float)trackBar7.Value / 100f * 2f;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            Editor.sun.bCheckTerrainCollision = checkBox4.Checked;
        }

        private void lightmapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowSaveDialog();
            Editor.console.Add("Saving texture distribution map as : " + saveFileDialog1.FileName);
            SaveTexture(Editor.heightmap.lightMap, saveFileDialog1.FileName);
        }

        private void normalmapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowSaveDialog();
            Editor.console.Add("Saving texture distribution map as : " + saveFileDialog1.FileName);
            SaveTexture(Editor.heightmap.normalMap, saveFileDialog1.FileName);
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Editor.heightmap = null;
            trackBar1.Enabled = true;
            trackBar4.Enabled = true;
            numericUpDown3.Enabled = true;
            numericUpDown4.Enabled = true;
        }
    }
}