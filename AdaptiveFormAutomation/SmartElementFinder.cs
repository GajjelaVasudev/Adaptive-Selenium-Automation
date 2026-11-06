
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;

public class SmartElementFinder
{
    private readonly IWebDriver driver;
    private readonly IJavaScriptExecutor js;

    public SmartElementFinder(IWebDriver driver)
    {
        this.driver = driver;
        js = (IJavaScriptExecutor)driver;
    }

    // Find a text field using multiple clues
    public IWebElement FindTextField(string fieldName)
    {
        var elements = driver.FindElements(
            By.CssSelector("input[type='text'], input:not([type]), textarea")
        );

        return FindBestMatch(elements, fieldName);
    }

    // Find a dropdown using multiple clues
    public IWebElement FindDropdown(string fieldName)
    {
        var elements = driver.FindElements(By.TagName("select"));

        return FindBestMatch(elements, fieldName);
    }

    // Find a radio option inside the requested group
    public IWebElement FindRadioOption(string groupName, string option)
    {
        var radios = driver.FindElements(
            By.CssSelector("input[type='radio']")
        );

        var scores = new List<(IWebElement Element, int Score)>();

        foreach (var radio in radios)
        {
            string groupContext = GetRadioGroupContext(radio);

            // Do not select a radio if its group cannot be identified.
            int groupScore = ScoreText(groupContext, groupName, 12);

            if (groupScore == 0)
                continue;

            int score = groupScore;

            string value = radio.GetAttribute("value") ?? "";
            score += ScoreText(value, option, 10);

            string label = GetLabels(radio);
            score += ScoreText(label, option, 10);

            string optionText = GetRadioOptionText(radio);
            score += ScoreText(optionText, option, 10);

            if (score >= 12)
            {
                scores.Add((radio, score));
            }
        }

        return GetBestElement(scores, groupName + " - " + option);
    }

    // Find the best matching text field or dropdown
    private IWebElement FindBestMatch(
        IReadOnlyCollection<IWebElement> elements,
        string fieldName)
    {
        var scores = new List<(IWebElement Element, int Score)>();

        foreach (var element in elements)
        {
            string labels = GetLabels(element);

            // Reject elements whose actual associated label
            // contradicts the requested field.
            if (!string.IsNullOrWhiteSpace(labels) &&
                ScoreText(labels, fieldName, 14) == 0)
            {
                continue;
            }

            int score = 0;

            score += ScoreText(
                element.GetAttribute("name"), fieldName, 10);

            score += ScoreText(
                element.GetAttribute("id"), fieldName, 8);

            score += ScoreText(
                element.GetAttribute("placeholder"), fieldName, 8);

            score += ScoreText(
                element.GetAttribute("aria-label"), fieldName, 12);

            score += ScoreText(
                element.GetAttribute("title"), fieldName, 5);

            score += ScoreText(labels, fieldName, 14);

            string nearbyLabel = GetNearbyLabel(element);
            score += ScoreText(nearbyLabel, fieldName, 8);

            // Avoid selecting elements based on very weak clues.
            if (score >= 8)
            {
                scores.Add((element, score));
            }
        }

        return GetBestElement(scores, fieldName);
    }

    // Get labels directly associated with an element
    private string GetLabels(IWebElement element)
    {
        return js.ExecuteScript(
            "return Array.from(arguments[0].labels || [])" +
            ".map(label => label.innerText).join(' ');",
            element
        )?.ToString() ?? "";
    }

    // Look for labels near an element
    private string GetNearbyLabel(IWebElement element)
    {
        return js.ExecuteScript(
            "let el = arguments[0];" +
            "let result = [];" +
            "for (let i = 0; i < 4 && el; i++) {" +
            "let prev = el.previousElementSibling;" +
            "while (prev) {" +
            "if (prev.matches('label')) result.push(prev.innerText);" +
            "prev = prev.previousElementSibling;" +
            "}" +
            "el = el.parentElement;" +
            "}" +
            "return result.join(' ');",
            element
        )?.ToString() ?? "";
    }

    // Identify the surrounding radio group label
    private string GetRadioGroupContext(IWebElement radio)
    {
        return js.ExecuteScript(
            "let el = arguments[0];" +
            "let result = [];" +
            "for (let i = 0; i < 5 && el; i++) {" +
            "if (el.matches('fieldset')) {" +
            "let legend = el.querySelector('legend');" +
            "if (legend) result.push(legend.innerText);" +
            "}" +
            "let prev = el.previousElementSibling;" +
            "while (prev) {" +
            "if (prev.matches('label, legend')) " +
            "result.push(prev.innerText);" +
            "prev = prev.previousElementSibling;" +
            "}" +
            "el = el.parentElement;" +
            "}" +
            "return result.join(' ');",
            radio
        )?.ToString() ?? "";
    }

    // Get the text associated with an individual radio option
    private string GetRadioOptionText(IWebElement radio)
    {
        return js.ExecuteScript(
            "let id = arguments[0].id;" +
            "let result = [];" +
            "if (id) {" +
            "let labels = document.querySelectorAll('label[for], span[for]');" +
            "labels.forEach(label => {" +
            "if (label.getAttribute('for') === id) " +
            "result.push(label.innerText);" +
            "});" +
            "}" +
            "return result.join(' ');",
            radio
        )?.ToString() ?? "";
    }

    // Compare text after normalizing it
    private int ScoreText(
        string? actual,
        string expected,
        int exactScore)
    {
        if (string.IsNullOrWhiteSpace(actual))
            return 0;

        string a = Normalize(actual);
        string e = Normalize(expected);

        if (a == e)
            return exactScore;

        if (a.Length >= 3 && e.Length >= 3 &&
            (a.Contains(e) || e.Contains(a)))
        {
            return exactScore / 2;
        }

        return 0;
    }

    // Remove spaces and special characters before comparison
    private string Normalize(string value)
    {
        return new string(
            value.ToLowerInvariant()
                 .Where(char.IsLetterOrDigit)
                 .ToArray()
        );
    }

    // Return the highest-scoring valid element
    private IWebElement GetBestElement(
        List<(IWebElement Element, int Score)> scores,
        string fieldName)
    {
        if (scores.Count == 0)
        {
            throw new NoSuchElementException(
                "Could not identify field: " + fieldName
            );
        }

        var ranked = scores
            .OrderByDescending(x => x.Score)
            .ToList();

        // Do not guess when two elements have the same score.
        if (ranked.Count > 1 &&
            ranked[0].Score == ranked[1].Score)
        {
            throw new Exception(
                "Ambiguous match for: " + fieldName
            );
        }

        IWebElement bestElement = ranked[0].Element;

        if (!bestElement.Displayed || !bestElement.Enabled)
        {
            throw new Exception(
                "The identified field is not available: " + fieldName
            );
        }

        return bestElement;
    }
}
