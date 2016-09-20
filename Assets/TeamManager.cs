using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class TeamManager : NetworkBehaviour {

    NetworkManager netManager;
    List<NetworkConnection> connections = new List<NetworkConnection>();
    int blueTeamPlayers = 0;
    int redTeamPlayers = 0;

    void Awake()
    {
        netManager = gameObject.GetComponent<NetworkManager>();
    }
    public void AddConnection(int connectionIN)
    {

    }
    public void ChooseTeam(bool chooseRed)
    {
        if (chooseRed)
        {
            if (blueTeamPlayers >= redTeamPlayers)
            {
                redTeamPlayers++;
                SwitchRedTeam();
            }
        }
    }
    private void SwitchRedTeam()
    {
       //
    }
}
