using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionUIManager : MonoBehaviour
{
    public static MissionUIManager Instance { get; private set; }

    private enum MissionUIState
    {
        Expanded,
        Unfolded
    }
    [SerializeField] private MissionUIState curState = MissionUIState.Expanded;

    public RectTransform switchButton;
    public GameObject missionScroll;
    public Image progressBar;



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
    public void OnSwitchButtonClicked()
    {
        if (curState == MissionUIState.Expanded)
        {
            curState = MissionUIState.Unfolded;
            switchButton.rotation = Quaternion.Euler(0, 0, -90);
            missionScroll.SetActive(false);
        }
        else
        {
            curState = MissionUIState.Expanded;
            switchButton.rotation = Quaternion.Euler(0, 0, 90);
            missionScroll.SetActive(true);
        }
    }
    public void UpdateProgressBar(float progress)
    {
        progressBar.fillAmount = progress;
    }
}
