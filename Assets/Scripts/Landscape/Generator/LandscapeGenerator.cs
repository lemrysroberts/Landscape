using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class LandscapeGenerator : MonoBehaviour
{
    public int Seed = -1;

    public List<Generator> Generators = new List<Generator>();

    public LandscapeRoot Root {  get { return m_root; } }

    private LandscapeRoot m_root;

    public void Generate(LandscapeRoot root)
    {
        m_root = root;

        InitRng();
        ClearPreviousLandscape();

        foreach(var generator in Generators)
        {
            generator.Generate(this);
        }
    }

    private void InitRng()
    {
        int currentSeed = Seed;
        if (Seed == -1)
        {
            currentSeed = Random.Range(int.MinValue, int.MaxValue);
        }

        Debug.Log("Initialised with seed: " + currentSeed);
        Random.InitState(currentSeed);
    }

    private void ClearPreviousLandscape()
    {
        Transform[] transforms = m_root.GetComponentsInChildren<Transform>().GroupBy(x => x.gameObject).Select(x => x.First()).ToArray();

        foreach(var transform in transforms)
        {
            if(transform != null && transform.gameObject != null)
            {
                if (transform.gameObject != gameObject)
                {
                    DestroyImmediate(transform.gameObject);
                }
            }
        }
    }
}
