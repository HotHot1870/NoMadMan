using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

public class FlatEnemyController : MonoBehaviour
{
    private EnemyScriptable m_Scriptable;
    [SerializeField] private GameObject m_Self;
    [SerializeField] private Vector3 m_Destination;

    private void Init(EnemyScriptable scriptable, Vector3 destination){
        m_Scriptable = scriptable;
        m_Destination = destination;
    }

    private void Start() {
        
    }

}
