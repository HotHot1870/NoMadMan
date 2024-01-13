
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ExtendedButtons;
using System.Linq;

public class BaseDefenceResultPanel : MonoBehaviour
{
    [SerializeField] private GameObject m_Self;
    [SerializeField] private TMP_Text m_ResultTitle;
    [SerializeField] private Transform m_EnemyBlockParent;
    [SerializeField] private GameObject m_EnemyBlockPrefab;
    [SerializeField] private Button2D m_CloseBtn;
    private List<int> m_AllKilledEnemy = new List<int>();
    
    private void Start()
    {
        m_Self.SetActive(false);
    }

    public void DeadEnemyId(int id){
        m_AllKilledEnemy.Add(id);
    }

    public void Init(bool isWin){
        m_ResultTitle.text = isWin?"Coast Clear":"Defeated";
        foreach (var item in m_AllKilledEnemy.Distinct())
        {
            // TODO : spawn enemy block
        }
    }
}
