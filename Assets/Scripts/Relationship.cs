using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Relationship
{

    public string name;
    //value between 0 and 1. value above 0.5 means person is liked
    public float liking;
    public GameObject reference;


    public Relationship(string name, float liking){

        this.name = name;
        this.liking = liking;

    }
    
}
