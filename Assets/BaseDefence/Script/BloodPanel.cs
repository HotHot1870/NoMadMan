using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BloodPanel : MonoBehaviour
{
    [SerializeField] private GameObject m_BloodPrefab;
    [SerializeField] private Transform m_BloodParent;

    public void SpawnBloodEffect(Vector2 screenPos){
        // blood effect on hit by enemy
        var blood = Instantiate(m_BloodPrefab,m_BloodParent);
        var bloodRect = blood.GetComponent<RectTransform>();
        bloodRect.position = screenPos;
        bloodRect.eulerAngles = new Vector3(0,0,Random.Range(0f,360f));
        StartCoroutine(SpawnBlood(blood));
    }

    private IEnumerator SpawnBlood(GameObject blood){
        yield return new WaitForSeconds(1f);
        float passTime = 0;
        float duration = 0.5f;
        while ( passTime < duration )
        {
            passTime += Time.deltaTime;
            blood.GetComponent<Image>().color = new Color(1,1,1,(duration-passTime)/ duration);
            yield return null;
        }
        Destroy(blood);
    }
}
