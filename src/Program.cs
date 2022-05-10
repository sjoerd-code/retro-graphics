using System;
using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Project
{
    public class Program
    {
        public static void Main()
        {
            // window setup
            const int screenWidth = 1280;
            const int screenHeight = 720;
            InitWindow(screenWidth, screenHeight, "window");
            SetTargetFPS(60);

            // camera setup
            Camera3D camera = new Camera3D();
            camera.position = new Vector3(0, 6, 8);
            camera.target = new Vector3(0.0f, 0.0f, 0.0f);
            camera.up = new Vector3(0.0f, 1.0f, 0.0f);
            camera.fovy = 45;
            camera.projection = CameraProjection.CAMERA_PERSPECTIVE;
            SetCameraMode(camera, CameraMode.CAMERA_ORBITAL);

            // Load shader and set up some uniforms
            float fogDensity = 0.1f;
            Shader defaultShader = LoadShader("resources/shaders/base_lighting.vs", "resources/shaders/fog.fs");
            Raylib.SetShaderValue(defaultShader, GetShaderLocation(defaultShader, "ambient"), new float[] { 0.2f, 0.2f, 0.2f, 1.0f }, ShaderUniformDataType.SHADER_UNIFORM_VEC4);
            Raylib.SetShaderValue(defaultShader, GetShaderLocation(defaultShader, "fogDensity"), fogDensity, ShaderUniformDataType.SHADER_UNIFORM_FLOAT);

            // load post shader
            Shader postShader = LoadShader(null, "resources/shaders/pixelizer.fs");

            // Create a RenderTexture2D to be used for render to texture
            RenderTexture2D target = LoadRenderTexture(screenWidth, screenHeight);

            // directional light setup
            lights.CreateLight(0, LightType.LIGHT_DIRECTIONAL, new Vector3(0, 1, 1), Vector3.Zero, Color.WHITE, defaultShader);

            // models setup
            Model rock_model = Utilities.SetupModel("resources/models/rock.glb", "resources/textures/rock_lowres.png", defaultShader);
            Model rock_grassy_model = Utilities.SetupModel("resources/models/rock_grassy.glb", "resources/textures/rock_grassy_lowres.png", defaultShader);

            // main loop
            while (!WindowShouldClose())
            {
                // Update
                UpdateCamera(ref camera);

                // shader values to update each frame
                Raylib.SetShaderValue(defaultShader, GetShaderLocation(defaultShader, "fogDensity"), fogDensity, ShaderUniformDataType.SHADER_UNIFORM_FLOAT);
                Raylib.SetShaderValue(defaultShader, GetShaderLocation(defaultShader, "viewPos"), camera.position, ShaderUniformDataType.SHADER_UNIFORM_VEC3);

                // Draw
                BeginDrawing();
                ClearBackground(Color.GRAY);

                BeginTextureMode(target); // normal pass
                    ClearBackground(Color.GRAY);
                    BeginMode3D(camera);
                        int amount = 10;
                        int distance = 3;
                        for (int x = 0; x < amount; x++) 
                            for (int z = 0; z < amount; z++) 
                                DrawModel(rock_grassy_model, new Vector3((-amount * distance / 2) + (x * distance), 0, (-amount * distance / 2) + (z * distance)), 1, Color.WHITE);
                    EndMode3D();
                EndTextureMode();

                BeginShaderMode(postShader); // post pass
                    DrawTextureRec(target.texture, new Rectangle(0, 0, target.texture.width, -target.texture.height), new Vector2(0, 0), Color.WHITE);
                EndShaderMode();

                EndDrawing();
            }

            // on window close
            CloseWindow();
        }

        private Model SetupModel(string modelPath, string texturePath, Shader shader)
        {
            Model model = LoadModel(modelPath);
            Texture2D texture = LoadTexture(texturePath);
            Raylib.SetMaterialTexture(ref model, 0, MaterialMapIndex.MATERIAL_MAP_ALBEDO, ref texture);
            Raylib.SetMaterialShader(ref model, 1, ref shader);
            return model;
        }
    }
}