namespace AmdConverterTelegramBot;

public class Dialogue
{
    public string[] Message { get; set; }
    public Reply Reply { get; set; }
}

public class Reply
{
    public string? Text { get; set; }
    public string? Sticker { get; set; }
}