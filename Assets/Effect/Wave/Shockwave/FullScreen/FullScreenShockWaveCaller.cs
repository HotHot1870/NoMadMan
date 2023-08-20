using System.Collections;
using System.Collections.Generic;
using ullScreenShockWaveControllerNameSpace;
using UnityEngine;
using UnityEngine.UI;

public class FullScreenShockWaveCaller : MonoBehaviour
{
    [SerializeField] private FullScreenShockWaveController m_FullScreenShockWaveController;
    [SerializeField] private Button m_Btn;
    [SerializeField] private FullScreenShockWaveConfig m_Config;

    private void Start() {
        m_Btn.onClick.AddListener(()=>m_FullScreenShockWaveController.Init(m_Config));
    }


}
