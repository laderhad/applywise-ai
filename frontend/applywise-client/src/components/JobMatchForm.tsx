import {
  useState,
  type ChangeEvent,
  type FormEvent,
} from 'react'
import { useI18n } from '../i18n/useI18n'
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
  const { t } = useI18n()
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

    const validationError = getResumePdfValidationError(file, {
      emptyFile: t('validation.resumeFile.empty'),
      invalidType: t('validation.resumeFile.invalidType'),
      tooLarge: t('validation.resumeFile.tooLarge', {
        maxSize: maxResumePdfSizeLabel,
      }),
    })

    if (validationError) {
      setResumeUploadError(validationError)
      event.target.value = ''

      return
    }

    setIsResumeUploading(true)

    try {
      const upload = await uploadResumePdf(
        file,
        t('api.resumeUploadFailed'),
      )
      setResumeText(upload.extractedText)
      setResumeUpload(upload)
    } catch (caughtError) {
      setResumeUploadError(
        caughtError instanceof Error
          ? caughtError.message
          : t('api.resumeUploadFailed'),
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
          <p className="eyebrow">{t('form.eyebrow')}</p>
          <h2>{t('form.title')}</h2>
        </div>
        <span>{t('form.privacyNote')}</span>
      </div>

      <div className="input-grid">
        <div className="text-field">
          <span className="field-heading">
            <label htmlFor="resume-text">{t('form.resume.label')}</label>
            <small>{t('form.resume.helper')}</small>
          </span>

          <div className="resume-upload">
            <div>
              <strong>{t('form.resumeUpload.title')}</strong>
              <span>
                {resumeUpload
                  ? t('form.resumeUpload.extracted', {
                      fileName: resumeUpload.fileName,
                      pageCount: resumeUpload.pageCount,
                      pageLabel: t(
                        resumeUpload.pageCount === 1
                          ? 'common.page'
                          : 'common.pages',
                      ),
                    })
                  : t('form.resumeUpload.max', {
                      maxSize: maxResumePdfSizeLabel,
                    })}
              </span>
            </div>
            <label className="file-upload-button">
              <input
                type="file"
                accept="application/pdf,.pdf"
                disabled={isLoading || isResumeUploading}
                onChange={handleResumeFileChange}
              />
              {isResumeUploading
                ? t('form.resumeUpload.extracting')
                : t('form.resumeUpload.choose')}
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
            placeholder={t('form.resume.placeholder')}
            rows={14}
            disabled={isLoading || isResumeUploading}
          />
        </div>

        <div className="text-field">
          <span className="field-heading">
            <label htmlFor="job-description">{t('form.job.label')}</label>
            <small>{t('form.job.helper')}</small>
          </span>
          <textarea
            id="job-description"
            value={jobDescription}
            onChange={(event) => setJobDescription(event.target.value)}
            placeholder={t('form.job.placeholder')}
            rows={14}
            disabled={isLoading}
          />
        </div>
      </div>

      <div className="form-footer">
        <p>{t('form.footerNote')}</p>
        <button type="submit" disabled={!canSubmit}>
          {isResumeUploading
            ? t('form.button.extractingResume')
            : isLoading
              ? t('form.button.analyzing')
              : t('form.button.runAnalysis')}
        </button>
      </div>
    </form>
  )
}
