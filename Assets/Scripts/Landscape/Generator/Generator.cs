using UnityEngine;
using System.Collections;

public abstract class Generator : MonoBehaviour
{
    public LandscapeGenerator LandscapeGenerator {  get { return m_generator; } }
    public LandscapeRoot LandscapeRoot { get { return m_root; } }

    protected LandscapeGenerator m_generator = null;
    protected LandscapeRoot m_root = null;

    public void Generate(LandscapeGenerator rootGenerator)
    {
        m_generator = rootGenerator;
        m_root = rootGenerator.Root;

        DoGenerate();
        Finish();
    }

    protected abstract void DoGenerate();
    protected virtual void Finish() { }
}
