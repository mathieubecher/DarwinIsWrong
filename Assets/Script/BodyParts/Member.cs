using UnityEngine;

public class Member : ScriptableObject
{
    [Range(0,1)] public float poids;

    public virtual float GetForce()
    {
        return 0f;
    }

    public virtual float GetVitesse()
    {
        return 0f;
    }
}
