namespace PlainCEETimer.Countdown;

public class ExamSettings
{
    public bool Enabled { get; set; }

    public int Mode { get; set; }

    public CountdownFormat Format { get; set; }

    public CountdownRule[] Rules { get; set; }

    public CountdownRule[] GlobalRules { get; set; }
}