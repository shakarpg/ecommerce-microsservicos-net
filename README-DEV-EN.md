# Technical Documentation - E-commerce Microservices 🇺🇸

This document provides the technical details and implementation decisions of the system.

## Technologies
- .NET Core (C#)
- Entity Framework
- RabbitMQ
- JWT
- API Gateway (Ocelot)

## Microservices
1. **Inventory Management**
   - Product registration (CRUD)
   - Stock queries and updates
   - Communication with Sales via RabbitMQ

2. **Sales Management**
   - Order creation with stock validation
   - Order tracking
   - Stock notification

## Security
- JWT Authentication
- Role-based access control

## Best Practices
- Repository Pattern
- DTOs for communication
- Unit tests for critical services

