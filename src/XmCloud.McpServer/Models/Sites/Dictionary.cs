namespace XmCloud.McpServer.Models.Sites;

public class Dictionary
{
    public int Total { get; set; }
    public List<DictionaryItem>? Results { get; set; }
    public PageInfo? PageInfo { get; set; }
}

