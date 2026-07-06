import { useState } from 'react'
import { JobMatchHistory } from './components/JobMatchHistory'
import { JobMatchForm } from './components/JobMatchForm'
import { MatchResult } from './components/MatchResult'
import { useJobMatchHistory } from './hooks/useJobMatchHistory'
import { useJobMatchHistoryDetail } from './hooks/useJobMatchHistoryDetail'
import { analyzeJobMatch } from './services/jobMatchApi'
import type { JobMatchRequest, JobMatchResponse } from './types/jobMatch'
import './App.css'

function App() {
  const [result, setResult] = useState<JobMatchResponse | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [isLoading, setIsLoading] = useState(false)
  const {
    error: historyError,
    history,
    isLoading: isHistoryLoading,
    refresh: refreshHistory,
  } = useJobMatchHistory()
  const {
    analysis: historyDetail,
    clear: clearHistorySelection,
    error: historyDetailError,
    isLoading: isHistoryDetailLoading,
    select: selectHistoryItem,
    selectedId: selectedHistoryId,
  } = useJobMatchHistoryDetail()

  async function handleAnalyze(request: JobMatchRequest) {
    setIsLoading(true)
    setError(null)
    setResult(null)

    try {
      setResult(await analyzeJobMatch(request))
      void refreshHistory()
    } catch (caughtError) {
      setError(
        caughtError instanceof Error
          ? caughtError.message
          : 'An unexpected error occurred.',
      )
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <main className="app-shell">
      <header className="app-header">
        <a className="brand" href="/" aria-label="ApplyWise AI home">
          <span className="brand-mark">A</span>
          <span>ApplyWise AI</span>
        </a>
        <span className="local-badge">
          <span aria-hidden="true" />
          Local-first · Ollama
        </span>
      </header>

      <section className="hero-copy">
        <p className="eyebrow">A clearer application starts here</p>
        <h1>See how your experience matches the role.</h1>
        <p>
          Compare your resume with a job description and get an honest,
          evidence-based analysis without sending your data to a paid AI API.
        </p>
      </section>

      <JobMatchForm isLoading={isLoading} onSubmit={handleAnalyze} />

      {isLoading && (
        <div className="status-card" role="status">
          <span className="spinner" aria-hidden="true" />
          <div>
            <strong>Analyzing your match</strong>
            <p>The local model is reading both documents.</p>
          </div>
        </div>
      )}

      {error && (
        <div className="status-card error-card" role="alert">
          <strong>Analysis failed</strong>
          <p>{error}</p>
        </div>
      )}

      {result && <MatchResult result={result} />}

      <JobMatchHistory
        detail={historyDetail}
        detailError={historyDetailError}
        error={historyError}
        isDetailLoading={isHistoryDetailLoading}
        isLoading={isHistoryLoading}
        items={history}
        onClearSelection={clearHistorySelection}
        onSelect={selectHistoryItem}
        selectedId={selectedHistoryId}
      />

      <footer>
        ApplyWise AI · Your resume stays local
      </footer>
    </main>
  )
}

export default App
