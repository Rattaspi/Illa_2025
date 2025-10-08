using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class ArtNetSender : MonoBehaviour {
    public static ArtNetSender instance;

    [Header("Destino Art-Net")]
    public string targetIp = "192.168.1.50";   // IP de tu nodo Art-Net
    public int targetPort = 6454;              // Puerto estándar Art-Net
    public ushort universe = 0;                // Universe DMX (0 por defecto)

    [SerializeField] int adress;
    //[SerializeField] byte value;

    int[] adresses = new int[0];
    [SerializeField] byte[] values = new byte[0];

    //float frequency = 0.25f; // 1 oscilación por segundo

    //[ContextMenu("All white")]
    //void SetAllToWhite() {
    //    for (int i = 1; i < 512; i++) {
    //        SendSingleDmxValue(i, 50);
    //    }
    //}

    void Awake() {
        instance = this;
    }

    void FixedUpdate() {
        SendDmxPacket(adresses, values);
        //double sin = Math.Sin(2 * Math.PI * frequency * Time.time);
        //byte value = (byte)((sin + 1) / 2 * 254 + 1);


        //int[] adresses = { 1,2,3, 4,5,6, 7,8,9 };
        //byte[] values = { value, value, value, value, value, value, value, value, value };

        //int[] adresses = { 1 };
        //byte[] values = { value };
        

        //SendSingleDmxValue(adress, value);
        //SendSingleDmxValue(adress+3, value);
        //SendSingleDmxValue(adress+6, value);
    }

    //[ContextMenu("Blackout all")]
    //void BlackoutAll() {
    //    for (int i = 1; i < 512; i++) {
    //        SendSingleDmxValue(i, 0);
    //    }
    //}

    public void UpdatePacketInfo(int[] adresses, byte[] values) {
        this.adresses = adresses;
        this.values = values;
    }
    public void ForcePacketSend() {
        SendDmxPacket(adresses, values);
    }

    void SendDmxPacket(int[] adresses, byte[] values) {
        byte[] dmxValues = new byte[512];


        for (int i = 0; i < adresses.Length; i++) {
            int adress = adresses[i];
            byte value = values[i];

            if (adress < 1 || adress > 512) {
                Debug.LogError("Canal DMX fuera de rango (1-512).");
                return;
            }

            dmxValues[adress - 1] = value; // array 0-indexed
        }        

        byte[] packet = BuildArtDmxPacket(universe, dmxValues);

        using (UdpClient udp = new UdpClient()) {
            udp.Send(packet, packet.Length,
                     new IPEndPoint(IPAddress.Parse(targetIp), targetPort));
        }
    }

    static byte[] BuildArtDmxPacket(ushort universe, byte[] dmxData) {
        byte[] id = System.Text.Encoding.ASCII.GetBytes("Art-Net\0");
        ushort opCode = 0x5000;       // OpDmx en little-endian
        ushort protVer = 14;          // Art-Net 4
        ushort length = (ushort)Mathf.Clamp(dmxData.Length, 1, 512);

        byte[] p = new byte[18 + length];

        // 0..7: "Art-Net\0"
        Buffer.BlockCopy(id, 0, p, 0, id.Length);
        // 8..9: OpCode (little-endian)
        p[8] = (byte)(opCode & 0xFF);
        p[9] = (byte)((opCode >> 8) & 0xFF);
        // 10..11: ProtVer (big-endian)
        p[10] = (byte)((protVer >> 8) & 0xFF);
        p[11] = (byte)(protVer & 0xFF);
        // 12: Sequence (0 = sin control)
        p[12] = 0;
        // 13: Physical (0)
        p[13] = 0;
        // 14..15: Universe (little-endian)
        p[14] = (byte)(universe & 0xFF);
        p[15] = (byte)((universe >> 8) & 0xFF);
        // 16..17: Length (big-endian)
        p[16] = (byte)((length >> 8) & 0xFF);
        p[17] = (byte)(length & 0xFF);
        // 18.. : DMX data
        Buffer.BlockCopy(dmxData, 0, p, 18, length);

        return p;
    }
}
