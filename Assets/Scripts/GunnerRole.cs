using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GunnerRole : MonoBehaviourPunCallbacks, IPunOwnershipCallbacks
{
    [SerializeField] private Ship ship;
    [SerializeField] private PhotonView shipPhotonView;
    [SerializeField] private PhotonView thisPhotonView;

    private void Start()
    {
        Debug.Log($"GunnerRole Start()    Is {(photonView.IsMine ? "": "NOT ")} mine. I am {Launcher.role}.");

        if (!photonView.IsMine && Launcher.role == Roles.Gunner)
        {
            //shipPhotonView.RequestOwnership();
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

            Debug.Log("Gunner (mine) sets time custom property in CurrentRoom: " + time);
            Hashtable props = new Hashtable
            {
                {"time", "gunner " + time}
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            var customProperties = PhotonNetwork.CurrentRoom.CustomProperties;
            Debug.Log("Gunner (mine) reads custom property in CurrentRoom: " + customProperties["time"]);
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
            stream.SendNext("gunner " + time);
            Debug.Log($"GunnerRole ({(photonView.IsMine ? "" : "NOT ")}mine) sent: gunner " + time);
        }
        else
        {
            // Network player, receive data
            Debug.Log($"GunnerRole ({(photonView.IsMine ? "" : "NOT ")}mine) received: "+ (string)stream.ReceiveNext());
        }
    }
}
