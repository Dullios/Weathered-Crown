using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    private AudioSource audioSource
    {
        get
        {
            return GetComponent<AudioSource>();
        }
    }

    public AudioClip hubSong;
    public AudioClip levelSong;
    
	// Use this for initialization
	void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
    {
        
	}

    public void OnLevelWasLoaded(int levelInt)
    {
        if (Application.loadedLevelName == "Title Screen" || Application.loadedLevelName == "Hub" || Application.loadedLevelName == "End Screen")
        {
            audioSource.clip = hubSong;
            playClip();
        }
        else
        {
            audioSource.clip = levelSong;
            playClip();
        }
    }

    private void playClip()
    {
        audioSource.Play();
    }
}
