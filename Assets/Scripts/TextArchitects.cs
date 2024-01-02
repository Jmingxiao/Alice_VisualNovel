using System.Collections;
using UnityEngine;
using TMPro;
using System.ComponentModel;

public class TextArchitects 
{
   private TextMeshProUGUI tmpro_ui;
   private TextMeshPro tmpro_world;

   public TMP_Text tmpro =>tmpro_ui !=null? tmpro_ui:tmpro_world;

   public string currentText => tmpro.text; 
   public string targetText{ get; private set;}="";
   public string preText{ get; private set;}= "";
   private int preTextLength =0;
   public string fulltargetText => preText+targetText;
   public enum BuildMethod{ instant, typewriter, fade }
   public BuildMethod buildMethod = BuildMethod.typewriter;
   
   public Color textColor{ get{return tmpro.color;} set{tmpro.color =value;}}

    private const float basespeed = 1;
    private float speedMultiplier = 1;
    public float speed { get{return basespeed * speedMultiplier;}set{ speedMultiplier =value;}}

    public int charavtersPercycle {get{ return speed <=2f? charactermultiplier: speed<=2.5f? charactermultiplier*2: charactermultiplier*3; }}
    private int charactermultiplier =1;
    public bool hurryUp = false;

    public TextArchitects(TextMeshProUGUI tmpro_ui){
        this.tmpro_ui = tmpro_ui;
    }
    public TextArchitects(TextMeshPro tmpro_world){
        this.tmpro_world = tmpro_world;
    }

    public Coroutine Build(string text){
        preText ="";
        targetText = text;
        Stop();
        buildProcess = tmpro.StartCoroutine(Building());
        return buildProcess;
    }

    public Coroutine Append(string text){
        preText = tmpro.text;
        targetText = text;
        Stop();
        buildProcess = tmpro.StartCoroutine(Building());
        return buildProcess;
    }

    private Coroutine buildProcess =null;
    public bool isBuilding =>buildProcess !=null;

    /// <summary>
    /// Append text to what is already in the text
    /// </summary>
    public void Stop(){
        if(!isBuilding)
            return;
        
        tmpro.StopCoroutine(buildProcess);
        buildProcess =null;
    }

    IEnumerator Building(){
        Prepare();

        switch(buildMethod){
            case BuildMethod.fade:
            yield return Build_Fade();
            break;
            case BuildMethod.typewriter:
            yield return Build_TypeWriter();
            break;
        }

        Oncomplete();
    }
    private void Oncomplete()
    {
        buildProcess =null;
        hurryUp =false;
    }
    public void ForceComplete()
    {
         switch(buildMethod){
            case BuildMethod.typewriter:
            tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
            break;
            case BuildMethod.fade:
            tmpro.ForceMeshUpdate();
            break;
        }
        Stop();
        Oncomplete();
    }
    private void Prepare()
    {
        switch(buildMethod){
            case BuildMethod.instant:
            Prepare_instant();
            break;
            case BuildMethod.typewriter:
            Prepare_typeWriter();
            break;
            case BuildMethod.fade:
            Prepare_fade();
            break;
        }
    }
    private void Prepare_instant()
    {
        tmpro.color = tmpro.color;
        tmpro.text = fulltargetText;
        tmpro.ForceMeshUpdate();
        tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
    }
     private void Prepare_typeWriter()
    {
        tmpro.color = tmpro.color;
        tmpro.maxVisibleCharacters = 0;
        tmpro.text = preText;
        if(preText !=""){
            tmpro.ForceMeshUpdate();
            tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
        }
        tmpro.text+= targetText;
        tmpro.ForceMeshUpdate();
    }

    private void Prepare_fade(){
        tmpro.text = preText;
        if(preText != ""){
            tmpro.ForceMeshUpdate();
            preTextLength=tmpro.textInfo.characterCount;
        }else{
            preTextLength =0;
        }

        tmpro.text+=targetText;
        tmpro.maxVisibleCharacters = int.MaxValue;
        tmpro.ForceMeshUpdate();

        TMP_TextInfo texinfo = tmpro.textInfo;

        Color colorVisable = new Color(textColor.r,textColor.g,textColor.b,1);
        Color colorHidden = new Color(textColor.r,textColor.g,textColor.b,0);

        Color32[] vertexColors = texinfo.meshInfo[texinfo.characterInfo[0].materialReferenceIndex].colors32;
        
        for(int i =0; i<texinfo.characterCount; i++){
           var charInfo = texinfo.characterInfo[i];

            if(!charInfo.isVisible)
                continue;

            if(i< preTextLength){
                vertexColors[charInfo.vertexIndex+0] = colorVisable;
                vertexColors[charInfo.vertexIndex+1] = colorVisable;
                vertexColors[charInfo.vertexIndex+2] = colorVisable;
                vertexColors[charInfo.vertexIndex+3] = colorVisable;
            }else{
                vertexColors[charInfo.vertexIndex+0] = colorHidden;
                vertexColors[charInfo.vertexIndex+1] = colorHidden;
                vertexColors[charInfo.vertexIndex+2] = colorHidden;
                vertexColors[charInfo.vertexIndex+3] = colorHidden;
            }
        }
        tmpro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }
    
    private IEnumerator Build_TypeWriter(){
       
        while(tmpro.maxVisibleCharacters<tmpro.textInfo.characterCount){
            tmpro.maxVisibleCharacters += hurryUp? charavtersPercycle *5: charavtersPercycle;
            yield return new WaitForSeconds(0.015f/speed);
        }
    }
    private IEnumerator Build_Fade(){
        int minrange = preTextLength;
        int maxrange = minrange+1;
        byte alphaThreshold = 15;

        TMP_TextInfo textInfo = tmpro.textInfo;

        Color32[] vertexColors = textInfo.meshInfo[textInfo.characterInfo[0].materialReferenceIndex].colors32;
        float[] alphas = new float[textInfo.characterCount];

        while(true)
        {
            float fadespeed = (hurryUp? charavtersPercycle *5: charavtersPercycle)*speed*4f;
            for(int i =minrange; i<maxrange; i++)
            {
                var charInfo = textInfo.characterInfo[i];

                if(!charInfo.isVisible)
                    continue;

                int vertexIndex = textInfo.characterInfo[i].vertexIndex;
                alphas[i] = Mathf.MoveTowards(alphas[i],255,fadespeed);

                for(int v=0; v<4; v++){
                    vertexColors[charInfo.vertexIndex+v].a = (byte)alphas[i];
                }
                if(alphas[i]>=255)
                    minrange++;
            }
            tmpro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            bool lastCharacterIsVisable = !textInfo.characterInfo[maxrange-1].isVisible;
            if(alphas[maxrange-1]>alphaThreshold|| lastCharacterIsVisable)
            {
                if(maxrange<textInfo.characterCount)
                    maxrange++;
                else if(alphas[maxrange-1]>=255 || lastCharacterIsVisable)
                    break;
            }
            yield return new WaitForEndOfFrame();
        }
        
    }

}
