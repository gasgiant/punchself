using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteSwitcher : MonoBehaviour
{
    Image image;
    public Sprite sprite1;
    public Sprite sprite2;

    bool toggle;

    private void OnEnable()
    {
        image = GetComponent<Image>();
        toggle = false;
        image.sprite = sprite1;
    }

    public void SwitchSprite()
    {
        if (toggle)
        {
            toggle = false;
            image.sprite = sprite1;
        }
        else
        {
            toggle = true;
            image.sprite = sprite2;
        }
    }
}