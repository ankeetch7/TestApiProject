# TestApiProject
This is Asp Net Core Web Api project. Which generates token and view customers and banks. For Create User and Authentication the method is Allowed Anonymous and for Get Bank and Get Customer it has to be authorized.

First of all create a database for this project. Since migration is already provided in the project. You just have to write "update-database" in package manager console to create a database.

Now after that run the project. Although you cannot directly access the Customer and Bank API because is has to be authorized at first.

To be authorized, you have to be authenticated. So you are allowed to create a user and authenticate the user because these method are allowed anonymous which means that anyone can access the API for creating and authenticating. 

If you have created the user then go on authenticate user and type username and password. If everything is valid then you will have a 200 Ok response with JWT token.

Now with that response token you can access to other API's of the Program.

Just type Bearer "<your token here>"

Now you are all set to go.
