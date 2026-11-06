
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

class Program
{
    static void Main(string[] args)
    {
        string url = args.Length > 0
            ? args[0]
            : "https://app.cloudqa.io/home/AutomationPracticeForm";

        IWebDriver? driver = null;

        try
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--start-maximized");

            driver = new ChromeDriver(options);
            driver.Navigate().GoToUrl(url);

            Console.WriteLine("Website opened successfully!");
            Console.WriteLine("URL: " + driver.Url);

            FormAutomation automation = new FormAutomation(driver);

            automation.RunFormTest("Vasudev", "Male", "India");

            Console.WriteLine("\nTest completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("\nTEST FAILED!");
            Console.WriteLine(ex.Message);
        }
        finally
        {
            driver?.Quit();
        }

        Console.WriteLine("\nPress ENTER to exit.");
        Console.ReadLine();
    }
}
