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

At the first launch of the application, the following accounts are automatically created in the database:
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
