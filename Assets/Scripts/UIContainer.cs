using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIContainer : MonoBehaviour
{
    public static UIContainer instance;

    private void Awake()
    {
        instance = this;
    }

    [SerializeField]
    private GameObject textContainer;
    [SerializeField]
    private Text textField;
    [SerializeField]
    private Text healthTextField;

    public void ShowText(string text)
    {
        textField.text = text;
        textContainer.SetActive(true);
    }
    public void HideText()
    {
        textContainer.SetActive(false);
    }
    public void SetHealth(int health)
    {
        healthTextField.text = "Health: " + health;
    }

}
