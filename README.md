# CPA-JsonHandler API

This is a simple .NET Core application API that accepts an HTTP POST request with any JSON payload and returns a success response containing an exact copy of the JSON request.

## Features

- Accepts any JSON payload via HTTP POST.
- Return back the same JSON payload in the response.


## How to Run

1. Clone the repository and navigate to the `CPA-JsonHandler` folder.
2. Run the application using the following command:
  There are some custom profiles you can run any or even more than one
   ```bash
   dotnet run --urls=http://localhost:<PORT>
   or
    dotnet run --launch-profile http
    dotnet run --launch-profile custom_instance
    dotnet run --launch-profile custom_instance2
    dotnet run --launch-profile custom_instance3
   ---
 
3.	Test the API using curl or Postman:  
curl -X POST http://localhost:<PORT>/api/echo \
     -H "Content-Type: application/json" \
     -d '{"key":"value"}'

Example Response:

{
    "key": "value"
}

### **Round Robin API: README.md**

```markdown
# CPA Round Robin API

The Round Robin API is a .NET Core application that receives HTTP POST requests and distributes them to multiple instances of the **CPA-JsonHander API** in a round-robin manner. It ensures requests are routed to healthy instances only.

## Features

- Implements round-robin routing to multiple backend services.
- Periodic health checks to monitor the availability of JsonHandler API instances.
- Dynamically reroutes requests if an instance is down.

## Requirements

- .NET Core SDK (6.0 or later)

## Configuration

The list of Application API instances is stored in `appsettings.json`. Update this file to add or remove instances:

```json
{
  "ApplicationInstances": [
    "http://localhost:5041/api/jsonhandler",
    "http://localhost:5042/api/jsonhandler",
    "http://localhost:5043/api/jsonhandler",
    "http://localhost:5044/api/jsonhandler"
  ]
}
```

## How to Run
	1.	Clone the repository and navigate to the RoundRobinAPI folder.
	2.	Restore dependencies and build the project:

dotnet restore
dotnet build


	3.	Run the application:

dotnet run --urls=http://localhost:5216
Or
dotnet run --launch-profile http

	4.	Send a POST request to the Round Robin API:

curl -X POST http://localhost:5216/api/roundrobin \
     -H "Content-Type: application/json" \
     -d '{"game":"Mobile Legends","gamerID":"GYUTDTE","points":20}'

Example Response:

{
    "game": "Mobile Legends",
    "gamerID": "GYUTDTE",
    "points": 20
}

Health Checks
	•	The Round Robin API performs periodic health checks on all configured JsonHandler API instances.
	•	If all instances are down, the API returns a 503 Service Unavailable response.

Example Usage
	1.	Start three instances of the JsonHandler API on different ports based on above configure profiles.
	2.	Start the Round Robin API.
	3.	Send requests to the Round Robin API, and they will be distributed among the Application API instances.
