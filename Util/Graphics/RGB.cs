namespace HlidacStatu.Util.Graphics
{
    public class RGB
    {
        public RGB() { }
        public RGB(byte r, byte g, byte b)
        {
            this.R = r;
            this.G = g;
            this.B = b;
        }
        public RGB(string r, string g, string b)
        {
            this.R = byte.Parse(r, System.Globalization.NumberStyles.HexNumber);
            this.G = byte.Parse(g, System.Globalization.NumberStyles.HexNumber);
            this.B = byte.Parse(b, System.Globalization.NumberStyles.HexNumber);
        }
        public RGB(string rgb)
         : this(rgb.Substring(0, 2), rgb.Substring(2, 2), rgb.Substring(4, 2))
        { }

        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public virtual string ToHex()
        {
            return "#"
                    + R.ToString("X2")
                    + G.ToString("X2")
                    + B.ToString("X2")
                    ;
        }


    }
}
