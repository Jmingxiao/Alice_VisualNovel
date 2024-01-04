using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
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
            dialogueSystem.ShowSpeakeName(line.speakerData.displayName);

        yield return BuildLineSegments(line.dialogueData);
        // wait for user input
        yield return WaitForUserInput();
    }

    IEnumerator Line_RunCommands(DIALOGUELINE line){
        Debug.Log(line.commandData);
        yield return null;
    }

    IEnumerator BuildLineSegments(DL_DIALOGUE_DATA line){
        for(int i = 0; i < line.segments.Count; i++){
            DL_DIALOGUE_DATA.DIALOGUE_SEGMENT segment = line.segments[i];

            yield return WaitForDialogueSegementSignalToBeTriggered(segment);
            yield return BuildDialogue(segment.dialogue,segment.appendText);
        }
    }

    IEnumerator WaitForDialogueSegementSignalToBeTriggered(DL_DIALOGUE_DATA.DIALOGUE_SEGMENT segment){
        switch(segment.startSignal){
            case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.C:
            case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.A:
                yield return WaitForUserInput();
                break;
            case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.WC:
            case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.WA:
                yield return new WaitForSeconds(segment.signalDelay);
                break;
            default:
                break;
        }
    }

    IEnumerator BuildDialogue(string dialogue,bool append = false){
        //build the dialogue
        if(!append)
            architects.Build(dialogue);
        else
            architects.Append(dialogue);

        //wait for the dialogue to finish building
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