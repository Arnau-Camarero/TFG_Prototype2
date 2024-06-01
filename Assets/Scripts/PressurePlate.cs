using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PressurePlate : NetworkBehaviour
{
    public bool isPressed = false;
    private NetworkVariable<bool> platePressed = new NetworkVariable<bool>(false);

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            enabled = false;
        }
        else
        {
            platePressed.OnValueChanged += OnPlatePressedChanged;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player" || col.gameObject.tag == "Prop")
        {
            isPressed = true;
            PlatePressedServerRpc(true);
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Player" || col.gameObject.tag == "Prop")
        {
            isPressed = false;
            PlatePressedServerRpc(false);
        }
    }

    [ServerRpc]
    void PlatePressedServerRpc(bool pressed)
    {
        platePressed.Value = pressed;
    }

    void OnPlatePressedChanged(bool oldValue, bool newValue)
    {
        isPressed = newValue;
        Debug.Log($"Plate pressed state changed to: {newValue}");
    }

    private void OnDestroy()
    {
        if (IsOwner)
        {
            platePressed.OnValueChanged -= OnPlatePressedChanged;
        }
    }
}
