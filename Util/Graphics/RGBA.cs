namespace HlidacStatu.Util.Graphics
{
    public class RGBA : RGB
    {
        public RGBA()
        {
        }

        public RGBA(string rgb) : base(rgb)
        {
        }

        public RGBA(byte r, byte g, byte b, byte a) : base(r, g, b)
        {
            this.A = a;
        }

        public RGBA(string r, string g, string b, string a = "FF") : base(r, g, b)
        {
            this.A = byte.Parse(a, System.Globalization.NumberStyles.HexNumber);
        }

        public RGBA(RGB rgb) : this(rgb.R, rgb.G, rgb.B, 255)
        {
        }

        public RGBA SetTransparency(decimal opacity)
        {
            if (opacity < 0)
                opacity = 0;
            if (opacity > 1)
                opacity = 1;

            this.A = (byte)(opacity * 2.55m);

            return this;
        }

        public byte A { get; set; } = 255;

        public override string ToHex()
        {
            return base.ToHex() + A.ToString("X2");
        }
    }


}
