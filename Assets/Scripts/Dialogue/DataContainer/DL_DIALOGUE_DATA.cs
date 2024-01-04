using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace DIALOGUE{

public class DL_DIALOGUE_DATA
{
    public List<DIALOGUE_SEGMENT> segments = new List<DIALOGUE_SEGMENT>();
    private const string segmentIndentifier = @"\{[ca]\}|\{w[ca]\s\d*\.?\d*\}";
    public DL_DIALOGUE_DATA(string rawDialogue)
    {
       segments = RipSegment(rawDialogue);
    }

    public List<DIALOGUE_SEGMENT> RipSegment(string rawDialogue)
    {
        MatchCollection matches = Regex.Matches(rawDialogue, segmentIndentifier);

        int lastIndex = 0;
        //find the first or onlu segment in the file
        DIALOGUE_SEGMENT segment = new DIALOGUE_SEGMENT();
        segment.dialogue = matches.Count == 0 ? rawDialogue : rawDialogue.Substring(0, matches[0].Index);
        segment.startSignal = DIALOGUE_SEGMENT.StartSignal.NONE;
        segment.signalDelay = 0;
        segments.Add(segment);

        if(matches.Count == 0) return segments;
        else lastIndex = matches[0].Index;

        for(int i =0;i<matches.Count;i++)
        {
            Match match = matches[i];
            segment = new DIALOGUE_SEGMENT();

            // Get the start signal for the segment
            string signalMatch = match.Value;// {A} 
            signalMatch = signalMatch.Substring(1,signalMatch.Length-2);// A
            string [] signalSplit = signalMatch.Split(' ');// [A]

            segment.startSignal = (DIALOGUE_SEGMENT.StartSignal)Enum.Parse(typeof(DIALOGUE_SEGMENT.StartSignal),signalSplit[0].ToUpper());

            if(signalSplit.Length>1)
                float.TryParse(signalSplit[1],out segment.signalDelay);
            
            //Get the dialogue for the segment
            int nextIndex = i+1<matches.Count ? matches[i+1].Index : rawDialogue.Length;
            segment.dialogue = rawDialogue.Substring(lastIndex+match.Length, nextIndex-(lastIndex+match.Length));
            lastIndex = nextIndex;

            segments.Add(segment);
        }

        return segments;
    }

    public struct DIALOGUE_SEGMENT
    {
        public string dialogue;

        public StartSignal startSignal;
        public float signalDelay;

        /// <summary>
        /// c = clear
        /// a = append
        /// wa = wait for append
        /// wc = wait for clear
        /// </summary>
        public enum StartSignal{NONE,C,A,WA,WC}

        public bool appendText => startSignal == StartSignal.A || startSignal == StartSignal.WA;
    }
   
}

}