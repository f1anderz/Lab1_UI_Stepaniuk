using System;
using System.Windows;
using System.Windows.Controls;

namespace FinanceCalculator
{
    public partial class MainWindow : Window
    {
        private const double TaxRate = 0.195;

        public MainWindow()
        {
            InitializeComponent();
            TermInput.Minimum = 1;
            TermInput.Maximum = 24;
            TermInput.Value = 3;
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(SumInput.Text, out double principal) || principal <= 0)
            {
                MessageBox.Show("Будь ласка, введіть коректну суму депозиту.");
                return;
            }

            int months = (int)TermInput.Value;

            // Базова ставка: від 4% до 16% залежно від місяців
            double baseAnnualRate = 4 + (12.0 * (months - 1) / 23);

            // Якщо дострокове розірвання — ставка знижується вдвічі
            if (EarlyTerminationCheck.IsChecked == true)
                baseAnnualRate /= 2;

            double monthlyRate = baseAnnualRate / 12 / 100;

            bool isCompound = CompoundCheck.IsChecked == true;

            double finalAmount = principal;
            double interestEarned;

            if (isCompound)
            {
                for (int i = 0; i < months; i++)
                    finalAmount += finalAmount * monthlyRate;

                interestEarned = finalAmount - principal;
            }
            else
            {
                interestEarned = principal * monthlyRate * months;
                finalAmount = principal + interestEarned;
            }

            double taxAmount = interestEarned * TaxRate;
            double netInterest = interestEarned - taxAmount;
            double netFinalAmount = principal + netInterest;

            double effectiveRate = (netInterest / principal) / (months / 12.0) * 100;

            // Вивід результатів
            ResultAmountText.Text = $"{netFinalAmount:0.##} ₴";
            InvestedText.Text = $"{principal:0.##} ₴";
            RateText.Text = $"{baseAnnualRate:0.##}% річних";
            TaxedRateText.Text = $"{effectiveRate:0.##}%";
            TaxAmountText.Text = $"{taxAmount:0.##} ₴";
            EarningsText.Text = $"Прибуток: {netInterest:0.##} ₴";
        }

        private void TermInput_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!TermOutput.IsLoaded)
            {
                return;
            }
            int value = (int)TermInput.Value;
            double rate = 4 + (12.0 * (value - 1) / 23);

            if (EarlyTerminationCheck?.IsChecked == true)
                rate /= 2;

            string unit = value == 1 ? "місяць" :
                          (value >= 2 && value <= 4) ? "місяці" : "місяців";
            TermOutput.Text = $"{value} {unit} під {rate:0.##}%";
        }
    }
}
