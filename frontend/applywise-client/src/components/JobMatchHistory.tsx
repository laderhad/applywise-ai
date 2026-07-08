import type {
  JobMatchHistoryDetail,
  JobMatchHistoryItem,
} from '../types/jobMatch'
import { JobMatchHistoryDetail as HistoryDetail } from './JobMatchHistoryDetail'

interface JobMatchHistoryProps {
  detail: JobMatchHistoryDetail | null
  detailError: string | null
  error: string | null
  isDetailLoading: boolean
  isLoading: boolean
  items: JobMatchHistoryItem[]
  onClearSelection: () => void
  onSelect: (id: string) => void
  selectedId: string | null
}

const dateFormatter = new Intl.DateTimeFormat(undefined, {
  dateStyle: 'medium',
  timeStyle: 'short',
})

export function JobMatchHistory({
  detail,
  detailError,
  error,
  isDetailLoading,
  isLoading,
  items,
  onClearSelection,
  onSelect,
  selectedId,
}: JobMatchHistoryProps) {
  return (
    <section className="history-section" aria-labelledby="history-title">
      <div className="section-heading">
        <div>
          <p className="eyebrow">Saved work</p>
          <h2 id="history-title">Recent analyses</h2>
        </div>
        {!isLoading && !error && items.length > 0 && (
          <span>{items.length} saved</span>
        )}
      </div>

      {isLoading && (
        <div className="history-state" role="status">
          Loading recent analyses…
        </div>
      )}

      {error && (
        <div className="history-state history-error" role="alert">
          {error}
        </div>
      )}

      {!isLoading && !error && items.length === 0 && (
        <div className="history-state">
          <strong>No analyses yet</strong>
          <p>Run your first comparison to build a small application history.</p>
        </div>
      )}

      {!isLoading && !error && items.length > 0 && (
        <ol className="history-list">
          {items.map((item) => (
            <li key={item.id}>
              <button
                className={`history-card${selectedId === item.id ? ' selected' : ''}`}
                type="button"
                aria-controls="history-detail"
                aria-expanded={selectedId === item.id}
                onClick={() => onSelect(item.id)}
              >
                <div className="history-score">
                  <strong>{item.matchScore}</strong>
                  <span>/ 100</span>
                </div>
                <div>
                  <p>{item.summary}</p>
                  <time dateTime={item.createdAt}>
                    {dateFormatter.format(new Date(item.createdAt))}
                  </time>
                </div>
                <span className="history-action">
                  {selectedId === item.id
                    ? isDetailLoading
                      ? 'Loading…'
                      : 'Viewing details'
                    : 'View details'}
                </span>
              </button>
            </li>
          ))}
        </ol>
      )}

      {detailError && (
        <div className="history-state history-error" role="alert">
          {detailError}
        </div>
      )}

      {detail && (
        <HistoryDetail
          analysis={detail}
          onClose={onClearSelection}
        />
      )}
    </section>
  )
}
