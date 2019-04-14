using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//Json Containers for loading the Json files
//Used http://json2csharp.com/ to generate JsonContainers
[System.Serializable]
public class Spin
{
    public List<int> ReelIndex;
    public int ActiveReelCount;
    public int WinAmount;
}
[System.Serializable]
public class SpinsContainer
{
    public List<Spin> Spins;
}

[System.Serializable]
public class ReelStripJsonContainer
{
    public List<List<string>> ReelStrips;
}

[System.Serializable]
public class ReelStrip
{
    public List<string> realStrip;

    public ReelStrip(List<string> strip)
    {
        realStrip = strip;
    }
}

//Bascially this is the same thing as a ReelStripJsonContainer, but Unity doesn't serialize List<List<T>>
//So I created this class with a "Copy" constructor of a ReelStripJsonContainer so it could be viewed in the editor
[System.Serializable]
public class ReelStrips
{
    public ReelStrip[] realStrips;
    

    public ReelStrips(ReelStripJsonContainer json)
    {
        realStrips = new ReelStrip[json.ReelStrips.Count];
        for(int i =0; i < realStrips.Length; i++)
        {
            realStrips[i] = new ReelStrip(json.ReelStrips[i]);
        }
    }
}
    



