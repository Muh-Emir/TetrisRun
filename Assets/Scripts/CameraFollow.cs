using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraFollow : MonoBehaviour
{
    /*
     * Kameranın playeri takip etmesi
     */
    public GameObject player;
    public GameManager gameManager;
    public Vector3 offset;
    public float lerpSpeed;

    [SerializeField]
    GameObject startScreen;
    [SerializeField]
    Text levelText;

    [SerializeField]
    GameObject winningFx;

    [SerializeField]
    GameObject trainer;
    [SerializeField]
    GameObject trainerHand;
    [SerializeField]
    Text trainText;

    Animator handAnimator;

    void Start()
    {
        offset = transform.position - player.transform.position;
        trainer.SetActive(false);
        handAnimator = trainerHand.GetComponent<Animator>();
    }

    void Update()
    {
        startScreen.SetActive(false);
        
        switch (gameManager.gameStatus)
        {
            case 0:
                //Başlangıç ekranı kısmı
                startScreen.SetActive(true);
                levelText.text = "LVL " + (gameManager.gameLevel + 1);
                break;
            case 1:
                PlayerFollow();
                break;
            case 3:
                winningFx.SetActive(true);
                break;
        }
    }

    private void PlayerFollow()
    {
        //Playeri takip etme
        Vector3 vector3Pos = Vector3.Lerp(transform.position, player.transform.position + offset, lerpSpeed);

        transform.position = vector3Pos;


        if (player.GetComponent<Player>().waiting)
        {
            trainer.SetActive(true);

            switch (player.GetComponent<Player>().trainType)
            {
                case 0:
                    trainText.text = "TAP TO ROTATE";
                    handAnimator.Play("TapAnim");
                    break;

                case 1:
                    trainText.text = "SWIPE DOWN";
                    handAnimator.Play("SwipeAnim");
                    break;
            }
        }
        else
        {
            trainer.SetActive(false);
        }
    }
}
