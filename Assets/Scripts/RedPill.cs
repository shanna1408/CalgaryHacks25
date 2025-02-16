using UnityEngine;
using System.Collections;
using TMPro;

public class RedPill : MonoBehaviour 
{
	public float reachRange = 1.8f;			

	// private Animator anim;
	private Camera fpsCam;
	private GameObject player;

	private bool playerEntered;
	private bool showInteractMsg;
	private GUIStyle guiStyle;
	private string msg;

	private int rayLayerMask; 

    public TMP_Text subjectID;
    public GameObject floppyDisk;
    private bool redPill = false;

    public bool flopObtained = false;

	void Start()
	{
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = false;
        }
        floppyDisk.SetActive(false);
        subjectID.enabled = false;
		//Initialize moveDrawController if script is enabled.
		player = GameObject.FindGameObjectWithTag("Player");

		fpsCam = Camera.main;
		if (fpsCam == null)	//a reference to Camera is required for rayasts
		{
			Debug.LogError("A camera tagged 'MainCamera' is missing.");
		}

		//the layer used to mask raycast for interactable objects only
		LayerMask iRayLM = LayerMask.NameToLayer("InteractRaycast");
		rayLayerMask = 1 << iRayLM.value;  

		//setup GUI style settings for user prompts
		setupGui();

	}
		
	void OnTriggerEnter(Collider other)
	{		
		if (other.gameObject == player)		//player has collided with trigger
		{			
			playerEntered = true;

		}
	}

	void OnTriggerExit(Collider other)
	{		
		if (other.gameObject == player)		//player has exited trigger
		{			
			playerEntered = false;
			//hide interact message as player may not have been looking at object when they left
			showInteractMsg = false;		
		}
	}



	void Update()
	{		
		if (playerEntered)
		{	

			//center point of viewport in World space.
			Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f,0.5f,0f));
			RaycastHit hit;

			//if raycast hits a collider on the rayLayerMask
			if (Physics.Raycast(rayOrigin,fpsCam.transform.forward, out hit,reachRange,rayLayerMask))
			{
				MoveableObject moveableObject = null;
				//is the object of the collider player is looking at the same as me?
				if (!isEqualToParent(hit.collider, out moveableObject))
				{	//it's not so return;
					return;
				}
					
				if (moveableObject != null)		//hit object must have MoveableDraw script attached
				{
					showInteractMsg = true;
                    msg = getGuiMsg();

					if (Input.GetKeyUp(KeyCode.R))
					{
                        Debug.Log("Ate the red pill!");
                        redPill = true;
                        if (!flopObtained) {
                            floppyDisk.SetActive(true);
                        }
                        subjectID.enabled = true;

					} else if (Input.GetKeyUp(KeyCode.B))
                    {
                        Debug.Log("Ate the blue pill!");
                        floppyDisk.SetActive(false);
                        redPill = false;
                        subjectID.enabled = false;
                    }
                    else if (Input.GetKeyUp(KeyCode.F)){
                        if (redPill && !flopObtained){
                            Debug.Log("Everybody do the flop");
                            flopObtained = true;
                            floppyDisk.SetActive(false);
                        }
                    }

				}
			}
			else
			{
				showInteractMsg = false;
			}
		}

	}

	//is current gameObject equal to the gameObject of other.  check its parents
	private bool isEqualToParent(Collider other, out MoveableObject draw)
	{
		draw = null;
		bool rtnVal = false;
		try
		{
			int maxWalk = 6;
			draw = other.GetComponent<MoveableObject>();

			GameObject currentGO = other.gameObject;
			for(int i=0;i<maxWalk;i++)
			{
				if (currentGO.Equals(this.gameObject))
				{
					rtnVal = true;	
					if (draw== null) draw = currentGO.GetComponentInParent<MoveableObject>();
					break;			//exit loop early.
				}

				//not equal to if reached this far in loop. move to parent if exists.
				if (currentGO.transform.parent != null)		//is there a parent
				{
					currentGO = currentGO.transform.parent.gameObject;
				}
			}
		} 
		catch (System.Exception e)
		{
			Debug.Log(e.Message);
		}
			
		return rtnVal;

	}
		

	#region GUI Config

	//configure the style of the GUI
	private void setupGui()
	{
		guiStyle = new GUIStyle();
		guiStyle.fontSize = 30;
		guiStyle.fontStyle = FontStyle.Bold;
		guiStyle.normal.textColor = Color.white;
	}

	private string getGuiMsg()
	{
		string rtnVal;
        if (flopObtained) {
            rtnVal = "We found a mysterious floppy disk here. We should see what it does.\nPress R to take the red pill \nPress B to take the blue pill";
        }
        else if (!redPill) {
            rtnVal = "Press R to take the red pill \nPress B to take the blue pill";
        } else {
            rtnVal = "Press R to take the red pill \nPress B to take the blue pill\nPress F to take the floppydisk";
        }
        Debug.Log("Pills detected");
		return rtnVal;
	}

	void OnGUI()
	{
		if (showInteractMsg)  //show on-screen prompts to user for guide.
		{
			GUI.Label(new Rect (50,Screen.height - 105,200,50), msg,guiStyle);
		}
	}		
	//End of GUI Config --------------
	#endregion
}
