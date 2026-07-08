import { useMemo } from 'react'
import { useI18n } from '../i18n/useI18n'
import type { JobMatchHistoryDetail as HistoryDetail } from '../types/jobMatch'
import { MatchResult } from './MatchResult'

interface JobMatchHistoryDetailProps {
  analysis: HistoryDetail
  onClose: () => void
}

export function JobMatchHistoryDetail({
  analysis,
  onClose,
}: JobMatchHistoryDetailProps) {
  const { locale, t } = useI18n()
  const dateFormatter = useMemo(
    () =>
      new Intl.DateTimeFormat(locale, {
        dateStyle: 'long',
        timeStyle: 'short',
      }),
    [locale],
  )

  return (
    <div className="history-detail" id="history-detail">
      <div className="history-detail-toolbar">
        <time dateTime={analysis.createdAt}>
          {t('historyDetail.savedAt', {
            date: dateFormatter.format(new Date(analysis.createdAt)),
          })}
        </time>
        <button type="button" onClick={onClose}>
          {t('historyDetail.close')}
        </button>
      </div>

      <div className="source-grid">
        <details className="source-card">
          <summary>{t('historyDetail.resumeText')}</summary>
          <p>{analysis.resumeText}</p>
        </details>
        <details className="source-card">
          <summary>{t('historyDetail.jobDescription')}</summary>
          <p>{analysis.jobDescription}</p>
        </details>
      </div>

      <MatchResult
        eyebrow={t('historyDetail.savedAnalysis')}
        result={analysis}
        title={t('historyDetail.savedReport')}
      />
    </div>
  )
}
