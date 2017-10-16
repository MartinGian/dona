using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace dona.Forms.Model
{
    public class Credit : INotifyPropertyChanged
    {
        public double Amount { get; set; }
        public DateTime Date { get; set; }

        private string _formattedCredit;
        public string FormattedCredit
        {
            get => _formattedCredit;
            set
            {
                _formattedCredit = value;
                OnPropertyChanged();
            }
        }

        public Credit(double amount)
        {
            Amount = amount;
            _formattedCredit = $"Su saldo es de: ${amount}";
            Device.StartTimer(TimeSpan.FromMinutes(1), () =>
            {
                var creditTime = DateTime.Now - Date;
                FormattedCredit = GetFormattedCredit(creditTime);
                return true;
            });
        }

        protected Credit()
        {
            Date = DateTime.Now;
        }

        private string GetFormattedCredit(TimeSpan creditTime)
            => creditTime.Minutes == 1 ? $"Su saldo es de: ${Amount} (hace 1 minuto)" : $"Su saldo es de: ${Amount} (hace {creditTime.Minutes} minutos)";

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class UnlimitedCredit : Credit
    {
        public UnlimitedCredit()
        {
            FormattedCredit = "Dispone de saldo ilimitado. Sus donaciones se cargarán en su factura.";
        }
    }
}