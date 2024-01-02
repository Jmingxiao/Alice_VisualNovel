using System;
using System.Collections;
using System.Collections.Generic;
using DIALOGUE;
using UnityEngine;

namespace TESTING{

public class TestParsing : MonoBehaviour
{
    //[SerializeField] private TextAsset fileName;
    // Start is called before the first frame update
    void Start()
    {   
        SendFileToParse();
    }

    // Update is called once per frame
    void SendFileToParse()
    {
        List<string> lines = FileManager.ReadTextAsset("test");
        foreach(string line in lines)
        {
            if(line==string.Empty)
                continue;
            DIALOGUELINE parsedLine = DialogueParser.Parse(line);
           
        }
    }
}

}