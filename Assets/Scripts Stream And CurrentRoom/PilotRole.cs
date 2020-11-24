using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PilotRole : MonoBehaviourPunCallbacks, IPunOwnershipCallbacks, IPunObservable
{
    [SerializeField] private Ship ship;
    [SerializeField] private PhotonView shipPhotonView;
    [SerializeField] private PhotonView thisPhotonView;

    private void Start()
    {
        Debug.Log($"PilotRole Start()    Is {(photonView.IsMine ? "": "NOT ")} mine. I am {Launcher.role}.");

        if (!photonView.IsMine && Launcher.role == Roles.Pilot)
        {
            shipPhotonView.RequestOwnership();
            photonView.RequestOwnership();
            thisPhotonView.RequestOwnership();
        }
    }

    private float time;

    private void Update()
    {
        if (!photonView.IsMine)
            return;

        if (Input.GetKeyDown(KeyCode.M))
        {
            time = Time.time;

            Debug.Log("Pilot (mine) sets time custom property in CurrentRoom: " + time);

            Hashtable props = new Hashtable
            {
                {"time", "pilot " + time}
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            var customProperties = PhotonNetwork.CurrentRoom.CustomProperties;
            Debug.Log("Pilot (mine) reads custom property in CurrentRoom: " + customProperties["time"]);
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext("pilot " + time);
            Debug.Log($"PilotRole ({(photonView.IsMine ? "" : "NOT ")}mine) sent: pilot " + time);
        }
        else
        {
            // Network player, receive data
            Debug.Log($"PilotRole ({(photonView.IsMine ? "" : "NOT ")}mine) received: "+ (string)stream.ReceiveNext());
        }
    }
}
