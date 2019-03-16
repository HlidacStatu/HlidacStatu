using System.Drawing;
using DotNet.Highcharts.Attributes;

namespace DotNet.Highcharts.Helpers
{
    public class BackgroundObject
    {
        [JsonFormatter(addPropertyName: true, useCurlyBracketsForObject: false)]
        public BackColorOrGradient BackgroundColor { get; set; }

        [JsonFormatter(addPropertyName: true, useCurlyBracketsForObject: false)]
        public PercentageOrPixel InnerRadius { get; set; }

        [JsonFormatter(addPropertyName: true, useCurlyBracketsForObject: false)]
        public PercentageOrPixel OuterRadius { get; set; }

        [JsonFormatter(addPropertyName: true, useCurlyBracketsForObject: false)]
        public PercentageOrPixel BorderWidth { get; set; }

        [JsonFormatter(addPropertyName: true, useCurlyBracketsForObject: false)]
        public Color? BorderColor { get; set; }
    }
}