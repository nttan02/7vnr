using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Button))]
public class ButtonUI : MonoBehaviour
{
    private Button button;
    public GameObject BagPanel;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        OpenPanel(ButtonType.BAG);
    }

    public void OpenPanel(ButtonType buttonType)
    {
        CloseAllPanel();
        switch (buttonType)
        {
            case ButtonType.BAG:
                BagPanel.SetActive(true);
                break;
        }
    }
    public void CloseAllPanel()
    {
        BagPanel.SetActive(false);
    }
}