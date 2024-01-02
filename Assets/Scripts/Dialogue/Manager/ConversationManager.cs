using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DIALOGUE{

public class ConversationManager 
{
    private DialogueSystem dialogueSystem = DialogueSystem.instance;
    private Coroutine process = null;
    public bool isRunning => process != null;
    
    private TextArchitects architects = null;
    private bool userPromt = false;

    public ConversationManager(TextArchitects architects){
        this.architects = architects;
        dialogueSystem.onUserPrompt_Next += OnUserPrompt_Next;
    }

    void OnUserPrompt_Next(){
        userPromt = true;
    }

    public void StartConversation(List<string> conversation){
        StopConversation();

        process = dialogueSystem.StartCoroutine(RunningConversation(conversation));
    }

    public void StopConversation(){
        if(!isRunning) return;

        dialogueSystem.StopCoroutine(process);
        process = null;
    }
    IEnumerator RunningConversation(List<string> conversation){
        for(int i = 0; i < conversation.Count; i++){
            //Dont show empty lines
            if(string.IsNullOrWhiteSpace(conversation[i]))
                continue;
            DIALOGUELINE parsedLine = DialogueParser.Parse(conversation[i]);
            //show dialogue
            if(parsedLine.hasDialogue)
                yield return Line_RunDialogue(parsedLine);
            
            if(parsedLine.hasCommands)
                yield return Line_RunCommands(parsedLine);
        }
    }

    IEnumerator Line_RunDialogue(DIALOGUELINE line){
        //show or hide speaker name if there is one present
        if(line.hasSpeaker)
            dialogueSystem.ShowSpeakeName(line.speaker);
        else
            dialogueSystem.HideSpeakerName();

        yield return BuildDialogue(line.dialogue);
        // wait for user input
        yield return WaitForUserInput();
    }

    IEnumerator Line_RunCommands(DIALOGUELINE line){
        Debug.Log(line.commands);
        yield return null;
    }
    IEnumerator BuildDialogue(string dialogue){
        architects.Build(dialogue);

        while(architects.isBuilding)
        {
            if(userPromt){
                if(!architects.hurryUp)
                    architects.hurryUp = true;
                else
                    architects.ForceComplete();
                
                userPromt = false;
            }
            yield return null;
        }
    }
    IEnumerator WaitForUserInput(){
        while(!userPromt)
            yield return null;
        userPromt = false;
    }
}

}