using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal
{
    //name of goal
    public string tag;
    //how important is event for self. negative is not desirable event, positiv is desirable.
    public float utility;

    //variables for hope or fear confirming emotions. How high is hope or fear for this goal.
    public float envisaging = 0;

    public List<string> actors;

    public Goal(string tag, float utility){

        this.tag = tag;
        this.utility = utility;

    }

    public Goal(string tag, float utility, List<string> actors){

        this.tag = tag;
        this.utility = utility;
        this.actors = actors;

    }

}
