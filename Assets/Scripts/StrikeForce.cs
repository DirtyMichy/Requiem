using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrikeForce : MonoBehaviour
{
    public int faction = 0; //0 = None, 1 = Faction 1, ...
    public GameObject currentLocation; //current sector of the strikeForce
    public List<GameObject> possibleDestiantions = new List<GameObject>();
    public List<GameObject> units = new List<GameObject>();
    private GameObject lastStrikeForce;
    public GameObject[] unitTypes;
    private bool isMoving = false;
    public AudioSource[] sounds;
    float iTweenScaleFactor = 0.5f;
    float iTweenMoveSpeed = 1f;
    public GameObject[,] unitAITypes;

    void setCurrentLocation(GameObject location)
    {
        currentLocation = location;
    }

    public void CalculateSize()
    {
        float size = TweenSize();

        iTween.ScaleTo(gameObject, iTween.Hash("x", size, "y", size, "easeType", "linear", "looptype", "none", "time", .5f));
    }

    private float TweenSize()
    {
        float size = Mathf.Clamp((0.5f + units.Count / 100f), 0.5f, 1.1f);
        return size;
    }

    public void Colorize()
    {
        gameObject.GetComponent<SpriteRenderer>().color = Manager.current.factionColors[faction];
    }

    void Awake()
    {
        //0 = Tank, 1 = Heli, 2 = Soldier
        unitAITypes = new GameObject[3, 2]
             {
            {unitTypes[0], unitTypes[0]},
            {unitTypes[2], unitTypes[2]},
            {unitTypes[1], unitTypes[1]}
             };
        sounds = GetComponents<AudioSource>();

        GameObject temp = (GameObject)Instantiate(unitTypes[Random.Range(0, unitTypes.Length)]);
        units.Add(temp);

        if (faction != 0)
            StartCoroutine("AIControls");
    }

    public IEnumerator AIControls()
    {
        yield return new WaitForSeconds(2f);
        while (true)
        {
            //Faction 0 controls the map and gets a lot of money, for balancing reasons units are 10 times more expensive
            if (faction == 0 && Manager.current.factionRessources[faction] >= 100)
            {
                Manager.current.factionRessources[faction] -= 100;

                GameObject temp = (GameObject)Instantiate(unitTypes[Random.Range(0, 3)]);

                units.Add(temp);
            }

            if (faction >= 2)
            {
                while (Manager.current.factionRessources[faction] >= 10)
                {
                    Manager.current.factionRessources[faction] -= 10;

                    //GameObject temp = (GameObject)Instantiate(unitTypes[Random.Range(0, unitTypes.Length)]);
                    GameObject temp = (GameObject)Instantiate(unitAITypes[faction - 2, Random.Range(0, 2)]);

                    units.Add(temp);
                }
                CalculateSize();
            }

            if (faction >= 2 && !isMoving)
                StartCoroutine("AIMovement");

            yield return new WaitForSeconds(Random.Range(2f, 4f));
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && (faction == 1))
        {
            GameObject temp = (GameObject)Instantiate(unitTypes[Random.Range(0, unitTypes.Length)]);
            units.Add(temp);

            Manager.current.CalculateUnits();
        }

        if (!isMoving && faction != 0 && currentLocation != null)
        {
            Vector3 pos = currentLocation.transform.position;
            pos.z = -1f;
            transform.position = pos;
        }
    }

    public void AIMovement()
    {
        if (currentLocation != null)
            possibleDestiantions = currentLocation.GetComponent<Region>().getNeighBours();

        if (!isMoving && gameObject.tag == "StrikeForce")
        {
            int destination = Random.Range(0, possibleDestiantions.Count - 1);

            //Faction means its a friendly AI controlled target
            if (units.Count > 0)
                if (possibleDestiantions[destination].GetComponent<Region>().currentStrikeForce.GetComponent<StrikeForce>().faction == faction)
                {
                    //If its a neighbour and this is not empty, then move and merge it
                    StartCoroutine(DeployAIStrikeForce(destination));
                }
                else
                {
                    StartCoroutine(AttackStrikeForce(gameObject, possibleDestiantions[destination].GetComponent<Region>().currentStrikeForce));
                }
        }
    }

    public IEnumerator DeployAIStrikeForce(int destination)
    {
        Debug.Log("Deploying AI");
        isMoving = true;

        Vector3 originalPosition = transform.position;

        //Merge strikeForces
        for (int i = 0; i < units.Count; i++)
        {
            possibleDestiantions[destination].GetComponent<Region>().currentStrikeForce.GetComponent<StrikeForce>().units.Add(units[i]);
        }
        units.Clear();

        Manager.current.CalculateUnits();

        iTween.MoveTo(gameObject, iTween.Hash("position", possibleDestiantions[destination].transform.position, "easeType", "linear", "looptype", "none", "time", iTweenMoveSpeed));

        yield return new WaitForSeconds(1f);

        transform.position = originalPosition;
        transform.localScale = new Vector3(0f, 0f, 0f);

        isMoving = false;

        //yield return new WaitForSeconds(1f);

        possibleDestiantions[destination].GetComponent<Region>().currentStrikeForce.GetComponent<StrikeForce>().CalculateSize();
        CalculateSize();
    }

    public void askForDestination()
    {
        if (!isMoving && gameObject.tag == "StrikeForce")
        {
            //only select the current strikeForce if its different to the last one, so the last and current one cant be the same
            if (gameObject != Manager.current.GetCurrentStrikeForce())
                Manager.current.SetCurrentStrikeForce(transform.root.gameObject);

            //Check if the selecting StrikeForce is an valid target for the last selection
            lastStrikeForce = Manager.current.GetLastSelectedStrikeForce();

            lastStrikeForce.GetComponent<StrikeForce>().Colorize();
            //lastStrikeForce.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
            Colorize();
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);

            //only deploy if its a neighbour
            bool isNeighbour = false;

            if (lastStrikeForce.GetComponent<StrikeForce>())
                for (int i = 0; i < lastStrikeForce.GetComponent<StrikeForce>().possibleDestiantions.Count; i++)
                {
                    if (transform.root.gameObject == lastStrikeForce.GetComponent<StrikeForce>().possibleDestiantions[i].GetComponent<Region>().currentStrikeForce)
                        isNeighbour = true;
                }

            if (!isNeighbour)
                Manager.current.CalculateUnits();

            Manager.current.CalculateButtons();

            GameObject[] removeMarker = GameObject.FindGameObjectsWithTag("Marker");

            for (int i = 0; i < removeMarker.Length; i++)
            {
                Destroy(removeMarker[i]);
            }

            //Faction 1 means its a player controlled target
            if (faction == 1)
            {
                currentLocation.BroadcastMessage("ShowNeighbours");

                possibleDestiantions = currentLocation.GetComponent<Region>().getNeighBours();

                //If last StrikeForce is a neighbour and not empty, then move and merge it to the current force
                if (isNeighbour && lastStrikeForce.GetComponent<StrikeForce>().units.Count > 0 && lastStrikeForce.GetComponent<StrikeForce>().faction == 1)
                {
                    if (!isMoving)
                        StartCoroutine("DeployStrikeForce");
                }
            }

            //If the faction isn´t 1, the target is an enemy and will be attacked
            if (faction != 1 && lastStrikeForce.GetComponent<StrikeForce>().faction == 1)
            {
                //If last StrikeForce (the attacker) is a neighbour of the target, then attack the target
                if (isNeighbour && lastStrikeForce.GetComponent<StrikeForce>().units.Count > 0)
                {
                    if (!isMoving)
                        StartCoroutine(AttackStrikeForce(lastStrikeForce.gameObject, gameObject));
                }
            }
        }
    }

    public IEnumerator DeployStrikeForce()
    {
        Debug.Log("Deploy to friendly zone");

        isMoving = true;

        Vector3 originalPosition = lastStrikeForce.transform.position;

        iTween.MoveTo(lastStrikeForce, iTween.Hash("position", gameObject.transform.position, "easeType", "linear", "looptype", "none", "time", iTweenMoveSpeed));

        sounds[2].Play();

        yield return new WaitForSeconds(1f);

        //Merge strikeForces
        for (int i = 0; i < lastStrikeForce.GetComponent<StrikeForce>().units.Count; i++)
        {
            units.Add(lastStrikeForce.GetComponent<StrikeForce>().units[i]);
        }
        lastStrikeForce.GetComponent<StrikeForce>().units.Clear();

        lastStrikeForce.transform.position = originalPosition;
        lastStrikeForce.transform.localScale = new Vector3(0f, 0f, 0f);

        Manager.current.CalculateUnits();

        isMoving = false;

        lastStrikeForce.GetComponent<StrikeForce>().Colorize();

        gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);

        lastStrikeForce.GetComponent<StrikeForce>().CalculateSize();

        CalculateSize();
    }

    private float CalculateDamage(GameObject unitA, GameObject unitB)
    {
        float damage = unitA.GetComponent<Unit>().dmg;

        int damageModifier = 4;

        if (unitA.GetComponent<Unit>().dmgType == "TankCannon")
        {
            if (unitB.GetComponent<Unit>().armorType == "HelicopterArmor")
                damage /= damageModifier;
            if (unitB.GetComponent<Unit>().armorType == "SoldierArmor")
                damage *= damageModifier;
        }
        if (unitA.GetComponent<Unit>().dmgType == "HelicopterCannon")
        {
            if (unitB.GetComponent<Unit>().armorType == "TankArmor")
                damage *= damageModifier;
            if (unitB.GetComponent<Unit>().armorType == "SoldierArmor")
                damage /= damageModifier;
        }
        if (unitA.GetComponent<Unit>().dmgType == "SoldierCannon")
        {
            if (unitB.GetComponent<Unit>().armorType == "HelicopterArmor")
                damage *= damageModifier;
            if (unitB.GetComponent<Unit>().armorType == "TankArmor")
                damage /= damageModifier;
        }

        return damage;
    }

    public IEnumerator AttackStrikeForce(GameObject attacker, GameObject defender)
    {
        isMoving = true;

        Vector3 originalPosition = attacker.transform.position;

        iTween.MoveTo(attacker, iTween.Hash("position", defender.transform.position, "easeType", "linear", "looptype", "none", "time", iTweenMoveSpeed));

        if (attacker.GetComponent<StrikeForce>().faction == 1)
            sounds[2].Play();

        int a = 0, b = 0;

        //Attack strikeForces until one get out of units
        if (defender.GetComponent<StrikeForce>().units.Count > 0)
            for (int i = 0; (i < attacker.GetComponent<StrikeForce>().units.Count) || (i < defender.GetComponent<StrikeForce>().units.Count); i++)
            {
                //two units attack eachother until one dies
                if (attacker.GetComponent<StrikeForce>().units[a] != null && defender.GetComponent<StrikeForce>().units[b] != null)
                    while (attacker.GetComponent<StrikeForce>().units[a].GetComponent<Unit>().hitPoints > 0 && defender.GetComponent<StrikeForce>().units[b].GetComponent<Unit>().hitPoints > 0)
                    {
                        //################################################################################################################################Tank=Stein,Heli=Papier,Sol=Schere
                        float damageAtt = CalculateDamage(attacker.GetComponent<StrikeForce>().units[a], defender.GetComponent<StrikeForce>().units[b]);
                        float damageDef = CalculateDamage(defender.GetComponent<StrikeForce>().units[b], attacker.GetComponent<StrikeForce>().units[a]);

                        attacker.GetComponent<StrikeForce>().units[a].GetComponent<Unit>().hitPoints -= damageDef;
                        defender.GetComponent<StrikeForce>().units[b].GetComponent<Unit>().hitPoints -= damageAtt;

                        if (attacker.GetComponent<StrikeForce>().units[a].GetComponent<Unit>().hitPoints <= 0 && a < attacker.GetComponent<StrikeForce>().units.Count - 1)
                            a++;
                        if (defender.GetComponent<StrikeForce>().units[b].GetComponent<Unit>().hitPoints <= 0 && b < defender.GetComponent<StrikeForce>().units.Count - 1)
                            b++;
                    }
            }

        //clear all dead units from the list
        for (int i = 0; i < attacker.GetComponent<StrikeForce>().units.Count; i++)
        {
            if (attacker.GetComponent<StrikeForce>().units[i] != null)
                if (attacker.GetComponent<StrikeForce>().units[i].GetComponent<Unit>().hitPoints <= 0)
                {
                    GameObject temp = attacker.GetComponent<StrikeForce>().units[i];
                    attacker.GetComponent<StrikeForce>().units.RemoveAt(i);
                    i--;
                    Destroy(temp);
                }
        }
        if (defender.GetComponent<StrikeForce>().units.Count > 0)
            for (int i = 0; i < defender.GetComponent<StrikeForce>().units.Count; i++)
            {
                if (defender.GetComponent<StrikeForce>().units[i] != null)
                    if (defender.GetComponent<StrikeForce>().units[i].GetComponent<Unit>().hitPoints <= 0)
                    {
                        GameObject temp = defender.GetComponent<StrikeForce>().units[i];
                        defender.GetComponent<StrikeForce>().units.RemoveAt(i);
                        i--;
                        Destroy(temp);
                    }
            }

        //Player/Attacker wins and the strikeforce gets calculated
        if (defender.GetComponent<StrikeForce>().units.Count == 0)
        {
            if (attacker.GetComponent<StrikeForce>().units.Count > 0)
            {
                for (int i = 0; i < attacker.GetComponent<StrikeForce>().units.Count; i++)
                {
                    defender.GetComponent<StrikeForce>().units.Add(attacker.GetComponent<StrikeForce>().units[i]);
                }
                attacker.GetComponent<StrikeForce>().units.Clear();

                Debug.Log(defender + " has lost");
                defender.GetComponent<StrikeForce>().faction = attacker.GetComponent<StrikeForce>().faction;

                defender.GetComponent<StrikeForce>().currentLocation.GetComponent<Region>().owner = attacker.GetComponent<StrikeForce>().faction;

                yield return new WaitForSeconds(1f);

                if (attacker.GetComponent<StrikeForce>().faction == 1)
                    sounds[3].Play();

                defender.GetComponent<StrikeForce>().GetComponent<SpriteRenderer>().sprite = attacker.GetComponent<SpriteRenderer>().sprite;

                defender.GetComponent<StrikeForce>().currentLocation.GetComponent<Region>().Colorize();
                attacker.GetComponent<StrikeForce>().currentLocation.GetComponent<Region>().Colorize();

                defender.GetComponent<StrikeForce>().Colorize();
                attacker.GetComponent<StrikeForce>().Colorize();

                if (faction == 1)
                    gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
            }
        }
        else
        {
            yield return new WaitForSeconds(1f);

            if (attacker.GetComponent<StrikeForce>().faction == 1)
                sounds[4].Play();
        }

        attacker.transform.position = originalPosition;
        attacker.transform.localScale = new Vector3(0f, 0f, 0f);

        isMoving = false;

        if (faction == 1)
            askForDestination();

        Manager.current.CalculateUnits();
        Manager.current.CheckForWinner();

        defender.GetComponent<StrikeForce>().CalculateSize();
        attacker.GetComponent<StrikeForce>().CalculateSize();
    }
}