import type { JobMatchHistoryItem } from '../types/jobMatch'

interface JobMatchHistoryProps {
  error: string | null
  isLoading: boolean
  items: JobMatchHistoryItem[]
}

const dateFormatter = new Intl.DateTimeFormat(undefined, {
  dateStyle: 'medium',
  timeStyle: 'short',
})

export function JobMatchHistory({
  error,
  isLoading,
  items,
}: JobMatchHistoryProps) {
  return (
    <section className="history-section" aria-labelledby="history-title">
      <div className="section-heading">
        <div>
          <p className="eyebrow">Saved locally</p>
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
          <p>Your completed analyses will appear here.</p>
        </div>
      )}

      {!isLoading && !error && items.length > 0 && (
        <ol className="history-list">
          {items.map((item) => (
            <li key={item.id}>
              <article className="history-card">
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
              </article>
            </li>
          ))}
        </ol>
      )}
    </section>
  )
}
