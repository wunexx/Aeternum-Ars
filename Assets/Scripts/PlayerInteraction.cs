using UnityEngine;
using UnityEngine.UI;
interface IInteractable
{
    public void Interact();
}
public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private Transform playerCamera;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip interactionSound;

    [Header("Gizmos")]
    [SerializeField] private bool drawGizmos = true;

    private InputSystem_Actions inputActions;
    
    bool canInteract = true;    

    Outline currentOutlinedObject;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Enable();
    }

    public void SetCanInteract(bool value)
    {
        canInteract = value;
    }

    private void Update()
    {
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out RaycastHit hit, interactionRange, interactableLayer))
        {
            Outline newOutline = hit.collider.GetComponent<Outline>()
                                 ?? hit.collider.GetComponentInParent<Outline>();

            if (newOutline != currentOutlinedObject)
            {
                if (currentOutlinedObject != null)
                    currentOutlinedObject.enabled = false;

                currentOutlinedObject = newOutline;

                if (currentOutlinedObject != null)
                    currentOutlinedObject.enabled = true;
            }

            if (canInteract && inputActions.Player.Interact.triggered && hit.collider != null)
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>()
                                          ?? hit.collider.GetComponentInParent<IInteractable>();

                if (interactable != null)
                {
                    interactable.Interact();
                    audioSource.pitch = Random.Range(0.9f, 1.1f);
                    audioSource.PlayOneShot(interactionSound);
                }
                else
                    Debug.LogWarning("The object does not implement IInteractable interface.");
            }
        }
        else
        {
            if (currentOutlinedObject != null)
            {
                currentOutlinedObject.enabled = false;
                currentOutlinedObject = null;
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;
        Gizmos.DrawRay(playerCamera.position, playerCamera.forward * interactionRange);
    }
#endif
}
