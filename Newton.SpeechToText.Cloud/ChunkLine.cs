using System.Collections.Generic;
using System.Text;

namespace Newton.SpeechToText.Cloud
{
    public class ChunkLine
    {
        public Push push { get; set; }


        public class Push
        {
            public Events events { get; set; }


            public class Events
            {
                static decimal toSec = 10000000m;
                public List<Term> ToTerms(Term prevTerm )
                {
                    List<Term> terms = new List<Term>();

                    if (events == null)
                        return terms;
                    if (events.Length == 0)
                        return terms;

                    Term t = new Term();
                    decimal lastTime = prevTerm?.Timestamp ?? 0m;

                    foreach (var ev in events)
                    {
                        if (!string.IsNullOrEmpty(ev?.timestamp?.timestamp))
                        {
                            if (long.TryParse(ev?.timestamp?.timestamp, out long actTime))
                            {
                                lastTime = actTime / toSec;
                            }
                        }
                        if (ev.label != null)
                        {
                            if (!string.IsNullOrEmpty(ev.label.item))
                            {
                                t.Timestamp = lastTime;

                                t.Character = Term.TermCharacter.Word;
                                t.Value += ev.label.item;
                            }
                            else
                            {
                                if (!t.IsEmpty())
                                {
                                    terms.Add(t);
                                    t = new Term();
                                }



                                if (!string.IsNullOrEmpty(ev.label.plus))
                                {
                                    t.Character = Term.TermCharacter.Separator;
                                    t.Value += ev.label.plus;
                                    t.Timestamp = lastTime;

                                    terms.Add(t);
                                    t = new Term();

                                }
                                if (!string.IsNullOrEmpty(ev.label.noise))
                                {
                                    t.Character = Term.TermCharacter.Noise;
                                    t.Value += ev.label.noise;
                                    t.Timestamp = lastTime;
                                    terms.Add(t);
                                    t = new Term();
                                }
                            }
                        }

                    } //for
                    if (!t.IsEmpty())
                    {
                        terms.Add(t);
                    }

                    return terms;
                }


                [System.Obsolete]
                public string ToText(long prevTimestamp, out long timestamp)
                {
                    timestamp = prevTimestamp;

                    if (events == null)
                        return "";
                    if (events.Length == 0)
                        return "";
                    StringBuilder sb = new StringBuilder();
                    long lastTime = prevTimestamp;
                    decimal diff = 0;

                    foreach (var ev in events)
                    {
                        if (!string.IsNullOrEmpty(ev?.timestamp?.timestamp))
                        {
                            if (long.TryParse(ev?.timestamp?.timestamp, out long actTime))
                            {
                                diff = (actTime - lastTime) / toSec;
                                System.Diagnostics.Debug.WriteLine(diff);
                                lastTime = actTime;
                            }
                        }
                        if (ev.label != null)
                        {
                            if (diff > 0.9m)
                            {
                                sb.AppendLine($"#LINE {diff}#");
                                diff = 0;
                            }

                            if (!string.IsNullOrEmpty(ev.label.plus))
                                sb.Append(ev.label.plus);
                            if (!string.IsNullOrEmpty(ev.label.item))
                                sb.Append(ev.label.item);
                            if (!string.IsNullOrEmpty(ev.label.noise))
                                sb.Append(ev.label.noise);
                        }
                    }
                    timestamp = lastTime;
                    return sb.ToString();
                }

                public Event[] events { get; set; }


                public class Event
                {
                    public Timestamp timestamp { get; set; }
                    public Meta meta { get; set; }
                    public Label label { get; set; }


                    public class Timestamp
                    {
                        public string timestamp { get; set; }
                        public string recovery { get; set; }
                    }

                    public class Meta
                    {
                        public Confidence confidence { get; set; }


                        public class Confidence
                        {
                            public float value { get; set; }
                        }
                    }
                    public class Label
                    {
                        public string plus { get; set; }
                        public string item { get; set; }
                        public string noise { get; set; }
                    }

                }
            }
        }
    }
}
