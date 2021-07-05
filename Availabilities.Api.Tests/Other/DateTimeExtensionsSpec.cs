using System;
using Availabilities.Other;
using FluentAssertions;
using Xunit;

namespace Availabilities.Api.Tests.Other
{
    [Trait("Category", "Unit")]
    public class DateTimeExtensionsSpec
    {
        private readonly DateTime noonToday;
        private readonly DateTime quarterPast;
        private readonly DateTime quarterTo;

        public DateTimeExtensionsSpec()
        {
            this.noonToday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 00, 00,
                DateTimeKind.Utc);
            this.quarterPast = this.noonToday.AddMinutes(15);
            this.quarterTo = this.noonToday.SubtractMinutes(15);
        }

        [Fact]
        public void WhenToNextOrCurrentQuarterHourAndOnTheHour_ThenReturnsOnTheHour()
        {
            var result = this.noonToday.ToNextOrCurrentQuarterHour();

            result.Should().Be(this.noonToday);
        }

        [Fact]
        public void WhenToNextOrCurrentQuarterHourAndJustPastTheHour_ThenReturnsOnTheHour()
        {
            var justPastHour = this.noonToday.AddSeconds(1);
            var result = justPastHour.ToNextOrCurrentQuarterHour();

            result.Should().Be(this.noonToday);
        }


        [Fact]
        public void WhenToNextOrCurrentQuarterHourAndAMinutePastTheHour_ThenReturnsQuarterPastTheHour()
        {
            var justPastHour = this.noonToday.AddMinutes(1);
            var result = justPastHour.ToNextOrCurrentQuarterHour();

            result.Should().Be(this.quarterPast);
        }

        [Fact]
        public void WhenToNextOrCurrentQuarterHourAndQuarterToTheHour_ThenReturnsQuarterToTheHour()
        {
            var result = this.quarterTo.ToNextOrCurrentQuarterHour();

            result.Should().Be(this.quarterTo);
        }


        [Fact]
        public void WhenToNextOrCurrentQuarterHourAndJustBeforeTheHour_ThenReturnsOnTheHour()
        {
            var justBeforeHour = this.noonToday.SubtractSeconds(1);
            var result = justBeforeHour.ToNextOrCurrentQuarterHour();

            result.Should().Be(this.noonToday);
        }


        [Fact]
        public void WhenToNextOrCurrentQuarterHourAndAMinuteBeforeTheHour_ThenReturnsOnTheHour()
        {
            var justBeforeHour = this.noonToday.SubtractMinutes(1);
            var result = justBeforeHour.ToNextOrCurrentQuarterHour();

            result.Should().Be(this.noonToday);
        }
    }
}