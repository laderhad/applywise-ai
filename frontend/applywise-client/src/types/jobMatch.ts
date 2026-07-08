export type JobMatchLanguage = 'en' | 'tr'

export interface JobMatchRequest {
  resumeText: string
  jobDescription: string
  language?: JobMatchLanguage
}

export interface ResumeUploadResponse {
  fileName: string
  sizeBytes: number
  pageCount: number
  extractedText: string
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

export interface JobMatchHistoryDetail extends JobMatchResponse {
  id: string
  resumeText: string
  jobDescription: string
  createdAt: string
}
