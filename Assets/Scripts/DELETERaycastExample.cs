using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DELETERaycastExample : MonoBehaviour
{

    float timerino = 0f;
    int lastHit = -10;
    // Update is called once per frame
    void Start()
    {

        StartCoroutine("aaa");

    }

    IEnumerator aaa()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.00001f);
            timerino += Time.deltaTime;
            RaycastHit hit;
            Ray r = new Ray(transform.position, -transform.up);
            Debug.DrawRay(r.origin, r.direction * 0.5f, Color.blue);
            if (Physics.Raycast(r, out hit))
            {
                if (hit.collider.GetInstanceID() != lastHit)
                {
                    Debug.Log(" TOOK " + timerino);
                    timerino = 0;
                    lastHit = hit.collider.GetInstanceID();
                    //Time.timeScale = 0;
                    //yield return new WaitForSecondsRealtime(2);
                    //Time.timeScale = 1;

                }
            }

        }
    }
}
