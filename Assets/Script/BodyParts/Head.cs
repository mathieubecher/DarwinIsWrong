using UnityEngine;

[CreateAssetMenu(fileName = "New Head", menuName = "Body Parts/New Head")]
public class Head : ScriptableObject
{
    [Range(0,1)] public float poids;
    [Range(0,1)] public float force;
    public Mob.Alimentation alimentation;
}
