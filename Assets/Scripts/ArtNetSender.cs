using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UIElements;

public class ArtNetSender : MonoBehaviour {
    [Header("Destino Art-Net")]
    public string targetIp = "192.168.1.50";   // IP de tu nodo Art-Net
    public int targetPort = 6454;              // Puerto estándar Art-Net
    public ushort universe = 0;                // Universe DMX (0 por defecto)

    [SerializeField] int adress;
    //[SerializeField] byte value;

    float frequency = 1.0f; // 1 oscilación por segundo

    void Start() {
        // Ejemplo: Enviar valor 255 al canal 1 (address 1) del universe 0
        //SendSingleDmxValue(adress+1, 50);
        //SendSingleDmxValue(adress+2, 50);
        //SendSingleDmxValue(adress, 50);
    }

    [ContextMenu("All white")]
    void SetAllToWhite() {
        for (int i = 1; i < 512; i++) {
            SendSingleDmxValue(i, 50);
        }
    }

    void Update() {
        double seno = Math.Sin(2 * Math.PI * frequency * Time.time);
        byte value = (byte)((seno + 1) / 2 * 254 + 1);
        SendSingleDmxValue(adress, value);
        SendSingleDmxValue(adress+3, value);
        SendSingleDmxValue(adress+6, value);
    }

    [ContextMenu("Blackout all")]
    void BlackoutAll() {
        for (int i = 1; i < 512; i++) {
            SendSingleDmxValue(i, 0);
        }
    }

    /// <summary>
    /// Envía un único valor DMX a un canal 1–512 del universo indicado.
    /// </summary>
    public void SendSingleDmxValue(int channel1Indexed, byte value) {
        if (channel1Indexed < 1 || channel1Indexed > 512) {
            Debug.LogError("Canal DMX fuera de rango (1-512).");
            return;
        }

        // Crea el buffer DMX con todos los canales a 0 y el canal elegido a 'value'
        byte[] dmx = new byte[channel1Indexed];
        dmx[channel1Indexed - 1] = value; // array 0-indexed

        byte[] packet = BuildArtDmxPacket(universe, dmx);

        using (UdpClient udp = new UdpClient()) {
            udp.Send(packet, packet.Length,
                     new IPEndPoint(IPAddress.Parse(targetIp), targetPort));
        }

        Debug.Log($"Enviado valor {value} al canal {channel1Indexed} del universe {universe}");
    }

    private static byte[] BuildArtDmxPacket(ushort universe, byte[] dmxData) {
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
