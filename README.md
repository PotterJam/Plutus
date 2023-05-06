## Plutus

The codebase for Plutus, a web app for wealth tracking and budgeting. More to come.

.NET 7 with a React frontend and Postgres database.

# Building the project

When first building the project, you'll want to install Rider, the latest version of .NET 7, and Node (for the web build, see README in `Plutus/ClientApp`)

Then the environment variables need to be set up, which for the developer build live in `Plutus/Properties/launchSettings.json`.

Starting with the `HOST_NAME`, you can leave this as is unless hosting.

For the database env vars, you will need a postgres database that has all of the sql in `/db.sql` (in the root directory).

To get the GitHub client id and secret you'll need to create an oauth application in GitHub (https://github.com/settings/applications).

For localhost you'll need a `Homepage URL` of `https://localhost:44492/` and `Authorization callback URL` of `https://localhost:44492/`.

For production you'll need a `Homepage URL` of `https://livelin.es/` and `Authorization callback URL` of `http://livelin.es/`. This is HTTP because of the TLS redirect I have in production.

Once the web build is initialised and all of the environment variables have been set, the build in rider should run the backend, and then create the proxy for the frontend. Tada.
