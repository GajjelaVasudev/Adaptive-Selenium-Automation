# Adaptive Selenium Form Automation

A C# console application that uses Selenium WebDriver to identify and interact with form fields using multiple HTML attributes and surrounding page context. It is designed to be less dependent on fixed element IDs, names, or page order.

The application automates three form operations:

- Enter a First Name.
- Select a Gender radio option.
- Select a State from a dropdown.

## Technologies

- C#
- .NET 10
- Selenium WebDriver 4.52.0
- Selenium Support 4.52.0
- Google Chrome

## Repository structure

```text
Adaptive-Selenium-Automation/
├── AdaptiveFormAutomation.slnx
├── README.md
├── .gitignore
└── AdaptiveFormAutomation/
    ├── AdaptiveFormAutomation.csproj
    ├── Program.cs
    ├── FormAutomation.cs
    ├── SmartElementFinder.cs
    └── modified-form.html
```

## How it works

### Program.cs

- Reads the website URL from the command line, or uses the CloudQA form URL by default.
- Starts Chrome and opens the selected URL.
- Runs the form automation workflow.
- Reports errors and closes the browser.

### SmartElementFinder.cs

Collects candidate elements and ranks them using matching clues such as:

- `name`, `id`, `placeholder`, `aria-label`, and `title` attributes.
- Associated label text and nearby label text.
- The surrounding label or legend for radio button groups.
- The value and displayed label of an individual radio option.

Text is normalized before comparison by converting it to lowercase and removing spaces and punctuation. The finder chooses the highest-scoring candidate. It reports an error when no candidate matches or when the top two candidates have equal scores.

### FormAutomation.cs

Uses the finder to fill the First Name field, select the requested Gender option, and select the requested State. It verifies the radio button and dropdown selections.

## Requirements

Before running the project, install:

- .NET 10 SDK
- Google Chrome
- Visual Studio 2022 or later with .NET development support, or the .NET CLI

Selenium packages are restored through the project file. Selenium Manager is used by Selenium to manage the browser driver.

## Run the project

Clone the repository:

```bash
git clone https://github.com/GajjelaVasudev/Adaptive-Selenium-Automation.git
```

Open a terminal in the project directory:

```bash
cd Adaptive-Selenium-Automation/AdaptiveFormAutomation
```

Restore dependencies and run:

```bash
dotnet restore
dotnet run
```

By default, the application opens:

```text
https://app.cloudqa.io/home/AutomationPracticeForm
```

### Use a different URL

Pass the URL as the first command-line argument:

```bash
dotnet run -- "https://example.com/form"
```

The current test values are set in `Program.cs`:

| Field | Value |
|---|---|
| First Name | Vasudev |
| Gender | Male |
| State | India |

Change these values in `Program.cs` to use different test data.

## Local test page

The repository includes `modified-form.html`, a local HTML page used to test the finder when form fields are reordered, element IDs and names are changed, and similar-looking decoy fields are present.

To run against this page, pass its file URL. For example, from the project directory on Windows:

```powershell
dotnet run -- "file:///G:/CloudQA/Task1/AdaptiveFormAutomation/AdaptiveFormAutomation/modified-form.html"
```

Use the correct file path for your own computer. The local test page has different dropdown options, so change the State test value in `Program.cs` to an option that exists on that page (for example, `Andhra Pradesh`).

## Test scenarios performed

| Scenario | Expected behaviour | Observed result |
|---|---|---|
| Original CloudQA form | Fill First Name, select Gender and State | Passed |
| Changed IDs and names | Find fields using other available clues | Passed |
| Reordered fields | Find fields without relying on their position | Passed |
| Similar-looking decoy fields | Prefer the field whose label and context match the requested field | Passed after improving label matching |
| First Name field missing | Report that the requested field cannot be identified rather than filling the decoy field | Passed |

The missing-field scenario is expected to report a failure: that is the correct behaviour because the requested field is absent.

## Approach and limitations

This project uses rule-based, score-based matching. It does not use machine learning or an AI model.

The approach can tolerate some changes to element IDs, names, and field order when other useful attributes or labels remain available. It is not guaranteed to identify fields on every website.

- Matching depends on useful HTML attributes, labels, or nearby context being present.
- Nearby-label and radio-group detection use DOM relationships, so substantially different page structures may need additional handling.
- If identifying information is missing, misleading, or shared by multiple elements, the finder may not be able to determine the intended field safely.
- The current ambiguity check rejects equal top scores; it does not guarantee that every close-scoring candidate is unambiguous.
- Dynamic forms may need explicit waits and additional handling.
- The implementation currently targets the requested text field, radio group, and dropdown.

## Possible future improvements

- Add a configurable way to provide test data.
- Add explicit waits for dynamically loaded fields.
- Improve ambiguity detection using a confidence threshold and score margin.
- Add automated tests and structured execution reports.
- Support more form control types.

## Author

**Vasudev Gajjela**

GitHub: https://github.com/GajjelaVasudev