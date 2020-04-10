using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class Genetic : MonoBehaviour
{
    private bool _verbose = false;
    
    public Mob defaultMob;
    public Material materialHerbivore;
    public Material materialCarnivore;
    
    public Mob.Alimentation alimentation;
    [Range(0,1)]
    public float force = 0.5f;
    [Range(0,1)]
    public float vitesse = 0.5f;
    [Range(0,1)]
    public float cycleReproduction = 0.5f;

    private struct MobNote : IComparable<MobNote>
    {
        public readonly Mob mob;
        public float note;

        public MobNote(Mob mob, float note = -1f)
        {
            this.mob = mob;
            this.note = note;
        }

        public int CompareTo(MobNote other)
        {
            return note.CompareTo(other.note);
        }
    }
    
    public void CreateMonster()
    {
        Log("Creating Monster");
        
        Log("Initialization");
        var parent = new GameObject("CreateMonsterTempStorage");
        const int maxPop = 10;
        var mobNs = new List<MobNote>();
        var torsos = Resources.LoadAll<Torso>("BodyParts/Torsos/");
        
        var heads = Resources.LoadAll<Head>("BodyParts/Heads/")
            .Where(head => head.alimentation == alimentation).ToList();

        var members = new List<Member>(Resources.LoadAll<Arm>("BodyParts/Arms/"));
        members.AddRange(Resources.LoadAll<Leg>("BodyParts/Legs/"));
        
        while (mobNs.Count < maxPop)
        {
            var mob = CreateMob(parent, torsos, heads, members);
            var note = Evaluate(mob);
            mobNs.Add(new MobNote(mob, note));
        }
        mobNs.Sort();

        Log("Running Genetic Algorithm");
        const int nbGen = 50;
        for (var i = 0; i < nbGen; ++i)
        {
            // Select parents (Delete those we don't want to keep)
            mobNs.GetRange(maxPop / 5, mobNs.Count - (maxPop/5)).ForEach(mobN => DestroyImmediate(mobN.mob.gameObject));
            mobNs.RemoveRange(maxPop / 5, mobNs.Count - (maxPop/5) );
            
            // Recombine
            while (mobNs.Count < maxPop)
            {
                int index1, index2 = index1 = Random.Range(0, maxPop / 5);
                while (index1 == index2) index2 = Random.Range(0, maxPop / 5);
                var mobN1 = mobNs[index1];
                var mobN2 = mobNs[index2];
                mobNs.Add(new MobNote(Combine(parent, mobN1.mob, mobN2.mob)));
            }
            
            // Mutate
            var indexes = new SortedSet<int>();
            while (indexes.Count <  maxPop / 10)
            {
                indexes.Add(Random.Range(0, maxPop));
            }
            foreach (var index in indexes)
            {
                Mutate(mobNs[index].mob, torsos, heads, members);
            }
            
            // Evaluate
            mobNs.ForEach(mobNote => mobNote.note = Evaluate(mobNote.mob));
            
            mobNs.Sort();
        }
        DestroyImmediate(mobNs[0].mob.GetComponent<MeshRenderer>().material);
        mobNs[0].mob.GetComponent<MeshRenderer>().material = mobNs[0].mob.alimentation == Mob.Alimentation.carnivore
            ? materialCarnivore
            : materialHerbivore;
        
        Log("Creating Prefab of the best monster");
        var localPath = "Assets/Resources/Mob";
        if (!AssetDatabase.IsValidFolder(localPath + "/Completed"))
            AssetDatabase.CreateFolder(localPath, "Completed");
        localPath += "/Completed/Mob_"
                     + (mobNs[0].mob.alimentation == Mob.Alimentation.carnivore ? "C" : "H")
                     + ".prefab";
        localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
        var newMob = PrefabUtility.SaveAsPrefabAssetAndConnect(mobNs[0].mob.gameObject, localPath, InteractionMode.UserAction);

        var replaced = false;
        for (var i = 0; i < GetComponent<GameManager>().mobsType.Count; i++)
            if (GetComponent<GameManager>().mobsType[i].alimentation == alimentation)
            {
                GetComponent<GameManager>().mobsType[i] = newMob.GetComponent<Mob>();
                replaced = true;
            }
        if (!replaced)
            GetComponent<GameManager>().mobsType.Add(newMob.GetComponent<Mob>());
        
        // Cleaning
        foreach (var mobN in mobNs) DestroyImmediate(mobN.mob.gameObject);
        DestroyImmediate(parent);
        Log("Monster created");
    }

    private void Log(string message)
    {
        if (_verbose)
            Debug.Log(message);
    }
    
    private float Evaluate(Mob mob)
    {
        return ( Mathf.Pow(mob.force - force, 2f)
                 + Mathf.Pow(mob.vitesse - vitesse, 2f)
                 + Mathf.Pow(mob.cycleReproduction - cycleReproduction, 2f)
               ) / 3f;
    }

    private Mob CreateMob(GameObject parent, IReadOnlyList<Torso> torsos, IReadOnlyList<Head> heads, IReadOnlyList<Member> members)
    {
        var mob = Instantiate(defaultMob, parent.transform);
        mob.torso = Instantiate(torsos[Random.Range(0, torsos.Count)]);
        mob.torso.head = Instantiate(heads[Random.Range(0, heads.Count)]);
        for (var i = 0; i < mob.torso.nbMember; i++)
        {
            mob.torso.members.Add(Instantiate(members[Random.Range(0, members.Count)]));
        }
        mob.ComputeStats();
        return mob;
    }

    private static Mob Combine(GameObject parent, Mob mob1, Mob mob2)
    {
        // mob is initialized as a copy of mob1
        var mob = Instantiate(mob1, parent.transform);
        mob.torso = Instantiate(mob1.torso);
        mob.torso.head = Instantiate(mob2.torso.head);

        // Choose which mob2 members will be used
        var indexes2 = new SortedSet<int>();
        while (indexes2.Count < Math.Min(mob.torso.nbMember / 2, mob2.torso.nbMember))
        {
            indexes2.Add(Random.Range(0, mob2.torso.nbMember));
        }
        // Choose which mob members will be replaced to take after mob2
        var indexes1 = new SortedSet<int>();
        while (indexes1.Count < indexes2.Count)
        {
            indexes1.Add(Random.Range(0, mob.torso.nbMember));
        }
        
        // Replace mob members
        using (var index1 = indexes1.GetEnumerator())
        {
            using (var index2 = indexes2.GetEnumerator())
            {
                index1.MoveNext();
                index2.MoveNext();
                for (var i = 0; i < mob.torso.nbMember; ++i)
                {
                    if (index1.Current == i)
                    {
                        mob.torso.members[index1.Current] = Instantiate(mob2.torso.members[index2.Current]);
                        index1.MoveNext();
                        index2.MoveNext();
                    }
                    else
                        mob.torso.members[i] = Instantiate(mob1.torso.members[i]);
                }
            }
        }

        mob.ComputeStats();
        return mob;
    }
    
    private static void Mutate(Mob mob, Torso[] torsos, List<Head> heads, List<Member> members)
        {
            var iMutation = Random.Range(0, 2 + mob.torso.nbMember);
            switch (iMutation)
            {
                case 0: // Torso
                    var newTorso = Instantiate(torsos[Random.Range(0, torsos.Length)]);
                    newTorso.head = mob.torso.head;
                    if (newTorso.nbMember == mob.torso.nbMember)
                        newTorso.members = mob.torso.members;
                    else if (newTorso.nbMember > mob.torso.nbMember)
                    {
                        newTorso.members.AddRange(mob.torso.members);
                        while (newTorso.members.Count < newTorso.nbMember)
                        {
                            newTorso.members.Add(Instantiate(mob.torso.members[Random.Range(0, mob.torso.nbMember)]));
                        }
                    }
                    else
                    {
                        var indexes = new SortedSet<int>();
                        while (indexes.Count < newTorso.nbMember)
                            indexes.Add(Random.Range(0, mob.torso.nbMember));
                        for (var i = 0; i < mob.torso.nbMember; ++i)
                        {
                            if (indexes.Contains(i))
                                newTorso.members.Add(mob.torso.members[i]);
                            else 
                                DestroyImmediate(mob.torso.members[i]);
                        }
                    }
                    DestroyImmediate(mob.torso);
                    mob.torso = newTorso;
                    break;
                case 1: // Head
                    DestroyImmediate(mob.torso.head);
                    mob.torso.head = Instantiate(heads[Random.Range(0, heads.Count)]);
                    break;
                default: // Member
                    iMutation -= 2;
                    DestroyImmediate(mob.torso.members[iMutation]);
                    mob.torso.members[iMutation] = Instantiate(members[Random.Range(0, members.Count)]);
                    break;
            }
            mob.ComputeStats();
        }
}
