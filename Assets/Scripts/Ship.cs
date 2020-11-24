using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Ship : MonoBehaviourPun, IOnPhotonViewOwnerChange
{
    public PhotonView PhotonView;
    public PilotRole PilotRole;
    public PhotonView PilotPhotonview;
    public GunnerRole GunnerRole;
    public PhotonView GunnerPhotonview;

    public void OnOwnerChange(Player newOwner, Player previousOwner)
    {
        Debug.Log("Ship OnOwnerChange    newOwner = " + newOwner.NickName);
    }
}
