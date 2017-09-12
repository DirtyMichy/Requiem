using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Region : MonoBehaviour
{
    public int owner;

    public List<GameObject> neighBours = new List<GameObject>();

    public GameObject currentStrikeForce; //every sector has a strikeforce
    public GameObject marker;

    // Use this for initialization
    public void SpawnStrikeForce()
    {
        //Spawn terrorists

            Vector3 spawnPos = transform.position;
            spawnPos.z = -1;
            currentStrikeForce = (GameObject)Instantiate(currentStrikeForce, spawnPos, currentStrikeForce.transform.rotation);
            currentStrikeForce.SendMessage("setCurrentLocation", (gameObject));
        
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Sector")
            neighBours.Add(collider.transform.root.gameObject);
    }

    public void ShowNeighbours()
    {
        for (int i = 0; i < neighBours.Count; i++)
        {
            Instantiate(marker, neighBours[i].transform.position, neighBours[i].transform.rotation);
        }
    }

    public List<GameObject> getNeighBours()
    {
        return neighBours;
    }

    // Update is called once per frame
    public void Colorize()
    {
        //First implementation of colors for testing
        if (owner == 0)
            gameObject.GetComponent<SpriteRenderer>().color = new Color(.0f, .0f, .5f, 1f);
        if (owner == 1)
            gameObject.GetComponent<SpriteRenderer>().color = new Color(.5f, 0f, .5f, 1f);
        if (owner == 2)
            gameObject.GetComponent<SpriteRenderer>().color = new Color(.5f, 0f, 0f, 1f);
        if (owner == 3)
            gameObject.GetComponent<SpriteRenderer>().color = new Color(0f, .5f, .5f, 1f);
        if (owner == 4)
            gameObject.GetComponent<SpriteRenderer>().color = new Color(.5f, .5f, 0f, 1f);
    }
}
