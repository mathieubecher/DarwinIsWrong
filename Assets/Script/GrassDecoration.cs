using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassDecoration : Decoration
{
    private float minTimerReproduce = 50;
    private float maxTimerReproduce = 100;
    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        timer = Random.value * (maxTimerReproduce-minTimerReproduce) + minTimerReproduce;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            transform.parent.GetComponent<Cube>().SpawnGrassNeighbour();
            timer = Random.value * (maxTimerReproduce-minTimerReproduce) + minTimerReproduce;
        }
    }
}
