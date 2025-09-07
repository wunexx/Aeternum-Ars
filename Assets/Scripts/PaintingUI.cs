using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class PaintingUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] Image paintingImage;
    [SerializeField] GameObject paintingPanel;
    [SerializeField] GameObject crosshair;

    [Header("Player Components")]
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerCamera playerCamera;
    [SerializeField] PlayerInteraction playerInteraction;

    Texture2D currentTexture;

    private InputSystem_Actions inputActions;

    [DllImport("__Internal")]
    private static extern void DownloadFile(byte[] array, int byteLength, string fileName);

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Enable();
    }

    private void Update()
    {
        if (inputActions.Player.Close.triggered)
        {
            ClosePanel();
        }
    }
    public void OpenPanel(Sprite sprite, Texture2D texture)
    {
        paintingPanel.SetActive(true);
        crosshair.SetActive(false);

        playerMovement.SetCanMove(false);
        playerCamera.SetCanLook(false);
        playerInteraction.SetCanInteract(false);

        currentTexture = texture;

        paintingImage.sprite = sprite;
    }
    public void ClosePanel()
    {
        paintingPanel.SetActive(false);
        crosshair.SetActive(true);

        playerMovement.SetCanMove(true);
        playerCamera.SetCanLook(true);
        playerInteraction.SetCanInteract(true);
    }
    public void DownloadPainting()
    {
#if UNITY_EDITOR
        Debug.Log("Downloading is not supported in the editor.");
        return;
#endif
        if (currentTexture == null)
        {
            Debug.LogError("No texture to download.");
            return;
        }

        byte[] bytes = currentTexture.EncodeToPNG();
        DownloadFile(bytes, bytes.Length, "painting.png");
    }
}
