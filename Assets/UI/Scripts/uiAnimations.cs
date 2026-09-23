using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;

public class uiAnimations : MonoBehaviour
{
    [SerializeField] public Animator MainAnim;
    [SerializeField] public Animator OptAnim;
    public GameObject OptionsScreen;
    public GameObject MenuScreen;

    public void Start()
    {
        Animator mainAnim = MainAnim;
        Animator optAnim = OptAnim;

        mainAnim.GetBool("isClosing");
        optAnim.GetBool("isClosing");
    }

    public void optionsOpen()
    {
        
        MainAnim.SetBool("isClosing", true);
        StartCoroutine(waitTwoSeconds());
    }


    IEnumerator waitTwoSeconds()
    {
        yield return new WaitForSeconds(2);
    }
}
