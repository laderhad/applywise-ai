import type { JobMatchResponse } from '../types/jobMatch'

interface MatchResultProps {
  result: JobMatchResponse
}

interface ListCardProps {
  title: string
  items: string[]
  tone: 'positive' | 'warning' | 'neutral'
}

function ListCard({ title, items, tone }: ListCardProps) {
  return (
    <section className={`result-card list-card ${tone}`}>
      <h3>{title}</h3>
      {items.length > 0 ? (
        <ul>
          {items.map((item, index) => (
            <li key={`${title}-${index}`}>{item}</li>
          ))}
        </ul>
      ) : (
        <p className="empty-copy">Nothing to highlight.</p>
      )}
    </section>
  )
}

export function MatchResult({ result }: MatchResultProps) {
  return (
    <section className="results" aria-live="polite">
      <div className="result-overview">
        <div
          className="score"
          style={{
            background: `conic-gradient(var(--brand) ${result.matchScore}%, var(--surface-muted) 0)`,
          }}
          aria-label={`Match score: ${result.matchScore} out of 100`}
        >
          <div>
            <strong>{result.matchScore}</strong>
            <span>/ 100</span>
          </div>
        </div>

        <div>
          <p className="eyebrow">Analysis complete</p>
          <h2>Your match at a glance</h2>
          <p className="summary">{result.summary}</p>
        </div>
      </div>

      <div className="result-grid">
        <ListCard
          title="Strong points"
          items={result.strongPoints}
          tone="positive"
        />
        <ListCard
          title="Weak points"
          items={result.weakPoints}
          tone="warning"
        />
        <ListCard
          title="Missing keywords"
          items={result.missingKeywords}
          tone="neutral"
        />
        <ListCard
          title="Recommended CV bullets"
          items={result.recommendedBullets}
          tone="positive"
        />
      </div>

      <div className="draft-grid">
        <section className="result-card draft-card">
          <h3>Cover letter draft</h3>
          <p>{result.coverLetterDraft}</p>
        </section>
        <section className="result-card draft-card">
          <h3>LinkedIn message</h3>
          <p>{result.linkedinMessageDraft}</p>
        </section>
      </div>
    </section>
  )
}
