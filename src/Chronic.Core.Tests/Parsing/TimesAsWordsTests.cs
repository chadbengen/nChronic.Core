using System;
using Chronic.Core.System;
using Chronic.Core.Tests.Utils;
using Humanizer;
using Xunit;
using Xunit.Abstractions;

namespace Chronic.Core.Tests.Parsing
{
    public class TimesAsWordsTests : ParsingTestsBase
    {
        private readonly ITestOutputHelper _outputHelper;

        public TimesAsWordsTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        protected override DateTime Now()
        {
            return DateTime.Now;
        }

        [Fact]
        public void one_pm()
        {
            Parse("one pm").AssertStartsAt(Time.New(Now(), 13, 0, 0));
        }

        [Fact]
        public void today_at_one_pm()
        {
            Parse("today one pm").AssertStartsAt(Time.New(Now(), 13, 0, 0));
        }
        [Fact]
        public void today_at_1_15_pm()
        {
            Parse("today 1:15 pm").AssertStartsAt(Time.New(Now(), 13, 15, 0));
        }

        [Fact]
        public void today_at_one_fifteen_pm()
        {
            Parse("today at one fifteen pm").AssertStartsAt(Time.New(Now(), 13, 15, 0));
        }

        [Fact]
        public void today_at_one_fifteen_am()
        {
            Parse("today at one fifteen am").AssertStartsAt(Time.New(Now(), 1, 15, 0));
        }
        [Fact]
        public void today_at_one_o_five()
        {
            Parse("today one o five").AssertStartsAt(Time.New(Now(), 1, 5, 0));
        }

        [Fact]
        public void today_at_one_oh_five_pm()
        {
            Parse("today one oh five pm").AssertStartsAt(Time.New(Now(), 13, 5, 0));
        }
        
        [Fact]
        public void today_at_one_five_pm()
        {
            Parse("today one five pm").AssertStartsAt(Time.New(Now(), 13, 5, 0));
        }

        [Fact]
        public void today_loop_minutes()
        {
            var date = DateTime.Now;
            var prefix = "today";
            var dayoffset = 0;

            // minutes
            for (int i = 1; i < 60; i++)
            {
                var input = $"{prefix} at one {i.ToWords()} am";
                var parsed = Parse(input, new Options { IntendingTime = true });
                _outputHelper.WriteLine(input);
                _outputHelper.WriteLine(parsed.Start.ToString());
                _outputHelper.WriteLine("");
                parsed.AssertStartsAt(Time.New(date.AddDays(dayoffset), 1, i, 0));
            }
        }
      
        [Fact]
        public void yesterday_loop_minutes()
        {
            var date = DateTime.Now;
            var prefix = "yesterday";
            var dayoffset = -1;

            // minutes
            for (int i = 1; i < 60; i++)
            {
                var input = $"{prefix} at one {i.ToWords()} am";
                var parsed = Parse(input, new Options { IntendingTime = true });
                _outputHelper.WriteLine(input);
                _outputHelper.WriteLine(parsed.Start.ToString());
                _outputHelper.WriteLine("");
                parsed.AssertStartsAt(Time.New(date.AddDays(dayoffset), 1, i, 0));
            }
        }

        [Fact]
        public void today_loop_hours()
        {
            var date = DateTime.Now;
            var prefix = "today";
            var dayoffset = 0;

            //hours
            new string[] { "am", "pm" }.ForEach(ampm =>
            {
                for (int i = 1; i < 13; i++)
                {
                    var input = $"{prefix} at {i.ToWords()} fifteen {ampm}";
                    var parsed = Parse(input, new Options { IntendingTime = true });
                    _outputHelper.WriteLine(input);
                    _outputHelper.WriteLine(parsed.Start.ToString());
                    _outputHelper.WriteLine("");

                    var expectedHour = i == 12 ? 0 : i;

                    expectedHour = ampm == "am"
                        ? expectedHour
                        : expectedHour + 12;

                    parsed.AssertStartsAt(Time.New(date.AddDays(dayoffset), expectedHour, 15, 0));
                }
            });
        }

        [Fact]
        public void yesterday_loop_hours()
        {
            var date = DateTime.Now;
            var prefix = "yesterday";
            var dayoffset = -1;

            //hours
            new string[] { "am", "pm" }.ForEach(ampm =>
            {
                for (int i = 1; i < 13; i++)
                {
                    var input = $"{prefix} at {i.ToWords()} fifteen {ampm}";
                    var parsed = Parse(input, new Options { IntendingTime = true });
                    _outputHelper.WriteLine(input);
                    _outputHelper.WriteLine(parsed.Start.ToString());
                    _outputHelper.WriteLine("");

                    var expectedHour = i == 12 ? 0 : i;

                    expectedHour = ampm == "am"
                        ? expectedHour
                        : expectedHour + 12;

                    parsed.AssertStartsAt(Time.New(date.AddDays(dayoffset), expectedHour, 15, 0));
                }
            });
        }

        [Fact]
        public void today_loop_military()
        {
            var date = DateTime.Now;
            var prefix = "today";
            var dayoffset = 0;

            // military
            for (int i = 12; i < 20; i++)
            {
                var input = $"{prefix} at {i.ToWords()} ten";
                var parsed = Parse(input, new Options { IntendingTime = true });
                _outputHelper.WriteLine(input);
                _outputHelper.WriteLine(parsed.Start.ToString());
                _outputHelper.WriteLine("");
                parsed.AssertStartsAt(Time.New(date.AddDays(dayoffset), i, 10, 0));
            }
        }
        [Fact]
        public void yesterday_loop_military()
        {
            var date = DateTime.Now;
            var prefix = "yesterday";
            var dayoffset = -1;

            // military
            for (int i = 12; i < 20; i++)
            {
                var input = $"{prefix} at {i.ToWords()} ten";
                var parsed = Parse(input, new Options { IntendingTime = true });
                _outputHelper.WriteLine(input);
                _outputHelper.WriteLine(parsed.Start.ToString());
                _outputHelper.WriteLine("");
                parsed.AssertStartsAt(Time.New(date.AddDays(dayoffset), i, 10, 0));
            }
        }

    }
}