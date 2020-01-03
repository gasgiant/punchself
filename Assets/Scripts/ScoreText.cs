using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreText : MonoBehaviour
{
    public TextMeshProUGUI leftScoreText;
    public TextMeshProUGUI rightScoreText;

    Coroutine showRoutine;

    private void OnEnable()
    {
        SetScore(0, 0);
    }

    public void SetScore(int left, int right)
    {
        leftScoreText.text = "" + left;
        rightScoreText.text = "" + right;
    }

    public WaitForSeconds ShowScore(int left, int right)
    {
        if (showRoutine == null)
            StartCoroutine(ShowRoutine(left, right));
        return new WaitForSeconds(2.4f);
    }

    IEnumerator ShowRoutine(int left, int right)
    {
        GetComponent<Animator>().SetTrigger("Appear");
        yield return new WaitForSeconds(0.7f);
        SetScore(left, right);
        yield return new WaitForSeconds(1f);
        GetComponent<Animator>().SetTrigger("Disappear");
        yield return new WaitForSeconds(0.7f);
        showRoutine = null;
    }
}
