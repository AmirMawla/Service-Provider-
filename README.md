# Service-Provider-
Graduation Project for City Management System And Micro Service About Service Provider


# Service Provider API Documentation

## Introduction

The Service Provider API is a RESTful interface designed to facilitate interactions with the Service Provider platform. It enables users, vendors, and administrators to manage carts, categories, orders, products, reviews, subcategories, vendors, and administrative tasks programmatically. This documentation provides detailed information on all available endpoints, including their methods, parameters, authentication requirements, and example requests and responses.

### Base URL

All endpoints are relative to the base URL: `{{host}}`. Replace `{{host}}` with the actual domain of the deployed service (e.g., `https://service-provider-api.example.com`).

### Authentication

Most endpoints require authentication using a JSON Web Token (JWT) bearer token. Tokens are obtained via the "Log In" endpoint in the "Auth" section.

- **Token Type**: Bearer
- **Header**: `Authorization: Bearer <token>`
- **Roles**: Some endpoints are restricted to specific roles (e.g., "Admin" or "Vendor"), as indicated in the endpoint descriptions.

### Error Handling

The API uses standard HTTP status codes to indicate the outcome of requests:

- **200 OK**: Request successful.
- **201 Created**: Resource created successfully.
- **400 Bad Request**: Invalid request data.
- **401 Unauthorized**: Missing or invalid authentication token.
- **403 Forbidden**: Authenticated but insufficient permissions.
- **404 Not Found**: Resource not found.
- **500 Internal Server Error**: Server-side error.

Error responses typically include a JSON body with details:

```json
{
  "message": "Error description"
}
```

---

## Endpoints

The API is organized into resource-based sections. Each section contains endpoints related to a specific functionality or entity.

### Auth

#### Log In

- **Method**: `POST`
- **URL**: `/Auth`
- **Description**: Authenticates a user and returns a JWT token for subsequent requests.
- **Authentication**: Not required
- **Request Body**:
  ```json
  {
    "email": "string",    // User's email address
    "password": "string"  // User's password
  }
  ```
- **Response** (200 OK):
  ```json
  {
    "id": "string",          // User's unique identifier
    "email": "string",       // User's email
    "fullName": "string",    // User's full name
    "businessName": "string", // Business name (if applicable)
    "businessType": "string", // Business type (if applicable)
    "token": "string",       // JWT token
    "expiresIn": "number"    // Token expiration time in seconds
  }
  ```
- **Example Request**:
  ```bash
  curl -X POST {{host}}/Auth \
  -H "Content-Type: application/json" \
  -d '{"email": "admin@vendor.com", "password": "Admin@123"}'
  ```
- **Example Response**:
  ```json
  {
    "id": "6b3eb7a8-e3f5-42ee-82bd-a7df4f3f28a3",
    "email": "admin@vendor.com",
    "fullName": "Admin User",
    "businessName": "Vendor Co",
    "businessType": "Services",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiresIn": 86400
  }
  ```

#### Register

- **Method**: `POST`
- **URL**: `/Auth/register`
- **Description**: Registers a new vendor account.
- **Authentication**: Not required
- **Request Body**:
  ```json
  {
    "email": "string",         // Vendor's email
    "password": "string",      // Vendor's password
    "fullName": "string",      // Vendor's full name
    "businessName": "string",  // Business name
    "businessType": "string",  // Business type
    "taxNumber": "string",     // Tax identification number
    "subCategoryIds": [number] // List of subcategory IDs associated with the vendor
  }
  ```
- **Response**: 
  - **201 Created**: Vendor registered successfully.
  - **400 Bad Request**: Invalid data or duplicate email.
- **Example Request**:
  ```bash
  curl -X POST {{host}}/Auth/register \
  -H "Content-Type: application/json" \
  -d '{"email": "foodvendor@service.com", "password": "P@ssword123", "fullName": "Sarah Chef", "businessName": "Tasty Bites Catering", "businessType": "Food Services", "taxNumber": "TAX-9876", "subCategoryIds": [1, 4]}'
  ```

---

### Carts

#### Get Cart

- **Method**: `GET`
- **URL**: `/api/Carts/{userId}`
- **Description**: Retrieves the cart details for a specific user.
- **Authentication**: Required (Bearer token)
- **Path Parameters**:
  - `userId` (string): The unique identifier of the user.
- **Response** (200 OK):
  ```json
  {
    "id": "number",          // Cart ID
    "items": [              // List of cart items
      {
        "productId": "number", // Product ID
        "nameEn": "string",    // Product name in English
        "nameAr": "string",    // Product name in Arabic
        "price": "number",     // Product price
        "quantity": "number"   // Quantity of the product
      }
    ]
  }
  ```
- **Example Request**:
  ```bash
  curl -X GET {{host}}/api/Carts/ahmed123 \
  -H "Authorization: Bearer <token>"
  ```
- **Example Response**:
  ```json
  {
    "id": 101,
    "items": [
      {
        "productId": 1,
        "nameEn": "Cheeseburger",
        "nameAr": "تشيز برجر",
        "price": 5.99,
        "quantity": 2
      },
      {
        "productId": 2,
        "nameEn": "Large Fries",
        "nameAr": "بطاطس مقلية كبيرة",
        "price": 3.50,
        "quantity": 1
      }
    ]
  }
  ```

#### Add Cart Item

- **Method**: `POST`
- **URL**: `/api/Carts/items`
- **Description**: Adds an item to the user's cart.
- **Authentication**: Required (Bearer token)
- **Request Body**:
  ```json
  {
    "userId": "string",    // User's unique identifier
    "productId": "number", // Product ID to add
    "quantity": "number"   // Quantity to add
  }
  ```
- **Response** (200 OK):
  ```json
  {
    "cartId": "number",    // Cart ID
    "productId": "number", // Added product ID
    "quantity": "number"   // Added quantity
  }
  ```
- **Example Request**:
  ```bash
  curl -X POST {{host}}/api/Carts/items \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{"userId": "ahmed123", "productId": 5, "quantity": 2}'
  ```
- **Example Response**:
  ```json
  {
    "cartId": 1,
    "productId": 5,
    "quantity": 2
  }
  ```

#### Update Cart Quantity

- **Method**: `PUT`
- **URL**: `/api/Carts/{userId}/items`
- **Description**: Updates the quantity of an item in the user's cart.
- **Authentication**: Required (Bearer token)
- **Path Parameters**:
  - `userId` (string): The unique identifier of the user.
- **Request Body**:
  ```json
  {
    "productId": "number", // Product ID to update
    "quantity": "number"   // New quantity
  }
  ```
- **Response** (200 OK):
  ```json
  {
    "cartId": "number",    // Cart ID
    "productId": "number", // Updated product ID
    "quantity": "number"   // Updated quantity
  }
  ```
- **Example Request**:
  ```bash
  curl -X PUT {{host}}/api/Carts/ahmed123/items \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{"productId": 5, "quantity": 3}'
  ```
- **Example Response**:
  ```json
  {
    "cartId": 1,
    "productId": 5,
    "quantity": 3
  }
  ```

---

### Categories

#### Get All Categories

- **Method**: `GET`
- **URL**: `/api/Categories`
- **Description**: Retrieves a list of all categories.
- **Authentication**: Not required
- **Response** (200 OK):
  ```json
  [
    {
      "id": "number",    // Category ID
      "nameEn": "string", // Category name in English
      "nameAr": "string"  // Category name in Arabic
    }
  ]
  ```
- **Example Request**:
  ```bash
  curl -X GET {{host}}/api/Categories
  ```
- **Example Response**:
  ```json
  [
    {
      "id": 1,
      "nameEn": "Food",
      "nameAr": "طعام"
    },
    {
      "id": 2,
      "nameEn": "Electronics",
      "nameAr": "إلكترونيات"
    }
  ]
  ```

#### Add Category

- **Method**: `POST`
- **URL**: `/api/Categories`
- **Description**: Adds a new category (admin only).
- **Authentication**: Required (Admin role)
- **Request Body**:
  ```json
  {
    "nameEn": "string", // Category name in English
    "nameAr": "string"  // Category name in Arabic
  }
  ```
- **Response**: 
  - **201 Created**: Category added successfully.
  - **403 Forbidden**: Insufficient permissions.
- **Example Request**:
  ```bash
  curl -X POST {{host}}/api/Categories \
  -H "Authorization: Bearer <admin-token>" \
  -H "Content-Type: application/json" \
  -d '{"nameEn": "Services", "nameAr": "خدمات"}'
  ```

#### Get Providers Under Category

- **Method**: `GET`
- **URL**: `/api/Categories/{categoryId}/providers`
- **Description**: Retrieves a list of providers (vendors) under a specific category.
- **Authentication**: Not required
- **Path Parameters**:
  - `categoryId` (number): The ID of the category.
- **Response** (200 OK): List of vendors (exact schema depends on DTO; assumed structure):
  ```json
  [
    {
      "id": "string",
      "email":"string",   
      "businessName": "string",
      "businessType": "string",
      "taxNumber":"string" 
    }
  ]
  ```
- **Example Request**:
  ```bash
  curl -X GET {{host}}/api/Categories/1/providers
  ```

#### Get SubCategories Under Category

- **Method**: `GET`
- **URL**: `/api/Categories/{categoryId}/subCategories`
- **Description**: Retrieves a list of subcategories under a specific category.
- **Authentication**: Not required
- **Path Parameters**:
  - `categoryId` (number): The ID of the category.
- **Response** (200 OK):
  ```json
  [
    {
      "id": "number",    // Subcategory ID
      "nameEn": "string", // Subcategory name in English
      "nameAr": "string"  // Subcategory name in Arabic
    }
  ]
  ```
- **Example Request**:
  ```bash
  curl -X GET {{host}}/api/Categories/1/subCategories
  ```

#### Add SubCategory Under Category

- **Method**: `POST`
- **URL**: `/api/Categories/{categoryId}/subCategories`
- **Description**: Adds a new subcategory under a specific category (admin only).
- **Authentication**: Required (Admin role)
- **Path Parameters**:
  - `categoryId` (number): The ID of the category.
- **Request Body**:
  ```json
  {
    "nameEn": "string", // Subcategory name in English
    "nameAr": "string"  // Subcategory name in Arabic
  }
  ```
- **Response**: 
  - **201 Created**: Subcategory added successfully.
  - **403 Forbidden**: Insufficient permissions.
- **Example Request**:
  ```bash
  curl -X POST {{host}}/api/Categories/1/subCategories \
  -H "Authorization: Bearer <admin-token>" \
  -H "Content-Type: application/json" \
  -d '{"nameEn": "Fast Food", "nameAr": "طعام سريع"}'
  ```

#### Delete Subcategory

- **Method**: `DELETE`
- **URL**: `/api/Categories/subcategories/{subCategoryId}`
- **Description**: Deletes a specific subcategory (admin only).
- **Authentication**: Required (Admin role)
- **Path Parameters**:
  - `subCategoryId` (number): The ID of the subcategory.
- **Response**: 
  - **204 No Content**: Subcategory deleted successfully.
  - **403 Forbidden**: Insufficient permissions.
- **Example Request**:
  ```bash
  curl -X DELETE {{host}}/api/Categories/subcategories/1 \
  -H "Authorization: Bearer <admin-token>"
  ```

---

### Orders

#### Get Order

- **Method**: `GET`
- **URL**: `/api/Orders/{orderId}`
- **Description**: Retrieves details of a specific order.
- **Authentication**: Required (Bearer token)
- **Path Parameters**:
  - `orderId` (number): The ID of the order.
- **Response** (200 OK): (Assumed schema based on typical order structure):
  ```json
  {
    "id": "number",        // Order ID
    "userId": "string",    // User ID
    "items": [            // List of order items
      {
        "productId": "number", // Product ID
        "quantity": "number",  // Quantity
        "price": "number"      // Price per unit
      }
    ],
    "total": "number",     // Total order amount
    "status": "string"     // Order status (e.g., "Pending", "Completed")
  }
  ```
- **Example Request**:
  ```bash
  curl -X GET {{host}}/api/Orders/1 \
  -H "Authorization: Bearer <token>"
  ```

#### Get User Orders

- **Method**: `GET`
- **URL**: `/api/Orders/Users/{userId}`
- **Description**: Retrieves all orders for a specific user.
- **Authentication**: Required (Bearer token)
- **Path Parameters**:
  - `userId` (string): The unique identifier of the user.
- **Response** (200 OK): List of orders (same schema as "Get Order").
- **Example Request**:
  ```bash
  curl -X GET {{host}}/api/Orders/Users/ahmed123 \
  -H "Authorization: Bearer <token>"
  ```

#### Make An Order

- **Method**: `POST`
- **URL**: `/api/Orders`
- **Description**: Creates a new order from the user's cart.
- **Authentication**: Required (Bearer token)
- **Request Body**:
  ```json
  {
    "userId": "string",      // User's unique identifier
    "paymentMethod": "string" // Payment method (e.g., "CreditCard", "Cash")
  }
  ```
- **Response**: 
  - **201 Created**: Order created successfully.
  - **400 Bad Request**: Invalid data or empty cart.
- **Example Request**:
  ```bash
  curl -X POST {{host}}/api/Orders \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{"userId": "ahmed123", "paymentMethod": "CreditCard"}'
  ```

#### Update Order Status

- **Method**: `PUT`
- **URL**: `/api/Orders/{orderId}/status`
- **Description**: Updates the status of an order (vendor or admin only).
- **Authentication**: Required (Vendor or Admin role)
- **Path Parameters**:
  - `orderId` (number): The ID of the order.
- **Request Body**:
  ```json
  {
    "newStatus": "string" // New status (e.g., "Shipped", "Delivered")
  }
  ```
- **Response**: 
  - **200 OK**: Status updated successfully.
  - **403 Forbidden**: Insufficient permissions.
- **Example Request**:
  ```bash
  curl -X PUT {{host}}/api/Orders/1/status \
  -H "Authorization: Bearer <vendor-token>" \
  -H "Content-Type: application/json" \
  -d '{"newStatus": "Shipped"}'
  ```

---

### Products

#### Get All Products

- **Method**: `GET`
- **URL**: `/api/Products`
- **Description**: Retrieves a list of all products.
- **Authentication**: Not required
- **Response** (200 OK): (Assumed schema):
  ```json
  [
    {
      "id": "number",        // Product ID
      "nameEn": "string",    // Name in English
      "nameAr": "string",    // Name in Arabic
      "description": "string", // Product description
      "price": "number",     // Price
      "subCategoryId": "number" // Subcategory ID
    }
  ]
  ```
- **Example Request**:
  ```bash
  curl -X GET {{host}}/api/Products
  ```

#### Get Specific Product

- **Method**: `GET`
- **URL**: `/api/Products/{productId}`
- **Description**: Retrieves details of a specific product.
- **Authentication**: Not required
- **Path Parameters**:
  - `productId` (number): The ID of the product.
- **Response** (200 OK): Single product (same schema as "Get All Products").
- **Example Request**:
  ```bash
  curl -X GET {{host}}/api/Products/2
  ```

#### Add Product

- **Method**: `POST`
- **URL**: `/api/Products`
- **Description**: Adds a new product (vendor only).
- **Authentication**: Required (Vendor role)
- **Request Body**:
  ```json
  {
    "nameEn": "string",      // Name in English
    "nameAr": "string",      // Name in Arabic
    "description": "string", // Product description
    "imageUrl": "string",    // URL to product image
    "price": "number",       // Price
    "subCategoryId": "number" // Subcategory ID
  }
  ```
- **Response**: 
  - **201 Created**: Product added successfully.
  - **403 Forbidden**: Insufficient permissions.
- **Example Request**:
  ```bash
  curl -X POST {{host}}/api/Products \
  -H "Authorization: Bearer <vendor-token>" \
  -H "Content-Type: application/json" \
  -d '{"nameEn": "Home Electrical Repair", "nameAr": "إصلاح الكهرباء المنزلية", "description": "Professional electrical repair service", "imageUrl": "https://example.com/images/electrical-repair.jpg", "price": 100, "subCategoryId": 32}'
  ```

#### Update An Product

- **Method**: `PUT`
- **URL**: `/api/Products/{productId}`
- **Description**: Updates a product (vendor only).
- **Authentication**: Required (Vendor role)
- **Path Parameters**:
  - `productId` (number): The ID of the product.
- **Request Body**:
  ```json
  {
    "nameEn": "string",      // Name in English
    "nameAr": "string",      // Name in Arabic
    "description": "string", // Product description
    "price": "number"        // Price
  }
  ```
- **Response**: 
  - **200 OK**: Product updated successfully.
  - **403 Forbidden**: Insufficient permissions.
- **Example Request**:
  ```bash
  curl -X PUT {{host}}/api/Products/2 \
  -H "Authorization: Bearer <vendor-token>" \
  -H "Content-Type: application/json" \
  -d '{"nameEn": "Classic Cheeseburger", "nameAr": "تشيز برجر كلاسيك", "description": "Very Good Classic Cheeseburger", "price": 50.00}'
  ```

#### Delete Product

- **Method**: `DELETE`
- **URL**: `/api/Products/{productId}`
- **Description**: Deletes a product (vendor only).
- **Authentication**: Required (Vendor role)
- **Path Parameters**:
  - `productId` (number): The ID of the product.
- **Response**: 
  - **204 No Content**: Product deleted successfully.
  - **403 Forbidden**: Insufficient permissions.
- **Example Request**:
  ```bash
  curl -X DELETE {{host}}/api/Products/2 \
  -H "Authorization: Bearer <vendor-token>"
  ```

#### Get Most Requested Product

- **Method**: `GET`
- **URL**: `/api/Products/most-requested`
- **Description**: Retrieves a list of the most requested products.
- **Authentication**: Not required
- **Response** (200 OK): List of products (same schema as "Get All Products").
- **Example Request**:
  ```bash
  curl -X GET {{host}}/api/Products/most-requested
  ```

#### Get Most Recent Products

- **Method**: `GET`
- **URL**: `/api/Products/most-recent`
- **Description**: Retrieves a list of the most recently added products.
- **Authentication**: Not required
- **Response** (200 OK): List of products (same schema as "Get All Products").
- **Example Request**:
  ```bash
  curl -X GET {{host}}/api/Products/most-recent
  ```

#### Get Product Reviews

- **Method**: `GET`
- **URL**: `/api/Products/{productId}/reviews`
- **Description**: Retrieves reviews for a specific product.
- **Authentication**: Not required
- **Path Parameters**:
  - `productId` (number): The ID of the product.
- **Response** (200 OK): (Assumed schema):
  ```json
  [
    {
      "id": "number",     // Review ID
      "userId": "string", // User ID
      "rating": "number", // Rating (e.g., 1-5)
      "comment": "string" // Review comment
    }
  ]
  ```
- **Example Request**:
  ```bash
  curl -X GET {{host}}/api/Products/2/reviews
  ```

#### Make Review for a Product

- **Method**: `POST`
- **URL**: `/api/Products/{productId}/reviews`
- **Description**: Adds a review for a product.
- **Authentication**: Required (Bearer token)
- **Path Parameters**:
  - `productId` (number): The ID of the product.
- **Request Body**:
  ```json
  {
    "userId": "string",  // User's unique identifier
    "rating": "number",  // Rating (e.g., 1-5)
    "comment": "string"  // Review comment
  }
  ```
- **Response**: 
  - **201 Created**: Review added successfully.
- **Example Request**:
  ```bash
  curl -X POST {{host}}/api/Products/2/reviews \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{"userId": "ahmed123", "rating": 4, "comment": "Great product!"}'
  ```

---

### Reviews

#### Edit Review

- **Method**: `PUT`
- **URL**: `/api/Reviews/{reviewId}`
- **Description**: Edits an existing review.
- **Authentication**: Required (Bearer token, user must own the review)
- **Path Parameters**:
  - `reviewId` (number): The ID of the review.
- **Request Body**:
  ```json
  {
    "userId": "string",  // User's unique identifier
    "rating": "number",  // Updated rating
    "comment": "string"  // Updated comment
  }
  ```
- **Response**: 
  - **200 OK**: Review updated successfully.
  - **403 Forbidden**: User does not own the review.
- **Example Request**:
  ```bash
  curl -X PUT {{host}}/api/Reviews/1 \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{"userId": "ahmed123", "rating": 3, "comment": "Not bad"}'
  ```

#### Delete Review

- **Method**: `DELETE`
- **URL**: `/api/Reviews/{reviewId}`
- **Description**: Deletes a review.
- **Authentication**: Required (Bearer token, user must own the review)
- **Path Parameters**:
  - `reviewId` (number): The ID of the review.
- **Response**: 
  - **204 No Content**: Review deleted successfully.
  - **403 Forbidden**: User does not own the review.
- **Example Request**:
  ```bash
  curl -X DELETE {{host}}/api/Reviews/1 \
  -H "Authorization: Bearer <token>"
  ```

#### Get Vendor Reviews

- **Method**: `GET`
- **URL**: `/api/Reviews/{vendorId}/vendor-reviews`
- **Description**: Retrieves paginated reviews for a specific vendor.
- **Authentication**: Required (Bearer token)
- **Path Parameters**:
  - `vendorId` (string): The ID of the vendor.
- **Query Parameters**:
  - `pageNumer` (number): Page number (default: 1)
  - `pageSize` (number): Items per page (default: 10)
- **Response** (200 OK): (Assumed paginated schema):
  ```json
  {
    "items": [
      {
        "id": "number",     // Review ID
        "userId": "string", // User ID
        "rating": "number", // Rating
        "comment": "string" // Comment
      }
    ],
    "totalCount": "number", // Total number of reviews
    "pageNumber": "number", // Current page
    "pageSize": "number"    // Items per page
  }
  ```
- **Example Request**:
  ```bash
  curl -X GET "{{host}}/api/Reviews/vendor1/vendor-reviews?pageNumer=1&pageSize=2" \
  -H "Authorization: Bearer <token>"
  ```

---

### SubCategories

#### Get All SubCategories

- **Method**: `GET`
- **URL**: `/api/SubCategory`
- **Description**: Retrieves a list of all subcategories.
- **Authentication**: Not required
- **Response** (200 OK): (Assumed schema):
  ```json
  [
    {
      "id": "number",    // Subcategory ID
      "nameEn": "string", // Name in English
      "nameAr": "string"  // Name in Arabic
    }
  ]
  ```
- **Example Request**:
  ```bash
  curl -X GET {{host}}/api/SubCategory
  ```

---

### Vendors

#### Approve Vendor

- **Method**: `POST`
- **URL**: `/api/Vendors/approve-vendor/{vendorId}`
- **Description**: Approves a pending vendor (admin only).
- **Authentication**: Required (Admin role)
- **Path Parameters**:
  - `vendorId` (string): The ID of the vendor.
- **Response**: 
  - **200 OK**: Vendor approved successfully.
  - **403 Forbidden**: Insufficient permissions.
- **Example Request**:
  ```bash
  curl -X POST {{host}}/api/Vendors/approve-vendor/c8e3ccdc-e4f2-4a7f-a1d9-b8ff5051e59a \
  -H "Authorization: Bearer <admin-token>"
  ```

#### Get All Vendors

- **Method**: `GET`
- **URL**: `/api/vendors`
- **Description**: Retrieves a list of all vendors.
- **Authentication**: Not required
- **Response** (200 OK): (Assumed schema):
  ```json
  [
    {
      "id": "string",        // Vendor ID
      "businessName": "string", // Business name
      "businessType": "string"  // Business type
    }
  ]
  ```
- **Example Request**:
  ```bash
  curl -X GET {{host}}/api/vendors
  ```

#### Get Specific Vendor

- **Method**: `GET`
- **URL**: `/api/vendors/{vendorId}`
- **Description**: Retrieves details of a specific vendor.
- **Authentication**: Not required
- **Path Parameters**:
  - `vendorId` (string): The ID of the vendor.
- **Response** (200 OK): Single vendor (same schema as "Get All Vendors").
- **Example Request**:
  ```bash
  curl -X GET {{host}}/api/vendors/vendor1
  ```

#### Get Vendor's Menu

- **Method**: `GET`
- **URL**: `/api/vendors/{vendorId}/menu`
- **Description**: Retrieves the menu (list of products) of a specific vendor.
- **Authentication**: Not required
- **Path Parameters**:
  - `vendorId` (string): The ID of the vendor.
- **Response** (200 OK): List of products (same schema as "Get All Products").
- **Example Request**:
  ```bash
  curl -X GET {{host}}/api/vendors/vendor1/menu
  ```

#### Get Pending Vendors

- **Method**: `GET`
- **URL**: `/api/Vendors/pending DIY**: Retrieves a list of pending (unapproved) vendors.
- **Authentication**: Required (Admin role)
- **Response** (200 OK): List of vendors (same schema as "Get All Vendors").
- **Example Request**:
  ```bash
  curl -X GET {{host}}/api/Vendors/pending-vendors \
  -H "Authorization: Bearer <admin-token>"
  ```

#### Change Password

- **Method**: `PUT`
- **URL**: `/api/Vendors/change-password`
- **Description**: Changes the authenticated vendor's password.
- **Authentication**: Required (Vendor role)
- **Request Body**:
  ```json
  {
    "currentPassword": "string", // Current password
    "newPassword": "string"     // New password
  }
  ```
- **Response**: 
  - **200 OK**: Password changed successfully.
  - **401 Unauthorized**: Current password incorrect.
- **Example Request**:
  ```bash
  curl -X PUT {{host}}/api/Vendors/change-password \
  -H "Authorization: Bearer <vendor-token>" \
  -H "Content-Type: application/json" \
  -d '{"currentPassword": "P@ssword123", "newPassword": "P@ssword1234"}'
  ```

#### Get Vendors Rating

- **Method**: `GET`
- **URL**: `/api/vendors/vendors-rating`
- **Description**: Retrieves paginated ratings of vendors.
- **Authentication**: Required (Admin role)
- **Query Parameters**:
  - `pageNumer` (number): Page number (default: 1)
  - `pageSize` (number): Items per page (default: 10)
- **Response** (200 OK): (Assumed schema):
  ```json
  {
    "items": [
      {
        "vendorId": "string",   // Vendor ID
        "businessName": "string", // Business name
        "averageRating": "number" // Average rating
      }
    ],
    "totalCount": "number",
    "pageNumber": "number",
    "pageSize": "number"
  }
  ```
- **Example Request**:
  ```bash
  curl -X GET "{{host}}/api/vendors/vendors-rating?pageNumer=1&pageSize=2" \
  -H "Authorization: Bearer <admin-token>"
  ```

---

### Admin

#### Today Stats

- **Method**: `GET`
- **URL**: `/api/Admin/today-stats`
- **Description**: Retrieves statistics for the current day.
- **Authentication**: Required (Admin role)
- **Response** (200 OK): (Assumed schema):
  ```json
  {
    "orders": "number",    // Number of orders today
    "revenue": "number",   // Total revenue today
    "newUsers": "number"   // New users registered today
  }
  ```
- **Example Request**:
  ```bash
  curl -X GET {{host}}/api/Admin/today-stats \
  -H "Authorization: Bearer <admin-token>"
  ```

#### Get Top 15 Vendors By Revenue

- **Method**: `GET`
- **URL**: `/api/Admin/top-vendors`
- **Description**: Retrieves the top 15 vendors by revenue.
- **Authentication**: Required (Admin role)
- **Response** (200 OK): List of vendors with revenue (assumed schema):
  ```json
  [
    {
      "vendorId": "string",   // Vendor ID
      "businessName": "string", // Business name
      "revenue": "number"     // Total revenue
    }
  ]
  ```
- **Example Request**:
  ```bash
  curl -X GET {{host}}/api/Admin/top-vendors \
  -H "Authorization: Bearer <admin-token>"
  ```

#### Get Overall Stats

- **Method**: `GET`
- **URL**: `/api/Admin/project-summary`
- **Description**: Retrieves overall project statistics.
- **Authentication**: Required (Admin role)
- **Response** (200 OK): (Assumed schema):
  ```json
  {
    "totalOrders": "number",   // Total number of orders
    "totalRevenue": "number",  // Total revenue
    "totalUsers": "number",    // Total users
    "totalVendors": "number"   // Total vendors
  }
  ```
- **Example Request**:
  ```bash
  curl -X GET {{host}}/api/Admin/project-summary \
  -H "Authorization: Bearer <admin-token>"
  ```

#### Get All Users

- **Method**: `GET`
- **URL**: `/api/Admin/all-users`
- **Description**: Retrieves a list of all users.
- **Authentication**: Required (Admin role)
- **Response** (200 OK): (Assumed schema):
  ```json
  [
    {
      "id": "string",    // User ID
      "email": "string", // User email
      "fullName": "string" // User full name
    }
  ]
  ```
- **Example Request**:
  ```bash
  curl -X GET {{host}}/api/Admin/all-users \
  -H "Authorization: Bearer <admin-token>"
  ```

#### Get All Transactions

- **Method**: `GET`
- **URL**: `/api/Admin/all-transactions`
- **Description**: Retrieves a list of all transactions.
- **Authentication**: Required (Admin role)
- **Response** (200 OK): (Assumed schema):
  ```json
  [
    {
      "id": "number",      // Transaction ID
      "orderId": "number", // Associated order ID
      "amount": "number",  // Transaction amount
      "date": "string"     // Transaction date (ISO 8601 format)
    }
  ]
  ```
- **Example Request**:
  ```bash
  curl -X GET {{host}}/api/Admin/all-transactions \
  -H "Authorization: Bearer <admin-token>"
  ```

#### Get All Transactions of Specific User

- **Method**: `GET`
- **URL**: `/api/Admin/users/{userId}/all-transactions`
- **Description**: Retrieves all transactions for a specific user.
- **Authentication**: Required (Admin role)
- **Path Parameters**:
  - `userId` (string): The unique identifier of the user.
- **Response** (200 OK): List of transactions (same schema as "Get All Transactions").
- **Example Request**:
  ```bash
  curl -X GET {{host}}/api/Admin/users/ahmed123/all-transactions \
  -H "Authorization: Bearer <admin-token>"
  ```

---

## Notes

- **Pagination**: Endpoints like "Get Vendor Reviews" and "Get Vendors Rating" support pagination using `pageNumer` and `pageSize` query parameters.
- **DTOs**: Response schemas are inferred from the Postman collection and typical ASP.NET Core practices. For precise schemas, refer to the DTO classes in the GitHub repository (`https://github.com/AmirMawla/Service-Provider-`) under the `Models` or `DTOs` folder.
- **Role-Based Access**: Endpoints requiring "Admin" or "Vendor" roles check the `roles` claim in the JWT token.

This documentation provides a comprehensive guide to interacting with the Service Provider API. For additional details or to contribute to the project, visit the repository linked above.
