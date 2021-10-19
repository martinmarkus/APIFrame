using Hangfire;

namespace APIFrame.BackgroundService.Constants
{
    public static class CronTimeValue
    {
        public static string Minutely => Cron.Minutely();
        public static string Hourly => Cron.Hourly();
        public static string Daily => Cron.Daily();
        public static string Weekly => Cron.Weekly();
        public static string Monthly => Cron.Monthly();
        public static string Yearly => Cron.Yearly();

        public static string SecondInterval(int second = 1) =>
            $"*/{second} * * * * *";

        public static string MinuteInterval(int minute = 1) =>
            $"*/{minute} * * * *";

        public static string HourInterval(int hour, int atMinute = 0) =>
            $"{atMinute} */{hour} * * *";

        public static string DayInterval(int day, int atHour = 0, int atMinute = 0) =>
            $"{atMinute} {atHour} */{day} * *";

        public static string Midnight() =>
            $"0 0 * * *";

        public static string AtReboot() =>
            "@reboot";

        public static string Annually() =>
            "@annually";
    }
}
