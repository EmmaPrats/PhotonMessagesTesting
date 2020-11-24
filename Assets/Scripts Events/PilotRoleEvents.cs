using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PilotRoleEvents : MonoBehaviourPunCallbacks, IPunOwnershipCallbacks
{
    [SerializeField] private Ship ship;
    [SerializeField] private PhotonView shipPhotonView;
    [SerializeField] private PhotonView thisPhotonView;

    private void Start()
    {
        Debug.Log($"PilotRole Start()    Is {(photonView.IsMine ? "": "NOT ")} mine. I am {Launcher.role}.");
    }

    private float time;

    private void Update()
    {
        if (!photonView.IsMine)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            time = Time.time;

            Debug.Log($"Pilot (mine) sends event 100 to interest group 11: " + time);

            PhotonNetwork.SetSendingEnabled(new byte[0], LauncherEvents.InterestGroups);

            var content = new object[] { "pilot " + time };
            var raiseEventOptions = new RaiseEventOptions {Receivers = ReceiverGroup.Others, InterestGroup = 11}; //We can only send to 1 interest group at once.

            PhotonNetwork.RaiseEvent(100, content, raiseEventOptions, SendOptions.SendReliable);
        }
    }

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        Debug.Log("PilotRole OnOwnershipRequest    requestingPlayer = " + requestingPlayer.NickName);
    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        Debug.Log("PilotRole OnOwnershipRequest    previousOwner = " + previousOwner.NickName);
    }
}
