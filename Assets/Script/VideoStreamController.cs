using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoStreamController : MonoBehaviour
{
    public RawImage rawImage;
    public VideoPlayer videoPlayer;
    public GameObject buttonFullScreen;
    public GameObject buttonCloseFullScreen;

    // Use this for initialization
    void Start()
    {
    }
    public IEnumerator PlayVideo()
    {
        videoPlayer.Prepare();
        WaitForSeconds waitForSeconds = new WaitForSeconds(1);
        while (!videoPlayer.isPrepared)
        {
            yield return waitForSeconds;
            break;
        }
        rawImage.texture = videoPlayer.texture;
        videoPlayer.Play();
    }

    public void OnClickFullScreen()
    {
        buttonFullScreen.SetActive(false);
        buttonCloseFullScreen.SetActive(true);
        rawImage.transform.position = new Vector3(rawImage.transform.position.x, rawImage.transform.position.y + 28, rawImage.transform.position.z);
        rawImage.GetComponent<RectTransform>().sizeDelta = new Vector2(800f, 450f);
    }

    public void OnClickCloseFullScreen()
    {
        buttonFullScreen.SetActive(true);
        buttonCloseFullScreen.SetActive(false);
        rawImage.transform.position = new Vector3(rawImage.transform.position.x, rawImage.transform.position.y - 28, rawImage.transform.position.z);
        rawImage.GetComponent<RectTransform>().sizeDelta = new Vector2(398f, 218f);
    }
}