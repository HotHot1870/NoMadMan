using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImageEffect : MonoBehaviour
{
    [Header(" *** IMPORTANT *** : the parent of this object must NOT move")]
    [SerializeField] private AfterImageEffectTarget m_TargetObject = null;
    [SerializeField][Range(1,20)] private int m_AfterImageCount = 5;
    [SerializeField][Range(0.1f,2)] private float m_AfterImageDistance = 0.5f;
    [SerializeField] private Gradient m_Gradient;
    
    List<AfterImageEffectTarget> m_AllAfterImage = new List<AfterImageEffectTarget>();

    private void Start() {
        if(m_TargetObject == null)
            return;

        
        this.transform.localScale = m_TargetObject.Self.transform.lossyScale;
        
    }

    private void FixedUpdate() {
        if(m_TargetObject == null)
            return;


        var targetGameOpbject = m_TargetObject.Self;
        if(m_AllAfterImage.Count<=0 || 
            Vector3.Distance( m_AllAfterImage[m_AllAfterImage.Count-1].Self.transform.position , targetGameOpbject.transform.position) >= m_AfterImageDistance ){

                GameObject newAfterImage = Instantiate(targetGameOpbject ); 
                newAfterImage.transform.SetParent(this.transform);
                newAfterImage.transform.position = targetGameOpbject.transform.position;
                newAfterImage.transform.rotation = targetGameOpbject.transform.rotation;
                newAfterImage.transform.localScale = targetGameOpbject.transform.localScale;

                foreach (var item in newAfterImage.GetComponent<AfterImageEffectTarget>().AllRenderer)
                {
                    if(item.TryGetComponent<Animator>(out var animatorToBeRemove))
                        Destroy(animatorToBeRemove);

                    item.sortingOrder --;
                }

                if(newAfterImage.TryGetComponent<AfterImageEffectTarget>(out var tmp) ){
                    m_AllAfterImage.Add(tmp);
                    for (int i = 0; i < m_AllAfterImage.Count; i++)
                    {
                        float index = i;
                        Color targetColor = m_Gradient.Evaluate( 1 - index/ (float)m_AfterImageCount );

                        foreach (var item in m_AllAfterImage[(int)index].AllRenderer)
                        {
                            item.color = targetColor;
                        }
                    }


                    if(m_AllAfterImage.Count>=m_AfterImageCount){
                        Destroy(m_AllAfterImage[0].gameObject);
                        m_AllAfterImage.RemoveAt(0);
                    }
                }

            }
    }
}
