using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInstantiator : MonoBehaviour
{
    //instantiates gameobjects from a queue, with a limit on how many can be created per frame
    public Queue<InstantiationData> goQueue = new ();
    [SerializeField] private int maxInstantiationsPerFrame;
    
    void Update()
    {
        int numObjs = 0;
        InstantiationData nextInstantiation = new InstantiationData();
        while (numObjs <= maxInstantiationsPerFrame)
        {
            if (goQueue.Count == 0) return;
            numObjs++;
            nextInstantiation = goQueue.Dequeue();
            Instantiate(nextInstantiation.go, nextInstantiation.pos, nextInstantiation.quaternion);
        }
    }
}