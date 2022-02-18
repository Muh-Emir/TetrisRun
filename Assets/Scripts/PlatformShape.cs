using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlatformShape : MonoBehaviour
{
    /*
     * Oluşturulan platform
     */

    public GameObject theShape;
    public bool shapeCorrect;
    public Vector3 shapeSize;

    public int rotCounter;

    [SerializeField]
    Color[] shapeColor;

    [SerializeField]
    bool isMoving;
    [SerializeField]
    float moveSpeed;
    [SerializeField]
    Vector3 shapePos;

    [SerializeField]
    bool isTurning;
    [SerializeField]
    float rotateSpeed;
    [SerializeField]
    Vector3[] shapeRot;

    [SerializeField]
    GameObject shapeEffect;

    void Start()
    {
        //Başlangıçta prefab için girilen verilere göre açı ve renk ayarı
        theShape.transform.DOLocalRotate(shapeRot[rotCounter], 0).SetEase(Ease.Linear);
        for (int i = 0; i < theShape.transform.childCount; i++)
        {
            if (theShape.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>() != null)
            {
                theShape.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>().material.color = shapeColor[rotCounter];
            }
            else
            {
                shapeEffect = theShape.transform.GetChild(i).gameObject;
            }
        }
    }

    public void GetDown()
    {
        //DoTween kullanarak döndürme işlemi tamamlanan parçayı vector3 olarak belirlenen konuma taşıma
        if (transform.position != shapePos && !isMoving && !shapeCorrect)
        {
            isMoving = true;
            theShape.transform.DOLocalMove(shapePos, moveSpeed).SetEase(Ease.InSine).OnComplete(() =>
            {
                isMoving = false;
                if (rotCounter == shapeRot.Length-1)
                {
                    shapeCorrect = true;
                    //Platform çözüldüğünde trigger kapanıyor böylece yama işlemi tetiklenmiyor
                    GetComponent<BoxCollider>().enabled = false;
                    shapeEffect.SetActive(true);
                }
                else
                {
                    Debug.Log("Not Correct!");
                }
            });
        }
    }

    public void GetTurn()
    {
        //DoTween kullanarak döndürme işlemi yapılması
        //vector3 listesinde belirlenen rotasyonlara adım sayısına göre dönme işlemi
        if (!isTurning && rotCounter != shapeRot.Length && !shapeCorrect && shapeRot.Length > 0)
        {
            isTurning = true;

            if (rotCounter != shapeRot.Length-1)
                rotCounter++;

            theShape.transform.DOLocalRotate(shapeRot[rotCounter], rotateSpeed).SetEase(Ease.Linear).OnComplete(() => isTurning = false);
            for (int i = 0; i < theShape.transform.childCount; i++)
            {
                if (theShape.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>() != null)
                {
                    theShape.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>().material.DOColor(shapeColor[rotCounter], rotateSpeed);
                }
            }
        }
    }
}