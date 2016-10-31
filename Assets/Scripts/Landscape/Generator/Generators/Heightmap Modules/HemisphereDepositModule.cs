using UnityEngine;
using System.Linq;
using System.Collections;

public class HemisphereDepositModule : HeightmapModule
{
    public int Iterations = 2;
    public float MaxSize = 0.1f;
    public float MinSize = 0.001f;
    public float ScaleFactor = 1.0f;
    public float ScalePower = 1.0f;

    public override void RunModule(HeightmapGenerator generator)
    {
        float width = generator.LandscapeRoot.Width;
        float height = generator.LandscapeRoot.Height;

        Color[,] heightmap = generator.CurrentHeightmap;

        for (int i = 0; i < Iterations; ++i)
        {
            float x = Random.value;
            float y = Random.value;
            float radius = Random.Range(MinSize, MaxSize);

            int startX = (int)(x * heightmap.GetLength(0));
            int startY = (int)(y * heightmap.GetLength(1));

            int vertexRadius = (int)(radius * heightmap.GetLength(0));

            for (int offsetX = -vertexRadius; offsetX < vertexRadius; ++offsetX)
            {
                for (int offsetY = -vertexRadius; offsetY < vertexRadius; ++offsetY)
                {
                    float magnitude = (new Vector3((float)offsetX, (float)(offsetY), 0.0f)).magnitude / (float)heightmap.GetLength(0);
                    float heightVal = Mathf.Max(radius - magnitude, 0.0f);

                    ChangeTerrainHeight(startX + offsetX, startY + offsetY, heightVal, ref heightmap);
                }
            }
        }

        // Re-normalize the terrain
        float minHeight = float.MaxValue;
        float maxHeight = -float.MaxValue;

        for (int x = 0; x < heightmap.GetLength(0); ++x)
        {
            for (int y = 0; y < heightmap.GetLength(1); ++y)
            {
                if (heightmap[x, y].r < minHeight) minHeight = heightmap[x, y].r;
                if (heightmap[x, y].r > maxHeight) maxHeight = heightmap[x, y].r;
            }
        }

        for (int x = 0; x < heightmap.GetLength(0); ++x)
        {
            for (int y = 0; y < heightmap.GetLength(1); ++y)
            {
                heightmap[x, y].r = (heightmap[x, y].r - minHeight) / (maxHeight - minHeight);
                heightmap[x, y].r = Mathf.Pow(heightmap[x, y].r, ScalePower);
                heightmap[x, y].r *= ScaleFactor;
            }
        }
    }
}
