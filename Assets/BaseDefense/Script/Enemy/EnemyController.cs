using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyController : MonoBehaviour
{
    protected EnemyScriptable Scriptable;
    protected float CurHp;
    public Action m_OnDead = null;
    protected bool IsDead = false;

    /// <summary>
    /// use negative for damage
    /// </summary>
    public void ChangeHp(float changes){
        if( IsDead )
            return;

        CurHp += changes;
        CurHp = Mathf.Clamp(CurHp,0f,Scriptable.MaxHp);
        if( CurHp<=0 ){
            // dead
            IsDead = true;
            OnDead();
        }
    }
    protected abstract void OnDead();
}
