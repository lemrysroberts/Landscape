using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using csDelaunay;

public class VoronoiHeightmapGenerator : HeightmapGenerator
{
    public int NumPoints = 200;

    private List<Vector2> m_points = new List<Vector2>();
    private List<VoronoiRegion> m_regions = new List<VoronoiRegion>();

    private struct VoronoiRegion
    {
        public List<Vector2> regionPoints;
        public bool isWater;
    }

    protected override void DoGenerate()
    {
        m_points = new List<Vector2>();
        m_regions = new List<VoronoiRegion>();

        for(int i = 0; i < NumPoints; ++i)
        {
            float x = Random.value;
            float y = Random.value;

            m_points.Add(new Vector2(x, y));
        }

        Voronoi voronoi = new Voronoi(m_points, new Rectf(0.0f, 0.0f, 1.0f, 1.0f), 30);
        m_points = voronoi.SiteCoords();
        
        foreach (var region in voronoi.Regions())
        {
            VoronoiRegion newRegion = new VoronoiRegion();
            newRegion.regionPoints = region;
            newRegion.isWater = Random.Range(0, 2) == 0;

            m_regions.Add(newRegion);
        }
        
    }

    protected override void Finish()
    {
        //  base.Finish();
        
        Color[] texFormatData = new Color[Resolution * Resolution];

        const float dotRange = 0.00002f;

        for (int y = 0; y < Resolution; ++y)
        {
            for (int x = 0; x < Resolution; ++x)
            {
                float normalisedX = (float)x / (float)Resolution;
                float normalisedY = (float)y / (float)Resolution;

                Vector2 testVec = new Vector2(normalisedX, normalisedY);

                if(m_points.Any(w => (w - testVec).sqrMagnitude < dotRange ))
                {
                    texFormatData[y * Resolution + x] = Color.red;
                }
                else
                {
                    texFormatData[y * Resolution + x] = Color.white;
                }
            }
        }

        Texture2D testTexture = new Texture2D(Resolution, Resolution);
        testTexture.SetPixels(texFormatData);

        System.IO.File.WriteAllBytes(@"D:\voronoi.png", testTexture.EncodeToPNG());
    }
}
