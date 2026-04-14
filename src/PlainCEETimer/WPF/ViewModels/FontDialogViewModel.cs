using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.Fody;
using PlainCEETimer.UI;
using PlainCEETimer.WPF.Extensions;
using PlainCEETimer.WPF.Models;
using PlainCEETimer.WPF.Modules;

namespace PlainCEETimer.WPF.ViewModels;

[NoConstants]
public sealed partial class FontDialogViewModel : ObservableObject, IConfirmClose
{
    [ObservableProperty]
    public partial string FontFamilyText { get; set; }

    [ObservableProperty]
    public partial string FontSizeText { get; set; }

    [ObservableProperty]
    public partial FontWeightItem FontWeight { get; set; }

    [ObservableProperty]
    public partial FontFamilyWrapper PreviewFontFamily { get; set; }

    [ObservableProperty]
    public partial double PreviewFontSize { get; set; }

    [ObservableProperty]
    public partial bool IsFontValid { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveChangesCommand))]
    public partial bool UserChanged { get; set; }

    public string PreviewText
    {
        get => previewText;
        set
        {
            var newValue = string.IsNullOrWhiteSpace(value) ? DefaultPreviewText : value;
            SetProperty(ref previewText, newValue);
        }
    }

    public ObservableCollection<FontWeightItem> FontWeightCollection => field ??=
    [
        /*
         
        字体粗细由小到大 参考：

        FontWeights Class (System.Windows) | Microsoft Learn
        https://learn.microsoft.com/en-us/dotnet/api/system.windows.fontweights

         */

        new("极细", FontWeights.Thin),
        new("特细", FontWeights.ExtraLight),
        new("细体", FontWeights.Light),
        new("常规", FontWeights.Regular),
        new("中等", FontWeights.Medium),
        new("半粗", FontWeights.SemiBold),
        new("粗体", FontWeights.Bold),
        new("特粗", FontWeights.ExtraBold),
        new("黑体", FontWeights.Black),
        new("特黑", FontWeights.ExtraBlack)
    ];

    public ObservableCollection<FontSizeItem> FontSizeCollection => field ??=
        new(FontSizeItem.Yield(ConfigValidator.MinFontSize, ConfigValidator.MaxFontSize, 2.0));

    public event Action<FontModel> ParseResult;

    private string previewText;
    private readonly bool init = true;
    private readonly string initFont;
    private readonly double initSize;
    private readonly FontWeight initWeight;
    private readonly IDialogService MessageX;

    private const string DefaultPreviewText =
        """
        天地玄黄，宇宙洪荒。
        Pack my box with five dozen liquor jugs.
        0123456789`~!@#$%^&*()[]{}<>\|/,.;:'"+-*/=
        """;

    public FontDialogViewModel(FontModel font = null, IDialogService service = null)
    {
        previewText = DefaultPreviewText;

        font ??= App.AppConfig.GetFont().Font1;
        initFont = font.FontFamily.Name;
        initSize = font.SizePt;
        initWeight = font.Weight;
        MessageX = service;

        FontFamilyText = initFont;
        FontSizeText = initSize.Format();
        FontWeight = FontWeightCollection.FirstOrDefault(x => x.FontWeight == initWeight) ?? FontWeightCollection[3];

        UpdateView();
        init = false;
    }

    public bool CanClose()
    {
        if (UserChanged)
        {
            return ShowUnsavedWarning("是否保存当前更改？", SaveChangesCore);
        }

        return true;
    }

    partial void OnFontFamilyTextChanged(string value)
    {
        InvokeUserChanged();
        UpdateView();
    }

    partial void OnFontWeightChanged(FontWeightItem value)
    {
        InvokeUserChanged();
    }

    partial void OnFontSizeTextChanged(string value)
    {
        InvokeUserChanged();
        UpdateView();
    }

    private void UpdateView()
    {
        try
        {
            var ff = FontFamilyText.Truncate(ConfigValidator.MaxFontFamilyLength, false);
            var font = new FontFamilyWrapper(ff);
            IsFontValid = font.Value.BaseUri == null && !string.IsNullOrWhiteSpace(ff);
            PreviewFontFamily = font;
        }
        catch
        {
            IsFontValid = false;
        }

        var size = ParseSizeTextPt(FontSizeText);

        if (size > 0)
        {
            PreviewFontSize = size.Pt2Dip();
        }
    }

    private double ParseSizeTextPt(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            ResetFontSizeText();
        }
        else
        {
            var item = FontSizeCollection.FirstOrDefault(x => x.Display == text);

            if (item != default)
            {
                return item.SizePt;
            }

            if (double.TryParse(text, out var result))
            {
                var clamped = result.Clamp(ConfigValidator.MinFontSize, ConfigValidator.MaxFontSize);

                if (clamped != result)
                {
                    ResetFontSizeText(clamped.Format());
                }

                return clamped;
            }
            else
            {
                ResetFontSizeText();
            }
        }

        return default;
    }

    private void ResetFontSizeText(string text = null)
    {
        FontSizeText = text ?? FontSizeCollection[0].Display;
    }

    [RelayCommand(CanExecute = nameof(UserChanged))]
    private void SaveChanges()
    {
        SaveChangesCore();
    }

    private bool SaveChangesCore()
    {
        if (!IsFontValid)
        {
            MessageX.Error("输入的字体无效！");
            return false;
        }

        UserChanged = false;

        OnParseResult(new FontModel()
        {
            FontFamily = PreviewFontFamily,
            Size = PreviewFontSize.Clamp(ConfigValidator.MinFontSize, ConfigValidator.MaxFontSize),
            Weight = FontWeight.FontWeight
        });

        return true;
    }

    [RelayCommand]
    private void Close()
    {
        if (CanClose())
        {
            OnParseResult(null);
        }
    }

    private void OnParseResult(FontModel result)
    {
        ParseResult?.Invoke(result);
    }

    private void InvokeUserChanged()
    {
        if (!init)
        {
            UserChanged = true;
        }
    }

    private bool ShowUnsavedWarning(string msg, Func<bool> SaveChanges)
    {
        switch (MessageX.Warn(msg, MessageButtons.YesNo))
        {
            case true:
                return SaveChanges();
            case false:
                UserChanged = false;
                return true;
            default:
                return false;
        }
    }
}