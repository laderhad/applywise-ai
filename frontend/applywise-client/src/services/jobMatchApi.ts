import type {
  JobMatchRequest,
  JobMatchResponse,
} from '../types/jobMatch'

interface ApiError {
  detail?: string
  message?: string
  title?: string
}

export async function analyzeJobMatch(
  request: JobMatchRequest,
): Promise<JobMatchResponse> {
  const response = await fetch('/api/job-match/analyze', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(request),
  })

  if (!response.ok) {
    let error: ApiError = {}

    try {
      error = (await response.json()) as ApiError
    } catch {
      // The fallback below handles non-JSON error responses.
    }

    throw new Error(
      error.detail ??
        error.message ??
        error.title ??
        'The analysis could not be completed.',
    )
  }

  return (await response.json()) as JobMatchResponse
}
