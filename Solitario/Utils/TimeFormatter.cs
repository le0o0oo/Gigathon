namespace Solitario.Utils;

internal static class TimeFormatter {
  internal static string FormatTime(TimeSpan date) {
    string output = "";

    if (date.Days >= 365) {
      output += $"{(int)(date.TotalDays / 365)}y ";
      // numero di anni * 365 per ottenere il numero di giorni da sottrarre
      date = date.Subtract(TimeSpan.FromDays((int)(date.TotalDays / 365) * 365));
    }
    if (date.Days > 0) output += $"{(int)date.TotalDays}d ";
    if (date.Hours > 0 || date.Days > 0) output += $"{date.Hours}h ";
    if (date.Minutes > 0 || date.Hours > 0 || date.Days > 0) output += $"{date.Minutes}m ";

    output += $"{date.Seconds}s";

    return output.TrimEnd();
  }

  public static string GetFormattedTimestamp() {
    return DateTime.Now.ToString("yyyy_MM_dd HH:mm:ss");
  }
}
