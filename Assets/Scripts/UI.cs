using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    public GameObject tutorial;

    // Use this for initialization
    void Start()
    {
        sounds = GetComponents<AudioSource>();
    }

    public AudioSource[] sounds;

    public void BuildTank()
    {
        Manager.current.AddTank();
    }
    public void BuildHeli()
    {
        Manager.current.AddHeli();
    }
    public void BuildSoldier()
    {
        Manager.current.AddSoldier();
    }

    public void StartGame()
    {
        sounds[1].Play();

        SceneManager.LoadScene("Requiem");
    }

    public void EndGame()
    {
        sounds[1].Play();

        Application.Quit();
    }

    public void ShowTutorial()
    {
        sounds[1].Play();

        if (tutorial.activeSelf)
        {
            tutorial.SetActive(false);
        }
        else
        {
            tutorial.SetActive(true);
        }
    }
}
