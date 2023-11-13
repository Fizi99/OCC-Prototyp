using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class NpcAi : MonoBehaviour
{

    private GameManager gm;

    //states
    public string[] states = {"wander", "wait", "flee", "alert", "follow", "agitated"};
    string state = "wander";

    public string npcName;
    private Emotion currEmotion = new Emotion("neutral", 0.1f, 0.0000001f);

    public List<Relationship> relationships;
    public List<Goal> goals;
    private NpcGenerator generator;


    private Transform target;

    private NpcMaterialManager matManager = null;
    private NpcBehaviourManager behaviourManager = null;
    public EmotionEngine emotionEngine = null;

    // Info Panel variables
    public Transform uiRotator;
    public RectTransform infoUi;

    public RectTransform dogTagContainer;
    private Image dogTagBackground;
    private Color bgColor;
    private TextMeshProUGUI dogTagText;

    private TextMeshProUGUI emotionsText;
    public RectTransform emotionsContainer;

    private TextMeshProUGUI relationsText;
    public RectTransform relationsContainer;

    private TextMeshProUGUI goalsText;
    public RectTransform goalsContainer;



    private void Start(){

        gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        generator = GameObject.Find("GameManager").GetComponent<NpcGenerator>();

        target = GameObject.Find("Player").transform;

        

        npcName = generator.GenerateName();
        relationships = generator.GenerateRelationships(npcName);
        
        matManager = transform.GetComponent<NpcMaterialManager>();

        behaviourManager = transform.GetComponent<NpcBehaviourManager>();

        goals = new List<Goal>();
        GenerateGoals();
        
        //initialize Emotion Engine
        emotionEngine = transform.GetComponent<EmotionEngine>();
        emotionEngine.npcName = npcName;
        emotionEngine.relationships = relationships;
        emotionEngine.goals = goals;

        dogTagText = dogTagContainer.GetComponentInChildren<TextMeshProUGUI>();
        dogTagBackground = dogTagContainer.GetComponent<Image>();


        emotionsText = emotionsContainer.GetComponentInChildren<TextMeshProUGUI>();
        relationsText = relationsContainer.GetComponentInChildren<TextMeshProUGUI>();
        goalsText = goalsContainer.GetComponentInChildren<TextMeshProUGUI>();

        

        state = "wander";
    }

    private void Update(){

        PickState();
 
        
        GenerateInfoPanel();
        GenerateDogTag();
        DogTagColor();

        RotateInfoPanel();

    }


    /// <summary>
    /// let emotionengine evaluate an ocurring event
    /// </summary>
    /// <param name="e"></param>
    public void ActivateEvent(EmotionalEvent e){
        emotionEngine.UpdateGoals(goals);
        foreach(Goal goal in goals){
            emotionEngine.EvaluateEvent(goal, e);
        }
    }

    // PICKSTATE FOR OCC
    /// <summary>
    /// choose random state or select a state
    /// </summary>
    /// <param name="input">if empty string select random, else choose input state</param>
    public void PickState(){

        string emotion = emotionEngine.CurrentEmotion().tag;

        if(currEmotion.tag != emotion){
            
            if (emotion == "neutral"){
                behaviourManager.ResetTaskList();
                behaviourManager.AddToTaskList(new Task("wander", 5f, "neutral_walk"));
                behaviourManager.AddToTaskList(new Task("wait", 3f, "neutral_idle"));
                behaviourManager.StartTaskList();
                matManager.ChangeEmotionMaterial("neutral");
                state = "wander";

            }else if(emotion == "joy"){
                behaviourManager.ResetTaskList();
                behaviourManager.AddToTaskList(new Task("follow", 5f, "neutral_run"));
                behaviourManager.AddToTaskList(new Task("waitFollow", 10f, "waving"));
                behaviourManager.StartTaskList();
                matManager.ChangeEmotionMaterial("joy");
                state = "follow";

            }else if (emotion == "distress"){
                behaviourManager.ResetTaskList();
                behaviourManager.AddToTaskList(new Task("flee", 5f, "sad_walk"));
                behaviourManager.AddToTaskList(new Task("waitFlee", 10f, "crying"));
                behaviourManager.StartTaskList();
                matManager.ChangeEmotionMaterial("distress");
                state = "flee";
                
            }else if (emotion == "happyFor"){
                behaviourManager.ResetTaskList();
                behaviourManager.AddToTaskList(new Task("followNpc", 5f, "neutral_run", emotionEngine.happyForNpc.transform));
                behaviourManager.AddToTaskList(new Task("waitFollowNpc", 10f, "waving", emotionEngine.happyForNpc.transform));
                behaviourManager.StartTaskList();
                matManager.ChangeEmotionMaterial("happyFor");
                state = "followNpc";

            }else if (emotion == "sorryFor"){
                behaviourManager.ResetTaskList();
                behaviourManager.AddToTaskList(new Task("followNpc", 5f, "sad_walk", emotionEngine.sorryForNpc.transform));
                behaviourManager.AddToTaskList(new Task("waitFollowNpc", 10f, "crying", emotionEngine.sorryForNpc.transform));
                behaviourManager.StartTaskList();
                matManager.ChangeEmotionMaterial("sorryFor");
                state = "followNpc";

            }else if (emotion == "resentment"){
                behaviourManager.ResetTaskList();
                behaviourManager.AddToTaskList(new Task("fleeNpc", 5f, "neutral_run", emotionEngine.resentmentNpc.transform));
                behaviourManager.AddToTaskList(new Task("waitFleeNpc", 10f, "headache", emotionEngine.resentmentNpc.transform));
                behaviourManager.StartTaskList();
                matManager.ChangeEmotionMaterial("resentment");
                state = "followNpc";

            }else if (emotion == "schadenfreude"){
                behaviourManager.ResetTaskList();
                behaviourManager.AddToTaskList(new Task("followNpc", 5f, "neutral_run", emotionEngine.schadenfreudeNpc.transform));
                behaviourManager.AddToTaskList(new Task("waitFollowNpc", 10f, "laughing", emotionEngine.schadenfreudeNpc.transform));
                behaviourManager.StartTaskList();
                matManager.ChangeEmotionMaterial("schadenfreude");
                state = "followNpc";
            }else if (emotion == "hope"){
                behaviourManager.ResetTaskList();
                behaviourManager.AddToTaskList(new Task("wander", 5f, "neutral_walk"));
                behaviourManager.AddToTaskList(new Task("wait", 3f, "neutral_idle"));
                behaviourManager.StartTaskList();
                matManager.ChangeEmotionMaterial("hope");
                state = "followNpc";

            }else if (emotion == "fear"){
                behaviourManager.ResetTaskList();
                behaviourManager.AddToTaskList(new Task("flee", 5f, "neutral_run"));
                behaviourManager.AddToTaskList(new Task("waitFlee", 10f, "headache"));
                behaviourManager.StartTaskList();
                matManager.ChangeEmotionMaterial("fear");
                state = "flee";
            }else if (emotion == "gratification"){
                behaviourManager.ResetTaskList();
                behaviourManager.AddToTaskList(new Task("follow", 5f, "neutral_run"));
                behaviourManager.AddToTaskList(new Task("waitFollow", 10f, "waving"));
                behaviourManager.StartTaskList();
                matManager.ChangeEmotionMaterial("gratification");
                state = "followNpc";

            }else if (emotion == "fearsConfirmed"){
                behaviourManager.ResetTaskList();
                behaviourManager.AddToTaskList(new Task("flee", 5f, "neutral_run"));
                behaviourManager.AddToTaskList(new Task("waitFlee", 10f, "crying"));
                behaviourManager.StartTaskList();
                matManager.ChangeEmotionMaterial("fearsConfirmed");
                state = "followNpc";
                
            }
            currEmotion = emotionEngine.CurrentEmotion();
        }

    }

   

    //InfoPanel for OCC
    /// <summary>
    /// Functions handling UI Component
    /// </summary>
    public void GenerateInfoPanel(){

        string text = npcName.ToUpper();
        text = text + "\n";

        List<Emotion> temp = emotionEngine.emotions;
        temp.Sort(delegate (Emotion x, Emotion y)
        {
            return x.value.CompareTo(y.value);
        });

        foreach (Emotion e in temp){
            text = text + "\n" + e.tag + ": " + e.percentage.ToString("0.") + "%";
            //text = text + "\nvalue: " + e.value;
            //text = text + "\nthreshhold: " + e.threshhold;
        }
        text = text + "\n";
        text = text + "\nTop Emotion: " + emotionEngine.CurrentEmotion().tag;

        emotionsText.text = text;


        text = npcName.ToUpper();
        text = text + "\n";
        foreach (Relationship r in relationships)
        {
            if(r.liking > 0.55) {
                text = text + "\nlikes " + r.name;
            }else if (r.liking < 0.45){
                text = text + "\ndislikes " + r.name;
            }else{
                text = text + "\nis indifferent to " + r.name;
            }
        }

        relationsText.text = text;

        text = npcName.ToUpper();
        text = text + "\n";
        foreach (Goal goal in goals)
        {
            text = text + "\nGoal Tag: " + goal.tag;
            if(goal.actors.Count != 0)
            {
                if (goal.actors[0] != npcName)
                {
                    text = text + "\nGoal Targets: ";
                    foreach (string s in goal.actors)
                    {
                        text = text + s + ", ";
                    }
                }
            }
            text = text + "\nGoal Utility: " + goal.utility;
            text = text + "\n";

        }

        goalsText.text = text;
    }
    
   
    public void SwapPanel(string input){
    
        if(input == "emotions")
        {
            emotionsContainer.gameObject.SetActive(true);

            relationsContainer.gameObject.SetActive(false);
            goalsContainer.gameObject.SetActive(false);
        }
        if(input == "relations")
        {
            relationsContainer.gameObject.SetActive(true);

            emotionsContainer.gameObject.SetActive(false);
            goalsContainer.gameObject.SetActive(false);
        }
        if (input == "goals")
        {
            goalsContainer.gameObject.SetActive(true);

            emotionsContainer.gameObject.SetActive(false);
            relationsContainer.gameObject.SetActive(false);
        }
    }

    private void GenerateDogTag()
    {
        string text = npcName.ToUpper();
        text = text + "\n";
        text = text + emotionEngine.CurrentEmotion().tag;

        dogTagText.text = text;

    }

    private void DogTagColor()
    {
        Color temp = currEmotion.color;
        temp.a = 0.7f;
        if (bgColor != temp)
        {
            bgColor = temp;
            dogTagBackground.color = temp;
            //dogTagBackground.color = Color.Lerp(bgColor, temp, colorLerp);
            //colorLerp += 0.05f;
        }

        
    }

    public void ShowInfoPanel(){

        if(!infoUi.gameObject.activeSelf){
            infoUi.gameObject.SetActive(true);
            dogTagContainer.gameObject.SetActive(false);
        }else{
            infoUi.gameObject.SetActive(false);
            dogTagContainer.gameObject.SetActive(true);
        }
    }

    private void RotateInfoPanel(){

        uiRotator.LookAt(new Vector3(target.transform.position.x, uiRotator.position.y, target.transform.position.z));
        uiRotator.Rotate(new Vector3(0f, 180f, 0f));
    }

    public Emotion GetCurrentEmotion(){
        return emotionEngine.CurrentEmotion();
    }

    private void GenerateGoals(){

        List<string> likedNpcs = new List<string>();
        List<string> dislikedNpcs = new List<string>();

        foreach(Relationship r in relationships){
            if(r.liking > 0.5f){
                likedNpcs.Add(r.name);
            }else{
                dislikedNpcs.Add(r.name);
            }
        }

        goals.Add(new Goal(generator.goalsTags[0], Random.Range(0f, 0.5f), new List<string> { npcName }));
        goals.Add(new Goal(generator.goalsTags[1], Random.Range(-0.5f, 0f), new List<string> { npcName }));
        goals.Add(new Goal(generator.goalsTags[0], Random.Range(0f, 0.5f), likedNpcs));
        goals.Add(new Goal(generator.goalsTags[0], Random.Range(-0.5f, 0f), dislikedNpcs));
        goals.Add(new Goal(generator.goalsTags[1], Random.Range(-0.5f, 0f), likedNpcs));
        goals.Add(new Goal(generator.goalsTags[1], Random.Range(0f, 0.5f), dislikedNpcs));

        goals.Add(new Goal(generator.goalsTags[2], Random.Range(0.5f, 1f), new List<string> { npcName }));
        goals.Add(new Goal(generator.goalsTags[3], Random.Range(-1f, -0.5f), new List<string> { npcName }));
        goals.Add(new Goal(generator.goalsTags[2], Random.Range(0.5f, 1f), likedNpcs));
        goals.Add(new Goal(generator.goalsTags[2], Random.Range(-1f, -0.5f), dislikedNpcs));
        goals.Add(new Goal(generator.goalsTags[3], Random.Range(-1f, -0.5f), likedNpcs));
        goals.Add(new Goal(generator.goalsTags[3], Random.Range(0.5f, 1f), dislikedNpcs));

    }

    

}
