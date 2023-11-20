# 🌐 SimbirSoft Web API in brief
A RESTful API service for a centralized database of past records, facilitating multi-year experiments, animal migrations, and environmental tracking. Tested with a Docker image.

# 📜 Legend
Our company "Drip-Chip" is engaged in the chip-tracking of animals in the country "Wonderland" to monitor their movements and life cycles. Movement of animals on the planet is extremely important, including to protect them from extinction.

This year, our company decided to create a unified base, where the records of previous years will be transferred, for conducting long-term experiments related to animal migrations, as well as for tracking changes in habitats and keeping a history.

# 🔧 System Functionality Task

### The system should have the following components:

Account
- Role
- Animal
- Animal Type
- Location Point
- Animal Visited Location
- Area
- Area Analytics

### The following functionality should be available in the controllers:

Authentication:
- Account registration

Account:
- View account information
- Search/change/delete account

Animal:

- View animal information
- Search/create/change/delete animal
- Create/change/delete animal type

Animal Type:
- View animal type information
- Create/change/delete animal type

Location Point:
- View location point information
- Create/change/delete location point

Animal Visited Location:
- View animal movement information
- Create/change/delete animal's location point

Area:
- View area information
- Create/change/delete area

Area Analytics:
- View animal movements in the area

### At the first launch of the application, the following accounts are automatically created in the database:
```json
{
  "id": 1,				// User account identifier
  "firstName": "adminFirstName",	// User's first name
  "lastName": "adminLastName",	        // User's last name
  "email": "admin@simbirsoft.com",	// User's email address
  "password": "qwerty123",		// User's account password
  "role": "ADMIN"			// User's account role
},
{
  "id": 2,			        // User account identifier
  "firstName": "chipperFirstName",	// User's first name
  "lastName": "chipperLastName",	// User's last name
  "email": "chipper@simbirsoft.com",	// User's email address
  "password": "qwerty123",		// User's account password
  "role": "CHIPPER"			// User's account role
},
{
  "id": 3,				// User account identifier
  "firstName": "userFirstName",		// User's first name
  "lastName": "userLastName",		// User's last name
  "email": "user@simbirsoft.com",	// User's email address
  "password": "qwerty123",		// User's account password
  "role": "USER"			// User's account role
}
```

# 🐾 API Declarations
### 1) User authentication
<i>API 1. Register a new account</i>

Endpoint: `POST /registration`

Request:
```json
{
  "firstName": "string",  // User's first name
  "lastName": "string",   // User's last name
  "email": "string",      // Email address
  "password": "string"    // User's account password
}
```
Response:
```json
{
  "id": "int",            // User's account ID
  "firstName": "string",  // User's first name
  "lastName": "string",   // User's last name
  "email": "string",      // Email address
  "role": "string"        // User's account role, defaults to "USER" upon registration
}
```

### 2) User account
<i>API 1: Get user account information</i>

Endpoint: `GET /accounts/{accountId}`

Request:
```json
{
// No request body
}
```
Response:
```json
{
 "id": "int",           // User's account ID
 "firstName": "string", // User's first name
 "lastName": "string",  // User's last name
 "email": "string",     // Email address
 "role": "string"       // User's account role, defaults to "USER" upon registration
}
```

<i>API 2: Search user accounts</i>

Endpoint: `GET /accounts/search/?firstName={firstName}&lastName={lastName}&email={email}&from={from}&size={size}`

Request:
```json
{
 // No request body
}
```
Response:
```json
[
 {
  "id": "int",          // User's account ID
  "firstName": "string", // User's first name
  "lastName": "string", // User's last name
  "email": "string",    // Email address
  "role": "string"      // User's account role, defaults to "USER" upon registration
 }
]
```

<i>API 3: Add a new user account</i>

Endpoint: `POST /accounts`

Request:
```json
{
 "firstName": "string", // User's first name
 "lastName": "string",  // User's last name
 "email": "string",     // Email address
 "password": "string",  // User's account password
 "role": "string"       // User's account role, available values "ADMIN", "CHIPPER", "USER"
}
```
Response:
```json
{
 "id": "int",           // User's account ID
 "firstName": "string", // User's first name
 "lastName": "string",  // User's last name
 "email": "string",     // Email address
 "role": "string"       // User's account role
}
```

<i>API 4: Update user account information</i>

Endpoint: `PUT /accounts/{accountId}`

Request:
```json
{
 "firstName": "string", // New user's first name
 "lastName": "string",  // New user's last name
 "email": "string",     // New email address
 "password": "string",  // New user's account password
 "role": "string"       // New user's account role, available values "ADMIN", "CHIPPER", "USER"
}
```
Response:
```json
{
 "id": "int",           // User's account ID
 "firstName": "string", // New user's first name
 "lastName": "string",  // New user's last name
 "email": "string",     // New email address
 "role": "string"       // New user's account role
}
```

<i>API 5: Delete a user account</i>

Endpoint: `DELETE /accounts/{accountId}`

Request:
```json
{
 // No request body
}
```
Response:
```json
{
 // No response body
}
```
