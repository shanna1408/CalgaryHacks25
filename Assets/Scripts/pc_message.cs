using UnityEngine;
using System.Collections;
using TMPro;

public class pc_message : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public TMP_Text message;
    public AudioSource NotifSFX;

    
    void Start()
    {
        // Start the coroutine to hide and show the object
        StartCoroutine(HideForSeconds(2f));
    }

    IEnumerator HideForSeconds(float delay)
    {
        // Hide the object by disabling it
        message.enabled = false;

        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Show the object by enabling it
        message.enabled = true;
        NotifSFX.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
