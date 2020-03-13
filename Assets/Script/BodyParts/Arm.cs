using UnityEngine;

[CreateAssetMenu(fileName = "New Arm", menuName = "Body Parts/New Arm")]
public class Arm : Member
{
    [Range(0,1)] public float force;

    public override float GetForce()
    {
        return force;
    }
}
