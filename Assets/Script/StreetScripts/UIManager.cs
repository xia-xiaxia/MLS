using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Button clearDataButton;  
    public GameObject confirmationPopup; 
    public Button confirmButton;   
    public Button cancelButton;
   
    void Start()
    {
   
        clearDataButton.onClick.AddListener(OnClearDataButtonClicked);

        confirmationPopup.SetActive(false); // start时隐藏是否确认panel
        confirmButton.onClick.AddListener(OnConfirmDelete);
        cancelButton.onClick.AddListener(OnCancelDelete);
    }

    private void OnClearDataButtonClicked()
    {
       
        confirmationPopup.SetActive(true);
    }

    private void OnConfirmDelete()
    {
        GameDataManager.Instance.ClearAllData(); //清除所有数据
        confirmationPopup.SetActive(false); 
    }

    private void OnCancelDelete()
    {
        confirmationPopup.SetActive(false);  
    }
}
