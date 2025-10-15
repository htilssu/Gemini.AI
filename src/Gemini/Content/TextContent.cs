namespace Gemini.Content;

public class TextContent : ContentPart
{
    public string Text { get; set; }

    public TextContent(string text)
    {
        Text = text;
    }
}