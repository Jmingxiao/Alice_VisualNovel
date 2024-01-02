using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DIALOGUE
{
    [System.Serializable]
/// <summary>
/// The box that hols the nametext on the screen. Part of the dialogue container.
/// </summary>
public class NameContainer 
{
    [SerializeField] private GameObject root;
    [SerializeField] private TextMeshProUGUI nameText;
    public void Show(string nameToShow = ""){
        root.SetActive(true);

        if(nameToShow != string.Empty){
            nameText.text = nameToShow;
        }
    }
    public void Hide(){
      root.SetActive(false);
    }
}

}