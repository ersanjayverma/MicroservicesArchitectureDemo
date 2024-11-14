# MicroservicesArchitectureDemo
# File Upload and Authentication Microservices

This project demonstrates a simple microservices architecture with a focus on **file upload** and **authentication/authorization** using JWT tokens. The project includes endpoints for file uploads, downloads, and a token verification service.

## Services Implemented

1. **File Upload Service**: A service to upload files, store them in memory, and retrieve them as needed.
2. **Authentication Service**: A service for user login, registration, and JWT-based authentication.
3. **In-Memory File Store**: Stores files temporarily in memory using an in-memory dictionary.

## Project Overview

The project consists of two main services:
1. **FileUploadService**: Handles file uploads, downloads, and stores files in memory.
2. **AuthService**: Handles user authentication and JWT token validation.

## Technologies Used

- **ASP.NET Core**: Framework for building the web API services.
- **JWT Authentication**: For secure authentication and authorization.
- **In-Memory File Store**: For temporarily storing files in memory.
- **Swagger**: For API documentation and testing.
  
## Setup and Installation

### Prerequisites

1. [.NET SDK](https://dotnet.microsoft.com/download) version 6.0 or later
2. Any code editor (Visual Studio, Visual Studio Code, etc.)

### Clone the Repository

```bash
git clone https://github.com/yourusername/file-upload-auth-microservice.git
cd file-upload-auth-microservice
Build and Run
Restore the required dependencies:


dotnet restore
Build the project:


dotnet build
Run the project:


dotnet run
The application will run on https://localhost:5001.

API Documentation (Swagger)
Once the application is running, you can access the Swagger UI at https://localhost:5001/swagger to view and test the available endpoints.

Available Endpoints
1. File Upload
Method: POST

Endpoint: /files/upload

Description: Upload a file and store it in memory.

Request: A multipart/form-data request containing the file.

Example Request (using curl):


curl -X POST https://localhost:5001/files/upload -F "file=@path/to/your/file.txt"
Response: A success message confirming the file upload.


{
  "message": "File file.txt uploaded successfully!"
}
2. File Download
Method: GET

Endpoint: /files/download/{fileName}

Description: Download a file from the in-memory file store.

Response: The requested file will be returned as a binary stream (Content-Disposition will be set to attachment).

Example Request:


curl -O https://localhost:5001/files/download/yourfile.txt
Response: The file content.

3. Token Verification
Method: GET

Endpoint: /auth/verify

Description: Verify whether the provided JWT token is valid.

Headers:

Authorization: Bearer token
Response: A message indicating whether the token is valid or not.

Example Request (using curl):


curl -X GET https://localhost:5001/auth/verify -H "Authorization: Bearer your_jwt_token"
Example Response:


{
  "message": "Token is valid."
}
4. User Login
Method: POST
Endpoint: /auth/login
Description: Authenticate a user and return a JWT token.
Request Body:

{
  "username": "yourUsername",
  "password": "yourPassword"
}
Response:

{
  "token": "your_jwt_token"
}
5. User Registration
Method: POST
Endpoint: /auth/register
Description: Register a new user.
Request Body:

{
  "username": "newUsername",
  "password": "newPassword"
}
Response:

{
  "message": "User registered successfully."
}
File Store (In-Memory)
The InMemoryFileStore class is used to temporarily store files in memory. Files are stored as byte arrays with the file name as the key.

AddFile: Adds a file to the store.
GetFile: Retrieves a file from the store based on its name.
This approach allows the project to be lightweight and does not require persistent storage, but it will lose the files when the application restarts.

JWT Authentication
JWT tokens are used to authenticate and authorize users. When users log in, a token is returned and needs to be passed in the Authorization header for accessing protected endpoints, such as file download and token verification.

Token Example

{
  "token": "your_jwt_token_here"
}
Use this token to authenticate requests to protected routes.

Additional Notes
Swagger UI: Swagger provides an easy-to-use interface for testing all available API endpoints. Access it by navigating to https://localhost:5001/swagger in your browser.
Error Handling: The API includes basic error handling for missing files, invalid tokens, and other potential issues. The server will return appropriate HTTP status codes and messages.
