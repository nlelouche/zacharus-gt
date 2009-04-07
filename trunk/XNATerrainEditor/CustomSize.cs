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

namespace XNATerrainEditor
{
    public partial class CustomSize : Form
    {
        //HeightmapSettings appSettings;

        public CustomSize(HeightmapSettings settings)
        {
            InitializeComponent();
            //appSettings = settings;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //appSettings.mapSize = new Vector2((float)numericUpDown1.Value, (float)numericUpDown2.Value);
            //appSettings.CreateMap();

            if (Editor.paintTools != null)
                Editor.paintTools.Close();
            if (Editor.heightTools != null)
                Editor.heightTools.Close();

            Editor.heightmap = new Heightmap(new Vector2(50f, 50f));
            Editor.heightmap.maxHeight = 500f;
            Editor.heightmap.CreateNewHeightmap(null, new Point((int)numericUpDown1.Value, (int)numericUpDown2.Value));
            Editor.settings.GetHeightmapData();
            Editor.mapName = string.Empty;

            this.Close();
        }
    }
}