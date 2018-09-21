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

