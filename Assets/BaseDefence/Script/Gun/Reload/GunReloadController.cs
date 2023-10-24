using System;
using System.Collections;
using System.Collections.Generic;
using ExtendedButtons;
using GunReloadScriptableNameSpace;
using BaseDefenceNameSpace;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MainGameNameSpace;


public class GunReloadController : MonoBehaviour
{
    [SerializeField] private GunReloadScriptable m_ReloadScriptable;
    [SerializeField] private bool m_IsTryReload = false;
    private GunReloadControllerConfig m_Config=null; 
    [SerializeField] private AudioSource m_GunReloadAudioSource;
    
    [Header("Reload")]
    [SerializeField] private Image m_MainGunImage;
    [SerializeField] private GameObject m_DragIcon;
    [SerializeField] private GameObject m_TapIcon;
    [SerializeField] private Transform m_GrayWhileDragPanel; // gray out while draging // no use for now
    [SerializeField] private Transform m_NotGrayWhileDragPanel; // NOT gray out while draging , for EndDragPrefab in ReloadScriptable // no use for now

    [Header("Drag")]
    [SerializeField] private GameObject m_ArrowPrefab;
    private RectTransform m_DragImage; // mouse while dragging
    private bool m_IsDraging = false;
    private GameObject m_EndDragImage;
    private GameObject m_DragArrow;


    private Sprite m_MainGunOldSprite; // incase canel drag and need to reset main gun image

    private List<GameObject> m_AllSpawnedImage = new List<GameObject>();
    private int m_CurReloadPhase = 0;

    private void Start() {
        if(m_IsTryReload){
            return;
        }
        BaseDefenceManager.GetInstance().m_ReloadUpdateAction += UpdateDragImagePosition;
        BaseDefenceManager.GetInstance().m_ChangeToReloadAction += SetStartReloadPanel;
        MainGameManager.GetInstance().AddNewAudioSource(m_GunReloadAudioSource);

        BaseDefenceManager.GetInstance().CloseReloadPanel();
    }

    private void UpdateDragImagePosition() {

        // set drag image position same as mouse position 
        if(m_DragImage != null){
            m_DragImage.position = Input.mousePosition;
        }
    }


    private void SetStartReloadPanel(){
        m_ReloadScriptable = m_Config.GunScriptable.ReloadScriptable;
        m_CurReloadPhase = 0;
        m_MainGunImage.sprite = m_ReloadScriptable.StartMainGunImage;
        m_MainGunImage.rectTransform.sizeDelta = m_ReloadScriptable.MainGunSize;
        SetReloadPhase();
    }

    public void StartReload(GunReloadControllerConfig config){
        m_Config = config;

        if(m_IsTryReload){
            return;
        }
        BaseDefenceManager.GetInstance().ChangeGameStage(BaseDefenceStage.Reload);
    }

    private void SetReloadPhase(){
        if(m_ReloadScriptable.ReloadPhases.Count<=m_CurReloadPhase){
            // out of phase , cancel reload 
            Debug.Log("Cancel Reload by out of phase");
            ResultAction(GunReloadActionResult.CancelReload);
            return;
        }

        // clear all spawn reload UI
        for (int i = 0; i < m_AllSpawnedImage.Count; i++)
        {
            if(m_AllSpawnedImage[i] != null){
                Destroy(m_AllSpawnedImage[i]);
            }
        }
        m_AllSpawnedImage.Clear();

        // remove arrow
        if(m_DragArrow != null){
            Destroy(m_DragArrow);
            m_DragArrow = null;
        }
        
        // remove end drag
        if(m_EndDragImage != null){
            Destroy(m_EndDragImage);
            m_EndDragImage = null;
        }

        GunReloadPhase currentGunReloadPhase = m_ReloadScriptable.ReloadPhases[m_CurReloadPhase];
        // spawn UI
        for (int i = 0; i < currentGunReloadPhase.ExtraImages.Count; i++)
        {
            SpawnUIObjectForReloadPhaseConfig extraItemConfig = new SpawnUIObjectForReloadPhaseConfig{
                Prefab = currentGunReloadPhase.ExtraImages[i].ImagePrefab,
                Position = currentGunReloadPhase.ExtraImages[i].Position,
                Size = currentGunReloadPhase.ExtraImages[i].Size
            };
            SpawnUIObjectForReloadPhase( extraItemConfig );
        }
        for (int i = 0; i < currentGunReloadPhase.DragFunction.Count; i++)
        {
            SpawnDragItems(currentGunReloadPhase.DragFunction[i]);  
        }

        for (int i = 0; i < currentGunReloadPhase.TapFunction.Count ; i++)
        {
            SpawnTapItem(currentGunReloadPhase.TapFunction[i]);
        }

        m_MainGunOldSprite = m_MainGunImage.sprite;
    }

    
    private GameObject SpawnUIObjectForReloadPhase(SpawnUIObjectForReloadPhaseConfig config){
        var spawnedUIObject = Instantiate(config.Prefab);
        spawnedUIObject.transform.SetParent(m_GrayWhileDragPanel);
        spawnedUIObject.GetComponent<RectTransform>().anchoredPosition = config.Position;
        m_AllSpawnedImage.Add(spawnedUIObject);

        spawnedUIObject.GetComponent<RectTransform>().sizeDelta = config.Size;

        if(spawnedUIObject.TryGetComponent<ReloadIcon>(out var reloadIcon))
            reloadIcon.m_UnderText.text = config.UnderText;

        return spawnedUIObject;
    }

    private void SpawnTapItem(GunReloadTapFunction tapFunction){
        SpawnUIObjectForReloadPhaseConfig config = new SpawnUIObjectForReloadPhaseConfig{
            UnderText = tapFunction.UnderText,
            Prefab = m_TapIcon,
            Position = tapFunction.Position
        };
        Button tapBtn = SpawnUIObjectForReloadPhase( config ).GetComponent<Button>();
        tapBtn.onClick.AddListener(()=>{
            OnClickTap(tapFunction);
        });
    }

    private void OnClickTap(GunReloadTapFunction tapFunction){
        m_GunReloadAudioSource.PlayOneShot(tapFunction.TapEndSound);
        m_MainGunImage.sprite = tapFunction.MainGunSpriteOnEnd;
        ResultAction(tapFunction.ResultAction);
    }

    #region Drag
    private void SpawnDragItems(GunReloadDragFunction dragFunction){
        
        SpawnUIObjectForReloadPhaseConfig config = new SpawnUIObjectForReloadPhaseConfig{
            UnderText = dragFunction.StartDragUnderText,
            Prefab = m_DragIcon,
            Position = dragFunction.StartDragPosition
        };
        Button2D startDragIcon = SpawnUIObjectForReloadPhase( config ).GetComponent<Button2D>();

        startDragIcon.onDown.AddListener(()=>{
            OnDownStartDrag(dragFunction);
        });

        
        // Spawn arrow
        SpawnUIObjectForReloadPhaseConfig arrowConfig = new SpawnUIObjectForReloadPhaseConfig{
            Prefab = m_ArrowPrefab,
            Position = dragFunction.StartDragPosition

        };
        m_DragArrow = SpawnUIObjectForReloadPhase( arrowConfig );
        m_DragArrow.transform.SetParent(m_NotGrayWhileDragPanel);
        m_DragArrow.GetComponent<RectTransform>().sizeDelta = new Vector2(
            m_DragArrow.GetComponent<RectTransform>().sizeDelta.x,
            Vector2.Distance(dragFunction.EndDragPosition , dragFunction.StartDragPosition)
        );
        Vector2 lookAngle = dragFunction.EndDragPosition - dragFunction.StartDragPosition;
        m_DragArrow.GetComponent<RectTransform>().localEulerAngles = new Vector3(0,0, Mathf.Rad2Deg * Mathf.Atan2(lookAngle.y,lookAngle.x) -90 );

        startDragIcon.onUp.AddListener(()=>{
                OnUpStartDrag(dragFunction);
        });
    }

    private void OnEnterEndDrag(GunReloadDragFunction dragFunction){
        m_GunReloadAudioSource.PlayOneShot(dragFunction.EndDragSound);
        m_IsDraging = false;
        //m_Gray.SetActive(m_IsDraging);
        m_MainGunImage.sprite = dragFunction.MainGunSpriteOnEnd;
        ResultAction( dragFunction.ResultAction );
    }

    private void OnUpStartDrag(GunReloadDragFunction dragFunction){

        m_GunReloadAudioSource.PlayOneShot(dragFunction.CancelDragSound);
        m_IsDraging = false;
        //m_Gray.SetActive(m_IsDraging);
        // remove arrow
        if(m_DragArrow != null){
            Destroy(m_DragArrow);
            m_DragArrow = null;
        }
        
        // remove end drag
        if(m_EndDragImage != null){
            Destroy(m_EndDragImage);
            m_EndDragImage = null;
        }

        // remove cursor image
        if(m_DragImage != null){
            Destroy(m_DragImage.gameObject);
            m_DragImage = null;
        }

        // reset main gun image
        m_MainGunImage.sprite = m_MainGunOldSprite;
    }

    private void OnDownStartDrag(GunReloadDragFunction dragFunction){

        // do not do anything if already draging
        if(m_IsDraging)
            return;

        m_GunReloadAudioSource.PlayOneShot(dragFunction.StartDragSound);
        m_IsDraging = true;
        //m_Gray.SetActive(m_IsDraging);

        // remove arrow
        if(m_DragArrow != null){
            Destroy(m_DragArrow);
            m_DragArrow = null;
        }
        
        // remove end drag
        if(m_EndDragImage != null){
            Destroy(m_EndDragImage);
            m_EndDragImage = null;
        }

        // change main gun sprite
        m_MainGunImage.sprite = dragFunction.MainGunSpriteOnStart;

/*
        // Spawn arrow
        SpawnUIObjectForReloadPhaseConfig arrowConfig = new SpawnUIObjectForReloadPhaseConfig{
            Prefab = m_ArrowPrefab,
            Position = dragFunction.StartDragPosition

        };
        m_DragArrow = SpawnUIObjectForReloadPhase( arrowConfig );
        m_DragArrow.transform.SetParent(m_NotGrayWhileDragPanel);
        m_DragArrow.GetComponent<RectTransform>().sizeDelta = new Vector2(
            m_DragArrow.GetComponent<RectTransform>().sizeDelta.x,
            Vector2.Distance(dragFunction.EndDragPosition , dragFunction.StartDragPosition)
        );
        Vector2 lookAngle = dragFunction.EndDragPosition - dragFunction.StartDragPosition;
        m_DragArrow.GetComponent<RectTransform>().localEulerAngles = new Vector3(0,0, Mathf.Rad2Deg * Mathf.Atan2(lookAngle.y,lookAngle.x) -90 );
*/
        // spawn cursor image
        if(dragFunction.DragCursorPrefab != null){
            SpawnUIObjectForReloadPhaseConfig cursorConfig = new SpawnUIObjectForReloadPhaseConfig{
                Prefab = dragFunction.DragCursorPrefab,
                Position = Input.mousePosition

            };
            GameObject cursor = SpawnUIObjectForReloadPhase( cursorConfig );
            cursor.transform.SetParent(m_NotGrayWhileDragPanel);
            m_DragImage = cursor.GetComponent<RectTransform>();
        }

        // Spawn end darg 
        SpawnUIObjectForReloadPhaseConfig dragEndConfig = new SpawnUIObjectForReloadPhaseConfig{
            UnderText = dragFunction.EndDragUnderText,
            Prefab = m_DragIcon,
            Position = dragFunction.EndDragPosition

        };
        m_EndDragImage = SpawnUIObjectForReloadPhase( dragEndConfig );
        m_EndDragImage.GetComponent<Button2D>().onEnter.AddListener(()=>{
            OnEnterEndDrag(dragFunction);
        });
    }
    #endregion

    #region Result Action

    private void ResultAction(GunReloadActionResult actionEnum){
        if(actionEnum == (actionEnum | GunReloadActionResult.CancelReload) ){
            // cancel reload 
            
            if(m_IsTryReload){
                return;
            }
            BaseDefenceManager.GetInstance().ChangeGameStage(BaseDefenceStage.Shoot);
            
        }

        if(actionEnum == ( actionEnum | GunReloadActionResult.GainOneAmmo ) ){
            // Gain one ammo 
            m_Config.GainAmmo?.Invoke(1);
        }


        if(actionEnum == ( actionEnum | GunReloadActionResult.FullAmmoReload ) ){
            // Full Ammo Reload
            m_Config.SetAmmoToFull?.Invoke();
        }

        if(actionEnum == ( actionEnum | GunReloadActionResult.PhaseChangeByAmmo ) ){
            // if ammo full , to next phase , refreash otherwise
            if(m_Config.IsFullClipAmmo.Invoke()){
                ++m_CurReloadPhase;
            }
            SetReloadPhase();   
        }

        if(actionEnum == ( actionEnum | GunReloadActionResult.SetClipAmmoToZero ) ){
            // Set Clip Ammo To Zero
            m_Config.SetAmmoToZero?.Invoke();
        }

        if(actionEnum == ( actionEnum | GunReloadActionResult.ToNextPhase ) ){
            // To Next Phase
            ++m_CurReloadPhase;
            SetReloadPhase();
        }

    }
        
    #endregion
}
