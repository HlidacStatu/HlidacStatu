using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Newton.SpeechToText.Cloud
{
    [DebuggerDisplay("{ToDebugString}")]
    public class Term
    {

        public enum TermCharacter
        {
            Unknown =0,
            Word = 1,
            Separator = 2,
            SpeakersNoise =3 ,
            Noise = 4   
        }

        //public decimal StartTimestamp { get; set; } //in seconds
        //public decimal EndTimestamp { get; set; } //in seconds
        public decimal Timestamp { get; set; } //in seconds
        public string Value { get; set; }
        public TermCharacter Character { get; set; }
        public decimal Confidence { get; set; }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(this.Value);
        }

        //public void Finalize()
        //{
        //    if (this.StartTimestamp > 0 && this.EndTimestamp == 0)
        //        this.EndTimestamp = this.StartTimestamp;
        //}

        public static List<Term> ToTerms(IEnumerable<ChunkLine> lines)
        {
            List<Term> t = new List<Term>();

            //var 


            return t;
        }

        public string ToDebugString
        {
            get
            {
                return $"{this.Value} - {this.Character} ({this.Timestamp})";
            }
        }

    }
}
