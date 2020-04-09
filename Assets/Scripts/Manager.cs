using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public static Manager current;
    public GameObject currentSelectedStrikeForce;
    public GameObject lastSelectedStrikeForce;
    public GameObject[] startingStrikeForces;
    public GameObject defaultStrikeForce;

    public Button[] buttons;
    public GameObject uiArrows;

    public GameObject tank;
    public GameObject soldier;
    public GameObject helicopter;

    public GameObject sectorNameText;
    public GameObject tankCountText;
    public GameObject helicopterCountText;
    public GameObject soldierCountText;
    public GameObject sectorOwnerText;
    public GameObject playerRessourcesText;
    public GameObject ESCMenu;
    public GameObject winText;
    public GameObject loseText;

    public GameObject mainCam;

    private int PLAYERFACTION = 1;
    private int FACTIONCOUNT = 5;

    private GameObject[] sectors;

    private bool end = false;

    public int[] factionRessources;

    public AudioSource[] sounds;

    public Color[] factionColors;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            DeselectStrikeForce();
        }

        if (Input.GetKey(KeyCode.Alpha2) && factionRessources[PLAYERFACTION] >= 10)
        {
            AddHeli();
        }

        if (Input.GetKey(KeyCode.Alpha3) && factionRessources[PLAYERFACTION] >= 10)
        {
            AddTank();
        }

        if (Input.GetKey(KeyCode.Alpha1) && factionRessources[PLAYERFACTION] >= 10)
        {
            AddSoldier();
        }

        if (Input.GetKeyDown(KeyCode.F10))
        {
            factionRessources[PLAYERFACTION] += 1337;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowESCMenu();
        }


    }

    public void CheckForWinner()
    {
        GameObject[] countStrikeForces = GameObject.FindGameObjectsWithTag("StrikeForce");
        int ownedSectors = 0;

        for (int j = 0; j < countStrikeForces.Length; j++)
        {
            if (countStrikeForces[j].GetComponent<StrikeForce>().faction == 1)
                ownedSectors++;
        }

        //Lose   
        if (ownedSectors == 0 && !end)
        {
            sounds[3].Play();

            end = true;

            loseText.SetActive(true);

            StartCoroutine("Ending");
        }

        //Win
        if ((ownedSectors == sectors.Length && !end))
        {
            sounds[2].Play();

            end = true;

            winText.SetActive(true);

            StartCoroutine("Ending");
        }
    }

    public IEnumerator Ending()
    {
        sounds[0].Stop();
        yield return new WaitForSeconds(20f);
        SceneManager.LoadScene("Requiem");
    }

    public void CalculateUnits()
    {
        if (currentSelectedStrikeForce.GetComponent<StrikeForce>().faction != 666)
        {
            sectorOwnerText.GetComponent<Text>().text = "Sektorinfo";
            tankCountText.GetComponent<Text>().text = "Panzer    : " + countUnitsInList(tank);
            soldierCountText.GetComponent<Text>().text = "Soldaten  : " + countUnitsInList(soldier);
            helicopterCountText.GetComponent<Text>().text = "Helikopter: " + countUnitsInList(helicopter);

            if (currentSelectedStrikeForce.GetComponent<StrikeForce>().faction == 1)
            {
                uiArrows.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                for (int i = 0; i < buttons.Length; i++)
                {
                    buttons[i].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                }
            }

        }
        else
        {
            sectorOwnerText.GetComponent<Text>().text = "";
            tankCountText.GetComponent<Text>().text = "";
            soldierCountText.GetComponent<Text>().text = "";
            helicopterCountText.GetComponent<Text>().text = "";

            uiArrows.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
            }
        }

        playerRessourcesText.GetComponent<Text>().text = factionRessources[PLAYERFACTION] + " $";
    }

    public void AddTank()
    {
        GameObject temp = (GameObject)Instantiate(tank);
        currentSelectedStrikeForce.GetComponent<StrikeForce>().units.Add(temp);
        currentSelectedStrikeForce.GetComponent<StrikeForce>().CalculateSize();

        factionRessources[PLAYERFACTION] -= 10;

        CalculateUnits();
        CalculateButtons();
        sounds[1].Play();
    }

    public void AddHeli()
    {
        GameObject temp = (GameObject)Instantiate(helicopter);
        currentSelectedStrikeForce.GetComponent<StrikeForce>().units.Add(temp);
        currentSelectedStrikeForce.GetComponent<StrikeForce>().CalculateSize();

        factionRessources[PLAYERFACTION] -= 10;

        CalculateUnits();
        CalculateButtons();
        sounds[1].Play();
    }

    public void AddSoldier()
    {
        GameObject temp = (GameObject)Instantiate(soldier);
        currentSelectedStrikeForce.GetComponent<StrikeForce>().units.Add(temp);
        currentSelectedStrikeForce.GetComponent<StrikeForce>().CalculateSize();

        factionRessources[PLAYERFACTION] -= 10;

        CalculateUnits();
        CalculateButtons();
        sounds[1].Play();
    }

    public void CalculateButtons()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (factionRessources[PLAYERFACTION] >= 10 && currentSelectedStrikeForce.GetComponent<StrikeForce>().faction == 1)
                buttons[i].interactable = true;
            else
                buttons[i].interactable = false;
        }
    }

    public IEnumerator Ressources()
    {
        while (true)
        {
            sectors = GameObject.FindGameObjectsWithTag("Sector");

            for (int i = 0; i < sectors.Length; i++)
            {
                if (sectors[i].GetComponent<Region>().owner != PLAYERFACTION)
                    factionRessources[sectors[i].GetComponent<Region>().owner] += 2; //AI is cheating :o
                else
                    factionRessources[PLAYERFACTION]++;
            }

            CalculateButtons();
            CalculateUnits();

            yield return new WaitForSeconds(1f);
        }
    }

    //Counting specific units
    int countUnitsInList(GameObject searched)
    {
        int amount = 0;
        if (currentSelectedStrikeForce.GetComponent<StrikeForce>().units.Count > 0)
            for (int i = 0; i < currentSelectedStrikeForce.GetComponent<StrikeForce>().units.Count; i++)
            {
                if (currentSelectedStrikeForce.GetComponent<StrikeForce>().units[i] != null)
                    if (currentSelectedStrikeForce.GetComponent<StrikeForce>().units[i].GetComponent<Unit>().unitName == searched.GetComponent<Unit>().unitName)
                        amount++;
            }
        return amount;
    }

    // Use this for initialization
    void Start()
    {
        factionRessources = new int[FACTIONCOUNT];

        for (int i = 0; i < factionRessources.Length; i++)
        {
            factionRessources[i] = 10;
        }

        sounds = GetComponents<AudioSource>();

        //Ensure that there is only one manager
        if (current == null)
            current = this;
        else
            Destroy(gameObject);

        sectors = GameObject.FindGameObjectsWithTag("Sector");

        //Mix up the order of the sectors, for random starting positions
        for (int i = 0; i < sectors.Length; i++)
        {
            GameObject tempSector = sectors[i];
            int rng = Random.Range(0, sectors.Length);
            sectors[i] = sectors[rng];
            sectors[rng] = tempSector;
        }

        //spawn all factions except neutral/blue
        for (int i = 0; i < startingStrikeForces.Length; i++)
        {
            sectors[i].GetComponent<Region>().currentStrikeForce = startingStrikeForces[i];
            sectors[i].GetComponent<Region>().owner = sectors[i].GetComponent<Region>().currentStrikeForce.GetComponent<StrikeForce>().faction;
        }

        for (int i = 0; i < startingStrikeForces.Length; i++)
        {
            startingStrikeForces[i].GetComponent<StrikeForce>().currentLocation = sectors[i];
            if (startingStrikeForces[i].GetComponent<StrikeForce>().faction == 1)
            {
                Vector3 pos = startingStrikeForces[i].GetComponent<StrikeForce>().currentLocation.transform.position;
                pos.z = -10f;
                mainCam.transform.position = pos;
            }
        }

        for (int i = 0; i < sectors.Length; i++)
        {
            sectors[i].name = "Sektor: " + i.ToString();

            if (sectors[i].GetComponent<Region>().owner == 0)
                sectors[i].GetComponent<Region>().SpawnStrikeForce();

            sectors[i].GetComponent<Region>().Colorize();
        }

        StartCoroutine("Ressources");

        currentSelectedStrikeForce.gameObject.GetComponent<StrikeForce>().askForDestination();

        StartCoroutine("LateStart");
    }

    IEnumerator LateStart()
    {
        yield return new WaitForSeconds(.1f);
        currentSelectedStrikeForce.gameObject.GetComponent<StrikeForce>().currentLocation.BroadcastMessage("ShowNeighbours");
    }

    public void DeselectStrikeForce()
    {
        if (ESCMenu.activeSelf)
        {
            sounds[4].Play();
            ESCMenu.SetActive(false);
        }

        GameObject[] removeMarker = GameObject.FindGameObjectsWithTag("Marker");

        for (int i = 0; i < removeMarker.Length; i++)
        {
            Destroy(removeMarker[i]);
        }

        //currentSelectedStrikeForce.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        currentSelectedStrikeForce.gameObject.GetComponent<StrikeForce>().Colorize();
        currentSelectedStrikeForce = defaultStrikeForce;
        CalculateButtons();
        CalculateUnits();
    }

    public void SetCurrentStrikeForce(GameObject sf)
    {
        lastSelectedStrikeForce = currentSelectedStrikeForce;
        currentSelectedStrikeForce = sf;
    }

    public void SetLastStrikeForce(GameObject sf)
    {
        lastSelectedStrikeForce = sf;
    }

    public GameObject GetCurrentStrikeForce()
    {
        return currentSelectedStrikeForce;
    }

    public GameObject GetLastSelectedStrikeForce()
    {
        return lastSelectedStrikeForce;
    }

    public void RestartScene()
    {
        sounds[4].Play();

        SceneManager.LoadScene("Requiem");
    }

    public void ShowESCMenu()
    {
        sounds[4].Play();

        if (ESCMenu.activeSelf)
        {
            ESCMenu.SetActive(false);
        }
        else
        {
            ESCMenu.SetActive(true);
        }
    }

    public void HideESCMenu()
    {
        sounds[4].Play();
        ESCMenu.SetActive(false);
    }

    public void EndGame()
    {
        sounds[4].Play();
        Application.Quit();
    }
}