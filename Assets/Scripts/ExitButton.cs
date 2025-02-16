using UnityEngine;

public class ExitButton : MonoBehaviour
{
    public bool hasVerification = false;

    public AudioSource doorSFX;

    public Color newColor = Color.blue; // Choose your color

    public bool redpill=true;

    void OnMouseUpAsButton()
    {
        Debug.Log("test");
        if (redpill) {
            Debug.Log("Color Change Triggered!");
            Renderer[] renderers = Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None);


            foreach (Renderer rend in renderers)
            {
                Debug.Log("Changing color of: " + rend.gameObject.name);
                rend.material.color = Color.red;
            }
            redpill = false;
        }

        if (!hasVerification)
        {
            return;
        }

        //Play the audio you attach to the AudioSource component
        doorSFX.Play();
    }
}

