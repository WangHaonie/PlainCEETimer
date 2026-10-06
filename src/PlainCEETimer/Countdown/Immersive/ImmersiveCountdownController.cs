using System;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Annotations.SourceGenerators;

namespace PlainCEETimer.Countdown.Immersive;

public partial class ImmersiveCountdownController
{
    [BackingField("_loop")]
    public partial bool Loop { get; set; }

    [BackingField("_skip")]
    public partial bool SkipExams { get; set; }

    [BackingField("_count")]
    public partial int Count { get; }

    public ColorPair DefaultColor { get; }

    private readonly Exam[] Exams;
    private readonly DefaultCountdownBuilder[] Builders;

    public ImmersiveCountdownController()
    {
        var a = CountdownStartInfo.FromConfig(App.Current.AppConfig);
        Exams = a.Exams;
        DefaultColor = a.DefaultColor;
        var count = Exams.Length;
        Builders = new DefaultCountdownBuilder[count];

        for (int i = 0; i < count; i++)
        {
            Builders[i] = new(Exams[i], a);
        }

        _count = count;
    }

    public string GetExamName(int index)
    {
        return index >= 0 && index < _count ? Exams[index].Name : string.Empty;
    }

    public bool TryBuild(int index, out string content, out ColorPair colors)
    {
        if (index >= 0 && index < _count)
        {
            return Builders[index].TryBuild(DateTime.Now, out content, out colors);
        }

        content = null;
        colors = default;
        return false;
    }

    public bool TryGetPN(int current, int offset, out int index)
    {
        index = -1;
        var n = _count;

        if (n <= 1 || offset == 0 || current < 0 || current >= n)
        {
            return false;
        }

        var i = current;
        var now = DateTime.Now;
        var steps = 0;

        while (steps < n - 1)
        {
            i += offset;

            if (i < 0 || i >= n)
            {
                if (!_loop)
                {
                    return false;
                }

                i = (i % n + n) % n;
            }

            steps++;

            if (!_skip || now <= Exams[i].End)
            {
                index = i;
                return true;
            }
        }

        return false;
    }
}
