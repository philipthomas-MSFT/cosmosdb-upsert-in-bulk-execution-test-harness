using Newtonsoft.Json;

public class Item
{
    [JsonProperty(PropertyName = "id")]
    public string Id { set; get; }
    public string? FirstName { set; get; }

    public string? LastName { get; set; }
}
