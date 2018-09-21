using UnityEngine;
using System;
using System.Collections;
using System.Linq;

[Serializable]
public class Asset
{
    //public fields as strings
    public string Name;
    public string url;
    public string type;

    //acces to web pages
    public WWW _www;
    //an asset object
    protected UnityEngine.Object AssetObj;

    //coroutine
    //what does virtual mean?
    public virtual IEnumerator Load()
    {
        //if the web page is null
        if (_www == null)
        {
            //make a new www with a url
            _www = new WWW(url);
            //pause here and come back later
            yield return _www;

            //if no errors
            if (_www.error == null)
            {
                //wait a frame
                yield return new WaitForEndOfFrame();

                //get all the assets through the url into the request
                var request = _www.assetBundle.LoadAllAssetsAsync();

                //return that request
                yield return request;

                //wait some more frames
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();

                //get the first of all the assets and put it into the assetObj
                AssetObj = request.allAssets[0];
            }

            //then add?
            Add();
        }
    }

    public virtual void Add()
    {
        //add this to the gamemanager assets
        GameManager.assets.Add(this);
    }


    public virtual void Instantiate()
    {
        //set the instantiated asset to null
        GameObject instantiatedasset = null;
        //if this type is a character
        if (type == "character")
        {
            //instantiate the assetObj
            instantiatedasset = GameObject.Instantiate(AssetObj, Vector3.zero, Quaternion.identity) as GameObject;
        }
    }
}

