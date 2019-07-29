## Connection String

Connection Sting is located in **IPInfo/WebAPI/appsetting.json**.
Modifications needed on
 1. Property -> data source. Add your server name instead.
 2. In case where windows authentication is not supported on your server, you should replace connection string in order to contain user and password 
> eg. Server=localhost,1433; Database=IPInfo;User=SA;
> Password=5XqroPsZb6bL; MultipleActiveResultSets=True


## Database

To create the Database choose one of the bellow ways

 1. EntityFramework command: Pick folder **IPInfo/WebAPI/** and run the command 
> dotnet ef database update
 2. Restore database on server. Backup file location: **IPInfo/WebAPI/SolutionItems/IPInfo.bak**
 3. Manually running sql queries. Queries location **IPInfo/WebAPI/SolutionItems/CreateTables.sql**