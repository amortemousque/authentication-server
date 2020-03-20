
# Authentication server
Authentication server base on Identity server 4. Manage your api, client, users, roles, permissions and tenants.

## Table of contents
- [Authentication server](#authentication-server)
  - [Table of contents](#table-of-contents)
  - [Installation](#installation)
  - [Authentication](#authentication)
  - [Manage authentication](#manage-authentication)
    - [Client](#client)
    - [API](#api)
  - [Manage authorization](#manage-authorization)
  - [Manage users](#manage-users)
  
## Installation

1. Restore the mongodb backup on the server of your choice.
2. Add you connection string on the appsettings
```shell
"ApplicationDbSettings": {
    "ConnectionString": "[ConnectionString]",
    "DatabaseName": "[DatabaseName]"
}
```
1. Generate locale certificate
```shell
dotnet dev-certs https --trust
```
1. Run the application
```shell
dotnet run
```
The application will be running on localhost:5001 by default.
The tenant should be indicated as a subdomain such : **sandbox**.localhost:5001 or **test**.localhost:5001. 

> ⚠️ If you there is no subdomain the fallback tenant will be **sandbox**

## Authentication
By default the app is initilizaed on  localhost:5001.
The sample database is initialized with a user granted with all permissions.
>Login: **admin@sandbox.com** \
Password: **7a4VHGEzuFiA**


## Manage authentication
### Client
Two types of clients are available. Single page application or machine to machine client.
- The first is to connect web client application with Implicit flow authentication.
- The second one connect a server to the a protected app throw client credentials (Id, secret). You can also activate the **resource owner **grant type to act on behalf of a user by sending the user’s name and password (Grant type : Resource Owner)

### API
Create OpenId API with extended customisable scopes.

## Manage authorization
Create roles, and permssions to restrict access to your api.
All permissions are associated with users throw roles.

## Manage users
You can create user from the backoffice or throw the registration page.
User authenticated throught the registration page has no permissions by default.

