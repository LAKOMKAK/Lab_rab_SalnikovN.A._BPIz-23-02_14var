using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace Lab1_Wpf_Var14_Product
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // ===== Валидация ввода: число с десятичным разделителем =====
        private void DecimalOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Разрешаем: цифры, точку, запятую
            foreach (char ch in e.Text)
            {
                if (!char.IsDigit(ch) && ch != '.' && ch != ',')
                {
                    e.Handled = true;
                    return;
                }
            }
        }

        // ===== Запрещаем пробел =====
        private void NoSpace_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void BtnToEuro_Click(object sender, RoutedEventArgs e)
        {
            if (!TryReadAll(out Product product, out decimal rate, out decimal percent)) return;

            decimal euro = product.GetPriceEuro(rate);
            TbResult.Text =
                $"Товар: {product}\n" +
                $"Курс: {rate:0.####} руб/€\n" +
                $"Цена в евро: {euro:0.####} €";
        }

        private void BtnIncIfSamsung_Click(object sender, RoutedEventArgs e)
        {
            if (!TryReadAll(out Product product, out decimal rate, out decimal percent)) return;

            decimal euroBefore = product.GetPriceEuro(rate);
            decimal euroAfter = product.IncreaseEuroIfSamsung(rate, percent);

            bool containsSamsung = product.Name.IndexOf("Samsung", StringComparison.OrdinalIgnoreCase) >= 0;

            TbResult.Text =
                $"Товар: {product}\n" +
                $"Курс: {rate:0.####} руб/€\n" +
                $"Цена в евро (до): {euroBefore:0.####} €\n" +
                (containsSamsung
                    ? $"Название содержит 'Samsung' → увеличили на {percent:0.####}%\nЦена в евро (после): {euroAfter:0.####} €"
                    : "Название НЕ содержит 'Samsung' → цена не менялась\nЦена в евро (после): " + $"{euroAfter:0.####} €");
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            TbName.Clear();
            TbPriceRub.Clear();
            TbManufacturer.Clear();
            TbRate.Text = "100";
            TbPercent.Text = "10";
            TbResult.Text = "—";
            TbName.Focus();
        }

        private bool TryReadAll(out Product product, out decimal rate, out decimal percent)
        {
            product = null!;
            rate = 0;
            percent = 0;

            string name = TbName.Text?.Trim() ?? "";
            string manuf = TbManufacturer.Text?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(manuf))
            {
                MessageBox.Show("Заполните наименование и изготовителя.", "Ошибка ввода",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!TryParseDecimal(TbPriceRub.Text, out decimal priceRub) ||
                !TryParseDecimal(TbRate.Text, out rate) ||
                !TryParseDecimal(TbPercent.Text, out percent))
            {
                MessageBox.Show("Проверьте числовые поля (цена, курс, процент).", "Ошибка ввода",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            try
            {
                product = new Product(name, priceRub, manuf);

                if (rate <= 0)
                    throw new ArgumentOutOfRangeException(nameof(rate), "Курс (руб/€) должен быть > 0.");
                if (percent < 0)
                    throw new ArgumentOutOfRangeException(nameof(percent), "Процент должен быть >= 0.");

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
        }

        private static bool TryParseDecimal(string? text, out decimal value)
        {
            value = 0m;
            text = (text ?? string.Empty).Trim();
            if (text.Length == 0) return false;

            // Разрешаем ввод и через запятую, и через точку
            text = text.Replace(',', '.');

            return decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
        }
    }
}
