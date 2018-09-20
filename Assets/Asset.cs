using UnityEngine;
using System;
using System.Collections;
using System.Linq;

[Serializable]
public class Asset
{
    public string Name;
    public string url;
    public string type;

    public virtual IEnumerator Load()
    {
        if (_www == null)
        {
            _www = new WWW(url);
            yield return _www;

            if (_www.error == null)
            {
                yield return new WaitForEndOfFrame();

                var request = _www.assetBundle.LoadAllAssetsAsync();

                yield return request;

                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();

                AssetObj = request.allAssets[0];
            }

            Add();
        }
    }

    public virtual void Add()
    {
        GameManager.assets.Add(this);
    }

    public WWW _www;
    protected UnityEngine.Object AssetObj;

    public virtual void Instantiate()
    {
        GameObject instantiatedasset = null;
        if (type == "character")
        {
            instantiatedasset = GameObject.Instantiate(AssetObj, Vector3.zero, Quaternion.identity) as GameObject;
        }
    }
}

[System.Serializable]
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