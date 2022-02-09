using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public bool vidPlayed = true;
    public long currentFrame;
    Camera cam;
    float rew = 0;
    float prev, next;
    public Text timeVid, speedVid, spotVid, zoomVid, volVid;
    List<float> spots = new List<float>() { 0, 20, 25, 40, 55, 70 };
    List<string> spotName = new List<string>() { "Opening", "spot 1", "spot 2", "spot 3", "spot 4", "spot 5" };
    int spotNow = 1;
    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        spotVid.text = spotName[0];
    }

    // Update is called once per frame
    void Update()
    {
        Gamepad pad = Gamepad.current;
        if (pad.buttonSouth.wasPressedThisFrame)
        {
            if (vidPlayed)
                videoPlayer.Pause();
            else
                videoPlayer.Play();
            vidPlayed = !vidPlayed;
        }
        if (pad.buttonEast.wasPressedThisFrame)
        {
            videoPlayer.playbackSpeed += 0.1f;
            speedVid.text = videoPlayer.playbackSpeed.ToString() + "x";
        }
        if (pad.buttonNorth.wasPressedThisFrame)
        {
            videoPlayer.playbackSpeed -= 0.1f;
            speedVid.text = videoPlayer.playbackSpeed.ToString() + "x";
        }

        if (pad.leftStick.up.ReadValue() > Mathf.Epsilon && cam.fieldOfView > 1)
        {
            if (pad.rightShoulder.isPressed)
            {
                cam.fieldOfView -= pad.leftStick.up.ReadValue() / 7;
                zoomVid.text = (66 - Mathf.Round(cam.fieldOfView)) + "x";
            }
            else if (pad.rightTrigger.isPressed)
            {
                if (videoPlayer.GetDirectAudioVolume(0) < 1f)
                    videoPlayer.SetDirectAudioVolume(0, videoPlayer.GetDirectAudioVolume(0) + .001f);
                else
                    videoPlayer.SetDirectAudioVolume(0, .99f);

                volVid.text = Mathf.Round(videoPlayer.GetDirectAudioVolume(0) * 100) + "%";
            }
            else
            {
                videoPlayer.frame += 1;
            }
        }
        else if (cam.fieldOfView <= 1)
        {
            cam.fieldOfView = 1.1f;
        }

        if (pad.leftStick.down.ReadValue() > Mathf.Epsilon && cam.fieldOfView < 65)
        {
            if (pad.rightShoulder.isPressed)
            {
                cam.fieldOfView += pad.leftStick.down.ReadValue() / 7;
                zoomVid.text = (66 - Mathf.Round(cam.fieldOfView)) + "x";
            }
            else if (pad.rightTrigger.isPressed)
            {
                if (videoPlayer.GetDirectAudioVolume(0) > .01f)
                    videoPlayer.SetDirectAudioVolume(0, videoPlayer.GetDirectAudioVolume(0) - .001f);
                else
                    videoPlayer.SetDirectAudioVolume(0, .02f);

                volVid.text = Mathf.Round(videoPlayer.GetDirectAudioVolume(0) * 100) + "%";
            }
            else
            {
                rew += pad.leftStick.down.ReadValue() / 50;
                timeVid.text = videoPlayer.time.ToString("F2") + ("( -" + rew.ToString("F2") + " )");
            }
        }
        else if (cam.fieldOfView >= 65)
        {
            cam.fieldOfView = 64.9f;
        }
        else if (rew > 0)
        {
            videoPlayer.time -= rew;
            rew = 0;
        }
        if (pad.leftStick.left.wasPressedThisFrame)
            prev = pad.leftStick.left.ReadValue();
        if (pad.leftStick.left.wasReleasedThisFrame && prev > .6f)
        {
            setSpot(-1);
            prev = 0;
        }
        if (pad.leftStick.right.wasPressedThisFrame)
            next = pad.leftStick.right.ReadValue();
        if (pad.leftStick.right.wasReleasedThisFrame && next > .6f)
        {
            setSpot(1);
            next = 0;
        }
        if (rew == 0)
        {
            timeVid.text = videoPlayer.time.ToString("F2");
        }
        for (int a = 0; a < spots.Count; a++)
        {
            if (videoPlayer.time >= spots[spots.Count - 1])
            {
                spotNow = spots.Count - 1;
                spotVid.text = spotName[spotNow];
            }
            else if (videoPlayer.time >= spots[a] && videoPlayer.time < spots[a + 1])
            {
                spotNow = a;
                spotVid.text = spotName[spotNow];
                break;
            }
        }

    }

    public void setSpot(int a=0)
    {
        if(spotNow == spots.Count-1 && a == 1)
        {
            videoPlayer.Stop();
            videoPlayer.time = 0;
        }
        else if(spotNow == 0 && a == -1)
        {
            videoPlayer.time = 0;
        }
        else
        {
            spotNow += a;
            videoPlayer.time = spots[spotNow];
            spotVid.text = spotName[spotNow];
        }
        
    }

}
