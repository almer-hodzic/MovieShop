# MovieShop Business Flow

## Authentication

Users can register, confirm email, log in, refresh tokens, request password reset, reset password, and complete two-factor verification. Admin-only backend endpoints are protected with the `AdminOnly` policy.

## Movie Browse and Purchase

1. A user logs in.
2. The user browses movies and opens movie details.
3. The user can add movies to favourites.
4. The user adds movies to the shopping cart.
5. The user opens checkout.
6. The backend creates a PayPal sandbox order for the cart total.
7. The user approves payment in PayPal sandbox.
8. The backend captures the PayPal order and clears/completes the checkout state.

## Notifications

1. An administrator creates a notification.
2. User notification rows are created for recipients.
3. Users see unread notifications.
4. Users can mark notifications as read/unread or delete their own notification entry.

## Administration

Administrators can view dashboard counts and manage settings, movies, categories, actors, directors, reviews, notifications, favourites, and shopping-cart administration screens.
