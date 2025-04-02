using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
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
    public GameObject progressBar;
    public TextMeshProUGUI missionDescription;
    public Image bar;
    public TextMeshProUGUI count;



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
            progressBar.SetActive(false);
        }
        else
        {
            curState = MissionUIState.Expanded;
            switchButton.rotation = Quaternion.Euler(0, 0, 90);
            missionScroll.SetActive(true);
            progressBar.SetActive(true);
        }
    }
    public void UpdateMissionView(Mission mission)
    {
        missionDescription.text = mission.description;
        bar.fillAmount = (float)mission.progress / mission.maxProgress;
        count.text = mission.progress + " / " + mission.maxProgress;
    }
    public void UpdateProgressBar(int progress, int maxProgress)
    {
        bar.fillAmount = (float)progress / maxProgress;
        count.text = progress + " / " + maxProgress;
    }
}
