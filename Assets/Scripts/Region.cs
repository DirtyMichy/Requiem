using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Region : MonoBehaviour
{
    public int owner;
    public List<GameObject> neighBours = new List<GameObject>();
    public GameObject currentStrikeForce; //every sector has a strikeforce
    public GameObject marker;

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

    public void Colorize()
    {
        //First implementation of colors for testing
        gameObject.GetComponent<SpriteRenderer>().color = Manager.current.factionColors[owner];
    }
}