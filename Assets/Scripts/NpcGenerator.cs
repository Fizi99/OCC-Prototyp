using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcGenerator : MonoBehaviour
{

    public List<string> goalsTags = new List<string>{ "praise", "insult", "give gift", "steal item" };
    public string[] actorNames = { "john", "hans", "luna" };
    public int npcAmount = 1;
    public List<Material> skins;
    private int nameIndex = 0;

    public GameObject npcPrefab;

    /// <summary>
    /// generate and place npc prefabs. Length of actorNames array indicates amount of created npcs.
    /// </summary>
    public void GenerateNpcs(){

        for(int i = 0; i < npcAmount; i++)
        {
            Instantiate(npcPrefab, new Vector3(Random.Range(-15, 15), 0, Random.Range(-15, 15)), Quaternion.identity);
        }

    }

    private void Awake(){
        GenerateNpcs();
    }



    public List<Relationship> GenerateRelationships(string self){

        List<Relationship> relationships = new List<Relationship>();

        for(int i = 0; i < npcAmount; i++)
        {
            if (actorNames[i] != self)
            {
                relationships.Add(new Relationship(actorNames[i], Random.Range(0f, 1f)));
            }

        }

        return relationships;

    }

    /// <summary>
    /// generate names for all npcs.
    /// </summary>
    /// <returns></returns>
    public string GenerateName(){

        return actorNames[nameIndex++];
    }

    public Material GenerateSkin()
    {
        return skins[Random.Range(0, skins.Count)];

    }

    
}
