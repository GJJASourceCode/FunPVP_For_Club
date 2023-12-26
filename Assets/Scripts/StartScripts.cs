using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartScripts : MonoBehaviour
{
    public Button printButton;

    void Start()
    {
        if(printButton != null)
            printButton.onClick.AddListener(() => {Debug.Log($"PrintBotton을 클릭함");});
            printButton.onClick.AddListener(() => {SceneManager.LoadScene("Select_Scenes");});
            
    }


    void Update()
    {
        
    }
}
