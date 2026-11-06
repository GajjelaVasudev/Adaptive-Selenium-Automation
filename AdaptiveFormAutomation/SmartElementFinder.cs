
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

    // Find a text box using its attributes and associated label
    public IWebElement FindTextField(string fieldName)
    {
        var elements = driver.FindElements(
            By.CssSelector("input[type='text'], input:not([type]), textarea")
        );

        return FindBestMatch(elements, fieldName);
    }

    // Find a dropdown using its attributes and associated label
    public IWebElement FindDropdown(string fieldName)
    {
        var elements = driver.FindElements(By.TagName("select"));

        return FindBestMatch(elements, fieldName);
    }

    // Find a radio button using its group name and option
    public IWebElement FindRadioOption(string groupName, string option)
    {
        var radios = driver.FindElements(
            By.CssSelector("input[type='radio']")
        );

        var scores = new List<(IWebElement Element, int Score)>();

        foreach (var radio in radios)
        {
            int score = 0;

            string value = radio.GetAttribute("value") ?? "";

            string groupContext = GetRadioGroupContext(radio);

            // Check whether this radio belongs to the requested group
            score += ScoreText(groupContext, groupName, 12);

            // Check whether the radio value matches the requested option
            score += ScoreText(value, option, 10);

            // Check whether its associated label matches the option
            string label = GetLabels(radio);

            score += ScoreText(label, option, 10);

            // Check the text of a nearby option label
            string optionText = GetRadioOptionText(radio);

            score += ScoreText(optionText, option, 10);

            if (score > 0)
            {
                scores.Add((radio, score));
            }
        }

        return GetBestElement(scores, groupName + " - " + option);
    }

    // Find the best matching text box or dropdown
    private IWebElement FindBestMatch(
        IReadOnlyCollection<IWebElement> elements,
        string fieldName)
    {
        var scores = new List<(IWebElement Element, int Score)>();

        foreach (var element in elements)
        {
            int score = 0;

            // Check HTML attributes
            score += ScoreText(element.GetAttribute("name"), fieldName, 10);

            score += ScoreText(element.GetAttribute("id"), fieldName, 8);

            score += ScoreText(
                element.GetAttribute("placeholder"), fieldName, 8);

            score += ScoreText(
                element.GetAttribute("aria-label"), fieldName, 12);

            score += ScoreText(
                element.GetAttribute("title"), fieldName, 5);

            // Check associated labels
            string labels = GetLabels(element);

            score += ScoreText(labels, fieldName, 14);

            // Check nearby form label text
            string nearbyLabel = GetNearbyLabel(element);

            score += ScoreText(nearbyLabel, fieldName, 8);

            if (score > 0)
            {
                scores.Add((element, score));
            }
        }

        return GetBestElement(scores, fieldName);
    }

    // Get labels connected to an element
    private string GetLabels(IWebElement element)
    {
        return js.ExecuteScript(
            "return Array.from(arguments[0].labels || [])" +
            ".map(label => label.innerText).join(' ');",
            element
        )?.ToString() ?? "";
    }

    // Find nearby labels without depending on a fixed parent structure
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

    // Find the surrounding context of a radio button group
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

    // Calculate the matching score
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

    // Normalize text before comparing it
    private string Normalize(string value)
    {
        return new string(
            value.ToLowerInvariant()
                 .Where(char.IsLetterOrDigit)
                 .ToArray()
        );
    }

    // Select the highest-scoring element
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
