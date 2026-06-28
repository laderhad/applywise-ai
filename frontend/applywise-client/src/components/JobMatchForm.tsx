import { useState, type FormEvent } from 'react'
import type { JobMatchRequest } from '../types/jobMatch'

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

  const canSubmit =
    resumeText.trim().length > 0 &&
    jobDescription.trim().length > 0 &&
    !isLoading

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
        <label className="text-field">
          <span className="field-heading">
            <span>Resume</span>
            <small>Plain text</small>
          </span>
          <textarea
            value={resumeText}
            onChange={(event) => setResumeText(event.target.value)}
            placeholder="Paste your resume text here..."
            rows={14}
            disabled={isLoading}
          />
        </label>

        <label className="text-field">
          <span className="field-heading">
            <span>Job description</span>
            <small>Requirements and responsibilities</small>
          </span>
          <textarea
            value={jobDescription}
            onChange={(event) => setJobDescription(event.target.value)}
            placeholder="Paste the job description here..."
            rows={14}
            disabled={isLoading}
          />
        </label>
      </div>

      <div className="form-footer">
        <p>Your data stays on your machine and is analyzed by local Ollama.</p>
        <button type="submit" disabled={!canSubmit}>
          {isLoading ? 'Analyzing…' : 'Analyze match'}
        </button>
      </div>
    </form>
  )
}
