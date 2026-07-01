using NUnit.Framework;
using Microsoft.Playwright;
using Allure.NUnit;
using Allure.Net.Commons;
using Allure.Net.Commons.Attributes;

[assembly: LevelOfParallelism(3)]

namespace SauceDemoTests;

[AllureNUnit]
[TestFixture]
[Parallelizable(ParallelScope.All)]
public class SauceDemoCheckoutTests
{
    private const string BaseUrl = "https://www.saucedemo.com/";
    private const string StandardUser = "standard_user";
    private const string SecretPassword = "secret_sauce";

    [Test]
    [AllureTag("E2E")]
    [AllureTag("Checkout")]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureOwner("QA Team")]
    [AllureFeature("Checkout Flow")]
    public async Task TestSauceDemoSimpleCheckout()
    {
        IPlaywright? playwright = null;
        IBrowser? browser = null;
        IPage? page = null;

        try
        {
            playwright = await Playwright.CreateAsync();
            browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false
            });
            page = await browser.NewPageAsync();

            // Step 1: Navigate & Login
            await AllureApi.Step("Navigate to SauceDemo and login with standard_user", async () =>
            {
                await page.GotoAsync(BaseUrl);
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                
                await page.Locator("#user-name").FillAsync(StandardUser);
                await page.Locator("#password").FillAsync("secret_sauce");
                await page.Locator("#login-button").ClickAsync();
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                await Task.Delay(2000);
            });

            // Step 2: Add to Cart
            await AllureApi.Step("Add Sauce Labs Backpack to cart", async () =>
            {
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                await page.Locator("#add-to-cart-sauce-labs-backpack").ClickAsync();
                await Task.Delay(2000);
            });

            // Step 3: Checkout
            await AllureApi.Step("Navigate to checkout", async () =>
            {
                await page.Locator("a.shopping_cart_link").ClickAsync();
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                
                await page.Locator("#checkout").ClickAsync();
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                await Task.Delay(2000);
            });

            // Step 4: Enter Details
            await AllureApi.Step("Enter checkout information (First Name, Last Name, Zip Code)", async () =>
            {
                await page.Locator("#first-name").FillAsync("Test");
                await page.Locator("#last-name").FillAsync("User");
                await page.Locator("#postal-code").FillAsync("12345");
                
                await page.Locator("#continue").ClickAsync();
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                await Task.Delay(2000);
            });

            // Step 5: Finish and Verify
            await AllureApi.Step("Complete order and verify success message", async () =>
            {
                await page.Locator("#finish").ClickAsync();
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                
                var successMessage = page.Locator(".complete-header");
                await successMessage.WaitForAsync();
                var messageText = await successMessage.TextContentAsync();
                Assert.That(messageText, Does.Contain("Thank you for your order!"), "Success message should be visible");
                await Task.Delay(2000);
            });
        }
        finally
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
    }
}

