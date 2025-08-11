# Local Setup Guide using Docker

## Prerequisites
- Docker Desktop installed and running
- Git installed

## Installation Steps

1. **Clone the repository**  
   Run the following command in your terminal:
   ```bash
   git clone https://github.com/minofis/Bank
   ```

2. **Navigate to the project directory**  
   ```bash
   cd Bank
   ```

3. **Start the containers**  
   Run the Docker compose command:
   ```bash
   docker compose up
   ```

4. **Access the application**  
   Once the containers are running, open your browser and navigate to:  
   [http://localhost:5001/swagger/index.html](http://localhost:5001/swagger/index.html)  
   This will open the Swagger UI interface.

## Notes
- The first run may take some time as Docker downloads the required images
- Make sure ports 5001 (API) and your database port are available
