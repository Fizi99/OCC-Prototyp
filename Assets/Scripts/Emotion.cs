using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emotion
{
    
    public string tag;
    public float value;
    public float threshhold;
    public float defaultThreshhold;
    public bool positiveEmotion;
    public float percentage;
    public Color color;

    public Emotion(string tag, float value, float defaultThreshhold){

        this.tag = tag;
        this.value = value;
        this.threshhold = defaultThreshhold;
        this.defaultThreshhold = defaultThreshhold;
        this.positiveEmotion = false;
        this.color = Color.gray;
    }

    public Emotion(string tag, float value, float defaultThreshhold, bool positiveEmotion, Color color){

        this.tag = tag;
        this.value = value;
        this.threshhold = defaultThreshhold;
        this.defaultThreshhold = defaultThreshhold;
        this.positiveEmotion = positiveEmotion;
        this.color = color;
    }
    


}
