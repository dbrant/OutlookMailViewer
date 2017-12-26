using System;
using System.Linq;

namespace PSTParse.LTP
{
    public class HNHDR
    {
        public enum ClientSig
        {
            TableContext = 0x7C,
            BTreeHeap = 0xB5,
            PropertyContext = 0xBC
        }
        public ulong OffsetHNPageMap { get; private set; }
        public ulong bSig { get; private set; }
        public ClientSig ClientSigType { get; private set; }
        public HID UserRoot { get; private set; }
        public ulong FillLevel_raw { get; private set; }

        public HNHDR(byte[] bytes)
        {
            this.ClientSigType = (ClientSig)bytes[3];
            this.bSig = bytes[2];
            this.OffsetHNPageMap = BitConverter.ToUInt16(bytes, 0);
            this.UserRoot = new HID(bytes.Skip(4).Take(4).ToArray());
            this.FillLevel_raw = BitConverter.ToUInt32(bytes, 8);
        }
    }
}
