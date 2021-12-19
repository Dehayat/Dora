using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextController : MonoBehaviour
{
    [SerializeField]
    private string text = "";

    public void ShowText()
    {
        UIContainer.instance.ShowText(text);
    }
    public void HideText()
    {
        UIContainer.instance.HideText();
    }

}
