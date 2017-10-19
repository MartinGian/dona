using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using dona.Forms.Events;
using dona.Forms.Model;
using Xamarin.Forms;

namespace dona.Forms.Services
{
    public class CreditService
    {
        private readonly ISmsService _smsService;

        private const string GetCreditNumber = "226";
        private const string GetCreditMessage = "Saldo";
        private static readonly TimeSpan GetCreditTimeout = TimeSpan.FromMinutes(2);

        public static CreditService Instance = new CreditService();

        private CreditService()
        {
            _smsService = new SmsService();
        }

        public async Task<Credit> GetCreditAsync()
        {
            try
            {
                Credit credit = null;
                var manualResetEvent = new ManualResetEvent(false);

                MessagingCenter.Subscribe<CreditInformationReceivedEvent>(this, "SmsMessageReceived", e =>
                {
                    try
                    {
                        if (e.Number == GetCreditNumber)
                        {
                            var amount = ParseCreditMessage(e.Message);
                            if (amount.HasValue)
                            {
                                credit = amount.Value == -1 ? new UnlimitedCredit() : new Credit(amount.Value);
                            }
                        }
                    }
                    finally
                    {
                        manualResetEvent.Set();
                    }
                });

                await _smsService.SendSmsAsync(GetCreditNumber, GetCreditMessage);

                await Task.Run(() =>
                {
                    manualResetEvent.WaitOne(GetCreditTimeout);
                });

                return credit;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                MessagingCenter.Unsubscribe<CreditInformationReceivedEvent>(this, "SmsMessageReceived");
            }
        }

        private static double? ParseCreditMessage(string message)
        {
            var getCreditRegex = new Regex(@"(?<credit>\d+\.*\d*)(?:(\s\$))");
            var getCreditRegexMatch = getCreditRegex.Match(message);
            if (getCreditRegexMatch.Success)
            {
                var credit = getCreditRegexMatch.Groups["credit"];
                if (double.TryParse(credit.Value, out var dCredit))
                {
                    return dCredit;
                }
            }

            var unlimitedCreditRegex = new Regex(@"(Navegacion ilimitada)");
            if (unlimitedCreditRegex.IsMatch(message))
                return -1;

            return null;
        }
    }
}
