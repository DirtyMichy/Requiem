using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyMarker : MonoBehaviour
{    
	public void askForDestination()
    {
        GameObject[] removeMarker = GameObject.FindGameObjectsWithTag("Marker");

        for (int i = 0; i < removeMarker.Length; i++)
        {
            Destroy(removeMarker[i]);
        }

        Manager.current.DeselectStrikeForce();
    }
}