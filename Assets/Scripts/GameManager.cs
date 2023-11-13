using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<NpcAi> npcs;
    public GameObject[] npcGOs;

    private void Start()
    {
        npcGOs = GameObject.FindGameObjectsWithTag("NPC");
        foreach (GameObject npc in npcGOs)
        {

            npcs.Add(npc.GetComponent<NpcAi>());
        }
    }

    public NpcAi SearchNpc(string name)
    {

        foreach (NpcAi npc in npcs){

            if (npc.npcName == name){
                return npc;
            }
        }

        return null;
    }

    public float GenerateRandom(float max)
    {
        return Random.Range(0f, max);

    }
}
