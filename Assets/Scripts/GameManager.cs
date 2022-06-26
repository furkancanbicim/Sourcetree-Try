using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Variables
    [Header("Game Objects")]
    public GameObject player;
    public GameObject stairPrefab;
    public GameObject scoreBoard;
    public GameObject lastScoreObj;
    public GameObject confetties;

    [Header("Game Objects For Levels")]
    public GameObject lvl1;
    public GameObject lvl2;

    [Header("UI Items")]
    public Text MoneyTxt;
    public Text scoreBoardTxt;
    public Text lastScoreTxt;
    public Text levelText;

    [HideInInspector]
    public GameObject spawnedObj, stepPositionObj;
    float height = 0, rotation = 0;
    [Header("Variables For Level 1")]
    public float levelHeight;
    public float levelStartHeight;
    [Header("Variables For Level 2")]
    public float level2Height;
    public float level2StartHeight;
    float levelHeightProgress;
    [Header("Variables")]
    [Range(0f, 0.1f)]
    public float stepHeight;
    [Range(0f, 30f)]
    public float stepRotation;
    [Range(0f, 1f)]
    public float spawnSpeed;
    float spawnTime = 0, pressTime = 0;
    bool isStarted = true;
    [HideInInspector]
    public bool gameOver;
    public bool finish;
    List<Rigidbody> spawnedObjects = new List<Rigidbody>();
    #endregion
    #region General Methods
    private void Awake()
    {
        if (PlayerPrefs.GetInt("Level") == 1)
        {
            lvl1.SetActive(false);
            lvl2.SetActive(true);
            levelHeight = level2Height;
            levelStartHeight = level2StartHeight;
        }
        levelText.text = "Level " + (PlayerPrefs.GetInt("Level")+1);
    }
    private void Start()
    {
        PlayerPrefs.SetFloat("Money", 9999);
        levelStartHeight = -1 * levelStartHeight;
        levelHeight = -1 * levelHeight;
        levelHeightProgress = levelHeight;
        MoneyTxt.text = string.Format("{0:0.0}", PlayerPrefs.GetFloat("Money"));

        if (PlayerPrefs.GetFloat("Last Height") == 0f)
        {
            lastScoreObj.transform.position = new Vector3(lastScoreObj.transform.position.x, -5, lastScoreObj.transform.position.z);
        }
        else
        {
            lastScoreObj.transform.position = new Vector3(lastScoreObj.transform.position.x, PlayerPrefs.GetFloat("Last Height"), lastScoreObj.transform.position.z);
        }
        scoreBoardTxt.text = "-" + string.Format("{0:0.0}", levelStartHeight) + "m";
        lastScoreTxt.text = "-"+string.Format("{0:0.0}", (PlayerPrefs.GetFloat("Score"))+ levelStartHeight- levelHeight) + "m";
        spawnTime = spawnSpeed;
        spawnSpeed = 0;
    }
    private void Update()
    {
        if(levelHeightProgress<=0)
        {
            Time.timeScale = 1f;
            finish = true;
            confetties.transform.position = new Vector3(0,Camera.main.transform.position.y,0);
            confetties.SetActive(true);
            player.GetComponent<Animator>().SetBool("isFinish",true);
        }
        MoneyTxt.text = string.Format("{0:0.0}", PlayerPrefs.GetFloat("Money"));
        if (player.GetComponent<Character>().isRunning && isStarted&& !gameOver&&!finish)
        {

            pressTime += Time.deltaTime;
            spawnSpeed -= Time.deltaTime;
            if (spawnSpeed <= 0)
            {
                isStarted = false;
                SpawnStap();
                scoreBoard.transform.position = spawnedObj.transform.position + new Vector3(0, 0.25f, 0);
                spawnSpeed = spawnTime;

            }
        }

        else if (!player.GetComponent<Character>().isRunning)
        {
            spawnSpeed = 0;
        }
        //Checking when move key up. Is anim still working?
        if (player.GetComponent<Character>().isRunning && isStarted)
        {

            if (GetCurrentAnimatorTime(player.GetComponent<Animator>()) < 0.45f)
            {
                if (pressTime > 0.5f)
                {//if anim working stop it.
                    player.GetComponent<Animator>().Rebind();

                }
            }
            pressTime = 0;
        }

    }
    #endregion
    #region Spawn Methods
    void SpawnStap()
    {
        if (!gameOver&&!finish)
        {
            if(PlayerPrefs.GetInt("Vibration") ==0)
            Handheld.Vibrate();

            Camera.main.GetComponent<CameraScript>().PlaySound();
            MoneyTxt.text = string.Format("{0:0.0}", PlayerPrefs.GetFloat("Money"));
            scoreBoard.SetActive(true);
            levelHeightProgress -= 0.4f;
            scoreBoardTxt.text = "-" + string.Format("{0:0.0}", levelHeightProgress + levelStartHeight - levelHeight) + "m";
            spawnedObj = Instantiate(stairPrefab, new Vector3(0, height, 0), Quaternion.Euler(0, rotation, 0));
            spawnedObjects.Add(spawnedObj.transform.GetChild(1).GetComponent<Rigidbody>());
            spawnedObj.transform.parent = transform;
            height = height + stepHeight;
            rotation = rotation + stepRotation;
            if (!isStarted)
                Invoke("SpawnSecondStap", spawnTime);
        }
    }
    void SpawnSecondStap()
    {
        stepPositionObj = spawnedObj;
        isStarted = true;
        SpawnStap();
    }
    #endregion
    #region Get Anim Time Method 
    //Methot check how much left current animation time
    public float GetCurrentAnimatorTime(Animator targetAnim, int layer = 0)
    {
        AnimatorStateInfo animState = targetAnim.GetCurrentAnimatorStateInfo(layer);
        float currentTime = animState.normalizedTime % 1;
        return currentTime;
    }
    public void TryAgain()
    {
        stepPositionObj.transform.GetChild(1).GetComponent<Rigidbody>().isKinematic = false;
        stepPositionObj.transform.GetChild(1).GetComponent<Animator>().enabled = false;
        for (int i = spawnedObjects.Count - 1; i >= 0; i--)
        {

            spawnedObjects[i].isKinematic = false;
            spawnedObjects[i].GetComponent<Animator>().enabled = false;
        }
        Invoke("SceneRestart", 1f);
    }
    #endregion
    #region Other Methods
    public void GameOver()
    {
        gameOver = true;
        Time.timeScale = 1f;
        if (levelHeightProgress < PlayerPrefs.GetFloat("Score",100))
        {
            PlayerPrefs.SetFloat("Score", levelHeightProgress);
            PlayerPrefs.SetFloat("Last Height", scoreBoard.transform.position.y);
        }
        
    }
    void SceneRestart()
    {
        SceneManager.LoadScene(0);
    }
    public float GetLvlProgress()
    {
        float currentLvlProgress = ((levelHeight / (levelHeight - levelHeightProgress)) * 100);
        currentLvlProgress = (1 / currentLvlProgress) * 100;

        float lastLvlProgress = (levelHeight / (levelHeight - PlayerPrefs.GetFloat("Score",levelHeight))) * 100;
        lastLvlProgress = (1 / lastLvlProgress) * 100;


        if (currentLvlProgress >= lastLvlProgress)
        {
            return currentLvlProgress;
        }
        else
        {
            return lastLvlProgress;
        }
    }
    #endregion
}
