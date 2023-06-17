# RedditShredder

Yet another Reddit shredder to modify all existing comments/posts in preparation for account deletion. The goal of this project is to make your content less useful before account deletion.

Features:
- Edit existing comments and self-posts with random content that appears to be valid text (reduces the usefulness of the data when being sold to a third-party). The random content is stable - meaning this script can be continued if stopped.
- Rate-limiting handling.
- Multi-threaded (defaults to number of CPU's).
- Multi-platform (Windows, OSX, and Linux).

## Releases

Releases can be download from the [Releases page](https://github.com/Silvenga/RedditShredder/releases/latest).

## Usage

| Option            | Required | Notes                                                                        |
|-------------------|----------|------------------------------------------------------------------------------|
| `--client-id`     | Yes      | Client id of an app registered with Reddit.                                  |
| `--client-secret` | Yes      | Client secret of an app registered with Reddit.                              |
| `--refresh-token` | Yes      | A refresh token of a user after authenticating using the provided client-id. |
| `--verbose`       | No       | Enable verbose logging.                                                      |
| `--footer`        | No       | Any footer to add to the end of generated content.                           |

For example:

```
./reddit-shredder --client-id <client-id> --client-secret <client-secret> --refresh-token <refresh-token>
```

## Getting Client Id/Secret and Refresh Tokens

The client id comes from the id that Reddit returns when you create a new Reddit app. New apps can be created [here](https://ssl.reddit.com/prefs/apps/). Use any name, and set the type to `script`. If using [Reddit OAuth Helper](https://not-an-aardvark.github.io/reddit-oauth-helper/) to create a refresh token, then the `redirect uri` must be `https://not-an-aardvark.github.io/reddit-oauth-helper/`

After creating the app, you'll have a client id (under the app name) and a client secret. To generate the refresh token, you can use [Reddit OAuth Helper](https://not-an-aardvark.github.io/reddit-oauth-helper/). Provide the `Client ID` and `Client Secret`, check `Permanent?` and all the listed scopes (for simplicity, only you will have access to this refresh token), and click `Generate tokens` at the bottom. The generated refresh token can then be used.
