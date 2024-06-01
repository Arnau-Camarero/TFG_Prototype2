using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

using UnityEngine;

using TMPro;

public class MatchMaker : MonoBehaviour
{
    public TextMeshProUGUI updateText;
    public Canvas canvas;

    public void Host(){
        NetworkManager.Singleton.StartHost();
        updateText.text = " Hosting: ";
    } 

    public void Join(){
        NetworkManager.Singleton.StartClient();


    }
}
