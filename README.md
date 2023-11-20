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
<i>API 1: Register a new account</i>

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

### 3) Animal Location Point
<i>API 1: Get animal location point information</i>

Endpoint: `GET /locations/{pointId}`

Request:
```json
{
 // No request body
}
```
Response:
```json
{
 "id": "long",          // Location point ID
 "latitude": "double",  // Geographic latitude in degrees
 "longitude": "double"  // Geographic longitude in degrees
}
```

<i>API 2: Add a new animal location point</i>

Endpoint: `POST /locations`

Request:
```json
{
 "latitude": "double",  // Geographic latitude in degrees
 "longitude": "double"  // Geographic longitude in degrees
}
```
Response:
```json
{
 "id": "long",          // Location point ID
 "latitude": "double",  // Geographic latitude in degrees
 "longitude": "double"  // Geographic longitude in degrees
}
```

<i>API 3: Update animal location point information</i>

Endpoint: `PUT /locations/{pointId}`

Request:
```json
{
 "latitude": "double",  // New geographic latitude in degrees
 "longitude": "double"  // New geographic longitude in degrees
}
```
Response:
```json
{
 "id": "long",          // Location point ID
 "latitude": "double",  // New geographic latitude in degrees
 "longitude": "double"  // New geographic longitude in degrees
}
```

<i>API 4: Delete an animal location point</i>

Endpoint: `DELETE /locations/{pointId}`

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

### 4) Zones
<i>API 1: Get zone information</i>

Endpoint: `GET /areas/{areaId}`

Request:
```json
{
 // No request body
}
```
Response:
```json
{
 "id": "long",         // Zone ID
 "name": "string",     // Zone name
 "areaPoints": [
   {
    "longitude": "double", // Geographic longitude in degrees
    "latitude": "double"  // Geographic latitude in degrees
   },
 ]
}
```

<i>API 2: Add a new zone</i>

Endpoint: `POST /areas`

Request:
```json
{
 "name": "string",     // Zone name
 "areaPoints": [
   {
    "longitude": "double", // Geographic longitude in degrees
    "latitude": "double"  // Geographic latitude in degrees
   },
 ]
}
```
Response:
```json
{
 "id": "long",         // Zone ID
 "name": "string",     // Zone name
 "areaPoints": [
   {
    "longitude": "double", // Geographic longitude in degrees
    "latitude": "double"  // Geographic latitude in degrees
   },
 ]
}
```

<i>API 3: Update zone information</i>

Endpoint: `PUT /areas/{areaId}`

Request:
```json
{
 "name": "string",     // Zone name
 "areaPoints": [
   {
    "longitude": "double", // Geographic longitude in degrees
    "latitude": "double"  // Geographic latitude in degrees
   },
 ]
}
```
Response:
```json
{
 "id": "long",         // Zone ID
 "name": "string",     // Zone name
 "areaPoints": [
   {
    "longitude": "double", // Geographic longitude in degrees
    "latitude": "double"  // Geographic latitude in degrees
   },
 ]
}

```

<i>API 4: Delete a zone</i>

Endpoint: `DELETE /areas/{areaId}`

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

### 5) Animal Types
<i>API 1: Get animal type information</i>

Endpoint: `GET /animals/types/{typeId}`

Request:
```json
{
 // No request body
}
```
Response:
```json
{
 "id": "long",        // Animal type ID
 "type": "string"     // Animal type
}
```

<i>API 2: Add a new animal type</i>

Endpoint: `POST /animals/types`

Request:
```json
{
 "type": "string"     // Animal type
}
```
Response:
```json
{
 "id": "long",        // Animal type ID
 "type": "string"     // Animal type
}
```

<i>API 3: Update animal type information</i>

Endpoint: `PUT /animals/types/{typeId}`

Request:
```json
{
 "type": "string"     // New animal type
}
```
Response:
```json
{
 "id": "long",        // Animal type ID
 "type": "string"     // New animal type
}
```

<i>API 4: Delete an animal type</i>

Endpoint: `DELETE /animals/types/{typeId}`

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

### 1) Animals
<i>API 1: Get animal information</i>

Endpoint: `GET /animals/{animalId}`

Request:
```json
{
 // No request body
}
```
Response:
```json
{
 "id": "long",                     // Animal ID
 "animalTypes": "[long]",           // Array of animal type IDs
 "weight": "float",                // Animal weight in kg
 "length": "float",                // Animal length in m
 "height": "float",                // Animal height in m
 "gender": "string",                // Animal gender (MALE, FEMALE, OTHER)
 "lifeStatus": "string",            // Animal life status (ALIVE, DEAD)
 "chippingDateTime": "dateTime",    // Animal chipping date and time in ISO-8601 format
 "chipperId": "int",                // Chipper account ID
 "chippingLocationId": "long",      // Animal chipping location ID
 "visitedLocations": "[long]",      // Array of visited location IDs
 "deathDateTime": "dateTime"        // Animal death date and time in ISO-8601 format (null while lifeStatus is ALIVE)
}
```

<i>API 2: Search animals by parameters</i>

Endpoint: `GET /animals/search?startDateTime={startDateTime}&endDateTime={endDateTime}&chipperId={chipperId}&chippingLocationId={chippingLocationId}&lifeStatus={lifeStatus}&gender={gender}&from=0&size=10`

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
  "id": "long",                     // Animal ID
  "animalTypes": "[long]",           // Array of animal type IDs
  "weight": "float",                // Animal weight in kg
  "length": "float",                // Animal length in m
  "height": "float",                // Animal height in m
  "gender": "string",                // Animal gender (MALE, FEMALE, OTHER)
  "lifeStatus": "string",            // Animal life status (ALIVE, DEAD)
  "chippingDateTime": "dateTime",    // Animal chipping date and time in ISO-8601 format
  "chipperId": "int",                // Chipper account ID
  "chippingLocationId": "long",      // Animal chipping location ID
  "visitedLocations": "[long]",      // Array of visited location IDs
  "deathDateTime": "dateTime"        // Animal death date and time in ISO-8601 format (null while lifeStatus is ALIVE)
 }
]
```

<i>API 3: Add a new animal</i>

Endpoint: `POST /animals`

Request:
```json
{
 "animalTypes": "[long]",           // Array of animal type IDs
 "weight": "float",                // Animal weight in kg
 "length": "float",                // Animal length in m
 "height": "float",                // Animal height in m
 "gender": "string",                // Animal gender (MALE, FEMALE, OTHER)
 "chipperId": "int",                // Chipper account ID
 "chippingLocationId": "long"       // Animal chipping location ID
}
```
Response:
```json
{
 "id": "long",                     // Animal ID
 "animalTypes": "[long]",           // Array of animal type IDs
 "weight": "float",                // Animal weight in kg
 "length": "float",                // Animal length in m
 "height": "float",                // Animal height in m
 "gender": "string",                // Animal gender (MALE, FEMALE, OTHER)
 "lifeStatus": "string",            // Animal life status (ALIVE, DEAD)
 "chippingDateTime": "dateTime",    // Animal chipping date and time in ISO-8601 format
 "chipperId": "int",                // Chipper account ID
 "chippingLocationId": "long",      // Animal chipping location ID
 "visitedLocations": "[long]",      // Array of visited location IDs
 "deathDateTime": "dateTime"        // Animal death date and time in ISO-8601 format (null while lifeStatus is ALIVE)
}
```

<i>API 4: Update animal information</i>

Endpoint: `PUT /animals/{animalId}`

Request:
```json
{
 "weight": "float",                // Animal weight in kg
 "length": "float",                // Animal length in m
 "height": "float",                // Animal height in m
 "gender": "string",                // Animal gender (MALE, FEMALE, OTHER)
 "lifeStatus": "string",            // Animal life status (ALIVE, DEAD)
 "chipperId": "int",                // Chipper account ID
 "chippingLocationId": "long"       // Animal chipping location ID
}
```
Response:
```json
{
 "id": "long",                     // Animal ID
 "animalTypes": "[long]",           // Array of animal type IDs
 "weight": "float",                // Animal weight in kg
 "length": "float",                // Animal length in m
 "height": "float",                // Animal height in m
 "gender": "string",                // Animal gender (MALE, FEMALE, OTHER)
 "lifeStatus": "string",            // Animal life status (ALIVE, DEAD)
 "chippingDateTime": "dateTime",    // Animal chipping date and time in ISO-8601 format
 "chipperId": "int",                // Chipper account ID
 "chippingLocationId": "long",      // Animal chipping location ID
 "visitedLocations": "[long]",      // Array of visited location IDs
 "deathDateTime": "dateTime"        // Animal death date and time in ISO-8601 format (null while lifeStatus is ALIVE)
}
```

<i>API 5: Delete an animal</i>

Endpoint: `DELETE /animals/{animalId}`

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

<i>API 6: Add an animal type to an animal</i>

Endpoint: `POST /animals/{animalId}/types/{typeId}`

Request:
```json
{
 // No request body
}
```
Response:
```json
{
 "id": "long",                   // Animal ID
 "animalTypes": "[long]",          // Array of animal type IDs
 "weight": "float",               // Animal weight in kg
 "length": "float",               // Animal length in m
 "height": "float",               // Animal height in m
 "gender": "string",               // Animal gender (MALE, FEMALE, OTHER)
 "lifeStatus": "string",           // Animal life status (ALIVE, DEAD)
 "chippingDateTime": "dateTime",   // Animal chipping date and time in ISO-8601 format
 "chipperId": "int",               // Chipper account ID
 "chippingLocationId": "long",     // Animal chipping location ID
 "visitedLocations": "[long]",     // Array of visited location IDs
 "deathDateTime": "dateTime"       // Animal death date and time in ISO-8601 format (null while lifeStatus is ALIVE)
}
```

<i>API 7: Change the animal type of an animal</i>

Endpoint: `PUT /animals/{animalId}/types`

Request:
```json
{
 "oldTypeId": "long", 			// Current animal type ID
 "newTypeId": "long" 			// New animal type ID for replacement
}
```
Response:
```json
{
 "id": "long",                   // Animal ID
 "animalTypes": "[long]",          // Array of animal type IDs
 "weight": "float",               // Animal weight in kg
 "length": "float",               // Animal length in m
 "height": "float",               // Animal height in m
 "gender": "string",               // Animal gender (MALE, FEMALE, OTHER)
 "lifeStatus": "string",           // Animal life status (ALIVE, DEAD)
 "chippingDateTime": "dateTime",   // Animal chipping date and time in ISO-8601 format
 "chipperId": "int",               // Chipper account ID
 "chippingLocationId": "long",     // Animal chipping location ID
 "visitedLocations": "[long]",     // Array of visited location IDs
 "deathDateTime": "dateTime"       // Animal death date and time in ISO-8601 format (null while lifeStatus is ALIVE)
}
```

<i>API 8: Delete the animal type of an animal</i>

Endpoint: `DELETE /animals/{animalId}/types/{typeId}`

Request:
```json
{
 // No request body
}
```
Response:
```json
{
 "id": "long",                   // Animal ID
 "animalTypes": "[long]",          // Array of animal type IDs
 "weight": "float",               // Animal weight in kg
 "length": "float",               // Animal length in m
 "height": "float",               // Animal height in m
 "gender": "string",               // Animal gender (MALE, FEMALE, OTHER)
 "lifeStatus": "string",           // Animal life status (ALIVE, DEAD)
 "chippingDateTime": "dateTime",   // Animal chipping date and time in ISO-8601 format
 "chipperId": "int",               // Chipper account ID
 "chippingLocationId": "long",     // Animal chipping location ID
 "visitedLocations": "[long]",     // Array of visited location IDs
 "deathDateTime": "dateTime"       // Animal death date and time in ISO-8601 format (null while lifeStatus is ALIVE)
}
```

### 7) Location point visited by the animal
<i>API 1: View location points visited by the animal</i>

Endpoint: `GET /animals/{animalId}/locations?startDateTime=&endDateTime=&from={from}&size={size}`

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
 "id": "long",                 // ID of the location object
 "dateTimeOfVisitLocationPoint": "dateTime", // Date and time of the location visit in ISO-8601 format
 "locationPointId": "long"     // ID of the visited location point
 }
]
```

<i>API 2: Add a location point visited by an animal</i>

Endpoint: `POST /animals/{animalId}/locations/{pointId}`

Request:
```json
{
 // No request body
}
```
Response:
```json
{ 
 "id": "long",                 // ID of the location object
 "dateTimeOfVisitLocationPoint": "dateTime", // Date and time of the location visit in ISO-8601 format
 "locationPointId": "long"     // ID of the visited location point
}
```

<i>API 3: Change the location point visited by the animal</i>

Endpoint: `PUT /animals/{animalId}/locations`

Request:
```json
{
 // No request body
}
```
Response:
```json
{ 
 "id": "long",                 // ID of the location object
 "dateTimeOfVisitLocationPoint": "dateTime", // Date and time of the location visit in ISO-8601 format
 "locationPointId": "long"     // ID of the visited location point
}
```

<i>API 4: Deleting a location visited by an animal</i>

Endpoint: `DELETE /animals/{animalId}/locations/{visitedPointId}`

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

