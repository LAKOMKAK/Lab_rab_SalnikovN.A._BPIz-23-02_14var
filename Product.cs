using System;

namespace Lab1_Wpf_Var14_Product
{
    /// <summary>
    /// Товар: наименование, цена в рублях, изготовитель.
    /// </summary>
    public sealed class Product
    {
        public string Name { get; }
        public decimal PriceRub { get; }
        public string Manufacturer { get; }

        public Product(string name, decimal priceRub, string manufacturer)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Наименование не должно быть пустым.", nameof(name));
            if (priceRub < 0)
                throw new ArgumentOutOfRangeException(nameof(priceRub), "Цена в рублях должна быть >= 0.");
            if (string.IsNullOrWhiteSpace(manufacturer))
                throw new ArgumentException("Изготовитель не должен быть пустым.", nameof(manufacturer));

            Name = name.Trim();
            PriceRub = priceRub;
            Manufacturer = manufacturer.Trim();
        }

        /// <summary>
        /// Метод 1 (вариант 14): пересчитать цену из рублей в евро по курсу (руб/€).
        /// </summary>
        public decimal GetPriceEuro(decimal rubPerEuro)
        {
            if (rubPerEuro <= 0)
                throw new ArgumentOutOfRangeException(nameof(rubPerEuro), "Курс (руб/€) должен быть > 0.");

            return PriceRub / rubPerEuro;
        }

        /// <summary>
        /// Метод 2 (вариант 14): увеличить цену в евро, если название содержит "Samsung".
        /// Возвращает итоговую цену в евро.
        /// </summary>
        public decimal IncreaseEuroIfSamsung(decimal rubPerEuro, decimal percent)
        {
            if (percent < 0)
                throw new ArgumentOutOfRangeException(nameof(percent), "Процент увеличения должен быть >= 0.");

            decimal euro = GetPriceEuro(rubPerEuro);

            if (Name.IndexOf("Samsung", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                euro = euro * (1m + percent / 100m);
            }

            return euro;
        }

        public override string ToString() => $"{Name} ({Manufacturer}), {PriceRub:0.##} RUB";
    }
}
