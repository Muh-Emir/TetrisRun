using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StartPlatform : MonoBehaviour
{
    /*
     *stoklanmış küplerin animasyonlu oluşturulması
     */

    public GameManager gameManager;
    [SerializeField]
    float nextTweenDelay;
    [SerializeField]
    Vector3 scaleSize;

    void Start()
    {
        //Tüm küpleri küçültme
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.transform.localScale = Vector3.zero;
        }
    }

    void Update()
    {
        if (gameManager != null && gameManager.gameStatus == 1 && transform.GetChild(0).gameObject.transform.localScale.x <= 0)
        {
            //Başlangıç objesi ise oyun başlar başlamaz ilk platformu oluşturma
            StartCoroutine(ScaleSet());
        }
        else if(gameManager == null && transform.GetChild(0).gameObject.transform.localScale.x <= 0)
        {
            //şekillli platformlar için oluşturma
            StartCoroutine(ScaleSet());
        }
    }

    IEnumerator ScaleSet()
    {
        /*
         * DoTween kullanarak küplerin büyüyerek gelmesi
         * Küpleri parentinin içindeki sıraya göre oluşturuyor
         * her küpten sonra bekleme süresi var
        */
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.transform.DOScale(scaleSize, nextTweenDelay).OnComplete(()=>
            {
                if (i == transform.childCount - 1 && gameManager != null)
                {
                    gameManager.CreateLevel();
                }
            });
            yield return new WaitForSeconds(.025f);
        }
    }
}