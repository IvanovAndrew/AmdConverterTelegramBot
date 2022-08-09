namespace AmdConverterTelegramBot;

public class Replies
{
    public Dialogue[] Dialogues { get; set; }

    public Replies(IConfiguration configuration)
    {
        Dialogues = configuration.GetSection("Replies").Get<Dialogue[]>();
    }
}