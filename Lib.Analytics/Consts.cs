using System;
using System.Linq;

namespace HlidacStatu.Lib.Analytics
{
    public static class Consts
    {
        public static int[] RegistrSmluvYearsList =  Enumerable.Range(2016, DateTime.Now.Year - 2016+1).ToArray();

    }
}
