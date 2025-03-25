using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float cooldown; //time between attacks
    private float timer;
    [SerializeField] private float distance; //distance of raycast
    [SerializeField] private float duration; //how long the attack lasts when initiated

    [Header("UI")] 
    [SerializeField] private Color colorEmpty;
    [SerializeField] private Color colorFull;
    [SerializeField] private Image cooldownImage;

    [Header("Animation")] 
    [SerializeField] private Animator animator;

    [Header("Score")] 
    [SerializeField] private int soulsPerKill;
    [SerializeField] private ScoreManager manager;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryAttack();
        }

        timer -= Time.deltaTime;
        if (timer < 0) timer = 0;
        
        //update graphic
        if (timer <= 0)
        {
            cooldownImage.color = colorFull;
            cooldownImage.fillAmount = 1;
        }
        else
        {
            cooldownImage.color = colorEmpty;
            cooldownImage.fillAmount = (cooldown - timer) / cooldown;
        }

    }

    private void TryAttack()
    {
        if (timer > 0) return;

        StartCoroutine(Attack());
        timer = cooldown;
        animator.SetTrigger("Attack");
        animator.SetLayerWeight(1,1);
    }

    IEnumerator Attack()
    {
        float timer = 0;
        RaycastHit hit;
        while (timer <= duration)
        {
            if (Physics.Raycast(transform.position, transform.forward, out hit, distance))
            {
                GameObject other = hit.collider.gameObject;
                if (other.CompareTag("Enemy"))
                {
                    Destroy(other);
                    manager.AddScore(soulsPerKill);
                }
            }
            
            timer += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
    }

    public void ResetAnimatorState()
    {
        animator.SetLayerWeight(1,0);
    }
}
