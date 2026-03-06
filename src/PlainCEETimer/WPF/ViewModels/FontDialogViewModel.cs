using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI;
using PlainCEETimer.WPF.Models;
using PlainCEETimer.WPF.Modules;

namespace PlainCEETimer.WPF.ViewModels;

public sealed partial class FontDialogViewModel : ObservableObject, IConfirmClose
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(UserChanged))]
    public partial string FontFamily { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(UserChanged))]
    public partial string FontSizeText { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(UserChanged))]
    public partial FontWeightModel FontWeight { get; set; }

    [ObservableProperty]
    public partial FontFamily PreviewFontFamily { get; set; }

    [ObservableProperty]
    public partial double PreviewFontSize { get; set; }

    [ObservableProperty]
    public partial bool IsFontValid { get; set; }

    public string PreviewText
    {
        get => previewText;
        set
        {
            var newValue = string.IsNullOrWhiteSpace(value) ? DefaultPreviewText : value;
            SetProperty(ref previewText, newValue);
        }
    }

    public bool UserChanged => FontFamily != initFont
        || ParseSizeTextPt(FontSizeText) != initSize
        || FontWeight?.FontWeight != initWeight;

    public ObservableCollection<FontWeightModel> FontWeightCollection => field ??=
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

    public ObservableCollection<FontSizeModel> FontSizeCollection => field ??=
        new(FontSizeModel.Yield(ConfigValidator.MinFontSize, ConfigValidator.MaxFontSize, 2.0));

    public event Action<FontModel> ParseResult;

    private string previewText;
    private readonly string initFont;
    private readonly double initSize;
    private readonly FontWeight initWeight;
    private readonly IDialogService MessageX;

    private readonly string DefaultPreviewText =
        """
        天地玄黄，宇宙洪荒。
        Pack my box with five dozen liquor jugs.
        0123456789`~!@#$%^&*()[]{}<>\|/,.;:'"+-*/=
        """;

    public FontDialogViewModel(FontModel font = null, IDialogService service = null)
    {
        previewText = DefaultPreviewText;

        font ??= App.AppConfig.GetFont();
        initFont = font.FontFamily.Source;
        initSize = font.SizePt;
        initWeight = font.Weight;
        MessageX = service;

        FontFamily = initFont;
        FontSizeText = initSize.ToString();
        FontWeight = FontWeightCollection.FirstOrDefault(x => x.FontWeight == initWeight) ?? FontWeightCollection[3];

        UpdateView();
    }

    public bool CanClose()
    {
        if (UserChanged
            && MessageX.Warn("是否保存当前更改？", MessageButtons.YesNo).AsBool())
        {
            return SaveChangesCore();
        }

        return false;
    }

    partial void OnFontFamilyChanged(string value)
    {
        UpdateView();
        SaveChangesCommand.NotifyCanExecuteChanged();
    }

    partial void OnFontWeightChanged(FontWeightModel value)
    {
        SaveChangesCommand.NotifyCanExecuteChanged();
    }

    partial void OnFontSizeTextChanged(string value)
    {
        UpdateView();
        SaveChangesCommand.NotifyCanExecuteChanged();
    }

    private void UpdateView()
    {
        try
        {
            var ff = FontFamily;
            var font = new FontFamily(ff);
            PreviewFontFamily = font;
            IsFontValid = font.BaseUri == null && !string.IsNullOrWhiteSpace(ff);
        }
        catch
        {
            IsFontValid = false;
        }

        var size = ParseSizeTextPt(FontSizeText);

        if (size > 0)
        {
            PreviewFontSize = size * (96.0 / 72.0);
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
                    ResetFontSizeText(clamped.ToString());
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
            MessageX.Error("字体名称无效！");
            return false;
        }

        OnParseResult(new FontModel()
        {
            FontFamily = PreviewFontFamily,
            Size = PreviewFontSize,
            Weight = FontWeight.FontWeight,
        });

        return true;
    }

    [RelayCommand]
    private void Close()
    {
        OnParseResult(null);
    }

    private void OnParseResult(FontModel result)
    {
        ParseResult?.Invoke(result);
    }
}