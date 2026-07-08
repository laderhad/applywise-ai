import { useMemo } from 'react'
import { useI18n } from '../i18n/useI18n'
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
  const { locale, t } = useI18n()
  const dateFormatter = useMemo(
    () =>
      new Intl.DateTimeFormat(locale, {
        dateStyle: 'medium',
        timeStyle: 'short',
      }),
    [locale],
  )

  return (
    <section className="history-section" aria-labelledby="history-title">
      <div className="section-heading">
        <div>
          <p className="eyebrow">{t('history.eyebrow')}</p>
          <h2 id="history-title">{t('history.title')}</h2>
        </div>
        {!isLoading && !error && items.length > 0 && (
          <span>{t('history.savedCount', { count: items.length })}</span>
        )}
      </div>

      {isLoading && (
        <div className="history-state" role="status">
          {t('history.loading')}
        </div>
      )}

      {error && (
        <div className="history-state history-error" role="alert">
          {error}
        </div>
      )}

      {!isLoading && !error && items.length === 0 && (
        <div className="history-state">
          <strong>{t('history.empty.title')}</strong>
          <p>{t('history.empty.body')}</p>
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
                  <span>{t('history.scoreSuffix')}</span>
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
                      ? t('history.loadingDetails')
                      : t('history.viewingDetails')
                    : t('history.viewDetails')}
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
