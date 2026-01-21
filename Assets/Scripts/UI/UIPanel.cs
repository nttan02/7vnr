using System.Collections.Generic;
using UnityEngine;

public class UIPanel : MonoBehaviour
{
    public List<ButtonUI> uiButtons;
    public void Close()
    {
        gameObject.SetActive(false);
    }
}