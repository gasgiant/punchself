using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public SmoothSlider progressSlider;
    public TextMeshProUGUI announceText;
    public Animator announceTextAnimator;
    public ScoreText scoreText;

    public Animator wintext1;
    public Animator wintext2;
    public TextMeshProUGUI wintext1TMP;
    public TextMeshProUGUI wintext2TMP;
    public Color color1;
    public Color color2;
    bool winText1Active;
    bool winText2Active;

    public GameObject mainMenu;
    public GameObject pasueMenu;

    private void Update()
    {
        progressSlider.SetTargetValue(
            (GameManager.Instance.bodyTransform.position.x / GameManager.Instance.distance + 1) * 0.5f);
    }

    public WaitForSeconds Announce(string text)
    {
        announceText.text = text;
        announceTextAnimator.SetTrigger("Appear");
        return new WaitForSeconds(85f / 60);
    }

    public void ShowWinTexts(bool leftWins)
    {
        if (leftWins)
        {
            wintext1TMP.text = "RED WINS";
            wintext2TMP.text = "RED WINS";
            wintext1TMP.color = color1;
            wintext2TMP.color = color1;
        }
        else
        {
            wintext1TMP.text = "BLUE WINS";
            wintext2TMP.text = "BLUE WINS";
            wintext1TMP.color = color2;
            wintext2TMP.color = color2;
        }

        winText1Active = true;
        winText2Active = true;
        wintext1.SetTrigger("AppearLeft");
        wintext2.SetTrigger("AppearRight");
    }

    public void HideWinTexts()
    {
        if (winText1Active)
            wintext1.SetTrigger("DisappearLeft");
        if (winText2Active)
            wintext2.SetTrigger("DisappearRight");
        winText1Active = false;
        winText2Active = false;
    }
}
