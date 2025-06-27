namespace Solitario.Utils;

internal static class TimeFormatter {
  internal static string FormatTime(TimeSpan date) {
    var parts = new List<string>();
    if (date.Days >= 365) parts.Add($"{(int)(date.Days / 365)}y");
    if (date.Days % 365 > 0) parts.Add($"{date.Days % 365}d");
    if (date.Hours > 0) parts.Add($"{date.Hours}h");
    if (date.Minutes > 0) parts.Add($"{date.Minutes}m");
    parts.Add($"{date.Seconds}s");
    return string.Join(" ", parts);
  }

  internal static string GetFormattedTimestamp() {
    return DateTime.Now.ToString("yyyy_MM_dd HH:mm:ss");
  }
}
