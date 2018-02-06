using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    // Use this for initialization
    public AudioClip[] sounds;
    AudioSource aSource;
    void Start () {
        aSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void EndGame()
    {
        aSource.Stop();
        aSource.clip = sounds[1];
        aSource.Play();
        aSource.loop = false;
    }
}
