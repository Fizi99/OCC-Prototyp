using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class EmotionEngine : MonoBehaviour
{
    
    private GameManager gm;

    public List<Goal> goals;
    public List<Relationship> relationships;
    public EventSheet eventSheet;
    [HideInInspector]
    public string npcName;

    public GameObject happyForNpc;
    public GameObject sorryForNpc;
    public GameObject resentmentNpc;
    public GameObject schadenfreudeNpc;

    private float hopeFearFactor = 0.5f;

    public Emotion joy;
    public Emotion distress;
    public Emotion happyFor;
    public Emotion sorryFor;
    public Emotion resentment;
    public Emotion schadenfreude;
    public Emotion hope;
    public Emotion fear;
    public Emotion gratification;
    public Emotion fearsConfirmed;

    private Emotion neutral = new Emotion("neutral", 0.00001f, 0f);

    public List<Emotion> emotions = new List<Emotion>();


    private void Start(){

        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        eventSheet = gm.GetComponent<EventSheet>();

        
        joy = new Emotion("joy",0f, gm.GenerateRandom(0.2f), true, new Color32(237, 233, 57, 255));
        distress = new Emotion("distress",0f, gm.GenerateRandom(0.2f), false, new Color32(212, 65, 76, 255));
        happyFor = new Emotion("happyFor", 0f, gm.GenerateRandom(0.2f), true, new Color32(12, 144, 74, 255));
        sorryFor = new Emotion("sorryFor", 0f, gm.GenerateRandom(0.2f), false, new Color32(42, 170, 214, 255));
        resentment = new Emotion("resentment", 0f, gm.GenerateRandom(0.2f), false, new Color32(204, 205, 35, 255));
        schadenfreude = new Emotion("schadenfreude", 0f, gm.GenerateRandom(0.2f), true, new Color32(171, 51, 88, 255));
        hope = new Emotion("hope", 0f, gm.GenerateRandom(0.2f), true, new Color32(96, 115, 201, 255));
        fear = new Emotion("fear", 0f, gm.GenerateRandom(0.2f), false, new Color32(0, 0, 0, 255));
        gratification = new Emotion("gratification", 0f, gm.GenerateRandom(0.2f), true, new Color32(237, 233, 57, 255));
        fearsConfirmed = new Emotion("fearsConfirmed", 0f, gm.GenerateRandom(0.2f), false, new Color32(212, 65, 76, 255));

        emotions.Add(neutral);

        emotions.Add(joy);
        emotions.Add(distress);
        emotions.Add(happyFor);
        emotions.Add(sorryFor);
        emotions.Add(resentment);
        emotions.Add(schadenfreude);
        emotions.Add(hope);
        emotions.Add(fear);
        emotions.Add(gratification);
        emotions.Add(fearsConfirmed);

    }

    private void Update(){

        DecayAllEmotions();
        foreach(Goal g in goals){
        
            foreach(EmotionalEvent e in eventSheet.eventList){
                EvaluateHope(g, e);
                EvaluateFear(g, e);
            }
        }
        UpdateEmotionPercentage();
    }


    public void UpdateGoals(List<Goal> goals){
        this.goals = goals;
    }

    /// <summary>
    /// Evaluate an event in perspective to specified goal.
    /// </summary>
    /// <param name="g"></param>
    /// <param name="e"></param>
    public void EvaluateEvent(Goal g, EmotionalEvent e){

        EmotionalEvent eventCopy = e;
        //eventCopy.congruence = CalcCongruence(g, e);

        EvaluateJoy(g, eventCopy);
        EvaluateDistress(g, eventCopy);
        EvaluateHappyFor(g, eventCopy);
        EvaluateSorryFor(g, eventCopy);
        EvaluateResentment(g, eventCopy);
        EvaluateSchadenfreude(g, eventCopy);
        EvaluateGratification(g, eventCopy);
        EvaluateFearsConfirmed(g, eventCopy);
    }

    /// <summary>
    /// Return current highest emotion.
    /// </summary>
    /// <returns></returns>
    public Emotion CurrentEmotion(){

        float highest = 0;
        Emotion output = neutral;
        foreach(Emotion e in emotions){
            if (e.value > highest){
                highest = e.value;
                output = e;
            }
        }


        return output;
    }

    public void UpdateEmotionPercentage()
    {
        float sum = 0f;
        foreach (Emotion e in emotions)
        {
            sum += e.value;
        }
        foreach (Emotion e in emotions)
        {
            e.percentage = (e.value / sum) * 100;
        }
    }

    /// <summary>
    /// Evaluation for joy Emotion.
    /// </summary>
    /// <param name="g"></param>
    /// <param name="e"></param>
    private void EvaluateJoy(Goal g, EmotionalEvent e)
    {
        if (g.actors.Contains(e.targets[0]) && e.targets[0] == npcName)
        {

            if (CalcDesirability(g, CalcCongruence(g, e)) > 0)
            {

                float joyPotential = Math.Abs(CalcDesirability(g, CalcCongruence(g, e)));
                if (joyPotential > joy.threshhold && joy.value < joyPotential - joy.threshhold)
                {
                    joy.value = joyPotential - joy.threshhold;
                    UpdateThreshholds(joy.value, true);
                }
            }

        }
    }

    /// <summary>
    /// Evaluation for Distress Emotion
    /// </summary>
    /// <param name="g"></param>
    /// <param name="e"></param>
    private void EvaluateDistress(Goal g, EmotionalEvent e){

        if (g.actors.Contains(e.targets[0])&& e.targets[0] == npcName)
        {

            if (CalcDesirability(g, CalcCongruence(g, e)) < 0)
            {

                float distressPotential = Math.Abs(CalcDesirability(g, CalcCongruence(g, e)));
                if (distressPotential > distress.threshhold && distress.value < distressPotential - distress.threshhold)
                {
                    distress.value = distressPotential - distress.threshhold;
                    UpdateThreshholds(distress.value, false);
                }
            }
        }
    }

    /// <summary>
    /// Evaluate Happy for emotion
    /// </summary>
    /// <param name="g"></param>
    /// <param name="e"></param>
    private void EvaluateHappyFor(Goal g, EmotionalEvent e){

        if(g.actors.Contains(e.targets[0]) && e.targets[0] != npcName){

            if(CalcDesirability(g, CalcCongruence(g, e)) > 0){
                
                NpcAi npc = gm.SearchNpc(e.targets[0]);
                float cong = 0;
                float desire = 0;
                foreach (Goal goal in npc.goals){
                    cong = npc.emotionEngine.CalcCongruenceForSelf(goal, e);
                    desire = Math.Max(npc.emotionEngine.CalcDesirability(goal, cong), desire);
                }

                if (desire > 0){

                    float happyForPotential = (Math.Abs(CalcDesirability(g, CalcCongruence(g, e))) + CalcLiking(e) + CalcDeserving(e)) / 3;
                    if(happyForPotential > happyFor.threshhold && happyFor.value < happyForPotential - happyFor.threshhold)
                    {
                        happyForNpc = gm.SearchNpc(e.targets[0]).gameObject;
                        happyFor.value = happyForPotential - happyFor.threshhold;
                        UpdateThreshholds(happyFor.value, true);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Evaluates Sorry For emotion
    /// </summary>
    /// <param name="g"></param>
    /// <param name="e"></param>
    private void EvaluateSorryFor(Goal g, EmotionalEvent e)
    {

        if (g.actors.Contains(e.targets[0]) && e.targets[0] != npcName)
        {
            
            if (CalcDesirability(g, CalcCongruence(g, e)) < 0)
            {

                NpcAi npc = gm.SearchNpc(e.targets[0]);
                float cong = 0;
                float desire = 0;
                foreach (Goal goal in npc.goals)
                {
                    cong = npc.emotionEngine.CalcCongruenceForSelf(goal, e);
                    desire = Math.Min(npc.emotionEngine.CalcDesirability(goal, cong), desire);
                }

                if (desire < 0)
                {

                    float sorryForPotential = (Math.Abs(CalcDesirability(g, CalcCongruence(g, e))) + CalcLiking(e) + (1 - CalcDeserving(e))) / 3;

                    if (sorryForPotential > sorryFor.threshhold && sorryFor.value < sorryForPotential - sorryFor.threshhold)
                    {
                        sorryForNpc = gm.SearchNpc(e.targets[0]).gameObject;
                        sorryFor.value = sorryForPotential - sorryFor.threshhold;
                        UpdateThreshholds(sorryFor.value, false);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Evaluates resentment emotion.
    /// </summary>
    /// <param name="g"></param>
    /// <param name="e"></param>
    private void EvaluateResentment(Goal g, EmotionalEvent e)
    {

        if (g.actors.Contains(e.targets[0]) && e.targets[0] != npcName)
        {

            if (CalcDesirability(g, CalcCongruence(g, e)) < 0)
            {

                NpcAi npc = gm.SearchNpc(e.targets[0]);
                float cong = 0;
                float desire = 0;
                foreach (Goal goal in npc.goals){
                    cong = npc.emotionEngine.CalcCongruenceForSelf(goal, e);
                    desire = Math.Max(npc.emotionEngine.CalcDesirability(goal, cong), desire);
                }

                if (desire > 0)
                {

                    float resentmentPotential = (Math.Abs(CalcDesirability(g, CalcCongruence(g, e))) + (1- CalcLiking(e)) + (1 - CalcDeserving(e))) / 3;

                    if (resentmentPotential > resentment.threshhold && resentment.value < resentmentPotential - resentment.threshhold)
                    {
                        resentmentNpc = gm.SearchNpc(e.targets[0]).gameObject;
                        resentment.value = resentmentPotential - resentment.threshhold;
                        UpdateThreshholds(resentment.value, false);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Evaluates Schadenfreude emotion.
    /// </summary>
    /// <param name="g"></param>
    /// <param name="e"></param>
    private void EvaluateSchadenfreude(Goal g, EmotionalEvent e)
    {

        if (g.actors.Contains(e.targets[0]) && e.targets[0] != npcName)
        {

            if (CalcDesirability(g, CalcCongruence(g, e)) > 0)
            {

                NpcAi npc = gm.SearchNpc(e.targets[0]);
                float cong = 0;
                float desire = 0;
                foreach (Goal goal in npc.goals)
                {
                    cong = npc.emotionEngine.CalcCongruenceForSelf(goal, e);
                    desire = Math.Min(npc.emotionEngine.CalcDesirability(goal, cong), desire);
                }

                if (desire < 0)
                {

                    float schadenfreudePotential = (Math.Abs(CalcDesirability(g, CalcCongruence(g, e))) + (1 - CalcLiking(e)) + CalcDeserving(e)) /3;

                    if (schadenfreudePotential > schadenfreude.threshhold && schadenfreude.value < schadenfreudePotential - schadenfreude.threshhold)
                    {
                        schadenfreudeNpc = gm.SearchNpc(e.targets[0]).gameObject;
                        schadenfreude.value = schadenfreudePotential - schadenfreude.threshhold;
                        UpdateThreshholds(schadenfreude.value, true);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Evaluates Hope emotion. Is called every frame.
    /// </summary>
    /// <param name="g"></param>
    /// <param name="e"></param>
    private void EvaluateHope(Goal g, EmotionalEvent e){

        if (CompareStrings(g.actors, e.targets))
        {
            if (CalcDesirability(g, CalcCongruence(g, e)) > 0)
            {

                float hopePotential = ((Math.Abs(CalcDesirability(g, CalcCongruence(g, e))) + CalcLikelihood(g, e)) * 0.5f) - hopeFearFactor;

                if (hopePotential > hope.threshhold && hope.value < hopePotential - hope.threshhold)
                {
                    if(g.utility > 0)
                    {
                        g.envisaging = hopePotential - hope.threshhold;
                    }
                    hope.value = hopePotential - hope.threshhold;

                    //UpdateThreshholds(hope.value, true);
                }
            }
        }
    }

    /// <summary>
    /// Evaluates Fear emotion. Is called every frame.
    /// </summary>
    /// <param name="g"></param>
    /// <param name="e"></param>
    private void EvaluateFear(Goal g, EmotionalEvent e)
    {

        if (CompareStrings(g.actors, e.targets))
        {
            if (CalcDesirability(g, CalcCongruence(g, e)) < 0)
            {

                float fearPotential = ((Math.Abs(CalcDesirability(g, CalcCongruence(g, e))) + CalcLikelihood(g, e)) * 0.5f) - hopeFearFactor;

                if (fearPotential > fear.threshhold && fear.value < fearPotential - fear.threshhold)
                {
                    if(g.utility < 0){
                        g.envisaging = -1* (fearPotential - fear.threshhold);
                    }
                    fear.value = fearPotential - fear.threshhold;
                }
            }
        }
    }

    /// <summary>
    /// Evaluates Gratification emotion, which occurs, when a hoped for event happens
    /// </summary>
    /// <param name="g"></param>
    /// <param name="e"></param>
    private void EvaluateGratification(Goal g, EmotionalEvent e){

        if (g.actors.Contains(e.targets[0]))
        {
            if (g.envisaging > 0)
            {
                float gratificationPotential = (Math.Abs(CalcDesirability(g, CalcCongruence(g, e))) + g.envisaging) * 0.5f;
                if (gratificationPotential > gratification.threshhold && gratification.value < gratificationPotential - gratification.threshhold)
                {
                    g.envisaging = 0;
                    gratification.value = gratificationPotential - gratification.threshhold;
                    UpdateThreshholds(gratification.value, true);
                }
            }

        }
    }

    /// <summary>
    /// Evaluates Fears Confirmed emotion, which occurs, when a feared event happens
    /// </summary>
    /// <param name="g"></param>
    /// <param name="e"></param>
    private void EvaluateFearsConfirmed(Goal g, EmotionalEvent e)
    {

        if (g.actors.Contains(e.targets[0]))
        {
            if (g.envisaging < 0)
            {
                float fearsConfirmedPotential = (Math.Abs(CalcDesirability(g, CalcCongruence(g, e))) - g.envisaging) * 0.5f;
                if (fearsConfirmedPotential > fearsConfirmed.threshhold && fearsConfirmed.value < fearsConfirmedPotential - fearsConfirmed.threshhold)
                {
                    g.envisaging = 0;
                    fearsConfirmed.value = fearsConfirmedPotential - fearsConfirmed.threshhold;
                    UpdateThreshholds(fearsConfirmed.value, false);
                }
            }

        }
    }

    /// <summary>
    /// Check if an event is congruent with a goal
    /// </summary>
    /// <param name="g">the goal that is checked against</param>
    /// <param name="e">the event that is checked</param>
    /// <returns></returns>
    public float CalcCongruence(Goal g, EmotionalEvent e){

        float output = 0;
        if(g.tag == e.tag){
            output = 1;
        }else{
            output = 0;
        }
        return output;
    }

    /// <summary>
    /// Calculate congruence only for goals that target oneself
    /// </summary>
    /// <param name="g"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    public float CalcCongruenceForSelf(Goal g, EmotionalEvent e){

        if (!g.actors.Contains(npcName))
        {
            return 0;
        }

        float output = 0;
        if (g.tag == e.tag)
        {
            output = 1;
        }
        else
        {
            output = 0;
        }
        return output;
    }

    /// <summary>
    /// How Desirable is an event
    /// </summary>
    /// <param name="g"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    public float CalcDesirability(Goal g, float congruence){

        return congruence * g.utility;
    }

    /// <summary>
    /// search for liking of npc in relationship.
    /// </summary>
    /// <param name="g"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    private float CalcLiking(EmotionalEvent e){

        return SearchRelationship(e.targets[0]).liking;
    }

    /// <summary>
    /// Calculate if another Npc is deserving a event. One deserves an event if the current emotion is of opposite polarity to predicted emotion. Strength of deserving is calculated with strength of current emotion.
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    private float CalcDeserving(EmotionalEvent e){

        float output = 0;
        //congruence and desirability of event for target npc
        float cong = 0;
        float desire = 0;

        NpcAi npc = gm.SearchNpc(e.targets[0]);
        foreach (Goal goal in npc.goals)
        {
            cong = npc.emotionEngine.CalcCongruence(goal, e);
            desire = npc.emotionEngine.CalcDesirability(goal, cong);
        }

        if (!npc.GetCurrentEmotion().positiveEmotion && desire > 0)
        {

            output = npc.GetCurrentEmotion().value;
        }
        else if (npc.GetCurrentEmotion().positiveEmotion && desire < 0)
        {
            output = npc.GetCurrentEmotion().value;
        }

        return output;
    }

    /// <summary>
    /// Calculate likelihood of an event happening, looking at events that happened before. Uses targets and tag of old events.
    /// </summary>
    /// <param name="g"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    private float CalcLikelihood(Goal g, EmotionalEvent e)
    {

        float likelihoodEvent = eventSheet.CalcLikelihoodOfEvent(e);

        float likelihoodTarget = 0;
        float count = 0;
        foreach (string s in g.actors)
        {
            count++;
            likelihoodTarget += eventSheet.CalcLikelihoodForTarget(e, s);
        }
        likelihoodTarget = likelihoodTarget / count;
        
        return (likelihoodTarget + likelihoodEvent) * 0.5f;
    }

    /// <summary>
    /// Update threshholds for all emotions to simulate overall mood. Positive emotions lower Threshhold for positive emotions and other way around.
    /// </summary>
    /// <param name="difference"></param>
    /// <param name="positive"></param>
    private void UpdateThreshholds(float difference, bool positive){

        if(positive){

            joy.threshhold = joy.threshhold - (joy.threshhold * difference);
            happyFor.threshhold = happyFor.threshhold - (happyFor.threshhold * difference);
            schadenfreude.threshhold = schadenfreude.threshhold - (schadenfreude.threshhold * difference);
            hope.threshhold = hope.threshhold - (hope.threshhold * difference);
            gratification.threshhold = gratification.threshhold - (gratification.threshhold * difference);

            distress.threshhold = distress.threshhold + (distress.threshhold * difference);
            sorryFor.threshhold = sorryFor.threshhold + (sorryFor.threshhold * difference);
            resentment.threshhold = resentment.threshhold + (resentment.threshhold * difference);
            fear.threshhold = fear.threshhold + (fear.threshhold * difference);
            fearsConfirmed.threshhold = fearsConfirmed.threshhold + (fearsConfirmed.threshhold * difference);
        }
        else{

            distress.threshhold = distress.threshhold - (distress.threshhold * difference);
            sorryFor.threshhold = sorryFor.threshhold - (sorryFor.threshhold * difference);
            resentment.threshhold = resentment.threshhold - (resentment.threshhold * difference);
            fear.threshhold = fear.threshhold - (fear.threshhold * difference);
            fearsConfirmed.threshhold = fearsConfirmed.threshhold - (fearsConfirmed.threshhold * difference);

            joy.threshhold = joy.threshhold + (joy.threshhold * difference);
            happyFor.threshhold = happyFor.threshhold + (happyFor.threshhold * difference);
            schadenfreude.threshhold = schadenfreude.threshhold + (schadenfreude.threshhold * difference);
            hope.threshhold = hope.threshhold + (hope.threshhold * difference);
            gratification.threshhold = gratification.threshhold + (gratification.threshhold * difference);
        }
    }

    /// <summary>
    /// Call Decay function for all emotions
    /// </summary>
    private void DecayAllEmotions(){

        Decay(joy, 0.02f);
        Decay(distress, 0.02f);
        Decay(happyFor, 0.02f);
        Decay(sorryFor, 0.02f);
        Decay(resentment, 0.02f);
        Decay(schadenfreude, 0.02f);
        Decay(hope, 0.02f);
        Decay(fear, 0.02f);
        Decay(gratification, 0.02f);
        Decay(fearsConfirmed, 0.02f);
    }

    /// <summary>
    /// Decay emotional value and slowly reset threshhold to simulate emotional decay.
    /// </summary>
    /// <param name="emotion"></param>
    /// <param name="rate"></param>
    private void Decay(Emotion emotion, float percentage){

        float rate = (1 - emotion.value) * percentage;

        if(emotion.value > 0){
            emotion.value -= rate * Time.deltaTime;
        }
        if(emotion.value <= 0.000001){
            emotion.value = 0;
        }

        if(emotion.threshhold > emotion.defaultThreshhold){
            emotion.threshhold -= percentage * Time.deltaTime;
        }
        if (emotion.threshhold < emotion.defaultThreshhold){
            emotion.threshhold += percentage * Time.deltaTime;
        }

    }

    /// <summary>
    /// Compares two strings
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns>returns true if a value is contained in both strings</returns>
    private bool CompareStrings(List<string> a, List<string> b){

        if(a != null && b != null){

            foreach (string s in a){
                if (b.Contains(s)){
                    return true;
                }
            }

            return false;

        }

        return false;
    }

    /// <summary>
    /// Search for specific relationship by npc name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private Relationship SearchRelationship(string name){

        Relationship output = null;
        foreach(Relationship r in relationships){
            if(r.name == name){
                output = r;
            }
        }

        return output;
    }
    
   
}
