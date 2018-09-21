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
        foreach (var text in _jsonTexts)
        {
            var asset = ReadFromJson(text.text);

            if (asset.type.Equals("audio"))
                asset = ReadAudioAssetFromJson(text.text);

            StartCoroutine(asset.Load());
        }

        AssetCount = 0;
    }

    public int AssetCount
    {
        set
        {
            if (assets.Count > value)
                assets.RemoveRange(value, assets.Count - value);
        }
    }

    private static void SetupScene()
    {
        if (assets.Count == ((GameManager)GameObject.Find("GameManager").GetComponent("GameManager"))._jsonTexts.Count && _isSceneSetup == false)
        {
            foreach (var asset in assets)
            {
                asset.Instantiate();
            }

            _isSceneSetup = true;

            //reset
            GameManager.Instance.AssetCount = 0;
        }
    }

    static public Asset ReadFromJson(string JSONString)
    {
        return JsonUtility.FromJson<Asset>(JSONString);
    }

    private static AudioAsset ReadAudioAssetFromJson(string JSONString)
    {
        return JsonUtility.FromJson<AudioAsset>(JSONString);
    }

    public void Update()
    {
        if (GameManager.CurrentInstantiatingAssetOperation != null)
        {
            if (GameManager.CurrentInstantiatingAssetOperation.isDone == true)
            {
                if (GameManager.ActiveLoadingAsset.type.Equals("character"))
                {
                    Debug.Log(GameManager.ActiveLoadingAsset.Name + "has finished loading from" + GameManager.ActiveLoadingAsset.url);
                }

                if (GameManager.ActiveLoadingAsset.type.Equals("location"))
                    Debug.Log("Location " + GameManager.ActiveLoadingAsset.Name + "has been loaded!");
            }
        }

        SetupScene();
    }
}