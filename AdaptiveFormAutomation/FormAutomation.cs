
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

public class FormAutomation
{
    private readonly IWebDriver driver;
    private readonly SmartElementFinder finder;

    public FormAutomation(IWebDriver driver)
    {
        this.driver = driver;
        finder = new SmartElementFinder(driver);
    }

    // Enter First Name
    public void FillFirstName(string firstName)
    {
        IWebElement nameField = finder.FindTextField("First Name");

        nameField.Clear();
        nameField.SendKeys(firstName);

        Console.WriteLine("First Name entered: " + firstName);
    }

    // Select Gender
    public void SelectGender(string gender)
    {
        IWebElement genderOption =
            finder.FindRadioOption("Gender", gender);

        genderOption.Click();

        if (!genderOption.Selected)
        {
            throw new Exception("Gender selection failed.");
        }

        Console.WriteLine("Gender selected: " + gender);
    }

    // Select State
    public void SelectState(string state)
    {
        IWebElement stateDropdown = finder.FindDropdown("State");

        SelectElement select = new SelectElement(stateDropdown);

        select.SelectByText(state);

        string selectedState = select.SelectedOption.Text.Trim();

        if (!selectedState.Equals(
            state,
            StringComparison.OrdinalIgnoreCase))
        {
            throw new Exception("State selection failed.");
        }

        Console.WriteLine("State selected: " + selectedState);
    }

    // Run all three form operations
    public void RunFormTest(
        string firstName,
        string gender,
        string state)
    {
        FillFirstName(firstName);

        SelectGender(gender);

        SelectState(state);

        Console.WriteLine("\nAll three fields completed successfully!");
    }
}
