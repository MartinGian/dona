using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using dona.Forms.Events;
using dona.Forms.Model;
using Newtonsoft.Json;
using Plugin.Messaging;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using dona.Forms.Exceptions;
using System.Diagnostics;

namespace dona.Forms.Services
{
    public class CreditService
    {
        private readonly ISmsService _smsService;

        private const string GetCreditNumber = "226";
        private const string GetCreditMessage = "Saldo";
        private static readonly TimeSpan GetCreditTimeout = TimeSpan.FromMinutes(1.5);
        private Credit _credit;

        public static CreditService Instance = new CreditService();

        private CreditService()
        {
            _smsService = new SmsService();
        }

        public async Task<Credit> GetCreditAsync()
        {
            try
            {
                var cachedCredit = GetCachedCredit();
                if (cachedCredit != null) return cachedCredit;

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
                                _credit = amount.Value == -1 ? new UnlimitedCredit() : new Credit(amount.Value);
                            }
                            else _credit = null;
                        }
                    }
                    catch (Exception)
                    {
                        _credit = null;
                    }
                    finally
                    {
                        manualResetEvent.Set();
                    }
                });

                await _smsService.SendSmsAsync(GetCreditNumber, GetCreditMessage);

                await Task.Run(() =>
                {
                    if (!manualResetEvent.WaitOne(GetCreditTimeout))
                        _credit = null;
                });

                MessagingCenter.Unsubscribe<CreditInformationReceivedEvent>(this, "SmsMessageReceived");

                return _credit;
            }
            catch (Exception)
            {
                MessagingCenter.Unsubscribe<CreditInformationReceivedEvent>(this, "SmsMessageReceived");
                throw;
            }
        }

        public Credit GetCachedCredit()
        {
            if (_credit is UnlimitedCredit) return _credit;

            if (_credit != null && DateTime.Now - _credit.Date < TimeSpan.FromHours(1))
                return _credit;

            return null;
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
