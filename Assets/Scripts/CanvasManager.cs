using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    #region Variables
    [Header("Panels")]
    public GameObject levelPanel;
    public GameObject finishPanel;
    public GameObject upgradePanel;
    public GameObject gameOverPanel;
    public GameObject rewardFailPanel;
    public GameObject pausePanel;
    public Image levelBar;
    GameManager gameManager;
    [Header("Texts")]
    public Text staminaTxt;
    public Text speedTxt, incomeTxt, staminaPriceTxt, speedPriceTxt, incomePriceTxt;
    [Header("Buttons")]
    public GameObject staminaBtn;
    public GameObject speedBtn, incomeBtn, staminaAdBtn, speedAdBtn, incomeAdBtn;
    [Header("Toggles")]
    public Toggle soundToggle;
    public Toggle vibrationToggle;
    [Header("Variables")]
    public int staminaPrice;
    public int speedPrice, incomePrice, stamina, maxStamina, maxIncome, maxSpeed;

    int currentStaminaPrice, currentSpeedPrice, currentIncomePrice;
    #endregion
    #region General Methods
    private void Start()
    {
        if (PlayerPrefs.GetInt("Speed") == 0)
        {
            PlayerPrefs.SetInt("Speed", 1);
            PlayerPrefs.SetInt("Income", 1);
            PlayerPrefs.SetInt("Stamina", 1);
        }
        pausePanel.SetActive(false);
        soundToggle.isOn = PlayerPrefs.GetInt("Sound") == 0 ? true : false;
        vibrationToggle.isOn = PlayerPrefs.GetInt("Vibration") == 0 ? true : false;
        Camera.main.GetComponent<AudioSource>().mute = !soundToggle.isOn;
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        CheckButtons();
    }
    private void Update()
    {
        levelBar.fillAmount = gameManager.GetLvlProgress();
        if (gameManager.gameOver)
        {
            gameOverPanel.SetActive(true);
        }
        if(gameManager.finish)
        {
            levelPanel.SetActive(false);
            finishPanel.SetActive(true);
        }
        if (gameManager.player.GetComponent<Character>().isRunning)
        {

            upgradePanel.SetActive(false);
        }
    }
    #endregion
    #region UI Methods
    public void Move()
    {
        gameManager.player.GetComponent<Character>().isRunning=true;
    }
    public void Stop()
    {
        gameManager.player.GetComponent<Character>().isRunning = false;
    }
    public void StaminaUp()
    {
        PlayerPrefs.SetInt("Stamina", PlayerPrefs.GetInt("Stamina", 1) + 1);
        PlayerPrefs.SetFloat("Money", PlayerPrefs.GetFloat("Money") - currentStaminaPrice);
        currentStaminaPrice = currentStaminaPrice + staminaPrice;
        gameManager.player.GetComponent<Character>().currentStamina = gameManager.player.GetComponent<Character>().stamina + PlayerPrefs.GetInt("Stamina") * 5;
        CheckButtons();
    }
    public void StaminaAD()
    {
        if (RewardedAd())
        {
            PlayerPrefs.SetInt("Stamina", PlayerPrefs.GetInt("Stamina", 1) + 1);
            currentStaminaPrice = currentStaminaPrice + staminaPrice;
            CheckButtons();
        }
        else
        {
            rewardFailPanel.SetActive(true);
            Invoke("RewardFailPanelOff", 0.5f);
        }
    }

    public void SpeedUp()
    {
        PlayerPrefs.SetInt("Speed", PlayerPrefs.GetInt("Speed", 1) + 1);
        PlayerPrefs.SetFloat("Money", PlayerPrefs.GetFloat("Money") - currentSpeedPrice);
        currentSpeedPrice = currentSpeedPrice + speedPrice;
        CheckButtons();
    }
    public void SpeedAD()
    {
        if (RewardedAd())
        {
            PlayerPrefs.SetInt("Speed", PlayerPrefs.GetInt("Speed", 1) + 1);
            currentSpeedPrice = currentSpeedPrice + speedPrice;
            CheckButtons();
        }
        else
        {
            rewardFailPanel.SetActive(true);
            Invoke("RewardFailPanelOff", 0.5f);
        }
    }

    public void IncomeUp()
    {
        PlayerPrefs.SetInt("Income", PlayerPrefs.GetInt("Income", 1) + 1);
        PlayerPrefs.SetFloat("Money", PlayerPrefs.GetFloat("Money") - currentIncomePrice);
        currentIncomePrice = currentIncomePrice + incomePrice;
        CheckButtons();
    }
    public void IncomeAD()
    {
        if (RewardedAd())
        {
            PlayerPrefs.SetInt("Income", PlayerPrefs.GetInt("Income", 1) + 1);
            currentIncomePrice = currentIncomePrice + incomePrice;
            CheckButtons();
        }
        else
        {
            rewardFailPanel.SetActive(true);
            Invoke("RewardFailPanelOff", 0.5f);
        }
    }

    public void TryAgain()
    {
        gameManager.TryAgain();
    }
    public void ContinueAd()
    {
        if (RewardedAd())
        {
            //Codes for continue
        }
        else
        {
            rewardFailPanel.SetActive(true);
            Invoke("RewardFailPanelOff", 0.5f);
        }

    }
    public void Pause()
    {
        pausePanel.SetActive(pausePanel.activeInHierarchy ? false : true);
        float pause = pausePanel.activeInHierarchy ? 0f : 1f;
        PlayerPrefs.SetFloat("Pause", pause);
        Time.timeScale = pause;
    }
    public void Restart()
    {
        PlayerPrefs.SetFloat("Pause", 1f);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void NextLvl()
    {
        PlayerPrefs.SetFloat("Score", PlayerPrefs.GetInt("Level",0) == 0 ? 1000:1000);
        PlayerPrefs.SetFloat("Last Height",0);
        PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level") == 0 ? 1 : 0);
        Restart();
    }
    public void DeleteSave()
    {
        PlayerPrefs.DeleteAll();
    }
    public void Sound()
    {
        int sound = !soundToggle.isOn ? 1 : 0;
        Camera.main.GetComponent<AudioSource>().mute = !soundToggle.isOn;
        PlayerPrefs.SetInt("Sound", sound);
    }
    public void Vibration()
    {
        int vibration = !vibrationToggle.isOn ? 1 : 0;
        PlayerPrefs.SetInt("Vibration", vibration);
    }
    void CheckButtons()
    {
        currentStaminaPrice = staminaPrice * PlayerPrefs.GetInt("Stamina");
        staminaTxt.text = "LVL " + PlayerPrefs.GetInt("Stamina");
        staminaPriceTxt.text = currentStaminaPrice.ToString();
        currentSpeedPrice = speedPrice * PlayerPrefs.GetInt("Speed");
        speedTxt.text = "LVL " + PlayerPrefs.GetInt("Speed");
        speedPriceTxt.text = currentSpeedPrice.ToString();
        currentIncomePrice = incomePrice * PlayerPrefs.GetInt("Income");
        incomeTxt.text = "LVL " + PlayerPrefs.GetInt("Income");
        incomePriceTxt.text = currentIncomePrice.ToString();
        if (maxStamina > PlayerPrefs.GetInt("Stamina"))
        {
            if (PlayerPrefs.GetFloat("Money") < currentStaminaPrice)
            {
                staminaBtn.SetActive(false);
                staminaAdBtn.SetActive(true);
            }
            else
            {
                staminaAdBtn.SetActive(false);
                staminaBtn.SetActive(true);
            }
        }
        else
        {
            staminaTxt.text = "Max LVL";
            staminaAdBtn.SetActive(false);
            staminaBtn.SetActive(false);
        }
        if (maxSpeed > PlayerPrefs.GetInt("Speed"))
        {
            if (PlayerPrefs.GetFloat("Money") < currentSpeedPrice)
            {
                speedBtn.SetActive(false);
                speedAdBtn.SetActive(true);
            }
            else
            {
                speedAdBtn.SetActive(false);
                speedBtn.SetActive(true);
            }
        }
          
        else
        {
            speedTxt.text = "Max LVL";
            speedAdBtn.SetActive(false);
            speedBtn.SetActive(false);
        }
        if (maxIncome > PlayerPrefs.GetInt("Income"))
        {
            if (PlayerPrefs.GetFloat("Money") < currentIncomePrice)
            {
                incomeBtn.SetActive(false);
                incomeAdBtn.SetActive(true);
            }
            else
            {
                incomeAdBtn.SetActive(false);
                incomeBtn.SetActive(true);
            }
        }
        else
        {
            incomeTxt.text = "Max LVL";
            incomeAdBtn.SetActive(false);
            incomeBtn.SetActive(false);
        }
    }
    void RewardFailPanelOff()
    {
        rewardFailPanel.SetActive(false);
    }
    bool RewardedAd()
    {
        return false;
    }
    #endregion

}
