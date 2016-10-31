using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeightmapGenerator : Generator
{
    public Color[,] CurrentHeightmap {  get { return m_currentHeightmap; } set { m_currentHeightmap = value; } }

    public List<HeightmapModule> HeightmapModules = new List<HeightmapModule>();
    public int Resolution = 512;
    public bool DumpDebugTex = true;
    public Texture2D OverrideHeightmap = null;

    private Color[,] m_currentHeightmap;

    protected override void DoGenerate()
    {
        GenerateHeightMapData();
    }

    protected override void Finish()
    {
        if (DumpDebugTex)
        {
            DumpDebugHeightmap();
            DumpDebugNormalMap();
        }
    }

    private void GenerateHeightMapData()
    {
        m_currentHeightmap = new Color[Resolution, Resolution];

        // Override map
        if (OverrideHeightmap != null)
        {
            for (int y = 0; y < Resolution; ++y)
            {
                for (int x = 0; x < Resolution; ++x)
                {
                    int remappedX = (int)(((float)x / (float)Resolution) * (float)OverrideHeightmap.width);
                    int remappedY = (int)(((float)y / (float)Resolution) * (float)OverrideHeightmap.height);

                    m_currentHeightmap[x, y] = OverrideHeightmap.GetPixel(remappedX, remappedY);
                }
            }
            m_root.heightmapData = m_currentHeightmap;
            return;
        }

        foreach (var module in HeightmapModules)
        {
            module.RunModule(this);
        }

        

        // Actual generation
/*        for (int y = 0; y < Resolution; ++y)
        {
            for (int x = 0; x < Resolution; ++x)
            {
                heightmapData[x, y] = new Color((Mathf.Sin(((float)x) / 10.0f) + 1.0f) / 2.0f, 0.0f, 0.0f);
            }
        }
        */
        m_root.heightmapData = m_currentHeightmap;
        m_root.CreateHeightmapTexture();
    }

    private void DumpDebugHeightmap()
    {
        // Re-normalize the terrain
        float minHeight = float.MaxValue;
        float maxHeight = -float.MaxValue;

        Color[,] heightmap = new Color[m_root.heightmapData.GetLength(0), m_root.heightmapData.GetLength(1)];

        System.Array.Copy(m_root.heightmapData, heightmap, m_root.heightmapData.GetLength(0) * m_root.heightmapData.GetLength(1));

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
            }
        }

        Color[] texFormatData = new Color[Resolution * Resolution];

        for (int y = 0; y < Resolution; ++y)
        {
            for (int x = 0; x < Resolution; ++x)
            {
                texFormatData[y * Resolution + x] = heightmap[x, y];
                texFormatData[y * Resolution + x].a = 1.0f;
            }
        }

        Texture2D testTexture = new Texture2D(Resolution, Resolution);
        testTexture.SetPixels(texFormatData);

        System.IO.File.WriteAllBytes(@"D:\heightmap.png", testTexture.EncodeToPNG());
    }

    private void DumpDebugNormalMap()
    {
        Color[] normalData = new Color[Resolution * Resolution];

        for (int y = 0; y < Resolution; ++y)
        {
            for (int x = 0; x < Resolution; ++x)
            {
                Vector3 normal = m_root.GetNormalAt((float)x / (float)Resolution, (float)y / (float)Resolution);
                normalData[y * Resolution + x] = new Color((normal.x + 1.0f) / 2.0f, (normal.y + 1.0f) / 2.0f, (normal.z + 1.0f) / 2.0f);
            }
        }

        Texture2D normalTexture = new Texture2D(Resolution, Resolution);
        normalTexture.SetPixels(normalData);

        System.IO.File.WriteAllBytes(@"D:\normals.png", normalTexture.EncodeToPNG());
    }
}
