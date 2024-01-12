using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TimmySlashController : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> m_SlashEffects = new List<ParticleSystem>();
    [SerializeField] private EnemySpawnController m_EnemySpawnController;
    [SerializeField] private Transform m_MainCamera;
    [SerializeField] private Color m_Color;
    [SerializeField] private AudioSource m_AudioSource;
    
    public void Slash(){
        for (int i = 0; i < BaseDefenceManager.GetInstance().GetLocationScriptable().Level+1; i++)
        {
            int index = i;
            StartCoroutine(Slash(index*0.15f+0.5f, index));
        }
        
    }

    private IEnumerator Slash(float delay, int index){
        yield return new WaitForSeconds(delay);
        // play sound
        m_AudioSource.Play();
        m_SlashEffects[index].Play();
        var allEnemy = m_EnemySpawnController.GetAllEnemyTrans().ToList();
        foreach (var item in allEnemy)
        {
            if(item == null)
                continue;

            if(Vector3.Distance(m_MainCamera.position,item.position)<4f){
                item.GetComponent<EnemyControllerBase>().ChangeHp(-45f);
                Vector2 hitPoint = Camera.main.WorldToScreenPoint(item.position);
                //Debug.Log(hitPoint);
                hitPoint += new Vector2(0,300);
                hitPoint = new Vector2(hitPoint.x,Mathf.Clamp(hitPoint.y,300f,700f));
                BaseDefenceManager.GetInstance().SetDamageText(65f,m_Color,hitPoint);
            }
        }

    }
}
