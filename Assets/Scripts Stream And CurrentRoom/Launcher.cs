using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum Roles { Pilot, Gunner }

public class Launcher : MonoBehaviourPunCallbacks
{
    [Header("Launcher:")]
    [SerializeField] private GameObject launcherPanel;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button joinRoomAsPilotButton;
    [SerializeField] private Button joinRoomAsGunnerButton;
    [SerializeField] private Toggle joinTeam1Toggle;

    [Header("In-Game:")]
    [SerializeField] private GameObject inGamePanel;
    [SerializeField] private Toggle subscribedTo1Toggle;
    [SerializeField] private Toggle subscribedTo2Toggle;
    [SerializeField] private Toggle subscribedTo11Toggle;
    [SerializeField] private Toggle subscribedTo12Toggle;

    private string gameVersion;
    private bool isConnecting;

    public static Roles role;

    public static byte[] InterestGroups = new byte[0];

    private void Awake()
    {
        gameVersion = Application.version;
    }

    private void Start()
    {
        joinRoomAsPilotButton.onClick.AddListener(OnJoinRoomPilotButtonClicked);
        joinRoomAsGunnerButton.onClick.AddListener(OnJoinRoomGunnerButtonClicked);
        launcherPanel.SetActive(true);
        inGamePanel.SetActive(false);
    }

    private void OnDestroy()
    {
        joinRoomAsPilotButton.onClick.RemoveListener(OnJoinRoomPilotButtonClicked);
        joinRoomAsGunnerButton.onClick.RemoveListener(OnJoinRoomGunnerButtonClicked);
    }

    private void OnJoinRoomPilotButtonClicked()
    {
        Debug.Log("OnJoinRoomPilotButtonClicked()");
        role = Roles.Pilot;
        OnJoinRoomButtonClicked();
    }

    private void OnJoinRoomGunnerButtonClicked()
    {
        Debug.Log("OnJoinRoomGunnerButtonClicked()");
        role = Roles.Gunner;
        OnJoinRoomButtonClicked();
    }

    private void OnJoinRoomButtonClicked()
    {
        Debug.Log($"PhotonNetwork is {(PhotonNetwork.IsConnected ? "" : "not ")} connected.");
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.OfflineMode = false;
            PhotonNetwork.GameVersion = gameVersion;
            isConnecting = PhotonNetwork.ConnectUsingSettings();
        }
    }

    private void UpdateSubscription(bool value)
    {
        var groups = new List<byte>();
        if (subscribedTo1Toggle.isOn) groups.Add(1);
        if (subscribedTo2Toggle.isOn) groups.Add(2);
        if (subscribedTo11Toggle.isOn) groups.Add(11);
        if (subscribedTo12Toggle.isOn) groups.Add(2);
        InterestGroups = groups.ToArray();
        PhotonNetwork.SetInterestGroups(new byte[0], InterestGroups);
        Debug.Log("Set interest groups: " + string.Join(", ", InterestGroups));
    }

    #region PhotonCallbacks

    public override void OnConnected()
    {
        base.OnConnected();

        var nickname = inputField.text;
#if UNITY_EDITOR
        if (string.IsNullOrEmpty(nickname))
            nickname = "Editor";
#else
        if (string.IsNullOrEmpty(nickname))
            nickname = "Standalone";
#endif

        
        Debug.Log($"OnConnected()    Nickname = {nickname}");
        PhotonNetwork.NickName = nickname;
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby()");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster()");
        isConnecting = false;
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("OnDisconnected()    reason {0}", cause);
        isConnecting = false;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom("my room");
    }

    public override void OnJoinedRoom ()
    {
        Debug.Log("OnJoinedRoom()    room = " + PhotonNetwork.CurrentRoom.Name);
        launcherPanel.SetActive(false);
        inGamePanel.SetActive(true);

        subscribedTo1Toggle.isOn = joinTeam1Toggle.isOn;
        subscribedTo2Toggle.isOn = !joinTeam1Toggle.isOn;
        subscribedTo11Toggle.isOn = false;
        subscribedTo12Toggle.isOn = false;
        subscribedTo1Toggle.onValueChanged.AddListener(UpdateSubscription);
        subscribedTo2Toggle.onValueChanged.AddListener(UpdateSubscription);
        subscribedTo11Toggle.onValueChanged.AddListener(UpdateSubscription);
        subscribedTo12Toggle.onValueChanged.AddListener(UpdateSubscription);

        Initialize();
    }

    #endregion

    private Ship ship;

    private void Initialize()
    {
        var team = (byte) (joinTeam1Toggle.isOn ? 1 : 2);
        PhotonNetwork.SetInterestGroups(new byte[0], new byte[1] { team });
        if (role == Roles.Pilot)
        {
            ship = PhotonNetwork.Instantiate("Ship", new Vector3(Random.Range(-3f, 3f), 0, 0), Quaternion.identity, team).GetComponent<Ship>();
        }
    }
}
