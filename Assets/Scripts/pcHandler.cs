using UnityEngine;
using System.Collections;
using TMPro;

public class pcHandler : MonoBehaviour 
{
	public float reachRange = 1.8f;			

	// private Animator anim;
	private Camera fpsCam;
	private GameObject player;

	private bool playerEntered;
	private bool showInteractMsg;
	private GUIStyle guiStyle;
	private string msg;
    
    private bool flopInserted = false;

	private int rayLayerMask; 

    public RedPill redpill;

    public TMP_Text pcmessg;

	public TMP_Text pswd;

    public bool flopObtained = false;

	public string passwordAttempt = "";

	string correctPass = "27486875";

	public bool hasVerification = false;
 

	void Start()
	{
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = false;
        }
        // floppyDisk.SetActive(false);
        // subjectID.enabled = false;
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
        flopObtained = redpill.flopObtained;
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
		flopObtained = redpill.flopObtained;
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

                    if (Input.GetKeyUp(KeyCode.F)){
                        if (flopObtained){
                            Debug.Log("Everybody do the flop");
                            pcmessg.text = "jdkwqnj&!jb(&&)";
                            flopInserted = true;
							pswd.text = "Enter Password: ";
                        }
                    }
					if (flopInserted)
					{
						if (Input.GetKeyUp(KeyCode.Alpha1)) AddNumberToPassword("1");
						if (Input.GetKeyUp(KeyCode.Alpha2)) AddNumberToPassword("2");
						if (Input.GetKeyUp(KeyCode.Alpha3)) AddNumberToPassword("3");
						if (Input.GetKeyUp(KeyCode.Alpha4)) AddNumberToPassword("4");
						if (Input.GetKeyUp(KeyCode.Alpha5)) AddNumberToPassword("5");
						if (Input.GetKeyUp(KeyCode.Alpha6)) AddNumberToPassword("6");
						if (Input.GetKeyUp(KeyCode.Alpha7)) AddNumberToPassword("7");
						if (Input.GetKeyUp(KeyCode.Alpha8)) AddNumberToPassword("8");
						if (Input.GetKeyUp(KeyCode.Alpha9)) AddNumberToPassword("9");
						if (Input.GetKeyUp(KeyCode.Alpha0)) AddNumberToPassword("0");
						// Check if the main Enter key or keypad Enter key is pressed
						if (Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.KeypadEnter))
						{
							if (passwordAttempt==correctPass){
								hasVerification = true;
								pswd.text = "Password Accepted. All systems down. Doors Unlocked.";
							}
							else {
								Debug.Log(passwordAttempt);
								passwordAttempt = "";
								pswd.text = "Password Rejected. Enter Password: ";
							}
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

	// Helper method to add a number to the password
	private void AddNumberToPassword(string number)
	{
		pswd.text += number;
		passwordAttempt += number;
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
		string rtnVal="";
        if (flopObtained && !flopInserted) {
            rtnVal = "Press F to insert the floppy disk";
        } if (flopInserted) {
			rtnVal = "Enter the password";
		}
		return rtnVal;
	}

	void OnGUI()
	{
		if (showInteractMsg)  //show on-screen prompts to user for guide.
		{
			GUI.Label(new Rect (50,Screen.height - 80,200,50), msg,guiStyle);
		}
	}		
	//End of GUI Config --------------
	#endregion
}
