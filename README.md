# src
Setting up database
https://stackoverflow.com/questions/1933134/add-iis-7-apppool-identities-as-sql-server-logons
Will have to add appPool as an identity
	- make sure to assign sysadmin role
	- map to chess database
	- can just copy the normal user login to be sure it works as intended