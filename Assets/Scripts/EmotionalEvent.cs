using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmotionalEvent
{
    
    public string tag;
    public float congruence;

    public int count;
    public float lastFire;

    public List<string> targets = new List<string>();

    public EmotionalEvent(string tag){

        this.tag = tag;
        this.congruence = 0;
        this.count = 1;
        this.lastFire = 0f;
    }

    public EmotionalEvent(string tag, string target){

        this.tag = tag;
        this.congruence = 0;
        this.targets.Add(target);
        this.count = 1;
        this.lastFire = 0f;
    }

    public void EventFired()
    {
        lastFire = 0f;
        count++;
    }
   
}
