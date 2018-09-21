using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    //this is the static instance of the game manager
    static private GameManager Instance;

    //this is a ist of json text assets
    public List<TextAsset> _jsonTexts;

    //this is a static list of assets
    static public List<Asset> assets;
    //this is a list of instantiated assets as gameobjects
    static public List<GameObject> InstantiatedAssets;

    //this is a active loading asset
    public static Asset ActiveLoadingAsset;
    //this is an asyncoperation / the current instantiating asset operation
    public static AsyncOperation CurrentInstantiatingAssetOperation { get; internal set; }

    //a bool if the scene is set up
    private static bool _isSceneSetup;

    // Use this for initialization
    private void Awake()
    {
        //assign the static instance to this one
        Instance = this;
        //make a new list of assets and put it into the static public one
        assets = new List<Asset>();
        //make a new list of gameobjects and put it into the instantiated assets public thing
        InstantiatedAssets = new List<GameObject>();
    }

    private void Start()
    {
        //for each text in the text list of text assets 
        foreach (var text in _jsonTexts)
        {
            //assign the content to asset
            var asset = ReadFromJson(text.text);

            //if the asset type is "audio"
            if (asset.type.Equals("audio"))
                //read it as an audio asset
                asset = ReadAudioAssetFromJson(text.text);
            //start the asset load as a coroutine
            StartCoroutine(asset.Load());
        }

        //some public int field with an overwritten set method
        AssetCount = 0;
    }

    //public field
    public int AssetCount
    {
        //overwritten setter
        set
        {
            //if the count of assets is higher than the value that is coming in
            if (assets.Count > value)
                //remove assets in that range from the list
                //from the back of it but also huh?
                assets.RemoveRange(value, assets.Count - value);
        }
    }

    //private method to set up the scene
    private static void SetupScene()
    {
        //if the assets.count is the same as the json text assets count and the scene is not set up yet
        if (assets.Count == ((GameManager)GameObject.Find("GameManager").GetComponent("GameManager"))._jsonTexts.Count && _isSceneSetup == false)
        {
            //for each asset in assets
            foreach (var asset in assets)
            {
                //instantiate it
                asset.Instantiate();
            }
            //after that set the scene to set up
            _isSceneSetup = true;

            //reset
            //and then set the asset counter to 0 again??
            GameManager.Instance.AssetCount = 0;
        }
    }

    //read the asset from json
    static public Asset ReadFromJson(string JSONString)
    {
        return JsonUtility.FromJson<Asset>(JSONString);
    }

    //read the audio asset from json
    private static AudioAsset ReadAudioAssetFromJson(string JSONString)
    {
        return JsonUtility.FromJson<AudioAsset>(JSONString);
    }

    public void Update()
    {
        //if the operation is not null, so probably if its not instantiating right now
        if (GameManager.CurrentInstantiatingAssetOperation != null)
        {
            //if its done
            if (GameManager.CurrentInstantiatingAssetOperation.isDone == true)
            {
                //if the active loading type is a character
                if (GameManager.ActiveLoadingAsset.type.Equals("character"))
                {
                    //print that to log
                    Debug.Log(GameManager.ActiveLoadingAsset.Name + "has finished loading from" + GameManager.ActiveLoadingAsset.url);
                }
                //if its a location
                if (GameManager.ActiveLoadingAsset.type.Equals("location"))
                    //print that to the log
                    Debug.Log("Location " + GameManager.ActiveLoadingAsset.Name + "has been loaded!");
            }
        }
        //set up the scene
        SetupScene();
    }
}