using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyController : MonoBehaviour
{
    protected EnemyScriptable Scriptable;
    protected float CurHp;

    /// <summary>
    /// use negative for damage
    /// </summary>
    public void ChangeHp(float changes){
        CurHp += changes;
        CurHp = Mathf.Clamp(CurHp,0f,Scriptable.MaxHp);
        Debug.Log(CurHp);
        if( CurHp<=0 ){
            // dead
            OnDead();
        }
    }
    protected abstract void OnDead();
}
