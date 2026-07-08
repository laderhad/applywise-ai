import {
  useState,
  type ChangeEvent,
  type FormEvent,
} from 'react'
import { uploadResumePdf } from '../services/jobMatchApi'
import type {
  JobMatchRequest,
  ResumeUploadResponse,
} from '../types/jobMatch'

interface JobMatchFormProps {
  isLoading: boolean
  onSubmit: (request: JobMatchRequest) => Promise<void>
}

export function JobMatchForm({
  isLoading,
  onSubmit,
}: JobMatchFormProps) {
  const [resumeText, setResumeText] = useState('')
  const [jobDescription, setJobDescription] = useState('')
  const [resumeUpload, setResumeUpload] =
    useState<ResumeUploadResponse | null>(null)
  const [resumeUploadError, setResumeUploadError] = useState<string | null>(
    null,
  )
  const [isResumeUploading, setIsResumeUploading] = useState(false)

  const canSubmit =
    resumeText.trim().length > 0 &&
    jobDescription.trim().length > 0 &&
    !isLoading &&
    !isResumeUploading

  async function handleResumeFileChange(
    event: ChangeEvent<HTMLInputElement>,
  ) {
    const file = event.target.files?.[0]

    if (!file) {
      return
    }

    setIsResumeUploading(true)
    setResumeUpload(null)
    setResumeUploadError(null)

    try {
      const upload = await uploadResumePdf(file)
      setResumeText(upload.extractedText)
      setResumeUpload(upload)
    } catch (caughtError) {
      setResumeUploadError(
        caughtError instanceof Error
          ? caughtError.message
          : 'The resume PDF could not be uploaded.',
      )
    } finally {
      event.target.value = ''
      setIsResumeUploading(false)
    }
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()

    if (!canSubmit) {
      return
    }

    await onSubmit({
      resumeText: resumeText.trim(),
      jobDescription: jobDescription.trim(),
    })
  }

  return (
    <form className="match-form" onSubmit={handleSubmit}>
      <div className="input-grid">
        <div className="text-field">
          <span className="field-heading">
            <label htmlFor="resume-text">Resume</label>
            <small>Paste text or upload PDF</small>
          </span>

          <div className="resume-upload">
            <label className="file-upload-button">
              <input
                type="file"
                accept="application/pdf,.pdf"
                disabled={isLoading || isResumeUploading}
                onChange={handleResumeFileChange}
              />
              {isResumeUploading ? 'Extracting PDF…' : 'Upload PDF'}
            </label>
            <span>
              {resumeUpload
                ? `${resumeUpload.fileName} · ${resumeUpload.pageCount} page${resumeUpload.pageCount === 1 ? '' : 's'}`
                : 'Max 5 MB, selectable text only'}
            </span>
          </div>

          {resumeUploadError && (
            <p className="resume-upload-error" role="alert">
              {resumeUploadError}
            </p>
          )}

          <textarea
            id="resume-text"
            value={resumeText}
            onChange={(event) => setResumeText(event.target.value)}
            placeholder="Paste your resume text here..."
            rows={14}
            disabled={isLoading || isResumeUploading}
          />
        </div>

        <div className="text-field">
          <span className="field-heading">
            <label htmlFor="job-description">Job description</label>
            <small>Requirements and responsibilities</small>
          </span>
          <textarea
            id="job-description"
            value={jobDescription}
            onChange={(event) => setJobDescription(event.target.value)}
            placeholder="Paste the job description here..."
            rows={14}
            disabled={isLoading}
          />
        </div>
      </div>

      <div className="form-footer">
        <p>Your data stays on your machine and is analyzed by local Ollama.</p>
        <button type="submit" disabled={!canSubmit}>
          {isResumeUploading
            ? 'Extracting resume…'
            : isLoading
              ? 'Analyzing…'
              : 'Analyze match'}
        </button>
      </div>
    </form>
  )
}
