# MovieShop API Services

This folder contains Angular service wrappers for the active MovieShop backend endpoints.

Current service areas:

- `auth`: register, confirm email, login, refresh, logout, forgot/reset password, and two-factor verification.
- `movies`, `categories`, `actors`, `directors`, `reviews`: catalog and administration APIs.
- `favourite-movies`: user favourite movie workflow.
- `shopping-cart`: active cart and checkout workflow.
- `paypal`: sandbox order creation and capture.
- `notifications`: user/admin notification workflow.
- `admin`: dashboard and settings APIs.
- `profile`: current user profile and password changes.

Keep this folder aligned with backend controller names and DTOs. Do not add template product/order service examples here.
