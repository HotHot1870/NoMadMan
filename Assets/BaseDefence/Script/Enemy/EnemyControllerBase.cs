using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class EnemyControllerInitConfig{
    public EnemyScriptable scriptable;
    public Vector3 destination;
    public Vector3 cameraPos;
    public Vector3 spawnPos;
    public Camera camera;
    public int spawnId;
}

public abstract class EnemyControllerBase : MonoBehaviour
{
    public Canvas HpCanvas;
    public Image HpBar;
    public GameObject HpParent;
    protected EnemyScriptable Scriptable;
    protected float CurHp;
    public Action OnDeadAction = null;
    protected bool IsThisDead = false;
    protected Vector3 CameraPos;
    protected Vector3 Destination;
    protected Camera MainCamera;
    protected int SpawnId;
    [SerializeField] protected ParticleSystem m_ExplodeHitParticle;
    [SerializeField] protected List<MeshRenderer> m_AllNetMeshRenderer = new List<MeshRenderer>();
    [SerializeField] protected List<SkinnedMeshRenderer> m_AllNetSkinnedMeshRenderer = new List<SkinnedMeshRenderer>();
    protected bool m_IsNeted = false;
 
    
    
    public virtual void Init(EnemyControllerInitConfig config){
        Scriptable = config.scriptable;
        Destination = config.destination;
        CurHp = Scriptable.MaxHp;
        CameraPos = config.cameraPos;
        MainCamera =config.camera;
        SpawnId = config.spawnId;
        this.transform.position = config.spawnPos;
        HpCanvas.worldCamera = config.camera;
        BaseDefenceManager.GetInstance().AddEnemyToList(this.transform);
        HpParent.SetActive(false);
    }

    public virtual void OnNet(){
        m_IsNeted = transform;
        // net effect
        foreach (var item in m_AllNetMeshRenderer)
        {
            if(item == null)
                continue;
            foreach (var material in item.materials)
            {
                material.SetFloat("_LineThiccness", 0.2f);
            }
        }

        foreach (var item in m_AllNetSkinnedMeshRenderer)
        {
            if(item == null)
                continue;
            foreach (var material in item.materials)
            {
                material.SetFloat("_LineThiccness", 0.2f);
            }
        }
        StartCoroutine(NetTime());
        // TODO : pause animation
    }

    private IEnumerator NetTime(){
        yield return new WaitForSeconds(BaseDefenceManager.GetInstance().GetLocationScriptable().Level*0.5f+2f);
        OnNetEnd();
    }

    protected virtual void OnNetEnd(){
        m_IsNeted = false;

        // remove net effect
        foreach (var item in m_AllNetMeshRenderer)
        {
            if(item == null)
                continue;
            foreach (var material in item.materials)
            {
                material.SetFloat("_LineThiccness", 0f);
            }
        }

        foreach (var item in m_AllNetSkinnedMeshRenderer)
        {
            if(item == null)
                continue;
            foreach (var material in item.materials)
            {
                material.SetFloat("_LineThiccness", 0f);
            }
        }

    }

    public int GetId(){
        return SpawnId;
    }

    /// <summary>
    /// use negative for damage
    /// </summary>
    public void ChangeHp(float changes){
        if( IsThisDead )
            return;

        CurHp += changes;
        
        HpParent.SetActive(true);
        HpParent.transform.rotation = new Quaternion(0,0,0,0);
        HpBar.fillAmount = CurHp / Scriptable.MaxHp;
        StartCoroutine(HideHpUI());
        CurHp = Mathf.Clamp(CurHp,0f,Scriptable.MaxHp);
        if( CurHp<=0 ){
            // dead
            IsThisDead = true;
            HpParent.SetActive(false);
            OnDead();
        }
    }

    private IEnumerator HideHpUI(){
        yield return new WaitForSeconds(3);
        HpParent.SetActive(false);
    }
/*
    public void ExplodeHitParticle(){
        var explodeHitParticle = Instantiate(m_ExplodeHitParticle);
        explodeHitParticle.transform.position = this.transform.position;
        explodeHitParticle.Play();
    }*/

    public bool IsDead(){
        return IsThisDead;
    }

    public float GetMaxHp(){
        return Scriptable.MaxHp * (BaseDefenceManager.GetInstance().GetLocationScriptable().HealthMutation/100f+1f);
    }

    public EnemyScriptable GetScriptable(){
        return Scriptable;
    }


    protected virtual void OnDead(){
        BaseDefenceManager.GetInstance().RemoveDeadEnemyFromList(this.transform);
        MainGameManager.GetInstance().ChangeGooAmount(Scriptable.GooOnKill);
        OnDeadAction?.Invoke();
    }
}
