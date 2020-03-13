using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public enum Type  { none = 0,dirt, grass, watter }

    public float weight = 0;
    public Vector2 position;
    public Type type;
    public GameManager manager;
    public Decoration onSurface;
    // Start is called before the first frame update
    
    void Start()
    {
        manager = FindObjectOfType<GameManager>();
    }

    public void AddDecoration()
    {
        
        manager = FindObjectOfType<GameManager>();
        
        float rand = Random.value;
       
        if (Random.value > 0.95)
        {
            int index = -1;
        
            List<Decoration> decorations = manager.map.decorationType;
            float totalWeight = TypeWeight(decorations);
            while (rand >= 0 && index < decorations.Count)
            {
                ++index;
                rand -= decorations[index].weight / totalWeight;

            }
            
            Decoration decorType = manager.map.decorationType[index];
            int i = 0;
            while (i < decorType.acceptFloor.Count && type != decorType.acceptFloor[i]) ++i;
            if(i<decorType.acceptFloor.Count) onSurface = Instantiate(decorType, new Vector3(position.x,1,position.y),Quaternion.identity,transform);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPosition(int x, int y)
    {
        position = new Vector2(x,y);
    }
    float TypeWeight(List<Decoration> decorations)
    {
        float weight = 0;
        for (int i = 0; i < decorations.Count; ++i) weight += decorations[i].weight;
        return weight;
    }
}
