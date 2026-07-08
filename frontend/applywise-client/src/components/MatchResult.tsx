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
        <p className="empty-copy">Nothing to highlight.</p>
      )}
    </section>
  )
}

export function MatchResult({
  eyebrow = 'Analysis complete',
  result,
  title = 'Match report',
}: MatchResultProps) {
  return (
    <section className="results" aria-live="polite">
      <div className="result-overview">
        <div
          className="score-card"
          aria-label={`Match score: ${result.matchScore} out of 100`}
        >
          <span>Match score</span>
          <strong>
            {result.matchScore}
            <small>/100</small>
          </strong>
          <div className="score-track" aria-hidden="true">
            <span style={{ width: `${result.matchScore}%` }} />
          </div>
        </div>

        <div>
          <p className="eyebrow">{eyebrow}</p>
          <h2>{title}</h2>
          <p className="summary">{result.summary}</p>
        </div>
      </div>

      <div className="result-grid">
        <ListCard
          title="Strong points"
          description="Signals already aligned with the role"
          items={result.strongPoints}
          tone="positive"
        />
        <ListCard
          title="Weak points"
          description="Areas that need clearer evidence"
          items={result.weakPoints}
          tone="warning"
        />
        <ListCard
          title="Missing keywords"
          description="Terms worth reflecting if they are accurate"
          items={result.missingKeywords}
          tone="neutral"
        />
        <ListCard
          title="Recommended resume bullets"
          description="Practical edits to make the resume stronger"
          items={result.recommendedBullets}
          tone="positive"
        />
      </div>

      <div className="draft-grid">
        <section className="result-card draft-card">
          <div className="result-card-header">
            <div>
              <h3>Cover letter draft</h3>
              <p>Use as a starting point, then personalize it.</p>
            </div>
          </div>
          <p>{result.coverLetterDraft}</p>
        </section>
        <section className="result-card draft-card">
          <div className="result-card-header">
            <div>
              <h3>LinkedIn message</h3>
              <p>Short outreach draft for recruiters or hiring teams.</p>
            </div>
          </div>
          <p>{result.linkedinMessageDraft}</p>
        </section>
      </div>
    </section>
  )
}
