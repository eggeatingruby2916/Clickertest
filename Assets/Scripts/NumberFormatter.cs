public static class NumberFormatter
{
    private static readonly string[] Units = { "경", "조", "억", "만", "" };
    private static readonly long[] Dividers = { 10000000000000000, 1000000000000, 100000000, 10000, 1 };

    public static string Format(long number, int maxUnits = 2)
    {
        if (number < 10000)
            return number.ToString("N0");

        var result = new System.Text.StringBuilder();
        int unitCount = 0;

        for (int i = 0; i < Units.Length; i++)
        {
            long value = number / Dividers[i];
            number %= Dividers[i];

            if (value > 0)
            {
                string suffix = string.IsNullOrEmpty(Units[i]) ? "" : $"{Units[i]} ";
                result.Append($"{value.ToString("N0")}{suffix}");
                unitCount++;

                if (unitCount >= maxUnits)
                    break;
            }
        }

        return result.ToString().Trim();
    }
}
