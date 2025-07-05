using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SelfCircleFormation : MonoBehaviour
{
    public float detectionRadius = 10f;  // Distance at which AI will start flashing
    public float flashDuration = 0.5f;   // Time for each flash color change
    public List<Color> flashColors = new List<Color> { Color.blue, Color.green, Color.yellow, Color.magenta }; // List of flash colors without red
    public bool isFleeing = false; // Set to true when the agent is fleeing

    private NavMeshAgent agent;
    private Renderer rend;
    private Material rendMaterial;  // To ensure a unique material instance for color changes
    private static Vector3 sharedCircleCenter;
    private int currentColorIndex = 0; // To track the current color in the list

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rend = GetComponent<Renderer>();  // Get the Renderer component for color change
        
        if (rend == null)
        {
            Debug.LogError("Renderer component is missing on this GameObject.");
            return;
        }

        // Ensure the material is a unique instance to avoid affecting other objects
        if (rend.sharedMaterial != null)
        {
            rendMaterial = new Material(rend.sharedMaterial);
            rend.material = rendMaterial;
        }
        else
        {
            Debug.LogError("Material is missing on the Renderer.");
        }
    }

    void Start()
    {
        // Find player automatically if not set
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) sharedCircleCenter = playerObj.transform.position;
    }

    void Update()
    {
        // Check if the agent is fleeing (from FleeBehavior)
        isFleeing = FleeBehavior.isFleeing;

        // Check distance to player
        float distanceToPlayer = Vector3.Distance(transform.position, sharedCircleCenter);

        // If the agent is fleeing and within detection range, start flashing
        if (isFleeing && distanceToPlayer <= detectionRadius)
        {
            if (!IsInvoking("FlashColor"))
            {
                InvokeRepeating("FlashColor", 0f, flashDuration);  // Start flashing
            }
        }
        else
        {
            if (IsInvoking("FlashColor"))
            {
                CancelInvoke("FlashColor");  // Stop flashing if out of range or not fleeing
                if (rendMaterial != null)
                {
                    rendMaterial.color = Color.white;  // Reset to default color
                }
            }
        }
    }

    void FlashColor()
    {
        // Check if rendMaterial is not null before accessing it
        if (rendMaterial != null)
        {
            // Set the material color to the current color in the list
            rendMaterial.color = flashColors[currentColorIndex];

            // Move to the next color in the list
            currentColorIndex++;

            // If we reach the end of the list, reset to the first color
            if (currentColorIndex >= flashColors.Count)
            {
                currentColorIndex = 0;
            }
        }
        else
        {
            Debug.LogError("Renderer material is not set properly.");
        }
    }
}
