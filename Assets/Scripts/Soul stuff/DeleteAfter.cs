using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteAfter : MonoBehaviour
{
    private void Start()
    {
        
    }


    public void AboutToDie()
    {
        StartCoroutine(Something());
        transform.parent = null;
        GetComponent<ParticleSystem>().emissionRate = 0;
        transform.localScale = new Vector3(1, 1, 1);


    }

    IEnumerator Something()
    {

        yield return new WaitForSeconds(3.0f);
        Destroy(this.gameObject);
    }
}
