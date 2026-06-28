# ApplyWise AI - Project Instructions

## Project Goal

ApplyWise AI is a local-first AI-powered CV and job description matching application.

The main goal is to analyze a user's CV/resume against a job description and return a structured evaluation including:

* Match score
* Strong points
* Weak points
* Missing keywords
* Recommended CV bullet improvements
* Cover letter draft
* LinkedIn message draft
* Short summary

This project must be useful as both:

1. A real personal career tool
2. A portfolio-level full-stack AI engineering project

---

## Core Principle

Build a working MVP first.

Do not overengineer the project in the beginning.

The first version should work with plain text input only:

```text
Resume text + Job description text -> Local LLM -> Structured JSON result
```

PDF support, embedding, vector database, RAG, and agent features will be added later.

---

## Tech Stack

### Frontend

* React
* TypeScript
* Vite
* ESLint
* Basic CSS first
* Tailwind can be added later if needed

### Backend

* ASP.NET Core Web API
* C#
* Dependency Injection
* HTTP client integration with Ollama
* Clean and readable folder structure

### LLM

* Ollama
* Local model only
* No paid OpenAI, Gemini, Claude, or external paid API in the MVP

Primary model:

```text
llama3.1:8b
```

Fallback model:

```text
gemma3:4b
```

If `llama3.1:8b` is too slow, switch to `gemma3:4b`.

### Database

For V1, database is optional.

Later:

* PostgreSQL
* Entity Framework Core
* Analysis history
* User/project records

### Vector Database

Not in V1.

Later options:

* PostgreSQL + pgvector
* Qdrant

---

## MVP Scope

The MVP must include:

### Backend

Create the endpoint:

```http
POST /api/job-match/analyze
```

Request body:

```json
{
  "resumeText": "string",
  "jobDescription": "string"
}
```

Response body:

```json
{
  "matchScore": 74,
  "strongPoints": [],
  "weakPoints": [],
  "missingKeywords": [],
  "recommendedBullets": [],
  "coverLetterDraft": "",
  "linkedinMessageDraft": "",
  "summary": ""
}
```

### Frontend

The frontend must include:

* Resume text area
* Job description text area
* Analyze button
* Loading state
* Error state
* Result display cards

The result page should clearly show:

* Match score
* Strong points
* Weak points
* Missing keywords
* Recommended bullets
* Cover letter draft
* LinkedIn message draft
* Summary

---

## Recommended Folder Structure

```text
applywise-ai/
  backend/
    ApplyWise.Api/
      Controllers/
      Models/
        Requests/
        Responses/
      Services/
      Prompts/
      Program.cs
      appsettings.json

  frontend/
    applywise-client/
      src/
        components/
        pages/
        services/
        types/
        App.tsx
        main.tsx

  INSTRUCTIONS.md
  README.md
```

---

## Backend Rules

Use clear and simple code.

Avoid unnecessary abstraction in V1.

Do not create complex architecture before the MVP works.

Recommended backend files:

```text
Controllers/JobMatchController.cs
Services/OllamaService.cs
Prompts/JobMatchPromptBuilder.cs
Models/Requests/AnalyzeJobMatchRequest.cs
Models/Responses/AnalyzeJobMatchResponse.cs
```

The backend should:

* Validate empty resume text
* Validate empty job description
* Call Ollama locally
* Ask the model to return strict JSON
* Parse JSON safely
* Return a typed response to the frontend

---

## Ollama Configuration

Ollama should run locally.

Default Ollama endpoint:

```text
http://localhost:11434
```

Default Ollama generate endpoint:

```text
http://localhost:11434/api/generate
```

Use configuration instead of hardcoding model values.

Example `appsettings.json`:

```json
{
  "Ollama": {
    "BaseUrl": "http://localhost:11434",
    "Model": "llama3.1:8b"
  },
  "AllowedHosts": "*"
}
```

If the model is too slow, change only this line:

```json
"Model": "gemma3:4b"
```

Never hardcode the model name inside business logic.

---

## Prompt Rules

The LLM prompt must force structured JSON output.

The model should not return Markdown.

The model should not explain outside JSON.

The prompt must include:

* User resume text
* Job description text
* Expected JSON schema
* Scoring criteria
* Output language instruction

The response should preferably be in English, because the tool is career-focused and useful for international job applications.

Later, a language selector can be added.

---

## JSON Output Rules

The model must return only this JSON shape:

```json
{
  "matchScore": 0,
  "strongPoints": [],
  "weakPoints": [],
  "missingKeywords": [],
  "recommendedBullets": [],
  "coverLetterDraft": "",
  "linkedinMessageDraft": "",
  "summary": ""
}
```

Rules:

* `matchScore` must be between 0 and 100.
* `strongPoints` must be specific.
* `weakPoints` must be honest.
* `missingKeywords` must be based on the job description.
* `recommendedBullets` must be CV-ready.
* `coverLetterDraft` must be concise.
* `linkedinMessageDraft` must be short and human.
* `summary` must give an honest final evaluation.

---

## Guardrail Rules

The system should not:

* Invent fake experience
* Add skills the user does not actually have
* Claim the candidate used a technology if it is not present in the resume
* Produce misleading CV bullets
* Create fake company names
* Create fake degrees
* Create fake certifications

The system may suggest wording improvements, but they must remain truthful.

Bad example:

```text
Built Kafka-based microservices in production.
```

If Kafka is not in the CV, do not say this.

Good example:

```text
If you have Kafka experience, consider adding a bullet about event-driven communication.
```

---

## Frontend Rules

Use simple and readable React code.

Do not overcomplicate the UI in the first version.

Use:

* React functional components
* TypeScript types
* API service file
* Simple reusable components
* ESLint

Avoid:

* Global state management in V1
* Redux in V1
* Complex UI libraries in V1
* Unnecessary routing in V1
* Overdesigned dashboard in V1

Recommended frontend files:

```text
src/
  components/
    JobMatchForm.tsx
    MatchResultCard.tsx
  services/
    jobMatchApi.ts
  types/
    jobMatch.ts
  App.tsx
  main.tsx
```

---

## Development Order

Follow this order strictly.

### Phase 1 - Working MVP

1. Create backend Web API
2. Create frontend React app
3. Connect backend to Ollama
4. Create `/api/job-match/analyze`
5. Create frontend form
6. Display result in UI

### Phase 2 - UI Improvement

1. Add better result cards
2. Add loading skeleton
3. Add error messages
4. Improve responsive layout
5. Add basic empty states

### Phase 3 - Persistence

1. Add PostgreSQL
2. Add EF Core
3. Save analysis history
4. List old analyses
5. View old analysis detail

### Phase 4 - PDF Support

1. Upload CV PDF
2. Extract text from PDF
3. Use extracted text in analysis
4. Show extracted text preview

### Phase 5 - Embedding and Semantic Matching

1. Chunk resume sections
2. Chunk job requirements
3. Generate local embeddings
4. Compare similarity
5. Show matched requirements

### Phase 6 - RAG

1. Store old CVs, projects, and analyses
2. Retrieve relevant user experience
3. Generate better CV bullets using retrieved context
4. Use retrieved evidence in the response

### Phase 7 - Agent Features

1. Generate CV improvements
2. Generate cover letter
3. Generate LinkedIn message
4. Save application record
5. Suggest follow-up date

---

## What Not To Do Early

Do not add these before the MVP works:

* Authentication
* User roles
* Payment
* Complex dashboard
* Microservices
* Kubernetes
* Cloud deployment
* RAG
* Agents
* Vector database
* PDF parsing
* Redux
* Complex UI framework

First, make the simple version work.

---

## Code Style

Use:

* Clear names
* Small methods
* Typed request/response models
* Async/await
* Dependency Injection
* Proper error handling
* Configuration-based model selection

Avoid:

* Giant controllers
* Hardcoded prompts everywhere
* Business logic inside `Program.cs`
* Unnecessary design patterns
* Premature Clean Architecture complexity
* Fake enterprise architecture

---

## README Goal

The final README should explain:

* What the project does
* Why it exists
* Tech stack
* How to run Ollama
* How to run backend
* How to run frontend
* Example request/response
* Screenshots
* Future roadmap

---

## Portfolio Positioning

This project should be presented as:

```text
Local-first AI-powered CV and job matching assistant built with React, ASP.NET Core, Ollama, and later PostgreSQL/pgvector.
```

It should demonstrate:

* Full-stack development
* Local LLM integration
* Prompt engineering
* Structured output handling
* Guardrails
* Later: PDF parsing
* Later: embeddings
* Later: RAG
* Later: agentic workflow

---

## Final Rule

Always prioritize a working product over a perfect architecture.

The correct order is:

```text
Make it work.
Make it clean.
Make it useful.
Make it impressive.
```
