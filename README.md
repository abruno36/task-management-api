# TaskManagement

API REST para gerenciamento de tarefas, desenvolvida como desafio técnico para o processo seletivo SADA.

A aplicação permite criar, consultar, atualizar, excluir, filtrar e pesquisar tarefas por meio de uma API HTTP documentada com Swagger/OpenAPI.

## Objetivo

O projeto foi desenvolvido com foco em:

- organização e separação de responsabilidades;
- aplicação de princípios SOLID;
- Dependency Injection;
- DTOs para os contratos da API;
- Entity Framework Core com banco InMemory;
- validação de entrada;
- tratamento global de exceções;
- logging;
- testes automatizados com xUnit;
- documentação da API com Swagger/OpenAPI.

## Tecnologias

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core 8
- EF Core InMemory
- Swagger / OpenAPI com Swashbuckle
- xUnit
- Microsoft.Extensions.Logging

## Arquitetura

A solução utiliza uma arquitetura simples em camadas, proporcional ao escopo do desafio:

```text
TaskManagement
│
├── src
│   ├── TaskManagement.Api
│   ├── TaskManagement.Application
│   ├── TaskManagement.Domain
│   └── TaskManagement.Infrastructure
│
└── tests
    └── TaskManagement.Tests
```

### Fluxo principal

```text
HTTP Request
     │
     ▼
┌──────────────────────┐
│         API          │
│    Controller        │
└──────────┬───────────┘
           │
           ▼
┌──────────────────────┐
│    Application       │
│ Service / DTOs       │
└──────────┬───────────┘
           │
           │ ITaskRepository
           ▼
┌──────────────────────┐
│   Infrastructure     │
│ Repository / EF Core │
└──────────┬───────────┘
           │
           ▼
     EF Core InMemory

Domain
  ▲
  │
  └── Entidades e enums utilizados pelas camadas internas
```

### Responsabilidade das camadas

#### Domain

Contém os elementos centrais do domínio:

- `TaskItem`
- `TaskItemStatus`

O Domain não possui dependência de API, EF Core ou infraestrutura.

#### Application

Contém os casos de uso e contratos da aplicação:

- DTOs de entrada e saída;
- `ITaskService`;
- `ITaskRepository`;
- `TaskService`.

O `TaskService` coordena as operações da aplicação e realiza o mapeamento entre DTOs e entidades.

#### Infrastructure

Contém os detalhes de persistência:

- `TaskManagementDbContext`;
- `TaskRepository`;
- configuração do EF Core InMemory.

`TaskRepository` implementa `ITaskRepository` definido na Application.

#### API

Contém a exposição HTTP da aplicação:

- `TasksController`;
- configuração de Dependency Injection;
- Swagger/OpenAPI;
- pipeline HTTP;
- tratamento global de exceções.

#### Tests

Contém testes unitários da camada de Application, principalmente do `TaskService`, utilizando um `FakeTaskRepository` para manter os testes isolados da infraestrutura real.

## SOLID e Dependency Injection

A arquitetura foi organizada para reduzir acoplamento e manter responsabilidades claras.

### Single Responsibility Principle

Cada componente possui uma responsabilidade principal:

- Controller: HTTP;
- Service: casos de uso e regras da aplicação;
- Repository: acesso aos dados;
- DbContext: integração com EF Core;
- Exception Handler: tratamento centralizado de exceções.

## Decisões arquiteturais

### Por que arquitetura em camadas?

Foi escolhida uma arquitetura em camadas simples e proporcional ao escopo do desafio, separando as responsabilidades em Domain, Application, Infrastructure e API.
Essa separação reduz o acoplamento entre as partes da aplicação, facilita os testes e permite substituir detalhes de infraestrutura sem alterar a lógica de negócio.
A estrutura foi mantida deliberadamente simples para evitar overengineering.

### Por que Repository + Service?

O `TaskService` concentra os casos de uso e a lógica da aplicação, enquanto o `ITaskRepository` abstrai o acesso aos dados.
Essa separação permite que a lógica de negócio seja testada sem depender diretamente do mecanismo de persistência.
Nos testes unitários, por exemplo, o `TaskService` recebe um `FakeTaskRepository` através da mesma abstração `ITaskRepository`.

### Por que DTOs?

Os DTOs (`CreateTaskRequest`, `UpdateTaskRequest`, `TaskResponse` e `TaskFilterRequest`) são utilizados para separar os contratos da API das entidades do domínio.
Dessa forma, a entidade `TaskItem` não é exposta diretamente como contrato HTTP e os modelos de entrada e saída podem evoluir independentemente do domínio.

### Por que não foi utilizado AutoMapper?

O projeto possui poucos DTOs e mapeamentos simples.
Por isso, o mapeamento manual foi mantido explícito no `TaskService`, evitando adicionar uma dependência adicional sem benefício significativo para o escopo do desafio.
Isso também torna o fluxo de conversão entre entidade e DTO fácil de entender e depurar.

### Por que EF Core InMemory?

O desafio técnico solicita explicitamente o uso do Entity Framework Core com o provider InMemory.
A utilização do InMemory permite executar a API sem depender de uma instalação ou configuração de banco de dados externo, mantendo o projeto simples para avaliação e execução local.
Como os dados ficam somente em memória, eles não são persistidos permanentemente e são perdidos quando a aplicação é encerrada.

### Dependency Inversion Principle

A Application depende de abstrações, especialmente `ITaskRepository`, e não de uma implementação concreta de persistência.

```text
TaskService
    │
    ▼
ITaskRepository
    ▲
    │
TaskRepository
```

Da mesma forma, o Controller depende de `ITaskService`.

As implementações concretas são registradas por Dependency Injection no `Program.cs`.

## Modelo de tarefa

Uma tarefa possui:

| Campo | Tipo | Obrigatório |
|---|---|---|
| `Id` | `Guid` | Gerado pela aplicação |
| `Title` | `string` | Sim |
| `Description` | `string?` | Não |
| `DueDate` | `DateTime?` | Não |
| `Status` | `TaskItemStatus` | Sim |

Status disponíveis:

| Valor | Status |
|---:|---|
| 1 | Pending |
| 2 | InProgress |
| 3 | Completed |

## Validações

Os DTOs de criação e atualização utilizam Data Annotations para validação de entrada.

Regras principais:

- `Title` é obrigatório;
- `Title` possui entre 1 e 200 caracteres;
- `Description` possui no máximo 1000 caracteres;
- `Status` deve corresponder a um valor válido de `TaskItemStatus`.

Com `[ApiController]`, requisições inválidas são automaticamente retornadas como `400 Bad Request`.

## Executando o projeto

### Pré-requisitos

- .NET 8 SDK
- Visual Studio 2022 ou outro ambiente compatível com .NET 8

### Pelo Visual Studio

1. Abra `TaskManagement.sln`.
2. Defina `TaskManagement.Api` como Startup Project.
3. Execute com `F5` ou `Ctrl + F5`.
4. O Swagger será aberto automaticamente no ambiente de desenvolvimento.

### Pela linha de comando

Na raiz da solução:

```bash
dotnet restore
dotnet build
dotnet run --project src/TaskManagement.Api
```

No perfil HTTP configurado atualmente, a aplicação utiliza:

```text
http://localhost:5198/swagger
```

O perfil HTTPS também está configurado para:

```text
https://localhost:7073/swagger
```

## Swagger / OpenAPI

A API possui documentação interativa com Swagger.

No ambiente de desenvolvimento, acesse:

```text
http://localhost:5198/swagger
```

ou o endereço HTTPS configurado pelo perfil de execução.

## Endpoints

### Listar tarefas

```http
GET /api/tasks
```

### Filtrar por status

```http
GET /api/tasks?status=1
```

### Filtrar por data de vencimento

```http
GET /api/tasks?dueDate=2026-10-10
```

### Pesquisar por título ou descrição

```http
GET /api/tasks?search=API
```

Os filtros podem ser combinados:

```http
GET /api/tasks?status=1&dueDate=2026-10-10&search=API
```

### Consultar por ID

```http
GET /api/tasks/{id}
```

Retornos principais:

- `200 OK` quando a tarefa existe;
- `404 Not Found` quando não existe.

### Criar tarefa

```http
POST /api/tasks
Content-Type: application/json
```

Exemplo:

```json
{
  "title": "Implementar API de tarefas",
  "description": "Desenvolver o desafio técnico da SADA",
  "dueDate": "2026-10-05T00:00:00",
  "status": 1
}
```

Retorno esperado:

```text
201 Created
```

O `Id` é gerado pela aplicação e o recurso criado é retornado com `CreatedAtAction`.

### Atualizar tarefa

```http
PUT /api/tasks/{id}
Content-Type: application/json
```

Exemplo:

```json
{
  "title": "Implementar API de tarefas - atualizado",
  "description": "Desafio técnico da SADA em desenvolvimento",
  "dueDate": "2026-10-10T00:00:00",
  "status": 2
}
```

Retornos principais:

- `200 OK` quando a tarefa é atualizada;
- `404 Not Found` quando não existe.

### Excluir tarefa

```http
DELETE /api/tasks/{id}
```

Retornos principais:

- `204 No Content` quando a tarefa é excluída;
- `404 Not Found` quando não existe.

## Tratamento de erros

Exceções não tratadas são processadas de forma centralizada pelo `GlobalExceptionHandler`, utilizando `IExceptionHandler`.

A API retorna `ProblemDetails` para erros internos, evitando expor detalhes técnicos da exceção ao consumidor.

Exemplo:

```json
{
  "title": "Internal Server Error",
  "status": 500,
  "detail": "An unexpected error occurred while processing the request."
}
```

A exceção original é registrada pelo sistema de logging para diagnóstico.

## Logging

O projeto utiliza `ILogger<T>` fornecido pelo ASP.NET Core.

Eventos relevantes são registrados no `TaskService`:

- criação de tarefa: `Information`;
- atualização de tarefa: `Information`;
- exclusão de tarefa: `Information`;
- tentativa de atualizar tarefa inexistente: `Warning`;
- tentativa de excluir tarefa inexistente: `Warning`;
- exceções não tratadas: `Error` no `GlobalExceptionHandler`.

Foi utilizado structured logging para propriedades como o `TaskId`.

## Testes automatizados

Os testes utilizam xUnit e estão concentrados na lógica da Application, principalmente no `TaskService`.

A suíte atual possui **12 testes**, cobrindo:

- criação;
- consulta por ID;
- atualização;
- exclusão;
- cenários de recurso inexistente;
- listagem;
- filtro por status;
- filtro por data;
- busca por título/descrição;
- combinação de filtros.

### Executar os testes

Na raiz da solução:

```bash
dotnet test
```

No estado atual do projeto, a suíte possui:

```text
12 Passed
0 Failed
```

## EF Core InMemory

A persistência utiliza `Microsoft.EntityFrameworkCore.InMemory`, conforme solicitado no desafio técnico.

O banco utilizado pela aplicação é configurado como:

```text
TaskManagementDb
```

Como se trata de um banco InMemory, os dados não devem ser considerados persistência permanente. Ao reiniciar a aplicação, os dados armazenados em memória são perdidos.

A escolha elimina a necessidade de configurar um banco externo para executar e avaliar o desafio.

## Estrutura do projeto

```text
TaskManagement
│
├── TaskManagement.sln
│
├── src
│   ├── TaskManagement.Api
│   │   ├── Controllers
│   │   │   └── TasksController.cs
│   │   ├── ExceptionHandlers
│   │   │   └── GlobalExceptionHandler.cs
│   │   └── Program.cs
│   │
│   ├── TaskManagement.Application
│   │   ├── DTOs
│   │   ├── Interfaces
│   │   └── Services
│   │
│   ├── TaskManagement.Domain
│   │   ├── Entities
│   │   └── Enums
│   │
│   └── TaskManagement.Infrastructure
│       ├── Data
│       └── Repositories
│
└── tests
    └── TaskManagement.Tests
        ├── Fakes
        └── Services
```

## Status da implementação

```text
Domain                         ✅
Application                    ✅
Infrastructure                 ✅
EF Core InMemory               ✅
Dependency Injection           ✅
Swagger/OpenAPI                ✅
CRUD REST                      ✅
Filtros e busca                ✅
Validações                     ✅
Tratamento global de erros     ✅
ProblemDetails                 ✅
Logging                        ✅
Testes xUnit                   ✅ 12 testes
```

## Entrega

O projeto foi estruturado para ser executado localmente e disponibilizado em um repositório GitHub ou plataforma equivalente, conforme solicitado pelo desafio técnico.
