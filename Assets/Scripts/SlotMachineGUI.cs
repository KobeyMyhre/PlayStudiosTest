using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//I thought this class would be used for more stuff, but It's pretty much only used for animating the winText
//This class would be used more for stuff like updating the UI for things like chips/XP or something
public class SlotMachineGUI : MonoBehaviour
{
    public SlotMachine slotMachine;

    private void Start()
    {
        slotMachine.onSpin += resetWinText;
        slotMachine.onWin += animateWinText;
    }


    public Transform winHolder;
    public Text winText;
    public float winAnimationDuration = 1;
    public float winAnimationScaleValue;
    bool isAnimatingWinText;
    public void animateWinText(int winAmount)
    {
        if(!isAnimatingWinText)
        {
            StartCoroutine(winTextAnimation(winAmount, winAnimationDuration));
        }
    }

    public void resetWinText()
    {
        winText.text = "0";
        winHolder.transform.localScale = Vector3.one;
        stopWinTextAnimation();
    }

    public void stopWinTextAnimation()
    {
        isAnimatingWinText = false;
    }
    IEnumerator winTextAnimation(int winAmount, float duration)
    {
        isAnimatingWinText = true;
        float t = 0;
        int sumAnimation = 0;
        Vector3 startScale = winHolder.transform.localScale;
        Vector3 endScale = startScale * winAnimationScaleValue;
        while(t < 1 && isAnimatingWinText)
        {
            t += Time.deltaTime / duration;
            sumAnimation = Mathf.RoundToInt(Mathf.Lerp(0, winAmount, t));
            winText.text = sumAnimation.ToString();
            winHolder.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }
        isAnimatingWinText = false;
    }
}
