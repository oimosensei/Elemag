using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.InteractionSystem;

public class ButtonClickToTeleport : MonoBehaviour
{
    // Start is called before the first frame update
    Button button;
    Player player;
    [SerializeField]
    private TeleportMarkerBase teleportMarker;

    void Start()
    {
        button = GetComponent<Button>();
        player = Player.instance;

        if (player == null)
        {
            Debug.LogError("<b>[SteamVR Interaction]</b> Teleport: No Player instance found in map.", this);
            Destroy(this.gameObject);
            return;
        }
        button.onClick.AddListener(() =>
        {
            Vector3 playerFeetOffset = player.trackingOriginTransform.position - player.feetPositionGuess;
            Vector3 teleportPosition = teleportMarker.transform.position;
            player.trackingOriginTransform.position = teleportPosition + playerFeetOffset;

            teleportMarker.TeleportPlayer(teleportMarker.transform.position);
            Teleport.Player.Send(teleportMarker);
            Debug.Log("Teleport");
        });
    }


}
