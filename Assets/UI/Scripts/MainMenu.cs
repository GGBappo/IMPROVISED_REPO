using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Animations;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public GameObject OptionsScreen; 
    public GameObject MenuScreen;
    [SerializeField] public Animator MainAnim;
    [SerializeField] public Animator OptAnim;

    public void Start()
    {
        Animator mainAnim = MainAnim;
        Animator optAnim = OptAnim;

        mainAnim.GetBool("isClosing");
        optAnim.GetBool("isClosing");
    }

    public void Play()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void OptionsOpen()
    {
        MainAnim.SetBool("isClosing", true);
        StartCoroutine(closeMain());
        

    }
    public void OptionsClose()
    {
        OptAnim.SetBool("isClosing", true);
        StartCoroutine(closeOption());
        

    }


    public void disableOptions()
    {
        OptionsScreen.SetActive(false);
        MenuScreen.SetActive(true);
        OptAnim.SetBool("isClosing", false);
    } 

    public void disableMain()
    {
        MenuScreen.SetActive(false);
        OptionsScreen.SetActive(true);
        MainAnim.SetBool("isClosing", false);
    }

    IEnumerator closeOption()
    {
        yield return new WaitForSeconds(1);
        disableOptions();
    }

    IEnumerator closeMain()
    {
        yield return new WaitForSeconds(1);
        disableMain();
    }

}
