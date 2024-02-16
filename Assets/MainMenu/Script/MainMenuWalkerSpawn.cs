using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MainMenuWalkerSpawn : MonoBehaviour
{
    [SerializeField] private GameObject m_WalkerPrefab;
    [SerializeField] private Transform m_WalkerParent;
    [SerializeField] private Transform m_Destination;

    void Start(){
        StartCoroutine(SpawnWalkers());
    }

    private IEnumerator SpawnWalkers(){
        float passTime = 0;
        float duration = 3;
        while (m_WalkerParent.childCount < 100f)
        {   
            passTime += Time.deltaTime;
            yield return null;
            if(passTime >duration){
                var newWalker = Instantiate(m_WalkerPrefab,m_WalkerParent);
                newWalker.transform.position = new Vector3(
                    Random.Range(-6,6),
                    0,
                    0
                )+this.transform.position;

                var destination = m_Destination.position+new Vector3(
                    Random.Range(-8,8),
                    0,
                    0
                );
                newWalker.transform.LookAt(destination);
                newWalker.GetComponent<WalkerController>().Init(destination);
                passTime = 0;
            }
        }
    }


}
