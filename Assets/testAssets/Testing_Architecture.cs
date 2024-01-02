using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DIALOGUE;
namespace TESTING
{

public class Testing_Architecture : MonoBehaviour
{
    DialogueSystem ds;
    TextArchitects architect;

    string[]lines =  new string[5]
    {
        "This is a random line of dialogue",
        " I wannna say sth random",
        " this world is a crazy place sometimes",
        " dont lose hope, things will get better",
        " balalallalallal."
    };
    // Start is called before the first frame update
    void Start()
    {
        ds = DialogueSystem.instance;
            architect = new TextArchitects(ds.dialogueContainer.dialogueText)
            {
                buildMethod = TextArchitects.BuildMethod.fade,
                speed = 0.5f  
            };
        }

    // Update is called once per frame
    void Update()
    {
        string longline =" I need a lone line but I have no fuking idea what to write so I am just writing shit instead of claiming that stuff are good stuff re not as as good as we concerned but asdfad";
        if(Input.GetKeyDown(KeyCode.Space)){
            if(architect.isBuilding){
                if(!architect.hurryUp)
                    architect.hurryUp =true;
                else
                    architect.ForceComplete();
            }else
            architect.Build(longline);
           // architect.Build(lines[Random.Range(0,lines.Length)]);
        }
        else if(Input.GetKeyDown(KeyCode.A)){
            architect.Append(longline);
            //architect.Append(lines[Random.Range(0,lines.Length)]);
        }
    }
}

}