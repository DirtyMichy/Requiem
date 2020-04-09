using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridAnimation : MonoBehaviour {

    private float amount = 0.2f;
    public GameObject Grid;

    // Update is called once per frame
    void Update () {
        float x = Mathf.Repeat(Time.time * .25f, 1);
        Vector2 offset = new Vector2(-x, Grid.GetComponent<Renderer>().sharedMaterial.GetTextureOffset("_MainTex").y);
        Grid.GetComponent<Renderer>().sharedMaterial.SetTextureOffset("_MainTex", offset);
    }
}
