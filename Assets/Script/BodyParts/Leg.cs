using UnityEngine;

[CreateAssetMenu(fileName = "New Leg", menuName = "Body Parts/New Leg")]
public class Leg : Member
{
    [Range(0,1)] public float vitesse;

    public override float GetVitesse()
    {
        return vitesse;
    }
}
