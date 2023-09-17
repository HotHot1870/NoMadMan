using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExtendedButtons;
using TMPro;

public class MapUIController : MonoBehaviour
{
    [SerializeField] private Button2D m_MapDragBtn;  
    [SerializeField] private Button m_CheckLocationBtn;
    //private MapLocationScriptable m_SelectedLocation = null;

    [Header("Location")]
    [SerializeField] private GameObject m_LocationDetailPanel; 
    [SerializeField] private TMP_Text m_LocationName;  
    [SerializeField] private TMP_Text m_FortifyCost;  
    [SerializeField] private TMP_Text m_FortifyRewardText;  
    [SerializeField] private Image m_RewardImage;
    [SerializeField] private TMP_Text m_EnemyText;  
    [SerializeField] private Transform m_EnemyList; 
    [SerializeField] private Button m_FortifyBtn;  
    [SerializeField] private Button m_StartWaveBtn;
    [SerializeField] private Button m_CancelBtn;

    [Header("Fortify Detail")]
    [SerializeField] private GameObject m_FortifyPanel; 
    [SerializeField] private TMP_Text m_FortifyCostInDetail; 
    [SerializeField] private TMP_Text m_RewardName;
    [SerializeField] private Image m_RewardImageInDetail;
    [SerializeField] private Button m_YesFortifyBtn;  
    [SerializeField] private Button m_NoFortifyBtn;  




    private void Start() {
        m_CheckLocationBtn.gameObject.SetActive(false);
        TurnOffAllPanel();
        var freeCameraController = MapManager.GetInstance().GetMapFreeCameraController();
        m_MapDragBtn.onDown.AddListener(freeCameraController.OnDragMapBtnDown);
        m_MapDragBtn.onUp.AddListener(freeCameraController.OnDragMapBtnUp);
        m_MapDragBtn.onExit.AddListener(freeCameraController.OnDragMapBtnUp);

        m_CheckLocationBtn.onClick.AddListener(ShowLocationDetail);

        // location detail
        m_FortifyBtn.onClick.AddListener(OnClickFortify);
        m_StartWaveBtn.onClick.AddListener(OnClickStartWave);
        m_CancelBtn.onClick.AddListener(CancelLocationDetail);

        // fortify detail panel
        m_NoFortifyBtn.onClick.AddListener(OnClickFortifyNo);
        m_YesFortifyBtn.onClick.AddListener(OnClickFortifyYes);
    }

    private void CancelLocationDetail() {
        m_LocationDetailPanel.SetActive(false);
        StartCoroutine(SetMovable());
    }

    private IEnumerator SetMovable(){
        // wait until location detail panel are hidden , then set movable
        yield return null;
        MapManager.GetInstance().GetVehicleController().ChangeMovable(true);

    }

    private void OnClickStartWave() {
        MapLocationScriptable locationData = MapManager.GetInstance().GetNearestLocationController().GetScriptable();
        if(locationData==null)
            return;
            
        MainGameManager.GetInstance().SetBaseDefenceScene(locationData);
    }

    private void ShowLocationDetail(){
        MapLocationScriptable locationData = MapManager.GetInstance().GetNearestLocationController().GetScriptable();
        if(locationData==null)
            return;

        
        MapManager.GetInstance().GetNearestLocationController().SetLocationCameraPiority(10);

        m_LocationDetailPanel.SetActive(true);
        m_LocationName.text = locationData.LocationName;
        m_FortifyCost.text = "Fortify Cost : "+locationData.FortifyCost;
        m_FortifyRewardText.text = "Fortify Reward : "+locationData.Reward.DisplayName;


        m_RewardImage.sprite = locationData.Reward.DisplayImage;
        m_EnemyText.text = " Enemy : ( Danger "+locationData.WaveData.NormalWavesDangerValue+","+locationData.WaveData.FinalWaveDangerValue+" )";
        // TODO : enemy list
        MapManager.GetInstance().GetVehicleController().ChangeMovable(false);

    }

    public void ChangeCheckLocationActive(bool isActive){
        m_CheckLocationBtn.gameObject.SetActive(isActive);
    }


    private void OnClickFortify(){
        MapLocationScriptable locationData = MapManager.GetInstance().GetNearestLocationController().GetScriptable();
        if(locationData==null)
            return;

        // show fortify reward
        m_FortifyPanel.SetActive(true);
        m_FortifyCostInDetail.text = "Fortify Cost : "+locationData.FortifyCost;
        m_RewardName.text = "Fortify Reward : "+locationData.Reward.DisplayName;
        m_RewardImageInDetail.sprite = locationData.Reward.DisplayImage;
    }

    private void OnClickFortifyYes(){
        // comfirm fortify
    }


    private void OnClickFortifyNo(){
        // cancel fortify
        m_FortifyPanel.SetActive(false);

    }

    private void TurnOffAllPanel(){
        m_LocationDetailPanel.SetActive(false);
        m_FortifyPanel.SetActive(false);
    }
}
