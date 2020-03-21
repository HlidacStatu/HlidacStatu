using Nest;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Drawing.Imaging;
using Devmasters.Imaging;

namespace HlidacStatu.Lib
{
    public static class RenderTools
    {

        public static void ProgressWriter_OutputFunc_EndIn(Devmasters.Core.Batch.ActionProgressData data)
        {
            DateTime end = data.EstimatedFinish;
            string send = "";
            if (data.EstimatedFinish > DateTime.MinValue)
            {
                TimeSpan endIn = data.EstimatedFinish - DateTime.Now;
                send = FormatAvailability(endIn, DateTimePart.Second);

            }

            Console.WriteLine(
                string.Format($"\n{data.Prefix}{DateTime.Now.ToLongTimeString()}: {data.ProcessedItems}/{data.TotalItems} {data.PercentDone}%  End in {send}")
                );
        }



        public enum DateTimePart
        {
            Year = 1,
            Month = 2,
            Day = 3,
            Hour = 4,
            Minute = 5,
            Second = 6
        }

        public static string FormatAvailability(TimeSpan ts, DateTimePart minDatePart, string numFormat = "N1")
        {

            var end = DateTime.Now;
            Devmasters.Core.DateTimeSpan dts = Devmasters.Core.DateTimeSpan.CompareDates(end - ts, end);
            string s = "";
            if (dts.Years > 0 && minDatePart > DateTimePart.Year)
            {
                s += " " + HlidacStatu.Util.PluralForm.Get(dts.Years, "{0} rok;{0} roky;{0} let");
            }
            else if (dts.Years > 0)
            {
                decimal part = dts.Years + dts.Months / 12m;
                if (part % 1 > 0)
                    s += string.Format(" {0:" + numFormat + "} let", part);
                else
                    s += HlidacStatu.Util.PluralForm.Get((int)part, " {0} rok; {0} roky; {0} let"); ;
                return s;
            }

            if (dts.Months > 0 && minDatePart > DateTimePart.Month)
            {
                s += " " + HlidacStatu.Util.PluralForm.Get(dts.Months, "{0} měsíc;{0} měsíce;{0} měsíců");
            }
            else if (dts.Months > 0)
            {
                decimal part = dts.Months + dts.Days / 30m;
                if (part % 1 > 0)
                    s += string.Format(" {0:" + numFormat + "} měsíců", part);
                else
                    s += HlidacStatu.Util.PluralForm.Get((int)part, " {0} měsíc; {0} měsíce; {0} měsíců"); ;
                return s;
            }

            if (dts.Days > 0 && minDatePart > DateTimePart.Day)
            {
                s = " " + HlidacStatu.Util.PluralForm.Get(dts.Days, " {0} den;{0} dny;{0} dnů");
            }
            else if (dts.Days > 0)
            {
                decimal part = dts.Days + dts.Hours / 24m;
                if (part % 1 > 0)
                    s = " " + string.Format(" {0:" + numFormat + "} dní", part);
                else
                    s = " " + HlidacStatu.Util.PluralForm.Get((int)part, " {0} den;{0} dny;{0} dnů"); ;
                return s;
            }

            if (dts.Hours > 0 && minDatePart > DateTimePart.Hour)
            {
                s += " " + HlidacStatu.Util.PluralForm.Get(dts.Hours, " {0} hodinu;{0} hodiny;{0} hodin");
            }
            else if (dts.Hours > 0)
            {
                decimal part = dts.Hours + dts.Minutes / 60m;
                if (part % 1 > 0)
                    s += string.Format(" {0:" + numFormat + "} hodin", part);
                else
                    s += " " + HlidacStatu.Util.PluralForm.Get((int)part, " {0} hodinu;{0} hodiny;{0} hodin");
                return s;
            }

            if (dts.Minutes > 0 && minDatePart > DateTimePart.Minute)
            {
                s += " " + HlidacStatu.Util.PluralForm.Get(dts.Minutes, "minutu;{0} minuty;{0} minut");
            }
            else if (dts.Minutes > 0)
            {
                decimal part = dts.Minutes + dts.Seconds / 60m;
                if (part % 1 > 0)
                    s += string.Format(" {0:" + numFormat + "} minut", part);
                else
                    s += " " + HlidacStatu.Util.PluralForm.Get((int)part, "minutu;{0} minuty;{0} minut"); ;
                return s;
            }

            if (dts.Seconds > 0 && minDatePart > DateTimePart.Second)
            {
                s += " " + HlidacStatu.Util.PluralForm.Get(dts.Seconds, "sekundu;{0} sekundy;{0} sekund");
            }
            else
            {
                decimal part = dts.Seconds + dts.Milliseconds / 1000m;
                if (part % 1 > 0)
                    s += string.Format(" {0:" + numFormat + "} sekund", part);
                else
                    s += " " + HlidacStatu.Util.PluralForm.Get((int)part, "sekundu;{0} sekundy;{0} sekund"); ;
                return s;
            }

            //if (dts.Milliseconds > 0)
            //    s += " " + HlidacStatu.Util.PluralForm.Get(dts.Milliseconds, "{0} ms;{0} ms;{0} ms");

            return s.Trim();

        }

        public static string FormatAvailability2(TimeSpan ts)
        {
            var end = DateTime.Now;
            Devmasters.Core.DateTimeSpan dts = Devmasters.Core.DateTimeSpan.CompareDates(end - ts, end);
            string s = "";
            if (dts.Years > 0)
            {
                s += " " + HlidacStatu.Util.PluralForm.Get(dts.Years, "rok;{0} roky;{0} let");
            }
            if (dts.Months > 0)
            {
                s += " " + HlidacStatu.Util.PluralForm.Get(dts.Months, "měsíc;{0} měsíce;{0} měsíců");
            }
            if (dts.Days > 0)
            {
                s += " " + HlidacStatu.Util.PluralForm.Get(dts.Days, "den;{0} dny;{0} dnů");
            }
            if (dts.Hours > 0)
            {
                s += " " + HlidacStatu.Util.PluralForm.Get(dts.Hours, "hodinu;{0} hodiny;{0} hodin");
            }
            if (dts.Minutes > 0)
            {
                s += " " + HlidacStatu.Util.PluralForm.Get(dts.Minutes, "minutu;{0} minuty;{0} minut");
            }
            if (dts.Seconds > 0)
            {
                s += " " + HlidacStatu.Util.PluralForm.Get(dts.Seconds, "sekundu;{0} sekundy;{0} sekund");
            }
            if (dts.Milliseconds > 0)
                s += " " + HlidacStatu.Util.PluralForm.Get(dts.Milliseconds, "{0} ms;{0} ms;{0} ms");

            return s.Trim();

        }

        public static string DateDiffShort(DateTime first, DateTime sec, string beforeTemplate, string afterTemplate)
        {
            if (first < DateTime.MinValue)
                first = DateTime.MinValue;
            if (sec < DateTime.MinValue)
                sec = DateTime.MinValue;
            if (first > DateTime.MaxValue)
                first = DateTime.MaxValue;
            if (sec > DateTime.MaxValue)
                sec = DateTime.MaxValue;

            bool after = first > sec;
            Devmasters.Core.DateTimeSpan dateDiff = Devmasters.Core.DateTimeSpan.CompareDates(first, sec);
            string txtDiff = string.Empty;
            if (dateDiff.Years > 0)
            {
                txtDiff = HlidacStatu.Util.PluralForm.Get(dateDiff.Years, "rok;{0} roky;{0} let");
            }
            else if (dateDiff.Months > 3)
            {
                txtDiff = HlidacStatu.Util.PluralForm.Get(dateDiff.Months, "měsíc;{0} měsíce;{0} měsíců");
            }
            else
            {
                txtDiff = Devmasters.Core.Lang.Plural.GetWithZero(dateDiff.Days,  "dnes","den","{0} dny","{0} dnů");
            }

            if (after)
                return string.Format(afterTemplate, txtDiff);
            else
                return string.Format(beforeTemplate, txtDiff);

        }


        public static string DateDiffShort_7pad(DateTime first, DateTime sec, string beforeTemplate, string afterTemplate)
        {
            if (first < DateTime.MinValue)
                first = DateTime.MinValue;
            if (sec < DateTime.MinValue)
                sec = DateTime.MinValue;
            if (first > DateTime.MaxValue)
                first = DateTime.MaxValue;
            if (sec > DateTime.MaxValue)
                sec = DateTime.MaxValue;

            bool after = first > sec;
            Devmasters.Core.DateTimeSpan dateDiff = Devmasters.Core.DateTimeSpan.CompareDates(first, sec);
            if (after)
            {
                string txtDiff = string.Empty;
                if (dateDiff.Years > 0)
                {
                    txtDiff = HlidacStatu.Util.PluralForm.Get(dateDiff.Years, "roce;{0} letech;{0} letech");
                }
                else if (dateDiff.Months > 3)
                {
                    txtDiff = HlidacStatu.Util.PluralForm.Get(dateDiff.Months, "měsíci;{0} měsících;{0} měsících");
                }
                else
                {
                    txtDiff = Devmasters.Core.Lang.Plural.GetWithZero(dateDiff.Days, "pár hodinách", "jednom dni", "{0} dnech", "{0} dny", "{0} dnů");
                }

                return string.Format(afterTemplate, txtDiff);
            }
            else
            {
                string txtDiff = string.Empty;
                if (dateDiff.Years > 0)
                {
                    txtDiff = HlidacStatu.Util.PluralForm.Get(dateDiff.Years, "rokem;{0} roky;{0} roky");
                }
                else if (dateDiff.Months > 3)
                {
                    txtDiff = HlidacStatu.Util.PluralForm.Get(dateDiff.Months, "měsícem;{0} měsíci;{0} měsíci");
                }
                else
                {
                    txtDiff = Devmasters.Core.Lang.Plural.GetWithZero(dateDiff.Days, "pár hodinami", "dnem", "{0} dny", "{0} dny");
                }

                return string.Format(beforeTemplate, txtDiff);
            }

        }

        public static IEnumerable<string> DetectAndParseFacesIntoFiles(string imageFile, int minSize, int marginInPercent)
        {
            return DetectAndParseFacesIntoFiles(System.IO.File.ReadAllBytes(imageFile), minSize, marginInPercent);
        }

        public static IEnumerable<string> DetectAndParseFacesIntoFiles(byte[] imageData, int minSize, int marginInPercent)
        {
            List<string> files = new List<string>();
            var fnroot = System.IO.Path.GetTempFileName();
            int count = 0;
            foreach (var img in DetectAndParseFaces(imageData, minSize, marginInPercent))
            {
                var fn = fnroot + "." + count+".faces.jpg";
                System.IO.File.WriteAllBytes(fn, img);
                files.Add(fn);
                count++;
            }
            return files;
        }

        public static IEnumerable<byte[]> DetectAndParseFaces(Byte[] imagedata, int minSize, int marginInPercent)
        {
            using (Devmasters.Imaging.InMemoryImage image = new InMemoryImage(imagedata))
            {
                if (marginInPercent > 99 || marginInPercent < 0)
                    throw new ArgumentOutOfRangeException("marginInPercent", "must be between 0 and 100");
                List<byte[]> facesImg = new List<byte[]>();
                CascadeClassifier _cascadeClassifier;
                _cascadeClassifier = new CascadeClassifier(Lib.Init.WebAppDataPath + "haarcascade_frontalface_default.xml");
                //Image<Bgr, byte> img = Image<Bgr, byte>.FromIplImagePtr(image.Image.GetHbitmap);  
                Image<Bgr, byte> img = new Image<Bgr, byte>(image.Image);
                Image<Gray, byte> grayframe = img.Convert<Gray, byte>();
                var faces = _cascadeClassifier.DetectMultiScale(grayframe, 1.1, 10, Size.Empty); //the actual face detection happens here
                foreach (var face in faces)
                {
                    if (face.Width < minSize || face.Height < minSize)
                        continue;

                    int newX = face.X;
                    int newY = face.Y;
                    int changeX = (int)Math.Round(face.Width * ((double)marginInPercent / 100D));
                    int changeY = (int)Math.Round(face.Height * ((double)marginInPercent / 100D));
                    newX = newX - changeX / 2; newX = newX < 0 ? 0 : newX;
                    newY = newY - changeY / 2; newY = newY < 0 ? 0 : newY;
                    int newWidth = face.Width + changeX;
                    int newHeight = face.Height + changeY;

                    if (newX + newWidth > image.Image.Width)
                        newWidth = image.Image.Width - newX;
                    if (newY + newHeight > image.Image.Height)
                        newHeight = image.Image.Height - newY;

                    Rectangle newFaceRect = new Rectangle(newX, newY, newWidth, newHeight);

                    //img.Draw(newFaceRect, new Bgr(Color.BurlyWood), 3); //the detected face(s) is highlighted here using a box that is drawn around it/them
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                    {
                        image.SaveAsJPEG(ms, 95);
                        using (InMemoryImage imi = new InMemoryImage(ms.ToArray()))
                        {
                            imi.Crop(newFaceRect);

                            using (System.IO.MemoryStream lms = new System.IO.MemoryStream())
                            {
                                imi.SaveAsJPEG(lms, 95);
                                facesImg.Add(lms.ToArray());
                            }
                        }
                    }
                }

                return facesImg;
            }
        }

    }
}
