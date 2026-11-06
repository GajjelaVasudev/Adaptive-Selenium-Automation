
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

class Program
{
    static void Main()
    {
        // Website URL
        string url = "https://app.cloudqa.io/home/AutomationPracticeForm";

        // Configure Chrome
        ChromeOptions options = new ChromeOptions();
        options.AddArgument("--start-maximized");

        // Launch browser
        IWebDriver driver = new ChromeDriver(options);

        try
        {
            // Open the website
            driver.Navigate().GoToUrl(url);

            // Display page information
            Console.WriteLine("Browser opened successfully!");
            Console.WriteLine("Page Title: " + driver.Title);
            Console.WriteLine("Current URL: " + driver.Url);

            Console.WriteLine("\nPress ENTER to close the browser.");
            Console.ReadLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        finally
        {
            // Close browser
            driver.Quit();
        }
    }
}
