import { useI18n } from '../i18n/useI18n'
import type { JobMatchResponse } from '../types/jobMatch'

interface MatchResultProps {
  eyebrow?: string
  result: JobMatchResponse
  title?: string
}

interface ListCardProps {
  description: string
  title: string
  items: string[]
  tone: 'positive' | 'warning' | 'neutral'
}

function ListCard({ description, title, items, tone }: ListCardProps) {
  const { t } = useI18n()

  return (
    <section className={`result-card list-card ${tone}`}>
      <div className="result-card-header">
        <div>
          <h3>{title}</h3>
          <p>{description}</p>
        </div>
        <span>{items.length}</span>
      </div>
      {items.length > 0 ? (
        <ul>
          {items.map((item, index) => (
            <li key={`${title}-${index}`}>{item}</li>
          ))}
        </ul>
      ) : (
        <p className="empty-copy">{t('result.empty')}</p>
      )}
    </section>
  )
}

export function MatchResult({
  eyebrow,
  result,
  title,
}: MatchResultProps) {
  const { t } = useI18n()

  return (
    <section className="results" aria-live="polite">
      <div className="result-overview">
        <div
          className="score-card"
          aria-label={t('result.scoreAria', {
            score: result.matchScore,
          })}
        >
          <span>{t('result.scoreLabel')}</span>
          <strong>
            {result.matchScore}
            <small>/100</small>
          </strong>
          <div className="score-track" aria-hidden="true">
            <span style={{ width: `${result.matchScore}%` }} />
          </div>
        </div>

        <div>
          <p className="eyebrow">{eyebrow ?? t('result.eyebrow')}</p>
          <h2>{title ?? t('result.matchReport')}</h2>
          <p className="summary">{result.summary}</p>
        </div>
      </div>

      <div className="result-grid">
        <ListCard
          title={t('result.strong.title')}
          description={t('result.strong.description')}
          items={result.strongPoints}
          tone="positive"
        />
        <ListCard
          title={t('result.weak.title')}
          description={t('result.weak.description')}
          items={result.weakPoints}
          tone="warning"
        />
        <ListCard
          title={t('result.missing.title')}
          description={t('result.missing.description')}
          items={result.missingKeywords}
          tone="neutral"
        />
        <ListCard
          title={t('result.bullets.title')}
          description={t('result.bullets.description')}
          items={result.recommendedBullets}
          tone="positive"
        />
      </div>

      <div className="draft-grid">
        <section className="result-card draft-card">
          <div className="result-card-header">
            <div>
              <h3>{t('result.coverLetter.title')}</h3>
              <p>{t('result.coverLetter.description')}</p>
            </div>
          </div>
          <p>{result.coverLetterDraft}</p>
        </section>
        <section className="result-card draft-card">
          <div className="result-card-header">
            <div>
              <h3>{t('result.linkedin.title')}</h3>
              <p>{t('result.linkedin.description')}</p>
            </div>
          </div>
          <p>{result.linkedinMessageDraft}</p>
        </section>
      </div>
    </section>
  )
}
