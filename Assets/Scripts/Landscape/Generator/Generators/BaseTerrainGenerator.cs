using UnityEngine;
using System.Collections.Generic;
using System;

public class BaseTerrainGenerator : Generator
{
    public Material baseMaterial = null;
    public float HeightmapMax = 10.0f;

    public int ResolutionX = 500;
    public int ResolutionY = 500;

    GameObject m_baseGameObject = null;
    MeshRenderer m_baseRenderer = null;
    MeshFilter m_baseFilter = null;

    private List<GameObject> m_subMeshes = new List<GameObject>();

    private int m_sectionsX = 0;
    private int m_sectionsY = 0;

    private Vector3[,][] m_vertexData;
    private Vector3[,][] m_normalData;
    private int[,][] m_indexData;
    private Color[,][] m_colorData;
    private Mesh[,] m_meshes;
    private Vector2[,][] m_uvData;

    protected override void DoGenerate()
    {
        Debug.Log("Base Terrain Generator Started...");

        CreateChildObject();
        GenerateInitialMesh();
    }

    protected void CreateChildObject()
    {
        m_baseGameObject = new GameObject(m_root.baseTerrainId);
        m_baseGameObject.transform.parent = m_root.gameObject.transform;

        m_baseRenderer = m_baseGameObject.AddComponent<MeshRenderer>();
        m_baseFilter = m_baseGameObject.AddComponent<MeshFilter>();
    }

    protected void GenerateInitialMesh()
    {
        m_subMeshes.Clear();

        int sectionSize = GetGridSectionSize();

        m_sectionsX = Mathf.CeilToInt((float)ResolutionX / (float)sectionSize);
        m_sectionsY = Mathf.CeilToInt((float)ResolutionY / (float)sectionSize);

        m_vertexData = new Vector3[m_sectionsX, m_sectionsY][];
        m_normalData = new Vector3[m_sectionsX, m_sectionsY][];
        m_colorData = new Color[m_sectionsX, m_sectionsY][];
        m_indexData = new int[m_sectionsX, m_sectionsY][];
        m_meshes = new Mesh[m_sectionsX, m_sectionsY];
        m_uvData = new Vector2[m_sectionsX, m_sectionsY][];

        for (int x = 0; x < m_sectionsX; ++x)
        {
            for (int y = 0; y < m_sectionsY; ++y)
            {
                m_meshes[x, y] = new Mesh();
            }
        }

        for (int x = 0; x < m_sectionsX; ++x)
        {
            for (int y = 0; y < m_sectionsY; ++y)
            {
                GenerateVisSubMeshSection(x, y);
            }
        }
    }

    private void GenerateVisSubMeshSection(int sectionIndexX, int sectionIndexZ)
    {
        int sectionSize = GetGridSectionSize();

        int offsetX = sectionIndexX * sectionSize;
        int offsetZ = sectionIndexZ * sectionSize;

        int sectionsX = Mathf.CeilToInt((float)ResolutionX / sectionSize);
        int sectionsZ = Mathf.CeilToInt((float)ResolutionY / sectionSize);

        int subMeshIndex = sectionIndexX * sectionsX + sectionIndexZ;

        while (subMeshIndex >= m_subMeshes.Count)
        {
            GameObject newSubMesh = new GameObject("Grid Submesh");
            newSubMesh.transform.parent = m_baseGameObject.transform;
            m_subMeshes.Add(newSubMesh);

            newSubMesh.AddComponent<MeshFilter>();
            newSubMesh.AddComponent<MeshRenderer>();
        }

        GameObject subMesh = m_subMeshes[subMeshIndex];
        subMesh.SetActive(true);

        MeshRenderer renderer = subMesh.GetComponent<MeshRenderer>();
        MeshFilter filter = subMesh.GetComponent<MeshFilter>();

        renderer.material = baseMaterial;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;

        int maxX = Mathf.Min(offsetX + sectionSize, ResolutionX);
        int maxZ = Mathf.Min(offsetZ + sectionSize, ResolutionY);

        int numVerts = (maxX - offsetX) * (maxZ - offsetZ) * 4;
        int numIndices = (maxX - offsetX) * (maxZ - offsetZ) * 6;

        if (m_vertexData[sectionIndexX, sectionIndexZ] == null || m_vertexData[sectionIndexX, sectionIndexZ].Length != numVerts)
        {
            m_vertexData[sectionIndexX, sectionIndexZ] = new Vector3[numVerts];
            m_normalData[sectionIndexX, sectionIndexZ] = new Vector3[numVerts];
            m_indexData[sectionIndexX, sectionIndexZ] = new int[numIndices];
            m_colorData[sectionIndexX, sectionIndexZ] = new Color[numVerts];
            m_uvData[sectionIndexX, sectionIndexZ] = new Vector2[numVerts];
        }
        Vector3[] vertices = m_vertexData[sectionIndexX, sectionIndexZ];
        Vector3[] normals = m_normalData[sectionIndexX, sectionIndexZ];
        Color[] colors = m_colorData[sectionIndexX, sectionIndexZ];
        int[] indices = m_indexData[sectionIndexX, sectionIndexZ];
        Vector2[] uvs = m_uvData[sectionIndexX, sectionIndexZ];

        Color red = Color.red;
        Color green = Color.green;

        Vector3 vertex = new Vector3();
        Vector3 normal = new Vector3();

        float m_cellSizeX = m_root.Width / (float)ResolutionX;
        float m_cellSizeY = m_root.Height / (float)ResolutionY;

        for (int x = offsetX; x < maxX; ++x)
        {
            float xSize = x * m_cellSizeX;
            float nextXSize = (x + 1) * m_cellSizeX;

      
            for (int z = offsetZ; z < maxZ; ++z)
            {
                float zSize = z * m_cellSizeY;
                float nextZSize = (z + 1) * m_cellSizeY;

                int index = (x - offsetX) * (maxZ - offsetZ) + (z - offsetZ);

                float currentHeight = m_root.GetHeightAt(xSize / m_root.Width, zSize / m_root.Height) * HeightmapMax;
                vertex.Set(xSize, currentHeight, zSize);
                normal = m_root.GetNormalAt(xSize / m_root.Width, zSize / m_root.Height);
                vertices[index * 4] = vertex;
                normals[index * 4] = normal;

                currentHeight = m_root.GetHeightAt(nextXSize / m_root.Width, zSize / m_root.Height) * HeightmapMax;
                normal = m_root.GetNormalAt(nextXSize / m_root.Width, zSize / m_root.Height);
                vertex.Set(nextXSize, currentHeight, zSize);
                vertices[index * 4 + 1] = vertex;
                normals[index * 4 + 1] = normal;

                currentHeight = m_root.GetHeightAt(xSize / m_root.Width, nextZSize / m_root.Height) * HeightmapMax;
                normal = m_root.GetNormalAt(xSize / m_root.Width, nextZSize / m_root.Height);
                vertex.Set(xSize, currentHeight, nextZSize);
                vertices[index * 4 + 2] = vertex;
                normals[index * 4 + 2] = normal;

                currentHeight = m_root.GetHeightAt(nextXSize / m_root.Width, nextZSize / m_root.Height) * HeightmapMax;
                normal = m_root.GetNormalAt(nextXSize / m_root.Width, nextZSize / m_root.Height);
                vertex.Set(nextXSize, currentHeight, nextZSize);
                vertices[index * 4 + 3] = vertex;
                normals[index * 4 + 3] = normal;

                /*
                Vector2 uv = m_source.GetUvPosition(x, z);
                Vector2 uvSize = m_source.GetUvSize(x, z);

                uvs[index * 4] = uv;
                uvs[index * 4 + 1] = uv + new Vector2(uvSize.x, 0.0f);
                uvs[index * 4 + 2] = uv + new Vector2(0.0f, uvSize.y);
                uvs[index * 4 + 3] = uv + new Vector2(uvSize.x, uvSize.y);
                */

                Color cellColor = Color.red;//.GetColor(x, z);

                colors[index * 4] = cellColor;
                colors[index * 4 + 1] = cellColor;
                colors[index * 4 + 2] = cellColor;
                colors[index * 4 + 3] = cellColor;

                indices[index * 6] = index * 4;
                indices[index * 6 + 1] = index * 4 + 2;
                indices[index * 6 + 2] = index * 4 + 1;
                indices[index * 6 + 3] = index * 4 + 1;
                indices[index * 6 + 4] = index * 4 + 2;
                indices[index * 6 + 5] = index * 4 + 3;

            }
        }

        Mesh mesh = m_meshes[sectionIndexX, sectionIndexZ];
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.SetIndices(indices, MeshTopology.Triangles, 0);
        mesh.colors = colors;
        mesh.RecalculateBounds();

        filter.mesh = mesh;
    }

    private int GetGridSectionSize()
    {
        int maxSquareVerts = (int)(Mathf.Sqrt(65000.0f) / 4.0f);
        return maxSquareVerts;
    }
}
