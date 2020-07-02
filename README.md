# AddressBook

AddressBook.API is a simple example implemented for studing .NET Core to develop WebApi.

## Persistency

Current implementation support two different target to store the data, SQLite and SQLServer 2019.\
This choice can be configured inside the *appsettings.json* file on the key **TargetDatabase** and the allowed key are:
* **SqlServer** to select SQLServer
* **SqlLite** to select SQLite

In the **ConnectionStrings** section there are the connection strings for the target databases.

### SQLite
The SQLite database dosn't needed any configuration and works fine, the only configuration is the path and the name for the database (for example *Data/AddressBook.db)

### SQLServer 2019
To use SQLServer for development I've used this guide to install SQLServer with docker on my mac [SQLServer 2019 with Docker on macos](https://database.guide/how-to-install-sql-server-on-a-mac/)
