using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlls : MonoBehaviour
{

    Rigidbody rigid = new Rigidbody();
    public float speed = 5;
    public float mouseSensitivity = 100f;
    private float xRotation = 0f;
    private float interactionRange = 100;

    public GameObject interactionMenu;

    private EventSheet eventSheet;

    public Camera mainCam = null;
    public Transform cameraRotation;

    private Transform target;
    private NpcAi targetAi;
    public GameObject targetMarkerPrefab;
    private GameObject targetMarker;

    private CharacterController controller;


    private GameManager gm;
    private List<NpcAi> npcs = new List<NpcAi>();

    public Vector3 cameraOffset = new Vector3(0, 0, 0);
    
    private void Start(){

        rigid = transform.GetComponent<Rigidbody>();

        eventSheet = new EventSheet();

        gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        eventSheet = gm.GetComponent<EventSheet>();

        controller = transform.GetComponent<CharacterController>();

        npcs = gm.npcs;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

    }

    private void Update(){



        ChangeInteractionMenu();

        if(interactionMenu.activeSelf == false)
        {
            PlayerLookRotation();
            PlayerMovement();
            SelectTarget();
        }
        else
        {
            SelectTargetInMenu();
            controller.Move(new Vector3(0, 0, 0));
        }
    }

    private void ChangeInteractionMenu()
    {
        if(interactionMenu.activeSelf == false)
        {
            if (Input.GetKeyDown(KeyCode.E)){
                interactionMenu.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                interactionMenu.SetActive(false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    private void PlayerMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);
    }

    private void PlayerLookRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraRotation.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(Vector3.up * mouseX);
    }

    /// <summary>
    /// Select npc target for interaction
    /// </summary>
    private void SelectTarget(){

        if (Input.GetKey(KeyCode.Mouse0)){

            int x = Screen.width / 2;
            int y = Screen.height / 2;
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(x, y, 0));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100)){

                if(hit.transform.tag == "NPC"){

                    if((hit.transform.position - transform.position).magnitude < interactionRange)
                    {
                        this.target = hit.transform;
                        this.targetAi = target.GetComponent<NpcAi>();
                        Destroy(targetMarker);
                        this.targetMarker = Instantiate(targetMarkerPrefab, target);
                        targetMarker.transform.SetParent(target);
                    }
                }
            }
        }
    }

    private void SelectTargetInMenu()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {

                if (hit.transform.tag == "NPC")
                {

                    if ((hit.transform.position - transform.position).magnitude < interactionRange)
                    {
                        this.target = hit.transform;
                        this.targetAi = target.GetComponent<NpcAi>();
                        Destroy(targetMarker);
                        this.targetMarker = Instantiate(targetMarkerPrefab, target);
                        targetMarker.transform.SetParent(target);
                    }
                }
            }
        }
    }



    /// <summary>
    /// Interact with target npc and fire event to all npcs.
    /// </summary>
    /// <param name="input"></param>
    public void Interact(string input){

        
        EmotionalEvent e = eventSheet.SearchEvent(input);

        if (e != null && targetAi != null)
        {
            if ((target.transform.position - transform.position).magnitude < interactionRange)
            {

                e.targets.Insert(0, targetAi.npcName);
                e.EventFired();

                ////////SCHERER///////////
                //e.actor = gameObject;
                //////////////////////////

                foreach (NpcAi npc in npcs)
                {

                    npc.ActivateEvent(e);

                }
            }
        }
        
    }

    /// <summary>
    /// show info panel of npc
    /// </summary>
    public void ShowNpcInfo(){

        if(targetAi != null){
            targetAi.GenerateInfoPanel();
            targetAi.ShowInfoPanel();
        }
    }

    public void SwapNpcInfo(string input)
    {

        if (targetAi != null)
        {
            ShowNpcInfo();
            targetAi.GenerateInfoPanel();
            targetAi.SwapPanel(input);
        }
    }
}
