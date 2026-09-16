# WeatherAPI

API backend desenvolvida em **ASP.NET Core Web API** para pesquisar uma cidade, obter as suas coordenadas geográficas, consultar o clima actual e guardar o histórico das consultas numa base de dados SQL Server.

## Tecnologias utilizadas

- C#
- ASP.NET Core Web API
- .NET 10
- Entity Framework Core
- SQL Server LocalDB
- Swagger/OpenAPI
- HttpClient
- Open-Meteo Geocoding API
- Open-Meteo Weather Forecast API

## Funcionalidades

- Pesquisa de cidades através da API de geocodificação da Open-Meteo.
- Consulta do clima actual usando latitude e longitude.
- Armazenamento das consultas no SQL Server.
- Consulta de todo o histórico.
- Filtros por cidade, país e intervalo de datas.
- Consulta de um registo pelo identificador.
- Eliminação de um registo.
- Endpoint de verificação do estado da aplicação.
- Logging das chamadas externas e operações principais.
- Tratamento global de erros e Correlation ID.

## Estrutura do projecto

```text
WeatherAPI/
├── Context/
│   └── AppDbContext.cs
├── Controllers/
│   └── WeatherController.cs
├── ExternalModel/
│   ├── DocsResponse.cs
│   └── GeoCodingResponse.cs
├── Middleware/
│   └── Exceptionsm.cs
├── Migrations/
├── Model/
│   └── Weather.cs
├── Service/
│   └── WeatherService.cs
├── Properties/
│   └── launchSettings.json
├── Program.cs
├── appsettings.json
└── WeatherAPI.csproj
```

## Pré-requisitos

Antes de executar o projecto, confirme que possui:

- Visual Studio 2022 ou superior;
- SDK do .NET 10;
- SQL Server LocalDB ou outra instância do SQL Server;
- Entity Framework Core Tools.

Para verificar o SDK instalado:

```bash
dotnet --version
```

Para instalar ou actualizar o Entity Framework Tools:

```bash
dotnet tool install --global dotnet-ef
```

ou:

```bash
dotnet tool update --global dotnet-ef
```

## Configuração da base de dados

A connection string está configurada no ficheiro `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=WeatherDb;Integrated Security=True;Pooling=False;Encrypt=False;Trust Server Certificate=False"
  }
}
```


## Restaurar os pacotes

Na pasta do projecto, execute:

```bash
dotnet restore
```

## Criar ou actualizar a base de dados

O projecto já possui migrations. Para criar ou actualizar a base de dados, execute:

```bash
dotnet ef database update
```

No Package Manager Console do Visual Studio, também pode utilizar:

```powershell
Update-Database
```

Caso seja necessário criar uma nova migration:

```bash
dotnet ef migrations add InitialCreate
```

Depois:

```bash
dotnet ef database update
```

## Executar a aplicação

Pelo Visual Studio, pressione `F5` ou clique no botão de execução.

Pelo terminal:

```bash
dotnet run
```

A aplicação está configurada para utilizar:

```text
http://localhost:5013
```

Swagger:

```text
http://localhost:5013/swagger
```

Health check:

```text
http://localhost:5013/health
```

## Endpoints

### Consultar o clima actual

```http
GET /api/Weather?city=Maputo
```

Com país:

```http
GET /api/Weather?city=Maputo&country=MZ
```

O parâmetro `city` é obrigatório e `country` é opcional.

Para evitar diferenças de idioma no nome do país, recomenda-se utilizar o código do país, por exemplo:

```text
MZ
PT
AO
ZA
BR
```

Exemplo de resposta:

```json
{
  "id": "a1f8c131-6078-45b0-a265-c5d617631501",
  "city": "Maputo",
  "country": "Moçambique",
  "countryCode": "MZ",
  "region": "Maputo City",
  "latitude": -25.9653,
  "longitude": 32.5892,
  "timeZone": "Africa/Maputo",
  "temperature": 27.4,
  "temperatureUnit": "°C",
  "apparentTemperature": 29.1,
  "humidity": 71,
  "humidityUnit": "%",
  "windSpeed": 12.5,
  "windSpeedUnit": "km/h",
  "weatherCode": 2,
  "weatherTime": "2026-07-15T10:00:00",
  "consultedAt": "2026-07-15T08:00:00Z"
}
```

### Consultar todo o histórico

```http
GET /api/Weather/history
```

### Filtrar por cidade

```http
GET /api/Weather/history?city=Maputo
```

### Filtrar por país

```http
GET /api/Weather/history?country=Moçambique
```

### Filtrar por intervalo de datas

```http
GET /api/Weather/history?startDate=2026-07-01&endDate=2026-07-15
```

### Combinar filtros

```http
GET /api/Weather/history?city=Maputo&country=Moçambique&startDate=2026-07-01&endDate=2026-07-15
```

### Consultar um registo por ID

```http
GET /api/Weather/history/{id}
```

Exemplo:

```http
GET /api/Weather/history/a1f8c131-6078-45b0-a265-c5d617631501
```

Quando o ID não existir, a API devolve `404 Not Found`.

### Eliminar um registo

```http
DELETE /api/Weather/history/{id}
```

Quando a eliminação for concluída, a API devolve `204 No Content`.

Quando o ID não existir, a API devolve `404 Not Found`.

### Verificar o estado da aplicação

```http
GET /health
```

Resposta:

```json
{
  "status": "healthy"
}
```

## Como testar no Swagger

1. Execute a aplicação.
2. Abra `http://localhost:5013/swagger`.
3. Seleccione um endpoint.
4. Clique em `Try it out`.
5. Preencha os parâmetros.
6. Clique em `Execute`.

Para o primeiro teste, utilize:

```text
city: Maputo
country: deixar vazio
```

Depois teste com:

```text
city: Maputo
country: MZ
```

## APIs externas utilizadas

### Open-Meteo Geocoding API

```text
https://geocoding-api.open-meteo.com/v1/search
```

Exemplo:

```text
https://geocoding-api.open-meteo.com/v1/search?name=Maputo&count=10&language=pt&format=json
```

### Open-Meteo Weather Forecast API

```text
https://api.open-meteo.com/v1/forecast
```

Exemplo:

```text
https://api.open-meteo.com/v1/forecast?latitude=-25.9653&longitude=32.5892&current=temperature_2m,relative_humidity_2m,apparent_temperature,weather_code,wind_speed_10m&timezone=auto
```

## Tratamento de erros

A aplicação trata os seguintes casos:

| Situação | Código HTTP |
|---|---:|
| Cidade vazia | 400 Bad Request |
| Cidade não encontrada | 404 Not Found |
| Registo não encontrado | 404 Not Found |
| Falha ou timeout numa API externa | 502 Bad Gateway |
| Resposta externa inválida | 502 Bad Gateway |
| Erro inesperado | 500 Internal Server Error |

Exemplo de erro:

```json
{
  "message": "Não foi possível consultar o serviço meteorológico.",
  "correlationId": "b038b670-2fe2-4e38-aa70-07a3e1e348e1"
}
```

## Timeout

O `HttpClient` está configurado com timeout de 60 segundos:

```csharp
builder.Services.AddHttpClient<WeatherService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(60);
    client.DefaultRequestHeaders.UserAgent.ParseAdd("WeatherAPI/1.0");
});
```

Se ocorrer timeout, teste as APIs externas directamente no navegador para confirmar se a máquina possui acesso à Internet e se não existe bloqueio por firewall, proxy, DNS ou antivírus.

## Decisões técnicas

- Foi utilizado SQL Server para garantir persistência dos dados após o reinício da aplicação.
- A integração externa está concentrada no `WeatherService`, evitando colocar toda a lógica no controller.
- Os modelos externos representam apenas os campos necessários das APIs da Open-Meteo.
- Quando existem vários resultados de geocodificação, é utilizado o primeiro resultado válido após o filtro por país.
- O histórico é devolvido da consulta mais recente para a mais antiga.
- O Swagger foi mantido activo para facilitar os testes da API.
