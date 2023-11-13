using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcMaterialManager : MonoBehaviour
{

    public Material[] emotionMaterials;

    private Material skin;
    private Material emotion;
    private SkinnedMeshRenderer meshrenderer;
    private NpcGenerator generator;



    /// <summary>
    /// initialize material Manager
    /// </summary>
    private void Start(){

        generator = GameObject.Find("GameManager").GetComponent<NpcGenerator>();
        meshrenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        ChangeSkin(generator.GenerateSkin());
        ChangeEmotionMaterial("neutral");
    }

    /// <summary>
    /// change material for facial expression to simulate emotion expression while keeping the other material.
    /// </summary>
    /// <param name="input">string of associated emotion. associated material has to contain emotion in its name.</param>
    public void ChangeEmotionMaterial(string input){

        if(input != null && emotionMaterials != null && meshrenderer != null){
            foreach(Material mat in emotionMaterials){
                if(mat.name.ToLower().Contains(input.ToLower())){
                    emotion = mat;
                    meshrenderer.materials = new Material[2]{skin, mat};
                }
            }
        }else{
            Debug.LogError("CANNOT BE NULL!");
        }
    }
    public void ChangeSkin(Material mat)
    {

        if (mat != null && meshrenderer != null)
        {

            skin = mat;
            meshrenderer.materials = new Material[2] { mat, emotion };

        }
        else
        {
            Debug.LogError("CANNOT BE NULL!");
        }
    }
    
}
