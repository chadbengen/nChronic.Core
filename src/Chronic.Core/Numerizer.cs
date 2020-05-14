﻿using System;
using System.Text.RegularExpressions;
using Chronic.Core.System;

namespace Chronic.Core
{
    public static class Numerizer
    {
        static readonly dynamic[,] DIRECT_NUMS = new dynamic[,]
            {
                {"o one", "01"},
                {"o two", "02"},
                {"o three", "03"},
                {@"o four(\W|$)", "04$1"},
                {"o five", "05"},
                {@"o six(\W|$)", "06$1"},
                {@"o seven(\W|$)", "07$1"},
                {@"o eight(\W|$)", "08$1"},
                {@"o nine(\W|$)", "09$1"},
                {"oh one", "01"},
                {"oh two", "02"},
                {"oh three", "03"},
                {@"oh four(\W|$)", "04$1"},
                {"oh five", "05"},
                {@"oh six(\W|$)", "06$1"},
                {@"oh seven(\W|$)", "07$1"},
                {@"oh eight(\W|$)", "08$1"},
                {@"oh nine(\W|$)", "09$1"},
                {"eleven", "11"},
                {"twelve", "12"},
                {"thirteen", "13"},
                {"fourteen", "14"},
                {"fifteen", "15"},
                {"sixteen", "16"},
                {"seventeen", "17"},
                {"eighteen", "18"},
                {"nineteen", "19"},
                {"ninteen", "19"}, // Common mis-spelling
                {"zero", "0"},
                {"one", "1"},
                {"two", "2"},
                {"three", "3"},
                {@"four(\W|$)", "4$1"},
                // The weird regex is so that it matches four but not fourty
                {"five", "5"},
                {@"six(\W|$)", "6$1"},
                {@"seven(\W|$)", "7$1"},
                {@"eight(\W|$)", "8$1"},
                {@"nine(\W|$)", "9$1"},
                {"ten", "10"},
                {@"\ba[\b^$]", "1"}
                // doesn"t make sense for an "a" at the end to be a 1
            };

        static readonly dynamic[,] ORDINALS = new dynamic[,]
            {
                {"first", "1"},
                {"third", "3"},
                {"fourth", "4"},
                {"fifth", "5"},
                {"sixth", "6"},
                {"seventh", "7"},
                {"eighth", "8"},
                {"ninth", "9"},
                {"tenth", "10"}
            };

        static readonly dynamic[,] TEN_PREFIXES = new dynamic[,]
            {
                {"twenty", 20},
                {"thirty", 30},
                {"forty", 40},
                {"fourty", 40}, // Common mis-spelling
                {"fifty", 50},
                {"sixty", 60},
                {"seventy", 70},
                {"eighty", 80},
                {"ninety", 90}
            };

        static readonly dynamic[,] BIG_PREFIXES = new dynamic[,]
            {
                {"hundred", 100},
                {"thousand", 1000},
                {"million", 1000000},
                {"billion", 1000000000},
                {"trillion", 1000000000000},
            };

        public static string Numerize(string value, bool intendTime)
        {
            var result = value;
            string pattern;
            MatchCollection matches;

            // preprocess
            result = @" +|([^\d])-([^\d])".Compile().Replace(result, "$1 $2");
            // will mutilate hyphenated-words but shouldn't matter for date extraction
            result = result.Replace("a half", "haAlf");
            // take the 'a' out so it doesn't turn into a 1, save the half for the end

            // easy/direct replacements

            DIRECT_NUMS.ForEach<string, string>(
                (p, r) =>
                    result =
                    Regex.Replace(
                        result,
                        p,
                        "<num>" + r));

            ORDINALS.ForEach<string, string>(
                (p, r) =>
                    result =
                    Regex.Replace(
                        result,
                        p,
                        "<num>" + r +
                            p.
                            LastCharacters
                            (2)));

            // ten, twenty, etc.

            TEN_PREFIXES.ForEach<string, int>(
                (p, r) =>
                    result =
                    Regex.Replace(
                        result,
                        "(?:" + p + @") *<num>(\d(?=[^\d]|$))*",
                        match => "<num>" + (r + int.Parse(match.Groups[1].Value))));

            var prefix = intendTime ? "<ten><num>" : "<num>";

            TEN_PREFIXES.ForEach<string, int>(
                (p, r) => result = Regex.Replace(result, p, prefix + r.ToString()));


            if (intendTime)
            {
                var tens = result.Split(new string[] { "<ten>" }, StringSplitOptions.None);
                var newResult = tens[0];

                for (int i = 1; i < tens.Length; i++)
                {
                    var clean = tens[i].Replace("- <num>", "<num>");
                    newResult += Andition(clean);
                }
                result = newResult;

                pattern = @"<num>\b(0*(?:[0-9][0-9]?))\b";
                matches = Regex.Matches(result, pattern);
                if (matches.Count > 0)
                {
                    for (int i = matches.Count - 1; i > 0; i--)
                    {
                        dynamic match = matches[i];
                        var index = match.Groups[1].Index;
                        if (match.Groups[1].Value.Length == 1)
                        {
                            result = result.Insert(index, "0");
                        }
                        result = result.Insert(index, ":");
                        var length = " <num>".Length;
                        result = result.Remove(index - length, length);
                    }
                }
            }
            else
            {
                result = result.Replace("<ten>", "");
            }

            // hundreds, thousands, millions, etc.

            BIG_PREFIXES.ForEach<string, long>(
                (p, r) =>
                    {
                        result = Regex.Replace(result, @"(?:<num>)?(\d*) *" + p, match => "<num>" + (r * int.Parse(match.Groups[1].Value)).ToString());
                        result = Andition(result);
                    });


            // fractional addition
            // I'm not combining this with the previous block as using float addition complicates the strings
            // (with extraneous .0"s and such )
            result = Regex.Replace(result, @"(\d +)(?: |and | -)*haAlf", match => (float.Parse(match.Groups[1].Value) + 0.5).ToString());
            result = result.Replace("<num>", "");

            return result;
        }

        static string Andition(string value)
        {
            var result = value;
            var pattern = @"<num>(\d+)( | and )<num>(\d+)(?=[^\w]|$)".Compile();
            while (true)
            {
                var match = pattern.Match(result);
                if (match.Success == false)
                    break;
                result = result.Substring(0, match.Index) +
                    "<num>" + ((int.Parse(match.Groups[1].Value) + int.Parse(match.Groups[3].Value)).ToString()) +
                    result.Substring(match.Index + match.Length);
            }
            return result;
        }
    }
}