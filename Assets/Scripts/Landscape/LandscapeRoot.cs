using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(LandscapeGenerator))]
public class LandscapeRoot : MonoBehaviour
{
    public float Width = 50.0f;
    public float Height = 50.0f;

    public string baseTerrainId {  get { return "base_terrain"; } }
    public Color[,] heightmapData { get; set; }

    private Texture2D m_heightmapTexture;

    public void Generate()
    {
        LandscapeGenerator generator = GetComponent<LandscapeGenerator>();
        generator.Generate(this);
    }
    
    public void CreateHeightmapTexture()
    {
        int Resolution = heightmapData.GetLength(0);
        Color[] texFormatData = new Color[Resolution * Resolution];

        for (int y = 0; y < Resolution; ++y)
        {
            for (int x = 0; x < Resolution; ++x)
            {
                texFormatData[y * Resolution + x] = heightmapData[x, y];
                texFormatData[y * Resolution + x].a = 1.0f;
            }
        }

        m_heightmapTexture = new Texture2D(Resolution, Resolution);
        m_heightmapTexture.SetPixels(texFormatData);
    }

    public float GetHeightAt(float normalisedX, float normalisedY)
    {
        if(heightmapData == null || m_heightmapTexture == null)
        {
         //   Debug.LogError("Accessing height-map data before it has been set");
            return 0.0f;
        }

        Debug.Log("Normalised: " + normalisedX + ", " + normalisedY);

        return m_heightmapTexture.GetPixelBilinear(normalisedX, normalisedY).r * 10.0f;
    }

    public Vector3 GetNormalAt(float normalisedX, float normalisedY)
    {
        float sampleOffset = 0.002f;
     
        float h0 = GetHeightAt(normalisedX - sampleOffset, normalisedY - sampleOffset);
        float h1 = GetHeightAt(normalisedX , normalisedY - sampleOffset);
        float h2 = GetHeightAt(normalisedX + sampleOffset, normalisedY - sampleOffset);

        float h3 = GetHeightAt(normalisedX - sampleOffset, normalisedY);
        float h4 = GetHeightAt(normalisedX, normalisedY );
        float h5 = GetHeightAt(normalisedX + sampleOffset, normalisedY);

        float h6 = GetHeightAt(normalisedX - sampleOffset, normalisedY + sampleOffset);
        float h7 = GetHeightAt(normalisedX, normalisedY + sampleOffset);
        float h8 = GetHeightAt(normalisedX + sampleOffset, normalisedY + sampleOffset);

        Vector3 normal = new Vector3();
        normal.x = -(h2 - h0 + 2.0f * (h5 - h3) + h8 - h6);
        normal.y = -(h6 - h0 + 2.0f * (h7 - h1) + h8 - h2);
        normal.z = 1.0f;

        return -normal.normalized;
    }
}
