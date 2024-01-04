using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DIALOGUE
{
public class DialogueSystem : MonoBehaviour
{   
    public DialogueContainer dialogueContainer = new DialogueContainer();
    private ConversationManager conversationManager ;
    private TextArchitects architects;

    public static DialogueSystem instance {get; private set;} 

    public delegate void DialogueSystemEvent();
    public event DialogueSystemEvent onUserPrompt_Next;
    public bool isRunningConversation => conversationManager.isRunning; 

    private void Awake() {
        if(instance==null){
            instance= this;   
            initialize(); 
        }else{
            DestroyImmediate(gameObject);
        }
    }

    bool _initialized = false;

    private void initialize(){
        if(_initialized) return;

        architects = new TextArchitects(dialogueContainer.dialogueText);
        conversationManager = new ConversationManager(architects);
    }

    public void OnUserPrompt_Next(){
        onUserPrompt_Next?.Invoke();
    }

    public void ShowSpeakeName(string speakerName =""){

        if(speakerName.ToLower()!="narrator")
            dialogueContainer.nameContainer.Show(speakerName);  
        else
            HideSpeakerName();

    } 
        
    public void HideSpeakerName()=> dialogueContainer.nameContainer.Hide();
    public void Say(string speaker,string dialogue){
       List<string> conversation = new List<string>(){$"{speaker} \"{dialogue}\""};
       Say(conversation);
    }

    public void Say(List<string> conversation){
        conversationManager.StartConversation(conversation);
    }
}
}