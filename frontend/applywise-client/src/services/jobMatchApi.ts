import type {
  JobMatchHistoryDetail,
  JobMatchHistoryItem,
  JobMatchRequest,
  JobMatchResponse,
  ResumeUploadResponse,
} from '../types/jobMatch'

interface ApiError {
  detail?: string
  message?: string
  title?: string
}

async function getErrorMessage(
  response: Response,
  fallbackMessage: string,
): Promise<string> {
  try {
    const error = (await response.json()) as ApiError

    return error.detail ?? error.message ?? error.title ?? fallbackMessage
  } catch {
    return fallbackMessage
  }
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
    throw new Error(
      await getErrorMessage(
        response,
        'The analysis could not be completed.',
      ),
    )
  }

  return (await response.json()) as JobMatchResponse
}

export async function uploadResumePdf(
  file: File,
): Promise<ResumeUploadResponse> {
  const formData = new FormData()
  formData.append('file', file)

  const response = await fetch('/api/resumes/upload', {
    method: 'POST',
    body: formData,
  })

  if (!response.ok) {
    throw new Error(
      await getErrorMessage(
        response,
        'The resume PDF could not be uploaded.',
      ),
    )
  }

  return (await response.json()) as ResumeUploadResponse
}

export async function getJobMatchHistory(): Promise<JobMatchHistoryItem[]> {
  const response = await fetch('/api/job-match/history')

  if (!response.ok) {
    throw new Error(
      await getErrorMessage(
        response,
        'The analysis history could not be loaded.',
      ),
    )
  }

  return (await response.json()) as JobMatchHistoryItem[]
}

export async function getJobMatchHistoryDetail(
  id: string,
): Promise<JobMatchHistoryDetail> {
  const response = await fetch(`/api/job-match/history/${id}`)

  if (!response.ok) {
    throw new Error(
      await getErrorMessage(
        response,
        'The saved analysis could not be loaded.',
      ),
    )
  }

  return (await response.json()) as JobMatchHistoryDetail
}
