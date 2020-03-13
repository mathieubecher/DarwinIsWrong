using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "New Torso", menuName = "Body Parts/New Torso")]
public class Torso : ScriptableObject
{
    [Range(0, 1)] public float poids;
    [Range(0, 1)] public float cycleReproduction;
    [Range(1, 4)] public int nbMember = 1;
    public Head head;
    public List<Member> members = new List<Member>();

    public float GetPoids()
    {
        return poids + head.poids + members.Sum(member => member.poids);
    }

    public Mob.Alimentation GetAlimentation()
    {
        return head.alimentation;
    }

    public float GetForce()
    {
        return head.force + members.Sum(member => member.GetForce());
    }

    public float GetVitesse()
    {
        return members.Sum(member => member.GetVitesse());
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
