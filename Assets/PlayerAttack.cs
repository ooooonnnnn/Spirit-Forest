using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float cooldown; //time between attacks
    private float timer;
    [SerializeField] private float distance; //distance of raycast
    [SerializeField] private float duration; //how long the attack lasts when initiated

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryAttack();
        }

        timer -= Time.deltaTime;
        if (timer <= 0) timer = cooldown;
    }

    private void TryAttack()
    {
        if (timer <= 0) return;

        StartCoroutine(Attack());
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
                }
            }
            
            timer += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
    }
}
