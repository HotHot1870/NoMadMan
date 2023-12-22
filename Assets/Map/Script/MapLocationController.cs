using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MapLocationController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera m_LocationCamera;
    [SerializeField] private List<GameObject> m_CoruuptionObject = new List<GameObject>();
    [SerializeField] private MapLocationScriptable m_Scriptable;
    private Color m_CorrupColor = new Color(50f/255f,0,1);
    private Color m_BaseColor = new Color(1,1,1);
    private Color m_NeverPlayedColor = new Color(1,1,0);
    private bool m_ShouldShowCorruption = false;


    // Start is called before the first frame update
    void Start()
    {
        m_LocationCamera.Priority = 0;
    }

    public bool ShouldShowCorruption(){
        SetCoruption();
        return m_ShouldShowCorruption;
    }

    public void SetScriptable(MapLocationScriptable scriptable){
        m_Scriptable = scriptable;
        SetCoruption();
    }

    private void SetCoruption(){
        // should show coruption
        m_ShouldShowCorruption = false;
        var allLocationScriptable =  MainGameManager.GetInstance().GetAllLocation();
        foreach (var item in m_Scriptable.LockBy)
        {
            if(item == -1){
                m_ShouldShowCorruption = false;
                break;
            }
            var targetScriptable = allLocationScriptable.Find(x=>x.Id==item);
            if( System.Convert.ToSingle( MainGameManager.GetInstance().GetData<int>(targetScriptable.DisplayName+item) ) <=0f ){

                m_ShouldShowCorruption = true;
                break;
            }
        }

        // Set coruption actiove
        foreach (var item in m_CoruuptionObject)
        {
            //item.SetActive( m_ShouldShowCorruption );
            var targetColor = m_ShouldShowCorruption? m_CorrupColor : m_BaseColor;
            if(!m_ShouldShowCorruption && (int)System.Convert.ToSingle( MainGameManager.GetInstance().GetData<int>(m_Scriptable.DisplayName+m_Scriptable.Id) ) <=0){
                // never played before
                targetColor = m_NeverPlayedColor;
            }
            item.GetComponent<MeshRenderer>().material.SetColor("_Color",  targetColor); 
            item.GetComponent<MeshRenderer>().material.SetColor("_IntersectColor",  targetColor); 
        }

    }

    public MapLocationScriptable GetScriptable(){
        return m_Scriptable;
    }

    public void SetLocationCameraPiority(int priority){
        m_LocationCamera.Priority = priority;
    }
}
