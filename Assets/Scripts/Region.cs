using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Region : MonoBehaviour
{
    public int owner;
    public List<GameObject> neighBours = new List<GameObject>();
    public GameObject currentStrikeForce; //every sector has a strikeforce
    public GameObject marker;

    private void Start()
    {
        transform.parent = null;
    }

    public void SpawnStrikeForce()
    {
        Vector3 spawnPos = transform.position;
        spawnPos.z = -1;
        currentStrikeForce = (GameObject)Instantiate(currentStrikeForce, spawnPos, currentStrikeForce.transform.rotation);
        currentStrikeForce.name = "SF" + Random.Range(1000,10000);
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
        gameObject.GetComponent<SpriteRenderer>().color = Manager.current.faction[owner].factionColor;
    }
}