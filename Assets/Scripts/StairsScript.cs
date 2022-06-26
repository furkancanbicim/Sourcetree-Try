using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StairsScript : MonoBehaviour
{
    public Text earnedMoneyTxt;
    private void Awake()
    {
        earnedMoneyTxt.text = "$" + PlayerPrefs.GetFloat("earnedMoney", 0.5f) * PlayerPrefs.GetInt("Income",1);
        PlayerPrefs.SetFloat("Money", PlayerPrefs.GetFloat("Money") + PlayerPrefs.GetFloat("earnedMoney",0.5f) * PlayerPrefs.GetInt("Income",1));
        earnedMoneyTxt.GetComponentInParent<Canvas>().transform.LookAt(Camera.main.transform);
    }
    private void Start()
    {
        Invoke("Destroy", 1f);

    }
    void Destroy()
    {
        Destroy(earnedMoneyTxt.GetComponentInParent<Canvas>().gameObject);
    }
}
