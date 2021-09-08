## Example Tendermint C# GRPC ABCI App

This is a `C#` example of a very simple key-value store kept in sync by validators. It is built on top of Tendermint (Cosmos) blockchain. I publish it due to the lack of clarity in official Tendermint documentation on how to run their marvelous piece of software with `C#` (there are no `.NET` examples at the moment). 

#### You need to manually apply the following migrations using `Package Manager Console` for the `Db` to be created properly:
```
Add-Migration InitialCreate
Update Database
```

#### Resetting the migrations:
```
Update Database 0
Remove-Migration InitialCreate
```
   
#### Main problems Tendermint documentation fails to mention:

1. You have to remove protocol from your configuration when pointing to the proxy app: `tcp://127.0.0.1:<port>` should be `127.0.0.1:<port>` and YES, it will throw regardless if you have protocol specified in the `.toml` file or as a flag in the console.
2. The flag is `--proxy_app` NOT `--proxy-app` 
3. Additionally to following the tutorial, you also have to EXPLICITLY override and implement `Info()`, `Echo()` and `InitChain()` methods, to prevent Unimplemented Exceptions from being thrown



