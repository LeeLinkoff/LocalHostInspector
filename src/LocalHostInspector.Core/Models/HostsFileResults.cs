namespace LocalHostInspector.Core.Models;


public class HostsFileResult
{
    /// <summary>
    /// Matching entries found in the hosts file.
    /// </summary>
    public List<string> Matches { get; set; } = new();

    /// <summary>
    /// True if one or more matching entries were found.
    /// </summary>
    public bool HasMatches => Matches.Count > 0;

    /// <summary>
    /// Full path of the hosts file that was inspected.
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// Line number where the first matching entry was found.
    /// Null if no matching entry exists.
    /// </summary>
    public int? LineNumber { get; set; }
}