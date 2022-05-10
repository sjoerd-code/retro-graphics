using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Project
{
    public static class Utilities
    {
        public static Model SetupModel(string modelPath, string texturePath, Shader shader)
        {
            Model model = LoadModel(modelPath);
            Texture2D texture = LoadTexture(texturePath);
            Raylib.SetMaterialTexture(ref model, 0, MaterialMapIndex.MATERIAL_MAP_ALBEDO, ref texture);
            Raylib.SetMaterialShader(ref model, 1, ref shader);
            return model;
        }
    }
}