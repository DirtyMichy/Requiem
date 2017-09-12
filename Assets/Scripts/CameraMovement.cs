using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private float amount = 0.2f;
    public GameObject Grid;

	// Update is called once per frame
	void Update ()
    {
        //Reset Camera
        if (Input.GetKey(KeyCode.Space))
        {
            transform.position = new Vector3(0f,0f,-10f);
            GetComponent<Camera>().orthographicSize = 5.4f;
        }

        //Zoom
        if (Input.GetKey(KeyCode.Q) && GetComponent<Camera>().orthographicSize > 5.4f)
        {
            GetComponent<Camera>().orthographicSize -= 0.2f;
        }
        if (Input.GetKey(KeyCode.E) && GetComponent<Camera>().orthographicSize < 30f)
        {
            GetComponent<Camera>().orthographicSize += 0.2f;
        }

        //Movement
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A) && transform.position.x > -14f)
        {
            Vector3 pos = transform.position;
            pos.x -= amount;
            transform.position = pos;

            float x = Mathf.Repeat(Time.time * .25f, 1);
            Vector2 offset = new Vector2(-x, Grid.GetComponent<Renderer>().sharedMaterial.GetTextureOffset("_MainTex").y);
            Grid.GetComponent<Renderer>().sharedMaterial.SetTextureOffset("_MainTex", offset);
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)  && transform.position.x < 14f)
        {
            Vector3 pos = transform.position;
            pos.x += amount;
            transform.position = pos;

            float x = Mathf.Repeat(Time.time * .25f, 1);
            Vector2 offset = new Vector2(x, Grid.GetComponent<Renderer>().sharedMaterial.GetTextureOffset("_MainTex").y);
            Grid.GetComponent<Renderer>().sharedMaterial.SetTextureOffset("_MainTex", offset);
        }

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W) && transform.position.y < 10f)
        {
            Vector3 pos = transform.position;
            pos.y += amount;
            transform.position = pos;

            float y = Mathf.Repeat(Time.time * .5f, 1);
            Vector2 offset = new Vector2(Grid.GetComponent<Renderer>().sharedMaterial.GetTextureOffset("_MainTex").x, y);
            Grid.GetComponent<Renderer>().sharedMaterial.SetTextureOffset("_MainTex", offset);
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S) && transform.position.y > -10f)
        {
            Vector3 pos = transform.position;
            pos.y -= amount;
            transform.position = pos;

            float y = Mathf.Repeat(Time.time * .5f, 1);
            Vector2 offset = new Vector2(Grid.GetComponent<Renderer>().sharedMaterial.GetTextureOffset("_MainTex").x, -y);
            Grid.GetComponent<Renderer>().sharedMaterial.SetTextureOffset("_MainTex", offset);
        }
    }
}
