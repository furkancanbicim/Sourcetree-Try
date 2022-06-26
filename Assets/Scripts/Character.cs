using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    #region Variables
    [Header("Game Objects")]
    public GameManager gameManager;
    public GameObject body;
    [Header("Particles")]
    public ParticleSystem destroy;
    public ParticleSystem sweat;
    [Header("Variables")]
    public float stamina;
    public float currentStamina;
    [HideInInspector]
    public bool isRunning, gameOver = false;
    [Header("Animations")]
    public AnimationClip firstTiredAnim;
    public AnimationClip secondTiredAnim;
    public AnimationClip thirdTiredAnim;
    public Animation anim;
    [HideInInspector]
    public Animator animator;
    #endregion
    #region General Methods
    private void Start()
    {
        destroy.Stop();
        sweat.Stop();
        animator = GetComponent<Animator>();
        stamina = stamina + PlayerPrefs.GetInt("Stamina") * 5;
        currentStamina = stamina;
        anim.Stop();
    }
    void Update()
    {
        if (gameOver && body.activeInHierarchy)
        {
            body.SetActive(false);
            destroy.Play();
            anim.Stop();
        }
        if (currentStamina / (stamina) * 100 <= 40 && !gameOver && !gameManager.finish)
        {
            StartCoroutine(changeColorToRed());
            sweat.Play();
            sweat.maxParticles = currentStamina / (stamina) * 100 >= 30 ? currentStamina / (stamina) * 100 >= 20 ? currentStamina / (stamina) * 100 >= 10 ? 1 : 4 : 7 : 10;
            anim.Play();
            AnimationClip clip = currentStamina / (stamina) * 100 >= 30 ? (firstTiredAnim) : (currentStamina / (stamina) * 100 >= 20 ? (secondTiredAnim) : (thirdTiredAnim));
            anim.clip = clip;
        }
        else
        {
            anim.Stop();
            sweat.Stop();
        }
        //Character move
        if (gameManager.stepPositionObj && !gameOver && !gameManager.finish)
        {
            transform.position = Vector3.Lerp(
                          transform.position,
                          gameManager.stepPositionObj.transform.GetChild(1).transform.GetChild(0).position,
                          Time.deltaTime * 8f);

            transform.LookAt(new Vector3(gameManager.spawnedObj.transform.GetChild(1).transform.position.x,
            transform.position.y, gameManager.spawnedObj.transform.GetChild(1).transform.position.z));
        }
        if (isRunning && PlayerPrefs.GetFloat("Pause", 1f) == 1f)
        {
            float speed = PlayerPrefs.GetInt("Speed");
            speed = 1f + speed / 10;
            Time.timeScale = PlayerPrefs.GetInt("Speed") == 1 ? 1f : speed;
            currentStamina -= Time.deltaTime * 5f;
        }
        else if (!gameOver && !gameManager.finish && currentStamina <= stamina / 2 && PlayerPrefs.GetFloat("Pause", 1f) == 1f)
        {
            Time.timeScale = 1f;
            currentStamina += Time.deltaTime * 2f;
        }
        if (currentStamina <= 0 && !gameOver && !gameManager.finish)
        {
            gameManager.GameOver();
            anim.Stop();
            sweat.Stop();
            gameOver = true;
            isRunning = false;
        }

        if (isRunning && !gameOver && !gameManager.finish)
        {
            animator.SetBool("isRunning", true);
            isRunning = true;
        }
        else if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Running Upstairs"))
        {
            animator.SetBool("isRunning", false);
        }
        else
        {
            if (transform.GetChild(1).GetComponent<Renderer>().material.mainTextureScale.y >= 0.2f && currentStamina >= (stamina * 20) / 100)
            {
                var tilingY = transform.GetChild(1).GetComponent<Renderer>().material.mainTextureScale.y;
                tilingY -= 0.001f;
                transform.GetChild(1).GetComponent<Renderer>().material.mainTextureScale = new Vector2(-2, tilingY);
            }
            isRunning = false;
            animator.SetBool("isRunning", false);
        }
    }
    #endregion
    IEnumerator changeColorToRed()
    {

        while (transform.GetChild(1).GetComponent<Renderer>().material.mainTextureScale.y <= 0.29f)
        {
            yield return new WaitForSeconds(0.08f);
            var tilingY = transform.GetChild(1).GetComponent<Renderer>().material.mainTextureScale.y;
            tilingY += 0.0001f;
            transform.GetChild(1).GetComponent<Renderer>().material.mainTextureScale = new Vector2(-2, tilingY);
        }
    }
    

}
