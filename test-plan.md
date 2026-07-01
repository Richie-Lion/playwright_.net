# Test Plan: SauceDemo Simple Checkout

**Scenario:** A standard user logs in, adds a single item to the cart, and finishes the checkout process.

**Steps:**
1. **Navigate & Login:** - Open `https://www.saucedemo.com/`.
   - Enter the username: `standard_user`.
   - Enter the password: `secret_sauce`.
   - Click the Login button.
2. **Add to Cart:**
   - Wait for the inventory page to load.
   - Click the "Add to cart" button for the "Sauce Labs Backpack".
3. **Checkout:**
   - Click the shopping cart icon at the top right.
   - Click the "Checkout" button.
4. **Enter Details:**
   - Enter First Name: `Test`.
   - Enter Last Name: `User`.
   - Enter Zip Code: `12345`.
   - Click the "Continue" button.
5. **Finish:**
   - Click the "Finish" button.
   - Assert that the success message "Thank you for your order!" is visible on the screen.