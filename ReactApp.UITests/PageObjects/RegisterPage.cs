namespace ReactApp.UITests.PageObjects;

/// <summary>
/// Page Object Model for the Register page
/// </summary>
public class RegisterPage(IPage page) : TestPageBase(page)
{
    // Locators - MUI TextFields need to target the actual input inside the wrapper
    private ILocator EmailInput => Page.GetByTestId("email-input").Locator("input");
    private ILocator PasswordInput => Page.GetByTestId("password-input").Locator("input");
    private ILocator ConfirmPasswordInput => Page.GetByTestId("confirm-password-input").Locator("input");
    private ILocator RegisterButton => Page.GetByTestId("register-button");
    private ILocator SuccessMessage => Page.Locator("text=Registration successful");

    public Task NavigateAsync(Uri baseUrl) => PerformNavigationAsync(baseUrl, "register");

    public async Task RegisterAsync(string email, string password)
    {
        await EmailInput.FillAsync(email);
        await PasswordInput.FillAsync(password);
        await ConfirmPasswordInput.FillAsync(password);
        
        // Ensure the button is ready (React might still be attaching event handlers)
        await RegisterButton.WaitForAsync(new LocatorWaitForOptions 
        { 
            State = WaitForSelectorState.Visible,
            Timeout = 5000 
        });
        
        // Wait for navigation to my-rooms after submitting the form
        // Use force: true to bypass actionability checks that may fail on Linux
        await Page.RunAndWaitForNavigationAsync(async () =>
        {
            await RegisterButton.ClickAsync(new LocatorClickOptions { Force = true });
        }, new PageRunAndWaitForNavigationOptions
        {
            UrlString = "**/my-rooms",
            Timeout = 30000,
            WaitUntil = WaitUntilState.Load  // Use Load instead of NetworkIdle for better Linux compatibility
        });
    }
    
    public async Task<bool> IsConfirmationMessageVisibleAsync()
    {
        // React app redirects to /my-rooms on successful registration
        // User is now logged in and on my-rooms page
        var url = Page.Url;
        return url.Contains("/my-rooms");
    }
    
    /// <summary>
    /// For the React app, email confirmation is not required for login.
    /// This method is kept for API compatibility but is a no-op.
    /// </summary>
    public async Task<string> GetEmailConfirmationLinkAsync()
    {
        // React app auto-confirms email, no link needed
        await Task.CompletedTask;
        return string.Empty;
    }

    public async Task ConfirmAccountAsync()
    {
        // React app auto-confirms email, no manual confirmation needed
        await Task.CompletedTask;
    }
}
