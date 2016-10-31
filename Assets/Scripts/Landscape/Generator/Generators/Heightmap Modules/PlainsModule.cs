using UnityEngine;
using System.Collections;

public class PlainsModule : HeightmapModule
{
    public override void RunModule(HeightmapGenerator generator)
    {
        float width = generator.LandscapeRoot.Width;
        float height = generator.LandscapeRoot.Height;

        Color[,] heightmap = generator.CurrentHeightmap;

        int xSize = heightmap.GetLength(0);
        int ySize = heightmap.GetLength(1);

        int maxSize = Mathf.Max(xSize, ySize);
        int nextPower = (int)Mathf.Pow(2, Mathf.Ceil(Mathf.Log(maxSize - 1) / Mathf.Log(2)));

        Debug.Log("Next power: " + nextPower);

        Color[,] target = new Color[nextPower + 1, nextPower + 1];

        const float minHeight = 30.0f;
        const float maxHeight = 30.0f;

        target[0, 0].r = Random.Range(minHeight, maxHeight);
        target[nextPower, 0].r = Random.Range(minHeight, maxHeight);
        target[0, nextPower].r = Random.Range(minHeight, maxHeight);
        target[nextPower, nextPower].r = Random.Range(minHeight, maxHeight);

        target[0, 0].a = 1.0f;
        target[nextPower, 0].a = 1.0f;
        target[0, nextPower].a = 1.0f;
        target[nextPower, nextPower].a = 1.0f;

        int halfOffset = nextPower / 2;
        int iteration = 0;

        int texSize = nextPower + 1;
        /*
        while(halfOffset >= 1 && iteration < 10000000)
        {
            // Diamond
            for (int y = halfOffset; y + halfOffset < texSize; y += halfOffset * 2)
            {
                for (int x = halfOffset; x + halfOffset < texSize; x += halfOffset * 2)
                {
                
                    if (target[x, y].a == 1.0f) continue;

                    

                    float v0 = target[x - halfOffset, y - halfOffset].r;
                    float v1 = target[x + halfOffset, y - halfOffset].r;
                    float v2 = target[x - halfOffset, y + halfOffset].r;
                    float v3 = target[x + halfOffset, y + halfOffset].r;

                    target[x, y].r = (v0 + v1 + v2 + v3) / 4.0f;
                    target[x, y].a = 1.0f;
                    Debug.Log("Diamond (" + x + ", " + y + ") : " + target[x, y].r.ToString());
                }
            }

            // Square
            for (int y = 0; y < texSize; y += halfOffset)
            {
                for (int x = 0; x < texSize; x += halfOffset)
                {
                
                    if (target[x, y].a == 1.0f) continue;

                    int index0X = ((x - halfOffset) + texSize) % texSize;
                    int index1X = (x + halfOffset) % texSize;
                    int index2X = x;
                    int index3X = x;

                    int index0Y = y;
                    int index1Y = y;
                    int index2Y = ((y - halfOffset) + texSize) % texSize;
                    int index3Y = (y + halfOffset) % texSize;

                    float v0 = target[index0X, index0Y].r;
                    float v1 = target[index1X, index1Y].r;
                    float v2 = target[index2X, index2Y].r;
                    float v3 = target[index3X, index3Y].r;

                    target[x, y].r = (v0 + v1 + v2 + v3) / 4.0f;
                    target[x, y].a = 1.0f;
                    Debug.Log("Square (" + x + ", " + y + ") : " + target[x, y].r.ToString());
                }
            }

            halfOffset = halfOffset / 2;
            iteration++;
        }
        
        for(int x = 0; x < heightmap.GetLength(0); ++x)
        {
            for (int y = 0; y < heightmap.GetLength(1); ++y)
            {
                float normalisedX = (float)x / (float)heightmap.GetLength(0);
                float normalisedY = (float)y / (float)heightmap.GetLength(1);

                
                heightmap[x, y] = target[(int)(normalisedX * (float)target.GetLength(0)), (int)(normalisedY * (float)target.GetLength(1))];
            }
        }*/
        generator.CurrentHeightmap = target;
    }
}
