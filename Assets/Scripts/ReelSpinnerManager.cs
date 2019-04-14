using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Enums used to replace the strings of ReelStrips
public enum ReelIconTypes
{
    clubs,
    hearts,
    spades,
    diamonds
}
//Used in editor to set up what image correlates to what Enum Type
[System.Serializable]
public class ReelImageAndType
{
    public Sprite reelImage;
    public ReelIconTypes reelType;
}
//Manages all the invidivual reels in the slotMachine
//Mostly holds all their shared variables and has wrapper
//functions to call a function on all/some of the reels
public class ReelSpinnerManager : MonoBehaviour
{
    public string pathToReelJson;
    public ReelStrips reelStrips;
    public ReelImageAndType[] reelImages;
    //Dictionary to look up what sprite is used for what ReelIconType
    public Dictionary<ReelIconTypes, Sprite> reelImageLookup;
    public ReelSpinner[] reelSpinners;
    [Header("Reel Spinner Variables")]
    public float spinSpeed;
    [Range(0, 1)]
    public float minAlpha;
    public float winAnimationSpeed;
    private void Start()
    {
        reelStrips = JsonLoader.instance.loadReelStrips(pathToReelJson);
        //Set up the Dictionary
        reelImageLookup = new Dictionary<ReelIconTypes, Sprite>();
        for(int i =0; i < reelImages.Length; i++)
        {
            reelImageLookup.Add(reelImages[i].reelType, reelImages[i].reelImage);
        }
        //Set up the ReelSpinners
        for(int i =0; i < reelSpinners.Length; i++)
        {
            reelSpinners[i].manager = this;
            reelSpinners[i].initReelSpinner(reelStrips.realStrips[i].realStrip);
        }
    }

    public Sprite getReelIcon(ReelIconTypes reelIconType)
    {
        return reelImageLookup[reelIconType];
    }

    //Used to call the animation function on the reels
    public void animateReels(int activeReels)
    {
        for(int i =0; i < activeReels; i++)
        {
            reelSpinners[i].playWinAnimation();
        }
    }
    //Used tp stop the animation function on the reels
    public void stopAnimateReels()
    {
        for (int i = 0; i < reelSpinners.Length; i++)
        {
            reelSpinners[i].stopWinAnimation();
        }
    }

    //Spins the reels
    public void spinReels()
    {
        StartCoroutine(spinReelsWithDelay());
    }

    //Adds a delay between each reel spin
    IEnumerator spinReelsWithDelay()
    {
        for (int i = 0; i < reelSpinners.Length; i++)
        {
            reelSpinners[i].startMoving();
            yield return new WaitForSeconds(.3f);

        }
    }

    //Tells a reel what idx it needs to stop at and if it's a winning stop
    public void stopReel(int reelIdx, int stopIdx, bool isWinningStop)
    {
        reelSpinners[reelIdx].stopReel(stopIdx, isWinningStop);
    }

    public IEnumerator waitForReelToStop(int idx)
    {
        yield return reelSpinners[idx].waitForMeToStop();
    }
}
