# ApplyWise AI

ApplyWise AI is a local-first CV and job description matching application. It
uses React, ASP.NET Core, and a local Ollama model to produce a structured,
evidence-based analysis.

## Prerequisites

- .NET 10 SDK
- Node.js 24 and npm
- Ollama
- Docker Desktop (only for the container workflow)

Pull the default model once:

```bash
ollama pull llama3.1:8b
```

To use the lighter fallback model instead:

```bash
ollama pull gemma3:4b
```

## Run for development

Start Ollama:

```bash
ollama serve
```

Start the backend in another terminal:

```bash
dotnet run \
  --project backend/ApplyWise.Api/ApplyWise.Api.csproj \
  --launch-profile http
```

Start the frontend in another terminal:

```bash
npm --prefix frontend/applywise-client run dev
```

Open `http://localhost:5173`.

## Run the app with Docker Compose

Ollama intentionally runs natively on macOS so it can use Metal GPU
acceleration. The backend and frontend run in containers.

Ollama binds to localhost by default. To make it reachable from Docker for the
current terminal session, quit any running Ollama app and start:

```bash
OLLAMA_HOST=0.0.0.0:11434 ollama serve
```

Only expose Ollama this way on a trusted local network. Then start ApplyWise:

```bash
docker compose up --build
```

Open `http://localhost:5173`. The backend is also available directly at
`http://localhost:5232`.

To select another installed model:

```bash
OLLAMA_MODEL=gemma3:4b docker compose up --build
```

Stop the containers:

```bash
docker compose down
```

## API

```http
POST /api/job-match/analyze
Content-Type: application/json
```

```json
{
  "resumeText": "C# and React developer",
  "jobDescription": "We are looking for a full-stack developer."
}
```

The API returns a match score, strengths, weaknesses, missing keywords,
recommended CV bullets, a cover letter draft, a LinkedIn message, and a short
summary.
