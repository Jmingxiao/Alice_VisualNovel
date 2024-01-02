using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DIALOGUE;
public class TestConversation : MonoBehaviour
{
    // Start is called before the first frame update
   void Start()
    {   
        StartConversation();
    }

    // Update is called once per frame
    void StartConversation()
    {
        List<string> lines = FileManager.ReadTextAsset("test");
        DialogueSystem.instance.Say(lines);
        
    }
}
