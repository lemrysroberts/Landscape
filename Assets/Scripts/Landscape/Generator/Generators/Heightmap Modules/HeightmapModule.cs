using UnityEngine;
using System.Collections;

public abstract class HeightmapModule : MonoBehaviour
{
    public abstract void RunModule(HeightmapGenerator generator);

    public void ChangeTerrainHeight(int x, int y, float amount, ref Color[,] heightmap)
    {
        if (x < 0) x += heightmap.GetLength(0);
        if (x >= heightmap.GetLength(0)) x -= heightmap.GetLength(0);
        if (y < 0) y += heightmap.GetLength(1);
        if (y >= heightmap.GetLength(1)) y -= heightmap.GetLength(1);

        heightmap[x, y].r += amount;
    }
}
