namespace Solitario.Utils;

internal static class TimeFormatter {
  internal static string FormatTime(TimeSpan date) {
    string output = "";

    if (date.Days > 0) output += $"{(int)date.TotalDays}d ";
    if (date.Hours > 0 || date.Days > 0) output += $"{date.Hours}h ";
    if (date.Minutes > 0 || date.Hours > 0 || date.Days > 0) output += $"{date.Minutes}m ";

    output += $"{date.Seconds}s";

    return output.TrimEnd();
  }
}
