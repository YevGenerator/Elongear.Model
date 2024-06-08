using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elongear.Server.MailService;

public class HtmlParser
{
    private StringBuilder StringBuilder { get; set; } = new();
    public string Html => StringBuilder.ToString();

    public async Task ReadHtml(string htmlPath)
    {
        StringBuilder = new(await File.ReadAllTextAsync(htmlPath));
    }
    public async Task SetStylesToHtml(string cssPath)
    {
        var css = await File.ReadAllTextAsync(cssPath);
        var indexToInsertCss = GetClosingHeadTagIndex() - 1;
        string openScriptTag = "<style>";
        string closeScriptTag = "</style>";
        StringBuilder.Insert(indexToInsertCss, openScriptTag);
        StringBuilder.Insert(indexToInsertCss + openScriptTag.Length, css);
        StringBuilder.Insert(indexToInsertCss + openScriptTag.Length+css.Length, closeScriptTag);
    }

    public int GetClosingHeadTagIndex() => Html.IndexOf("</head>");
    public int GetHrefInputIndex()
    {
        string aTag = "<a href=\"";
        return Html.IndexOf(aTag) + aTag.Length;
    }
    public int GetDigitsStartIndex()
    {
        string paragraph = "<p class=\"confirmNumber\">";
        return Html.IndexOf(paragraph) + paragraph.Length;
    }
    public void SetConfirmationLink(string link)
    {
        int hrefIndex = GetHrefInputIndex();
        StringBuilder.Insert(hrefIndex, link);
    }
    public void SetDigits(byte[] digits)
    {
        var startIndex = GetDigitsStartIndex();
        StringBuilder digitBuilder = new();
        foreach(var digit in digits)
        {
            digitBuilder.Append("<span>");
            digitBuilder.Append(digit);
            digitBuilder.Append("</span>");
        }
        StringBuilder.Insert(startIndex, digitBuilder.ToString());
    }


}
