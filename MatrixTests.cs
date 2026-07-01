using NUnit.Framework;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using Allure.NUnit;
using Allure.Net.Commons;
using Allure.Net.Commons.Attributes;

namespace SauceDemoTests;

[AllureNUnit]
[TestFixture("chromium", TestName = "MatrixTests.Chromium")]
[TestFixture("firefox", TestName = "MatrixTests.Firefox")]
[TestFixture("webkit", TestName = "MatrixTests.Webkit")]
[Parallelizable(ParallelScope.All)]
public class MatrixTests
{
    private const string BaseUrl = "https://www.saucedemo.com/";
    private const string StandardUser = "standard_user";
    private const string SecretPassword = "secret_sauce";

    private readonly string browserType;
    private IPlaywright? playwright;
    private IBrowser? browser;
    private IPage? page;

    public MatrixTests(string browserType)
    {
        this.browserType = browserType;
    }

    [SetUp]
    public async Task SetUp()
    {
        playwright = await Playwright.CreateAsync();

        browser = browserType switch
        {
            "chromium" => await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false }),
            "firefox" => await playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false }),
            "webkit" => await playwright.Webkit.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false }),
            _ => throw new ArgumentOutOfRangeException(nameof(browserType), browserType, "Unsupported browser type")
        };

        page = await browser.NewPageAsync();
    }

    [TearDown]
    public async Task TearDown()
    {
        if (page != null)
        {
            await page.CloseAsync();
        }

        if (browser != null)
        {
            await browser.CloseAsync();
        }

        playwright?.Dispose();
    }

    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    [AllureTag("E2E")]
    [AllureTag("Checkout")]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureOwner("QA Team")]
    [AllureFeature("Checkout Flow")]
    public async Task SauceDemoCheckoutFlow(int instanceId)
    {
        await AllureApi.Step("Navigate to SauceDemo and login with standard_user", async () =>
        {
            await page!.GotoAsync(BaseUrl);
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            await page.Locator("#user-name").FillAsync(StandardUser);
            await page.Locator("#password").FillAsync(SecretPassword);
            await page.Locator("#login-button").ClickAsync();
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await Task.Delay(2000);
        });

        await AllureApi.Step("Add Sauce Labs Backpack to cart", async () =>
        {
            await page!.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await page.Locator("#add-to-cart-sauce-labs-backpack").ClickAsync();
            await Task.Delay(2000);
        });

        await AllureApi.Step("Navigate to checkout", async () =>
        {
            await page!.Locator("a.shopping_cart_link").ClickAsync();
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            await page.Locator("#checkout").ClickAsync();
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await Task.Delay(2000);
        });

        await AllureApi.Step("Enter checkout information (First Name, Last Name, Zip Code)", async () =>
        {
            await page!.Locator("#first-name").FillAsync("Test");
            await page.Locator("#last-name").FillAsync("User");
            await page.Locator("#postal-code").FillAsync("12345");

            await page.Locator("#continue").ClickAsync();
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await Task.Delay(2000);
        });

        await AllureApi.Step("Complete order and verify success message", async () =>
        {
            await page!.Locator("#finish").ClickAsync();
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var successMessage = page.Locator(".complete-header");
            await successMessage.WaitForAsync();
            var messageText = await successMessage.TextContentAsync();
            Assert.That(messageText, Does.Contain("Thank you for your order!"), "Success message should be visible");
            await Task.Delay(2000);
        });
    }
}
