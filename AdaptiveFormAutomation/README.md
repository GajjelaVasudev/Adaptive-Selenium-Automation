# Adaptive Selenium Form Automation

A C# and Selenium-based automation project that identifies and interacts with form elements using multiple HTML attributes, associated labels, and surrounding context instead of relying entirely on fixed selectors.

The project automates three form operations:
- Entering a First Name.
- Selecting a Gender radio button.
- Selecting a State from a dropdown.

The element finder uses a scoring-based matching approach to identify the most suitable form element and rejects missing or ambiguous matches.

## Features

- **Adaptive element detection:** Uses multiple HTML attributes and contextual information to identify elements.
- **Text field automation:** Finds and fills the First Name field.
- **Radio button selection:** Identifies the requested option within the Gender group.
- **Dropdown selection:** Finds the State dropdown and selects the requested option.
- **Ambiguity handling:** Rejects candidates when the highest-scoring elements have equal scores.
- **Missing element handling:** Reports an error when a suitable field cannot be identified.
- **Configurable URL:** Accepts a website URL through command-line arguments.
- **Selection verification:** Checks whether radio button and dropdown selections succeeded.

## Technologies Used

| Technology | Purpose |
|---|---|
| C# | Programming language |
| .NET 10 | Application framework |
| Selenium WebDriver 4.52.0 | Browser automation |
| Selenium Support 4.52.0 | Dropdown interaction |
| Google Chrome | Browser used for testing |

## Project Structure

```text
AdaptiveFormAutomation/
│
├── Program.cs
├── FormAutomation.cs
├── SmartElementFinder.cs
├── TestPages/
│   └── modified-form.html
├── AdaptiveFormAutomation.csproj
├── README.md
└── .gitignore
```

### Main Components

**1. Program.cs**

- Starts the Chrome browser.
- Opens the configured website.
- Creates the form automation object.
- Executes the form automation workflow.
- Handles exceptions and closes the browser.

**2. SmartElementFinder.cs**

Responsible for identifying form elements using a scoring-based matching system.

It examines:
- HTML `name` attribute.
- HTML `id` attribute.
- `placeholder` attribute.
- `aria-label` attribute.
- `title` attribute.
- Associated labels.
- Nearby label text.
- Radio group context.

**3. FormAutomation.cs**

Contains the operations required to:
- Fill the First Name field.
- Select a Gender option.
- Select a State.
- Verify the selections.

**4. TestPages/modified-form.html**

A local HTML test page used to evaluate element detection under modified page conditions.

## Setup and Installation

### Prerequisites

Install the following:

- .NET 10 SDK
- Visual Studio 2022 or later with .NET development support
- Google Chrome

### Clone the repository

```bash
git clone https://github.com/GajjelaVasudev/Adaptive-Selenium-Automation.git
```

Navigate to the project directory:

```bash
cd Adaptive-Selenium-Automation/AdaptiveFormAutomation
```

### Restore dependencies

```bash
dotnet restore
```

## Running the Automation

### Run against the default website

```bash
dotnet run
```

The default URL is:

```text
https://app.cloudqa.io/home/AutomationPracticeForm
```

### Run against a different website

A URL can be supplied as a command-line argument:

```bash
dotnet run -- "https://example.com/form"
```

The application uses the supplied URL instead of the default URL.

### Test data

The current test execution uses:

| Field | Test value |
|---|---|
| First Name | Vasudev |
| Gender | Male |
| State | India |

These values can be changed in `Program.cs`.

## Adaptive Element Matching Approach

The application does not depend exclusively on fixed IDs or CSS selectors to identify the requested fields.

Instead, it uses the following process:

1. **Candidate collection:** Finds elements based on their HTML type, such as text inputs, radio buttons, and dropdowns.
2. **Attribute inspection:** Examines attributes including `id`, `name`, `placeholder`, `aria-label`, and `title`.
3. **Context inspection:** Checks associated labels and nearby form labels. For radio buttons, it also examines the surrounding group context.
4. **Text normalization:** Converts text to lowercase and removes spaces and special characters before comparison.
5. **Scoring:** Assigns scores to matching clues and ranks candidate elements.
6. **Validation:** Rejects missing matches and equal top-scoring candidates instead of selecting an arbitrary element.

The element with the highest matching score is selected only when it is a unique match and is visible and enabled.

## Test Scenarios

The implementation was tested against the original CloudQA form and a modified local HTML page.

| Test scenario | Expected behaviour | Result |
|---|---|---|
| Original CloudQA form | Fill First Name, select Gender and State | Passed |
| Changed element IDs and names | Identify fields using remaining clues | Passed |
| Reordered form fields | Identify fields independently of their position | Passed |
| Similar-looking decoy fields | Prefer the correct field using label and context information | Passed after matching improvement |
| Missing First Name field | Report that the requested field cannot be identified | Passed |

The missing-field scenario intentionally produces a test failure because the application correctly refuses to fill an unrelated field.

## Limitations

- Element identification depends on meaningful HTML attributes, labels, or surrounding context.
- If all identifying information is removed or misleading, the application cannot guarantee correct identification.
- Nearby-label detection relies on the page's DOM structure and may need adjustment for substantially different layouts.
- The scoring approach is heuristic-based and does not use machine learning or AI.
- Dynamic pages may require additional explicit waits.
- The current implementation is designed for the specified form fields and is not a universal automation solution for every type of website.
- Changes to the website's DOM structure may require improvements to the element detection logic.

## Future Improvements

- Add configurable test data.
- Introduce explicit waits for dynamically loaded elements.
- Add automated unit and integration tests.
- Improve matching confidence thresholds.
- Support additional form element types.
- Generate structured test execution reports.

## Author

**Vasudev Gajjela**

GitHub: https://github.com/GajjelaVasudev