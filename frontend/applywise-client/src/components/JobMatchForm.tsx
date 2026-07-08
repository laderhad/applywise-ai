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
import {
  getResumePdfValidationError,
  maxResumePdfSizeLabel,
} from '../utils/resumePdfValidation'

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

    setResumeUpload(null)
    setResumeUploadError(null)

    const validationError = getResumePdfValidationError(file)

    if (validationError) {
      setResumeUploadError(validationError)
      event.target.value = ''

      return
    }

    setIsResumeUploading(true)

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
      <div className="form-heading">
        <div>
          <p className="eyebrow">New analysis</p>
          <h2>Start with your resume and target role</h2>
        </div>
        <span>Nothing is submitted until you run the analysis.</span>
      </div>

      <div className="input-grid">
        <div className="text-field">
          <span className="field-heading">
            <label htmlFor="resume-text">Resume</label>
            <small>PDF upload or plain text</small>
          </span>

          <div className="resume-upload">
            <div>
              <strong>Resume PDF</strong>
              <span>
                {resumeUpload
                  ? `${resumeUpload.fileName} · ${resumeUpload.pageCount} page${resumeUpload.pageCount === 1 ? '' : 's'} extracted`
                  : `Max ${maxResumePdfSizeLabel}, selectable text only`}
              </span>
            </div>
            <label className="file-upload-button">
              <input
                type="file"
                accept="application/pdf,.pdf"
                disabled={isLoading || isResumeUploading}
                onChange={handleResumeFileChange}
              />
              {isResumeUploading ? 'Extracting…' : 'Choose PDF'}
            </label>
          </div>

          {resumeUploadError && (
            <p className="resume-upload-error" role="alert">
              {resumeUploadError}
            </p>
          )}

          <textarea
            id="resume-text"
            value={resumeText}
            onChange={(event) => {
              setResumeText(event.target.value)
              setResumeUpload(null)
            }}
            placeholder="Paste your resume text here, or upload a PDF above..."
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
            placeholder="Paste the role description, requirements, and responsibilities here..."
            rows={14}
            disabled={isLoading}
          />
        </div>
      </div>

      <div className="form-footer">
        <p>You can edit extracted text before running the comparison.</p>
        <button type="submit" disabled={!canSubmit}>
          {isResumeUploading
            ? 'Extracting resume…'
            : isLoading
              ? 'Analyzing…'
              : 'Run analysis'}
        </button>
      </div>
    </form>
  )
}
