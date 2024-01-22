using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExtendedButtons;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocationPanelController : MonoBehaviour
{
    [SerializeField] private GameObject m_Self; 
    [SerializeField] private TMP_Text m_LocationName;   
    [SerializeField] private Transform m_EnemyBlockParent;  
    [SerializeField] private GameObject m_EnemyBlockPrefab; 
    [SerializeField] private Button2D m_DefenceBtn;
    [SerializeField] private Button2D m_CancelLocationDetailBtn;

    

    void Start(){

        m_DefenceBtn.onDown.AddListener(()=>{
            MainGameManager.GetInstance().OnClickStartSound();
        });
        m_DefenceBtn.onUp.AddListener(()=>{
            MainGameManager.GetInstance().OnClickEndSound();
        });
        m_DefenceBtn.onClick.AddListener(MapManager.GetInstance().GetMapUIController().OnClickDefence);


        m_CancelLocationDetailBtn.onDown.AddListener(()=>{
            MainGameManager.GetInstance().OnClickStartSound();
        });
        m_CancelLocationDetailBtn.onUp.AddListener(()=>{
            MainGameManager.GetInstance().OnClickEndSound();
        });
        m_CancelLocationDetailBtn.onClick.AddListener(CancelLocationDetail);
    }

    public void TurnOffPanel(){
        m_Self.SetActive(false);
    }

    
    private void CancelLocationDetail() {
        TurnOffPanel();
        MapManager.GetInstance().GetLocationController().SetLocationCameraPiority(0);
        
    }

    

    public bool ShouldShowLocationDetail(){
        return !m_Self.activeSelf;
    }

    public void Init(){
        MapLocationScriptable locationData = MapManager.GetInstance().GetLocationController().GetScriptable();
        if(locationData==null || m_Self.activeSelf)
            return;

        
        MapManager.GetInstance().GetLocationController().SetLocationCameraPiority(10);

        m_Self.SetActive(true);
        m_DefenceBtn.gameObject.SetActive( !MapManager.GetInstance().GetLocationController().ShouldShowCorruption() );
        m_LocationName.text = locationData.DisplayName;

        // enemy list
        
        var allenemy = MainGameManager.GetInstance().GetAllEnemy();
        var allEnemyId = locationData.NormalWaveEnemy.Union<int>(locationData.FinalWaveEnemy).ToList<int>();
        
        for (int i = 0; i < m_EnemyBlockParent.childCount; i++)
        {
            Destroy(m_EnemyBlockParent.GetChild(i).gameObject);
        }
        foreach (var item in allEnemyId.Distinct())
        {
            var newEnemyBlock = Instantiate(m_EnemyBlockPrefab,m_EnemyBlockParent );
            var enemyScriptable = allenemy.Find(x=>x.Id==item);
            newEnemyBlock.GetComponent<EnemyBlockController>().Init(enemyScriptable);
        }

        // TODO : Mutation
    }
}
