using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class LobbyPlayerToGame : NetworkBehaviour {

    public Color color;
    public GameObject ship;

	// Use this for initialization
	public void StartPlayer (GameObject LobbyPlayerIn) {
        
        Debug.Log("Player started");
        GameObject playerShip = Instantiate(ship, Vector3.zero, Quaternion.identity) as GameObject;
        ship.name = LobbyPlayerIn.name;
        playerShip.GetComponent<N_PlayerController>().AssignNetID(LobbyPlayerIn.GetComponent<NetworkIdentity>());
        NetworkConnection PlayerID = LobbyPlayerIn.GetComponent<NetworkIdentity>().connectionToClient;
        NetworkServer.SpawnWithClientAuthority(ship, PlayerID);
        playerShip.GetComponent<N_PlayerController>().OnStartLocalPlayer();


    }
}
