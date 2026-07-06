export interface JobMatchRequest {
  resumeText: string
  jobDescription: string
}

export interface JobMatchResponse {
  matchScore: number
  strongPoints: string[]
  weakPoints: string[]
  missingKeywords: string[]
  recommendedBullets: string[]
  coverLetterDraft: string
  linkedinMessageDraft: string
  summary: string
}

export interface JobMatchHistoryItem {
  id: string
  matchScore: number
  summary: string
  createdAt: string
}
