# Employee API Authorization

This document explains the authorization implementation for the Employee CRUD API.

## Overview

The Employee API uses JWT (JSON Web Token) based authentication and role-based authorization to secure its endpoints.

## Authentication Flow

1. **Register**: Create a new user with a username, password, and role (Admin or User)
2. **Login**: Authenticate with credentials to receive a JWT token
3. **Access API**: Include the JWT token in the Authorization header for API requests

## Roles and Permissions

### Admin Role
- Can perform all operations:
  - View all employees
  - View employee details
  - Create new employees
  - Update employee information
  - Delete employees
  - View high earners list
  - View department statistics

### User Role
- Has read-only access:
  - View all employees
  - View employee details
  - View high earners list

## API Endpoints

### Authentication Endpoints
- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Login and get JWT token

### Employee Endpoints
- `GET /api/employee` - Get all employees (Admin/User)
- `GET /api/employee/{id}` - Get employee by ID (Admin/User)
- `POST /api/employee` - Create new employee (Admin only)
- `PUT /api/employee/{id}` - Update employee (Admin only)
- `DELETE /api/employee/{id}` - Delete employee (Admin only)
- `GET /api/employee/high-salary` - Get high earners (Admin/User)
- `GET /api/employee/stats` - Get department statistics (Admin only)

## Usage Example

### 1. Register an Admin User
```bash
curl -X POST http://localhost:5156/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "Admin123!",
    "role": "Admin"
  }'
```

### 2. Login and Get Token
```bash
curl -X POST http://localhost:5156/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "Admin123!"
  }'
```

### 3. Use Token to Access Protected Endpoint
```bash
curl -X GET http://localhost:5156/api/employee \
  -H "Authorization: Bearer YOUR_JWT_TOKEN_HERE"
```

## Implementation Details

### JWT Configuration
The JWT settings are configured in `appsettings.json`:
- Key: Secret key for token signing
- Issuer: Token issuer
- Audience: Intended audience for the token
- Expiration: Tokens expire after 7 days

### Authorization Attributes
The API uses ASP.NET Core authorization attributes:
- `[Authorize]` - Requires authentication
- `[Authorize(Roles = "Admin")]` - Requires Admin role
- `[Authorize(Roles = "Admin,User")]` - Requires either Admin or User role

### Security Features
- Passwords are hashed using BCrypt
- JWT tokens contain user identity and role claims
- All employee endpoints require authentication
- Sensitive operations require Admin role

## Testing

Use the provided `EmployeeApiAuth.http` file with Visual Studio's REST Client extension to test the authorization flow. The file includes examples for:
- Registering users
- Logging in
- Accessing endpoints with different roles

Remember to replace the placeholder tokens with actual JWT tokens received from the login endpoint.