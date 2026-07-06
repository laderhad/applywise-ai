import type { JobMatchHistoryDetail as HistoryDetail } from '../types/jobMatch'
import { MatchResult } from './MatchResult'

interface JobMatchHistoryDetailProps {
  analysis: HistoryDetail
  onClose: () => void
}

const dateFormatter = new Intl.DateTimeFormat(undefined, {
  dateStyle: 'long',
  timeStyle: 'short',
})

export function JobMatchHistoryDetail({
  analysis,
  onClose,
}: JobMatchHistoryDetailProps) {
  return (
    <div className="history-detail" id="history-detail">
      <div className="history-detail-toolbar">
        <time dateTime={analysis.createdAt}>
          Saved {dateFormatter.format(new Date(analysis.createdAt))}
        </time>
        <button type="button" onClick={onClose}>
          Close details
        </button>
      </div>

      <div className="source-grid">
        <details className="source-card">
          <summary>Resume text</summary>
          <p>{analysis.resumeText}</p>
        </details>
        <details className="source-card">
          <summary>Job description</summary>
          <p>{analysis.jobDescription}</p>
        </details>
      </div>

      <MatchResult
        eyebrow="Saved analysis"
        result={analysis}
        title="Your previous match at a glance"
      />
    </div>
  )
}
