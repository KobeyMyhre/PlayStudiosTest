using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;


//Helper class for loading Json Data
public class JsonLoader : MonoBehaviour {

    public static JsonLoader instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }else { Destroy(this); }
    }

    
	
    //Takes in a path to the resources folder and returns the SpinsContainer that it finds
    public SpinsContainer loadSpins(string path)
    {
        SpinsContainer retval = null;
        retval = JsonConvert.DeserializeObject<SpinsContainer>(getStringFromTextFile(path));
        if(retval == null)
        {
            Debug.Log("Couldn't find spins at path: " + path);
        }
        return retval;
    }

    //Takes in a path to the resources folder and returns the ReelStrips that it finds
    public ReelStrips loadReelStrips(string path)
    {
        ReelStripJsonContainer reelStripJson = JsonConvert.DeserializeObject<ReelStripJsonContainer>(getStringFromTextFile(path));
        if(reelStripJson == null)
        {
            Debug.Log("Couldn't find Reels at path: " + path);
        }
        else
        {
            ReelStrips retval = new ReelStrips(reelStripJson);
            return retval;
        }
        return null;
    }


    //Returns a string of the text in a textFile at the given path
	public string getStringFromTextFile(string path)
    {
        TextAsset spinsFile = Resources.Load<TextAsset>(path);
        if(spinsFile != null)
        {
            return spinsFile.text;
        }
        return "No File Found At: " + path;
    }
}
