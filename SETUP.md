# 🚀 Guia de Setup - CompraCertaAI

## 📋 Pré-requisitos

- **.NET 8.0 SDK** instalado
- **SQL Server** (SQL Server Express ou LocalDB)
- **Visual Studio 2022** ou **VS Code**
- **Git**
- **Chave de API Groq** (gratuita em https://console.groq.com)

---

## 🔧 1. Configuração do Ambiente

### 1.1 Instalar dependências

```bash
cd CompraCertaAI
dotnet restore
```

### 1.2 Configurar `appsettings.json`

Edite o arquivo `CompraCertaAI.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=CompraCertaAIDb;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "Jwt": {
    "Key": "a05ac9f24b400ea200b5b2c84bf58625b09335d68fd3e0d4071ed2861e7eb1ee",
    "Issuer": "CompraCertaAI",
    "Audience": "CompraCertaAIUser",
    "ExpireHours": 2
  },
  "Groq": {
    "ApiUrl": "https://api.groq.com/openai/v1/chat/completions",
    "ApiKey": "SUA_CHAVE_GROQ_AQUI",  // ← PREENCHER COM SUA CHAVE!
    "Model": "llama-3.3-70b-versatile"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

#### Obter Chave Groq:
1. Acesse https://console.groq.com
2. Faça login ou crie conta (gratuita)
3. Vá em **API Keys** → **Criar Nova Chave**
4. Copie a chave e cole em `Groq:ApiKey`

### 1.3 Banco de Dados

#### Opção A: Migrations do EF Core (Recomendado)
```bash
cd CompraCertaAI.API
dotnet ef database update
```

#### Opção B: Script SQL Manual
Execute as migrations localizado em `CompraCertaAI.Repositorio/Migrations/`

---

## ▶️ 2. Executar o Projeto

### 2.1 Via Terminal

```bash
cd CompraCertaAI.API
dotnet run
```

A API estará disponível em: `https://localhost:5139`

**Swagger (Documentação interativa)**: `https://localhost:5139/swagger/ui`

### 2.2 Via Visual Studio

1. Abra `CompraCertaAI.sln`
2. Clique em **Executar** (F5) ou **Iniciar Sem Depuração** (Ctrl+F5)

---

## 📡 3. Fluxos de API

### 3.1 Cadastro de Usuário

**POST** `/api/auth/register`

```json
{
  "nome": "João Silva",
  "email": "joao@example.com",
  "senha": "Senha123!",
  "categoriasFavoritasIds": [1, 2, 3, 4, 5]
}
```

**Resposta (201)**:
```json
{
  "id": 1,
  "nome": "João Silva",
  "email": "joao@example.com",
  "dataCriacao": "2026-04-11T10:30:00Z",
  "categoriasFavoritas": [...]
}
```

---

### 3.2 Login

**POST** `/api/auth/login`

```json
{
  "email": "joao@example.com",
  "senha": "Senha123!"
}
```

**Resposta (200)**:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiraEm": "2026-04-11T12:30:00Z",
  "email": "joao@example.com",
  "id": 1
}
```

> ⚠️ Use o `token` no header `Authorization: Bearer {token}` para requisições autenticadas

---

### 3.3 Recomendações com IA (10 ofertas das categorias favoritas)

**GET** `/api/products/recommendations`

**Headers**:
```
Authorization: Bearer {seu_token_jwt}
```

**Resposta (200)**:
```json
[
  {
    "nomeProduto": "Notebook Dell Inspiron 15",
    "descricao": "Processador Intel i7, 16GB RAM, 512GB SSD",
    "imagemUrl": "https://example.com/img.jpg",
    "loja": "Amazon",
    "linkProduto": "https://amazon.com/..."
  },
  ...
]
```

> 🤖 As recomendações são geradas em tempo real usando **Groq Llama 3.3** com base nas 5 categorias favoritas do usuário

---

### 3.4 Busca de Produtos com IA

**GET** `/api/products/search?query=notebook`

**Headers**:
```
Authorization: Bearer {seu_token_jwt}
```

**Query Parameters**:
- `query` (string) - Termo de busca
- `categoriaId` (int, opcional) - Filtrar por categoria

**Resposta (200)**:
```json
[
  {
    "nomeProduto": "Notebook Gamer ASUS",
    "descricao": "RTX 4060, i9, 32GB RAM",
    "imagemUrl": "https://...",
    "loja": "Mercado Livre",
    "linkProduto": "https://..."
  },
  ...
]
```

> 📝 A busca é registrada automaticamente no histórico do usuário
> 🤖 Usa **Groq Llama 3.3** para interpretar a busca e retornar produtos relevantes

---

### 3.5 Histórico de Pesquisas

**GET** `/api/historico/usuario`

**Headers**:
```
Authorization: Bearer {seu_token_jwt}
```

**Resposta (200)**:
```json
[
  {
    "id": 1,
    "termoBusca": "notebook",
    "dataPesquisa": "2026-04-11T11:00:00Z",
    "resultados": 10
  },
  ...
]
```

---

### 3.6 Listar Categorias Disponíveis

**GET** `/api/usuario/categorias/disponiveis`

**Resposta (200)**:
```json
[
  {
    "id": 1,
    "nome": "Eletrônicos",
    "ativa": true
  },
  {
    "id": 2,
    "nome": "Moda",
    "ativa": true
  },
  ...
]
```

---

### 3.7 Atualizar Categorias Favoritas

**PUT** `/api/usuario/categorias`

```json
{
  "categoriaIds": [1, 3, 5, 7, 9]
}
```

**Resposta (200)**:
```json
{
  "mensagem": "Categorias atualizadas com sucesso."
}
```

---

## 🧠 4. Arquitetura & Fluxo de IA

### Componentes Principais

```
┌─────────────────────────────────────────────────┐
│         CompraCertaAI.API (Controllers)         │  ← HTTP Endpoints
├─────────────────────────────────────────────────┤
│     CompraCertaAI.Aplicacao (Use Cases)         │  ← Lógica de Negócio
├─────────────────────────────────────────────────┤
│ CompraCertaAI.Service (Groq + Processamento)    │  ← Integrações Externas
├─────────────────────────────────────────────────┤
│   CompraCertaAI.Repositorio (Data Access)       │  ← SQL Server / EF Core
├─────────────────────────────────────────────────┤
│       CompraCertaAI.Dominio (Entities)          │  ← Modelos de Domínio
└─────────────────────────────────────────────────┘
```

### Fluxo de Recomendação com IA

```
Usuario
   ↓
ProductsController.Recommendations()
   ↓
RecomendacaoService.ObterRecomendacoesAsync()
   ↓
AiPromptTemplates.BuildRecommendationPrompt(categorias)
   ↓
AiService.GetAiResponseAsync(prompt)  ← Groq Llama 3.3
   ↓
AiProductParser.ParseProducts() ← Parse JSON
   ↓
ProdutoDTO[] (Até 10 ofertas)
```

### IA & Prompts

- **Recomendações**: `AiPromptTemplates.BuildRecommendationPrompt()`
  - Lê as 5 categorias favoritas do usuário
  - Pede até 10 produtos da Groq
  - Prioriza lojas populares (Amazon, Mercado Livre, etc.)

- **Busca**: `AiPromptTemplates.BuildSearchPrompt()`
  - Interpreta o termo de busca
  - Retorna até 10 produtos relevantes
  - Registra automaticamente no histórico

---

## 🧪 5. Testes

### Via Swagger (Recomendado)

1. Abra `https://localhost:5139/swagger/ui`
2. Expanda qualquer endpoint
3. Clique em **Try it out**
4. Preencha os campos e clique **Execute**

### Via cURL

#### Cadastro
```bash
curl -X POST "https://localhost:5139/api/auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "João",
    "email": "joao@test.com",
    "senha": "Teste123!",
    "categoriasFavoritasIds": [1,2,3,4,5]
  }' \
  -k
```

#### Login
```bash
curl -X POST "https://localhost:5139/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"email":"joao@test.com","senha":"Teste123!"}'  \
  -k
```

#### Recomendações (Autenticado)
```bash
curl -X GET "https://localhost:5139/api/products/recommendations" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -k
```

---

## ⚠️ 6. Troubleshooting

### Erro: "Chave da Groq não configurada"
- **Solução**: Preencha `Groq:ApiKey` em `appsettings.json`

### Erro: "Database connection failed"
- **Solução**: Verifique se SQL Server está rodando
- Corrija a `DefaultConnection` string em `appsettings.json`

### Erro: "JWT validation failed"
- **Solução**: Certifique-se que o token está no header correto:
  ```
  Authorization: Bearer {token}
  ```

### IA retorna vazio ou erro
- **Solução**: Testar chave Groq em https://console.groq.com
- Verificar quota de API (limite gratuito: ~30 req/min)

---

## 📚 7. Referências

- **Groq API**: https://console.groq.com
- **Documentação Groq**: https://console.groq.com/docs
- **Modelo Groq**: Llama 3.3 70B Versatile
- **.NET 8**: https://learn.microsoft.com/en-us/dotnet/
- **Entity Framework Core**: https://learn.microsoft.com/en-us/ef/core/

---

## 🎯 Checklist de Implementação

- ✅ JWT Authentication com tokens de 2 horas
- ✅ Cadastro de usuário (nome, email, senhaHash, dataCriacao)
- ✅ 5 categorias favoritas por usuário
- ✅ Histórico de pesquisas automático
- ✅ Recomendações IA com Groq (10 ofertas iniciais)
- ✅ Busca de produtos com IA usando AiPromptTemplates
- ✅ Integração com Groq Llama 3.3
- ✅ Fallback em banco de dados se IA falhar
- ✅ Autenticação em endpoints sensíveis

---

## 📞 Suporte

Para dúvidas ou issues, abra um ticket no repositório:
https://github.com/RodrigoPCamilo/CompraCerta-AI-Back-end
