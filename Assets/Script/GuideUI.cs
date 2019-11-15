using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideUI : MonoBehaviour
{
    // Container UI
    public GameObject menuContainer;
    public GameObject subMenuContainer;
    public GameObject gameContainer;

    public AudioSource soundGuide;
    public AudioClip quitAudio;
    public AudioClip leaderBoardAudio;
    public AudioClip settingAudio;
    public AudioClip helpAudio;
    public AudioClip playAudio;
    public AudioClip yesAudio;
    public AudioClip noAudio;
    public AudioClip onSoundAudio;
    public AudioClip offSoundAudio;
    public AudioClip onBGMAudio;
    public AudioClip offBGMAudio;
    public AudioClip closeAudio;
    public AudioClip playPlayerAudio;
    public AudioClip deletePlayerAudio;
    public AudioClip addPLayerAudio;
    public AudioClip submitPlayerAudio;
    public AudioClip detailAudio;
    public AudioClip infoAudio;
    public AudioClip deleteRecordAudio;

    // Sub AudioClip
    public AudioClip backAudio;
    public AudioClip bentukAudio;
    public AudioClip bentukWarnaAudio;

    // GameAudioClip
    public AudioClip pauseAudio;
    public AudioClip resumeAudio;
    public AudioClip menuAudio;
    // GameAudioClip
    public AudioClip scoreAudio;
    public AudioClip timeAudio;
    public AudioClip soalAudio;
    public AudioClip jawabanAudio;

    // Menu UI 
    public void quitButton()
    {
        soundGuide.clip = quitAudio;
        soundGuide.Play();
    }
    public void leaderBoardButton()
    {
        soundGuide.clip = leaderBoardAudio;
        soundGuide.Play();
    }
    public void settingButton()
    {
        soundGuide.clip = settingAudio;
        soundGuide.Play();
    }
    public void helpButton()
    {
        soundGuide.clip = helpAudio;
        soundGuide.Play();
    }
    public void playButton()
    {
        soundGuide.clip = playAudio;
        soundGuide.Play();
    }
    public void yesButton()
    {
        soundGuide.clip = yesAudio;
        soundGuide.Play();
    }
    public void noButton()
    {
        soundGuide.clip = noAudio;
        soundGuide.Play();
    }
    public void onSoundButton()
    {
        soundGuide.clip = onSoundAudio;
        soundGuide.Play();
    }
    public void offSoundButton()
    {
        soundGuide.clip = offSoundAudio;
        soundGuide.Play();
    }
    public void onBGMButton()
    {
        soundGuide.clip = onBGMAudio;
        soundGuide.Play();
    }
    public void offBGMButton()
    {
        soundGuide.clip = offBGMAudio;
        soundGuide.Play();
    }
    public void closeButton()
    {
        soundGuide.clip = closeAudio;
        soundGuide.Play();
    }
    public void playPlayerButton()
    {
        subMenuContainer.SetActive(true);
        menuContainer.SetActive(false);
        soundGuide.clip = playPlayerAudio;
        soundGuide.Play();
    }
    public void deletePlayerButton()
    {
        soundGuide.clip = deletePlayerAudio;
        soundGuide.Play();
    }
    public void addPLayerButton()
    {
        soundGuide.clip = addPLayerAudio;
        soundGuide.Play();
    }
    public void submitPlayerButton()
    {
        subMenuContainer.SetActive(true);
        menuContainer.SetActive(false);
        soundGuide.clip = submitPlayerAudio;
        soundGuide.Play();
    }
    public void detailButton()
    {
        soundGuide.clip = detailAudio;
        soundGuide.Play();
    }
    public void infoButton()
    {
        soundGuide.clip = infoAudio;
        soundGuide.Play();
    }
    public void deleteRecordButton()
    {
        soundGuide.clip = deleteRecordAudio;
        soundGuide.Play();
    }

    // Sub void
    public void backButton()
    {
        subMenuContainer.SetActive(false);
        menuContainer.SetActive(true);
        soundGuide.clip = backAudio;
        soundGuide.Play();
    }
    public void bentukButton()
    {
        soundGuide.clip = bentukAudio;
        soundGuide.Play();
    }
    public void bentukWarnaButton()
    {
        soundGuide.clip = bentukWarnaAudio;
        soundGuide.Play();
    }

    // Gamevoid
    public void pauseButton()
    {
        soundGuide.clip = pauseAudio;
        soundGuide.Play();
    }
    public void resumeButton()
    {
        soundGuide.clip = resumeAudio;
        soundGuide.Play();
    }
    public void menuButton()
    {
        gameContainer.SetActive(false);
        menuContainer.SetActive(true);
        soundGuide.clip = menuAudio;
        soundGuide.Play();
    }

    // Gamevoid
    public void scoreContainer()
    {
        soundGuide.clip = scoreAudio;
        soundGuide.Play();
    }
    public void timeContainer()
    {
        soundGuide.clip = timeAudio;
        soundGuide.Play();
    }
    public void soalContainer()
    {
        soundGuide.clip = soalAudio;
        soundGuide.Play();
    }
    public void jawabanContainer()
    {
        soundGuide.clip = jawabanAudio;
        soundGuide.Play();
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}
