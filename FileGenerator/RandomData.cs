namespace FileGenerator
{
    internal static class RandomData
    {
        private static readonly Random _random = new Random();

        private static readonly DateTime _dateStart;
        private static readonly int _daysRange;
        private static readonly string _engSymbols;
        private static readonly string _rusSymbols;

        static RandomData()
        {
            _dateStart = DateTime.Today.AddYears(-5);
            _daysRange = (DateTime.Today - _dateStart).Days;
            _engSymbols = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            _rusSymbols = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя";
        }

        internal static string GetRandomDate(string outputDateFormat = "dd.MM.yyyy")
        {
            return _dateStart.AddDays(_random.Next(_daysRange)).ToString(outputDateFormat);
        }

        internal static string GetRandomEngSymbols(int count = 10)
        {
            char[] symbols = new char[count];

            while (count-- > 0)
            {
                symbols[count] = _engSymbols[_random.Next(_engSymbols.Length)];
            }

            return new string(symbols);
        }

        internal static string GetRandomRusSymbols(int count = 10)
        {
            char[] symbols = new char[count];

            while (count-- > 0)
            {
                symbols[count] = _rusSymbols[_random.Next(_rusSymbols.Length)];
            }

            return new string(symbols);
        }

        internal static string GetRandomInt(int minValue = 1, int maxValue = 100000000)
        {
            return _random.Next(minValue, maxValue).ToString();
        }

        internal static string GetRandomDouble(int minValue = 1, int maxValue = 2000000000)
        {
            return _random.Next(minValue, maxValue).ToString().Insert(2, ",");
        }
    }
}