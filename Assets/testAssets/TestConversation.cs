using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DIALOGUE;
public class TestConversation : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]TextAsset Filetoload;
   void Start()
    {   
        StartConversation();
    }

    // Update is called once per frame
    void StartConversation()
    {
        List<string> lines = FileManager.ReadTextAsset(Filetoload);
        // DialogueSystem.instance.Say(lines);

        foreach (string line in lines)
        {
            if(string.IsNullOrWhiteSpace(line)) 
                continue;

            DIALOGUELINE dialogueLine = DialogueParser.Parse(line);
            
            for (int i = 0; i < dialogueLine.commandData.commands.Count; i++)
            {   
                DL_COMMAND_DATA.Command command = dialogueLine.commandData.commands[i];
                Debug.Log($"Command [{i}] '{command.name}' has arguments [{string.Join(", ",command.arguments)}]");
            }
            
        }



    }   
}
