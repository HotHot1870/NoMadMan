using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

public class FlatEnemyController : EnemyController
{
    [SerializeField] private GameObject m_Self;
    [SerializeField] private Vector3 m_Destination;

    public void Init(EnemyScriptable scriptable, Vector3 destination){
        Scriptable = scriptable;
        m_Destination = destination;
        CurHp = Scriptable.MaxHp;
    }

    private void Start() {
        
    }

    private void Update() {
        if( IsDead )
            return;

        if(Vector2.Distance( new Vector2(m_Self.transform.position.x,m_Self.transform.position.z) , m_Destination)<0.25f){
           // close enough for attack 

        }else{
            // move
            float moveDistance = Scriptable.MoveSpeed * Time.deltaTime;
            m_Self.transform.position = Vector3.MoveTowards(
                m_Self.transform.position, m_Destination, moveDistance);
        }
    }
    protected override void OnDead(){
        m_OnDead?.Invoke();
        Destroy(m_Self,1);
    }

}
