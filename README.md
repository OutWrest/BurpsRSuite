# Burps R Suite

Burps R Suite is a test application made to explore vulnerabilities on ASP.NET web applications.

## Installation 

Use [.NET](https://dotnet.microsoft.com/download) to compile and run the project:

```bash
git clone https://github.com/OutWrest/BurpsRSuite.git
cd BurpsRSuite
dotnet run
```

The app will run on http://localhost:5000/.

## Testing

There are pre-made test accounts to try out:

*Please note that the password rules are really lax to allow for faster testing.*

| Username     | Password        | First Name | Last Name | Email                     | Account Number | Answer 1 (Challenge Question) | Answer 2 (Challenge Question) | Has Two Factor |
|--------------|-----------------|------------|-----------|---------------------------|----------------|-------------------------------|-------------------------------|----------------|
| user         | asd             | name       | -         | user@aol.com              | 123            | a                             | a                             | false          |
| admin        | T0tally1337Pa$$ | name       | -         | admin@admin.com           | 1337           | a                             | a                             | false          |
| secureUser   | *               | Secure     | User      | user@protonmail.com       | **             | *                             | *                             | true           |
| secureUser** | *               | VerySecure | User      | secureuser@protonmail.com | **             | *                             | *                             | true           |
| secureAdmin  | *               | Secure     | Admin     | admin@protonmail.com      | **             | *                             | *                             | true           |

**random lowercase letters (length = 5)*

***random numbers (length = 2 for username and 9 for account number)*

## Known Issues

#### Cookie retention 

When closing a session while logged in, the browser saves the current cookie and loads it again in any new sessions. This causes exceptions because the users are deleted and reset every new session.

##### Fix (not really, just a workaround)

After clicking continue on the exceptions, delete the cookies and refresh the page. 

#### Failure to load users

There is a small chance that only some of the users will be created in a new session. It has something to do with the threads and I have no idea how to fix it.

#### Fix (also, not really)

Restart session and pray.
