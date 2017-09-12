using UnityEngine;
using System.Collections;

public class MouseClick : MonoBehaviour
{
    GameObject chosen;

    // Use this for initialization
    void Start()
    {
        chosen = transform.root.gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseDown()
    {
        this.BroadcastMessage("askForDestination");
    }
}
