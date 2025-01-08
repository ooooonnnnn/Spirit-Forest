using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    private Animation animation;
    [SerializeField] private AnimationClip spawnClip, idleClip;

    public void Start()
    {
        animation = GetComponent<Animation>();
        
        //play spawn animation and then idle animation
        animation.Play(spawnClip.name);
    }

    public void UpdateAnimClip()
    {
        //this is called from an event in spawnClip
        //when idleClip is played, change to loop mode
        animation.wrapMode = WrapMode.Loop;
        animation.Play(idleClip.name);
    }
}
