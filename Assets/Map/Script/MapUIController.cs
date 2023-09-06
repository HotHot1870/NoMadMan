using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExtendedButtons;
using TMPro;

public class MapUIController : MonoBehaviour
{
    [SerializeField] private Button2D m_MapDragBtn;  

    private void Start() {
        var freeCameraController = MapManager.GetInstance().GetMapFreeCameraController();
         m_MapDragBtn.onDown.AddListener(freeCameraController.OnDragMapBtnDown);
         m_MapDragBtn.onUp.AddListener(freeCameraController.OnDragMapBtnUp);
         m_MapDragBtn.onExit.AddListener(freeCameraController.OnDragMapBtnUp);
    }

}
