using System;
using System.Net;
using System.Net.Sockets;

public class ArtNetSender : IDisposable {
    private readonly UdpClient udp;
    private IPEndPoint targetEndPoint;

    // Ajusta si lo necesitas
    public const int ArtNetPort = 6454;
    public byte Sequence = 0;  // 0 = sin control de secuencia
    public byte Physical = 0;
    public ushort ProtocolVersion = 14; // Art-Net 4 usa 14

    public ArtNetSender(string targetIp, int port = ArtNetPort, bool enableBroadcast = false) {
        udp = new UdpClient();
        udp.EnableBroadcast = enableBroadcast;
        targetEndPoint = new IPEndPoint(IPAddress.Parse(targetIp), port);
    }

    public void SetTarget(string targetIp, int port = ArtNetPort) {
        targetEndPoint = new IPEndPoint(IPAddress.Parse(targetIp), port);
    }

    /// <summary>
    /// Envía un paquete ArtDMX.
    /// universe: 0–32767 (Art-Net usa LSB primero en el campo Universe).
    /// dmx: array con valores 0–255 (longitud 1–512).
    /// </summary>
    public void SendDmx(ushort universe, byte[] dmx) {
        if (dmx == null) throw new ArgumentNullException(nameof(dmx));
        if (dmx.Length == 0 || dmx.Length > 512) throw new ArgumentOutOfRangeException(nameof(dmx), "DMX debe tener 1..512 canales.");

        byte[] packet = BuildArtDmxPacket(universe, dmx);
        udp.Send(packet, packet.Length, targetEndPoint);
    }

    private byte[] BuildArtDmxPacket(ushort universe, byte[] dmx) {
        // Cabecera "Art-Net\0"
        byte[] id = System.Text.Encoding.ASCII.GetBytes("Art-Net\0");

        // OpCode ArtDMX = 0x5000 (little-endian en el wire: 0x00, 0x50)
        ushort opCode = 0x5000;

        // Longitud DMX en big-endian según spec
        ushort length = (ushort)dmx.Length;

        byte[] packet = new byte[18 + dmx.Length];

        // 0..7: ID
        Buffer.BlockCopy(id, 0, packet, 0, id.Length);

        // 8..9: OpCode (little-endian)
        packet[8] = (byte)(opCode & 0xFF);
        packet[9] = (byte)((opCode >> 8) & 0xFF);

        // 10..11: ProtVer (big-endian)
        packet[10] = (byte)((ProtocolVersion >> 8) & 0xFF);
        packet[11] = (byte)(ProtocolVersion & 0xFF);

        // 12: Sequence
        packet[12] = Sequence;

        // 13: Physical
        packet[13] = Physical;

        // 14..15: Universe (little-endian LSB/MSB)
        packet[14] = (byte)(universe & 0xFF);
        packet[15] = (byte)((universe >> 8) & 0xFF);

        // 16..17: Length (big-endian)
        packet[16] = (byte)((length >> 8) & 0xFF);
        packet[17] = (byte)(length & 0xFF);

        // 18..: Datos DMX
        Buffer.BlockCopy(dmx, 0, packet, 18, dmx.Length);

        return packet;
    }

    public void Dispose() {
        udp?.Close();
        udp?.Dispose();
    }
}
