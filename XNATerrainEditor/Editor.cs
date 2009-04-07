//======================================================================
// XNA Terrain Editor
// Copyright (C) 2008 Eric Grossinger
// http://psycad007.spaces.live.com/
//======================================================================
#region Using Statements
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
#endregion

namespace XNATerrainEditor
{
    public class Editor : Microsoft.Xna.Framework.Game
    {
        public static GraphicsDeviceManager graphics;
        public static ContentManager content;
        SpriteBatch spriteBatch;

        private Grid grid;
        public static bool bDrawGrid = true;

        public static Camera camera;

        public static Heightmap heightmap;

        //Optimized_Heightmap testHeightmap;
        //HeightmapModifier heightmapModifier;

        public static Skybox skybox;
        public static bool bDrawSkybox = true;

        public static Sun sun;
        public static bool bDrawSun = true;
        
        public static Water water;
        public static float waterHeight = 20f;
        public static bool bDrawWater = false;

        public static string mapName = string.Empty;

        HUD hud;

        #region PostProcessing
        
        RenderTarget2D renderTarget;
        Texture2D texturedRenderedTo;
        
        Effect postEffect;
        SpriteBlendMode postBlendMode = SpriteBlendMode.None;
        float camSpeed = 0f;
        float effectSpeed = 0f;
        bool bDrawPostEffect = false;

        #endregion

        RenderTarget2D refractionRenderTarg;
        Texture2D refractionMap;

        RenderTarget2D reflectionRenderTarg;
        Texture2D reflectionMap;

        public static bool bMouseVisible = false;

        public static int timeSeconds = 0;
        public static int timeMilliseconds = 0;

        public static HeightmapSettings settings;
        public static HeightTools heightTools;
        public static PaintTools paintTools;
        
        private static ConsoleHUD consoleHUD;
        public static ConsoleHUD console
        {
            get { return consoleHUD; }
            set { consoleHUD = value; }
        }

        public static FogMode fogMode = FogMode.Linear;
        public static bool bUseFog = true;
        public static Color fogColor = Color.White;
        public static float fogStart = 1000f;
        public static float fogEnd = 10000f;
        public static float fogDensity = 0.8f;

        public static bool bExit = false;

        public static string AppPath = "";

        public Editor()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            content = new ContentManager(Services);
            AppPath = Directory.GetCurrentDirectory();
        }

        protected override void Initialize()
        {
            consoleHUD = new ConsoleHUD();

            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);

            renderTarget = new RenderTarget2D(graphics.GraphicsDevice, 800, 600, 1, SurfaceFormat.Color);            
            refractionRenderTarg = new RenderTarget2D(graphics.GraphicsDevice, 800, 600, 1, SurfaceFormat.Color);
            reflectionRenderTarg = new RenderTarget2D(graphics.GraphicsDevice, 800, 600, 1, SurfaceFormat.Color);

            camera = new MouseCam();
            camera.SetPosition(new Vector3(1500f, 400f, 1500f));
            camera.rotation = new Vector3(0f, MathHelper.ToRadians(180f), 0f);

            //grid = new Grid(new Vector2(1000f, 1000f), 10, Color.Black, 1f, Vector3.Zero, new Vector3(0f, 0f, 0f));
            //water = new Water(new Vector2(40000f, 40000f), false);

            skybox = new Skybox();
            sun = new Sun(342f, 335f);
            hud = new HUD();

            InitFog();

            base.Initialize();

            settings = new HeightmapSettings();
            settings.Show();

            //camera.viewDistance = 6000f;
            //testHeightmap = new Optimized_Heightmap(graphics.GraphicsDevice, new Point(256, 256), new Vector2(50f, 50f), new Point(16, 16));
            //heightmapModifier = new HeightmapModifier(ref testHeightmap, 30);
        }

        public static void InitFog()
        {
            graphics.GraphicsDevice.RenderState.FogStart = fogStart;
            graphics.GraphicsDevice.RenderState.FogEnd = fogEnd;
            graphics.GraphicsDevice.RenderState.FogDensity = fogDensity;
            graphics.GraphicsDevice.RenderState.FogColor = fogColor;
            graphics.GraphicsDevice.RenderState.FogTableMode = fogMode;
            graphics.GraphicsDevice.RenderState.FogVertexMode = fogMode;
        }

        protected override void LoadContent()
        {
            postEffect = content.Load<Effect>(@"content\\shaders\\postProcessing");
            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) || bExit)
                this.Exit();

            //heightmapModifier.Update();
            //testHeightmap.Update();
           
            timeSeconds = (int)gameTime.TotalRealTime.TotalSeconds;
            timeMilliseconds = (int)gameTime.TotalRealTime.TotalMilliseconds;

            CheckPCInput();

            if (camera != null)
            {
                camera.Update(gameTime);

                if (camera.state == Camera.State.Fixed && !IsMouseVisible)
                    IsMouseVisible = true;
                else if (camera.state != Camera.State.Fixed && IsMouseVisible)
                    IsMouseVisible = false;
            }

            if (skybox != null && camera != null && bDrawSkybox)
                skybox.Update(camera.position);

            if (sun != null)
                sun.Update();

            if (water != null && bDrawWater)
                water.Update(gameTime);

            if (heightmap != null && heightmap.bUpdateAndDraw)
                heightmap.Update();

            if (consoleHUD != null)
                consoleHUD.Update();

            base.Update(gameTime);
        }

        private void CheckPCInput()
        {
            KeyboardState ks = Keyboard.GetState();

            if ((ks.IsKeyDown(Keys.LeftControl) || ks.IsKeyDown(Keys.RightControl)))
            {                
                if (ks.IsKeyDown(Keys.S) && (settings == null || !settings.Visible || !settings.Focused))
                {
                    if (settings == null)
                    {
                        settings = new HeightmapSettings();
                        settings.Visible = true;
                    }
                    else if (!settings.Visible)
                    {
                        settings.Visible = true;
                    }
                    else if(!settings.Focused)
                    {
                        settings.Focus();
                        settings.BringToFront();
                    }

                    if (camera != null)
                        camera.SetState(Camera.State.Fixed);
                }                
            }
        }     

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            //graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

            if (graphics.GraphicsDevice.RenderState.AlphaBlendEnable)
                graphics.GraphicsDevice.RenderState.AlphaBlendEnable = false;
            if (!graphics.GraphicsDevice.RenderState.DepthBufferEnable)
                graphics.GraphicsDevice.RenderState.DepthBufferEnable = true;
            if (!graphics.GraphicsDevice.RenderState.DepthBufferWriteEnable)
                graphics.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
            //graphics.GraphicsDevice.RenderState.StencilEnable = true;

            //Water Reflection/Refraction
            /*if (IsActive)
            {
                if (heightmap != null && water != null)
                    DrawRefractionMap();
                if (water != null && skybox != null)
                    DrawReflectionMap();
            }
            */

            //graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1.0f, 0);

            //if (bDrawPostEffect)
            //    PreRender();

            if (grid != null && bDrawGrid)
                grid.Draw(camera.view, camera.projection);

            if (skybox != null && bDrawSkybox)
                skybox.Draw(camera.view, camera.projection);

            if (heightmap != null && heightmap.bUpdateAndDraw)
                heightmap.Draw(camera.view, camera.projection);

            //testHeightmap.Draw(graphics.GraphicsDevice, camera.view, camera.projection);

            if (water != null && bDrawWater)
                water.Draw(camera.view, camera.projection, camera.reflectionView, reflectionMap, refractionMap);

            base.Draw(gameTime);

            if (bDrawSun && sun != null && camera != null)
                sun.Draw(camera.view, camera.projection);

            if (bDrawPostEffect)
                PostRender();

            if (hud != null)
                hud.Draw();

            if (consoleHUD != null && (consoleHUD.state == ConsoleHUD.State.Opening || consoleHUD.state == ConsoleHUD.State.Opened || consoleHUD.state == ConsoleHUD.State.Closing))
                consoleHUD.Draw();
        }

        private void PreRender()
        {
            graphics.GraphicsDevice.SetRenderTarget(0, renderTarget);
        }

        private void PostRender()
        {
            //graphics.GraphicsDevice.ResolveRenderTarget(0);
            texturedRenderedTo = renderTarget.GetTexture();

            graphics.GraphicsDevice.SetRenderTarget(0, null);
            graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

            postEffect.Begin();

            camSpeed = Math.Abs(Editor.camera.speed);

            if (camSpeed != 0f)
            {
                if (effectSpeed < camSpeed * 0.003f)
                    effectSpeed = camSpeed * 0.003f;
            }
            else
            {
                if (effectSpeed > 0.0003f)
                    effectSpeed *= 0.92f;
                else
                    effectSpeed = 0f;
            }

            postEffect.Parameters["speed"].SetValue(effectSpeed);

            spriteBatch.Begin(postBlendMode, SpriteSortMode.Immediate, SaveStateMode.SaveState);

            EffectPass pass = postEffect.CurrentTechnique.Passes[0];
            pass.Begin();

            spriteBatch.Draw(texturedRenderedTo, new Vector2(0, 0), null, Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 1);

            pass.End();
            spriteBatch.End();
            postEffect.End();
        }
        
        /// <summary>
        /// http://www.riemers.net/eng/Tutorials/XNA/Csharp/Series4/Refraction_map.php
        /// </summary>
        bool bSaveRefraction = false;
        private void DrawRefractionMap()
        {
            Vector3 planeNormalDirection = new Vector3(0, -1, 0);
            planeNormalDirection.Normalize();
            Vector4 planeCoefficients = new Vector4(planeNormalDirection, waterHeight);

            Matrix camMatrix = camera.view * camera.projection;
            Matrix invCamMatrix = Matrix.Invert(camMatrix);
            invCamMatrix = Matrix.Transpose(invCamMatrix);

            planeCoefficients = Vector4.Transform(planeCoefficients, invCamMatrix);
            Plane refractionClipPlane = new Plane(planeCoefficients);

            graphics.GraphicsDevice.ClipPlanes[0].Plane = refractionClipPlane;
            graphics.GraphicsDevice.ClipPlanes[0].IsEnabled = true;

            graphics.GraphicsDevice.SetRenderTarget(0, refractionRenderTarg);
            graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

            if (heightmap != null)
                heightmap.Draw(camera.view, camera.projection);
            
            //graphics.GraphicsDevice.ResolveRenderTarget(0);
            refractionMap = refractionRenderTarg.GetTexture();
            graphics.GraphicsDevice.SetRenderTarget(0, null);

            graphics.GraphicsDevice.ClipPlanes[0].IsEnabled = false;

            if (bSaveRefraction)
            {
                refractionMap.Save("refractionMap.jpg", ImageFileFormat.Jpg);
                Editor.console.Add("Refraction jpeg saved.");
                bSaveRefraction = false;
            }
        }

        /// <summary>
        /// http://www.riemers.net/eng/Tutorials/XNA/Csharp/Series4/Reflection_map.php
        /// </summary>
        bool bSaveReflection = false;
        private void DrawReflectionMap()
        {
            Vector3 planeNormalDirection = new Vector3(0, 1, 0);
            planeNormalDirection.Normalize();
            Vector4 planeCoefficients = new Vector4(planeNormalDirection, -waterHeight);

            Matrix camMatrix = camera.reflectionView * camera.projection;
            Matrix invCamMatrix = Matrix.Invert(camMatrix);
            invCamMatrix = Matrix.Transpose(invCamMatrix);

            planeCoefficients = Vector4.Transform(planeCoefficients, invCamMatrix);
            Plane reflectionClipPlane = new Plane(planeCoefficients);

            graphics.GraphicsDevice.ClipPlanes[0].Plane = reflectionClipPlane;
            graphics.GraphicsDevice.ClipPlanes[0].IsEnabled = true;

            graphics.GraphicsDevice.SetRenderTarget(0, reflectionRenderTarg);
            graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

            if (skybox != null)
                skybox.Draw(camera.reflectionView, camera.projection);

            if (heightmap != null)
                heightmap.Draw(camera.reflectionView, camera.projection);

            //graphics.GraphicsDevice.ResolveRenderTarget(0);
            reflectionMap = reflectionRenderTarg.GetTexture();
            graphics.GraphicsDevice.SetRenderTarget(0, null);

            graphics.GraphicsDevice.ClipPlanes[0].IsEnabled = false;

            if (bSaveReflection)
            {
                reflectionMap.Save("reflectionMap.jpg", ImageFileFormat.Jpg);
                Editor.console.Add("Reflection jpeg saved.");
                bSaveReflection = false;
            }
        }
    }
}