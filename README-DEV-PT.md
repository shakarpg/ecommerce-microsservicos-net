# Documentação Técnica - Microserviços de E-commerce 🇧🇷

Este documento detalha as escolhas técnicas e a implementação do sistema.

## Tecnologias
- .NET Core (C#)
- Entity Framework
- RabbitMQ
- JWT
- API Gateway (Ocelot)

## Microserviços
1. **Gestão de Estoque**
   - Cadastro de produtos (CRUD)
   - Consulta e atualização de estoque
   - Comunicação com vendas via RabbitMQ

2. **Gestão de Vendas**
   - Criação de pedidos com validação de estoque
   - Consulta de pedidos
   - Notificação de estoque

## Segurança
- Autenticação JWT
- Controle de acesso por roles

## Boas Práticas
- Padrão Repository
- DTOs para comunicação
- Testes unitários em serviços críticos

