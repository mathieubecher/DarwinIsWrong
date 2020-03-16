using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "New Torso", menuName = "Body Parts/New Torso")]
public class Torso : ScriptableObject
{
    [Range(0, 1)] public float poids;
    [Range(0, 1)] public float cycleReproduction;
    [Range(0, 4)] public int nbMember = 2;
    public Head head;
    public List<Member> members = new List<Member>();

    public float GetPoids()
    {
        return Mathf.Min(poids + head.poids + members.Sum(member => member.poids), 1f);
    }

    public Mob.Alimentation GetAlimentation()
    {
        return head.alimentation;
    }

    public float GetForce()
    {
        return Mathf.Min(head.force + members.Sum(member => member.GetForce()), 1f);
    }

    public float GetVitesse()
    {
        return Mathf.Max(Mathf.Min(members.Sum(member => member.GetVitesse()) - GetPoids() / 2f, 1f), 0f);
    }

    public float GetSatiete()
    {
        return GetPoids();
    }

    public float GetMaturationTemps()
    {
        return GetPoids();
    }

    public float GetCycleReproduction()
    {
        return cycleReproduction;
    }
    
    
}
