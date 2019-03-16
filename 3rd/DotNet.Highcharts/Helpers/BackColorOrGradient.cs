using System.Drawing;
using DotNet.Highcharts.Attributes;

namespace DotNet.Highcharts.Helpers
{
    public class BackColorOrGradient
    {
        public BackColorOrGradient(Color color) { Color = color; }

        public BackColorOrGradient(Gradient gradient) { Gradient = gradient; }

        public static implicit operator BackColorOrGradient(Color value) { return new BackColorOrGradient(value); }

        public static implicit operator BackColorOrGradient(Gradient value) { return new BackColorOrGradient(value); }

        [JsonFormatter(addPropertyName: false, useCurlyBracketsForObject : false)]
        public Color? Color { get; private set; }

        [JsonFormatter(addPropertyName: false, useCurlyBracketsForObject: true)]
        public Gradient Gradient { get; private set; }
    }

    public class Gradient
    {
        public int[] LinearGradient { get; set; }
        public RadialGradient RadialGradient { get; set; }
        public object[,] Stops { get; set; }
    }

    public class RadialGradient
    {
        public Number Cx { get; set; }
        public Number Cy { get; set; }
        public Number R { get; set; }
    }
}