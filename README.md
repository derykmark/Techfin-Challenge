# 💳 TechfinChallenge API

API REST desenvolvida como case técnico para a vaga de **Analista de Desenvolvimento de Software** na Techfin.

## 📋 Sobre o Projeto

Sistema composto por 3 APIs REST para gerenciamento de autenticação, clientes e simulação de transações financeiras, implementado com **Clean Architecture** e **Domain-Driven Design (DDD)**.

## 🏗️ Arquitetura

```
┌─────────────────────────────────────────────────────────┐
│                    API (Presentation)                    │
│           Controllers · Middlewares · Swagger            │
├─────────────────────────────────────────────────────────┤
│                     Application                          │
│          Use Cases · DTOs · Validators                   │
├─────────────────────────────────────────────────────────┤
│                   Domain (Core)                          │
│    Entities · Value Objects · Events · Interfaces         │
├─────────────────────────────────────────────────────────┤
│                    Infrastructure                        │
│   Dapper · SQLite · JWT · BCrypt · MassTransit · Cache   │
└─────────────────────────────────────────────────────────┘
```

**Regra de dependência:** as camadas externas dependem das internas. O Domain não depende de nenhuma outra camada.

## 🛠️ Tecnologias Utilizadas

| Tecnologia | Propósito |
|------------|-----------|
| **.NET 10 / C#** | Stack principal |
| **Dapper** | Micro-ORM para acesso a dados |
| **SQLite In-Memory** | Banco de dados em memória |
| **JWT Bearer** | Autenticação e autorização |
| **BCrypt** | Hash de senhas |
| **MassTransit + RabbitMQ** | Mensageria assíncrona |
| **IMemoryCache** | Cache em memória (10 min) |
| **FluentValidation** | Validação de requests |
| **Swagger / Swashbuckle** | Documentação da API |
| **xUnit + Moq + FluentAssertions** | Testes automatizados |

## 📁 Estrutura de Projetos

```
TechfinChallenge/
├── src/
│   ├── TechfinChallenge.Domain/            # Entidades, Value Objects, Interfaces
│   ├── TechfinChallenge.Application/       # Use Cases, DTOs, Validators
│   ├── TechfinChallenge.Infrastructure/    # Repositórios, JWT, RabbitMQ, Cache
│   └── TechfinChallenge.API/               # Controllers, Middlewares, Program.cs
├── tests/
│   ├── TechfinChallenge.UnitTests/         # Testes unitários (21 testes)
│   └── TechfinChallenge.IntegrationTests/  # Testes de integração
└── TechfinChallenge.sln
```

## ⚙️ Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [RabbitMQ](https://www.rabbitmq.com/) (opcional — necessário apenas para mensageria)

### Subindo o RabbitMQ com Docker (opcional)

```bash
docker run -d --name rabbitmq \
  -p 5672:5672 \
  -p 15672:15672 \
  rabbitmq:management
```

> Painel de gerenciamento: http://localhost:15672 (guest/guest)

## 🚀 Como Executar

```bash
# Clonar o repositório
git clone https://github.com/seu-usuario/TechfinChallenge.git
cd TechfinChallenge

# Restaurar dependências
dotnet restore

# Compilar
dotnet build

# Executar a API
dotnet run --project src/TechfinChallenge.API
```

A API estará disponível em:
- **Swagger UI:** https://localhost:5001/swagger
- **HTTP:** http://localhost:5000

## 🧪 Executar Testes

```bash
# Todos os testes
dotnet test

# Apenas testes unitários
dotnet test tests/TechfinChallenge.UnitTests

# Com detalhes
dotnet test --verbosity normal
```

## 📡 Endpoints

### 🔐 Autenticação (`/api/auth`)

#### Registrar Usuário
```http
POST /api/auth/registrar
Content-Type: application/json

{
  "email": "usuario@email.com",
  "senha": "senha123"
}
```
**Response:** `201 Created`

#### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "usuario@email.com",
  "senha": "senha123"
}
```
**Response:** `200 OK`
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "expiracao": "2024-01-01T02:00:00Z"
}
```

---

### 👤 Clientes (`/api/clientes`) — *Requer JWT*

#### Cadastrar Cliente
```http
POST /api/clientes
Authorization: Bearer {token}
Content-Type: application/json

{
  "nome": "João Silva",
  "cpf": "52998224725",
  "valorLimite": 5000.00
}
```
**Response (Sucesso):** `201 Created`
```json
{
  "idCliente": "9cf0111b-c8c1-4d51-857f-0135650ad586",
  "status": "OK"
}
```
**Response (Erro):** `400 Bad Request`
```json
{
  "status": "ERRO",
  "detalheErro": "Já existe um cliente cadastrado com o CPF 52998224725."
}
```

#### Listar Clientes (com cache de 10 min)
```http
GET /api/clientes
Authorization: Bearer {token}
```
**Response:** `200 OK`
```json
[
  {
    "idCliente": "9cf0111b-c8c1-4d51-857f-0135650ad586",
    "nome": "João Silva",
    "cpf": "52998224725",
    "valorLimite": 5000.00
  }
]
```

---

### 💳 Transações (`/api/transacoes`) — *Requer JWT*

#### Simular Autorização de Transação
```http
POST /api/transacoes
Authorization: Bearer {token}
Content-Type: application/json

{
  "idCliente": "9cf0111b-c8c1-4d51-857f-0135650ad586",
  "valorSimulacao": 1500.00
}
```
**Response (Aprovado):** `200 OK`
```json
{
  "status": "APROVADO",
  "idTransacao": "a1b2c3d4-e5f6-7890-abcd-ef1234567890"
}
```
**Response (Negado):** `200 OK`
```json
{
  "status": "NEGADO"
}
```

## 🔄 Fluxo de Mensageria (RabbitMQ)

```
Transação Aprovada → API de Transação publica evento
                          ↓
                     RabbitMQ (MassTransit)
                          ↓
              TransacaoAutorizadaConsumer
                          ↓
           AtualizarLimiteClienteUseCase
                          ↓
              Débito do limite do cliente
```

Quando uma transação é aprovada, a API de Transação publica um evento `TransacaoAutorizadaEvent` no RabbitMQ. O `TransacaoAutorizadaConsumer` consome o evento e atualiza o limite do cliente, garantindo a comunicação assíncrona entre os contextos.

## 📐 Regras de Negócio

### Clientes
- ✅ Clientes duplicados (mesmo CPF) não podem ser cadastrados
- ✅ ID gerado automaticamente (GUID)
- ✅ Valor de limite não pode ser negativo
- ✅ Validação completa de CPF (dígitos verificadores)
- ✅ Cache de 10 minutos na listagem

### Transações
- ✅ Só autoriza para clientes cadastrados
- ✅ Valor não pode exceder o limite do cliente
- ✅ Transação aprovada gera GUID como identificador
- ✅ Débito do limite via mensageria (RabbitMQ)

### Autenticação
- ✅ Cadastro com e-mail e senha
- ✅ Senha armazenada com hash BCrypt
- ✅ Token JWT com expiração de 1 hora
- ✅ Token válido nas APIs de Clientes e Transações

## 🧪 Cobertura de Testes

| Camada | Testes | Cobertura |
|--------|--------|-----------|
| **Domain** | 15 | Entidades (Cliente, Transacao), Value Objects (CPF) |
| **Application** | 5 | Use Cases (AutorizarTransacao, CadastrarCliente) |
| **Integração** | 2 | Controllers (Auth) |
| **Total** | **21+** | |

### Exemplos de cenários testados:
- Criar cliente com dados válidos / inválidos
- Debitar limite com valor suficiente / insuficiente
- Validar CPF válido / inválido / repetido
- Autorizar transação com limite suficiente / insuficiente / cliente inexistente
- Cadastrar cliente duplicado (mesmo CPF)

## 📝 Decisões Técnicas

| Decisão | Justificativa |
|---------|---------------|
| **SQLite In-Memory** | Atende ao requisito de banco em memória + compatível com Dapper |
| **MassTransit** | Abstração robusta sobre RabbitMQ, facilita testes com `InMemoryTestHarness` |
| **BCrypt** | Algoritmo seguro e amplamente adotado para hash de senhas |
| **FluentValidation** | Validação declarativa separada dos controllers |
| **Middleware de exceção** | Tratamento centralizado de erros, retornando o formato JSON padronizado |

## 👤 Autor

Desenvolvido como case técnico para processo seletivo.

## 📄 Licença

Este projeto é confidencial e destinado exclusivamente para fins avaliativos no processo seletivo.
