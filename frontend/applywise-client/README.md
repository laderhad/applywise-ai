# ApplyWise frontend

This is the React + TypeScript frontend for ApplyWise.

It provides the resume upload form, job description input, localized EN/TR UI,
analysis result cards, and saved analysis history view. API calls are proxied
through the deployed frontend server to the ASP.NET Core backend.

## Scripts

Install dependencies from this directory:

```bash
npm install
```

Run the Vite development server:

```bash
npm run dev
```

Run lint:

```bash
npm run lint
```

Create a production build:

```bash
npm run build
```

Preview a production build locally:

```bash
npm run preview
```

## Docker behavior

The Docker image builds the Vite app and serves the static output with Nginx.
Because this is a production build, source changes do not hot-reload inside the
running container. Rebuild the frontend container after UI changes:

```bash
docker compose up -d --build frontend
```

The frontend is served at:

```text
http://localhost:5173
```
