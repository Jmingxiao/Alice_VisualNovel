using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class FileManager 
{
    public static List<string> ReadTextFile(string path , bool includeBlankLines = true)
    {
        if(!path.StartsWith('/'))
        {
            path = FilePaths.root + path;
        }
        List<string> lines = new List<string>();
        try
        {
            using (StreamReader sr = new StreamReader(path))
            {
                while(!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if(includeBlankLines || !string.IsNullOrEmpty(line))
                    {
                        lines.Add(line);
                    }
                }
            }
        }
        catch (FileNotFoundException e)
        {
            Debug.LogError($"File not found:'{e.FileName}'");
        }
        return lines;
    }

    public static List<string> ReadTextAsset(string path, bool includeBlankLines = true)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(path);
        if(textAsset == null)
        {
            Debug.LogError($"Asset not found:'{path}'");
            return null;
        }
        return ReadTextAsset(textAsset,includeBlankLines);
    }

    public static List<string> ReadTextAsset(TextAsset textAsset,bool includeBlankLines = true)
    {
        List<string> lines = new List<string>();
        using (StringReader sr = new StringReader(textAsset.text))
        {
            while(sr.Peek() > -1)
            {
                string line = sr.ReadLine();
                if(includeBlankLines || !string.IsNullOrEmpty(line))
                {
                    lines.Add(line);
                }
            }
        }
        return lines;
    }
}
