using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    /*
     * Oyunun genel yönetimi ve level yönetimi
     */

    public int gameLevel;
    public int gameStatus;
    public bool levelCreated;
    public GameObject platformParent;
    [SerializeField]
    GameObject freePlatform;
    [SerializeField]
    GameObject endPlatform;
    [SerializeField]
    List<GameObject> platforms;
    [SerializeField]
    Vector3 firstPlatformPos;
    [SerializeField]
    Vector3 nextPlatformPos;
    [SerializeField]
    int spawnCount;
    [SerializeField]
    string platformTypes = "";

    void Start()
    {
        gameLevel = PlayerPrefs.GetInt("GameLevel");
        nextPlatformPos += firstPlatformPos;
    }

    public void RestartLevels()
    {
        //Tüm oyunu sıfırlayarak ilk levelden başlatma
        PlayerPrefs.SetInt("GameLevel", 0);
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    public void LevelManagement(bool isWin)
    {
        //Kazanma durumuna bağlı olarak leveli yeniden yükleme
        if (isWin)
        {
            PlayerPrefs.SetInt("GameLevel", gameLevel + 1);
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }
    }

    public void CreateLevel()
    {
        //Level oluşturma
        if (!levelCreated && gameLevel > 0)
        {
            /*
             * Her oynanışta her cihaz için aynı levellerin gelmesi
             * ama her levelin de birbirinden farklı olması ve bu işlemin sonsuzlanması için noise kullanımı
             */
            string theLevel = (Mathf.PerlinNoise(gameLevel * 0.035f, 1 * 0.035f) * 10000000).ToString();
            int step = 0;
            foreach (var item in theLevel)
            {
                int digit = System.Convert.ToInt32(item.ToString());
                if (step == 0)
                {
                    //Çekilen noise verisinin ilk basamağını peş peşe kaç şekil olacağını belirlemek için kullandım
                    if (digit < platforms.Count)
                        digit += (platforms.Count - 1);

                    spawnCount = digit;
                }
                else if (step <= spawnCount)
                {
                    if (digit > platforms.Count)
                    {
                        //2. basamaktan itibaren ilk basamakta belirlenen şekil sayısı kadar rakamı alıp eğer şekil çeşitliliğinden büyük ise belirli düzeyde azaltma
                        digit = digit - platforms.Count * (digit / platforms.Count);
                        if (digit < 0)
                            digit = 0;
                    }
                    platformTypes = platformTypes + digit.ToString();
                }
                step++;
            }
            levelCreated = true;
            SpawnPlatform();
        }
        else if (!levelCreated)
        {
            //ilk level için öğretici sistemine elle girilen level
            spawnCount = 3;
            platformTypes = "112";
            levelCreated = true;
            SpawnPlatform();
        }
    }

    void SpawnPlatform()
    {
        //playformların oluşturulması
        GameObject spawnedPlatform;
        GameObject fPlatform;

        //Her platform ile araya 1 adet şekilsiz boş platform atma
        fPlatform = Instantiate(freePlatform, nextPlatformPos, Quaternion.identity, platformParent.transform);
        nextPlatformPos += fPlatform.GetComponent<PlatformShape>().shapeSize;

        foreach (var item in platformTypes)
        {
            int digit = System.Convert.ToInt32(item.ToString());
            if (digit == 0)
            {
                fPlatform = Instantiate(freePlatform, nextPlatformPos, Quaternion.identity, platformParent.transform);
                nextPlatformPos += fPlatform.GetComponent<PlatformShape>().shapeSize;
            }
            else
            {
                spawnedPlatform = Instantiate(platforms[digit - 1], nextPlatformPos, Quaternion.identity, platformParent.transform);
                nextPlatformPos += spawnedPlatform.GetComponent<PlatformShape>().shapeSize;
            }
        }
        Instantiate(endPlatform, nextPlatformPos, Quaternion.identity, platformParent.transform);
    }
}