using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyControllerBase : MonoBehaviour
{
    protected EnemyScriptable Scriptable;
    protected float CurHp;
    public Action m_OnDead = null;
    protected bool m_IsDead = false;

    /// <summary>
    /// use negative for damage
    /// </summary>
    public void ChangeHp(float changes){
        if( m_IsDead )
            return;

        CurHp += changes;
        CurHp = Mathf.Clamp(CurHp,0f,Scriptable.MaxHp);
        if( CurHp<=0 ){
            // dead
            m_IsDead = true;
            OnDead();
        }
    }

    public bool IsDead(){
        return m_IsDead;
    }


    protected abstract void OnDead();
}
