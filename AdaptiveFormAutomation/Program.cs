
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

class Program
{
    static void Main(string[] args)
    {
        // URL can be passed through the command line
        string url = args.Length > 0
            ? args[0]
            : "https://app.cloudqa.io/home/AutomationPracticeForm";

        ChromeOptions options = new ChromeOptions();
        options.AddArgument("--start-maximized");

        IWebDriver driver = new ChromeDriver(options);

        try
        {
            driver.Navigate().GoToUrl(url);

            Console.WriteLine("Website opened successfully!");
            Console.WriteLine("URL: " + driver.Url);

            FormAutomation automation = new FormAutomation(driver);

            // Test data
            automation.RunFormTest(
                "Vasu",
                "Male",
                "India"
            );

            Console.WriteLine("\nTest completed successfully!");

            Console.WriteLine("\nPress ENTER to close browser.");
            Console.ReadLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine("\nTEST FAILED!");
            Console.WriteLine(ex.Message);

            Console.WriteLine("\nPress ENTER to close browser.");
            Console.ReadLine();
        }
        finally
        {
            driver.Quit();
        }
    }
}
