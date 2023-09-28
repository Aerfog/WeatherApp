# WeatherApp

WeatherApp is a sample application that allows users to manage weather information for different locations.

## Features

- View weather information for a specific location
- Add new weather records
- Update existing weather records
- Delete weather records
- Refresh weather data from an external API

## Technologies Used

- .NET Core
- ASP.NET Core Web API
- Entity Framework Core
- In-Memory Database
- HttpClient

## Getting Started

### Prerequisites

- .NET Core SDK [Download here](https://dotnet.microsoft.com/download)
- Git [Download here](https://git-scm.com/downloads)

### Installation

1. Clone the repository: `git clone https://github.com/your-username/WeatherApp.git`
2. Navigate to the project folder: `cd WeatherApp`

### Configuration

1. Open the `WeatherApp/appsettings.json` file.
2. Set the value of the `"WeatherAPIKey"` property to your API key for the weather API.

### Run Locally

1. Build the project: `dotnet build`
2. Run the application: `dotnet run --project WeatherApp`

The application will start running on `http://localhost:5000`. You can access the API endpoints using tools like Postman or a web browser.

## API Endpoints

- `GET /api/weather/{id}`: Retrieve weather information for a specific weather record.
- `POST /api/weather`: Create a new weather record.
- `PUT /api/weather/{id}`: Update an existing weather record.
- `DELETE /api/weather/{id}`: Delete a weather record.
- `POST /api/weather/refresh`: Refresh weather data from the external API.

## Contributing

Contributions are welcome! If you find any bugs or have suggestions for improvements, please feel free to open an issue or submit a pull request.

## License

The project is licensed under the [MIT license](https://opensource.org/licenses/MIT).