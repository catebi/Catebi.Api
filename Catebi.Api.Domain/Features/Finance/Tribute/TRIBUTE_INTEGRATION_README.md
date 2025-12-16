# Tribute.tg Webhook Integration

This document describes the Tribute.tg webhook integration for receiving subscription and donation events.

## Overview

The integration receives webhooks from Tribute.tg through a unified endpoint, validates signatures, stores data in Airtable, and sends notifications to a Telegram superchat topic. All webhook event types (new_subscription, cancelled_subscription, recurrent_donation, cancelled_donation, and test events) are processed through a single endpoint that routes based on the event name.

## Architecture

### Components

1. **TributeController** - Handles webhook endpoints
2. **TributeService** - Business logic for processing subscriptions and donations
3. **TributeWebhookAuthorizeAttribute** - Validates webhook signatures using HMAC-SHA256
4. **Airtable Models** - Data models for Airtable storage
5. **DTOs** - Data transfer objects for API responses

## Configuration

### appsettings.json

Add the following configuration section:

```json
"Finance": {
  "Tribute": {
    "ApiKey": "your-tribute-api-key-here"
  },
  "Telegram": {
    "SuperchatId": "your-telegram-chat-id",
    "TopicId": "your-topic-id"
  },
  "Airtable": {
    "ApiKey": "your-airtable-api-key",
    "BaseId": "your-airtable-base-id"
  }
}
```

### Configuration Parameters

- **Tribute:ApiKey**: Your Tribute API key from Creator Dashboard → Settings → API Keys
- **Telegram:SuperchatId**: The Telegram chat/superchat ID where notifications will be sent
- **Telegram:TopicId**: The topic/thread ID within the superchat for notifications
- **Airtable:ApiKey**: Your Airtable API key
- **Airtable:BaseId**: The Airtable base ID for Finance

## Airtable Setup

Create a new Airtable base with two tables:

### TributeSubscription Table

Fields:
- `WebhookName` (Single line text) - Tribute webhook event name (e.g., "new_subscription", "cancelled_subscription")
- `Type` (Single line text) - Event type, same as WebhookName, used for filtering
- `SubscriptionName` (Single line text)
- `SubscriptionId` (Number)
- `PeriodId` (Number)
- `Period` (Single line text)
- `Price` (Number)
- `Amount` (Number)
- `Currency` (Single line text)
- `UserId` (Number)
- `TelegramUserId` (Single line text)
- `TelegramUsername` (Single line text)
- `ChannelId` (Number)
- `ChannelName` (Single line text)
- `ExpiresAt` (Date)
- `CreatedAt` (Date)
- `SentAt` (Date)

### TributeDonation Table

Fields:
- `WebhookName` (Single line text) - Tribute webhook event name (e.g., "new_donation", "recurrent_donation", "cancelled_donation")
- `Type` (Single line text) - Event type, same as WebhookName, used for filtering
- `DonationRequestId` (Number)
- `DonationName` (Single line text)
- `Period` (Single line text)
- `Amount` (Number)
- `Currency` (Single line text)
- `Anonymously` (Checkbox)
- `WebAppLink` (URL)
- `UserId` (Number)
- `TelegramUserId` (Single line text)
- `TelegramUsername` (Single line text)
- `CreatedAt` (Date)
- `SentAt` (Date)

## Webhook Endpoint

### Unified Webhook

**Endpoint**: `POST /Tribute/Webhook`

**Description**: Single unified endpoint that handles all Tribute webhook event types. The endpoint routes events based on the `name` field in the request body.

**Authentication**: Validates `trbt-signature` header using HMAC-SHA256

**Supported Event Types**:
- `new_subscription` - New subscription created
- `cancelled_subscription` - Subscription cancelled
- `new_donation` - One-time or first donation payment
- `recurrent_donation` - Recurring donation payment
- `cancelled_donation` - Recurring donation cancelled
- Test event (special format: `{"test_event": "test_event"}`)

### Event Examples

#### New Subscription
```json
{
  "name": "new_subscription",
  "created_at": "2025-08-25T01:15:58.33246Z",
  "sent_at": "2025-08-25T01:15:58.542279448Z",
  "payload": {
    "subscription_name": "Support creativity 🌟",
    "subscription_id": 1644,
    "period_id": 1547,
    "period": "monthly",
    "price": 1000,
    "amount": 700,
    "currency": "eur",
    "user_id": 31326,
    "telegram_user_id": 12321321,
    "channel_id": 614,
    "channel_name": "lbs",
    "expires_at": "2025-04-20T01:15:57.305733Z"
  }
}
```

**Response**:
```json
{
  "success": true,
  "message": "Subscription processed successfully",
  "recordId": "recXXXXXXXXXXXXXX"
}
```

#### Cancelled Subscription
```json
{
  "name": "cancelled_subscription",
  "created_at": "2025-03-21T11:20:44.013969Z",
  "sent_at": "2025-03-21T11:20:44.527657077Z",
  "payload": {
    "subscription_name": "Support creativity 🌟",
    "subscription_id": 1646,
    "period_id": 1549,
    "period": "monthly",
    "price": 1000,
    "amount": 1000,
    "currency": "eur",
    "user_id": 31326,
    "telegram_user_id": 12321321,
    "channel_id": 614,
    "channel_name": "lbs",
    "cancel_reason": "User requested cancellation",
    "expires_at": "2025-03-20T11:13:44.737Z"
  }
}
```

**Response**:
```json
{
  "success": true,
  "message": "Cancelled subscription processed successfully",
  "recordId": "recXXXXXXXXXXXXXX"
}
```

#### New Donation
```json
{
  "name": "new_donation",
  "created_at": "2025-03-20T01:15:58.33246Z",
  "sent_at": "2025-03-20T01:15:58.542279448Z",
  "payload": {
    "donation_request_id": 123,
    "donation_name": "Support my work",
    "message": "Thank you for your content!",
    "period": "once",
    "amount": 1000,
    "currency": "usd",
    "anonymously": false,
    "web_app_link": "https://t.me/tribute/app?startapp=d123",
    "user_id": 31326,
    "telegram_user_id": 12321321
  }
}
```

**Response**:
```json
{
  "success": true,
  "message": "New donation processed successfully",
  "recordId": "recYYYYYYYYYYYYYY"
}
```

#### Recurrent Donation
```json
{
  "name": "recurrent_donation",
  "created_at": "2025-03-20T01:15:58.33246Z",
  "sent_at": "2025-03-20T01:15:58.542279448Z",
  "payload": {
    "donation_request_id": 123,
    "donation_name": "Monthly support",
    "period": "monthly",
    "amount": 500,
    "currency": "eur",
    "anonymously": false,
    "web_app_link": "https://t.me/tribute/app?startapp=d456",
    "user_id": 31326,
    "telegram_user_id": 12321321
  }
}
```

**Response**:
```json
{
  "success": true,
  "message": "Donation processed successfully",
  "recordId": "recYYYYYYYYYYYYYY"
}
```

#### Cancelled Donation
```json
{
  "name": "cancelled_donation",
  "created_at": "2025-03-20T01:15:58.33246Z",
  "sent_at": "2025-03-20T01:15:58.542279448Z",
  "payload": {
    "donation_request_id": 123,
    "donation_name": "Monthly support",
    "period": "monthly",
    "amount": 500,
    "currency": "eur",
    "anonymously": false,
    "web_app_link": "https://t.me/tribute/app?startapp=d456",
    "user_id": 31326,
    "telegram_user_id": 12321321
  }
}
```

**Response**:
```json
{
  "success": true,
  "message": "Cancelled donation processed successfully",
  "recordId": "recYYYYYYYYYYYYYY"
}
```

#### Test Event
```json
{
  "test_event": "test_event"
}
```

**Response**:
```json
{
  "success": true,
  "message": "Test event received successfully"
}
```

## Admin Endpoints

### Get Subscriptions

**Endpoint**: `GET /Tribute/GetSubscriptions`

**Description**: Retrieves all subscriptions from Airtable

**Authentication**: Requires Telegram authentication with Admin role

**Response**: Array of subscription DTOs

### Get Donations

**Endpoint**: `GET /Tribute/GetDonations`

**Description**: Retrieves all donations from Airtable

**Authentication**: Requires Telegram authentication with Admin role

**Response**: Array of donation DTOs

## Security

### Webhook Signature Verification

All webhook endpoints validate the `trbt-signature` header using HMAC-SHA256:

1. The signature is extracted from the `trbt-signature` header
2. The request body is read as a string
3. HMAC-SHA256 hash is computed using the Tribute API key as the secret
4. The computed signature is compared with the received signature
5. If signatures don't match, the request is rejected with 401 Unauthorized

### Retry Policy

Tribute.tg automatically retries failed webhook deliveries:
- After 5 minutes
- After 15 minutes
- After 30 minutes
- After 1 hour
- After 10 hours

## Telegram Notifications

When a new subscription or donation is received, a notification is sent to the configured Telegram superchat and topic.

### Subscription Notification Format

```
🎉 New Subscription!

📝 Name: Support creativity 🌟
💰 Amount: 7.00 EUR
⏰ Period: monthly
👤 User ID: 31326
📱 Telegram ID: 12321321
📺 Channel: lbs
📅 Expires: 2025-04-20 01:15
🆔 Record ID: recXXXXXXXXXXXXXX
```

### Donation Notification Format

```
💝 New Donation!

📝 Name: Monthly support
💰 Amount: 5.00 EUR
⏰ Period: monthly
👤 Donor: User 31326 (TG: 12321321)
🔗 Link: https://t.me/tribute/app?startapp=d456
🆔 Record ID: recYYYYYYYYYYYYYY
```

For anonymous donations, the donor information shows "Anonymous".

## Setup in Tribute Creator Dashboard

1. Go to Creator Dashboard → Settings (three-dot menu) → API Keys section
2. Generate API key if you haven't already
3. Copy the API key and add it to your `appsettings.json`
4. In the webhook settings, specify your webhook URL:
   - Webhook URL: `https://your-api-domain.com/Tribute/Webhook`
   
This single endpoint will handle all webhook event types (new_subscription, cancelled_subscription, recurrent_donation, cancelled_donation).

## Testing

### Using Tribute Test API

Tribute provides a test API method that sends a test event to your webhook:

```json
{
  "test_event": "test_event"
}
```

The endpoint will respond with:
```json
{
  "success": true,
  "message": "Test event received successfully"
}
```

### Manual Testing with curl

You can test the endpoint manually with curl (you'll need to generate a valid signature):

```bash
# Calculate signature for test event
echo -n '{"test_event":"test_event"}' | \
  openssl dgst -sha256 -hmac "YOUR_API_KEY" | \
  awk '{print $2}'

# Make test request
curl -X POST https://your-api-domain.com/Tribute/Webhook \
  -H "Content-Type: application/json" \
  -H "trbt-signature: CALCULATED_SIGNATURE" \
  -d '{"test_event":"test_event"}'

# Test new subscription event
echo -n '{"name":"new_subscription","created_at":"2025-08-25T01:15:58.33246Z","sent_at":"2025-08-25T01:15:58.542279448Z","payload":{"subscription_name":"Test","subscription_id":1,"period_id":1,"period":"monthly","price":1000,"amount":700,"currency":"eur","user_id":123,"telegram_user_id":12345678,"channel_id":1,"channel_name":"test","expires_at":"2025-04-20T01:15:57.305733Z"}}' | \
  openssl dgst -sha256 -hmac "YOUR_API_KEY" | \
  awk '{print $2}'

curl -X POST https://your-api-domain.com/Tribute/Webhook \
  -H "Content-Type: application/json" \
  -H "trbt-signature: CALCULATED_SIGNATURE" \
  -d '{"name":"new_subscription","created_at":"2025-08-25T01:15:58.33246Z","sent_at":"2025-08-25T01:15:58.542279448Z","payload":{"subscription_name":"Test","subscription_id":1,"period_id":1,"period":"monthly","price":1000,"amount":700,"currency":"eur","user_id":123,"telegram_user_id":12345678,"channel_id":1,"channel_name":"test","expires_at":"2025-04-20T01:15:57.305733Z"}}'
```

## Troubleshooting

### Webhook Signature Validation Fails

- Verify the API key in `appsettings.json` matches the one from Tribute dashboard
- Ensure the request body is not modified before validation
- Check that the `trbt-signature` header is present in the request

### Airtable Errors

- Verify the Airtable API key and base ID are correct
- Ensure the table names match exactly: "TributeSubscription" and "TributeDonation"
- Check that all required fields exist in the Airtable tables

### Telegram Notifications Not Sent

- Verify the superchat ID and topic ID are correct
- Ensure the bot has permission to post in the superchat and topic
- Check the logs for any Telegram API errors

## Monitoring

All webhook events are logged with appropriate log levels:
- Info: Successful processing
- Warning: Invalid webhook names or missing configuration
- Error: Processing failures, Airtable errors, Telegram errors

Monitor your application logs for any issues with webhook processing.

