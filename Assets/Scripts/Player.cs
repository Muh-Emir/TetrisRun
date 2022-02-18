using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    /*
     * Player controller
     */

    public GameManager gameManager;
    public float playerSpeed;
    public bool waiting;
    public int trainType;

    public GameObject triggerPlatform;

    [SerializeField]
    GameObject cubeMesh;
    [SerializeField]
    GameObject playerFx;
    [SerializeField]
    GameObject winFx;
    [SerializeField]
    GameObject loseFx;

    //swipe
    Vector2 firstPressPos;
    Vector2 secondPressPos;
    Vector2 currentSwipe;

    void Start()
    {
        //DoTween kullanarak elastik zıplama animasyonu
        PlayerAnim();
    }

    void Update()
    {
        Raycast();
        Swipe();

        if (gameManager.gameStatus == 1 && !waiting)
        {
            transform.Translate(transform.right * Time.deltaTime * -playerSpeed);
        }

        if (gameManager.gameLevel == 0)
        {
            PlayerTraining();
        }

        if (gameManager.gameStatus == 3)
        {
            StartCoroutine(LevelWin());
        }
    }

    void PlayerAnim()
    {
        transform.GetChild(0).gameObject.transform.DOLocalJump(Vector3.zero, .05f, 1, .5f).SetEase(Ease.Linear).OnComplete(PlayerAnim);
    }

    private void Raycast()
    {
        // uzaktan obje tespiti için ray kullanımı
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out hit, Mathf.Infinity))
        {
            //raycast değen objeleri atama
            triggerPlatform = hit.collider.gameObject;
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.left) * hit.distance, Color.yellow);
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.left) * 1000, Color.white);
        }
    }

    private void PlayerTraining()
    {
        //Eğer ilk level ise öğretici olayları
        if (triggerPlatform != null && triggerPlatform.GetComponent<PlatformShape>() != null)
        {
            //uzaklık ayarı
            if (2 > Vector3.Distance(transform.position, triggerPlatform.transform.position) && !triggerPlatform.GetComponent<PlatformShape>().shapeCorrect)
            {
                //Öğretici bekleme
                waiting = true;
                if (triggerPlatform.GetComponent<PlatformShape>().rotCounter < 2)
                {
                    //Öğretici türü dokunma ya da kaydırma
                    trainType = 0;
                }
                else
                {
                    trainType = 1;
                }
            }
            else
            {
                waiting = false;
            }
        }
        else
        {
            waiting = false;
        }
    }

    private void Swipe()
    {
        //Dokunma ya da kaydırma inputları
        if (Input.GetMouseButtonDown(0))
        {
            //ilk basılan pozisyon
            firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (gameManager.gameStatus == 1)
            {
                //parmak kaldırılınca alınan pozisyon
                secondPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

                //ilk ve ikinci input arasındaki fark
                currentSwipe = new Vector2(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);

                //normalize etme
                currentSwipe.Normalize();

                //kaydırma mı dokunma mı olduğunu anlama
                if (currentSwipe.y < 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
                {
                    //swipe down
                    SwipeDown();
                }
                else
                {
                    //press
                    PressTouch();
                }
            }
            else if(gameManager.gameStatus == 0)
            {
                //oyunu başlatma
                gameManager.gameStatus = 1;
            }
        }
    }

    void SwipeDown()
    {
        //kaydırma işleminin ray ile tespit edilen platforma iletimi
        if (triggerPlatform != null && triggerPlatform.GetComponent<PlatformShape>() != null)
        {
            triggerPlatform.GetComponent<PlatformShape>().GetDown();
            waiting = false;
        }
    }

    void PressTouch()
    {
        //dokunma işleminin ray ile tespit edilen platforma iletimi
        if (triggerPlatform != null && triggerPlatform.GetComponent<PlatformShape>() != null)
        {
            triggerPlatform.GetComponent<PlatformShape>().GetTurn();
        }
    }

    private IEnumerator LevelWin()
    {
        //Kazanma efekti
        winFx.SetActive(true);
        //bekleme
        yield return new WaitForSeconds(2);
        //Yeni levele geçme
        gameManager.LevelManagement(true);
    }

    private IEnumerator LevelLose()
    {
        //patlama efekti
        cubeMesh.transform.DOScale(Vector3.zero, 0.3f);
        playerFx.SetActive(false);
        //kaybetme efekti
        loseFx.SetActive(true);
        //bekleme
        yield return new WaitForSeconds(2);
        //Aynı leveli tekrarlama
        gameManager.LevelManagement(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "FinishPlatform":
                //Son platforma değip bitirme kısmı
                gameManager.gameStatus = 3;
                break;

            case "Platform":
                //Eğer platforma trigger olarak değerse yanma işlemi
                //Değmeden önce platform çözülürse trigger kapanıyor
                if (other.gameObject.GetComponent<PlatformShape>() != null && !other.gameObject.GetComponent<PlatformShape>().shapeCorrect)
                {
                    SwipeDown();
                    gameManager.gameStatus = 2;
                    StartCoroutine(LevelLose());
                }
                break;
        }
    }
}