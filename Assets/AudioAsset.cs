using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AudioAsset : Asset
{
    public AudioClip AudioClip;

    public AudioClip AudioClipProperty
    {
        get { return AudioClip; }
        set
        {
            if (AudioClip == null)
            {
                AudioClip = value;
                GameManager.assets.Add(this);
            }
            else
            {
                for (int i = 0; i < GameManager.assets.Count(); ++i)
                {
                    if (GameManager.assets[i] == this)
                    {
                        Debug.LogWarning("There is already an audio asset in audioasset like this: " + AudioClip);
                    }
                }
            }
        }
    }

    public override IEnumerator Load()
    {
        if (_www == null)
        {
            _www = new WWW(url);
            yield return _www;

            if (_www.error == null)
            {
                yield return new WaitForEndOfFrame();

                if (_www.GetAudioClip() != null)
                {
                    var request = _www.GetAudioClip(true);

                    yield return request;

                    AudioClip = request;
                }
            }

            Add();
        }
    }

    public override void Add()
    {
        base.Add();

        Debug.Log("Audio loaded");
    }

    public override void Instantiate()
    {
        new GameObject("Audio", typeof(AudioSource));
        AudioSource audiosource = GameObject.Find("Audio").GetComponent("AudioSource") as AudioSource;

        audiosource.clip = AudioClip;
        audiosource.Play();
        Debug.Log("For audio assets there is nothing to instantiate!");
    }
}