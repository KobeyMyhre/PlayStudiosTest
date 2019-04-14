using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ReelStripType
{
    public ReelIconTypes[] reelType;

    public ReelStripType(List<string> jsonReelStrip)
    {
        reelType = new ReelIconTypes[jsonReelStrip.Count];
        for(int i =0; i < reelType.Length; i++)
        {
            switch(jsonReelStrip[i])
            {
                case "DIAMOND":
                    reelType[i] = ReelIconTypes.diamonds;
                    break;
                case "CLUBS":
                    reelType[i] = ReelIconTypes.clubs;
                    break;
                case "SPADES":
                    reelType[i] = ReelIconTypes.spades;
                    break;
                case "HEARTS":
                    reelType[i] = ReelIconTypes.hearts;
                    break;
            }
        }
    }
}
[System.Serializable]
public class ReelIconContainer
{
    public RectTransform rectTransform;
    public Image image;

    public ReelIconContainer(RectTransform rect, Image img)
    {
        image = img;
        rectTransform = rect;
    }
}

//Handles the logic for an individual ReelSpinner
public class ReelSpinner : MonoBehaviour
{
    public ReelSpinnerManager manager;
    public RectTransform reelIconPrefab;
    public RectTransform reelView;
    public List<ReelIconContainer> icons;
    [HideInInspector]
    public float speed;

    public bool shouldSpin;
    public ReelStripType reelStripType;
    public int reelIdx = 0;

    
    [Header("Slowing Down Variables (Runtime)")]
    public int stopIdx;
    public bool isSlowingDown;
    public bool isWinningStop;
    public ReelIconContainer iconToStop;
    bool isStopped;
    float slowDownRange;

    [HideInInspector]
    public float minAlpha;
    [HideInInspector]
    public float winAnimationSpeed;
    Color animStartColor;
    Color animEndColor;
    
    //Sets up variables for the ReelSpinner, generates the icons, and copies the managers shared variables
    public void initReelSpinner(List<string> reelStripString)
    {
        reelStripType = new ReelStripType(reelStripString);
        icons = new List<ReelIconContainer>();
        Vector3 startPos = reelView.transform.position + (((reelView.rect.height / 2) + (reelIconPrefab.rect.height /2)) * Vector3.up);
        slowDownRange = startPos.y - reelView.transform.position.y;
        for (int i =0; i < 4; i++)
        {
            GameObject spawnedIcon = Instantiate(reelIconPrefab.gameObject);
            RectTransform rectTransform = spawnedIcon.GetComponent<RectTransform>();
            
            spawnedIcon.transform.position = startPos;
            spawnedIcon.transform.parent = reelView;
            startPos -= Vector3.up * reelIconPrefab.rect.height;
            spawnedIcon.GetComponent<Image>().sprite = manager.getReelIcon(reelStripType.reelType[reelIdx + i]);
            Image image = spawnedIcon.GetComponent<Image>();
            
            icons.Add(new ReelIconContainer(rectTransform, image));
        }
        animStartColor = icons[0].image.color;
        animEndColor = new Color(animStartColor.r, animStartColor.g, animStartColor.b, minAlpha);
        speed = manager.spinSpeed;
        minAlpha = manager.minAlpha;
        winAnimationSpeed = manager.winAnimationSpeed;
    }

    //This starts the movement of the reel and resets the animations
    public void startMoving()
    {
        isStopped = false;
        shouldSpin = true;
        if(iconToStop.image != null)
        {
            iconToStop.image.color = new Color(iconToStop.image.color.r, iconToStop.image.color.g, iconToStop.image.color.b, 1);
        }
        iconToStop = null;
        isSlowingDown = false;
        stopIdx = -1;
    }

   
    //Yields until the reelSpinner has come to a complete stop
    public IEnumerator waitForMeToStop()
    {
        while(!isStopped)
        {
            yield return null;
        }
    }

    private void Update()
    {
        if(shouldSpin)
            moveIconsDown();
    }

    //Once we find the Icon we're going to stop. We scale the speed of the ReelSpinner based on the distance to
    //the stop position. If it is not a winningStop then we stop the Reel so that it is crooked 
    float getSlowDownSpeed()
    {
        float distance = (iconToStop.rectTransform.transform.position.y - reelView.position.y);
        if (!isWinningStop) { distance -= iconToStop.rectTransform.rect.height / 2; }
        float percent = distance / slowDownRange;
        return speed * percent;
    }

    //Tells the reel where to stop
    public void stopReel(int _stopidx, bool _isWinnignStop)
    {
        stopIdx = _stopidx;
        isWinningStop = _isWinnignStop;
    }

    //Moves the Reel Icons downwards based on the speed and checks for a stop
    void moveIconsDown()
    {
        float usedSpeed = isSlowingDown ? getSlowDownSpeed() : speed;
        if(usedSpeed <= 0.2f)
        {
            isStopped = true;
        }
        for(int i =0; i < icons.Count; i++)
        {
            icons[i].rectTransform.position += Vector3.down * usedSpeed * Time.deltaTime ;
            resetIconPosition(icons[i]);
        }
    }

    //Loops the Icon back to the top after it has passed a threshold. Also checks for the stop idx and begins
    //the stop if valid
    void resetIconPosition(ReelIconContainer icon)
    {
        if(icon.rectTransform.transform.position.y <= (reelView.transform.position.y - (reelView.rect.height /2) - (icon.rectTransform.rect.height /2)))
        {
            //icon.transform.position = reelView.transform.position + (((reelView.rect.height / 2) + (reelIconPrefab.rect.height / 2)) * Vector3.up);
            icon.rectTransform.transform.position += Vector3.up * (reelView.rect.height + icon.rectTransform.rect.height);
            reelIdx--;
            if(reelIdx < 0) { reelIdx = reelStripType.reelType.Length - 1; }
            if(reelIdx == stopIdx)
            {
                iconToStop = icon;
                isSlowingDown = true;
            }
            icon.image.sprite = manager.getReelIcon(reelStripType.reelType[reelIdx]);
        }
    }

    public void playWinAnimation()
    {
        StartCoroutine(animateWinningIcon(iconToStop));
    }

    public void stopWinAnimation()
    {
        shouldAnimateWin = false;
    }
    public bool shouldAnimateWin;
    //Animates the alpha of the stopped Icon
    IEnumerator animateWinningIcon(ReelIconContainer icon)
    {
        shouldAnimateWin = true;
        float t = 0;
        
        int sign = 1;
        while (shouldAnimateWin)
        {
            t += Time.deltaTime / winAnimationSpeed * sign;
            icon.image.color = Color.Lerp(animStartColor, animEndColor, t);
            if(t >= 1 || t <= 0) { sign *= -1; t = Mathf.Clamp01(t); }
            yield return null;
        }
    }

}
