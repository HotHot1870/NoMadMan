using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KineticTransmitterController : MonoBehaviour
{
    [SerializeField] private EnemySpawnController m_EnemySpawnController;
    public void init(){
        var allEnemy = m_EnemySpawnController.GetAllEnemyTrans().ToList();
        foreach (var item in allEnemy)
        {
            item.GetComponent<EnemyControllerBase>().OnNet();
        }
    }
}
