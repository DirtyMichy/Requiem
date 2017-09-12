using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulse : MonoBehaviour
{

    public void Awake()
    {

        StartCoroutine("PulseLoop");

    }

    public IEnumerator PulseLoop()
    {
        while (true)
        {
            iTween.ScaleBy(gameObject, iTween.Hash("amount", new Vector3(2f, 2f, 0f), "easeType", "easeInOutExpo", "looptype", "pingpong", "time", 1f));
            iTween.RotateBy(gameObject, iTween.Hash("amount", new Vector3(0f, 0f, 1f), "easeType", "easeInOutExpo", "looptype", "linear", "time", 10f));
            yield return new WaitForSeconds(1f);
        }
    }
}