using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MapLocationController : MonoBehaviour
{
    [SerializeField] private CinemachineFreeLook m_LocationCamera;
    [SerializeField] private MapLocationScriptable m_Scriptable;


    // Start is called before the first frame update
    void Start()
    {
        m_LocationCamera.Priority = 0;
    }

    public void SetScriptable(MapLocationScriptable scriptable){
        m_Scriptable = scriptable;
    }

    public MapLocationScriptable GetScriptable(){
        return m_Scriptable;
    }

    public void SetLocationCameraPiority(int priority){
        m_LocationCamera.Priority = priority;
    }
}
