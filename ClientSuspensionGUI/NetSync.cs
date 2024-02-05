using System.Linq;
using CarX;
using SyncMultiplayer;

namespace ClientSuspensionGUI;

public static class NetSync
{
    public static event PacketCallback? ProcessPacket;
    private static SmartfoxRoomClient? Room { get; set; }
    private static SmartfoxClient? Client { get; set; }
    private static NetGameSubroomsSystem? SubRoom { get; set; }
    public delegate void PacketCallback(NetworkPlayer sender, SmartfoxDataPackage data);
    private static bool _reloadClient;
    private static bool _reloadSubRoom;

    public static void Update()
    {
        if (Room == null || _reloadClient)
        {
            Room = NetworkController.InstanceGame.Client;
            TrySetupSubRoom();
            TrySetupClient();
        }
        if (SubRoom == null || _reloadSubRoom)
        {
            TrySetupSubRoom();
        }
        if (Client == null || _reloadClient)
        {
            TrySetupClient();
        }
        if (Client != null && !Client.Sfs.IsConnected)
        {
            Client.Sfs.InitUDP();
        }
        if (Client != null && Client.State != ClientState.Joined)
        {
            Client.Sfs.InitUDP();
        }
    }

    private static void TrySetupSubRoom()
    {
        SubRoom = NetworkController.InstanceGame.systems.Get<NetGameSubroomsSystem>();
        _reloadSubRoom = false;
    }

    private static void TrySetupClient()
    {
        if (Room == null) return;
        Client = Room.m_client;
        if (Client != null && !Client.Sfs.IsConnected)
        {
            Client.Sfs.InitUDP();
        }
        NetworkController.InstanceGame.packetHandler.Subscribe(PacketId.Subroom, MainPacketHandler);
        _reloadClient = false;
    }

    private static void MainPacketHandler(NetworkPlayer sender, SmartfoxDataPackage data)
    {
        ProcessPacket?.Invoke(sender, data);
    }

    #region Send Methods that neeed debugging?
    //public static void Send(SmartfoxDataPackage data, bool includeSelf = false) // Causes INCREDIBLE LAG
    //{
    //    var game = NetworkController.InstanceGame;
    //    if (game == null) return;
    //    var client = game.Client;
    //    if (client == null) return;
    //    if (!includeSelf)
    //        client.Send(data, true);
    //    else
    //        client.SendIncludingSelf(data, true);
//
    //    if (client.State == ClientState.Joined) return;
    //    _reloadClient = true;
    //    client.m_client.InitUDP();
    //}
//
    //public static void Send(SmartfoxDataPackage data, params int[] receivers) // Causes INCREDIBLE LAG
    //{
    //    var game = NetworkController.InstanceGame;
    //    if (game == null) return;
    //    var client = game.Client;
    //    if (client == null) return;
    //    client.Send(data, true, receivers);
//
    //    if (client.State == ClientState.Joined) return;
    //    _reloadClient = true;
    //    client.m_client.InitUDP();
    //}
    #endregion
    private static void Send(SmartfoxDataPackage data, bool includeSelf = false)
    {
        if (Client == null) return;
        data.Add("0", (byte)PacketId.Subroom);
        if (includeSelf)
        {
            Room!.SendIncludingSelf(data, true);
        }
        else
        {
            Room!.Send(data, true);
        }
        if (Client.State == ClientState.Joined) return;
        _reloadClient = true;
        Client.Sfs.InitUDP();
    }

    public static void Send(SmartfoxDataPackage data, params int[] receivers)
    {
        if (Client == null) return;
        data.Add("0", (byte)PacketId.Subroom);
        Room!.Send(data, true, NetworkController.InstanceGame.CurrentRoom.UserList.Where(x => receivers.Contains(x.Id)).Select(x => x.Id).ToArray());
        if (Client.State == ClientState.Joined) return;
        _reloadClient = true;
        Client.Sfs.InitUDP();
    }

    public static void SendSuspensionData(Wheel frontLeft, Wheel frontRight, Wheel rearLeft, Wheel rearRight)
    {
        var createPacket = new SmartfoxDataPackage(PacketId.Subroom); // possibly not required? can also use SFSObject.NewIstance();
        createPacket.Data.PutFloat("csusFL", frontLeft.maxSpringLen);
        createPacket.Data.PutFloat("csusFR", frontRight.maxSpringLen);
        createPacket.Data.PutFloat("csusRL", rearLeft.maxSpringLen);
        createPacket.Data.PutFloat("csusRR", rearRight.maxSpringLen);
        Send(createPacket);
    }

    public static void HandleSync(NetworkPlayer sender, SmartfoxDataPackage data)
    {
        if (!data.Data.ContainsKey("csusFL")) return;
        if (!data.Data.ContainsKey("csusFR")) return;
        if (!data.Data.ContainsKey("csusRL")) return;
        if (!data.Data.ContainsKey("csusRR")) return;
        if (sender.userCar is null) return;

        var suspensionData = data.Data;
        var fl = suspensionData.GetFloat("csusFL");
        var fr = suspensionData.GetFloat("csusFR");
        var rl = suspensionData.GetFloat("csusRL");
        var rr = suspensionData.GetFloat("csusRR");
        if (sender.userCar != null)
        {
            Adjust(sender.userCar.carX, fl, fr, rl, rr);
        }
    }

    private static void Adjust(Car car, float fl, float fr, float rl, float rr)
    {
        var wheel = car.GetWheel(WheelIndex.FrontLeft);
        var wheel2 = car.GetWheel(WheelIndex.FrontRight);
        var wheel3 = car.GetWheel(WheelIndex.RearLeft);
        var wheel4 = car.GetWheel(WheelIndex.RearRight);
        wheel.maxSpringLen = fl;
        wheel2.maxSpringLen = fr;
        wheel3.maxSpringLen = rl;
        wheel4.maxSpringLen = rr;
    }
}