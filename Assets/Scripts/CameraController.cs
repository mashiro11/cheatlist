using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    // Use this for initialization
    public AudioClip[] sounds;
    AudioSource aSource;
    private bool busted = false;
    const int STAGE_THEME = 0;
    const int BUSTED_THEME = 1;
    const int WIN_THEME = 2;

    void Start () {
        aSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		if(busted && !aSource.isPlaying)
        {
            busted = false;
            PlayMusic(STAGE_THEME, true);
        }
    }

    public void Busted()
    {
        PlayMusic(BUSTED_THEME, false);
        busted = true;
    }

    public void WinGame()
    {
        PlayMusic(WIN_THEME, false);
    }

    private void PlayMusic(int music, bool loop)
    {
        aSource.Stop();
        aSource.clip = sounds[music];
        aSource.Play();
        aSource.loop = loop;
    }
}
