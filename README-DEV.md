# 🛠️ E-commerce Microservices — Detalhes Técnicos  

## 🏗️ Arquitetura  

- **API Gateway (Ocelot)** → Roteamento centralizado + autenticação JWT  
- **Inventory.API** → Gestão de produtos e estoque  
- **Sales.API** → Criação e gerenciamento de pedidos  
- **RabbitMQ** → Comunicação assíncrona entre serviços  
- **SQL Server** → Persistência de dados com EF Core  
- **Docker Compose** → Orquestração dos serviços  

### Fluxo de Negócio
1. Pedido criado em **Sales.API** → valida estoque via HTTP (sincronia).  
2. Pedido confirmado → evento publicado no **RabbitMQ**.  
3. **Inventory.API** consome o evento e reduz o estoque (assíncrono).  

---

## 🚀 Como Executar  

1. Clone o repositório:  
   ```bash
   git clone https://github.com/seu-usuario/ecommerce-microservices.git
   cd ecommerce-microservices
   ```

2. Suba os containers:  
   ```bash
   docker compose up -d --build
   ```

3. Gere um token JWT:  
   ```bash
   curl -X POST http://localhost:8080/sales/auth/token      -H "Content-Type: application/json"      -d '{"username":"admin","password":"admin"}'
   ```

4. Use o token para chamar os endpoints via Gateway:  
   - `POST /inventory/products` → cadastra produto  
   - `POST /sales/orders` → cria pedido  
   - `GET /sales/orders` → lista pedidos  

> Painel RabbitMQ: [http://localhost:15672](http://localhost:15672) (guest/guest)  

---

## 📂 Estrutura do Repositório  

```
.
├── gateway/             # API Gateway (Ocelot)
├── inventory.api/       # Microsserviço de Estoque
├── sales.api/           # Microsserviço de Vendas
├── docker-compose.yml   # Orquestração completa
├── README.md            # Visão geral (Recrutadores)
└── README-DEV.md        # Detalhes técnicos (Devs)
```

---

## ✅ Testes Automatizados  

Exemplo com **xUnit**:  
- Criação de produto  
- Status inicial de pedido  
- Validação de estoque  

Rodar testes localmente:  
```bash
dotnet test
```

---

## 🔮 Possíveis Melhorias  

- Outbox Pattern para garantir entrega de eventos  
- Observabilidade (OpenTelemetry, Prometheus, Grafana)  
- Serviço de autenticação dedicado (ex: IdentityServer, Keycloak)  
- CI/CD pipeline para build e deploy automatizados  
