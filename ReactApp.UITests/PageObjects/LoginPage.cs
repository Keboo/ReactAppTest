namespace ReactApp.UITests.PageObjects;

/// <summary>
/// Page Object Model for the Login page
/// </summary>
public class LoginPage(IPage page): TestPageBase(page)
{
    // Locators - MUI TextFields need to target the actual input inside the wrapper
    private ILocator EmailInput => Page.GetByTestId("email-input").Locator("input");
    private ILocator PasswordInput => Page.GetByTestId("password-input").Locator("input");
    private ILocator LoginButton => Page.GetByTestId("login-button");
    private ILocator LogoutButton => Page.Locator("button:has-text('Logout')");

    public Task NavigateAsync(Uri baseUrl) => PerformNavigationAsync(baseUrl, "login");

    public async Task LoginAsync(string email, string password)
    {
        await EmailInput.FillAsync(email);
        await PasswordInput.FillAsync(password);
        
        // Ensure the button is ready (React might still be attaching event handlers)
        await LoginButton.WaitForAsync(new LocatorWaitForOptions 
        { 
            State = WaitForSelectorState.Visible,
            Timeout = 5000 
        });
        
        // Wait for navigation to my-rooms after submitting the form
        // Use force: true to bypass actionability checks that may fail on Linux
        await Page.RunAndWaitForNavigationAsync(async () =>
        {
            await LoginButton.ClickAsync(new LocatorClickOptions { Force = true });
        }, new PageRunAndWaitForNavigationOptions
        {
            UrlString = "**/my-rooms",
            Timeout = 30000,
            WaitUntil = WaitUntilState.Load  // Use Load instead of NetworkIdle for better Linux compatibility
        });
    }
    
    public async Task<bool> IsLoggedInAsync()
    {
        // Check if we're on a page that requires authentication
        // or if we can find user-specific elements
        var url = Page.Url;
        
        // If we're on my-rooms page, we're logged in
        if (url.Contains("/my-rooms"))
        {
            return true;
        }
        
        // If we're still on the login page, we're not logged in
        if (url.Contains("/login"))
        {
            return false;
        }
        
        // Look for various indicators that user is logged in
        // Use Count to avoid strict mode violations
        var logoutButtonCount = await LogoutButton.CountAsync();
        var myRoomsButtonCount = await Page.Locator("button:has-text('My Rooms')").CountAsync();
        
        return logoutButtonCount > 0 || myRoomsButtonCount > 0;
    }
    
    public async Task LogoutAsync()
    {
        await LogoutButton.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }
}
