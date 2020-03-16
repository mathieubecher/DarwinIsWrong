using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Mob))]
public class MobEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        var mob = (Mob) target;
        base.OnInspectorGUI();

        GUILayout.Label("Head : " + (mob.torso == null || mob.torso.head == null ? ""
                            : mob.torso.head.name.Remove(mob.torso.head.name.IndexOf('('))));
        GUILayout.Label("Torso : " + (mob.torso == null ? ""
                            : mob.torso.name.Remove(mob.torso.name.IndexOf('('))));
        var members = "Members :";
        if (mob.torso != null && mob.torso.members != null)
            members = mob.torso.members.Aggregate(members, (current, member) => current + (" " + member.name.Remove(member.name.IndexOf('('))));
        GUILayout.Label(members);
    }
}
