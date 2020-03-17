using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decoration : MonoBehaviour
{
    public enum Type{grass,rock}
    public List<Cube.Type> acceptFloor;

    public Type type;
    public float weight;

    public bool Block()
    {
        return type == Type.rock;
    }
}
